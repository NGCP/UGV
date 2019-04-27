//#define USE_ABS
#define FPGA_Sensors
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using UGV.Core.Sensors;
using UGV.Core.IO;
using UGV.Core.Maths;
using UGV.Core.Navigation;
using static UGVBehaviorMap.UGVXbee;

//using Comnet;

namespace NGCP.UGV
{
    public partial class UGV
    {
        /// <summary>
        /// Driving mode of UGV
        /// </summary>
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

        #region Public Properties

        /// <summary>
        /// Indicate if UGV is enabled
        /// </summary>
        public volatile bool Enabled = true;

        /// <summary>
        /// Speed factor of front wheel driving from -1000 to 1000
        /// </summary>
        private double localSpeed;
        public double LocalSpeed
        {
            get { return localSpeed; }
            set
            {
                localSpeed = value;
                SendControl();
            }
        }
        /// <summary>
        /// Steering factor of driving from -1000 to 1000
        /// </summary>
        private double localSteering;
        public double LocalSteering
        {
            get { return localSteering; }
            set
            {
                localSteering = value;
                SendControl();
            }
        }

        #region Autonomous Related

        /// <summary>
        /// Speed factor of front wheel driving from -1000 to 1000
        /// </summary>
        private double speed;      
        public double Speed
        {
            get { return speed; }
            set
            {
                speed = value;
                if(speed != lastSpeed)
                    //SendControl();
                lastSpeed = speed;
            }
        }
        private double lastSpeed;
      

        /// <summary>
        /// Steering factor of driving from -1000 to 1000
        /// </summary>
        private double steering;           
        public double Steering
        {
            get { return steering; }
            set
            {
                steering = value;
                if(steering != lastSteering)
                    //SendControl();
                lastSteering = steering;
            }
        }
        private double lastSteering;
        


        #endregion Autonomous Related

        /// <summary>
        /// Final output for Front Wheel
        /// </summary>
        public double FinalFrontWheel { get; private set; }

        /// <summary>
        /// Final output for Rear Wheel
        /// </summary>
        public double FinalRearWheel { get; private set; }

        /// <summary>
        /// Final output for Steer
        /// </summary>
        public double FinalSteering { get; private set; }

        /// <summary>
        /// Settings of UGV
        /// </summary>
        public UGVSetting Settings { get; private set; }

        /// <summary>
        /// Linked IOs of UGV
        /// </summary>
        public Dictionary<string, Link> Links = new Dictionary<string, Link>();

        /// <summary>
        /// If vehicle is inside boundary
        /// </summary>
        public bool InsideBoundary { get; private set; }

        /// <summary>
        /// If Vehicle is close to boundary
        /// </summary>
        public bool CloseBoundary { get; private set; }

        /// <summary>
        /// If vehicle is inside SafeZone
        /// </summary>
        public bool InsideSafeZone { get; private set; }

        /// <summary>
        /// Boundary of the active zone
        /// </summary>
        public List<WayPoint> Boundary { get; set; }

        /// <summary>
        /// Boundary of Safe Zone
        /// </summary>
        public List<WayPoint> SafeZone { get; set; }

        /// <summary>
        /// The geo location as starting default
        /// </summary>
        public WayPoint DefaultLocation { get; set; }

        /// <summary>
        /// If Target has dropped
        /// </summary>
        public bool TargetDropped { get; private set; }

        /// <summary>
        /// Obstacle vector
        /// </summary>
        public Vector2d AvoidanceVector = new Vector2d(0, 0);

        /// <summary>
        /// Number of Lidar lines considered
        /// </summary>
        public int LineCount { get; set; }

        /// <summary>
        /// If the target is found
        /// </summary>
        public bool TargetFound { get; set; }
        /// <summary>
        /// if payload is foumd by webcam on arm udp
        /// </summary>
        public bool PayloadFound { get; set; }
        /// <summary>
        /// Location of the target
        /// </summary>
        public WayPoint TargetWaypoint { get; set; }

        public WayPoint StartWaypoint { get; set; }


        /// <summary>
        /// Way point of UGV route to search for target
        /// </summary>
        public ConcurrentQueue<WayPoint> Waypoints { get; set; }

        /// <summary>
        /// Straight line distance towards the next waypoint
        /// </summary>
        public double NextWaypointDistance { get; set; }

        /// <summary>
        /// Straight line bearing angle towards the next waypoint
        /// </summary>
        public double NextWaypointBearing { get; set; }

        /// <summary>
        /// Straight line bearing angle towards the next waypoint
        /// </summary>
        public double NextWaypointBearingError { get; set; }

        /// <summary>
        /// A Debug message from upper layer
        /// </summary>
        public StringBuilder DebugMessage = new StringBuilder();

        /// <summary>
        /// Straight line distance towards the next waypoint
        /// </summary>
        public double NextVisionDistance { get; set; }

        /// <summary>
        /// Straight line bearing angle towards the next waypoint
        /// </summary>
        public double NextVisionBearing { get; set; }

        /// <summary>
        /// Straight line bearing angle towards the next waypoint
        /// </summary>
        public double NextVisionBearingError { get; set; }

        /// <summary>
        /// A list of waypoint from vision
        /// </summary>
        public List<VisionWayPoint> VisionWaypoints = new List<VisionWayPoint>();

        /// <summary>
        /// Last time stamp of vision waypoint
        /// </summary>
        public DateTime VisionWaypointReceiveTime { get; private set; }

        /// <summary>
        /// A list of target from vision
        /// </summary>
        public List<VisionTarget> VisionTargets = new List<VisionTarget>();

        /// <summary>
        /// Last time stamp of vision target
        /// </summary>
        public DateTime VisionTargetReceiveTime { get; private set; }

        /// <summary>
        /// Battery status of UGV
        /// </summary>
        public BatteryStatus BatteryInfo { get; private set; }

        /// <summary>
        /// Control state of the motors true FPGA controls motors by direct command of UGV
        /// false the UGV passes a heading a distance and a speed and the FPGA handles it on there own
        /// </summary>
        public bool ByWireMotorControl { get; private set; }
      
        public int LeftClicks;
        public int RightClicks;
        public double TargetDistance;
        public double TargetAngle;

        /// <summary>
        /// This is the tri colored payload arm is supposed to retrieve
        /// </summary>
        public Payload main_payload;
        private Stopwatch sw;

        public int x_mainpayload = 0;
        public int y_mainpayload = 0;



        //debug properties
        
        #endregion Public Properties

        #region Forwarding Data
        /// <summary>
        /// Encoder Speed
        /// </summary>
        public double EncoderSpeed { get { return encoders.speed; } }

        /// <summary>
        /// Encoder Turn
        /// </summary>
        public double EncoderTurn { get { return encoders.turn; } }

        /// <summary>
        /// Pitch angle of UGV
        /// </summary>
        public double Pitch { get { return imu.Pitch; } }

        /// <summary>
        /// Roll angle of UGV
        /// </summary>
        public double Roll { get { return imu.Roll; } }

        /// <summary>
        /// Heading angle of UGV
        /// </summary>
        public double Heading { get { return imu.Heading; } }

        /// <summary>
        /// Latitude of UGV
        /// </summary>
        public double Latitude { get { if (Settings.UseNav) return imu.Latitude; return 34.058963; } } //default value for testing in lab

        /// <summary>
        /// Longitude of UGV
        /// </summary>
        public double Longitude { get { if (Settings.UseNav) return imu.Longitude; return -117.821589; } } //default value for testing in lab return

        /// <summary>
        /// Altitude of UGV
        /// </summary>
        public double Altitude { get { return imu.Altitude; } }

        /// <summary>
        /// Ground Speed of UGV
        /// </summary>
        public double GroundSpeed { get { return imu.GroundSpeed; } }

        /// <summary>
        /// TrackAngle of UGV
        /// </summary>
        public double TrackAngle { get { return 0; } } // gps.TrackAngle; } }

        /// <summary>
        /// Data State of UGV
        /// </summary>
        public bool GPSLock { get { return imu.GoodData; } }

        /// <summary>
        /// Satellite Lock of UGV
        /// </summary>
        public int SatelliteCount { get { return 0; } } // gps.SatelliteCount; } }

        /// <summary>
        /// Data Time of UGV
        /// </summary>
        //public DateTime GPSTime { get { return new DateTime(); }} // gps.GPSTime; } }


        public double EncoderLat { get { return tracker.getLatitude(); } }

        public double EncoderLon { get { return tracker.getLongitude(); } }
        #endregion Forwarding Data

        #region Private Properties


        bool NavDebug = false;

        /// <summary>
        /// Timer for sequencial action
        /// </summary>
        System.Timers.Timer controlTimer;

        /// <summary>
        /// Timer for system state board casting
        /// </summary>
        System.Timers.Timer boardcastTimer;

        /// <summary>
        /// Wheel power from XBee
        /// </summary>
        byte CommWheel = 127;

        /// <summary>
        /// Steering Angle from XBee;
        /// </summary>
        byte CommSteering = 127;

        /// <summary>
        /// If Override control by Comm
        /// </summary>
        public bool CommOverride { get; private set; }

        public UGVXbee xbee;

        /// <summary>
        /// IMU of UGV
        /// </summary>
        Encoders encoders { get; set; }

        /// <summary>
        /// IMU of UGV
        /// </summary>
        Nav440 imu;

        /// <summary>
        /// GPS of UGV
        /// </summary>
        //GPS gps { get; set; }

        /// <summary>
        /// IMU of UGV
        /// </summary>
        Tracker tracker { get; set; }

        /// <summary>
        /// Stanag port of UGV
        /// </summary>
        Serial Xbee { get; set; }

        /// <summary>
        /// Ftdi port of UGV
        /// </summary>
        Serial ftdi_gps { get; set; }

        /// <summary>
        /// Ftdi port of UGV
        /// </summary>
        Serial arduino { get; set; }

        /// <summary>
        /// Ftdi port of UGV
        /// </summary>
        Serial fpga { get; set; }

        /// <summary>
        /// UDP for Lidar
        /// </summary>
        UdpClientSocket udp_lidar { get; set; }

        /// <summary>
        /// UDP for Camera
        /// </summary>
        UdpClientSocket udp_camera { get; set; }

        /// <summary>
        /// Protonet of UGV
        /// </summary>
      //  Comnet.Node protonet { get; set; }

        /// <summary>
        /// Port of the Nav440
        /// </summary>
        Serial Nav { get; set; }



        #endregion Private Properties

        #region Constructor

        /// <summary>
        /// Constructor of UGV
        /// </summary>
        public UGV()
        {
            encoders = new Encoders();
            imu = new Nav440();
            Settings = new UGVSetting();
            Boundary = new List<WayPoint>();
            SafeZone = new List<WayPoint>();
            AvoidanceVector = new Vector2d(0, 0);
            Waypoints = new ConcurrentQueue<WayPoint>();
            controlTimer = new System.Timers.Timer();
            controlTimer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Tick);
            boardcastTimer = new System.Timers.Timer();
            boardcastTimer.Elapsed += new System.Timers.ElapsedEventHandler(BoardCast_Tick);
            CommOverride = false;

            tracker = new Tracker(Latitude, Longitude, Heading);
            TargetFound = false;
            PayloadFound = false;

               
        
            main_payload = new Payload();
            StartWaypoint = new WayPoint(this.Latitude, this.Longitude, this.Altitude);

        }
                       
        #endregion Constructor

        #region Public Methods
        public void debugUGV()
        {
            DebugMessage.Clear();
            if (controlTimer.Enabled == true)
            {
                DebugMessage.Append("\ncontrol timer is enabled");
            }
            else
            {
                DebugMessage.Append("\ncontrol timer is not enabled");            
            }
            if (boardcastTimer.Enabled == true)
            {
                DebugMessage.Append("\nboardcastTimer is enabled");
            }
            else
            {
                DebugMessage.Append("\nboardcastTimer is not enabled");
            }      //test git
        }

        /// <summary>
        /// Start opeartion of UGV
        /// </summary>          
        public void Start()
        {
            //clear links
            Links.Clear();

            #region FPGA Connection

            //open a fpga serial port            

            //open a fpga serial port
            fpga = new Serial(Settings.FPGAPort, Settings.FPGABaud);
            fpga.EscapeToken = new byte[] { 251, 252, 253, 254, 255 };
            Links.Add("FPGA FTDI", fpga);
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
                        int SB = bytes[4] - 0x30;
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
            if (Settings.UseFPGA)
                fpga.Start();

            #endregion FPGA Connection
            //UGVBehaviorMap.ControlProgram.InitializeConnection();


            /*
            * open connection to Gnav, 
            * if Gnav not found use old GPS/IMU setup
            * else process Imu through gnav class
            * and set the GPS to run normally but using 
            * GNavs gps port
            */

            Nav = new Serial(Settings.NavPort, Settings.NavBaud);
            Nav.EscapeToken = new byte[0];
            Nav.PackageMode = Serial.PackageModes.UseFunction;
            Links.Add("NAV FTDI", Nav);
            Nav.FindPackageEnd = (bytes =>
            {
                int offset = 0;
                for (int i = 0; i < bytes.Length - 1; i++)
                {

                    if (bytes[i] == 0x55 && bytes[i + 1] == 0x55)
                    {
                        offset = i;
                    }
                }
                if (bytes.Length >= 5 && bytes[offset] == 0x55 && bytes[offset + 1] == 0x55)
                {
                    if (bytes.Length > offset + 4 &&
                        bytes.Length >= (offset + 7 + bytes[offset + 4]))
                    {
                        return offset + 7 + bytes[offset + 4];
                    }
                }
                return -1;
            });
            Nav.PackageReceived = navCallback;

            if (Settings.UseNav)
                Nav.Start();


            #region Encoders Connection

            //open a serial port
            //arduino = new Serial(Settings.ArduinoPort, Settings.ArduinoBaud);
            //arduino.EscapeToken = new byte[] { (byte)'\n' };
            //Links.Add("Encoder", arduino);
            ////read all feedback from arduino
            //arduino.PackageReceived = (bytes =>
            //{
            //    //bit converting method
            //    /*Console.Write(BitConverter.ToString(bytes));
            //    Console.WriteLine("Length: " + bytes.Length);
            //    double left = BitConverter.ToDouble(bytes, 0);
            //    double right = BitConverter.ToDouble(bytes, 64);

            //    EncodersPackage package = new EncodersPackage();
            //    package.left = left;
            //    package.right = right;
            //    encoders.Update(package);
            //    tracker.add(EncoderSpeed, EncoderTurn, left, right);*/

            //    //string parsing
            //    if (bytes[0] == 'R')
            //    {
            //        int i = 0;
            //        while (bytes[i] != 'L')
            //            i++;

            //        int k = 0;
            //        while (bytes[k] != 'C')
            //            k++;

            //        int l = 0;
            //        while (bytes[l] != 'D')
            //            l++;

            //        string rightString = "";
            //        for (int j = 1; j < i; j++)
            //            rightString += (char)bytes[j];

            //        string leftString = "";
            //        for (int j = i + 1; j < k; j++)
            //            leftString += (char)bytes[j];

            //        string rightClicksString = "";
            //        for (int j = k + 1; j < l; j++)
            //            rightClicksString += (char)bytes[j];

            //        string leftClicksString = "";
            //        for (int j = l + 1; j < bytes.Length; j++)
            //            leftClicksString += (char)bytes[j];

            //        try
            //        {
            //            double left = double.Parse(leftString);
            //            double right = double.Parse(rightString);
            //            LeftClicks = int.Parse(leftClicksString);
            //            RightClicks = int.Parse(rightClicksString);
            //            EncodersPackage package = new EncodersPackage();
            //            package.left = left;
            //            package.right = right;
            //            encoders.Update(package);
            //            tracker.add(EncoderSpeed, EncoderTurn, left, right);
            //        }
            //        catch (FormatException) { }
            //    }
            //});
            ////start
            //if (Settings.UseArduino)
            //    arduino.Start();
            
            #endregion IMU Connection

#if oldGPS
            #region GPS Connection

            //open a ftdi serial port
            ftdi_gps = new Serial(Settings.GPSPort, Settings.GPSBaud);
            ftdi_gps.EscapeToken = new byte[] { (byte)'\n' };
            Links.Add("GPS", ftdi_gps);
            //read all feedback from ftdi
            ftdi_gps.PackageReceived = (bytes =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
                //update gps
                GPSPackage gpsPackage = new GPSPackage();
                gpsPackage.NMEA = Encoding.ASCII.GetString(bytes);
                gps.Update(gpsPackage);
                //determine boundary state
                InsideBoundary = WayPoint.IsInsideBoundary(this.Latitude, this.Longitude, Boundary);
                var projection = WayPoint.Projection(new WayPoint(this.Latitude, this.Longitude, 0), Heading, 4);
                CloseBoundary = WayPoint.IsInsideBoundary(projection.Lat, projection.Long, Boundary);
                InsideSafeZone = WayPoint.IsInsideBoundary(this.Latitude, this.Longitude, SafeZone);
            });
            //start
            if (Settings.UseGPS)
                ftdi_gps.Start();

            #endregion GPS Connection
#endif

#if oldIMU
            #region IMU/Encoders Connection SET THE PORTS

            imu.ComplementaryFilterRatio = Settings.IMUGain;
            //open a ftdi serial port
            ftdi_imu = new Serial(Settings.IMUPort, Settings.IMUBaud);
            ftdi_imu.EscapeToken = new byte[] { 252, 253, 254, 255 };
            Links.Add("IMU", ftdi_imu);
            //read all feedback from ftdi
            ftdi_imu.PackageReceived = (bytes =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;                
                try
                {
                    if (bytes.Length < 27 && bytes > 16)
                    {
                        IMUPackage package = new IMUPackage();
                        package.AccelX = BitConverter.ToInt16(bytes, 0);
                        package.AccelY = BitConverter.ToInt16(bytes, 3);
                        package.AccelZ = -BitConverter.ToInt16(bytes, 6);
                        package.GyroX = BitConverter.ToInt16(bytes, 9);
                        package.GyroY = BitConverter.ToInt16(bytes, 12);
                        package.GyroZ = BitConverter.ToInt16(bytes, 15);
                        package.MagX = BitConverter.ToInt16(bytes, 18);
                        package.MagY = BitConverter.ToInt16(bytes, 21);
                        package.MagZ = BitConverter.ToInt16(bytes, 24);
                        //update imu
                        imu.Update(package);
                    }
                    else if (bytes =16 )
                    {
                        EncodersPackage package = new EncodersPackage();
                        package.left = BitConverter.ToDouble(bytes, 0);
                        package.right = BitConverter.ToDouble(bytes, 8);
                        tracker.add(EncoderSpeed, EncoderTurn, BitConverter.ToDouble(bytes, 0), BitConverter.ToDouble(bytes, 8));
                    }    
                }
                catch (ArgumentOutOfRangeException)
                { }
            });
            //start
            if (Settings.UseIMU)
                ftdi_imu.Start();

            #endregion IMU Connection
#endif

            #region Vision Connection

            //start lidar udp
            udp_lidar = new UdpClientSocket(
                System.Net.IPAddress.Parse(Settings.VisionHostIP), Settings.VisionHostPort);
            Links.Add("Lidar UDP", udp_lidar);
            //define call back
            udp_lidar.PackageReceived = (bytes =>
            {
                int num = BitConverter.ToInt32(bytes, 0);
                double magnitude = BitConverter.ToDouble(bytes, sizeof(int));
                double angle = BitConverter.ToDouble(bytes, sizeof(double) + sizeof(int));
                LineCount = num;
                AvoidanceVector = new Vector2d(angle, magnitude);
            });

            //start camera udp
            udp_camera = new UdpClientSocket(
                System.Net.IPAddress.Parse(Settings.VisionHostIP), Settings.CameraHostPort);
            Links.Add("Camera UDP", udp_camera);
            //define call back
            udp_armcam = new UdpClientSocket(
                System.Net.IPAddress.Parse(Settings.VisionHostIP), 8006);
            Links.Add("Arm Camera UDP", udp_armcam);
            udp_armcam.PackageReceived = (bytes =>
            {
                PayloadFound = BitConverter.ToBoolean(bytes, 0);
                x_mainpayload = BitConverter.ToInt32(bytes, sizeof(bool));
                y_mainpayload = BitConverter.ToInt32(bytes, (sizeof(int) + sizeof(bool)));


            });

            udp_camera.PackageReceived = (bytes =>
            {

                TargetFound = BitConverter.ToBoolean(bytes, 0);
                TargetAngle = BitConverter.ToDouble(bytes, sizeof(bool));
                if(TargetFound)
                {
                    
                    TargetDistance = WayPoint.GetDistance(this.Latitude, this.Longitude, TargetWaypoint.Lat, TargetWaypoint.Long);
                }
                else
                {
                    TargetAngle = 0;
                    TargetDistance = 0;
                }
            });
            //start
            if (Settings.UseVision)
                udp_lidar.Start();
            if (Settings.UseCamera)
            {
                udp_camera.Start();//pointgrey
                udp_armcam.Start();//armwebcam
            }

            #endregion Vision Connection

            #region Robotic Arm Connection
            // instantiate the arm
            //arm_ugv = new Roboto_Arm(Settings.ArmPort, Settings.ArmBaud);
            //if (Settings.UseArm)
            //    arm_ugv.StartArm();

            #endregion Robotic Arm Connection

            #region Communication Connection
            // commented out the snippen of code until required libraries are included
            ////open communication port
            if (Settings.UseUGVXbee)
            {
                UGVXbee xbee = new UGVXbee(Settings.CommPort, Settings.CommBaud, Settings.CommAddress);

                xbee.ReceiveConnectionAck = (o, eventArgs) => {}; // start sending update messages
                xbee.ReceiveAddMission = (o, eventArgs) => {}; // start task
                xbee.ReceivePause = (o, eventArgs) => {}; // pause
                xbee.ReceiveResume = (o, eventArgs) => {}; // resume
                xbee.ReceiveStop = (o, eventArgs) => {}; // stop mission
            }

            #endregion Communication Connection

            //define timer interval
            controlTimer.Interval = Settings.ControlRate;
            boardcastTimer.Interval = Settings.BoardCastRate;
            //start timers
            controlTimer.Start();
            boardcastTimer.Start();
            //start do work in a separate thread
            ThreadPool.QueueUserWorkItem(new WaitCallback(StartBehavior));
            Thread dowork = new Thread(new ThreadStart(DoWork));
            dowork.Start();
        }
        /// <summary>
        /// Stop operation of UGV
        /// </summary>
        public void Stop()
        {
            ftdi_gps.Stop();
            arduino.Stop();
            fpga.Stop();
            controlTimer.Stop();
            boardcastTimer.Stop();
            Nav.Stop();
        }

        /// <summary>
        /// Reset the mission
        /// </summary>
        public void ResetMission()
        {
            this.State = DriveState.SearchTarget;
            this.VisionTargets = new List<VisionTarget>();
            this.VisionWaypoints = new List<VisionWayPoint>();
            this.Waypoints = new ConcurrentQueue<WayPoint>();
            this.TargetLockedLocation = null;
            this.ResetAllState();
        }

        /// <summary>
        /// Update Battery Status
        /// </summary>
        public void UpdateBattery(BatteryStatus battery)
        {
            BatteryInfo = battery;
            if (battery.Voltage12V > 2.0 && battery.Voltage12V < 11.4)
                this.Enabled = false;
            else if (battery.Voltage12V > 2.0 && battery.Voltage12V < 11.1)
                System.Diagnostics.Process.Start("shutdown", "-s -t 5");
        }

        #region Waypoint Operation

        /// <summary>
        /// Insert a waypoint at a certain index, of index is out of range, it will insert at the end
        /// </summary>
        /// <param name="index"></param>
        /// <param name="waypoint"></param>
        public void InsertWaypointAt(int index, WayPoint waypoint)
        {
            List<WayPoint> tempWayPoints = Waypoints.ToList();
            if (index >= 0)
            {
                if (index >= tempWayPoints.Count)
                {
                    Waypoints.Enqueue(waypoint);
                }
                else
                {
                    tempWayPoints.Insert(index, waypoint);
                    Waypoints = new ConcurrentQueue<WayPoint>(tempWayPoints);
                }
            }
        }

        /// <summary>
        /// Edit waypoint at a certain indedx
        /// </summary>
        /// <param name="index"></param>
        /// <param name="waypoint"></param>
        public void EditWaypointAt(int index, WayPoint waypoint)
        {
            List<WayPoint> tempWayPoints = Waypoints.ToList();
            if (index >= 0 && index < tempWayPoints.Count)
            {
                tempWayPoints[index] = waypoint;
                Waypoints = new ConcurrentQueue<WayPoint>(tempWayPoints);
            }
        }

        /// <summary>
        /// Remove a waypoint at a certain index
        /// </summary>
        public void RemoveWaypointAt(int index)
        {

            List<WayPoint> tempWayPoints = Waypoints.ToList();
            if (index < tempWayPoints.Count && index >= 0)
            {
                if (index == 0)
                {
                    WayPoint temp;
                    Waypoints.TryDequeue(out temp);
                }
                else
                {
                    tempWayPoints.RemoveAt(index);
                    Waypoints = new ConcurrentQueue<WayPoint>(tempWayPoints);
                }
            }
        }

        #endregion Waypoint Operation

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Start behavior code
        /// </summary>
        /// <param name="obj"></param>
        void StartBehavior(object obj)
        {
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
            DoWork();
        }

        /// <summary>
        /// EventHandler when timer tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Timer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {   
            //controlTimer.Enabled = false;
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
            if(Settings.DriveMode == DriveMode.Autonomous || Settings.DriveMode == DriveMode.SemiAutonomous)
                SendControl();
            //controlTimer.Enabled = true; 
        }

        #region arm and motor controls
        /// <summary>
        /// Seqencial action to send control
        /// </summary>
        void SendControl()
        {
            //Compute Control
            if (CommOverride)
            {
                //scale range input to outpur
                FinalFrontWheel = (CommWheel - 127.5) * 2.0;
                FinalSteering = (-CommSteering * 200 / 256.0) + 412;
            }
            else if (Settings.DriveMode == DriveMode.SemiAutonomous)
            {
                //scale range input to outpur                
                FinalFrontWheel = (localSpeed * 255.0 / 1000.0);
                FinalSteering = (-steering * 27.0 / 1000.0) + 27.0; // changed from 100 to 340, from 512 to 2048
                commProtocol.SendSpeedSteeringCommand((ushort)localSpeed, (ushort)steering);

            }
            else if (Settings.DriveMode == DriveMode.Autonomous)
            {
                //scale range input to outpur
                FinalFrontWheel = (speed * 255.0 / 1000.0);
                FinalSteering = (-steering * 27.0 / 1000.0) + 27.0; // changed from 100 to 340, from 512 to 2048
                commProtocol.SendSpeedSteeringCommand((ushort)speed, (ushort)steering);

            }
            else if (Settings.DriveMode == DriveMode.LocalControl)
            {
                //scale range input to outpur
                FinalFrontWheel = (localSpeed * 255.0 / 1000.0);
                FinalSteering = (-localSteering * 27.0 / 1000.0) + 27.0; // changed from 100 to 340, from 512 to 2048
                commProtocol.SendSpeedSteeringCommand((ushort)localSpeed, (ushort)localSteering);

            }
            //make sure vehicle is enabled
            if (Enabled)
            {
                FinalFrontWheel = Math.Min(Math.Max(FinalFrontWheel, -255), 255);
                FinalSteering = Math.Min(Math.Max(FinalSteering, 0), 54); // changed from 412 to 1720, changed from 612 to 2400
            }
            else
            {
                FinalFrontWheel = 0;
                FinalSteering = 0;
            }

            // scale speed and control for microZed protocol
            FinalFrontWheel = (FinalFrontWheel * 99.0) / 255.0;



            //prepare control
            byte FrontWheelDirection = FinalFrontWheel >= 0 ? (byte)'1' : (byte)'0';
            //RearWheelDirection = Math.Abs(FinalRearWheel) < Settings.DeadZone ? (byte)0x00 : RearWheelDirection;
            //FrontWheelDirection = Math.Abs(FinalFrontWheel) < Settings.DeadZone ? (byte)0x00 : FrontWheelDirection;
#if USE_ABS
            if (this.Enabled)
            {
                bool RearLock = false;
                bool FrontLock = false;
                byte FrontLockCount = 0;
                byte RearLockCount = 0;
                const byte CycleTimeout = 2;
                RearWheelDirection = FinalRearWheel == 0 ? (byte)0x03 : RearWheelDirection;
                FrontWheelDirection = FinalFrontWheel == 0 ? (byte)0x03 : FrontWheelDirection;
                if (RearWheelDirection == (byte)0x03)
                {
                    RearWheelDirection = RearLock ? (byte)0x03 : (byte)0x00;
                    if (RearLockCount >= CycleTimeout)
                    {
                        RearLockCount = 0;
                        RearLock = !RearLock;
                    }
                    else
                        RearLockCount++;
                }
                if (FrontWheelDirection == (byte)0x03)
                {
                    FrontWheelDirection = FrontLock ? (byte)0x03 : (byte)0x00;
                    if (FrontLockCount >= CycleTimeout)
                    {
                        FrontLockCount = 0;
                        FrontLock = !FrontLock;
                    }
                    else
                        FrontLockCount++;
                }
            }
#endif

            int FrontWheelSpeed = (int)Math.Abs(FinalFrontWheel);
            int Steering = (int)(FinalSteering * 10);


            byte[] FrontWheelSpeedByte = Encoding.ASCII.GetBytes(FrontWheelSpeed.ToString());
            List<byte> FrontWheelSpeedList = FrontWheelSpeedByte.ToList();  //  MSB = index0,  LSB = index1,                  
            if (FrontWheelSpeedList.Count == 1)
                FrontWheelSpeedList.Insert(0,0x30);                     // add 0 in ascii to first item in list 

            byte[] SteeringByte = Encoding.ASCII.GetBytes(Steering.ToString());
            List<byte> SteeringList = SteeringByte.ToList();  //  MSB = index0,  LSB = index1,                  
            if (SteeringList.Count == 2)
                SteeringList.Insert(0, 0x30);                     // add 0 in ascii to first item in list 
            else if (SteeringList.Count == 1)
            {
                SteeringList.Insert(0, 0x30);                     // add 0 in ascii to first item in list 
                SteeringList.Insert(0, 0x30);                     // add 0 in ascii to first item in list 
            }



            #region old control packet
            /* old control
            //Apply Control
            //Construct Motor Package
            // #3
            byte[] _motorPackage = new byte[] {
                    0,                  3,                  111,                    111,
                    111,                111,                111,                    111,
                    111,                111,                111,                    111,
                    RearWheelDirection, RearWheelSpeed,     FrontWheelDirection,    FrontWheelSpeed
            };
            //Construct Servo Package
            // #4
            byte[] _servoPackage = new byte[] {
                    0,                      4,                      111,                    111,
                    111,                    111,                    111,                    111,
                    111,                    111,                    111,                    111,
                    SteeringAngle[1],       SteeringAngle[0], ReverseSteeringAngle[1], ReverseSteeringAngle[0]        //oj - reversed byte order output
            };
            */
            #endregion old control packet

            // new control
            //Apply Control
            //Construct Motor Package
            // #3
            byte checkSum =0x00;
            
            //send
            if (Settings.UseFPGA)
            {
                byte[] _motorPackage = new byte[] {
                0x01,                                   // Start of Transmission
                0x41,                                   // ID of Device to be controlled (ALPHABETIC)
                0x02,                                   // Start of Data (Parameters of Device)
                FrontWheelDirection,           // direction  ASCII '1-forward' or '0-backward'
                FrontWheelSpeedList[0],           // MSB - speed 0x-9x
                FrontWheelSpeedList[1],           // LSB - speed x0-x9
                0x03,                                   // End of Data
                0x00,                                   // Checksum = ~(ID + DATA) 1 BYTE!
                0x04                                    // End of Transmission
                };

                checkSum = (byte)(~(0x41 + FrontWheelDirection + FrontWheelSpeedList[0] + FrontWheelSpeedList[1]));

                _motorPackage.SetValue(checkSum, 7);
                
                byte[] _servoPackage = new byte[] {
                0x01,                                   // Start of Transmission
                0x42,                                   // ID of Device to be controlled (ALPHABETIC)
                0x02,                                   // Start of Data (Parameters of Device)
                SteeringList[0],                        // MSB - speed -ex:5
                SteeringList[1],                        // --- - speed -ex:4
                SteeringList[2],                        // LSB - speed -ex:.0
                0x03,                                   // End of Data
                0x00,                                   // Checksum = ~(ID + DATA) 1 BYTE!
                0x04                                    // End of Transmission
                };

                checkSum = (byte)(~(0x42 + SteeringList[0] + SteeringList[1] + SteeringList[2]));

                _servoPackage.SetValue(checkSum, 7);

                fpga.Send(_motorPackage);
                fpga.Send(_servoPackage);
            }

        }


        private int LidarDistance;
        public int turretServo = 135;
        public int armX = 0;
        public int armY = 0;
        private int gimbalX = 180;
        private int gimbalY = 50;
        public bool gripper = true;
        private bool armReset = false;
        private bool firstreset = false;
        private bool PackageRecieved = false;
        private int RightEncoder = 0;
        private int LeftEnconder = 0;
        private bool track = false;
        private int xDir = 0;
        private int yDir = 0;
        private const byte  turretServo_id = 0x45,
                            armX_id = 0x44,
                            armY_id = 0x43,
                            gripper_id = 0x46;

        /// <summary>
        /// Sequencial action to send control values to arm
        /// </summary>
        void GimbalTracking(int xError, int yError)
        {
            if (gimbalY + yError < 100 && gimbalY + yError > 0)
                gimbalY += yError;
            if (gimbalX + xError < 360 && gimbalX + xError > 0)
                gimbalX += xError;
        }


        void TurrentServo(int turrentServo)
        {
            byte checkSum;
            this.turretServo = turrentServo;
            commProtocol.SendArmCommand(turretServo_id, turrentServo, 100);
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
            this.armY = armY;
            commProtocol.SendArmCommand(armY_id, armY, 100);
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
            this.armX = armX;
            commProtocol.SendArmCommand(armX_id, armX, 100);
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
            if (firstreset)
            {
                PackageRecieved = false;
                LidarRecieve();
                while (!PackageRecieved) ;// Maybe create a limit for how long it waits 
                PackageRecieved = false;
                RobotArmDirections = AutonomousRetrieval(gimbalX, gimbalY, LidarDistance);
                if ((RobotArmDirections[1] >= 0 && RobotArmDirections[1] <= 270) || (RobotArmDirections[2] >= 0 && RobotArmDirections[2] <= 120) || (RobotArmDirections[0] >= 0 && RobotArmDirections[0] <= 203))
                {
                    TurrentServo(RobotArmDirections[1]);
                    ArmXSend(RobotArmDirections[2]);
                    ArmYSend(RobotArmDirections[0]);
                }
            }
        }             
        void GripperControl(bool gripper)
        {
            byte checkSum;
            this.gripper = gripper;
            if (gripper)
                commProtocol.SendArmCommand(gripper_id, 1, 100);
            else
                commProtocol.SendArmCommand(gripper_id, 0, 100);

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
            checkSum = unchecked((byte)(~(0x4A + armResetList[0] + 0x30 + 0x30)));
            _armResetPackage.SetValue(checkSum, 7);
            fpga.Send(_armResetPackage);
        }
                  

        int[] AutonomousRetrieval(int gimbalX, int gimbalY, int RangeFinderDistance)
        {
            gimbalX = gimbalX - 180;
            gimbalY = gimbalY + 40;
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

            checkSum = unchecked((byte)(~(0x4B + 0x30 + 0x30 + 0x30)));
            _LidarRecievePackage.SetValue(checkSum, 7);
            fpga.Send(_LidarRecievePackage);
        }
        #endregion arm and motor controls


        /// <summary>
        /// EventHandler when boardcast timer tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BoardCast_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            boardcastTimer.Enabled = false;
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
            BoardCast();
            boardcastTimer.Enabled = true;

        }

        int dividerCount = 0;
        private UdpClientSocket udp_armcam;
        private int x;
        private int y;

        /// <summary>
        /// Board Cast System State
        /// </summary>
        void BoardCast()
        {
            //capture current state
            UGVState state = UGVState.Capture(this);
            if (Settings.UseVision && GPSLock)
            {
                SystemState sysState = state.ToSystemState(this);
                //serialize state and send out
                //@TODO I have no idea what this is trying to do it doesn't work on my compueter
                /*
                string xml = Serialize.XmlSerialize<SystemState>(sysState);
                udp_lidar.Send(Encoding.ASCII.GetBytes(xml));
                */
            }
            if (Settings.UseCommProtocol)
            {
                if (dividerCount % Settings.PositionRate == 0)
                {
                    commProtocol.SendState(state);
                    commProtocol.sendMode(Settings.DriveMode);
                    //@TODO ARM replace the arguments and delete the dummy variables
                    //int position1 = 0;
                    //int position2 = 0;
                    //int position3 = 0;
                    //int position4 = 0;
                    //byte UGV_ARM_CONTROLER = 100;//define some where else should be changed to a settings value michael wallace 5/12/2017
                    //commProtocol.SendArmPosition(position1, position2, position3, position4, UGV_ARM_CONTROLER);
                    // sending the Base, Shoulder, Elbow, and Gripper present positions
                    //commProtocol.SendArmPosition(arm_ugv.Base.dxl_present_position, arm_ugv.Shoulder.dxl_present_position, arm_ugv.Elbow.dxl_present_position, arm_ugv.Gripper.dxl_present_position, UGV_ARM_CONTROLER);
                }
            }
            //inc
            dividerCount++;

        }

#if FPGA_Sensors
        void FPGACallback(byte[] bytes)
        {
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;

            if (SerialPackage.Check(bytes))
            {
                byte[] data = SerialPackage.Read(bytes);
            }

        }
#else
        void FPGACallback(byte[] bytes)
        {
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
            if (SerialPackage.Check(bytes))
            {
                byte[] data = SerialPackage.Read(bytes);
                if (data.Length == 16)
                {
                    string str = "";
                    foreach (var b in data)
                        str += b + " ";

                    DebugMessage.Clear();
                    DebugMessage.Append(str);

                    long TempRightEncoder = (long)data[7] + ((long)data[6]) * 256 + ((long)data[5] * 65536);
                    long TempLeftEncoder = (long)data[11] + ((long)data[10]) * 256 + ((long)data[9] * 65536);
                    RightEncoder = data[4] % 2 == 0 ? TempRightEncoder : -TempRightEncoder;
                    LeftEncoder = data[8] % 2 == 0 ? TempLeftEncoder : -TempLeftEncoder;
                }
            }
        }
#endif

        void navCallback(byte[] incoming)
        {
            byte[] preamble = { 0x55, 0x55 };
            GnavPackage raw = new GnavPackage();
            //first 2 bytes is always 0x5555, second 2 bytes is the packet type
            if (NavDebug)
                Console.WriteLine("Incoming: " + incoming.Length);
            if (preamble[0] == incoming[0] && preamble[1] == incoming[1]) //packet is good
            {
                //get the packet type as a short, switch them for the endian
                short packetType = BitConverter.ToInt16(new byte[] { incoming[3], incoming[2] }, 0);
                if (packetType == 0x504B) //0x504B, 'PK', ping
                {
                    Console.WriteLine("Nav440 - Ping!");
                }
                else
                {
                    //in packets other than ping the 5th byte is the size of the payload in bytes
                    byte size = incoming[4];
                    //the payload is next
                    byte[] payload = new byte[size];
                    for (byte i = 0; i < size; i++)
                    {
                        try
                        {
                            payload[i] = incoming[5 + i];
                        }
                        catch (Exception) { }

                    }
                    //the CRC is the last 2 bytes after the payload
                    byte[] crc = new byte[2];
                    crc[0] = incoming[incoming.Length - 2];
                    crc[1] = incoming[incoming.Length - 1];

                    switch (packetType)
                    {
                        case 0x4348: //0x4348, 'CH', echo
                            Console.Write("Nav440 - Echo: ");
                            for (byte j = 2; j < size; j++)
                            {
                                Console.Write(incoming[j]);
                            }
                            Console.WriteLine();
                            break;
                        case 0x4152: //0x4152, 'AR', Algorithm Reset Response
                            Console.WriteLine("Nav440 - Algorithm Reset");
                            break;
                        case 0x5352: //0x5352, 'SR', Software reset response
                            Console.WriteLine("Nav440 - Software Reset");
                            break;
                        case 0x5743: //0x5743, 'WC', Calibration Acknoledgement
                            Console.Write("Nav440 - Calibration Acknowledged. Type: ");
                            //The payload is the calibration type, always 2 bytes
                            short calibrationType = BitConverter.ToInt16(new byte[] { payload[0], payload[1] }, 0);
                            switch (calibrationType)
                            {
                                case 0x0009: //0x0009, Begin magnetic alignment without automatic termination.
                                    Console.WriteLine("Magnetic calibration without automatic termination, rotate device more than 360 degrees.");
                                    break;
                                case 0x000B: //0x000B, Terminate magnetic alignment
                                    Console.WriteLine("Terminate magnetic alignment.");
                                    break;
                                case 0x000C: //0x000C, Begin magnetic alignment with automatic termination
                                    Console.WriteLine("Begin magnetic alignment with automatic termination, rotate device through 380 degrees.");
                                    break;
                                case 0x000E: //0x000E, Wrote megentic calibration
                                    Console.WriteLine("Wrote magnetic calibration to EEPROM");
                                    break;
                            }
                            break;
                        case 0x4343: //0x4343, 'CC', Calibration completed
                            Console.Write("Nav440 - Calibration Completed. Type: ");
                            //The payload is the calibration type, always 2 bytes
                            calibrationType = BitConverter.ToInt16(new byte[] { payload[0], payload[1] }, 0);
                            switch (calibrationType)
                            {
                                case 0x000B: //0x000B, Terminate magnetic alignment
                                    Console.WriteLine("Terminate magnetic alignment.");
                                    break;
                                case 0x000C: //0x000C, Begin magnetic alignment with automatic termination
                                    Console.WriteLine("Begin magnetic alignment with automatic termination, rotate device through 380 degrees.");
                                    break;
                            }
                            int xHardIron = BitConverter.ToInt16(new byte[] { payload[2], payload[3] }, 0) * (int)(2.0 / Math.Pow(2, 16)); //include scale
                            int yHardIron = BitConverter.ToInt16(new byte[] { payload[4], payload[5] }, 0) * (int)(2.0 / Math.Pow(2, 16));
                            uint softIronScaleRatio = (uint)BitConverter.ToInt16(new byte[] { payload[6], payload[7] }, 0) * (uint)(2.0 / Math.Pow(2, 16));
                            Console.WriteLine("X Hard Iron: " + xHardIron);
                            Console.WriteLine("Y Hard Iron: " + yHardIron);
                            Console.WriteLine("Soft Iron Scale Ratio: " + softIronScaleRatio);
                            break;
                        case 0x1515:
                            Console.Write("Nav440 - Error: Failed input packet type.");
                            break;
                        case 0x4944: //0x4944, 'ID', identification packet
                            uint serialNumber = (uint)BitConverter.ToInt32(new byte[] { payload[0], payload[1], payload[2], payload[3] }, 0);
                            string modelString = "";
                            byte currentbyte = payload[4];
                            int i = 4;
                            while (currentbyte != 0x00)
                            {
                                modelString += currentbyte;
                                i++;
                                currentbyte = payload[i];
                            }
                            Console.WriteLine("Nav440 - Serial Number: " + serialNumber);
                            Console.WriteLine("Nav440 - Model String: " + modelString);
                            break;
                        case 0x5652: //0x5652, 'VR', Version
                            uint major = payload[0];
                            uint minor = payload[1];
                            uint patch = payload[2];
                            uint stage = payload[3];
                            uint buildNumber = payload[4];
                            Console.WriteLine("Nav440 - Version: " + major + "." + minor + "." + patch + "." + stage + "." + buildNumber);
                            break;
                        case 0x4E31: //0x4E31, 'N1', Nav Data Packet 1
                            //StringBuilder stri = new StringBuilder();
                            //stri.Append(BitConverter.ToString(payload));
                            //Console.WriteLine(stri);
                            //This is it baby! The packet with all that sweet, sweet data.
                            //The payload is 32 bytes in length.
                            //The first 2 bytes is a 2 byte int, the roll angle in radians, scaled by 2*pi/ 2^16 
                            raw.roll = BitConverter.ToInt16(new byte[] { payload[1], payload[0] }, 0) * 2.0 * Math.PI / Math.Pow(2, 16);
                            //Then pitch
                            raw.pitch = BitConverter.ToInt16(new byte[] { payload[3], payload[2] }, 0) * 2.0 * Math.PI / Math.Pow(2, 16);
                            //then yaw (true north)
                            raw.yaw = BitConverter.ToInt16(new byte[] { payload[5], payload[4] }, 0) * 2.0 * Math.PI / Math.Pow(2, 16);
                            //then the xRateCorrected in radians per sec, the x angular rate corrected
                            raw.xRateCorrected = BitConverter.ToInt16(new byte[] { payload[7], payload[6] }, 0) * 7.0 * Math.PI / Math.Pow(2, 16);
                            //then yRateCorrected
                            raw.yRateCorrected = BitConverter.ToInt16(new byte[] { payload[9], payload[8] }, 0) * 7.0 * Math.PI / Math.Pow(2, 16);
                            //then z
                            raw.zRateCorrected = BitConverter.ToInt16(new byte[] { payload[11], payload[10] }, 0) * 7.0 * Math.PI / Math.Pow(2, 16);
                            //acceleration * 20 / 2 ^ 16
                            raw.xAccel = BitConverter.ToInt16(new byte[] { payload[13], payload[12] }, 0) * 20.0 / Math.Pow(2, 16);
                            //then east velocity
                            raw.yAccel = BitConverter.ToInt16(new byte[] { payload[15], payload[14] }, 0) * 20.0 / Math.Pow(2, 16);
                            //then down velocity
                            raw.zAccel = BitConverter.ToInt16(new byte[] { payload[17], payload[16] }, 0) * 20.0 / Math.Pow(2, 16);
                            //then north velocity scaled by 512/2^16 in meter per second
                            raw.nVel = BitConverter.ToInt16(new byte[] { payload[19], payload[18] }, 0) * 512.0 / Math.Pow(2, 16);
                            //then east velocity
                            raw.eVel = BitConverter.ToInt16(new byte[] { payload[21], payload[20] }, 0) * 512.0 / Math.Pow(2, 16);
                            //then down velocity
                            raw.dVel = BitConverter.ToInt16(new byte[] { payload[23], payload[22] }, 0) * 512.0 / Math.Pow(2, 16);
                            //GPS logitude in radians scaled by 2*pi/2^32
                            raw.longitudeGPS = BitConverter.ToInt32(new byte[] { payload[27], payload[26], payload[25], payload[24] }, 0) * 360.0 / Math.Pow(2, 32);
                            //GPS latitude
                            raw.latitudeGPS = BitConverter.ToInt32(new byte[] { payload[31], payload[30], payload[29], payload[28] }, 0) * 360.0 / Math.Pow(2, 32);
                            //GPS altitude
                            raw.altitudeGPS = BitConverter.ToInt16(new byte[] { payload[33], payload[32] }, 0) * Math.Pow(2, 14) / Math.Pow(2, 16);
                            //xRateTemp, 2 unsigned bytes
                            raw.xRateTemp = BitConverter.ToInt16(new byte[] { payload[35], payload[34], }, 0) * (int)(200 / Math.Pow(2, 16));
                            //time ITOW
                            raw.timeitow = BitConverter.ToInt16(new byte[] { payload[37], payload[36], payload[39], payload[38] }, 0);
                            //bit status
                            raw.bitstatus = BitConverter.ToInt16(new byte[] { payload[41], payload[40], }, 0);

                            //Console.WriteLine("Longitude: " + raw.longitudeGPS);
                            //Console.WriteLine("Latitude: " + raw.latitudeGPS);
                            imu.Update(raw);

                            break;
                    }
                }
            }
            if (NavDebug)
            {
                StringBuilder str = new StringBuilder(BitConverter.ToString(incoming));
                str.Append(BitConverter.ToString(incoming));
                Console.WriteLine(str.ToString());
            }
        }

        #endregion Private Methods

        #region Settings
        /// <summary>
        /// UGV Setting class
        /// </summary>
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
            public string CommPort = "COM0";
            public int CommBaud = 57600;
            public string CommAddress = "";
            public bool UseUGVXbee = true;
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
            public UGV.DriveMode DriveMode = UGV.DriveMode.LocalControl;


            /// <summary>
            /// Dead Zone for slow speed to prevent hardward damage
            /// </summary>
            public double DeadZone = 20;

            public byte UGVProtonetNodeID = 46;
            public sbyte AttiudeRate = 20;
            public sbyte PositionRate = 20;

        }

        #endregion Settings

        #region SerialPackage

        /// <summary>
        /// Extended class for UGV
        /// </summary>
        public class SerialPackage
        {
            /// <summary>
            /// Package a byte array to Serial Shift prevented package
            /// </summary>
            /// <param name="bytes"></param>
            /// <returns></returns>
            public static byte[] Package(byte[] bytes)
            {
                //read bytes to list
                List<byte> bs = bytes.ToList();
                //adjust length to divisible by 4
                while (bs.Count % 4 > 0)
                    bs.Add(0);
                //insert 0 in front
                for (int i = bs.Count - 4; i >= 0; i -= 4)
                    bs.Insert(i, 0);
                //add end
                bs.AddRange(new byte[] { 0xFB, 0xFC, 0xFD, 0xFE, 0xFF });
                //return result
                return bs.ToArray();
            }

            /// <summary>
            /// Check if a byte array agree with 
            /// </summary>
            /// <param name="bytes"></param>
            /// <returns></returns>
            public static bool Check(byte[] bytes)
            {
                //prevent null
                if (bytes == null)
                    return false;
                //if divisible by 5
                if (bytes.Length % 5 != 0)
                    return false;
                //check starter
                for (int i = 0; i < bytes.Length; i += 5)
                    if (bytes[i] != 0)
                        return false;
                return true;
            }

            /// <summary>
            /// Read data from a serial package
            /// </summary>
            /// <param name="bytes"></param>
            /// <returns></returns>
            public static byte[] Read(byte[] bytes)
            {
                //return null if it is not serial package
                if (!Check(bytes))
                    return null;
                //prepare empty data list
                List<byte> data = new List<byte>();
                //turn array of bytes to list
                List<byte> raw = bytes.ToList();
                //read
                while (raw.Count > 4)
                {
                    raw.RemoveAt(0);
                    data.AddRange(raw.GetRange(0, 4));
                    raw.RemoveRange(0, 4);
                }
                return data.ToArray();
            }
        }

        #endregion SerialPackage

        /// <summary>
        /// Battery Status
        /// </summary>
        public struct BatteryStatus
        {
            public float Voltage3_3V;
            public float Voltage5V;
            public float Voltage12V;
        }
    }
}