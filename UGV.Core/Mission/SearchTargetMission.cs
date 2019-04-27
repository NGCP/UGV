using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGV.Core.Navigation
{
    public class SearchTargetMission : Mission
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ugv"></param>
        public SearchTargetMission(UGV ugv)
            : base(ugv)
        {

        }

        /// <summary>
        /// The limit of apporaching zone in mm
        /// </summary>
        const int ApporachingZone = 6000;

        /// <summary>
        /// The distance considered as at target in mm 
        /// </summary>
        const int TargetZone = 2000;
        /// <summary>
        /// if system is navigate relatively
        /// </summary>
        bool RelativeNavFlag = false;
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

        /// <summary>
        /// Reset
        /// </summary>
        public override void Reset()
        {
            RelativeNavCount = 0;
            TargetApporachCount = 0;
        }

        /// <summary>
        /// Search target mode
        /// </summary>
        public override UGV.DriveState DoWork()
        {
            while (true)
            {
                WayPoint nextWaypoint = null;
                //set behavior
                if (ugv.VisionTargets.Count > 0 || (ugv.Waypoints.Count > 0 && ugv.Waypoints.TryPeek(out nextWaypoint)))
                {
                    if (nextWaypoint != null)
                    {
                        ugv.NextWaypointBearing = WayPoint.GetBearing(ugv.Latitude, ugv.Longitude, nextWaypoint.Lat, nextWaypoint.Long);
                        ugv.NextWaypointDistance = WayPoint.GetDistance(ugv.Latitude, ugv.Longitude, nextWaypoint.Lat, nextWaypoint.Long);
                        //calculate difference angle
                        double errorWaypoint = ugv.NextWaypointBearing - ugv.Heading;
                        if (errorWaypoint > Math.PI)
                            errorWaypoint -= Math.PI * 2.0;
                        else if (errorWaypoint < -Math.PI)
                            errorWaypoint += Math.PI * 2.0;
                        ugv.NextWaypointBearingError = errorWaypoint;
                    }

                    //vision
                    VisionWayPoint nextVisionWaypoint = null;
                    VisionWayPoint[] tempVisionWaypoints = ugv.VisionWaypoints.ToArray();
                    if (tempVisionWaypoints.Length > 0)
                    {
                        nextVisionWaypoint = tempVisionWaypoints[0];
                        ugv.NextVisionBearing = WayPoint.GetBearing(ugv.Latitude, ugv.Longitude
                            , nextVisionWaypoint.X, nextVisionWaypoint.Y);
                        ugv.NextVisionDistance = WayPoint.GetDistance(ugv.Latitude, ugv.Longitude
                            , nextVisionWaypoint.X, nextVisionWaypoint.Y);
                        //calculate difference angle
                        double errorVision = ugv.NextVisionBearing - ugv.Heading;
                        if (errorVision > Math.PI)
                            errorVision -= Math.PI * 2.0;
                        else if (errorVision < -Math.PI)
                            errorVision += Math.PI * 2.0;
                        ugv.NextVisionBearingError = errorVision;
                    }

                    //target
                    //read target distance
                    double targetDistance = Double.MaxValue;
                    double targetAngle = 0;
                    //read non-zero target distance
                    if (ugv.VisionTargets.Count > 0)
                    {
                        targetDistance = ugv.VisionTargets[0].distance == 0 ? targetDistance : ugv.VisionTargets[0].distance;
                        targetAngle = ugv.VisionTargets[0].angle;
                    }

                    if (ugv.GPSLock)
                    {
                        //if use vision to drive
                        if (ugv.Settings.UseVision && (nextVisionWaypoint != null || targetDistance < ApporachingZone))
                        {
                            //apply steer control
                            double TempSteering = 0;
                            //apply speed control
                            double TempSpeed = 0;
                            //determine behavior
                            if (targetDistance < ApporachingZone)
                            {
                                ugv.DebugMessage.Clear();
                                ugv.DebugMessage.Append("Search: Apporaching Target at distance " + targetDistance);
                                TempSteering = targetAngle * 11.1;
                                TempSpeed = targetDistance > ApporachingZone ? 1000 : 500.0 * (targetDistance / ApporachingZone);
                                if (targetDistance < TargetZone)
                                {
                                    TempSpeed = 0;
                                    TargetApporachCount++;
                                    ugv.DebugMessage.Append(TargetApporachCount + " / " + TargetApporachTimeout);
                                    if (TargetApporachCount < TargetApporachTimeout)
                                        return UGV.DriveState.LockTarget;
                                }
                                TargetApporachCount = 0;
                                RelativeNavFlag = true;
                                RelativeNavCount = 0;
                            }
                            //give some timeout before decide to go off relative
                            else if (RelativeNavFlag)
                            {
                                ugv.DebugMessage.Clear();
                                ugv.DebugMessage.Append("Search: Lost target from relative nav " + RelativeNavCount + "/" + RelativeNavTimeout);
                                TempSpeed = 0;
                                if (RelativeNavCount > RelativeNavTimeout)
                                    RelativeNavFlag = false;
                                RelativeNavCount++;
                            }
                            else
                            {
                                ugv.DebugMessage.Clear();
                                ugv.DebugMessage.Append("Search: Looking for Target");
                                TempSteering = ugv.NextVisionBearingError * 2000.0 / Math.PI;
                                TempSpeed = 1000;
                                TargetApporachCount = 0;
                            }
                            ugv.Steering = Math.Min(Math.Max(TempSteering, -1000), 1000);
                            //set speed
                            ugv.Speed = ugv.InsideBoundary ? TempSpeed : 0;
                        }
                        //if use gps only to drive
                        else
                        {
                            //apply steer control
                            double TempSteering = ugv.NextWaypointBearingError * 2000.0 / Math.PI;
                            // SteerPID.Feed(error);
                            ugv.Steering = Math.Min(Math.Max(TempSteering, -1000), 1000);
                            //set speed
                            ugv.Speed = ugv.InsideBoundary ? 1000 : 0;
                        }
                    }
                    else
                    {
                        //set all to 0 if no gps lock
                        ugv.Speed = 0;
                        ugv.Steering = 0;
                    }
                    //only reach target when drive auto
                    if (ugv.Settings.DriveMode == UGV.DriveMode.Autonomous || ugv.Settings.DriveMode == UGV.DriveMode.SemiAutonomous)
                    {
                        //check if reached
                        if (ugv.NextWaypointDistance < 2.0)
                            ugv.Waypoints.TryDequeue(out nextWaypoint);
                    }
                }
                else
                {
                    //if target not found
                    if (ugv.VisionTargets.Count == 0)
                    {
                        WayPoint wp = WayPoint.GenerateRandomWaypoint(ugv.Boundary);
                        if (wp != null)
                            ugv.Waypoints.Enqueue(wp);
                    }
                    //set all to 0 if no waypoints
                    ugv.Speed = 0;
                    ugv.Steering = 0;
                }
                //sleep to avoid overload
                System.Threading.Thread.Sleep(SleepTime);
            }
        }

    }
}
