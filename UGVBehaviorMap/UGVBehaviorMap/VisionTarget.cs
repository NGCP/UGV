using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGCP.UGV
{
    public class VisionTarget
    {
        private double _X;//Relitive dstance in meters from the from the LIDR
        private double _Y;//Relitive distnace to the right + or left - oof the front of the LIDAR;
        private double _Lat;
        private double _Long;
        private double _distance;//Distnace in mm from the LIDAR (straight lne distance)
        private double _angle;//Angle from straight to the taget + right -left.
        public double X
        {
            get { return _X; }
            set { _X = value; }
        }
        public double Y
        {
            get { return _Y; }
            set { _Y = value; }
        }
        public double Lat
        {
            get { return _Lat; }
            set { _Lat = value; }
        }
        public double Long
        {
            get { return _Long; }
            set { _Long = value; }
        }
        public double distance
        {
            get { return _distance; }
            set { _distance = value; }
        }
        public double angle
        {
            get { return _angle; }
            set { _angle = value; }
        }
        public override string ToString()
        {

            return "X: " + _X.ToString() + " Y: " + _Y.ToString() + " Lat: " + _Lat + " Long: " + _Long;
        }
    }
}
