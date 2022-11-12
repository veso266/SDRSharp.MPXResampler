using System;
using System.Diagnostics;
using SDRSharp.Common;
using SDRSharp.Radio;
using SDRSharp.Radio.PortAudio;

namespace SDRSharp.MPXResampler
{
    public class AudioPlayer
    {

        public int DeviceIndex
        {
            set
            {
                this._deviceIndex = value;
            }
        }

        public float Gain
        {
            set
            {
                this._gain = value;
            }
        }

        public int LostBuffers
        {
            get
            {
                return this._lostBuffers;
            }
        }

        public int BufferSize
        {
            get
            {
                return this._bufferSize;
            }
        }

        public int InternalSampleRate
        {
            get
            {
                return (int)(this._sampleRate) / 1000;
            }
        }

        public bool isDecimationlerClass
        {
            set
            {
                this._isDecimationlerClass = value;
            }
        }

        public bool needConfigure
        {
            get
            {
                if (_audioProcessor != null)
                    return _audioProcessor.needConfigure;
                return false;
            }
            set
            {
                if (_audioProcessor != null)
                    _audioProcessor.needConfigure = value;
            }
        }

        private const float OutputLatency = 0.1f;

        private FloatFifoStream _audioStream;
        private AudioProcessor _audioProcessor;
        private WavePlayer _wavePlayer;
        private int _deviceIndex;
        private double _sampleRateOut;
        private int _outputLength;
        private float _gain;
        private int _lostBuffers;
        private int _bufferSize;
        private int _maxBufferSize;
        private double _sampleRate;
        private int _inputLength;
        private bool _isDecimationlerClass;

        //Resapmler thinggy
        private FloatDecimator _channelDecimator;
        private Resampler _resampler;

        //Audio in
        private UnsafeBuffer _InputBuffer;
        private unsafe float* _InputBufferPtr;

        //Temp Audio buffer
        private UnsafeBuffer _tempAudioBuffer;
        private unsafe float* _tempAudioBufferPtr;

        //SDRSharpControl
        ISharpControl control;
        private int _audioDecimationFactor;

        public unsafe AudioPlayer(ISharpControl control, AudioProcessor audioProcessor)
        {
            this.control = control;
            this._audioProcessor = audioProcessor;
            this._audioProcessor.AudioReady += this.AudioSamplesIn;
            this._audioProcessor.Enabled = false;
        }

        public void Start()
        {
            this._lostBuffers = 0;
            this._audioStream = new FloatFifoStream(BlockMode.None);
            this._audioProcessor.Enabled = true;
        }

        public unsafe void Stop()
        {
            this._audioProcessor.Enabled = false;
            if (this._wavePlayer != null)
            {
                this._wavePlayer.Dispose();
                this._wavePlayer = null;
            }
            if (this._audioStream != null)
            {
                this._audioStream.Close();
                this._audioStream = null;
            }
            if (this._resampler != null)
            {
                this._resampler = null;
                this._InputBuffer.Dispose();
                this._InputBuffer = null;
                this._InputBufferPtr = null;
            }
            if (this._channelDecimator != null)
            {
                this._channelDecimator = null;
                this._tempAudioBuffer.Dispose();
                this._tempAudioBuffer = null;
                this._tempAudioBufferPtr = null;
            }
            this._sampleRate = 0.0;
            this._sampleRateOut = 0.0;
            this._bufferSize = 0;
        }

        //This method is called whenever something in the plugin needs change
        public void reConfigure()
        {
            
        }

        /// <summary>
        /// This method is called whenever SDR# gives samples to it
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="samplerate"></param>
        /// <param name="length"></param>
        private unsafe void AudioSamplesIn(float* buffer, double samplerate, int length)
        {
            if (this._wavePlayer == null || samplerate != this._sampleRate)
            {
                this._sampleRate = samplerate;
                this._sampleRateOut = this.control.AudioSampleRate;
                this._maxBufferSize = (int)this._sampleRate;

                //We multiplay our Length by 0.1 so audio device doesn't run out of samples (hear silence when that happens)
                this._inputLength = (int)(this._sampleRate * OutputLatency);
                this._outputLength = (int)(this._sampleRateOut * OutputLatency);

                #region Initialize buffers
                //Input buffer
                if (this._InputBuffer == null || this._InputBuffer.Length != length)
                {
                    this._InputBuffer = UnsafeBuffer.Create(this._inputLength, sizeof(float));
                    this._InputBufferPtr = (float*)this._InputBuffer;
                }
                #endregion 

                //Configure resampler
                //this.needConfigure = true;

                #region Init Audio player
                if (this._wavePlayer != null)
                {
                    this._wavePlayer.Dispose();
                    this._wavePlayer = null;
                }
                this._wavePlayer = new WavePlayer(this._deviceIndex, this._sampleRateOut, this._outputLength, new AudioBufferNeededDelegate(this.PlayerProcess));
            }
            if (this._audioStream.Length >= this._maxBufferSize)
            {
                this._lostBuffers++;
                return;
            }
            #endregion

            this._audioStream.Write(buffer, length); //Send samples to a intermediate FIFO buffer, where PlayerProcess will later take them from 
        }

        /// <summary>
        /// This method is called whenever soundcard wants samples (you should feed them to it, else, you hear nothing)
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        private unsafe void PlayerProcess(float* buffer, int length)
        {
            /*
            if (this._audioStream == null)
            {
                return;
            }
            this._bufferSize = (int)Math.Min((float)this._audioStream.Length / (float)this._maxBufferSize * 100f, 100f);
            if (this._audioStream.Length < this._inputLength)
            {
                this._lostBuffers++;
                for (int i = 0; i < length; i++)
                {
                    buffer[i] = 0f;
                }
                return;
            }
            */

            //If audio players needs to reconfigure itself
            if (needConfigure)
            {
                //Configure resampler
                if (!this._isDecimationlerClass) //If we are using resampling class
                    this._resampler = new Resampler(this._sampleRate, this._sampleRateOut);
                else //if we want to be smart and reinvent the wheel
                {
                    int DecimationRatio = (int)Math.Round(Math.Sqrt((int)this._sampleRate / (int)this._sampleRateOut)); //-> (192000 / 48000) = sqrt(4) = 2
                    if (this._sampleRate <= this._sampleRateOut)
                        DecimationRatio = 0;

                    this._audioDecimationFactor = (int)Math.Pow(2.0, DecimationRatio); //4 = 2^2

                    if (_channelDecimator == null || _audioDecimationFactor != _channelDecimator.DecimationRatio)
                    {
                        _channelDecimator = new FloatDecimator(_audioDecimationFactor);
                    }
                }

                needConfigure = false;
            }

            //Read the audiostream (samples) into InputBufferPtr (InputBufferPtr holds our audio direcly from SDR#)
            //this._audioStream.Read(this._InputBufferPtr, this._inputLength);

            //Resample to 48khz
            if (!this._isDecimationlerClass && this._resampler != null) //If we are using resampling class
                this._resampler.Process(this._InputBufferPtr, buffer, this._inputLength);
            else if (this._isDecimationlerClass && this._channelDecimator != null) //if we want to be smart and reinvent the wheel
            {
                #region Prepare buffer

                if (_tempAudioBufferPtr == null || _tempAudioBuffer.Length != this._inputLength)
                {
                    _tempAudioBuffer = UnsafeBuffer.Create(this._inputLength, sizeof(float));
                    _tempAudioBufferPtr = (float*)_tempAudioBuffer;
                }

                #endregion

                #region Fill temp buffer
                Utils.Memcpy(this._tempAudioBufferPtr, this._InputBufferPtr, this._inputLength * sizeof(float));
                _channelDecimator.Process(this._tempAudioBufferPtr, this._inputLength);
                #endregion

                #region Fill output buffer
                Utils.Memcpy(buffer, this._tempAudioBufferPtr, this._outputLength * sizeof(float));
                #endregion
            }
            else //I output nothing because I am broken
            {
                for (int i = 0; i < length; i++)
                {
                    buffer[i] = 0f;
                }
            }

            //Boost output 
            this.BoostOutput(buffer, this._outputLength);

        }

        private unsafe void BoostOutput(float* buffer, int length)
        {
            for (var i = length - 1; i >= 0; i--)
            {
                var sample = buffer[i] * this._gain * 2;
                buffer[i * 2] = sample;             //Left Channel
                buffer[i * 2 + 1] = sample;         //Right Channel
            }
        }
    }
}