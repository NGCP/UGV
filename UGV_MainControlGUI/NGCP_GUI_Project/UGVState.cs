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

    }
}
