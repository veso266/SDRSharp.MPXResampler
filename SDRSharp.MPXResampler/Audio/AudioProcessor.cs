using SDRSharp.Common;
using SDRSharp.Radio;
using System;

namespace SDRSharp.MPXResampler
{
    public unsafe class AudioProcessor : IRealProcessor, IStreamProcessor, IBaseProcessor
    {
        private bool _isDecimationlerClass;
        private double _sampleRate;
        private double _sampleRateOut;
        private bool _enabled;
        private bool _needConfigure = true; //When plugins feals like configuring itself, this will be used

        /// <summary>
        /// Audio decimation factor or Decimation for short
        /// is a value that tells the computer how many samples should it take when resampling down
        /// If we are resampling from 192khz to 48khz, we take 192000/48000 = 4 samples
        /// </summary>
        private int _audioDecimationFactor;

        //Resapmler thinggy
        private FloatDecimator _channelDecimator;
        private Resampler _resampler;

        //Audio in
        private UnsafeBuffer _InputBuffer;
        private unsafe float* _InputBufferPtr;

        //Temp Audio buffer: stores samples before they get Decimated
        private UnsafeBuffer _tempAudioBuffer;
        private unsafe float* _tempAudioBufferPtr;

        public AudioProcessor(ISharpControl control)
        {

            _sampleRateOut = control.AudioSampleRate;
        }

        public double SampleRate
        {
            get { return _sampleRate; }
            set
            {
                _sampleRate = value;
                _needConfigure = true;
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
                return _needConfigure;
            }
            set
            {
                _needConfigure = value;
            }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        private void Configure()
        {
            //Configure resampler
            if (!this._isDecimationlerClass) //If we are using resampling class
                this._resampler = new Resampler(this._sampleRate, this._sampleRateOut);
            else //if we want to be smart and reinvent the wheel
            {
                int DecimationRatio = (int)Math.Round(Math.Sqrt((int)this._sampleRate / (int)this._sampleRateOut)); //-> (192000 / 48000) = sqrt(4) = 2
                if (this._sampleRate <= this._sampleRateOut)
                    DecimationRatio = 0;

                _audioDecimationFactor = (int)Math.Pow(2.0, DecimationRatio); //4 = 2^2

                if (_channelDecimator == null || _audioDecimationFactor != _channelDecimator.DecimationRatio)
                {
                    _channelDecimator = new FloatDecimator(_audioDecimationFactor);
                }
            }
        }
        /// <summary>
        /// This method is called whenever SDR# gives samples to it, it will only process float samples
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="samplerate"></param>
        /// <param name="length"></param>
        public void Process(float* buffer, int length)
        {
            if (_needConfigure)
            {
                Configure();
                _needConfigure = false; //If audio players needs to reconfigure itself
            }

            #region Initialize buffers
            //Input buffer
            if (this._InputBuffer == null || this._InputBuffer.Length != length)
            {
                this._InputBuffer = UnsafeBuffer.Create(length, sizeof(float));
                this._InputBufferPtr = (float*)this._InputBuffer;
            }
            #endregion

            #region Fill input buffer
            Utils.Memcpy(this._InputBufferPtr, buffer, length * sizeof(float));
            #endregion

            //Resample to 48khz
            int resampledLength = 0;
            if (!this._isDecimationlerClass && this._resampler != null) //If we are using resampling class
                resampledLength = this._resampler.Process(this._InputBufferPtr, buffer, resampledLength);
            else if (this._isDecimationlerClass && this._channelDecimator != null) //if we want to be smart and reinvent the wheel
            {
                #region Prepare buffer

                if (_tempAudioBufferPtr == null || _tempAudioBuffer.Length != length)
                {
                    _tempAudioBuffer = UnsafeBuffer.Create(length, sizeof(float));
                    _tempAudioBufferPtr = (float*)_tempAudioBuffer;
                }

                #endregion

                //Fill temp buffer
                Utils.Memcpy(this._tempAudioBufferPtr, this._InputBufferPtr, length * sizeof(float));

                //Decimate the buffer
                resampledLength = _channelDecimator.Process(this._tempAudioBufferPtr, resampledLength);


                #region Fill output buffer
                //Utils.Memcpy(buffer, this._tempAudioBufferPtr, length * sizeof(float));

                for (var i = 0; i < resampledLength; i++)
                {
                    var sample = this._tempAudioBufferPtr[i];
                    buffer[i] = sample;
                    buffer[i + 1] = sample;
                }

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
            BoostOutput(buffer, length);
        }
        /// <summary>
        /// This method will just boost the amplitude * 2, just so we can hear our plugin doing something
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        private unsafe void BoostOutput(float* buffer, int length)
        {
            for (var i = 0; i < length; i++)
            {
                var sample = buffer[i] * 2;
                buffer[i] = sample;
            }
        }
    }
}