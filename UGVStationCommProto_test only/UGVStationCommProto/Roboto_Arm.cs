using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGV.Core.IO;
using dynamixel_sdk;

namespace NGCP.UGV
{
    public class Roboto_Arm
    {
        #region Constant Properties
        private const Int16 Base_ID = 11;
        private const Int16 Shoulder_ID = 12;
        private const Int16 Elbow_ID = 13;
        private const Int16 Gripper_ID = 14;


        // Default setting


        private const Int16 Base_Search_Position = 2200;                  // start position of the Base servo
        private const Int16 Shoulder_Search_Position = 2300;               // start position of the Shoulder servo
        private const Int16 Elbow_Search_Position = 800;                  // start position of the Elbow servo
        private int BAUDRATE = 8000;
        private string DEVICENAME = "COM11";                               // Check which port is being used on your controller
        private int port_num = 0;                                                   // the port of the USB connected to the computer

        #endregion

        #region Changing Properties
        public bool hasPortBeenOpened = false;                  // whether the port for the dynamixel has been opened so other servos dont attempt to open again
                                                                // the port of the USB connected to the computer

        #endregion

        #region DXL Servos and PIDs
        public DXL_Servo Base { get; set; }
        public DXL_Servo Shoulder { get; set; }
        public DXL_Servo Elbow { get; set; }
        public DXL_Servo Gripper { get; set; }

        public PID_Contoller basePID;
        public PID_Contoller elbowPID;


        #endregion

        #region Constructors

        public static UdpClientSocket udpcam { get; private set; }
        System.Timers.Timer controlTimer;
        /// <summary>
        /// Timer for system state board casting
        /// </summary>
        System.Timers.Timer boardcastTimer;

        #endregion

        #region Contructors

        public Roboto_Arm()
        {
            Base = new DXL_Servo(Base_ID, -1500, 1500, ref hasPortBeenOpened);
            Shoulder = new DXL_Servo(Shoulder_ID, 14000, ref hasPortBeenOpened);
            Elbow = new DXL_Servo(Elbow_ID, -11000, ref hasPortBeenOpened);
            Gripper = new DXL_Servo(Gripper_ID, -6000, 17000, ref hasPortBeenOpened);
            //basePID = new PID_Contoller(2.7, 0, 0.00255, 200, -200);       // assuming 1000x1000 pixel frame with goal center position at 500,500
            //elbowPID = new PID_Contoller(1, 0, 0, Elbow.DXL_MAXIMUM_POSITION_VALUE, Elbow.DXL_MINIMUM_POSITION_VALUE);                
        }
        /// <summary>
        /// this is the main constructor used to allow the ugv vehicle to start the arm setup
        /// </summary>
        /// <param name="comport_param"></param>
        /// <param name="baudrate_param"></param>
        public Roboto_Arm(String comport_param, int baudrate_param)
        {
            DEVICENAME = comport_param;
            BAUDRATE = baudrate_param;
            Base = new DXL_Servo(Base_ID, -1500, 1500);
            Shoulder = new DXL_Servo(Shoulder_ID, 14000);
            Elbow = new DXL_Servo(Elbow_ID, -11000);
            Gripper = new DXL_Servo(Gripper_ID, -5000, 10000);
        }
        #endregion Constructors

        #region Methods
        public void StartArm()
        {            // Initialize PortHandler Structs
            // Set the port path
            // Get methods and members of PortHandlerLinux or PortHandlerWindows
            port_num = dynamixel.portHandler(DEVICENAME);

            // Initialize PacketHandler Structs
            dynamixel.packetHandler();

            // Open port
            if (dynamixel.openPort(port_num))
            {
                Console.WriteLine("Succeeded to open the port!");
            }
            else
            {
                Console.WriteLine("Failed to open the port!");
                Console.WriteLine("Press any key to terminate...");
                Console.ReadKey();
                return;
            }

            // Set port baudrate
            if (dynamixel.setBaudRate(port_num, BAUDRATE))
            {
                Console.WriteLine("Succeeded to change the baudrate!");
            }
            else
            {
                Console.WriteLine("Failed to change the baudrate!");
                Console.WriteLine("Press any key to terminate...");
                Console.ReadKey();
                return;
            }

            // enable the turret, shoulder, elbow and base
            Base.StartServo(port_num);
            Shoulder.StartServo(port_num);
            Elbow.StartServo(port_num);
            Gripper.StartServo(port_num);
        }



        /// <summary>
        /// Converts the integer value of the servo to degrees
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static short convertFromDegrees(int degree)
        {
            return (short)(degree / .088);
        }

        /// <summary>
        /// The values to write to each servo to move the arm of the UGV to a postion that allows for optimal view of the main payload
        /// </summary>
        public void ArmSearchPosition()
        {
            Base.Move_To(Base_Search_Position);
            Shoulder.Move_To(Shoulder_Search_Position);
            Elbow.Move_To(Elbow_Search_Position);
        }

        /// <summary>
        /// Method to close the gripper in order to grab the main payload
        /// </summary>
        public void CloseClaw()
        {
            //code for closing claw and retracting arm to UGV
            int writeVal = (int)(Gripper.DXL_MAXIMUM_POSITION_VALUE * 0.8);
            Gripper.Move_To(writeVal);
        }
        #endregion Methods
    }
    }
