using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NGCP.UGV;
using UGV.Core;

namespace UGVStationCommProto
{
    class Program
    {
        static CommProtocol commProtocol;
        static UGVSetting Settings = new UGVSetting();

        public static NGCP.UGV.UGV.DriveState State { get; private set; }

        static void Main(string[] args)
        {

            if (Settings.UseCommProtocol)
            {


                commProtocol = new CommProtocol(Settings.CommNode);
                commProtocol.initializeConnection(Settings.CommPort, Settings.CommBaud);
                //parse address to add
                //this is a bad way to do this should be changed michael wallace 5/12/2017
                string[] words = Settings.CommAddresses.Split(null);
                int[] destNode = Array.ConvertAll(words.Where((str, ix) => ix % 2 == 0).ToArray(), int.Parse);//even
                string[] destAddress = words.Where((str, ix) => ix % 2 == 1).ToArray();//odd
                if (destNode.Length == destAddress.Length)
                {
                    for (int x = 0; x < destNode.Length; x++)
                    {
                        commProtocol.addAddress(destNode[x], destAddress[x]);
                    }
                }
                //link call backs
                commProtocol.LinkCallback(new NGCP.VehicleWaypointCommand(), new Comnet.CallBack(VehicleWaypointCommandCallback));
                commProtocol.LinkCallback(new NGCP.VehicleModeCommand(), new Comnet.CallBack(VehicleModeCommandCallback));
                commProtocol.LinkCallback(new NGCP.ArmCommand(), new Comnet.CallBack(ArmCommandCallback));
                commProtocol.LinkCallback(new NGCP.VehicleSystemStatus(), new Comnet.CallBack(VehicleSystemStatusCallback));

                commProtocol.start();
            }

            State = NGCP.UGV.UGV.DriveState.DriveAwayFromTarget;

            while (true)
            {
                Console.ReadLine();
                Console.WriteLine("test package sent\n");
                commProtocol.SendState(new UGVState(), 100);
            }
        }


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
        static public int VehicleModeCommandCallback(Comnet.Header header, Comnet.ABSPacket packet, Comnet.CommNode node)
        {
            //validate who the packet is from 1 is gcs
            if (header.GetSourceID() == 1)
            {
                NGCP.VehicleModeCommand twc = Comnet.ABSPacket.GetValue<NGCP.VehicleModeCommand>(packet);
                //Settings.DriveMode = (DriveMode)twc.vehicle_mode;
            }

            //make sure you return this way to declare succes and destory the pointer(c++)
            return (Int32)(Comnet.CallBackCodes.CALLBACK_SUCCESS | Comnet.CallBackCodes.CALLBACK_DESTROY_PACKET);
        }

        static public int ArmCommandCallback(Comnet.Header header, Comnet.ABSPacket packet, Comnet.CommNode node)
        {
            NGCP.ArmCommand ac = Comnet.ABSPacket.GetValue<NGCP.ArmCommand>(packet);
            //do some kind of switch case
            //@TODO ARM
            int servoPosition = ac.id;
            int servoValue = ac.position;
            Console.WriteLine("test armCommand");
            //id_servo_GUI = ac.id;
            //val_servo_GUI = ac.position;


            
            return (Int32)(Comnet.CallBackCodes.CALLBACK_SUCCESS | Comnet.CallBackCodes.CALLBACK_DESTROY_PACKET);


        }


        static public int VehicleSystemStatusCallback(Comnet.Header header, Comnet.ABSPacket packet, Comnet.CommNode node)
        {
            NGCP.VehicleSystemStatus vss = Comnet.ABSPacket.GetValue<NGCP.VehicleSystemStatus>(packet);
            //State = (UGV.DriveState)vss.vehicle_state;
            //make sure you return this way to declare succes and destory the pointer(c++)
            return (Int32)(Comnet.CallBackCodes.CALLBACK_SUCCESS | Comnet.CallBackCodes.CALLBACK_DESTROY_PACKET);
        }
    }

    public class UGVSetting
    {
        /// <summary>
        /// Rate of update in sequencial control in ms
        /// </summary>
        public int ControlRate = 50;
        /// <summary>
        /// Rate of board cast in sequencial control in ms
        /// </summary>
        public int BoardCastRate = 50;

        /// <summary>
        /// IMU Gain
        /// </summary>
        public double IMUGain = 0.5;
        //{"UseArm", "False"},
        //   {"ArmPort", "COM11"},
        //   {"ArmBaud", "8000"},
        public string ArduinoPort = "COM0";
        public int ArduinoBaud = 9600;
        public bool UseArduino = true;
        public string GPSPort = "COM0";
        public int GPSBaud = 115200;
        public bool UseGPS = false;
        public string IMUPort = "COM0";
        public int IMUBaud = 9600;
        public bool UseIMU = false;
        public string FPGAPort = "COM0";
        public int FPGABaud = 115200;
        public bool UseFPGA = true;
        public string CommPort = "COM13";
        public int CommBaud = 57600;
        public int CommNode = 7;
        public string CommAddresses = "100 0013A20040A5430F 1 0013A2004067E4AE";//"5 0013A20040A5430F";//"1 0013A2004067E4AE 100 0013A20040A814FD";
        public bool UseXBeeComm = true;
        public bool UseCommProtocol = true;
        public byte SteeringLimit = 127; //94
        public bool UseVision = true;
        public bool UseCamera = true;
        public string VisionHostIP = "127.0.0.1";
        public int VisionHostPort = 5500;
        public int CameraHostPort = 5501;
        public string VisionTargetIP = "127.0.0.1";
        public int VisionTargetPort = 5501;
        public string NavPort = "COM2";
        public int NavBaud = 57600;
        public bool UseNav = true;
        public int ArmBaud = 8000;
        public bool UseArm = false;
        public string ArmPort = "COM11";
       // public UGV.DriveMode DriveMode = UGV.DriveMode.LocalControl;


        /// <summary>
        /// Dead Zone for slow speed to prevent hardward damage
        /// </summary>
        public double DeadZone = 20;

        public byte UGVProtonetNodeID = 46;
        public sbyte AttiudeRate = 20;
        public sbyte PositionRate = 20;

    }
}
