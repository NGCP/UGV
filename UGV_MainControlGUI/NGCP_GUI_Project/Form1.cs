using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NGCP.UGV;

namespace NGCP_GUI_Project
{
    public partial class Form1 : Form
    {
        private const int UGVStationNodeID = 100;
        private const string commPort = "COM5";
        private const int commBaud = 57600;
        private const byte turretServo_id = 0x45, armX_id = 0x44, armY_id = 0x43, gripper_id = 0x46;



        public Form1()
        {
            InitializeComponent();
            LocalMode_radioButton1.CheckedChanged += new EventHandler(radioButtons_CheckedChanged);
            SemiAutoMode_radioButton2.CheckedChanged += new EventHandler(radioButtons_CheckedChanged);
            AutoMode_radioButton3.CheckedChanged += new EventHandler(radioButtons_CheckedChanged);
        }

        static CommProtocol commProtocol;

        private void Form1_Load(object sender, EventArgs e)
        {

            commProtocol = new CommProtocol(UGVStationNodeID);
            commProtocol.initializeConnection(commPort, 57600);
            // Add UGV destination node and MAC address
            commProtocol.addAddress(7, "0013A20040A54318");

            //link call backs
            commProtocol.LinkCallback(new NGCP.VehicleWaypointCommand(), new Comnet.CallBack(VehicleWaypointCommandCallback));
            commProtocol.LinkCallback(new NGCP.VehicleModeCommand(), new Comnet.CallBack(VehicleModeCommandCallback));
            commProtocol.LinkCallback(new NGCP.ArmCommand(), new Comnet.CallBack(ArmCommandCallback));
            commProtocol.LinkCallback(new NGCP.VehicleSystemStatus(), new Comnet.CallBack(VehicleSystemStatusCallback));
            commProtocol.LinkCallback(new NGCP.VehicleGlobalPosition(), new Comnet.CallBack(VehicleGlobalPositionCallback));
            commProtocol.LinkCallback(new NGCP.SpeedSteeringCommand(), new Comnet.CallBack(SpeedSteeringCommandCallback));

            commProtocol.start();
       
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //capture up arrow key
            if (keyData == Keys.Up)
            {
                if (Speed_Bar.Value + 50 <= 2000)
                    Speed_Bar.Value = Speed_Bar.Value + 50;
                commProtocol.SendSpeedSteeringCommand((ushort)Speed_Bar.Value, (ushort)Turning_Bar.Value);

                return true;
            }
            //capture down arrow key
            if (keyData == Keys.Down)
            {
                if (Speed_Bar.Value - 50 >= 0)
                    Speed_Bar.Value = Speed_Bar.Value - 50;
                commProtocol.SendSpeedSteeringCommand((ushort)Speed_Bar.Value, (ushort)Turning_Bar.Value);

                return true;
            }
            //capture left arrow key
            if (keyData == Keys.Left)
            {
                if (Turning_Bar.Value - 1 >= 0)
                    Turning_Bar.Value = Turning_Bar.Value - 1;
                commProtocol.SendSpeedSteeringCommand((ushort)Speed_Bar.Value, (ushort)Turning_Bar.Value);

                return true;
            }
            //capture right arrow key
            if (keyData == Keys.Right)
            {
                if (Turning_Bar.Value + 1 <= 54)
                    Turning_Bar.Value = Turning_Bar.Value + 1;
                commProtocol.SendSpeedSteeringCommand((ushort)Speed_Bar.Value, (ushort)Turning_Bar.Value);

                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void radioButtons_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            if (LocalMode_radioButton1.Checked)
            {
                //Drive_Type_Box.Text = LocalMode_radioButton1.Text;
                commProtocol.sendMode(DriveMode.LocalControl, 7);            
            }
            else if (SemiAutoMode_radioButton2.Checked)
            {
                //Drive_Type_Box.Text = SemiAutoMode_radioButton2.Text;
                commProtocol.sendMode(DriveMode.SemiAutonomous, 7);
            }
            else if (AutoMode_radioButton3.Checked)
            {
                //Driving_Type_Box.Text = AutoMode_radioButton3.Text;
                commProtocol.sendMode(DriveMode.Autonomous, 7);
            }
        }

       

        private void button1_Click(object sender, EventArgs e)
        {
            Speed_Bar.Value = 1000;
            Turning_Bar.Value = 27;
            commProtocol.SendSpeedSteeringCommand((ushort)Speed_Bar.Value, (ushort)Turning_Bar.Value);
        }

        private void Speed_Bar_ValueChanged(object sender, EventArgs e)
        {
            

            Int16 number = Convert.ToInt16(Speed_Bar.Value);

        }

        private void Turning_Bar_ValueChanged(object sender, EventArgs e)
        {

        }

        private void Base_Servo_Bar_ValueChanged(object sender, EventArgs e)
        {
           commProtocol.SendArmCommand(turretServo_id, Base_Servo_Bar.Value, 7);
        }

        private void Arm_X_Servo_Bar_ValueChanged(object sender, EventArgs e)
        {
            commProtocol.SendArmCommand(armX_id, Arm_X_Servo_Bar.Value, 7);
        }



        private void Arm_Y_Servo_Bar_ValueChanged(object sender, EventArgs e)
        {
            commProtocol.SendArmCommand(armY_id, 360-Arm_Y_Servo_Bar.Value, 7);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            System_State_Box.Text = System_State_comboBox.Text;
            //Use System_State_comboBox.SelectedIndex to get the value of the system state
            //MessageBox.Show(System_State_comboBox.SelectedIndex.ToString()); 
            System_State_Label.Text = System_State_comboBox.SelectedIndex.ToString();
            commProtocol.sendDriveState((DriveState)System_State_comboBox.SelectedIndex, 7);


        }


        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (GripperRetracted_radioButton4.Checked == true)      //retracted
            {
                commProtocol.SendArmCommand(gripper_id, 0, 7);
                Int16 number = Convert.ToInt16(0);

            }
            else if (GripperClosed_radioButton5.Checked == true)   //closed
            {
                commProtocol.SendArmCommand(gripper_id, 1, 7);
                Int16 number = Convert.ToInt16(1);

            }
        }




        private void button2_Click(object sender, EventArgs e)
        {
            List<byte> escapeBytes = new List<byte> { 253, 254, 255 };

            double GPS_X = Convert.ToDouble(GPS_X_Box.Text);
            double GPS_Y = Convert.ToDouble(GPS_Y_Box.Text);
            //commProtocol.SendState(new UGVState(), 7);
            commProtocol.VehicleWaypointCommand(GPS_Y, GPS_X, 7);

        }

        #region Commprotocol Callbacks
        ///<sumary>
        ///Commprotocol callbacks
        ///</sumary>    
        static public int VehicleWaypointCommandCallback(Comnet.Header header, Comnet.ABSPacket packet, Comnet.CommNode node)
        {
            //validate who the packet is from 1 is gcs
            if (header.GetSourceID() == 1)
            {
                NGCP.VehicleWaypointCommand twc = Comnet.ABSPacket.GetValue<NGCP.VehicleWaypointCommand>(packet);
                //WayPoint commandWayPoint = new WayPoint(twc.latitude, twc.longitude, 1);

                //using a queue but want command waypoint in front :(
                //maybe want to switch this data type
                //Waypoints.Enqueue(commandWayPoint);

            }

            //make sure you return this way to declare succes and destory the pointer(c++)
            return (Int32)(Comnet.CallBackCodes.CALLBACK_SUCCESS | Comnet.CallBackCodes.CALLBACK_DESTROY_PACKET);
        }
        public int VehicleModeCommandCallback(Comnet.Header header, Comnet.ABSPacket packet, Comnet.CommNode node)
        {
            //validate who the packet is from 1 is gcs
            if (header.GetSourceID() == 1)
            {
                NGCP.VehicleModeCommand twc = Comnet.ABSPacket.GetValue<NGCP.VehicleModeCommand>(packet);
                if ((DriveMode)twc.vehicle_mode == DriveMode.LocalControl)
                {
                    Drive_Type_Box.Text = "Local Mode";
                }
                else if ((DriveMode)twc.vehicle_mode == DriveMode.SemiAutonomous)
                {
                    Drive_Type_Box.Text = "Semi-Auto Mode";
                }
                else if ((DriveMode)twc.vehicle_mode == DriveMode.Autonomous)
                {
                    Drive_Type_Box.Text = "Auto Mode";
                }

                //Settings.DriveMode = (DriveMode)twc.vehicle_mode;
            }
            else if (header.GetSourceID() == 7)
            {
                NGCP.VehicleModeCommand vmc = Comnet.ABSPacket.GetValue<NGCP.VehicleModeCommand>(packet);
                if ((DriveMode)vmc.vehicle_mode == DriveMode.LocalControl)
                {
                    Drive_Type_Box.Text = "Local Mode";
                }
                else if ((DriveMode)vmc.vehicle_mode == DriveMode.SemiAutonomous)
                {
                    Drive_Type_Box.Text = "Semi-Auto Mode";
                }
                else if ((DriveMode)vmc.vehicle_mode == DriveMode.Autonomous)
                {
                    Drive_Type_Box.Text = "Auto Mode";
                }
            }

                //make sure you return this way to declare succes and destory the pointer(c++)
                return (Int32)(Comnet.CallBackCodes.CALLBACK_SUCCESS | Comnet.CallBackCodes.CALLBACK_DESTROY_PACKET);
        }

        /*static public int ArmCommandCallback(Comnet.Header header, Comnet.ABSPacket packet, Comnet.CommNode node)
        {
            NGCP.ArmCommand ac = Comnet.ABSPacket.GetValue<NGCP.ArmCommand>(packet);
            //do some kind of switch case
            //@TODO ARM
            int servoPosition = ac.id;
            int servoValue = ac.position;
            //id_servo_GUI = ac.id;
            //val_servo_GUI = ac.position;



            return (Int32)(Comnet.CallBackCodes.CALLBACK_SUCCESS | Comnet.CallBackCodes.CALLBACK_DESTROY_PACKET);


        }*/


        static public int VehicleSystemStatusCallback(Comnet.Header header, Comnet.ABSPacket packet, Comnet.CommNode node)
        {
            NGCP.VehicleSystemStatus vss = Comnet.ABSPacket.GetValue<NGCP.VehicleSystemStatus>(packet);
            //State = (UGV.DriveState)vss.vehicle_state;
            //make sure you return this way to declare succes and destory the pointer(c++)
            return (Int32)(Comnet.CallBackCodes.CALLBACK_SUCCESS | Comnet.CallBackCodes.CALLBACK_DESTROY_PACKET);
        }

        
         public int VehicleGlobalPositionCallback(Comnet.Header header, Comnet.ABSPacket packet, Comnet.CommNode node)
        {
            //validate who the packet is from 1 is gcs
            if (header.GetSourceID() == 7)
            {
                NGCP.VehicleGlobalPosition vgp = Comnet.ABSPacket.GetValue<NGCP.VehicleGlobalPosition>(packet);
                GPS_X_Box.Text = vgp.latitude.ToString();
                GPS_Y_Box.Text = vgp.longitude.ToString();
            }

            //make sure you return this way to declare succes and destory the pointer(c++)
            return (Int32)(Comnet.CallBackCodes.CALLBACK_SUCCESS | Comnet.CallBackCodes.CALLBACK_DESTROY_PACKET);
        }
        public int ArmCommandCallback(Comnet.Header header, Comnet.ABSPacket packet, Comnet.CommNode node)
        {
            NGCP.ArmCommand ac = Comnet.ABSPacket.GetValue<NGCP.ArmCommand>(packet);

            //do some kind of switch case
            //@TODO ARM
            int armServo = ac.id;
            int servoValue = ac.position;

            //// this method 
            switch (armServo)
            {
                case armY_id: //Arm Stepper Y      
                    Arm_Y_Servo_Box.Text = (servoValue).ToString();
                    break;
                case armX_id: //Arm Stepper X
                    Arm_X_Servo_Box.Text = (servoValue).ToString();
                    break;
                case turretServo_id: //Turret Servo
                    Base_Servo_Box.Text = (servoValue).ToString();
                    break;
                case gripper_id: //Gripper
                    if (servoValue == 1)
                        Gripper_Servo_Box.Text = "Closed";
                    else
                        Gripper_Servo_Box.Text = "Retracted";
                    break;
                default:
                    // write to none of the servos, the id written was not recognized
                    break;
            }
            return (Int32)(Comnet.CallBackCodes.CALLBACK_SUCCESS | Comnet.CallBackCodes.CALLBACK_DESTROY_PACKET);
        }

        public int SpeedSteeringCommandCallback(Comnet.Header header, Comnet.ABSPacket packet, Comnet.CommNode node)
        {
            NGCP.SpeedSteeringCommand ssc = Comnet.ABSPacket.GetValue<NGCP.SpeedSteeringCommand>(packet);

            Speed_Box.Text = ((short)ssc.speed).ToString();
            Turning_Box.Text = ((short)ssc.steering * 27 / 1000).ToString();

            return (Int32)(Comnet.CallBackCodes.CALLBACK_SUCCESS | Comnet.CallBackCodes.CALLBACK_DESTROY_PACKET);
        }

        #endregion


        public enum DriveState
        {
            /// <summary>
            /// 1) Vehicle will search for target
            /// </summary>
            SearchTarget = 0,
            /// <summary>
            /// 2)Vehicle sees red ball so it will be guided by CV to go to ball
            /// </summary>
            GotoBall = 1,
            /// <summary>
            /// 3) Vehicle will lock target for a certain time
            /// </summary>
            LockTarget = 2,
            /// <summary>
            /// 4) Vehicle will search for the payload
            /// </summary>
            SearchPayload = 3,
            /// <summary>
            /// 5) Drive away till a certain distance of target
            /// </summary>
            DriveAwayFromTarget = 4,
            /// <summary>
            /// 6) Vehicle will drive back to a safe zone
            /// </summary>
            DriveToSafeZone = 5,
            /// <summary>
            /// 7) Vehicle will wait for GCS to confirm the drop
            /// </summary>
            WaitDrop = 6,
            /// <summary>
            /// 8) Vehicle will drive to dropped location and verify
            /// </summary>
            VerifyTarget = 7,
            /// <summary>
            /// 9) Vehicle will drive to start point
            /// </summary>
            DriveToStart = 8,
            /// <summary>
            /// 10) Generate search path if point of interest is not given
            /// </summary>
            GenerateSearchPath = 9,
            /// <summary>
            /// 11) Vehicle will idle and wait for command
            /// </summary>
            Idle = 10,
            /// <summary>
            /// 12) Manual ARM Control
            /// </summary>
            GrabPayloadManual = 11,


        }

        public enum DriveMode
        {
            /// <summary>
            /// Use Local control on speed and steering
            /// </summary>
            LocalControl,

            /// <summary>
            /// Use Local Speed and Autonomous Steering
            /// </summary>
            SemiAutonomous,

            /// <summary>
            /// Use Autonomous Speed and Steering
            /// </summary>
            Autonomous
        }

    }
}
