using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGV.Core.Navigation;

namespace NGCP.UGV
{
    public class UGVState
    {
        #region Public Properties

        public double Latitude;     //8 bytes

        public double Longitude;    //8 bytes

        public double Pitch;        //8 bytes

        public double Roll;         //8 bytes

        public double Heading;      //8 bytes

        public double FrontWheelOutput; //8 bytes

        public double RearWheelOutput;  //8 bytes

        public double SteerOutput;      //8 bytes

        public double WayPointBearing;  //8 bytes

        public double WayPointDistance; //8 bytes

        public byte DriveMode;

        public WayPoint TargetLocation;

        #endregion

        #region Public Methods

        /// <summary>
        /// Capture UGV state
        /// </summary>
        /// <param name="ugv"></param>
        /// <returns></returns>
        public static UGVState Capture(UGV ugv)
        {
            UGVState state = new UGVState();
            state.Latitude = ugv.Latitude;
            state.Longitude = ugv.Longitude;
            state.Pitch = ugv.Pitch;
            state.Roll = ugv.Roll;
            state.Heading = ugv.Heading;
            state.FrontWheelOutput = ugv.FinalFrontWheel;
            state.RearWheelOutput = ugv.FinalRearWheel;
            state.SteerOutput = ugv.FinalSteering;
            state.WayPointBearing = ugv.NextWaypointBearing;
            state.WayPointDistance = ugv.NextWaypointDistance;
            state.DriveMode = (byte)ugv.Settings.DriveMode;
            state.TargetLocation = ugv.TargetLockedLocation;
            return state;
        }

        /// <summary>
        /// Convert UGV State to system state
        /// </summary>
        /// <returns></returns>
        public SystemState ToSystemState(UGV ugv)
        {
            SystemState state = new SystemState();
            state.Lat = this.Latitude;
            state.Long = this.Longitude;
            state.Bearing = this.Heading;
            WayPoint waypoint;
            var tempTargets = ugv.VisionTargets.ToArray();
            if (tempTargets.Length > 0 &&
                (ugv.State == UGV.DriveState.SearchTarget || ugv.State == UGV.DriveState.LockTarget))
            {
                state.TargetX = tempTargets[0].Lat;
                state.TargetY = tempTargets[0].Long;
            }
            else if (ugv.Waypoints.TryPeek(out waypoint))
            {
                state.TargetX = waypoint.Lat;
                state.TargetY = waypoint.Long;
            }
            else
            {
                state.TargetX = this.Latitude;
                state.TargetY = this.Longitude;
            }
            state.Velocity = 0;
            return state;
        }

        /// <summary>
        /// Serialize state into bytes
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Latitude));
            bytes.AddRange(BitConverter.GetBytes(Longitude));
            bytes.AddRange(BitConverter.GetBytes(Pitch));
            bytes.AddRange(BitConverter.GetBytes(Roll));
            bytes.AddRange(BitConverter.GetBytes(Heading));
            bytes.AddRange(BitConverter.GetBytes(FrontWheelOutput));
            bytes.AddRange(BitConverter.GetBytes(RearWheelOutput));
            bytes.AddRange(BitConverter.GetBytes(SteerOutput));
            bytes.AddRange(BitConverter.GetBytes(WayPointBearing));
            bytes.AddRange(BitConverter.GetBytes(WayPointDistance));
            bytes.Add(DriveMode);
            return bytes.ToArray();
        }

        #endregion
    }
}
