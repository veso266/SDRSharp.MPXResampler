using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SDRSharp.MPXResampler
{
    partial class MPXResamplerPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.audioDeviceComboBox = new ComboBox();
            this.label1 = new Label();
            this.volumeTrackBar = new TrackBar();
            this.label2 = new Label();
            this.disbalanceLabel = new Label();
            this.displayTimer = new Timer(this.components);
            this.bufferProgressBar = new ProgressBar();
            this.label3 = new Label();
            this.groupBox1 = new GroupBox();
            this.auxAudioEnableCheckBox = new CheckBox();
            ((ISupportInitialize)this.volumeTrackBar).BeginInit();
            this.groupBox1.SuspendLayout();
            base.SuspendLayout();
            this.audioDeviceComboBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            this.audioDeviceComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.audioDeviceComboBox.DropDownWidth = 330;
            this.audioDeviceComboBox.FormattingEnabled = true;
            this.audioDeviceComboBox.Location = new Point(5, 39);
            this.audioDeviceComboBox.Name = "audioDeviceComboBox";
            this.audioDeviceComboBox.Size = new Size(188, 21);
            this.audioDeviceComboBox.TabIndex = 0;
            this.audioDeviceComboBox.SelectedIndexChanged += this.audioDeviceComboBox_SelectedIndexChanged;
            this.label1.AutoSize = true;
            this.label1.Location = new Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new Size(69, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Audio device";
            this.volumeTrackBar.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            this.volumeTrackBar.AutoSize = false;
            this.volumeTrackBar.Location = new Point(5, 85);
            this.volumeTrackBar.Maximum = 100;
            this.volumeTrackBar.Name = "volumeTrackBar";
            this.volumeTrackBar.Size = new Size(188, 35);
            this.volumeTrackBar.TabIndex = 5;
            this.volumeTrackBar.TickFrequency = 10;
            this.volumeTrackBar.Scroll += this.volumeTrackBar_Scroll;
            this.label2.AutoSize = true;
            this.label2.Location = new Point(6, 69);
            this.label2.Name = "label2";
            this.label2.Size = new Size(64, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Output level";
            this.disbalanceLabel.AutoSize = true;
            this.disbalanceLabel.Location = new Point(124, 132);
            this.disbalanceLabel.Name = "disbalanceLabel";
            this.disbalanceLabel.Size = new Size(71, 13);
            this.disbalanceLabel.TabIndex = 7;
            this.disbalanceLabel.Text = "Lost buffers 0";
            this.displayTimer.Enabled = true;
            this.displayTimer.Interval = 500;
            this.displayTimer.Tick += this.displayTimer_Tick;
            this.bufferProgressBar.Location = new Point(68, 127);
            this.bufferProgressBar.Name = "bufferProgressBar";
            this.bufferProgressBar.Size = new Size(50, 23);
            this.bufferProgressBar.TabIndex = 8;
            this.label3.AutoSize = true;
            this.label3.Location = new Point(6, 132);
            this.label3.Name = "label3";
            this.label3.Size = new Size(56, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Use buffer";
            this.groupBox1.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            this.groupBox1.Controls.Add(this.auxAudioEnableCheckBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.audioDeviceComboBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.volumeTrackBar);
            this.groupBox1.Controls.Add(this.bufferProgressBar);
            this.groupBox1.Controls.Add(this.disbalanceLabel);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new Point(4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(200, 160);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.auxAudioEnableCheckBox.AutoSize = true;
            this.auxAudioEnableCheckBox.Location = new Point(9, 0);
            this.auxAudioEnableCheckBox.Name = "auxAudioEnableCheckBox";
            this.auxAudioEnableCheckBox.Size = new Size(118, 17);
            this.auxAudioEnableCheckBox.TabIndex = 3;
            this.auxAudioEnableCheckBox.Text = "Enable";
            this.auxAudioEnableCheckBox.UseVisualStyleBackColor = true;
            this.auxAudioEnableCheckBox.CheckedChanged += this.auxAudioEnableCheckBox_CheckedChanged;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.Controls.Add(this.groupBox1);
            base.Name = "DSDPanel";
            base.Size = new Size(204, 188);
            ((ISupportInitialize)this.volumeTrackBar).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            base.ResumeLayout(false);
        }
        private ComboBox audioDeviceComboBox;
        private Label label1;
        private TrackBar volumeTrackBar;
        private Label label2;
        private Label disbalanceLabel;
        private Timer displayTimer;
        private ProgressBar bufferProgressBar;
        private Label label3;
        private GroupBox groupBox1;
        private CheckBox auxAudioEnableCheckBox;
        #endregion
    }
}
