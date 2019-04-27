
namespace CameraCapture
{
    partial class CameraCapture
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
            ReleaseData();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.imageBox2 = new Emgu.CV.UI.ImageBox();
            this.imageBox1 = new Emgu.CV.UI.ImageBox();
            this.captureButton = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.HueMax_trackBar6 = new System.Windows.Forms.TrackBar();
            this.SatMin_trackBar5 = new System.Windows.Forms.TrackBar();
            this.SatMax_trackBar4 = new System.Windows.Forms.TrackBar();
            this.ValMin_trackBar3 = new System.Windows.Forms.TrackBar();
            this.HueMin_trackBar1 = new System.Windows.Forms.TrackBar();
            this.valMax_trackBar1 = new System.Windows.Forms.TrackBar();
            this.TargetX_textBox1 = new System.Windows.Forms.TextBox();
            this.TargetY_textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.minDist_trackBar1 = new System.Windows.Forms.TrackBar();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.dp_trackBar2 = new System.Windows.Forms.TrackBar();
            this.minDist_textBox1 = new System.Windows.Forms.TextBox();
            this.dp_textBox2 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.ballfound = new System.Windows.Forms.Label();
            this.MaxContour_trackBar1 = new System.Windows.Forms.TrackBar();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HueMax_trackBar6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SatMin_trackBar5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SatMax_trackBar4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ValMin_trackBar3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HueMin_trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.valMax_trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minDist_trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dp_trackBar2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxContour_trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // imageBox2
            // 
            this.imageBox2.Location = new System.Drawing.Point(30, 32);
            this.imageBox2.Margin = new System.Windows.Forms.Padding(4);
            this.imageBox2.Name = "imageBox2";
            this.imageBox2.Size = new System.Drawing.Size(416, 332);
            this.imageBox2.TabIndex = 12;
            this.imageBox2.TabStop = false;
            // 
            // imageBox1
            // 
            this.imageBox1.Location = new System.Drawing.Point(30, 391);
            this.imageBox1.Margin = new System.Windows.Forms.Padding(4);
            this.imageBox1.Name = "imageBox1";
            this.imageBox1.Size = new System.Drawing.Size(416, 332);
            this.imageBox1.TabIndex = 13;
            this.imageBox1.TabStop = false;
            // 
            // captureButton
            // 
            this.captureButton.Location = new System.Drawing.Point(960, 639);
            this.captureButton.Name = "captureButton";
            this.captureButton.Size = new System.Drawing.Size(166, 37);
            this.captureButton.TabIndex = 14;
            this.captureButton.Text = "Start Capture";
            this.captureButton.UseVisualStyleBackColor = true;
            this.captureButton.Click += new System.EventHandler(this.captureButton_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(1207, 370);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(73, 17);
            this.label8.TabIndex = 35;
            this.label8.Text = "Value Max";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(1206, 308);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 17);
            this.label7.TabIndex = 34;
            this.label7.Text = "Value Min";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(1207, 246);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 17);
            this.label6.TabIndex = 33;
            this.label6.Text = "Saturation Max";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1207, 184);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 17);
            this.label5.TabIndex = 32;
            this.label5.Text = "Saturation Min";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(1207, 122);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 17);
            this.label4.TabIndex = 31;
            this.label4.Text = "Hue Max";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1207, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 17);
            this.label3.TabIndex = 30;
            this.label3.Text = "Hue Min";
            // 
            // HueMax_trackBar6
            // 
            this.HueMax_trackBar6.Location = new System.Drawing.Point(841, 122);
            this.HueMax_trackBar6.Maximum = 255;
            this.HueMax_trackBar6.Name = "HueMax_trackBar6";
            this.HueMax_trackBar6.Size = new System.Drawing.Size(359, 56);
            this.HueMax_trackBar6.TabIndex = 29;
            this.HueMax_trackBar6.Value = 255;
            this.HueMax_trackBar6.ValueChanged += new System.EventHandler(this.HueMax_trackBar6_ValueChanged);
            // 
            // SatMin_trackBar5
            // 
            this.SatMin_trackBar5.Location = new System.Drawing.Point(841, 184);
            this.SatMin_trackBar5.Maximum = 255;
            this.SatMin_trackBar5.Name = "SatMin_trackBar5";
            this.SatMin_trackBar5.Size = new System.Drawing.Size(359, 56);
            this.SatMin_trackBar5.TabIndex = 28;
            this.SatMin_trackBar5.ValueChanged += new System.EventHandler(this.SatMin_trackBar5_ValueChanged);
            // 
            // SatMax_trackBar4
            // 
            this.SatMax_trackBar4.Location = new System.Drawing.Point(841, 246);
            this.SatMax_trackBar4.Maximum = 255;
            this.SatMax_trackBar4.Name = "SatMax_trackBar4";
            this.SatMax_trackBar4.Size = new System.Drawing.Size(359, 56);
            this.SatMax_trackBar4.TabIndex = 27;
            this.SatMax_trackBar4.Value = 255;
            this.SatMax_trackBar4.ValueChanged += new System.EventHandler(this.SatMax_trackBar4_ValueChanged);
            // 
            // ValMin_trackBar3
            // 
            this.ValMin_trackBar3.Location = new System.Drawing.Point(841, 308);
            this.ValMin_trackBar3.Maximum = 255;
            this.ValMin_trackBar3.Name = "ValMin_trackBar3";
            this.ValMin_trackBar3.Size = new System.Drawing.Size(359, 56);
            this.ValMin_trackBar3.TabIndex = 26;
            this.ValMin_trackBar3.ValueChanged += new System.EventHandler(this.ValMin_trackBar3_ValueChanged);
            // 
            // HueMin_trackBar1
            // 
            this.HueMin_trackBar1.Location = new System.Drawing.Point(841, 56);
            this.HueMin_trackBar1.Maximum = 255;
            this.HueMin_trackBar1.Name = "HueMin_trackBar1";
            this.HueMin_trackBar1.Size = new System.Drawing.Size(359, 56);
            this.HueMin_trackBar1.TabIndex = 24;
            this.HueMin_trackBar1.ValueChanged += new System.EventHandler(this.HueMin_trackBar1_ValueChanged);
            // 
            // valMax_trackBar1
            // 
            this.valMax_trackBar1.Location = new System.Drawing.Point(842, 370);
            this.valMax_trackBar1.Maximum = 255;
            this.valMax_trackBar1.Name = "valMax_trackBar1";
            this.valMax_trackBar1.Size = new System.Drawing.Size(359, 56);
            this.valMax_trackBar1.TabIndex = 36;
            this.valMax_trackBar1.Value = 255;
            this.valMax_trackBar1.ValueChanged += new System.EventHandler(this.valMax_trackBar1_ValueChanged);
            // 
            // TargetX_textBox1
            // 
            this.TargetX_textBox1.Location = new System.Drawing.Point(717, 76);
            this.TargetX_textBox1.Name = "TargetX_textBox1";
            this.TargetX_textBox1.Size = new System.Drawing.Size(100, 22);
            this.TargetX_textBox1.TabIndex = 37;
            // 
            // TargetY_textBox2
            // 
            this.TargetY_textBox2.Location = new System.Drawing.Point(717, 104);
            this.TargetY_textBox2.Name = "TargetY_textBox2";
            this.TargetY_textBox2.Size = new System.Drawing.Size(100, 22);
            this.TargetY_textBox2.TabIndex = 38;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(690, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 17);
            this.label1.TabIndex = 39;
            this.label1.Text = "X:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(690, 107);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 17);
            this.label2.TabIndex = 40;
            this.label2.Text = "Y:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(700, 46);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(108, 17);
            this.label9.TabIndex = 41;
            this.label9.Text = "Target Location";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(974, 439);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(107, 17);
            this.label10.TabIndex = 42;
            this.label10.Text = "Circle Detection";
            // 
            // minDist_trackBar1
            // 
            this.minDist_trackBar1.Location = new System.Drawing.Point(843, 504);
            this.minDist_trackBar1.Maximum = 1000;
            this.minDist_trackBar1.Minimum = 1;
            this.minDist_trackBar1.Name = "minDist_trackBar1";
            this.minDist_trackBar1.Size = new System.Drawing.Size(359, 56);
            this.minDist_trackBar1.TabIndex = 46;
            this.minDist_trackBar1.Value = 100;
            this.minDist_trackBar1.ValueChanged += new System.EventHandler(this.minDist_trackBar1_ValueChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(1206, 509);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(54, 17);
            this.label12.TabIndex = 45;
            this.label12.Text = "minDist";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(1207, 462);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(24, 17);
            this.label13.TabIndex = 44;
            this.label13.Text = "dp";
            // 
            // dp_trackBar2
            // 
            this.dp_trackBar2.Location = new System.Drawing.Point(842, 459);
            this.dp_trackBar2.Maximum = 300;
            this.dp_trackBar2.Name = "dp_trackBar2";
            this.dp_trackBar2.Size = new System.Drawing.Size(359, 56);
            this.dp_trackBar2.TabIndex = 43;
            this.dp_trackBar2.Value = 100;
            this.dp_trackBar2.ValueChanged += new System.EventHandler(this.dp_trackBar2_ValueChanged);
            // 
            // minDist_textBox1
            // 
            this.minDist_textBox1.Location = new System.Drawing.Point(1270, 504);
            this.minDist_textBox1.Name = "minDist_textBox1";
            this.minDist_textBox1.Size = new System.Drawing.Size(46, 22);
            this.minDist_textBox1.TabIndex = 49;
            // 
            // dp_textBox2
            // 
            this.dp_textBox2.Location = new System.Drawing.Point(1270, 459);
            this.dp_textBox2.Name = "dp_textBox2";
            this.dp_textBox2.Size = new System.Drawing.Size(46, 22);
            this.dp_textBox2.TabIndex = 48;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(974, 32);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(98, 17);
            this.label11.TabIndex = 50;
            this.label11.Text = "Hue Detection";
            // 
            // ballfound
            // 
            this.ballfound.AutoSize = true;
            this.ballfound.Location = new System.Drawing.Point(585, 621);
            this.ballfound.Name = "ballfound";
            this.ballfound.Size = new System.Drawing.Size(54, 17);
            this.ballfound.TabIndex = 52;
            this.ballfound.Text = "label14";
            // 
            // MaxContour_trackBar1
            // 
            this.MaxContour_trackBar1.Location = new System.Drawing.Point(493, 562);
            this.MaxContour_trackBar1.Maximum = 500;
            this.MaxContour_trackBar1.Name = "MaxContour_trackBar1";
            this.MaxContour_trackBar1.Size = new System.Drawing.Size(270, 56);
            this.MaxContour_trackBar1.TabIndex = 53;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(500, 529);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(95, 17);
            this.label14.TabIndex = 54;
            this.label14.Text = "Max Countour";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(500, 621);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(79, 17);
            this.label15.TabIndex = 55;
            this.label15.Text = "Ball Found:";
            // 
            // CameraCapture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1334, 737);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.MaxContour_trackBar1);
            this.Controls.Add(this.ballfound);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.minDist_textBox1);
            this.Controls.Add(this.dp_textBox2);
            this.Controls.Add(this.minDist_trackBar1);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.dp_trackBar2);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TargetY_textBox2);
            this.Controls.Add(this.TargetX_textBox1);
            this.Controls.Add(this.valMax_trackBar1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.HueMax_trackBar6);
            this.Controls.Add(this.SatMin_trackBar5);
            this.Controls.Add(this.SatMax_trackBar4);
            this.Controls.Add(this.ValMin_trackBar3);
            this.Controls.Add(this.HueMin_trackBar1);
            this.Controls.Add(this.captureButton);
            this.Controls.Add(this.imageBox1);
            this.Controls.Add(this.imageBox2);
            this.Name = "CameraCapture";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CameraCapture_FormClosing);
            this.Load += new System.EventHandler(this.CameraCapture_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imageBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HueMax_trackBar6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SatMin_trackBar5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SatMax_trackBar4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ValMin_trackBar3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HueMin_trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.valMax_trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minDist_trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dp_trackBar2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxContour_trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Emgu.CV.UI.ImageBox imageBox2;
        private Emgu.CV.UI.ImageBox imageBox1;
        private System.Windows.Forms.Button captureButton;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar HueMax_trackBar6;
        private System.Windows.Forms.TrackBar SatMin_trackBar5;
        private System.Windows.Forms.TrackBar SatMax_trackBar4;
        private System.Windows.Forms.TrackBar ValMin_trackBar3;
        private System.Windows.Forms.TrackBar valMax_trackBar1;
        private System.Windows.Forms.TextBox TargetX_textBox1;
        private System.Windows.Forms.TextBox TargetY_textBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TrackBar minDist_trackBar1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TrackBar dp_trackBar2;
        private System.Windows.Forms.TextBox minDist_textBox1;
        private System.Windows.Forms.TextBox dp_textBox2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label ballfound;
        private System.Windows.Forms.TrackBar MaxContour_trackBar1;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TrackBar HueMin_trackBar1;
    }
}

