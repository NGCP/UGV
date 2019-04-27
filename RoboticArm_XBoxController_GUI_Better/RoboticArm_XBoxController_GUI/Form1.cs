using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX.XInput;
using System.Threading;
using UGV.Core.IO;

namespace RoboticArm_XBoxController_GUI
{
    /// <summary>
    /// To use the XBox One controller, need to run the following commands on the NuGet Package Manager Console to include needed API
    /// Install-Package SharpDX
    /// Install-Package SharpDX.XInput -Version 3.1.1
    /// </summary>
    
    public partial class Form1 : Form
    {
        // properties
        /// <summary>
        /// The connected Xbox One controller. All properties are updated with in one of its methods.
        /// </summary>
        //private XInputController xb1c;

        /// <summary>
        /// The id values for each of the servos. These are to be sent to the vehicle so it can know which one to write to.
        /// </summary>
        private const byte turretServo_id = 0x45, armX_id = 0x44, armY_id = 0x43, gripper_id = 0x46;

      /// <summary>
      /// The normalized value read from the controller. This should be multiplied by a gain, depending on each servo, and stored in a separtely to be sent.
      /// The Xbox One controller servo assignments is as follows:
      /// Base : Left joystick, X-direction
      /// Shoulder : Left joystick, Y-direction
      /// Elbow : Right joystick, Y-direction
      /// Gripper : Right joystick, X-direction
      /// </summary>
      private int turretServo = 135;
      private int armX = 0;
      private int armY = 0;
      private int gimbalX = 180;
      private int gimbalY = 50;
      private bool gripper = true;
      private bool armReset = false;
      private bool firstreset = false;
      private bool PackageRecieved = false;
      private int RightEncoder = 0;
      private int LeftEnconder = 0;
      private bool track = false;
      private int xDir = 0;
      private int yDir = 0;
      private bool found = false; 
      UdpClientSocket udp_camera { get; set; }
      private bool leftpan = true;
      private bool rightpan = false; 
        private void trackBar_armX_ValueChanged(object sender, EventArgs e)
        {
            // send a packet to the UGV with the updated change 
            armX = trackBar_armX.Value;
         //SendArmControl(armX, armY, turretServo, gripper,gimbalX,gimbalY,armReset);
            if(firstreset)
               ArmXSend(armX);
        }

        private void trackBar_base_ValueChanged(object sender, EventArgs e)
        {
            // send a packet to the UGV with the updated change values
            turretServo = trackBar_base.Value;
            if(firstreset)
               TurrentServo(turretServo);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (firstreset == false)
               firstreset = true; 
            armReset = true;
            ArmReset(armReset);
            armReset = false; 
        }


        private void trackBar_gimbalX_ValueChanged(object sender, EventArgs e)
        {
            gimbalX = trackBar_gimbalX.Value;
            GimbalPhi(gimbalX);
        }

        private void trackBar_ArmY_ValueChanged(object sender, EventArgs e)
        {
            armY = trackBar_ArmY.Value;
            if(firstreset)
               ArmYSend(armY);
        }

        private void trackBar_gimbalY_ValueChanged(object sender, EventArgs e)
        {
            gimbalY = trackBar_gimbalY.Value;
            GimbalTheta(gimbalY); 
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            LidarRecieve();
        }

        private void radioButton_retracted_Click(object sender, EventArgs e)
        {
         if (armY > 115)
         {
            gripper = false;
            if (firstreset)
               GripperControl(gripper);
         }
        }

        private void radioButton_closed_Click(object sender, EventArgs e)
        {
            gripper = true;
            if(firstreset)
               GripperControl(gripper);
        }



        /// <summary>
        /// The defined value each joystick coordinate must be passed in order to write any gain values.
        /// </summary>
        private const int Xthreshold = 30, Ythreshold = 30;

        private int LidarDistance;
        Serial fpga = new Serial("COM10", 9600);  // use 9600 for FPGA, use 57600

        public Form1()
        {
            InitializeComponent();

            //construct fpga
            fpga.PackageMode = Serial.PackageModes.UseFPGA;       // for FPGA
            //define callback
            fpga.PackageReceived = (bytes =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
                Console.WriteLine("package length: {0}", bytes.Length);
                for (int i = 0; i < bytes.Length; i++)
                {
                    Console.WriteLine("byte {0}: {1}", i, (char)bytes[i]);
                }
                switch (bytes[1])
                {
                    case 0x4B:
                        int temp = 0;
                        int MSB = bytes[3] - 0x30;
                        int SB =  bytes[4] - 0x30;
                        int LSB = bytes[5] - 0x30;
                        temp += LSB;
                        temp += SB * 10;
                        temp += MSB * 100;
                        LidarDistance = temp;  
                        PackageRecieved = true;
                        break;
                  case 0x49:
                     LSB = bytes[5] - 0x30;
                     if (LSB == 1)
                        RightEncoder += 1;
                     else
                        RightEncoder -= 1;  
                     break;
                  case 0x4C:
                     LSB = bytes[5] - 0x30;
                     if (LSB == 1)
                        LeftEnconder += 1;
                     else
                        LeftEnconder -= 1;
                     break;
                }
                Console.WriteLine("\n");
            });
         //start    
         
         udp_camera = new UdpClientSocket(
         System.Net.IPAddress.Parse("127.0.0.1"), 8008);
         //define call back
         udp_camera.PackageReceived = (bytes =>
         {
            xDir = BitConverter.ToInt32(bytes, 0);
            yDir = BitConverter.ToInt32(bytes, sizeof(int));
            //y_mainpayload = BitConverter.ToInt32(bytes, (sizeof(int) + sizeof(bool)));
            if (track) 
                  GimbalTracking(xDir, yDir);
             
         });
         udp_camera.Start();
      
         fpga.Start();

            // set timer event to start reading and updating from the controller
         timer1.Start();
        }

        /// <summary>
        /// Event that is triggerd periodically. Currently the refresh is set to 10 Hz. This can be changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
         Thread demoThread = new Thread(new ThreadStart(this.ThreadProcSafe));
         demoThread.Start();

      }
      void GimbalTracking(int xError, int yError)
      {
         //640 height
         //480 width
         xError = (xError - 240)/45;
         yError = (yError - 320)/45;
         if(gimbalY - yError < 100 && gimbalY - yError > 0)
            gimbalY -= yError;
         if (gimbalX - xError < 360 && gimbalX - xError > 0)
            gimbalX -= xError;
         GimbalPhi(gimbalX);
         GimbalTheta(gimbalY);
      }
      void SearchObject()
      {
         if(leftpan)
         {
            if(gimbalX - 1 >= 0)
               gimbalX -= 1; 
            else
            {
               leftpan = false;
               rightpan = true; 
            }
         }
         else if(rightpan)
         {
            if (gimbalY + 1 <= 360)
               gimbalY += 1;
            else
            {
               rightpan = false;
               leftpan = true; 
            }
         }
      }

      void TurrentServo(int turrentServo)
      {
         byte checkSum;
         byte[] turrentServoByte = Encoding.ASCII.GetBytes(turrentServo.ToString());
         List<byte> turrentServoList = turrentServoByte.ToList();  //  MSB = index0,  LSB = index1,                  
         if (turrentServoList.Count == 1)
         {
            turrentServoList.Insert(0, 0x30);                     // add 0 in ascii to first item in list
            turrentServoList.Insert(0, 0x30);
         }
         else if (turrentServoList.Count == 2)
            turrentServoList.Insert(0, 0x30);
         byte[] _turrentServoPackage = new byte[] {
                0x01,                                   // Start of Transmission
                0x45,                                   // ID of Device to be controlled (ALPHABETIC)
                0x02,                                   // Start of Data (Parameters of Device)
                turrentServoList[0],           // MSB Digit in degrees  
                turrentServoList[1],           // Second Digit in degrees  ###### Check ORDER!!
                turrentServoList[2],           // LSB Third Digit in degrees
                0x03,                                   // End of Data
                0x00,                                   // Checksum = ~(ID + DATA) 1 BYTE!
                0x04                                    // End of Transmission
                };

         checkSum = (byte)(~(0x45 + turrentServoList[0] + turrentServoList[1] + turrentServoList[2]));

         _turrentServoPackage.SetValue(checkSum, 7);
         fpga.Send(_turrentServoPackage);
      }

      void ArmYSend(int armY)
      {
         byte checkSum;
         byte[] armYByte = Encoding.ASCII.GetBytes(armY.ToString());
         List<byte> armYList = armYByte.ToList();  //  MSB = index0,  LSB = index1,                  
         if (armYList.Count == 2)
            armYList.Insert(0, 0x30);                     // add 0 in ascii to first item in list 
         else if (armYList.Count == 1)
         {
            armYList.Insert(0, 0x30);                     // add 0 in ascii to first item in list 
            armYList.Insert(0, 0x30);                     // add 0 in ascii to first item in list 
         }
         byte[] _armYPackage = new byte[] {
                  0x01,                                   // Start of Transmission
                  0x43,                                   // ID of Device to be controlled (ALPHABETIC)
                  0x02,                                   // Start of Data (Parameters of Device)
                  armYList[0],           // MSB Digit in Milimeters  
                  armYList[1],           // Second Digit in Milimeters  ###### Check ORDER!!
                  armYList[2],           // LSB Third Digit in Milimeters
                  0x03,                                   // End of Data
                  0x00,                                   // Checksum = ~(ID + DATA) 1 BYTE!
                  0x04                                    // End of Transmission
                  };

         checkSum = (byte)(~(0x43 + armYList[0] + armYList[1] + armYList[2]));
         _armYPackage.SetValue(checkSum, 7);
         fpga.Send(_armYPackage);
      }

        void ArmXSend(int armX)
      {
         byte checkSum; 
         byte[] armXByte = Encoding.ASCII.GetBytes(armX.ToString());
         List<byte> armXList = armXByte.ToList();  //  MSB = index0,  LSB = index1,                  
         if (armXList.Count == 1)
         {
            armXList.Insert(0, 0x30);                     // add 0 in ascii to first item in list
            armXList.Insert(0, 0x30);
         }
         else if (armXList.Count == 2)
            armXList.Insert(0, 0x30);
         byte[] _armXPackage = new byte[] {
                0x01,                                   // Start of Transmission
                0x44,                                   // ID of Device to be controlled (ALPHABETIC)
                0x02,                                   // Start of Data (Parameters of Device)
                armXList[0],           // MSB Digit in Milimeters  
                armXList[1],           // Second Digit in Milimeters  ###### Check ORDER!!
                armXList[2],           // LSB Third Digit in Milimeters 
                0x03,                                   // End of Data
                0x00,                                   // Checksum = ~(ID + DATA) 1 BYTE!
                0x04                                    // End of Transmission
                };

         checkSum = (byte)(~(0x44 + armXList[0] + armXList[1] + armXList[2]));
         
         _armXPackage.SetValue(checkSum, 7);
         fpga.Send(_armXPackage);
      }
      
      void GimbalPhi(int gimbalPhi)
      {
         byte checkSum; 
         byte[] gimbalPhiByte = Encoding.ASCII.GetBytes(gimbalPhi.ToString());
         List<byte> gimbalPhiList = gimbalPhiByte.ToList();  //  MSB = index0,  LSB = index2,                  
         if (gimbalPhiList.Count == 2)
            gimbalPhiList.Insert(0, 0x30);                     // add 0 in ascii to first item in list 
         else if (gimbalPhiList.Count == 1)
         {
            gimbalPhiList.Insert(0, 0x30);                     // add 0 in ascii to first item in list 
            gimbalPhiList.Insert(0, 0x30);                     // add 0 in ascii to first item in list 
         }
         byte[] _gimbalPhiPackage = new byte[] {
                0x01,                                   // Start of Transmission
                0x48,                                   // ID of Device to be controlled (ALPHABETIC)
                0x02,                                   // Start of Data (Parameters of Device)
                gimbalPhiList[0],           // MSB Digit in degrees  
                gimbalPhiList[1],           // Second Digit in degrees  ###### Check ORDER!!
                gimbalPhiList[2],           // LSB Third Digit in degrees 
                0x03,                                   // End of Data
                0x00,                                   // Checksum = ~(ID + DATA) 1 BYTE!
                0x04                                    // End of Transmission
                };

         checkSum = (byte)(~(0x48 + gimbalPhiList[0] + gimbalPhiList[1] + gimbalPhiList[2]));
         _gimbalPhiPackage.SetValue(checkSum, 7);
         fpga.Send(_gimbalPhiPackage);
      }
      void GimbalTheta(int GimbalTheta)
      {
         byte checkSum;
         byte[] gimbalThetaByte = Encoding.ASCII.GetBytes(GimbalTheta.ToString());
         List<byte> gimbalThetaList = gimbalThetaByte.ToList();  //  MSB = index0,  LSB = index2,                  
         if (gimbalThetaList.Count == 2)
            gimbalThetaList.Insert(0, 0x30);                     // add 0 in ascii to first item in list 
         else if (gimbalThetaList.Count == 1)
         {
            gimbalThetaList.Insert(0, 0x30);                     // add 0 in ascii to first item in list 
            gimbalThetaList.Insert(0, 0x30);                     // add 0 in ascii to first item in list 
         }
         byte[] _gimbalThetaPackage = new byte[] {
                0x01,                                   // Start of Transmission
                0x47,                                   // ID of Device to be controlled (ALPHABETIC)
                0x02,                                   // Start of Data (Parameters of Device)
                gimbalThetaList[0],           // MSB Digit in degrees  
                gimbalThetaList[1],           // Second Digit in degrees  ###### Check ORDER!!
                gimbalThetaList[2],           // LSB Third Digit in degrees 
                0x03,                                   // End of Data
                0x00,                                   // Checksum = ~(ID + DATA) 1 BYTE!
                0x04                                    // End of Transmission
                };

         checkSum = (byte)(~(0x47 + gimbalThetaList[0] + gimbalThetaList[1] + gimbalThetaList[2]));
         _gimbalThetaPackage.SetValue(checkSum, 7);
         fpga.Send(_gimbalThetaPackage);
      }

      private void AutoGrab_Click(object sender, EventArgs e)//############ Next thing to test
      {
         int[] RobotArmDirections = { 0, 0, 0 };
         if(firstreset)
         {
            PackageRecieved = false;
            LidarRecieve();
            while (!PackageRecieved) ;// Maybe create a limit for how long it waits 
            PackageRecieved = false;
            RobotArmDirections =AutonomousRetrieval(gimbalX, gimbalY, LidarDistance);
            if ((RobotArmDirections[1] >= 0 && RobotArmDirections[1] <= 270) || (RobotArmDirections[2] >= 0 && RobotArmDirections[2] <= 120) || (RobotArmDirections[0] >= 0 && RobotArmDirections[0] <= 203))
            {
               TurrentServo(RobotArmDirections[1]);
               ArmXSend(RobotArmDirections[2]);
               ArmYSend(RobotArmDirections[0]);
            }
         }
      }

      private void lblBase_pp_Click(object sender, EventArgs e)
      {

      }

      private void lblElbow_pp_Click(object sender, EventArgs e)
      {

      }

      private void lblShoulder_pp_Click(object sender, EventArgs e)
      {

      }

      private void lblGripper_pp_Click(object sender, EventArgs e)
      {

      }

      void GripperControl(bool gripper)
      {
         byte checkSum;
         int grippervalue = gripper ? 1 : 0; // converts to value from boolean 
         byte[] gripperByte = Encoding.ASCII.GetBytes(grippervalue.ToString());
         List<byte> gripperList = gripperByte.ToList();
         byte[] _gripperPackage = new byte[] {
                0x01,                                   // Start of Transmission
                0x46,                                   // ID of Device to be controlled (ALPHABETIC)
                0x02,                                   // Start of Data (Parameters of Device)
                0x30,           // 00  
                0x30,           // 00
                gripperList[0],           // Boolean of if gripper is open or closed 
                0x03,                                   // End of Data
                0x00,                                   // Checksum = ~(ID + DATA) 1 BYTE!
                0x04                                    // End of Transmission
                };

         checkSum = (byte)(~(0x46 + gripperList[0] + 0x30 + 0x30));

         _gripperPackage.SetValue(checkSum, 7);
         fpga.Send(_gripperPackage);
         lblGripper_pp.Text = gripper.ToString();
      }
      void ArmReset(bool armReset)
      {
         byte checkSum;
         int ResetValue = armReset ? 1 : 0; // converts to value from boolean 
         byte[] armResetByte = Encoding.ASCII.GetBytes(ResetValue.ToString());
         List<byte> armResetList = armResetByte.ToList();
         byte[] _armResetPackage = new byte[] {
                0x01,                                   // Start of Transmission
                0x4A,                                   // ID of Device to be controlled (ALPHABETIC)
                0x02,                                   // Start of Data (Parameters of Device)
                0x30,           // 00  
                0x30,           // 00
                0x31,           // Boolean of if gripper is open or closed 
                0x03,                                   // End of Data
                0x00,                                   // Checksum = ~(ID + DATA) 1 BYTE!
                0x04                                    // End of Transmission
                };
         checkSum = unchecked((byte)(~(0x4A + armResetList[0]+0x30+0x30)));
         _armResetPackage.SetValue(checkSum, 7);
         fpga.Send(_armResetPackage);
      }

    

      int[] AutonomousRetrieval(int gimbalX, int gimbalY,int RangeFinderDistance)
        {
            gimbalX = gimbalX - 180;
            gimbalY = gimbalY +40;
            double RangeFinderDistanceMM = RangeFinderDistance * 10; 
            double GimbalHeight = 457.2;
            double DifferenceofGimbalHeightToRobotArm = 203.2;
            double DistanceFromGimbalToRobotArm = 215.9;
            //initial calculations
            double DistanceFromPayload = Math.Sin(gimbalY * Math.PI / 180) * RangeFinderDistanceMM;
            double zCoord = GimbalHeight - Math.Cos(gimbalY * Math.PI / 180) * RangeFinderDistanceMM;
            double xCoord = DistanceFromPayload * Math.Cos(gimbalX * Math.PI / 180);
            double yCoord = DistanceFromPayload * Math.Sin(gimbalX * Math.PI / 180);
            //Translation
            xCoord = xCoord - DistanceFromGimbalToRobotArm;
            zCoord = zCoord - DifferenceofGimbalHeightToRobotArm;
            //Robot arm calculations 
            int RobotArmHeight = (int)Math.Abs(zCoord);
            int RobotArmAngle = (int)(Math.Atan2(yCoord, xCoord) * 180 / Math.PI);
            int RobotArmDistance = (int)Math.Sqrt(xCoord * xCoord + yCoord * yCoord);
            int[] Array = { RobotArmHeight, RobotArmAngle, RobotArmDistance };
            return Array; 
        }

      private void Tracking_Click(object sender, EventArgs e)
      {
         track = !track; 
      }

      void LidarRecieve()
        {
            byte checkSum = 0x00; 
            byte[] _LidarRecievePackage = new byte[] {
                0x01,                                   // Start of Transmission
                0x4B,                                   // ID of Device to be controlled (ALPHABETIC)
                0x02,                                   // Start of Data (Parameters of Device)
                0x30,           // MSB Digit in Milimeters  
                0x30,           // Second Digit in Milimeters  ###### Check ORDER!!
                0x30,           // LSB Third Digit in Milimeters
                0x03,                                   // End of Data
                0x00,                                   // Checksum = ~(ID + DATA) 1 BYTE!
                0x04                                    // End of Transmission
                };

            checkSum = unchecked ((byte)(~(0x4B+0x30+0x30+0x30)));
            _LidarRecievePackage.SetValue(checkSum, 7);
            fpga.Send(_LidarRecievePackage);
        }
      void SendArmControl(int armX, int armY, int turrentServo, bool gripper, int gimbalTheta, int gimbalPhi, bool armReset)
      {
         byte checkSum = 0x00;
         //Gripper Distance Conversion 
         byte[] armXByte = Encoding.ASCII.GetBytes(armX.ToString());
         List<byte> armXList = armXByte.ToList();  //  MSB = index0,  LSB = index1,                  
         if (armXList.Count == 1)
         {
            armXList.Insert(0, 0x30);                     // add 0 in ascii to first item in list
            armXList.Insert(0, 0x30);
         }
         else if (armXList.Count == 2)
            armXList.Insert(0, 0x30);
         //Gripper Height Conversion 
         byte[] armYByte = Encoding.ASCII.GetBytes(armY.ToString());
         List<byte> armYList = armYByte.ToList();  //  MSB = index0,  LSB = index1,                  
         if (armYList.Count == 2)
            armYList.Insert(0, 0x30);                     // add 0 in ascii to first item in list 
         else if (armYList.Count == 1)
         {
            armYList.Insert(0, 0x30);                     // add 0 in ascii to first item in list 
            armYList.Insert(0, 0x30);                     // add 0 in ascii to first item in list 
         }
         //Turrent Angle Conversion 
         byte[] turrentServoByte = Encoding.ASCII.GetBytes(turrentServo.ToString());
         List<byte> turrentServoList = armXByte.ToList();  //  MSB = index0,  LSB = index1,                  
         if (turrentServoList.Count == 1)
         {
            turrentServoList.Insert(0, 0x30);                     // add 0 in ascii to first item in list
            turrentServoList.Insert(0, 0x30);
         }
         else if (turrentServoList.Count == 2)
            turrentServoList.Insert(0, 0x30);

         //Gripper Servo Converison  
         int grippervalue = gripper ? 1 : 0; // converts to value from boolean 
         byte[] gripperByte = Encoding.ASCII.GetBytes(grippervalue.ToString());
         List<byte> gripperList = gripperByte.ToList();
         //Arm Reset Conversion
         int ResetValue = armReset ? 1 : 0; // converts to value from boolean 
         byte[] armResetByte = Encoding.ASCII.GetBytes(ResetValue.ToString());
         List<byte> armResetList = armResetByte.ToList();
         //Gimbal Theta
         byte[] gimbalThetaByte = Encoding.ASCII.GetBytes(gimbalTheta.ToString());
         List<byte> gimbalThetaList = gimbalThetaByte.ToList();  //  MSB = index0,  LSB = index2,                  
         if (gimbalThetaList.Count == 2)
            gimbalThetaList.Insert(0, 0x30);                     // add 0 in ascii to first item in list 
         else if (gimbalThetaList.Count == 1)
         {
            gimbalThetaList.Insert(0, 0x30);                     // add 0 in ascii to first item in list 
            gimbalThetaList.Insert(0, 0x30);                     // add 0 in ascii to first item in list 
         }
         //Gimbal Phi 
         byte[] gimbalPhiByte = Encoding.ASCII.GetBytes(gimbalPhi.ToString());
         List<byte> gimbalPhiList = gimbalThetaByte.ToList();  //  MSB = index0,  LSB = index2,                  
         if (gimbalPhiList.Count == 2)
            gimbalPhiList.Insert(0, 0x30);                     // add 0 in ascii to first item in list 
         else if (gimbalPhiList.Count == 1)
         {
            gimbalPhiList.Insert(0, 0x30);                     // add 0 in ascii to first item in list 
            gimbalPhiList.Insert(0, 0x30);                     // add 0 in ascii to first item in list 
         }
         //Gripper Distance Packet
         byte[] _armXPackage = new byte[] {
                0x01,                                   // Start of Transmission
                0x44,                                   // ID of Device to be controlled (ALPHABETIC)
                0x02,                                   // Start of Data (Parameters of Device)
                armXList[0],           // MSB Digit in Milimeters  
                armXList[1],           // Second Digit in Milimeters  ###### Check ORDER!!
                armXList[2],           // LSB Third Digit in Milimeters 
                0x03,                                   // End of Data
                0x00,                                   // Checksum = ~(ID + DATA) 1 BYTE!
                0x04                                    // End of Transmission
                };

         checkSum = (byte)(~(0x44 + armXList[0] + armXList[1] + armXList[2]));

         _armXPackage.SetValue(checkSum, 7);
         //Gripper Height package 
         byte[] _armYPackage = new byte[] {
                0x01,                                   // Start of Transmission
                0x43,                                   // ID of Device to be controlled (ALPHABETIC)
                0x02,                                   // Start of Data (Parameters of Device)
                armYList[0],           // MSB Digit in Milimeters  
                armYList[1],           // Second Digit in Milimeters  ###### Check ORDER!!
                armYList[2],           // LSB Third Digit in Milimeters
                0x03,                                   // End of Data
                0x00,                                   // Checksum = ~(ID + DATA) 1 BYTE!
                0x04                                    // End of Transmission
                };

         checkSum = (byte)(~(0x43 + armYList[0] + armYList[1] + armYList[2]));

         _armYPackage.SetValue(checkSum, 7);
         //Turrent Servo Package 
         byte[] _turrentServoPackage = new byte[] {
                0x01,                                   // Start of Transmission
                0x45,                                   // ID of Device to be controlled (ALPHABETIC)
                0x02,                                   // Start of Data (Parameters of Device)
                turrentServoList[0],           // MSB Digit in degrees  
                turrentServoList[1],           // Second Digit in degrees  ###### Check ORDER!!
                turrentServoList[2],           // LSB Third Digit in degrees
                0x03,                                   // End of Data
                0x00,                                   // Checksum = ~(ID + DATA) 1 BYTE!
                0x04                                    // End of Transmission
                };

         checkSum = (byte)(~(0x45 + turrentServoList[0] + turrentServoList[1] + turrentServoList[2]));

         _turrentServoPackage.SetValue(checkSum, 7);
         //Gripper Servo
         byte[] _gripperPackage = new byte[] {
                0x01,                                   // Start of Transmission
                0x46,                                   // ID of Device to be controlled (ALPHABETIC)
                0x02,                                   // Start of Data (Parameters of Device)
                0x30,           // 00  
                0x30,           // 00
                gripperList[0],           // Boolean of if gripper is open or closed 
                0x03,                                   // End of Data
                0x00,                                   // Checksum = ~(ID + DATA) 1 BYTE!
                0x04                                    // End of Transmission
                };

         checkSum = (byte)(~(0x46 + gripperList[0]));

         _gripperPackage.SetValue(checkSum, 7);
         //Gimbal Theta Packet 
         byte[] _gimbalThetaPackage = new byte[] {
                0x01,                                   // Start of Transmission
                0x47,                                   // ID of Device to be controlled (ALPHABETIC)
                0x02,                                   // Start of Data (Parameters of Device)
                gimbalThetaList[0],           // MSB Digit in degrees  
                gimbalThetaList[1],           // Second Digit in degrees  ###### Check ORDER!!
                gimbalThetaList[2],           // LSB Third Digit in degrees 
                0x03,                                   // End of Data
                0x00,                                   // Checksum = ~(ID + DATA) 1 BYTE!
                0x04                                    // End of Transmission
                };

         checkSum = (byte)(~(0x47 + gimbalThetaList[0] + gimbalThetaList[1] + gimbalThetaList[2]));
         _gimbalThetaPackage.SetValue(checkSum, 7);
         //Gimbal Phi Package 
         byte[] _gimbalPhiPackage = new byte[] {
                0x01,                                   // Start of Transmission
                0x48,                                   // ID of Device to be controlled (ALPHABETIC)
                0x02,                                   // Start of Data (Parameters of Device)
                gimbalPhiList[0],           // MSB Digit in degrees  
                gimbalPhiList[1],           // Second Digit in degrees  ###### Check ORDER!!
                gimbalPhiList[2],           // LSB Third Digit in degrees 
                0x03,                                   // End of Data
                0x00,                                   // Checksum = ~(ID + DATA) 1 BYTE!
                0x04                                    // End of Transmission
                };

         checkSum = (byte)(~(0x48 + gimbalPhiList[0] + gimbalPhiList[1] + gimbalPhiList[2]));
         _gimbalPhiPackage.SetValue(checkSum, 7);
         //Arm Reset Package
         byte[] _armResetPackage = new byte[] {
                0x01,                                   // Start of Transmission
                0x50,                                   // ID of Device to be controlled (ALPHABETIC)
                0x02,                                   // Start of Data (Parameters of Device)
                0x30,           // 00  
                0x30,           // 00
                armResetList[0],           // Boolean of if gripper is open or closed 
                0x03,                                   // End of Data
                0x00,                                   // Checksum = ~(ID + DATA) 1 BYTE!
                0x04                                    // End of Transmission
                };

         checkSum = (byte)(~(0x50 + armResetList[0]));
         _armResetPackage.SetValue(checkSum, 7);
         //Send packages to FPGA
         fpga.Send(_armXPackage);
         fpga.Send(_armYPackage);
         fpga.Send(_turrentServoPackage);
         fpga.Send(_gripperPackage);
         fpga.Send(_gimbalThetaPackage);
         fpga.Send(_gimbalPhiPackage);

      }
      private void btnTestThread_Click(object sender, EventArgs e)
      {

      }

      // This method is executed on the worker thread and makes 
      // a thread-safe call on the TextBox control. 
      private void ThreadProcSafe()
      {
         ThreadHelperClass.SetText(this, lblGripper_pp, gripper.ToString());
         ThreadHelperClass.SetText(this, lblElbow_pp, armX.ToString());
         ThreadHelperClass.SetText(this, lblShoulder_pp, armY.ToString());
         ThreadHelperClass.SetText(this, lblBase_pp, turretServo.ToString());
         ThreadHelperClass.SetText(this, EncoderLeft, LeftEnconder.ToString());
         ThreadHelperClass.SetText(this, EncoderRight, RightEncoder.ToString());
         ThreadHelperClass.SetText(this, LidarData, LidarDistance.ToString());
         ThreadHelperClass.SetText(this, RightError, xDir.ToString());
         ThreadHelperClass.SetText(this, LeftError, yDir.ToString());
         ThreadHelperClass.SetText(this, Ygimbal, gimbalX.ToString());
         ThreadHelperClass.SetText(this, Xgimbal, gimbalY.ToString());
      }

   }

   public static class ThreadHelperClass
   {
      delegate void SetTextCallback(Form f, Control ctrl, string text);
      /// <summary>
      /// Set text property of various controls
      /// </summary>
      /// <param name="form">The calling form</param>
      /// <param name="ctrl"></param>
      /// <param name="text"></param>
      public static void SetText(Form form, Control ctrl, string text)
      {
         // InvokeRequired required compares the thread ID of the 
         // calling thread to the thread ID of the creating thread. 
         // If these threads are different, it returns true. 
         if (ctrl.InvokeRequired)
         {
            SetTextCallback d = new SetTextCallback(SetText);
            form.Invoke(d, new object[] { form, ctrl, text });
         }
         else
         {
            ctrl.Text = text;
         }
      }
   }

}       
