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


        private const float OutputLatency = 0.1f;
        private FloatFifoStream _audioStream;
        private AudioProcessor _audioProcessor;
        private WavePlayer _wavePlayer;
        private Resampler _InputResampler;
        private Resampler _resampler;
        private UnsafeBuffer _InputBuffer;
        private unsafe float* _InputBufferPtr;
        private int _deviceIndex;
        private double _InputSampleRate;
        private double _sampleRateOut;
        private int _outputLength;
        private float _gain;
        private int _lostBuffers;
        private int _bufferSize;
        private int _maxBufferSize;
        private double _sampleRate;
        private int _inputLength;
        private int _ResampledInputLength;

        //Resample Audio in
        private UnsafeBuffer _ResampledInputBuffer;
        private unsafe float* _ResampledInputBufferPtr;

        //Audio out
        private UnsafeBuffer _AudioOutBuffer;
        private unsafe float* _AudioOutBufferPtr;


        //SDRSharpControl
        ISharpControl control;

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
                this._InputResampler = null;
                this._resampler = null;
                this._AudioOutBuffer.Dispose();
                this._AudioOutBuffer = null;
                this._AudioOutBufferPtr = null;
                this._InputBuffer.Dispose();
                this._InputBuffer = null;
                this._InputBufferPtr = null;
                                
                this._ResampledInputBuffer.Dispose();
                this._ResampledInputBuffer = null;
                this._ResampledInputBuffer = null;
            }
            this._sampleRate = 0.0;
            this._InputSampleRate = 0.0;
            this._sampleRateOut = 0.0;
            this._bufferSize = 0;
        }

        private unsafe void AudioSamplesIn(float* buffer, double samplerate, int length)
        {
            if (this._wavePlayer == null || samplerate != this._sampleRate)
            {
                if (samplerate <= 0.0 || samplerate > 400000.0)
                {
                    return;
                }
                this._sampleRate = samplerate;
                this._InputSampleRate = 192000;
                this._sampleRateOut = this.control.AudioSampleRate;
                this._maxBufferSize = (int)this._sampleRate;


                //We multiplay our Length by 0.1 so audio device doesn't run out of samples (hear silence when that happens)
                this._inputLength = (int)(this._sampleRate * 0.1);
                this._ResampledInputLength = (int)(this._InputSampleRate * 0.1);
                this._outputLength = (int)(this._sampleRateOut * 0.1);



                this._InputResampler = new Resampler(this._sampleRate, this._InputSampleRate);
                this._resampler = new Resampler((this._InputSampleRate / 1e3) * 4, this._InputSampleRate / 1e3); //Configure resampler for Interpolation: 192, , Decimation: 768


                //Input buffer
                this._InputBuffer = UnsafeBuffer.Create(this._inputLength, sizeof(float));
                this._InputBufferPtr = (float*)this._InputBuffer;

                //Resampled Input buffer (this buffer is always 192khz)
                this._ResampledInputBuffer = UnsafeBuffer.Create(this._inputLength, sizeof(float));
                this._ResampledInputBufferPtr = (float*)this._ResampledInputBuffer;


                //Output buffer
                _AudioOutBuffer = UnsafeBuffer.Create(this._outputLength, sizeof(float));
                this._AudioOutBufferPtr = (float*)this._AudioOutBuffer;

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

            this._audioStream.Write(buffer, length); //Send stream to audio device




        }

        private unsafe void PlayerProcess(float* buffer, int length)
        {
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

            //Read the audiostream into InputBufferPtr (InputBufferPtr holds our audio direcly from SDR#)
            this._audioStream.Read(this._InputBufferPtr, this._inputLength);

            //Resample the input just in case its not 192khz to 192khz
            this._InputResampler.Process(this._InputBufferPtr, this._ResampledInputBufferPtr, this._inputLength);

            //Connect the audio to output direcly
            //AudioWire(this._ResampledInputBufferPtr, buffer, this._outputLength);

            //Resample the audio to from 192khz to 48Khz before feeding it into an audio device
            this._resampler.Process(this._ResampledInputBufferPtr, buffer, this._ResampledInputLength);


            this.ScaleOutputBufferToStereo(buffer, this._outputLength);

        }

        private unsafe void AudioWire(float* AudioInput, float* AudioOutput, int length) //This should take Audio input and feed it into a buffer that goes to sound card
        {
            for (int i = 0; i < length; i++)
            {
                *(AudioOutput++) = AudioInput[i] * this._gain;
            }
        }

        private unsafe void ScaleOutputBufferToStereo(float* buffer, int length)
        {
            int start = (length - 1) * 2;
            for (int i = length - 1; i >= 0; i--)
            {
                buffer[start] = (buffer[start + 1] = buffer[i] * this._gain);
                start -= 2;
            }
        }
    }
}
