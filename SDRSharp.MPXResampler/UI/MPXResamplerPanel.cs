using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SDRSharp.Common;
using SDRSharp.Radio;
using SDRSharp.Radio.PortAudio;

namespace SDRSharp.MPXResampler
{
    public partial class MPXResamplerPanel : UserControl
    {
        public MPXResamplerPanel(AudioProcessor audioProcessor, ISharpControl control)
        {
            this.InitializeComponent();
            this._audioProcessor = audioProcessor;
            this._control = control;
            this._control.PropertyChanged += this.PropertyChangedHandler;

            this.auxAudioEnableCheckBox.Checked = Utils.GetBooleanSetting("MPXOutEnable");
            this.EnableControls();
        }

        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName;
            if (!(propertyName == "StartRadio"))
            {
                if (!(propertyName == "StopRadio"))
                {
                    return;
                }
                this.EnableControls();
                this.StopAux();
            }
            else
            {
                this.EnableControls();
                if (this.auxAudioEnableCheckBox.Checked)
                {
                    this.StartAux();
                    return;
                }
            }
        }

        public void StoreSettings()
        {
            Utils.SaveSetting("MPXResamplerEnable", this.auxAudioEnableCheckBox.Checked);
        }

        private void EnableControls()
        {
            bool isPlaying = this._control.IsPlaying;
            this.auxAudioEnableCheckBox.Enabled = isPlaying;
        }

        public void StartAux()
        {
            if (this._playerIsStarted)
            {
                return;
            }
            this._playerIsStarted = true;
            this._audioProcessor.Enabled = this._playerIsStarted;
            //_control.AudioIsMuted = true;
            this.EnableControls();
        }

        public void StopAux()
        {
            if (!this._playerIsStarted)
            {
                return;
            }
            this._playerIsStarted = false;
            //_control.AudioIsMuted = false;
            this._audioProcessor.Enabled = this._playerIsStarted;
            this.EnableControls();
        }

        private void displayTimer_Tick(object sender, EventArgs e)
        {
            //Show user what internal sapmle rate we have
            this.SampleRateLbl.Text = String.Format("Sample rate: {0} kHz", this._audioProcessor.InternalSampleRate);
        }

        private void auxAudioEnableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!this._control.IsPlaying)
            {
                return;
            }
            if (this.auxAudioEnableCheckBox.Checked)
            {
                this.StartAux();
                return;
            }
            if (!this.auxAudioEnableCheckBox.Checked)
            {
                this.StopAux();
            }
        }

        private AudioProcessor _audioProcessor;
        private ISharpControl _control;
        private bool _playerIsStarted;

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (this._audioProcessor.Enabled)
                this._audioProcessor.needConfigure = true;

            // Tell audio player what resample method we choose
            this._audioProcessor.isDecimationlerClass = radioButton2.Checked;
        }
    }
}
