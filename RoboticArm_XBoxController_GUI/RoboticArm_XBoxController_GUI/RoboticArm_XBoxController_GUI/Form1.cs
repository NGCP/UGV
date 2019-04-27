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
using UGV.Core.IO;
using System.Threading;

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
        private int turretServo, armX, armY; 
        private bool gripper;



        private void trackBar_armX_ValueChanged(object sender, EventArgs e)
        {
            // send a packet to the UGV with the updated change 
            SendArmControl(armX, armY, turretServo, (bool)gripper);
        }


        private void radioButton_retracted_CheckedChanged(object sender, EventArgs e)
        {
            // send a packet to the UGV with the updated change values
            gripper = false;
            SendArmControl(armX, armY, turretServo, (bool)gripper);
        }

        private void trackBar_shoulder_ValueChanged(object sender, EventArgs e)
        {
            // send a packet to the UGV with the updated change values
            SendArmControl(armX, armY, turretServo, (bool)gripper);
        }

        private void radioButton_closed_CheckedChanged(object sender, EventArgs e)
        {
            // send a packet to the UGV with the updated change values
            gripper = true;
            SendArmControl(armX, armY, turretServo, (bool)gripper);
        }

        private void trackBar_base_ValueChanged(object sender, EventArgs e)
        {
            // send a packet to the UGV with the updated change values
            SendArmControl(armX, armY, turretServo, (bool)gripper);
        }

        /// <summary>
        /// The defined value each joystick coordinate must be passed in order to write any gain values.
        /// </summary>
        private const int Xthreshold = 30, Ythreshold = 30;
        Serial fpga = new Serial("COM7", 9600);  // use 9600 for FPGA, use 57600

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
                Console.WriteLine("\n");
            });
            //start                
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


        }



        //private void CalculateGains()
        //{
        //    // update class properties
        //    turretServo = xb1c.leftThumb.X;
        //    armX = xb1c.leftThumb.Y;
        //    armY = xb1c.rightThumb.Y;
        //    gripper = xb1c.rightThumb.X;

        //    // determine gains
        //    g_base = (int)(Math.Abs(turretServo) > Xthreshold ? 2.0 * turretServo : 0);
        //    g_shoulder = (int)(Math.Abs(armX) > Ythreshold ? 2.0 * armX : 0);
        //    g_elbow = (int)(Math.Abs(armY) > Ythreshold ? 2.5 * armY : 0);
        //    g_gripper = (int)(Math.Abs(gripper) > Xthreshold ? 4.0 * gripper : 0);
        //}


        void SendArmControl(int armX, int armY, int turretServo, bool gripper)
        {
            byte[] _motorPackage = new byte[] {
                0x01,                                   // Start of Transmission
                0x41,                                   // ID of Device to be controlled (ALPHABETIC)
                0x02,                                   // Start of Data (Parameters of Device)
                0x00,           // direction  ASCII '1-forward' or '0-backward'
                0x00,           // MSB - speed 0x-9x
                0x00,           // LSB - speed x0-x9
                0x03,                                   // End of Data
                0x00,                                   // Checksum = ~(ID + DATA) 1 BYTE!
                0x04                                    // End of Transmission
                };

            //checkSum = (byte)(~(0x41 + FrontWheelDirection + FrontWheelSpeedList[0] + FrontWheelSpeedList[1]));

            //_motorPackage.SetValue(checkSum, 7);


            fpga.Send(_motorPackage);
        }
    }
}       
