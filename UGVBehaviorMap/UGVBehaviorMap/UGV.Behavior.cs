//#define USE_MISSION
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using UGV.Core.Sensors;
using UGV.Core.IO;
using UGV.Core.Maths;
using UGV.Core.Navigation;
using System.IO;

namespace NGCP.UGV
{
    /// <summary>
    /// This file describes the UGV autonomous behaviors properties and methods
    /// </summary>
    public partial class UGV
    {
        #region Behavior Related Properties

        /// <summary>
        /// pid control for steering
        /// </summary>
        PID SteerPID = new PID(50, 250, 100, 10, 1000);     //not being used(all commented out)
            

        /// <summary>
        /// Drive state
        /// </summary>
        public DriveState State = DriveState.SearchTarget;

        /// <summary>
        /// Location where the target locked
        /// </summary>
        public WayPoint TargetLockedLocation = null;        //Come back and check if its being used

        /// <summary>
        /// Target vector
        /// </summary>
        public Vector2d WaypointVector = new Vector2d(0, 0);

        /// <summary>
        /// Sum vector
        /// </summary>
        public Vector2d SumVector = new Vector2d(0, 0);

        /// <summary>
        /// Drive state of UGV
        /// </summary>
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

        /// <summary>
        /// Sleep Time each cycle
        /// </summary>
        const int SleepTime = 100;

        /// <summary>
        /// Full Speed for Autonomous
        /// </summary>
        const double FullSpeed = 800;

        /// <summary>
        /// Steering Ratio
        /// </summary>
        const double SteerRatio = 1000.0 ;//changed from 1000*3 to just 1000

        /// <summary>
        /// Speed Ratio When outside boundary
        /// </summary>
        const double OutsideSpeedRatio = 0.5;

        /// <summary>
        /// Steer Ratio When outside boundary
        /// </summary>
        const double OutsideSteerRatio = 2.0;

        /// <summary>
        /// How close to check for obstacles in meters
        /// </summary>
        const int AvoidanceThreshold = 10;                  //Come back and check if its being used

        public long StartLockTime = 0;
        public long CurrentTime = 0;

        /// <summary>
        /// The distance considered hit a waypoint
        /// </summary>
        const double ReachWaypointZone = 3.0;               //measured in meters

        /// <summary>
        /// Whether to use search path generated from autonomous behavior method
        /// </summary>
        bool usePathGen = false; //false for testing purposes

        #region Properties for SearchTarget() Behaivor

        /// <summary>
        /// The limit of apporaching zone in mm
        /// </summary>
        const int ApporachingZone = 8000;

        /// <summary>
        /// The distance considered as at target in mm 
        /// </summary>
        const int TargetZone = 4000;

        /// <summary>
        /// if system is navigate relatively
        /// </summary>
        bool RelativeNavFlag = false;

        bool goToSafe = false;

        /// <summary>
        /// RelativeNav count
        /// </summary>
        int RelativeNavCount = 0;

        /// <summary>
        /// RelativeNav for 1 seconds
        /// </summary>
        const int RelativeNavTimeout = 1000 / SleepTime;

        /// <summary>
        /// Apporaching count
        /// </summary>
        int TargetApporachCount = 0;

        /// <summary>
        /// track target for 0.5 seconds
        /// </summary>
        const int TargetApporachTimeout = 500 / SleepTime;

        #endregion

        #region Properties for LockTarget Behavior

        //target lost 3 second then search again
        const int TargetLostTimeout = 3000;

        #endregion

        #region Properties for Drive Away From Target Behavior

        int SafeDistanceCount = 0;

        //track target for 30 seconds
        const int SafeDistanceTimeout = 1000 / SleepTime;

        /// <summary>
        /// The distance considered as far enough from target
        /// </summary>
        const int SafeDistance = 6000;

        #endregion

        #region Properties for WaitDrop Behavior

        /// <summary>
        /// count tick for payload drop
        /// </summary>
        int PayloadDropCount = 0;

        /// <summary>
        /// wait payload drop for 30 sec
        /// </summary>
        const int PayloadDropTimeout = 30000 / SleepTime;

        #endregion

        #endregion Behavior Related Properties

        #region Methods for Autonomous Behavior
        /// <summary>
        /// Method to constantly update changes in system state
        /// </summary>
        void DoWork()
        {
#if USE_MISSION
            //FUTURE MISSION Statement control
#else
            while (true)
            {
                //System.Diagnostics.Debugger.Break();
                if (State == DriveState.GenerateSearchPath)
                    GenerateSearchPath();
                else if (State == DriveState.SearchTarget)
                    SearchTarget();
                else if (State == DriveState.LockTarget)
                    LockTarget();
                //else if (State == DriveState.SearchPayload)
                    //SearchPayload();
                else if (State == DriveState.DriveAwayFromTarget)
                    DriveAwayFromTarget();
                else if (State == DriveState.DriveToSafeZone)
                    DriveToSafeZone();
                else if (State == DriveState.WaitDrop)
                    WaitDrop();
                else if (State == DriveState.VerifyTarget)
                    VerifyTarget();
                else if (State == DriveState.DriveToStart)
                    DriveToStart();
                else if (State == DriveState.Idle)
                    Idle();
                else if (State == DriveState.GotoBall)
                    GoToBall();
                else if (State == DriveState.GrabPayloadManual)
                    //GrabPayloadManual();
                //prevent Overload
                Thread.Sleep(SleepTime);
            }
#endif
        }

        /// <summary>
        /// Reset All drive state
        /// </summary>
        void ResetAllState()
        {
            ResetSearchTarget();
            ResetDriveAwayFromTarget();
            ResetWaitDrop();
        }
        #endregion Methods

        #region Autonomous Behaivors

        #region Generate Search Path

        void GenerateSearchPath()
        {
            List<WayPoint> map = new List<WayPoint>();
            WayPoint nextWayPoint;
            WayPoint center = WayPoint.GetCenter(Boundary);
            nextWayPoint = center;
            map.Add(nextWayPoint);
            double distance = 0.5; //meters

            // ====== Flower Petal Pattern ======
            //r = 1 + cos(k * theta)
            distance = 8; // meters
            double r;
            double theta = 0;
            double k = 5.0 / 2.0;
            r = 1 + Math.Cos(k * theta);
            r *= distance;
            nextWayPoint = WayPoint.Projection(center, theta, r);
            for (int i = 0; i < 51; i++)
            {
                if (WayPoint.IsInsideBoundary(nextWayPoint.Lat, nextWayPoint.Long, Boundary))
                    map.Add(nextWayPoint);
                theta += 0.25;
                r = 1 + Math.Cos(k * theta);
                r *= distance;
                nextWayPoint = WayPoint.Projection(center, theta, r);
            }
            map.Add(map[1]);

            // ========== Recording ========== //
            string[] cords = new string[map.Count];
            for (int j = 0; j < map.Count; j++)
            {
                cords[j] = map[j].Lat + " " + map[j].Long + "\n";
            }
            //System.IO.File.WriteAllLines(@"C:\Users\UGV_usr\output.txt", cords);

            foreach (WayPoint point in map)
            {
                Waypoints.Enqueue(point);
            }
        }   // end of GenerateMap

      #endregion

      #region Search Target

      /// <summary>
      /// Search target mode
      /// </summary>

      void SearchTarget()
      {
         WayPoint nextWaypoint = null;
         //set behavior
         //if (TargetFound)
         //{
         //    State = DriveState.GotoBall;
         //    return;
         //}
         if (Waypoints.Count == 0 && usePathGen && !goToSafe)
         {
            State = DriveState.GenerateSearchPath;
            Speed = 0;
            Steering = 0;
            GenerateSearchPath();
            usePathGen = false;
            goToSafe = true;
            State = DriveState.SearchTarget;
         }
         else if (Waypoints.Count == 0 && !usePathGen)
         {
            //State = DriveState.Idle;
         }
         if (Waypoints.Count > 0 && Waypoints.TryPeek(out nextWaypoint))
         {
            if (nextWaypoint != null)
            {
               WayPoint currentLocation = new WayPoint(Latitude, Longitude, 0);
               WaypointVector = new Vector2d(currentLocation, nextWaypoint);
               NextWaypointDistance = WayPoint.GetDistance(this.Latitude, this.Longitude, nextWaypoint.Lat, nextWaypoint.Long);

               //// ************* Obstacle Avoidance Code ******************
               nextWaypoint = obstacleAvoidance(nextWaypoint, currentLocation);
               // Everything is in meters and radians
               //if (Settings.UseVision)
               //{
               //    if (AvoidanceVector.magnitude > 0)
               //    {
               //        SumVector = new Vector2d(currentLocation, nextWaypoint);
               //        SumVector.magnitude /= SumVector.magnitude; //Normalize(Check if normalize is needed)
               //        SumVector.magnitude *= Alpha * MaxSpeed; // Influence(Check if maxspeed is needed)
               //        AvoidanceVector.angle = (AvoidanceVector.angle + 90 + Heading);// Make Lidar data relative to robot position 
               //        SumVector += AvoidanceVector; //combine Avoidance and Attraction vector                  
               //        SumVector.magnitude = Math.Max(ReachWaypointZone + 1, SumVector.magnitude); //set the minimum vector length to 4 meters
               //        nextWaypoint = WayPoint.Projection(currentLocation, SumVector.angle, SumVector.magnitude);
               //    }

               //}

               /*if (Settings.DriveMode == DriveMode.Autonomous)
               {
                   Speed = 
               }*/
               NextWaypointBearing = WayPoint.GetBearing(this.Latitude, this.Longitude, nextWaypoint.Lat, nextWaypoint.Long);
               //calculate difference angle
               double errorWaypoint = NextWaypointBearing - Heading;
               if (errorWaypoint > Math.PI)
                  errorWaypoint -= Math.PI * 2.0;
               else if (errorWaypoint < -Math.PI)
                  errorWaypoint += Math.PI * 2.0;
               NextWaypointBearingError = errorWaypoint;
            }
            //else
            // State = DriveState.Idle;

            if (imu.GoodData)
            {
               //if use gps only to drive
               //apply steer control
               double TempSteering = NextWaypointBearingError * SteerRatio / Math.PI;
               // SteerPID.Feed(error);
               Steering = CloseBoundary ? Math.Min(Math.Max(TempSteering, -1000), 1000)
                                        : Math.Min(Math.Max(TempSteering * OutsideSteerRatio, -1000), 1000);
               //set speed
               Speed = InsideBoundary ? FullSpeed : FullSpeed * OutsideSpeedRatio;
            }
            else
            {
               //set all to 0 if no gps lock
               Speed = 0;
               Steering = 0;
            }
            //only reach target when drive auto
            if (Settings.DriveMode == DriveMode.Autonomous || Settings.DriveMode == DriveMode.SemiAutonomous)
            {
               //check if reached
               if (NextWaypointDistance < ReachWaypointZone)
               {
                  Waypoints.TryDequeue(out nextWaypoint);
               }

               //TargetDistance = WayPoint.GetDistance(this.Latitude, this.Longitude, TargetWaypoint.Lat, TargetWaypoint.Long);

               //if (TargetDistance < ReachWaypointZone)
               //    State=DriveState.GrabPayloadManual;


            }
         }
         else
         {
            Speed = 0;
            Steering = 0;
            State = DriveState.Idle;
         }
      }

      /// <summary>
      /// Reset search target
      /// </summary>
      void ResetSearchTarget()
        {
            RelativeNavFlag = false;
            RelativeNavCount = 0;
            TargetApporachCount = 0;
        }

        #endregion

        #region Go To ball
        public void GoToBall()
        {
            if (TargetFound)//only works if ball is seen by camera
            {
                //Centers UGV so the ball is straght ahead(0 degrees)
                double TempAngle = TargetAngle;
                if (TargetAngle > 10 || TargetAngle < -10)
                {
                    CenterUGV();
                }
                //calculate waypoint based on camera angle and distance to target waypoint
                double TempDistance = WayPoint.GetDistance(this.Latitude, this.Longitude, TargetWaypoint.Lat, TargetWaypoint.Long);
                WayPoint TempWaypoint = WayPoint.Projection(new WayPoint(Latitude, Longitude, 0), Heading - TempAngle, TempDistance);

                WayPoint nextWaypoint = TempWaypoint;

                //Inch toward temp waypoint calculated
                if (nextWaypoint != null)
                {
                    WayPoint currentLocation = new WayPoint(Latitude, Longitude, 0);
                    WaypointVector = new Vector2d(currentLocation, nextWaypoint);
                    NextWaypointDistance = WayPoint.GetDistance(this.Latitude, this.Longitude, nextWaypoint.Lat, nextWaypoint.Long);
                    // ************* Obstacle Avoidance Code ******************
                    // Everything is in meters and radians
                    if (Settings.UseVision)
                    {
                        if (AvoidanceVector.magnitude > 0)
                        {
                            SumVector = new Vector2d(currentLocation, nextWaypoint);
                            SumVector.magnitude /= SumVector.magnitude; //Normalize
                            SumVector.magnitude *= 0.6; // Influence

                            AvoidanceVector.magnitude /= AvoidanceVector.magnitude; //Normalize
                            AvoidanceVector.magnitude *= 0.4; //Influence
                            //SumVector -= AvoidanceVector;

                            SumVector += AvoidanceVector;                   // changed from (-) --> (+)

                            SumVector.magnitude = Math.Max(ReachWaypointZone + 1, SumVector.magnitude); //set the minimum vector length to 4 meters
                            nextWaypoint = WayPoint.Projection(currentLocation, SumVector.angle, SumVector.magnitude);
                        }
                    }

                    NextWaypointBearing = WayPoint.GetBearing(this.Latitude, this.Longitude, nextWaypoint.Lat, nextWaypoint.Long);
                    //calculate difference angle
                    double errorWaypoint = NextWaypointBearing - Heading;
                    if (errorWaypoint > Math.PI)
                        errorWaypoint -= Math.PI * 2.0;
                    else if (errorWaypoint < -Math.PI)
                        errorWaypoint += Math.PI * 2.0;
                    NextWaypointBearingError = errorWaypoint;
                }
                else
                    State = DriveState.Idle;

                if (imu.GoodData)
                {
                    //if use gps only to drive
                    //apply steer control
                    double TempSteering = NextWaypointBearingError * SteerRatio / Math.PI;
                    // SteerPID.Feed(error);
                    Steering = CloseBoundary ? Math.Min(Math.Max(TempSteering, -1000), 1000)
                        : Math.Min(Math.Max(TempSteering * OutsideSteerRatio, -1000), 1000);
                    //set speed
                    Speed = InsideBoundary ? FullSpeed : FullSpeed * OutsideSpeedRatio;
                }
                else
                {
                    //set all to 0 if no gps lock
                    Speed = 0;
                    Steering = 0;
                }
                //only reach target when drive auto
                if (Settings.DriveMode == DriveMode.Autonomous || Settings.DriveMode == DriveMode.SemiAutonomous)
                {
                    //check if we are close enough to target waypoint and if we are then go to search payload
                    if (TargetDistance < ReachWaypointZone)
                        State = DriveState.SearchPayload;
                }
            }
            else //if target found is false, go back to search target
            {
                State = DriveState.SearchTarget;
            }
        }
        public void CenterUGV()
        {

            //bool center = false;


           // while (!center)
            //{
                if (TargetAngle <= 90 && TargetAngle > 50)
                {
                    FinalFrontWheel = 51;
                    FinalRearWheel = 51;
                    FinalSteering = 2252;//-600
                }
                else if (TargetAngle <= 50 && TargetAngle > 35)
                {
                    FinalFrontWheel = 51;
                    FinalRearWheel = 51;
                    FinalSteering = 2150;//-300
                }
                else if (TargetAngle <= 35 && TargetAngle > 10)
                {
                    FinalFrontWheel = 51;
                    FinalRearWheel = 51;
                    FinalSteering = 2082;//-100
                }
                else if (TargetAngle <= 10 && TargetAngle >= -10)
                {
                    //center = true;
                    FinalFrontWheel = 0;
                    FinalRearWheel = 0;
                    FinalSteering = 2048;
                    return;
                }
                else if (TargetAngle < -10 && TargetAngle >= -35)
                {
                    FinalFrontWheel = 51;
                    FinalRearWheel = 51;
                    FinalSteering = 2014;//100
                }
                else if (TargetAngle < -35 && TargetAngle >= -50)
                {
                    FinalFrontWheel = 51;
                    FinalRearWheel = 51;
                    FinalSteering = 1946;//300
                }
                else if (TargetAngle < -50 && TargetAngle >= -90)
                {
                    FinalFrontWheel = 51;
                    FinalRearWheel = 51;
                    FinalSteering = 1844;//600
                }
           // }
        }
        #endregion

        #region Lock Target

        /// <summary>
        /// Lock target mode
        /// </summary>
        void LockTarget()
        {
            CurrentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            if (TargetFound == true)
            {
                StartLockTime = CurrentTime;
            }
            if (CurrentTime - StartLockTime >= TargetLostTimeout) // Stop locking to the target and look for it if we lost it
            {
                State = DriveState.SearchTarget;
                return;
            }
            if (TargetDistance < 3 && TargetDistance > 0) // If we are within 3 meters of the target, switch to looking for the payload
            {
                State = DriveState.SearchPayload;
                return;
            }
            else
            {
                WayPoint nextWaypoint = TargetWaypoint;
                WayPoint currentLocation = new WayPoint(Latitude, Longitude, 0);
                WaypointVector = new Vector2d(currentLocation, nextWaypoint);
                NextWaypointDistance = WayPoint.GetDistance(this.Latitude, this.Longitude, nextWaypoint.Lat, nextWaypoint.Long);
                // ************* Obstacle Avoidance Code ******************
                // Everything is in meters and radians
                if (Settings.UseVision)
                {
                    if (AvoidanceVector.magnitude > 0)
                    {
                        SumVector = new Vector2d(currentLocation, nextWaypoint);
                        SumVector.magnitude /= SumVector.magnitude; //Normalize
                        SumVector.magnitude *= 0.5; // Influence
                        AvoidanceVector.magnitude /= AvoidanceVector.magnitude; //Normalize
                        AvoidanceVector.magnitude *= 0.5; //Influence
                        SumVector -= AvoidanceVector;
                        SumVector.magnitude = Math.Max(ReachWaypointZone + 1, SumVector.magnitude); //set the minimum vector length to 4 meters
                        nextWaypoint = WayPoint.Projection(currentLocation, SumVector.angle, SumVector.magnitude);
                    }
                }
                NextWaypointBearing = WayPoint.GetBearing(this.Latitude, this.Longitude, nextWaypoint.Lat, nextWaypoint.Long);
                //calculate difference angle
                double errorWaypoint = NextWaypointBearing - Heading;
                if (errorWaypoint > Math.PI)
                    errorWaypoint -= Math.PI * 2.0;
                else if (errorWaypoint < -Math.PI)
                    errorWaypoint += Math.PI * 2.0;
                NextWaypointBearingError = errorWaypoint;

                if (imu.GoodData)
                {
                    //if use gps only to drive
                    //apply steer control
                    double TempSteering = NextWaypointBearingError * SteerRatio / Math.PI;
                    // SteerPID.Feed(error);
                    Steering = CloseBoundary ? Math.Min(Math.Max(TempSteering, -1000), 1000)
                        : Math.Min(Math.Max(TempSteering * OutsideSteerRatio, -1000), 1000);
                    //set speed
                    Speed = InsideBoundary ? FullSpeed : FullSpeed * OutsideSpeedRatio;
                }
                else
                {
                    //set all to 0 if no gps lock
                    Speed = 0;
                    Steering = 0;
                }
            }
        }

        #endregion

        #region Search Payload
        /// <summary>
        /// Search Payload mode
        /// </summary>
        bool StartWaiting = false;
        long StartTime;
        long WaitTime = 10000;
        private List<WayPoint> spiralPath_Payload = new List<WayPoint>();
        private int turret_Payload_Position = 0;

        //void SearchPayload()
        //{
        //    WayPoint nextWaypoint = null;

        //    arm_ugv.ArmSearchPosition();

        //    if (PayloadFound)
        //    {
        //        // State = DriveState.LockPayload;
        //        State = DriveState.Idle;                // this is temporary until the LockPayload Behavior is added
        //        return;
        //    }

        //    if (spiralPath_Payload.Count == 0)
        //    {
        //        // there are no more waypoints along the spiral to check anymore
        //        Speed = 0;
        //        Steering = 0;
        //        // Generate a new, modified, spiral search path
        //        // sort the spiral waypoints to go to the closest waypoint first
        //        // continue in the SearchPayload function
        //    }

        //    // ?? Maybe add an Idle() command to stop ugv before sweep
        //    Idle();

        //    // sweep the UGV left then right while checking if we found the payload
        //    for (int i = arm_ugv.Base.DXL_MINIMUM_POSITION_VALUE + 100; i < arm_ugv.Base.DXL_MAXIMUM_POSITION_VALUE - 100 && !PayloadFound; i += 100)
        //    {
        //        // write the position to the turret of the arm
        //        // wait until the base has moved to its position
        //        bool finishedMoving = arm_ugv.Base.Move_To(i);
        //        while (!finishedMoving) ;

        //        // check if the payload was found
        //        if (PayloadFound)
        //        {
        //            // record the position of the turret where the payload was seen
        //            turret_Payload_Position = arm_ugv.Base.dxl_present_position;
        //            // write to the arm again to override the current goal position with the present position
        //            //arm_ugv.Base.Move_To(arm_ugv.Base.dxl_present_position);
        //            // Switch states to approach payload
        //            // State = DriveState.LockPayload;
        //            State = DriveState.Idle;                        // need to change this once the LockPayload behavior is added
        //            break;
        //        }
        //    }

        //    // have arm go back to Search position
        //    arm_ugv.ArmSearchPosition();

        //    //if(payloadFound)
        //    //{
        //    //    // center UGV to Payload
        //    //    // start timer to be in the Approach_Payload behavior
        //    //    // go to Approach_Payload behavior
        //    //}

        //    // If payload not found then move towards the next WayPoint in the list containing the sprial path around the payload
        //    if (spiralPath_Payload.Count != 0 && (nextWaypoint = spiralPath_Payload[0]) != null)
        //    {
        //        WayPoint currentLocation = new WayPoint(Latitude, Longitude, 0);
        //        WaypointVector = new Vector2d(currentLocation, nextWaypoint);
        //        NextWaypointDistance = WayPoint.GetDistance(this.Latitude, this.Longitude, nextWaypoint.Lat, nextWaypoint.Long);
        //        // ************* Obstacle Avoidance Code ******************
        //        // Everything is in meters and radians
        //        if (Settings.UseVision)
        //        {
        //            if (AvoidanceVector.magnitude > 0)
        //            {
        //                SumVector = new Vector2d(currentLocation, nextWaypoint);
        //                SumVector.magnitude /= SumVector.magnitude; //Normalize
        //                SumVector.magnitude *= 0.6; // Influence

        //                AvoidanceVector.magnitude /= AvoidanceVector.magnitude; //Normalize
        //                AvoidanceVector.magnitude *= 0.4; //Influence
        //                                                  //SumVector -= AvoidanceVector;

        //                SumVector += AvoidanceVector;                   // changed from (-) --> (+)

        //                SumVector.magnitude = Math.Max(ReachWaypointZone + 1, SumVector.magnitude); //set the minimum vector length to 4 meters
        //                nextWaypoint = WayPoint.Projection(currentLocation, SumVector.angle, SumVector.magnitude);
        //            }
        //        }

        //        NextWaypointBearing = WayPoint.GetBearing(this.Latitude, this.Longitude, nextWaypoint.Lat, nextWaypoint.Long);
        //        //calculate difference angle
        //        double errorWaypoint = NextWaypointBearing - Heading;
        //        if (errorWaypoint > Math.PI)
        //            errorWaypoint -= Math.PI * 2.0;
        //        else if (errorWaypoint < -Math.PI)
        //            errorWaypoint += Math.PI * 2.0;
        //        NextWaypointBearingError = errorWaypoint;

        //        if (imu.GoodData)
        //        {
        //            //if use gps only to drive
        //            //apply steer control
        //            double TempSteering = NextWaypointBearingError * SteerRatio / Math.PI;
        //            // SteerPID.Feed(error);
        //            Steering = CloseBoundary ? Math.Min(Math.Max(TempSteering, -1000), 1000)
        //                : Math.Min(Math.Max(TempSteering * OutsideSteerRatio, -1000), 1000);
        //            //set speed
        //            Speed = InsideBoundary ? FullSpeed : FullSpeed * OutsideSpeedRatio;
        //        }
        //        else
        //        {
        //            //set all to 0 if no gps lock
        //            Speed = 0;
        //            Steering = 0;
        //        }

        //        //only reach target when drive auto
        //        if (Settings.DriveMode == DriveMode.Autonomous || Settings.DriveMode == DriveMode.SemiAutonomous)
        //        {
        //            //check if reached
        //            if (NextWaypointDistance < ReachWaypointZone)
        //                spiralPath_Payload.Remove(nextWaypoint);
        //        }
        //    }
        //    else
        //    {
        //        Speed = 0;
        //        Steering = 0;
        //        State = DriveState.Idle;
        //    }

        //    //State = DriveState.Idle;
        //    /*if (!StartWaiting)
        //    {
        //        StartTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        //        StartWaiting = true;
        //    }
        //    CurrentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        //    if (CurrentTime - StartTime > WaitTime)
        //        State = DriveState.DriveToStart;*/


        //}

        // modified generate search path behaivor
        void GenerateSearchPayloadPath()
        {
            List<WayPoint> map = new List<WayPoint>();
            WayPoint nextWayPoint;
            WayPoint center = WayPoint.GetCenter(Boundary);
            nextWayPoint = center;
            map.Add(nextWayPoint);
            double distance = 0.5; //meters

            // ===== spiral pattern ======
            /*double[] bearing = new double[8] { 0, Math.PI / 4, Math.PI / 2, 3 * Math.PI / 4, Math.PI, -3 * Math.PI / 4, -Math.PI / 2, -Math.PI / 4 };
            int i = 0;

            nextWayPoint = WayPoint.Projection(center, bearing[i], distance);
            while (WayPoint.IsInsideBoundary(nextWayPoint.Lat, nextWayPoint.Long, Boundary))
            {
                map.Add(nextWayPoint);
                i = (i + 1) % 8;
                distance += 0.5;
                nextWayPoint = WayPoint.Projection(center, bearing[i], distance);
            }*/

            // ====== Flower Petal Pattern ======
            //r = 1 + cos(k * theta)
            distance = 8; // meters
            double r;
            double theta = 0;
            double k = 5.0 / 2.0;
            r = 1 + Math.Cos(k * theta);
            r *= distance;
            nextWayPoint = WayPoint.Projection(center, theta, r);
            for (int i = 0; i < 51; i++)
            {
                if (WayPoint.IsInsideBoundary(nextWayPoint.Lat, nextWayPoint.Long, Boundary))
                    map.Add(nextWayPoint);
                theta += 0.25;
                r = 1 + Math.Cos(k * theta);
                r *= distance;
                nextWayPoint = WayPoint.Projection(center, theta, r);
            }
            map.Add(map[1]);

            // ========== Recording ========== //
            string[] cords = new string[map.Count];
            for (int j = 0; j < map.Count; j++)
            {
                cords[j] = map[j].Lat + " " + map[j].Long + "\n";
            }
            //System.IO.File.WriteAllLines(@"C:\Users\UGV_usr\output.txt", cords);

            foreach (WayPoint point in map)
            {
                spiralPath_Payload.Add(point);
            }
        }   // end of GenerateMap

        #endregion Search Payload

        #region Drive Away From target

        /// <summary>
        /// Reset Drive Away
        /// </summary>
        void ResetDriveAwayFromTarget()
        {
            SafeDistanceCount = 0;
        }

        /// <summary>
        /// Lock target mode
        /// </summary>
        void DriveAwayFromTarget()
        {
            //read target distance
            double targetDistance = Double.MaxValue;
            double targetAngle = 0;
            double GeoTargetDistance = Double.MaxValue;
            //read non-zero target distance
            if (VisionTargets.Count > 0)
            {
                var target = VisionTargets[0];
                targetDistance = target.distance == 0 ? targetDistance : target.distance;
                targetAngle = target.angle;
                GeoTargetDistance = WayPoint.GetDistance(this.Latitude, this.Longitude, target.Lat, target.Long);
                //keep drive backward
                Steering = 0;
                //target too far
                if (targetDistance > SafeDistance || GeoTargetDistance > SafeDistance)
                {
                    Speed = 0;
                    SafeDistanceCount++;
                }
                else
                {
                    Speed = -FullSpeed * 0.5;
                    SafeDistanceCount = 0;
                }

                //change state when time reach
                if (SafeDistanceCount > SafeDistanceTimeout)
                {
                    //clear waypoints and change state
                    State = DriveState.DriveToSafeZone;
                }
            }
            else
            {
                //target lost
                Speed = 0;
                State = DriveState.DriveToSafeZone;
            }
        }

        #endregion Drive Away From target

        #region Drive To Safe Zone

        /// <summary>
        /// Drive to safe zone
        /// </summary>
        void DriveToSafeZone()
        {
            WayPoint nextWaypoint = null;
            //set behavior
            if (Waypoints.Count > 0 && Waypoints.TryPeek(out nextWaypoint))
            {
                if (nextWaypoint != null)
                {
                    NextWaypointBearing = WayPoint.GetBearing(this.Latitude, this.Longitude, nextWaypoint.Lat, nextWaypoint.Long);
                    NextWaypointDistance = WayPoint.GetDistance(this.Latitude, this.Longitude, nextWaypoint.Lat, nextWaypoint.Long);
                    //calculate difference angle
                    double errorWaypoint = NextWaypointBearing - Heading;
                    if (errorWaypoint > Math.PI)
                        errorWaypoint -= Math.PI * 2.0;
                    else if (errorWaypoint < -Math.PI)
                        errorWaypoint += Math.PI * 2.0;
                    NextWaypointBearingError = errorWaypoint;

                    //vision
                    VisionWayPoint nextVisionWaypoint = null;
                    VisionWayPoint[] tempVisionWaypoints = VisionWaypoints.ToArray();
                    if (tempVisionWaypoints.Length > 0)
                    {
                        nextVisionWaypoint = tempVisionWaypoints[0];
                        NextVisionBearing = WayPoint.GetBearing(this.Latitude, this.Longitude
                            , nextVisionWaypoint.X, nextVisionWaypoint.Y);
                        NextVisionDistance = WayPoint.GetDistance(this.Latitude, this.Longitude
                            , nextVisionWaypoint.X, nextVisionWaypoint.Y);
                        //calculate difference angle
                        double errorVision = NextVisionBearing - Heading;
                        if (errorVision > Math.PI)
                            errorVision -= Math.PI * 2.0;
                        else if (errorVision < -Math.PI)
                            errorVision += Math.PI * 2.0;
                        NextVisionBearingError = errorVision;
                    }

                    if (imu.GoodData)
                    {
                        //wait for distance to reach
                        if (NextWaypointDistance < ReachWaypointZone && InsideSafeZone)
                        {
                            //when reach stop and wait for drop
                            Speed = 0;
                            Steering = 0;
                            //clear waypoints and wait drop
                            Waypoints = new System.Collections.Concurrent.ConcurrentQueue<WayPoint>();
                            this.State = DriveState.WaitDrop;
                        }
                        //if use vision to drive
                        if (Settings.UseVision && nextVisionWaypoint != null)
                        {
                            //avoid 500 500
                            if (nextVisionWaypoint.X > 360.0 || nextVisionWaypoint.Y > 360.0)
                            {
                                WayPoint wp = WayPoint.GenerateRandomWaypoint(SafeZone);
                                if (wp != null)
                                {
                                    WayPoint temp;
                                    Waypoints.TryDequeue(out temp);
                                    Waypoints.Enqueue(wp);
                                    return;
                                }
                            }
                            //apply steer control
                            double TempSteering = 0;
                            //apply speed control
                            double TempSpeed = 0;
                            //determine behavior
                            TempSteering = NextVisionBearingError * SteerRatio / Math.PI;
                            TempSpeed = FullSpeed;
                            TargetApporachCount = 0;
                            Steering = CloseBoundary ? Math.Min(Math.Max(TempSteering, -1000), 1000)
                                : Math.Min(Math.Max(TempSteering * OutsideSteerRatio, -1000), 1000);
                            //set speed
                            Speed = InsideBoundary ? TempSpeed : TempSpeed * OutsideSpeedRatio;
                        }
                        //if use gps only to drive
                        else
                        {
                            //apply steer control
                            double TempSteering = NextWaypointBearingError * SteerRatio / Math.PI;
                            // SteerPID.Feed(error);
                            Steering = CloseBoundary ? Math.Min(Math.Max(TempSteering, -1000), 1000)
                                : Math.Min(Math.Max(TempSteering * OutsideSteerRatio, -1000), 1000);
                            //set speed
                            Speed = InsideBoundary ? FullSpeed : FullSpeed * OutsideSpeedRatio;
                        }
                    }
                    else
                    {
                        //set all to 0 if no gps lock
                        Speed = 0;
                        Steering = 0;
                    }
                }
            }
            else
            {
                //if target not found
                if (Waypoints.Count == 0)
                {
                    WayPoint wp = WayPoint.GetCenter(SafeZone);
                    if (wp != null)
                        Waypoints.Enqueue(wp);
                }
                //set all to 0 if no waypoints
                Speed = 0;
                Steering = 0;
            }
        }

        #endregion Drive To Safe Zone

        #region Wait Drop

        /// <summary>
        /// Reset Wait Drop Counter
        /// </summary>
        void ResetWaitDrop()
        {
            PayloadDropCount = 0;
        }

        /// <summary>
        /// Wait for drop command
        /// </summary>
        void WaitDrop()
        {
            if (TargetDropped)
            {
                PayloadDropCount++;
                DebugMessage.Clear();
                DebugMessage.Append(PayloadDropCount + "/" + PayloadDropTimeout);
                if (PayloadDropCount > PayloadDropTimeout)
                {
                    State = DriveState.VerifyTarget;
                }
            }
            else
            {
                PayloadDropCount = 0;
            }
            Speed = 0;
            Steering = 0;
        }

        #endregion Wait Drop

        #region Verify Target

        /// <summary>
        /// Apporach and verify target
        /// </summary>
        void VerifyTarget()
        {
            State = DriveState.DriveToStart;
            Speed = 0;
            Steering = 0;
        }

        #endregion Verify Target

        #region Drive To Start

        /// <summary>
        /// Drive back to start point
        /// </summary>
        void DriveToStart()
        {
            WayPoint nextWaypoint = StartWaypoint;
            //set behavior

            if (nextWaypoint != null)
            {
                WayPoint currentLocation = new WayPoint(Latitude, Longitude, 0);
                WaypointVector = new Vector2d(currentLocation, nextWaypoint);
                NextWaypointDistance = WayPoint.GetDistance(this.Latitude, this.Longitude, nextWaypoint.Lat, nextWaypoint.Long);
                // ************* Obstacle Avoidance Code ******************
                // Everything is in meters and radians
                if (Settings.UseVision)
                {
                    if (AvoidanceVector.magnitude > 0)
                    {
                        SumVector = new Vector2d(currentLocation, nextWaypoint);
                        SumVector.magnitude /= SumVector.magnitude; //Normalize
                        SumVector.magnitude *= 0.6; // Influence

                        AvoidanceVector.magnitude /= AvoidanceVector.magnitude; //Normalize
                        AvoidanceVector.magnitude *= 0.4; //Influence
                                                          //SumVector -= AvoidanceVector;

                        SumVector += AvoidanceVector;                   // changed from (-) --> (+)

                        SumVector.magnitude = Math.Max(ReachWaypointZone + 1, SumVector.magnitude); //set the minimum vector length to 4 meters
                        nextWaypoint = WayPoint.Projection(currentLocation, SumVector.angle, SumVector.magnitude);
                    }

                }

                /*if (Settings.DriveMode == DriveMode.Autonomous)
                {
                    Speed = 
                }*/
                NextWaypointBearing = WayPoint.GetBearing(this.Latitude, this.Longitude, nextWaypoint.Lat, nextWaypoint.Long);
                //calculate difference angle
                double errorWaypoint = NextWaypointBearing - Heading;
                if (errorWaypoint > Math.PI)
                    errorWaypoint -= Math.PI * 2.0;
                else if (errorWaypoint < -Math.PI)
                    errorWaypoint += Math.PI * 2.0;
                NextWaypointBearingError = errorWaypoint;
            }
            else
                State = DriveState.Idle;

            if (imu.GoodData)
            {
                //if use gps only to drive
                //apply steer control
                double TempSteering = NextWaypointBearingError * SteerRatio / Math.PI;
                // SteerPID.Feed(error);
                Steering = CloseBoundary ? Math.Min(Math.Max(TempSteering, -1000), 1000)
                                         : Math.Min(Math.Max(TempSteering * OutsideSteerRatio, -1000), 1000);
                //set speed
                Speed = InsideBoundary ? FullSpeed : FullSpeed * OutsideSpeedRatio;
            }
            else
            {
                //set all to 0 if no gps lock
                Speed = 0;
                Steering = 0;
            }

            //only reach target when drive auto
            if (Settings.DriveMode == DriveMode.Autonomous || Settings.DriveMode == DriveMode.SemiAutonomous)
                {
                //check if reached
                if (NextWaypointDistance < ReachWaypointZone)
                    State = DriveState.Idle;
                }
            
        }

        #endregion Drive To Start

        #region Idle

        /// <summary>
        /// Do nothing and wait for command
        /// </summary>
        void Idle()
        {
            Speed = 0;
            Steering = 0;
            LocalSpeed = 0;
            LocalSteering = 0;
            SendControl();
            //Enabled = false;
        }

        #endregion Idle

        #region Grap Payload Manual
        
        /// <summary>
        /// Idle the vehicle while the robotic arm is controlled manually through a GUI and sent transmissions by Xbee
        /// </summary>
        //public void GrabPayloadManual()
        //{
        //    if (incomingChange == 0xFF)
        //    {
        //        // grabbing payload with arm has finished 
        //        // change state to retract payload and go back to start location
        //        State = DriveState.Idle;        // should add a state to retract the payload or handle while in current state
        //    }
        //}

      #endregion
      const double Alpha = 1.0;

      /// <summary>
      /// code for combining the obstacle avoidance vector with the waypoint vector
      /// </summary>
      Vector2d temp = new Vector2d(0, 0);

      WayPoint obstacleAvoidance(WayPoint nextWaypoint, WayPoint currentLocation)
      {

         // Everything is in meters and radians
         if (Settings.UseVision)
         {
            if (AvoidanceVector.magnitude > 0)
            {
               SumVector = new Vector2d(currentLocation, nextWaypoint);
               SumVector.magnitude /= SumVector.magnitude; //Normalize(Check if normalize is needed)
               SumVector.magnitude *= Alpha; // Influence(Check if maxspeed is needed)
               AvoidanceVector.angle = (temp.angle + (45 * Math.PI / 180) + Heading);// add heading + 90Make Lidar data relative to robot position
               DebugMessage.Clear();
               DebugMessage.Append((AvoidanceVector.angle * 180 / Math.PI));
               SumVector += AvoidanceVector; //combine Avoidance and Attraction vector                  
               SumVector.magnitude = Math.Max(ReachWaypointZone + 1, SumVector.magnitude); //set the minimum vector length to 4 meters
               nextWaypoint = WayPoint.Projection(currentLocation, SumVector.angle, SumVector.magnitude);
            }
         }
         return nextWaypoint;
      }
      #endregion
   }
}
