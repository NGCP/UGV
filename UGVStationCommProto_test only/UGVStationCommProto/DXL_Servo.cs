using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dynamixel_sdk;

namespace NGCP.UGV
{
    public class DXL_Servo
    {
        #region Properties
        // *** SDL Components *** //
        // Control table address
        private const int ADDR_MX_TORQUE_ENABLE = 24;                           // Control table address is different in Dynamixel model
        private const int ADDR_MX_GOAL_POSITION = 30;
        private const int ADDR_MX_PRESENT_POSITION = 36;

        // Protocol version
        private const int PROTOCOL_VERSION = 1;                                 // See which protocol version is used in the Dynamixel

        // Default setting
        private const int BAUDRATE = 8000;
        private const string DEVICENAME = "COM11";                               // Check which port is being used on your controller
        private const int TORQUE_ENABLE = 1;                                    // Value for enabling the torque
        private const int TORQUE_DISABLE = 0;                                   // Value for disabling the torque
        private const int DXL_MOVING_STATUS_THRESHOLD = 50;                     // Dynamixel moving status threshold

        private const byte ESC_ASCII_VALUE = 0x1b;

        private const int COMM_SUCCESS = 0;                                     // Communication Success result value
        private const int COMM_TX_FAIL = -1001;                                 // Communication Tx Failed

        // class properties
        private int DXL_ID { get; } = 1;                                            // address of the servo
        public int DXL_MINIMUM_POSITION_VALUE { get; private set; } = 100;          // Dynamixel is not to go below this value
        public int DXL_MAXIMUM_POSITION_VALUE { get; private set; } = 4000;         // Dynamixwl is not to go past this value
        public int dxl_present_position { get; private set; } = 0;                // the current position of the Dynamixel
        public int dxl_goal_position { get; private set; }
        private int port_num { get; set; } = 0;                                                   // the port of the USB connected to the computer
        private int dxl_comm_result = COMM_TX_FAIL;                                 // the return result of the servo for after given a command
        private byte dxl_error = 0;                                                 // the return result

        // servo id values for check when writing
        private const Int16 Base_ID = 11;
        private const Int16 Shoulder_ID = 12;
        private const Int16 Elbow_ID = 13;

        #endregion

        #region Constructors
        /// <summary>
        /// This constructor method allows the arm to setup the com port for communication.
        /// The minimimum and maximum values are defined as offsets from the read setup position.
        /// The setup position is read in a start method to update the max and min.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public DXL_Servo(Int16 id, int min, int max)
        {
            DXL_ID = id;
            DXL_MAXIMUM_POSITION_VALUE =  max;            // sign is taken care of as parameter
            DXL_MINIMUM_POSITION_VALUE =  min;            // sign is taken care of as parameter
        }
        /// <summary>
        /// This constructor method allows the arm to setup the com port for communication.
        /// The minimum is defined as the start position and the maximum as an offset from that position.
        /// The setup position is read in a start method to update the max and min.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="max"></param>
        public DXL_Servo(Int16 id, int max)
        {
            DXL_ID = id;
            DXL_MAXIMUM_POSITION_VALUE = max;
            DXL_MINIMUM_POSITION_VALUE = 0;
        }

        public DXL_Servo(Int16 id, int min, int max, ref bool isPortOpen)
        {
            DXL_ID = id;
            Setup_Port(ref isPortOpen);
            dxl_present_position = dynamixel.read2ByteTxRx(port_num, PROTOCOL_VERSION, (byte)DXL_ID, ADDR_MX_PRESENT_POSITION);
            DXL_MAXIMUM_POSITION_VALUE = dxl_present_position + max;            // sign is taken care of as parameter
            DXL_MINIMUM_POSITION_VALUE = dxl_present_position + min;            // sign is taken care of as parameter
        }

        public DXL_Servo(Int16 id, int max, ref bool isPortOpen)
        {
            DXL_ID = id;
            DXL_MAXIMUM_POSITION_VALUE = max;
            Setup_Port(ref isPortOpen);
            dxl_present_position = dynamixel.read2ByteTxRx(port_num, PROTOCOL_VERSION, (byte)DXL_ID, ADDR_MX_PRESENT_POSITION);
            DXL_MINIMUM_POSITION_VALUE = dxl_present_position;
            DXL_MAXIMUM_POSITION_VALUE += dxl_present_position;
        }

        ~DXL_Servo()
        {
            // Disable Dynamixel Torque
            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, (byte)DXL_ID, ADDR_MX_TORQUE_ENABLE, TORQUE_DISABLE);
            if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, PROTOCOL_VERSION)) != COMM_SUCCESS)
            {
                dynamixel.printTxRxResult(PROTOCOL_VERSION, dxl_comm_result);
            }
            else if ((dxl_error = dynamixel.getLastRxPacketError(port_num, PROTOCOL_VERSION)) != 0)
            {
                dynamixel.printRxPacketError(PROTOCOL_VERSION, dxl_error);
            }

            // Close port
            dynamixel.closePort(port_num);
        }
        #endregion

        #region Methods
        public void Setup_Port(ref bool isPortOpened)
        {
            if (!isPortOpened)
            {
                // Initialize PortHandler Structs
                // Set the port path
                // Get methods and members of PortHandlerLinux or PortHandlerWindows
                port_num = dynamixel.portHandler(DEVICENAME);

                // Initialize PacketHandler Structs
                dynamixel.packetHandler();

                // Open port
                if (dynamixel.openPort(port_num))
                {
                    Console.WriteLine("Succeeded to open the port!");
                    isPortOpened = true;
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
            }

            // Enable Dynamixel Torque
            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, (byte)DXL_ID, ADDR_MX_TORQUE_ENABLE, TORQUE_ENABLE);
            if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, PROTOCOL_VERSION)) != COMM_SUCCESS)
            {
                dynamixel.printTxRxResult(PROTOCOL_VERSION, dxl_comm_result);
            }
            else if ((dxl_error = dynamixel.getLastRxPacketError(port_num, PROTOCOL_VERSION)) != 0)
            {
                dynamixel.printRxPacketError(PROTOCOL_VERSION, dxl_error);
            }
            else
            {
                Console.WriteLine("Dynamixel {0} has been successfully connected", DXL_ID);
            }
        }

        public void StartServo(int portNum_param)
        {
            port_num = portNum_param;       // update the port_num from the value recieved in roboto_arm

            // Enable Dynamixel Torque
            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, (byte)DXL_ID, ADDR_MX_TORQUE_ENABLE, TORQUE_ENABLE);
            if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, PROTOCOL_VERSION)) != COMM_SUCCESS)
            {
                Console.WriteLine("ID: {0}", DXL_ID);
                dynamixel.printTxRxResult(PROTOCOL_VERSION, dxl_comm_result);
            }
            else if ((dxl_error = dynamixel.getLastRxPacketError(port_num, PROTOCOL_VERSION)) != 0)
            {
                Console.WriteLine("ID: {0}", DXL_ID);
                dynamixel.printRxPacketError(PROTOCOL_VERSION, dxl_error);
            }
            else
            {
                Console.WriteLine("Dynamixel {0} has been successfully connected", DXL_ID);
            }


            // update maximum and mimum values by fetching the startup position of the servo
            dxl_present_position = dynamixel.read2ByteTxRx(port_num, PROTOCOL_VERSION, (byte)DXL_ID, ADDR_MX_PRESENT_POSITION);
            if (DXL_MINIMUM_POSITION_VALUE == 0)
            {
                // this method uses the read position as the minimum and add the max as the offset
                DXL_MAXIMUM_POSITION_VALUE += dxl_present_position;            // sign is taken care of as maximum value
                DXL_MINIMUM_POSITION_VALUE = dxl_present_position;            
            }
            else
            {
                // this method uses the read position as the center point of the current max and min values.
                DXL_MAXIMUM_POSITION_VALUE += dxl_present_position;            // sign is taken care of as maximum value
                DXL_MINIMUM_POSITION_VALUE += dxl_present_position;            // sign is taken care of as mimimum value
            }

        }

        public bool Move_To(int goal_position)
        {
            // Write a check to make sure the goal position is within range and allowable
            Check_Input_Range((short)DXL_ID, ref goal_position);
            dxl_goal_position = (short)goal_position;

            // Write goal position
            dynamixel.write2ByteTxRx(port_num, PROTOCOL_VERSION, (byte)DXL_ID, ADDR_MX_GOAL_POSITION, (short)goal_position);
            if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, PROTOCOL_VERSION)) != COMM_SUCCESS)
            {
                dynamixel.printTxRxResult(PROTOCOL_VERSION, dxl_comm_result);
                return false;
            }
            else if ((dxl_error = dynamixel.getLastRxPacketError(port_num, PROTOCOL_VERSION)) != 0)
            {
                dynamixel.printRxPacketError(PROTOCOL_VERSION, dxl_error);
                return false;
            }

            do
            {
                // Read present position
                dxl_present_position = dynamixel.read2ByteTxRx(port_num, PROTOCOL_VERSION, (byte)DXL_ID, ADDR_MX_PRESENT_POSITION);
                if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, PROTOCOL_VERSION)) != COMM_SUCCESS)
                {
                    dynamixel.printTxRxResult(PROTOCOL_VERSION, dxl_comm_result);
                }
                else if ((dxl_error = dynamixel.getLastRxPacketError(port_num, PROTOCOL_VERSION)) != 0)
                {
                    dynamixel.printRxPacketError(PROTOCOL_VERSION, dxl_error);
                }

                //Console.WriteLine("[ID: {0}] GoalPos: {1}  PresPos: {2}", DXL_ID, goal_position, dxl_present_position);

            } while ((Math.Abs(goal_position - dxl_present_position) > DXL_MOVING_STATUS_THRESHOLD));

            return true;
        }

        private void Check_Input_Range(Int16 id, ref int goal_pos)
        {
            switch (id)
            {
                case Elbow_ID:  // special case for the elbow id
                    if (Math.Abs(goal_pos) > Math.Abs(DXL_MAXIMUM_POSITION_VALUE))
                    {
                        Console.WriteLine("The input value {0} exceeds the maximum value {1}. The goal position will be set to the max.", goal_pos, DXL_MAXIMUM_POSITION_VALUE);
                        goal_pos = DXL_MAXIMUM_POSITION_VALUE;
                    }
                    else if (goal_pos > DXL_MINIMUM_POSITION_VALUE)
                    {
                        Console.WriteLine("The input value {0} exceeds the minimum value {1}. The goal position will be set to the min.", goal_pos, DXL_MINIMUM_POSITION_VALUE);
                        goal_pos = DXL_MINIMUM_POSITION_VALUE;
                    }
                    break;
                default:        // default case for base and shoulder
                    if (Math.Abs(goal_pos) > Math.Abs(DXL_MAXIMUM_POSITION_VALUE))
                    {
                        Console.WriteLine("The input value {0} exceeds the maximum value {1}. The goal position will be set to the max.", goal_pos, DXL_MAXIMUM_POSITION_VALUE);
                        goal_pos = DXL_MAXIMUM_POSITION_VALUE;
                    }
                    else if (Math.Abs(goal_pos) < Math.Abs(DXL_MINIMUM_POSITION_VALUE))
                    {
                        Console.WriteLine("The input value {0} exceeds the minimum value {1}. The goal position will be set to the min.", goal_pos, DXL_MINIMUM_POSITION_VALUE);
                        goal_pos = DXL_MINIMUM_POSITION_VALUE;
                    }
                    break;
            }
            
        }
        #endregion
    }
}
