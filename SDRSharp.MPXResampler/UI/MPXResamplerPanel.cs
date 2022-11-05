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
            this._player = new AudioPlayer(control, this._audioProcessor);
            this.AudioDeviceGet();
            this.audioDeviceComboBox_SelectedIndexChanged(null, null);
            this.volumeTrackBar.Value = Utils.GetIntSetting("MPXResamplerGain", 50);
            this.volumeTrackBar_Scroll(null, null);
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
            Utils.SaveSetting("MPXResamplerDevice", this.audioDeviceComboBox.SelectedItem);
            Utils.SaveSetting("MPXResamplerGain", this.volumeTrackBar.Value);
        }

        private void EnableControls()
        {
            bool isPlaying = this._control.IsPlaying;
            this.auxAudioEnableCheckBox.Enabled = isPlaying;
            this.audioDeviceComboBox.Enabled = !this._playerIsStarted;
        }

        public void StartAux()
        {
            if (this._playerIsStarted)
            {
                return;
            }
            this._player.Start();
            this._playerIsStarted = true;
            _control.AudioIsMuted = true;
            this.EnableControls();
        }

        public void StopAux()
        {
            if (!this._playerIsStarted)
            {
                return;
            }
            this._player.Stop();
            this._playerIsStarted = false;
            _control.AudioIsMuted = false;
            this.EnableControls();
        }

        private void audioDeviceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            AudioDevice audioDevice = (AudioDevice)this.audioDeviceComboBox.SelectedItem;
            this._player.DeviceIndex = audioDevice.Index;
        }

        private void AudioDeviceGet()
        {
            int num = 0;
            int num2 = -1;
            List<AudioDevice> devices = AudioDevice.GetDevices(DeviceDirection.Output);
            string stringSetting = Utils.GetStringSetting("MPXResamplerDevice", string.Empty);
            for (int i = 0; i < devices.Count; i++)
            {
                this.audioDeviceComboBox.Items.Add(devices[i]);
                if (devices[i].IsDefault)
                {
                    num = i;
                }
                if (devices[i].ToString() == stringSetting)
                {
                    num2 = i;
                }
            }
            if (this.audioDeviceComboBox.Items.Count > 0)
            {
                this.audioDeviceComboBox.SelectedIndex = ((num2 >= 0) ? num2 : num);
            }
        }

        private void volumeTrackBar_Scroll(object sender, EventArgs e)
        {
            this._player.Gain = (float)Math.Pow((double)this.volumeTrackBar.Value, 3.0);
        }

        private void displayTimer_Tick(object sender, EventArgs e)
        {
            this.disbalanceLabel.Text = string.Format("Lost buffers {0:f0}", this._player.LostBuffers);
            this.bufferProgressBar.Value = this._player.BufferSize;

            //Show user what internal sapmle rate we have
            this.SampleRateLbl.Text = String.Format("Sample rate: {0} kHz", this._player.InternalSampleRate);
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
        private AudioPlayer _player;
        private bool _playerIsStarted;

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            //if (this._player != null)
                this._player.needsConfigure = true;

            // Tell audio player what resample method we choose
            this._player.isDecimationlerClass = radioButton2.Checked;
        }
    }
}
