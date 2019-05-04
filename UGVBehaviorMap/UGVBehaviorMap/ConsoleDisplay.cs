#define USE_WPF
//#define logging
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Configuration;
using System.Reflection;



//GITHUB
namespace UGVBehaviorMap
{
    public class ConsoleDisplay
    {
        /// <summary>
        /// UGV object
        /// </summary>
        public static NGCP.UGV.UGV ugv;
        /// <summary>
        /// Whether dispaly map or not
        /// </summary>
        public static bool DisplayMap;
        /// <summary>
        /// Centering for map display
        /// </summary>
        public static bool AlwaysCenter;
        /// <summary>
        /// UI timer
        /// </summary>
        static System.Timers.Timer timer;
        /// <summary>
        /// Local speed change
        /// </summary>
        const int SpeedChange = 50;
        /// <summary>
        /// Local steer change
        /// </summary>
        const int SteerChange = 100;
#if logging
        public static System.IO.StreamWriter logFile = new System.IO.StreamWriter(@"C:\Users\UGV_usr\log.txt");
        public static System.IO.StreamWriter encoderLogFile = new System.IO.StreamWriter(@"C:\Users\UGV_usr\encoderLog.txt");
#endif

#if USE_WPF
        public static void StartMain(string[] args)
#else
        public static void Main(string[] args)
#endif
        {
            //System.Diagnostics.Debugger.Break();
            //Set the window size, it's in chars for the width and lines for the height, not pixels.
            Console.SetWindowSize(100, 32);
            //initialize
            try {
                ugv = new NGCP.UGV.UGV();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(TimerTick);

            //read configuration
            Dictionary<string, string> Settings = new Dictionary<string, string>
            {
                {"UseFPGA", "False"},
                {"FPGAPort", "COM7"},
                {"FPGABaud", "9600"},
                {"UseUGVXbee", "False"},
                {"CommPort", "COM3"},
                {"CommBaud", "57600"},
                {"CommAddress", "0013A2004067E4AE"},
                {"UseNav", "False"},
                {"NavPort", "COM9"},
                {"NavBaud", "57600"},
                {"UseArm", "False"},
                {"ArmPort", "COM11"},
                {"ArmBaud", "8000"},
                {"UseVision", "True"},
                {"VisionHostIP", "127.0.0.1"},
                {"VisionHostPort", "8009"},
                {"UseCamera", "False" },
                {"CameraHostPort", "5501" },
                {"VisionTargetIP", "127.0.0.1"},
                {"VisionTargetPort", "5501"}
            };

            try
            {
                //read ini file
                ReadIni(Settings);
            }
            catch (UnauthorizedAccessException uae)
            {
                Console.WriteLine("An Unauthorized Access Exception happened:");
                Console.WriteLine("Please either remove the existing setting in registry: " + RegPath + " or start this code as admin.");
                Console.WriteLine("Exception: " + uae.ToString());
                Console.ReadLine();
                return;
            }

            //read configuration
            ugv.Settings.UseFPGA = Boolean.Parse(Settings["UseFPGA"]);
            ugv.Settings.UseUGVXbee = Boolean.Parse(Settings["UseUGVXbee"]);
            ugv.Settings.UseNav = Boolean.Parse(Settings["UseNav"]);
            ugv.Settings.UseVision = Boolean.Parse(Settings["UseVision"]);
            ugv.Settings.UseCamera = Boolean.Parse(Settings["UseCamera"]);
            ugv.Settings.FPGAPort = Settings["FPGAPort"];
            ugv.Settings.FPGABaud = Convert.ToInt32(Settings["FPGABaud"]);
            ugv.Settings.CommPort = Settings["CommPort"];
            ugv.Settings.CommBaud = Convert.ToInt32(Settings["CommBaud"]);
            ugv.Settings.CommAddress = (Settings["CommAddress"]);
            ugv.Settings.NavPort = Settings["NavPort"];
            ugv.Settings.NavBaud = Convert.ToInt32(Settings["NavBaud"]);
            ugv.Settings.ArmPort = Settings["ArmPort"];
            ugv.Settings.ArmBaud = Convert.ToInt32(Settings["ArmBaud"]);
            ugv.Settings.UseArm = Boolean.Parse(Settings["UseArm"]);
            ugv.Settings.VisionHostIP = Settings["VisionHostIP"];
            ugv.Settings.VisionHostPort = Convert.ToInt32(Settings["VisionHostPort"]);
            ugv.Settings.CameraHostPort = Convert.ToInt32(Settings["CameraHostPort"]);
            ugv.Settings.VisionTargetIP = Settings["VisionTargetIP"];
            ugv.Settings.VisionTargetPort = Convert.ToInt32(Settings["VisionTargetPort"]);
            ugv.Settings.DriveMode = NGCP.UGV.UGV.DriveMode.LocalControl;

            bool DirectStart = false;

            //wait for s to start
            ConsoleKeyInfo keyInfo;

            if (!DirectStart)
            {
                do
                {
                    //get port names
                    List<string> PortNames = SerialPort.GetPortNames().ToList();
                    Console.Clear();
                    //show config
                    Console.WriteLine("Please Verify setting before start:");
                    foreach (var kv in Settings)
                    {
                        //check port exist
                        string exist = PortNames.Contains(kv.Value) ? " - Port Exist" : "";
                        Console.WriteLine("\t{0,-20} = \t{1,-10}\t{2,-10}", kv.Key, kv.Value, exist);
                        //Console.WriteLine("\t" + kv.Key + " = \t" + kv.Value + "\t" + exist);
                    }
                    Console.WriteLine("Dynamic Library Info:");
                    Console.WriteLine(GetDllInfo(typeof(UGV.Core.IO.Link)));
                    Console.WriteLine(GetDllInfo(typeof(NGCP.UGV.UGV)));
                    Console.WriteLine("Hit S to start...");
                    keyInfo = Console.ReadKey();
                }
                while (keyInfo.Key != ConsoleKey.S);
            }
            //Logging stuff
            #if logging
            System.Timers.Timer logTimer = new System.Timers.Timer(1000);
            logTimer.Elapsed += new System.Timers.ElapsedEventHandler(LogTick);
            #endif

            //start ugv
            try
            {
                ugv.Start();
                timer.Start();
                #if logging
                logTimer.Start();
                #endif
            }
            catch (Exception ex)
            {
                timer.Stop();
                Console.WriteLine("Cannot Start UGV:\n\t" + ex.ToString());
            }

#region Waypoint and Boundary Definition
            UGV.Core.Navigation.WayPoint WaypointA = new UGV.Core.Navigation.WayPoint(34.0588093, -117.8216925,0);
            UGV.Core.Navigation.WayPoint WaypointB = new UGV.Core.Navigation.WayPoint(34.0590510, -117.8219402,0);
            UGV.Core.Navigation.WayPoint WaypointC = new UGV.Core.Navigation.WayPoint(34.0594426, -117.8220626,0);

            //UGV.Core.Navigation.WayPoint WaypointB = new UGV.Core.Navigation.WayPoint(34.0590760, -117.8213999, 0);
            /*   UGV.Core.Navigation.WayPoint WaypointC = new UGV.Core.Navigation.WayPoint(34.0590225, -117.8216451, 0);
               UGV.Core.Navigation.WayPoint WaypointD = new UGV.Core.Navigation.WayPoint(34.0587586, -117.8218449, 0);
   */

            ugv.Waypoints.Enqueue(WaypointA);
            
            ugv.Waypoints.Enqueue(WaypointB);
            ugv.Waypoints.Enqueue(WaypointC);

            ////ugv.Waypoints.Enqueue(WaypointD);
            //ugv.TargetWaypoint = WaypointC;

            List<UGV.Core.Navigation.WayPoint> KLotBoundary = new List<UGV.Core.Navigation.WayPoint>()
            {
                new UGV.Core.Navigation.WayPoint(34.05114, -117.81804, 0),// 34.05920, -117.82170, 0),
                new UGV.Core.Navigation.WayPoint(34.05087, -117.81746, 0),//34.05940, -117.82125, 0),
                new UGV.Core.Navigation.WayPoint(34.05107, -117.81746, 0),//34.05888, -117.82147, 0)
                new UGV.Core.Navigation.WayPoint(34.05134, -117.81804, 0)//34.05900, -117.82115, 0),
            };
            List<UGV.Core.Navigation.WayPoint> EMBoundary = new List<UGV.Core.Navigation.WayPoint>()
            {
                new UGV.Core.Navigation.WayPoint(34.05920, -117.82170, 0),
                new UGV.Core.Navigation.WayPoint(34.05940, -117.82125, 0),
                new UGV.Core.Navigation.WayPoint(34.05900, -117.82115, 0),
                new UGV.Core.Navigation.WayPoint(34.05888, -117.82147, 0)
            };
            List<UGV.Core.Navigation.WayPoint> SLOBoundary = new List<UGV.Core.Navigation.WayPoint>()
            {
                new UGV.Core.Navigation.WayPoint(35.328238, -120.752526, 0),
                new UGV.Core.Navigation.WayPoint(35.328314, -120.752398, 0),
                new UGV.Core.Navigation.WayPoint(35.327938, -120.752006, 0),
                new UGV.Core.Navigation.WayPoint(35.328000, -120.751923, 0),
                new UGV.Core.Navigation.WayPoint(35.329025, -120.752930, 0),
                new UGV.Core.Navigation.WayPoint(35.328957, -120.753015, 0),
                new UGV.Core.Navigation.WayPoint(35.328552, -120.752635, 0),
                new UGV.Core.Navigation.WayPoint(35.328475, -120.752757, 0)
            };

            List<UGV.Core.Navigation.WayPoint> NGCWoodlandHillsBoundary = new List<UGV.Core.Navigation.WayPoint>() 
            {
                new UGV.Core.Navigation.WayPoint(34.170803, -118.594467,0),
                new UGV.Core.Navigation.WayPoint(34.170806, -118.594306, 0),
                new UGV.Core.Navigation.WayPoint(34.170136, -118.5943, 0),
                new UGV.Core.Navigation.WayPoint(34.170136, -118.594461, 0),
            };
            List<UGV.Core.Navigation.WayPoint> GymBoundary = new List<UGV.Core.Navigation.WayPoint>()
            {
                new UGV.Core.Navigation.WayPoint(34.054943, -117.8196, 0),
                new UGV.Core.Navigation.WayPoint(34.054648, -117.819438, 0),
                new UGV.Core.Navigation.WayPoint(34.054428, -117.820116, 0),
                new UGV.Core.Navigation.WayPoint(34.054706, -117.820269, 0)
            };
            List<UGV.Core.Navigation.WayPoint> QuadBoundary = new List<UGV.Core.Navigation.WayPoint>()
            {
                new UGV.Core.Navigation.WayPoint(34.058642, -117.823358, 0),
                new UGV.Core.Navigation.WayPoint(34.058419, -117.823070, 0),
                new UGV.Core.Navigation.WayPoint(34.058323, -117.823114, 0),
                new UGV.Core.Navigation.WayPoint(34.058527, -117.823507, 0)
            };

            //debug safezone
            List<UGV.Core.Navigation.WayPoint> KLotSafeZone = new List<UGV.Core.Navigation.WayPoint>()
            {
                new UGV.Core.Navigation.WayPoint(34.051103, -117.817660, 0),//34.05925, -117.82160, 0),
                new UGV.Core.Navigation.WayPoint(34.051083, -117.817662, 0),//34.05935, -117.82135, 0),
                new UGV.Core.Navigation.WayPoint(34.051111, -117.817727, 0),//34.05923, -117.82132, 0),
                new UGV.Core.Navigation.WayPoint(34.051136, -117.817726, 0)//34.05913, -117.82157, 0)
            };
            List<UGV.Core.Navigation.WayPoint> EMSafeZone = new List<UGV.Core.Navigation.WayPoint>()
            {
                new UGV.Core.Navigation.WayPoint(34.05925, -117.82160, 0),
                new UGV.Core.Navigation.WayPoint(34.05935, -117.82135, 0),
                new UGV.Core.Navigation.WayPoint(34.05923, -117.82132, 0),
                new UGV.Core.Navigation.WayPoint(34.05913, -117.82157, 0)
            };
            List<UGV.Core.Navigation.WayPoint> SLOSafeZone = new List<UGV.Core.Navigation.WayPoint>()
            {
                new UGV.Core.Navigation.WayPoint(35.328469, -120.752738, 0),
                new UGV.Core.Navigation.WayPoint(35.328250, -120.752524, 0),
                new UGV.Core.Navigation.WayPoint(35.328319, -120.752420, 0),
                new UGV.Core.Navigation.WayPoint(35.328531, -120.752628, 0)
                //new UGV.Core.Navigation.WayPoint(35.328238, -120.752526, 0),
                //new UGV.Core.Navigation.WayPoint(35.328314, -120.752398, 0),
                //new UGV.Core.Navigation.WayPoint(35.328552, -120.752635, 0),
                //new UGV.Core.Navigation.WayPoint(35.328475, -120.752757, 0)
            };

            List<UGV.Core.Navigation.WayPoint> NGCWoodlandHillsSafeZone = new List<UGV.Core.Navigation.WayPoint>() 
            {
                new UGV.Core.Navigation.WayPoint(34.170803, -118.594467,0),
                new UGV.Core.Navigation.WayPoint(34.170806, -118.594306, 0),
                new UGV.Core.Navigation.WayPoint(34.170725, -118.594306, 0),
                new UGV.Core.Navigation.WayPoint(34.170728, -118.594469,0)
            };

            List<UGV.Core.Navigation.WayPoint> GymSafeZone = new List<UGV.Core.Navigation.WayPoint>()
            {
                new UGV.Core.Navigation.WayPoint(34.054943, -117.8196, 0),
                new UGV.Core.Navigation.WayPoint(34.054648, -117.819438, 0),
                new UGV.Core.Navigation.WayPoint(34.054643, -117.819513, 0),
                new UGV.Core.Navigation.WayPoint(34.054914, -117.819681, 0)
                
            };
            List<UGV.Core.Navigation.WayPoint> QuadSafeZone = new List<UGV.Core.Navigation.WayPoint>()
            {
                new UGV.Core.Navigation.WayPoint(34.058527, -117.823507, 0),
                new UGV.Core.Navigation.WayPoint(34.058643, -117.823362, 0),
                new UGV.Core.Navigation.WayPoint(34.058591, -117.823289, 0),
                new UGV.Core.Navigation.WayPoint(34.058510, -117.823405, 0)

            };

            var KLotDefault = new UGV.Core.Navigation.WayPoint(34.05107, -117.81746, 0);
            var EMDefault = new UGV.Core.Navigation.WayPoint(34.05913, -117.82157, 0);
            var SLODefault = new UGV.Core.Navigation.WayPoint(35.328455, -120.752294, 0);
            var NGCWoodlandHillsDefault = new UGV.Core.Navigation.WayPoint(34.170764, 118.594394, 0);
            var GymDefault = new UGV.Core.Navigation.WayPoint(34.054943, -117.8196, 0);
            var QuadDefault = new UGV.Core.Navigation.WayPoint(34.058577, -117.823388, 0);

            ugv.Settings.IMUGain = 0.5;
# endregion

            // Set field
            ugv.Boundary = EMBoundary;
            ugv.SafeZone = EMSafeZone;
            ugv.DefaultLocation = EMDefault;

            // added this line to change state of the ugv and start in the grabPayloadManual method
            ugv.State = NGCP.UGV.UGV.DriveState.SearchTarget;

            //check for enable
            do
            {
                keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.E)
                    ugv.Enabled = !ugv.Enabled;
                else if (keyInfo.Key == ConsoleKey.F1)
                {
                    ugv.Settings.DriveMode = NGCP.UGV.UGV.DriveMode.LocalControl;
                    ugv.State = NGCP.UGV.UGV.DriveState.SearchTarget;
                }
                else if (keyInfo.Key == ConsoleKey.F2)
                    ugv.Settings.DriveMode = NGCP.UGV.UGV.DriveMode.SemiAutonomous;
                else if (keyInfo.Key == ConsoleKey.F3)
                    ugv.Settings.DriveMode = NGCP.UGV.UGV.DriveMode.Autonomous;
                else if (keyInfo.Key == ConsoleKey.W)
                    ugv.LocalSpeed = ugv.LocalSpeed >= 1000 ? 1000 : ugv.LocalSpeed + SpeedChange;
                else if (keyInfo.Key == ConsoleKey.S)
                    ugv.LocalSpeed = ugv.LocalSpeed <= -1000 ? -1000 : ugv.LocalSpeed - SpeedChange;
                else if (keyInfo.Key == ConsoleKey.A)
                    ugv.LocalSteering = ugv.LocalSteering <= -1000 ? -1000 : ugv.LocalSteering - SteerChange;
                else if (keyInfo.Key == ConsoleKey.D)
                    ugv.LocalSteering = ugv.LocalSteering >= 1000 ? 1000 : ugv.LocalSteering + SteerChange;
                else if (keyInfo.Key == ConsoleKey.R)
                    ugv.ResetMission();
                else if (keyInfo.Key == ConsoleKey.P)
                {
                    UGV.Core.Navigation.WayPoint temp;
                    ugv.Waypoints.TryDequeue(out temp);
                }
                else if (keyInfo.Key == ConsoleKey.Z)
                {
                    ugv.Settings.DriveMode = NGCP.UGV.UGV.DriveMode.LocalControl;
                    ugv.LocalSpeed = 0;
                    ugv.LocalSteering = 0;
                }
                else if (keyInfo.Key == ConsoleKey.M)
                {
                    DisplayMap = !DisplayMap;
                }
                else if (keyInfo.Key == ConsoleKey.C)
                {
                    ugv.LocalSteering = 0;
                    //AlwaysCenter = !AlwaysCenter;
                }
                else if (keyInfo.Key == ConsoleKey.I)
                {
                    ZoomIn = true;
                }
                else if (keyInfo.Key == ConsoleKey.O)
                {
                    ZoomOut = true;
                }
#if logging
                else if (keyInfo.Key == ConsoleKey.L)
                {
                    logTimer.Close();
                    logFile.Close();
                    encoderLogFile.Close();
                }
#endif
            }
            while (true);
        }

        public static bool ZoomIn = false;

        public static bool ZoomOut = false;

#if logging
        /// <summary>
        /// Log timer tick event
        /// </summary>
        static void LogTick(object sender, EventArgs e)
        {
            logFile.WriteLine(ugv.Latitude + "\t" + ugv.Longitude + "\t" + ugv.Roll + "\t" + ugv.Pitch + "\t" + ugv.Heading);
            encoderLogFile.WriteLine(ugv.EncoderLat + "\t" + ugv.EncoderLon + "\t" + ugv.EncoderSpeed + "\t" + ugv.EncoderTurn + "\t" + ugv.LeftClicks + "\t" + ugv.RightClicks);
        }
#endif
        /// <summary>
        /// UI timer tick event
        /// </summary>
        static void TimerTick(object sender, EventArgs e)
        {
            StringBuilder output = new StringBuilder();
            output.Append("Status Update " + DateTime.Now + " (Auth Node ID: " + /*ugv.AuthNodeID +*/ ")");
            output.Append("\n(Mode : " + ugv.Settings.DriveMode + ") (State : " + ugv.State + ")");
            output.Append("\n--- IO Link Status ---\n");
            foreach (var kv in ugv.Links)
                output.Append("<" + kv.Key + " = " + (kv.Value.IsActive() ? "ON" : "OFF") + ">");
            //output.Append("\n--- Orientation ---   GPS Time = " + ugv.GPSTime);
            output.Append(String.Format("\n(Lat, Long) = {0:N9} {1:N9}", ugv.Latitude, ugv.Longitude));
            output.Append(String.Format("\n(Pitch, Roll, Heading) = {0:N5} {1:N5} {2:N5}"
                , (ugv.Pitch * 180.0 / Math.PI), (ugv.Roll * 180.0 / Math.PI), (ugv.Heading * 180.0 / Math.PI)));
            //output.Append("\nGround Speed = " + ugv.GroundSpeed + ", Track Angle = " + ugv.TrackAngle + ", GPS Lock = " + ugv.GPSLock + ", " + ugv.SatelliteCount);
            output.Append("\n--- Navigation ---    Inside Boundary: " + ugv.InsideBoundary + " close:" + ugv.CloseBoundary + " Inside SafeZone: " + ugv.InsideSafeZone);
            //output.Append(String.Format("\nEncoder: Speed {0, 7} Steering {1, 7} ", ugv.EncoderSpeed, ugv.EncoderTurn));
            //output.Append(String.Format("\nEncoder: Latitude {0, 7} Longitude {1, 7} ", ugv.EncoderLat, ugv.EncoderLon));
            //output.Append(String.Format("\nEncoder Clicks: Left {0, 7} Right {1, 7} ", ugv.LeftClicks, ugv.RightClicks));
            output.Append(String.Format("\nGPS:    Distance = {0:N5} m, Bearing = {1:N5} | {2:N5} degree"
                , ugv.NextWaypointDistance, (ugv.NextWaypointBearing * 180.0 / Math.PI), (ugv.NextWaypointBearingError * 180.0 / Math.PI)));
            output.Append(String.Format("\nLine Count {0, 7}", ugv.LineCount));
            output.Append(String.Format("\nAvoidance Vector: Angle {0, 7}, Magnitude {1, 7}", ugv.AvoidanceVector.angle * 180.0 / Math.PI, ugv.AvoidanceVector.magnitude));
            output.Append(String.Format("\nSum Vector: Angle {0, 7}, Magnitude {1, 7}", ugv.SumVector.angle * 180.0 / Math.PI, ugv.SumVector.magnitude));
            output.Append(String.Format("\nTarget Found {0, 7}", ugv.TargetFound));
            output.Append(String.Format("\nTarget Vector: Angle {0, 7}, Distance {1, 7}", ugv.TargetAngle, ugv.TargetDistance));
            //output.Append(String.Format("\nTarget Waypoint: Lat {0, 7}, Long {1, 7}", ugv.TargetWaypoint.Lat, ugv.TargetWaypoint.Long));
            output.Append("\n--- Control ---    Safety Switch: " + !ugv.Enabled + " Protonet Override: " + ugv.CommOverride);
            output.Append(String.Format("\nLocal: Speed {0, 7} Steer {1, 7} ", ugv.LocalSpeed, ugv.LocalSteering));
            output.Append(String.Format("\nAuton: Speed {0, 7} Steer {1, 7} ", ugv.Speed, ugv.Steering));
            output.Append(String.Format("\nFinal: Speed {0, 7} Steer {1, 7} ", ugv.FinalFrontWheel, ugv.FinalSteering));
            //output.Append(String.Format("\nPayload X coor  {0, 7} Payload Y coor {1, 7} ", ugv.payloadx, ugv.payloady));
            output.Append(String.Format("\nTurret Servo: {0}", ugv.turretServo));
            output.Append(String.Format("\nArm X:  {0}", ugv.armX));
            output.Append(String.Format("\nArm Y:  {0}", ugv.armY));
            output.Append(String.Format("\nGripper:  {0}", ugv.gripper));

            ugv.debugUGV();
            output.Append("\n--- Debug ---  " + ugv.DebugMessage);
            {
                //locked on location
                if (ugv.TargetLockedLocation != null)
                {
                    output.Append(String.Format("\nLock On: LAT:{0:N7} LON:{1:N7}"
                        , ugv.TargetLockedLocation.Lat, ugv.TargetLockedLocation.Long));
                    output.Append(" Dropped: " + ugv.TargetDropped);
                }
                //get waypoint count
                int c = Math.Min(4, ugv.Waypoints.Count);
                output.Append("\nFirst " + c + " Waypoints in Queue (" + ugv.Waypoints.Count + "): ");
                //read waypoing, but avoid crash
                try
                {
                    if (c > 0)
                        foreach (var waypoint in ugv.Waypoints.ToList().GetRange(0, c))
                            output.Append(String.Format("\n  LAT:{0:N7} LON:{1:N7} - {2,-10} "
                                , waypoint.Lat, waypoint.Long, waypoint.GUID.ToString()));
                }
                catch (Exception) { }
            }

            Console.Clear();
            Console.WriteLine(output);
            Console.WriteLine("(E = Safety Switch) (P = Bypass 1st Waypoint) (WASD = Local Control)");
            Console.WriteLine("(M = Map) (C = Center Tracking) (Z = Zero Control)");
            Console.WriteLine("(F1 = Local Mode) (F2 = SemiAuto Mode) (F3 = Auto Mode) (R = Reset Mission)");
        }

#region Registry

        static string RegPath = @"SOFTWARE\NGCP\UGV";

        /// <summary>
        /// Read registry from app config
        /// </summary>
        /// <param name="Settings"></param>
        static void ReadReg(Dictionary<string, string> Settings)
        {
            bool NeedRewrite = false;
            foreach (var kv in Settings.ToArray())
            {
                var subkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegPath);
                if (subkey == null)
                {
                    NeedRewrite = true;
                    break;
                }
                var value = subkey.GetValue(kv.Key);
                if (value != null && value.ToString().Length > 0)
                    Settings[kv.Key] = value.ToString();
                else
                    NeedRewrite = true;
            }
            //write config in case of missing
            if (NeedRewrite)
                WriteReg(Settings);
        }

        /// <summary>
        /// Write registry to app config
        /// </summary>
        /// <param name="Settings"></param>
        static void WriteReg(Dictionary<string, string> Settings)
        {
            var subkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegPath);
            //create if not exist
            if (subkey == null)
                subkey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(RegPath);
            foreach (var kv in Settings)
                subkey.SetValue(kv.Key, kv.Value);
        }

#endregion Registry

        /// <summary>
        /// Read registry from an INI file.
        /// </summary>
        /// <param name="Settings"></param>
        static void ReadIni(Dictionary<string, string> Settings)
        {
            var file = new IniFile("Settings.ini");
            foreach (var kv in Settings.ToArray())
            {
                var value = file.Read(kv.Key, "UGV");
                if (value != null && value.ToString().Length > 0)
                {
                    Settings[kv.Key] = value.ToString();
                }
            }
        }

#region AppConfig

        /// <summary>
        /// Read Configurationf from app config
        /// </summary>
        /// <param name="Settings"></param>
        /// <returns></returns>
        static void ReadConfig(Dictionary<string, string> Settings)
        {
            bool NeedRewrite = false;
            foreach (var kv in Settings.ToArray())
            {
                string value = ConfigurationManager.AppSettings.Get(kv.Key);
                if (value != null && value.Length > 0)
                    Settings[kv.Key] = value;
                else
                    NeedRewrite = true;
            }
            //write config in case of missing
            if (NeedRewrite)
                WriteConfig(Settings);
        }

        /// <summary>
        /// Write Configuration to app config
        /// </summary>
        /// <param name="Settings"></param>
        static void WriteConfig(Dictionary<string, string> Settings)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            foreach (var kv in Settings)
            {
                if (ConfigurationManager.AppSettings.AllKeys.ToList().Contains(kv.Key))
                    ConfigurationManager.AppSettings.Set(kv.Key, kv.Value);
                else
                    config.AppSettings.Settings.Add(kv.Key, kv.Value);
            }
            config.Save();
        }

#endregion AppConfig

#region Dll Info

        /// <summary>
        /// Get printable data of dll
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetDllInfo(Type type)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(type.Assembly.GetName().Name + ", " + type.Assembly.GetName().Version + ", ");
            //read dll path
            string dllPath = type.Assembly.GetName().CodeBase.Replace(@"file:///", "");
            if (System.IO.File.Exists(dllPath))
                sb.Append(System.IO.File.GetLastWriteTime(dllPath));
            return sb.ToString();
        }

#endregion Dll Info
    }
}
