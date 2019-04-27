namespace RoboticArm_XBoxController_GUI
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
         this.components = new System.ComponentModel.Container();
         this.timer1 = new System.Windows.Forms.Timer(this.components);
         this.label15 = new System.Windows.Forms.Label();
         this.label17 = new System.Windows.Forms.Label();
         this.label20 = new System.Windows.Forms.Label();
         this.trackBar_base = new System.Windows.Forms.TrackBar();
         this.label7 = new System.Windows.Forms.Label();
         this.label9 = new System.Windows.Forms.Label();
         this.label13 = new System.Windows.Forms.Label();
         this.trackBar_ArmY = new System.Windows.Forms.TrackBar();
         this.label5 = new System.Windows.Forms.Label();
         this.label3 = new System.Windows.Forms.Label();
         this.label4 = new System.Windows.Forms.Label();
         this.trackBar_armX = new System.Windows.Forms.TrackBar();
         this.groupBox4 = new System.Windows.Forms.GroupBox();
         this.lblElbow_pp = new System.Windows.Forms.Label();
         this.label18 = new System.Windows.Forms.Label();
         this.groupBox3 = new System.Windows.Forms.GroupBox();
         this.lblShoulder_pp = new System.Windows.Forms.Label();
         this.label12 = new System.Windows.Forms.Label();
         this.groupBox2 = new System.Windows.Forms.GroupBox();
         this.lblBase_pp = new System.Windows.Forms.Label();
         this.label19 = new System.Windows.Forms.Label();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.lblGripper_pp = new System.Windows.Forms.Label();
         this.label8 = new System.Windows.Forms.Label();
         this.radioButton_retracted = new System.Windows.Forms.RadioButton();
         this.radioButton_closed = new System.Windows.Forms.RadioButton();
         this.groupBox_GripperServo = new System.Windows.Forms.GroupBox();
         this.trackBar_gimbalX = new System.Windows.Forms.TrackBar();
         this.trackBar_gimbalY = new System.Windows.Forms.TrackBar();
         this.label1 = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.Reset = new System.Windows.Forms.Button();
         this.button1 = new System.Windows.Forms.Button();
         this.AutoGrab = new System.Windows.Forms.Button();
         this.label6 = new System.Windows.Forms.Label();
         this.EncoderRight = new System.Windows.Forms.Label();
         this.EncoderLeft = new System.Windows.Forms.Label();
         this.label14 = new System.Windows.Forms.Label();
         this.label16 = new System.Windows.Forms.Label();
         this.LidarData = new System.Windows.Forms.Label();
         this.label11 = new System.Windows.Forms.Label();
         this.Tracking = new System.Windows.Forms.Button();
         this.label10 = new System.Windows.Forms.Label();
         this.label21 = new System.Windows.Forms.Label();
         this.RightError = new System.Windows.Forms.Label();
         this.LeftError = new System.Windows.Forms.Label();
         this.Ygimbal = new System.Windows.Forms.Label();
         this.label23 = new System.Windows.Forms.Label();
         this.Xgimbal = new System.Windows.Forms.Label();
         this.label25 = new System.Windows.Forms.Label();
         ((System.ComponentModel.ISupportInitialize)(this.trackBar_base)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.trackBar_ArmY)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.trackBar_armX)).BeginInit();
         this.groupBox4.SuspendLayout();
         this.groupBox3.SuspendLayout();
         this.groupBox2.SuspendLayout();
         this.groupBox1.SuspendLayout();
         this.groupBox_GripperServo.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.trackBar_gimbalX)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.trackBar_gimbalY)).BeginInit();
         this.SuspendLayout();
         // 
         // timer1
         // 
         this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
         // 
         // label15
         // 
         this.label15.AutoSize = true;
         this.label15.Location = new System.Drawing.Point(709, 66);
         this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         this.label15.Name = "label15";
         this.label15.Size = new System.Drawing.Size(36, 20);
         this.label15.TabIndex = 44;
         this.label15.Text = "270";
         // 
         // label17
         // 
         this.label17.AutoSize = true;
         this.label17.Location = new System.Drawing.Point(340, 66);
         this.label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         this.label17.Name = "label17";
         this.label17.Size = new System.Drawing.Size(18, 20);
         this.label17.TabIndex = 43;
         this.label17.Text = "0";
         // 
         // label20
         // 
         this.label20.AutoSize = true;
         this.label20.Location = new System.Drawing.Point(508, 18);
         this.label20.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         this.label20.Name = "label20";
         this.label20.Size = new System.Drawing.Size(91, 20);
         this.label20.TabIndex = 42;
         this.label20.Text = "Base Servo";
         // 
         // trackBar_base
         // 
         this.trackBar_base.Location = new System.Drawing.Point(382, 46);
         this.trackBar_base.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
         this.trackBar_base.Maximum = 270;
         this.trackBar_base.Name = "trackBar_base";
         this.trackBar_base.Size = new System.Drawing.Size(318, 69);
         this.trackBar_base.SmallChange = 5;
         this.trackBar_base.TabIndex = 41;
         this.trackBar_base.Value = 135;
         this.trackBar_base.ValueChanged += new System.EventHandler(this.trackBar_base_ValueChanged);
         // 
         // label7
         // 
         this.label7.AutoSize = true;
         this.label7.Location = new System.Drawing.Point(709, 191);
         this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         this.label7.Name = "label7";
         this.label7.Size = new System.Drawing.Size(77, 20);
         this.label7.TabIndex = 40;
         this.label7.Text = "Extended";
         // 
         // label9
         // 
         this.label9.AutoSize = true;
         this.label9.ForeColor = System.Drawing.Color.Red;
         this.label9.Location = new System.Drawing.Point(292, 191);
         this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         this.label9.Name = "label9";
         this.label9.Size = new System.Drawing.Size(80, 20);
         this.label9.TabIndex = 39;
         this.label9.Text = "Retracted";
         // 
         // label13
         // 
         this.label13.AutoSize = true;
         this.label13.Location = new System.Drawing.Point(502, 140);
         this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         this.label13.Name = "label13";
         this.label13.Size = new System.Drawing.Size(98, 20);
         this.label13.TabIndex = 38;
         this.label13.Text = "Arm Y Servo";
         // 
         // trackBar_ArmY
         // 
         this.trackBar_ArmY.Location = new System.Drawing.Point(382, 171);
         this.trackBar_ArmY.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
         this.trackBar_ArmY.Maximum = 200;
         this.trackBar_ArmY.Name = "trackBar_ArmY";
         this.trackBar_ArmY.Size = new System.Drawing.Size(318, 69);
         this.trackBar_ArmY.SmallChange = 5;
         this.trackBar_ArmY.TabIndex = 37;
         this.trackBar_ArmY.ValueChanged += new System.EventHandler(this.trackBar_ArmY_ValueChanged);
         // 
         // label5
         // 
         this.label5.AutoSize = true;
         this.label5.Location = new System.Drawing.Point(709, 322);
         this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         this.label5.Name = "label5";
         this.label5.Size = new System.Drawing.Size(77, 20);
         this.label5.TabIndex = 36;
         this.label5.Text = "Extended";
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.ForeColor = System.Drawing.Color.Red;
         this.label3.Location = new System.Drawing.Point(292, 322);
         this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(80, 20);
         this.label3.TabIndex = 35;
         this.label3.Text = "Retracted";
         // 
         // label4
         // 
         this.label4.AutoSize = true;
         this.label4.Location = new System.Drawing.Point(502, 272);
         this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(98, 20);
         this.label4.TabIndex = 34;
         this.label4.Text = "Arm X Servo";
         // 
         // trackBar_armX
         // 
         this.trackBar_armX.Location = new System.Drawing.Point(383, 300);
         this.trackBar_armX.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
         this.trackBar_armX.Maximum = 120;
         this.trackBar_armX.Name = "trackBar_armX";
         this.trackBar_armX.Size = new System.Drawing.Size(318, 69);
         this.trackBar_armX.SmallChange = 5;
         this.trackBar_armX.TabIndex = 0;
         this.trackBar_armX.Value = 1;
         this.trackBar_armX.ValueChanged += new System.EventHandler(this.trackBar_armX_ValueChanged);
         // 
         // groupBox4
         // 
         this.groupBox4.Controls.Add(this.lblElbow_pp);
         this.groupBox4.Controls.Add(this.label18);
         this.groupBox4.Location = new System.Drawing.Point(14, 288);
         this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
         this.groupBox4.Name = "groupBox4";
         this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
         this.groupBox4.Size = new System.Drawing.Size(243, 80);
         this.groupBox4.TabIndex = 31;
         this.groupBox4.TabStop = false;
         this.groupBox4.Text = "Arm X";
         // 
         // lblElbow_pp
         // 
         this.lblElbow_pp.AutoSize = true;
         this.lblElbow_pp.Location = new System.Drawing.Point(152, 34);
         this.lblElbow_pp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         this.lblElbow_pp.Name = "lblElbow_pp";
         this.lblElbow_pp.Size = new System.Drawing.Size(24, 20);
         this.lblElbow_pp.TabIndex = 3;
         this.lblElbow_pp.Text = "---";
         this.lblElbow_pp.Click += new System.EventHandler(this.lblElbow_pp_Click);
         // 
         // label18
         // 
         this.label18.AutoSize = true;
         this.label18.Location = new System.Drawing.Point(9, 34);
         this.label18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         this.label18.Name = "label18";
         this.label18.Size = new System.Drawing.Size(132, 20);
         this.label18.TabIndex = 2;
         this.label18.Text = "Present Position: ";
         // 
         // groupBox3
         // 
         this.groupBox3.Controls.Add(this.lblShoulder_pp);
         this.groupBox3.Controls.Add(this.label12);
         this.groupBox3.Location = new System.Drawing.Point(14, 154);
         this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
         this.groupBox3.Name = "groupBox3";
         this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
         this.groupBox3.Size = new System.Drawing.Size(243, 86);
         this.groupBox3.TabIndex = 32;
         this.groupBox3.TabStop = false;
         this.groupBox3.Text = "Arm Y";
         // 
         // lblShoulder_pp
         // 
         this.lblShoulder_pp.AutoSize = true;
         this.lblShoulder_pp.Location = new System.Drawing.Point(152, 34);
         this.lblShoulder_pp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         this.lblShoulder_pp.Name = "lblShoulder_pp";
         this.lblShoulder_pp.Size = new System.Drawing.Size(24, 20);
         this.lblShoulder_pp.TabIndex = 3;
         this.lblShoulder_pp.Text = "---";
         this.lblShoulder_pp.Click += new System.EventHandler(this.lblShoulder_pp_Click);
         // 
         // label12
         // 
         this.label12.AutoSize = true;
         this.label12.Location = new System.Drawing.Point(9, 34);
         this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         this.label12.Name = "label12";
         this.label12.Size = new System.Drawing.Size(132, 20);
         this.label12.TabIndex = 2;
         this.label12.Text = "Present Position: ";
         // 
         // groupBox2
         // 
         this.groupBox2.Controls.Add(this.lblBase_pp);
         this.groupBox2.Controls.Add(this.label19);
         this.groupBox2.Location = new System.Drawing.Point(14, 39);
         this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
         this.groupBox2.Name = "groupBox2";
         this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
         this.groupBox2.Size = new System.Drawing.Size(243, 78);
         this.groupBox2.TabIndex = 30;
         this.groupBox2.TabStop = false;
         this.groupBox2.Text = "Base";
         // 
         // lblBase_pp
         // 
         this.lblBase_pp.AutoSize = true;
         this.lblBase_pp.Location = new System.Drawing.Point(152, 34);
         this.lblBase_pp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         this.lblBase_pp.Name = "lblBase_pp";
         this.lblBase_pp.Size = new System.Drawing.Size(24, 20);
         this.lblBase_pp.TabIndex = 3;
         this.lblBase_pp.Text = "---";
         this.lblBase_pp.Click += new System.EventHandler(this.lblBase_pp_Click);
         // 
         // label19
         // 
         this.label19.AutoSize = true;
         this.label19.Location = new System.Drawing.Point(9, 34);
         this.label19.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         this.label19.Name = "label19";
         this.label19.Size = new System.Drawing.Size(132, 20);
         this.label19.TabIndex = 2;
         this.label19.Text = "Present Position: ";
         // 
         // groupBox1
         // 
         this.groupBox1.Controls.Add(this.lblGripper_pp);
         this.groupBox1.Controls.Add(this.label8);
         this.groupBox1.Location = new System.Drawing.Point(14, 386);
         this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
         this.groupBox1.Size = new System.Drawing.Size(243, 80);
         this.groupBox1.TabIndex = 32;
         this.groupBox1.TabStop = false;
         this.groupBox1.Text = "Gripper";
         // 
         // lblGripper_pp
         // 
         this.lblGripper_pp.AutoSize = true;
         this.lblGripper_pp.Location = new System.Drawing.Point(152, 34);
         this.lblGripper_pp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         this.lblGripper_pp.Name = "lblGripper_pp";
         this.lblGripper_pp.Size = new System.Drawing.Size(24, 20);
         this.lblGripper_pp.TabIndex = 3;
         this.lblGripper_pp.Text = "---";
         this.lblGripper_pp.Click += new System.EventHandler(this.lblGripper_pp_Click);
         // 
         // label8
         // 
         this.label8.AutoSize = true;
         this.label8.Location = new System.Drawing.Point(9, 34);
         this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         this.label8.Name = "label8";
         this.label8.Size = new System.Drawing.Size(132, 20);
         this.label8.TabIndex = 2;
         this.label8.Text = "Present Position: ";
         // 
         // radioButton_retracted
         // 
         this.radioButton_retracted.AutoSize = true;
         this.radioButton_retracted.Location = new System.Drawing.Point(54, 26);
         this.radioButton_retracted.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
         this.radioButton_retracted.Name = "radioButton_retracted";
         this.radioButton_retracted.Size = new System.Drawing.Size(105, 24);
         this.radioButton_retracted.TabIndex = 50;
         this.radioButton_retracted.TabStop = true;
         this.radioButton_retracted.Text = "Retracted";
         this.radioButton_retracted.UseVisualStyleBackColor = true;
         this.radioButton_retracted.Click += new System.EventHandler(this.radioButton_retracted_Click);
         // 
         // radioButton_closed
         // 
         this.radioButton_closed.AutoSize = true;
         this.radioButton_closed.Location = new System.Drawing.Point(54, 60);
         this.radioButton_closed.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
         this.radioButton_closed.Name = "radioButton_closed";
         this.radioButton_closed.Size = new System.Drawing.Size(83, 24);
         this.radioButton_closed.TabIndex = 51;
         this.radioButton_closed.TabStop = true;
         this.radioButton_closed.Text = "Closed";
         this.radioButton_closed.UseVisualStyleBackColor = true;
         this.radioButton_closed.Click += new System.EventHandler(this.radioButton_closed_Click);
         // 
         // groupBox_GripperServo
         // 
         this.groupBox_GripperServo.Controls.Add(this.radioButton_retracted);
         this.groupBox_GripperServo.Controls.Add(this.radioButton_closed);
         this.groupBox_GripperServo.Location = new System.Drawing.Point(454, 378);
         this.groupBox_GripperServo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
         this.groupBox_GripperServo.Name = "groupBox_GripperServo";
         this.groupBox_GripperServo.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
         this.groupBox_GripperServo.Size = new System.Drawing.Size(173, 105);
         this.groupBox_GripperServo.TabIndex = 52;
         this.groupBox_GripperServo.TabStop = false;
         this.groupBox_GripperServo.Text = "Gripper Servo";
         // 
         // trackBar_gimbalX
         // 
         this.trackBar_gimbalX.Location = new System.Drawing.Point(800, 248);
         this.trackBar_gimbalX.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
         this.trackBar_gimbalX.Maximum = 360;
         this.trackBar_gimbalX.Name = "trackBar_gimbalX";
         this.trackBar_gimbalX.Size = new System.Drawing.Size(295, 69);
         this.trackBar_gimbalX.SmallChange = 5;
         this.trackBar_gimbalX.TabIndex = 53;
         this.trackBar_gimbalX.Value = 180;
         this.trackBar_gimbalX.ValueChanged += new System.EventHandler(this.trackBar_gimbalX_ValueChanged);
         // 
         // trackBar_gimbalY
         // 
         this.trackBar_gimbalY.Location = new System.Drawing.Point(800, 359);
         this.trackBar_gimbalY.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
         this.trackBar_gimbalY.Maximum = 100;
         this.trackBar_gimbalY.Name = "trackBar_gimbalY";
         this.trackBar_gimbalY.Size = new System.Drawing.Size(295, 69);
         this.trackBar_gimbalY.SmallChange = 5;
         this.trackBar_gimbalY.TabIndex = 54;
         this.trackBar_gimbalY.Value = 50;
         this.trackBar_gimbalY.ValueChanged += new System.EventHandler(this.trackBar_gimbalY_ValueChanged);
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(897, 334);
         this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(70, 20);
         this.label1.TabIndex = 55;
         this.label1.Text = "GimbalY";
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(897, 220);
         this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(70, 20);
         this.label2.TabIndex = 56;
         this.label2.Text = "GimbalX";
         // 
         // Reset
         // 
         this.Reset.Location = new System.Drawing.Point(912, 436);
         this.Reset.Name = "Reset";
         this.Reset.Size = new System.Drawing.Size(75, 47);
         this.Reset.TabIndex = 57;
         this.Reset.Text = "Reset";
         this.Reset.UseVisualStyleBackColor = true;
         this.Reset.Click += new System.EventHandler(this.button1_Click);
         // 
         // button1
         // 
         this.button1.Location = new System.Drawing.Point(670, 436);
         this.button1.Name = "button1";
         this.button1.Size = new System.Drawing.Size(101, 47);
         this.button1.TabIndex = 58;
         this.button1.Text = "Lidar Data";
         this.button1.UseVisualStyleBackColor = true;
         this.button1.Click += new System.EventHandler(this.button1_Click_1);
         // 
         // AutoGrab
         // 
         this.AutoGrab.Location = new System.Drawing.Point(311, 450);
         this.AutoGrab.Name = "AutoGrab";
         this.AutoGrab.Size = new System.Drawing.Size(93, 45);
         this.AutoGrab.TabIndex = 59;
         this.AutoGrab.Text = "Auto Grab";
         this.AutoGrab.UseVisualStyleBackColor = true;
         this.AutoGrab.Click += new System.EventHandler(this.AutoGrab_Click);
         // 
         // label6
         // 
         this.label6.AutoSize = true;
         this.label6.ForeColor = System.Drawing.Color.Crimson;
         this.label6.Location = new System.Drawing.Point(872, 413);
         this.label6.Name = "label6";
         this.label6.Size = new System.Drawing.Size(144, 20);
         this.label6.TabIndex = 60;
         this.label6.Text = "TOUCH ME FIRST";
         // 
         // EncoderRight
         // 
         this.EncoderRight.AutoSize = true;
         this.EncoderRight.Location = new System.Drawing.Point(830, 106);
         this.EncoderRight.Name = "EncoderRight";
         this.EncoderRight.Size = new System.Drawing.Size(19, 20);
         this.EncoderRight.TabIndex = 61;
         this.EncoderRight.Text = "--";
         // 
         // EncoderLeft
         // 
         this.EncoderLeft.AutoSize = true;
         this.EncoderLeft.Location = new System.Drawing.Point(830, 66);
         this.EncoderLeft.Name = "EncoderLeft";
         this.EncoderLeft.Size = new System.Drawing.Size(19, 20);
         this.EncoderLeft.TabIndex = 62;
         this.EncoderLeft.Text = "--";
         // 
         // label14
         // 
         this.label14.AutoSize = true;
         this.label14.Location = new System.Drawing.Point(786, 86);
         this.label14.Name = "label14";
         this.label14.Size = new System.Drawing.Size(111, 20);
         this.label14.TabIndex = 63;
         this.label14.Text = "Right Encoder";
         // 
         // label16
         // 
         this.label16.AutoSize = true;
         this.label16.Location = new System.Drawing.Point(796, 39);
         this.label16.Name = "label16";
         this.label16.Size = new System.Drawing.Size(101, 20);
         this.label16.TabIndex = 64;
         this.label16.Text = "Left Encoder";
         // 
         // LidarData
         // 
         this.LidarData.AutoSize = true;
         this.LidarData.Location = new System.Drawing.Point(830, 146);
         this.LidarData.Name = "LidarData";
         this.LidarData.Size = new System.Drawing.Size(19, 20);
         this.LidarData.TabIndex = 65;
         this.LidarData.Text = "--";
         // 
         // label11
         // 
         this.label11.AutoSize = true;
         this.label11.Location = new System.Drawing.Point(786, 126);
         this.label11.Name = "label11";
         this.label11.Size = new System.Drawing.Size(111, 20);
         this.label11.TabIndex = 66;
         this.label11.Text = "Lidar Distance";
         // 
         // Tracking
         // 
         this.Tracking.Location = new System.Drawing.Point(311, 378);
         this.Tracking.Name = "Tracking";
         this.Tracking.Size = new System.Drawing.Size(93, 40);
         this.Tracking.TabIndex = 67;
         this.Tracking.Text = "Tracking";
         this.Tracking.UseVisualStyleBackColor = true;
         this.Tracking.Click += new System.EventHandler(this.Tracking_Click);
         // 
         // label10
         // 
         this.label10.AutoSize = true;
         this.label10.Location = new System.Drawing.Point(984, 39);
         this.label10.Name = "label10";
         this.label10.Size = new System.Drawing.Size(86, 20);
         this.label10.TabIndex = 68;
         this.label10.Text = "Right Error";
         // 
         // label21
         // 
         this.label21.AutoSize = true;
         this.label21.Location = new System.Drawing.Point(984, 79);
         this.label21.Name = "label21";
         this.label21.Size = new System.Drawing.Size(76, 20);
         this.label21.TabIndex = 69;
         this.label21.Text = "Left Error";
         // 
         // RightError
         // 
         this.RightError.AutoSize = true;
         this.RightError.Location = new System.Drawing.Point(1014, 59);
         this.RightError.Name = "RightError";
         this.RightError.Size = new System.Drawing.Size(19, 20);
         this.RightError.TabIndex = 70;
         this.RightError.Text = "--";
         // 
         // LeftError
         // 
         this.LeftError.AutoSize = true;
         this.LeftError.Location = new System.Drawing.Point(1014, 99);
         this.LeftError.Name = "LeftError";
         this.LeftError.Size = new System.Drawing.Size(19, 20);
         this.LeftError.TabIndex = 71;
         this.LeftError.Text = "--";
         // 
         // Ygimbal
         // 
         this.Ygimbal.AutoSize = true;
         this.Ygimbal.Location = new System.Drawing.Point(1014, 191);
         this.Ygimbal.Name = "Ygimbal";
         this.Ygimbal.Size = new System.Drawing.Size(19, 20);
         this.Ygimbal.TabIndex = 72;
         this.Ygimbal.Text = "--";
         // 
         // label23
         // 
         this.label23.AutoSize = true;
         this.label23.Location = new System.Drawing.Point(984, 171);
         this.label23.Name = "label23";
         this.label23.Size = new System.Drawing.Size(70, 20);
         this.label23.TabIndex = 73;
         this.label23.Text = "GimbalY";
         // 
         // Xgimbal
         // 
         this.Xgimbal.AutoSize = true;
         this.Xgimbal.Location = new System.Drawing.Point(1014, 154);
         this.Xgimbal.Name = "Xgimbal";
         this.Xgimbal.Size = new System.Drawing.Size(19, 20);
         this.Xgimbal.TabIndex = 74;
         this.Xgimbal.Text = "--";
         // 
         // label25
         // 
         this.label25.AutoSize = true;
         this.label25.Location = new System.Drawing.Point(984, 126);
         this.label25.Name = "label25";
         this.label25.Size = new System.Drawing.Size(70, 20);
         this.label25.TabIndex = 75;
         this.label25.Text = "GimbalX";
         // 
         // Form1
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(1108, 498);
         this.Controls.Add(this.label25);
         this.Controls.Add(this.Xgimbal);
         this.Controls.Add(this.label23);
         this.Controls.Add(this.Ygimbal);
         this.Controls.Add(this.LeftError);
         this.Controls.Add(this.RightError);
         this.Controls.Add(this.label21);
         this.Controls.Add(this.label10);
         this.Controls.Add(this.Tracking);
         this.Controls.Add(this.label11);
         this.Controls.Add(this.LidarData);
         this.Controls.Add(this.label16);
         this.Controls.Add(this.label14);
         this.Controls.Add(this.EncoderLeft);
         this.Controls.Add(this.EncoderRight);
         this.Controls.Add(this.label6);
         this.Controls.Add(this.AutoGrab);
         this.Controls.Add(this.button1);
         this.Controls.Add(this.Reset);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.trackBar_gimbalY);
         this.Controls.Add(this.trackBar_gimbalX);
         this.Controls.Add(this.groupBox_GripperServo);
         this.Controls.Add(this.groupBox1);
         this.Controls.Add(this.label15);
         this.Controls.Add(this.label17);
         this.Controls.Add(this.label20);
         this.Controls.Add(this.trackBar_base);
         this.Controls.Add(this.label7);
         this.Controls.Add(this.label9);
         this.Controls.Add(this.label13);
         this.Controls.Add(this.trackBar_ArmY);
         this.Controls.Add(this.label5);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.label4);
         this.Controls.Add(this.trackBar_armX);
         this.Controls.Add(this.groupBox4);
         this.Controls.Add(this.groupBox3);
         this.Controls.Add(this.groupBox2);
         this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.Name = "Form1";
         this.Text = "Form1";
         ((System.ComponentModel.ISupportInitialize)(this.trackBar_base)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.trackBar_ArmY)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.trackBar_armX)).EndInit();
         this.groupBox4.ResumeLayout(false);
         this.groupBox4.PerformLayout();
         this.groupBox3.ResumeLayout(false);
         this.groupBox3.PerformLayout();
         this.groupBox2.ResumeLayout(false);
         this.groupBox2.PerformLayout();
         this.groupBox1.ResumeLayout(false);
         this.groupBox1.PerformLayout();
         this.groupBox_GripperServo.ResumeLayout(false);
         this.groupBox_GripperServo.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.trackBar_gimbalX)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.trackBar_gimbalY)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TrackBar trackBar_base;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TrackBar trackBar_ArmY;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TrackBar trackBar_armX;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label lblElbow_pp;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblShoulder_pp;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblBase_pp;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblGripper_pp;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RadioButton radioButton_retracted;
        private System.Windows.Forms.RadioButton radioButton_closed;
        private System.Windows.Forms.GroupBox groupBox_GripperServo;
        private System.Windows.Forms.TrackBar trackBar_gimbalY;
        private System.Windows.Forms.TrackBar trackBar_gimbalX;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Reset;
        private System.Windows.Forms.Button button1;
      private System.Windows.Forms.Button AutoGrab;
      private System.Windows.Forms.Label label6;
      private System.Windows.Forms.Label EncoderRight;
      private System.Windows.Forms.Label EncoderLeft;
      private System.Windows.Forms.Label label14;
      private System.Windows.Forms.Label label16;
      private System.Windows.Forms.Label LidarData;
      private System.Windows.Forms.Label label11;
      private System.Windows.Forms.Button Tracking;
      private System.Windows.Forms.Label label10;
      private System.Windows.Forms.Label label21;
      private System.Windows.Forms.Label RightError;
      private System.Windows.Forms.Label LeftError;
      private System.Windows.Forms.Label Ygimbal;
      private System.Windows.Forms.Label label23;
      private System.Windows.Forms.Label Xgimbal;
      private System.Windows.Forms.Label label25;
   }
}

