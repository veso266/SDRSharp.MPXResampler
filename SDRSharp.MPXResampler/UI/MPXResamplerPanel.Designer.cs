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
            this.components = new System.ComponentModel.Container();
            this.displayTimer = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.auxAudioEnableCheckBox = new System.Windows.Forms.CheckBox();
            this.SampleRateLbl = new System.Windows.Forms.Label();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // displayTimer
            // 
            this.displayTimer.Enabled = true;
            this.displayTimer.Interval = 500;
            this.displayTimer.Tick += new System.EventHandler(this.displayTimer_Tick);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.SampleRateLbl);
            this.groupBox1.Controls.Add(this.auxAudioEnableCheckBox);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Location = new System.Drawing.Point(4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(205, 104);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            // 
            // auxAudioEnableCheckBox
            // 
            this.auxAudioEnableCheckBox.AutoSize = true;
            this.auxAudioEnableCheckBox.Location = new System.Drawing.Point(9, 0);
            this.auxAudioEnableCheckBox.Name = "auxAudioEnableCheckBox";
            this.auxAudioEnableCheckBox.Size = new System.Drawing.Size(59, 17);
            this.auxAudioEnableCheckBox.TabIndex = 3;
            this.auxAudioEnableCheckBox.Text = "Enable";
            this.auxAudioEnableCheckBox.UseVisualStyleBackColor = true;
            this.auxAudioEnableCheckBox.CheckedChanged += new System.EventHandler(this.auxAudioEnableCheckBox_CheckedChanged);
            // 
            // SampleRateLbl
            // 
            this.SampleRateLbl.AutoSize = true;
            this.SampleRateLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.SampleRateLbl.Location = new System.Drawing.Point(6, 75);
            this.SampleRateLbl.Name = "SampleRateLbl";
            this.SampleRateLbl.Size = new System.Drawing.Size(123, 20);
            this.SampleRateLbl.TabIndex = 11;
            this.SampleRateLbl.Text = "Sample Rate: ";
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(117, 41);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(73, 17);
            this.radioButton2.TabIndex = 13;
            this.radioButton2.Text = "Decimator";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(9, 41);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(102, 17);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Resampler class";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Method of resampling";
            // 
            // MPXResamplerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "MPXResamplerPanel";
            this.Size = new System.Drawing.Size(209, 121);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }
        private Timer displayTimer;
        private GroupBox groupBox1;
        private CheckBox auxAudioEnableCheckBox;
        #endregion

        private Label SampleRateLbl;
        private RadioButton radioButton2;
        private RadioButton radioButton1;
        private Label label1;
    }
}
