using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGV.Core.Vision
{
    /// <summary>
    /// Class to store spatial information for specific targets
    /// </summary>
    public class VisionTarget
    {
        private double _X;//Relative distance in meters from the from the LIDAR
        private double _Y;//Relative distance in meters to the right(> 0) or left(< 0) of the front of the LIDAR;
        private double _Lat;//Not sure if this will be used
        private double _Long;//Not sure if this will be used
        private double _distance;//Distance in mm from the LIDAR
        private double _angle;//Angle from straight to the target right(> 0) or left(< 0).

        /// <summary>
        /// X coordinate for specific target relative to LIDAR
        /// </summary>
        public double X
        {
            get { return _X; }
            set { _X = value; }
        }
        /// <summary>
        /// Y coordinate for specific target: (positive = right)(negative = left)
        /// </summary>
        public double Y
        {
            get { return _Y; }
            set { _Y = value; }
        }
        /// <summary>
        /// GPS Latitude
        /// </summary>
        public double Lat
        {
            get { return _Lat; }
            set { _Lat = value; }
        }
        /// <summary>
        /// GPS Longitude
        /// </summary>
        public double Long
        {
            get { return _Long; }
            set { _Long = value; }
        }
        /// <summary>
        /// Distance from LIDAR in millimeters
        /// </summary>
        public double distance
        {
            get { return _distance; }
            set { _distance = value; }
        }
        /// <summary>
        /// Angle relative to directly in front of the LIDAR: (positive = right)(negative = left)
        /// </summary>
        public double angle
        {
            get { return _angle; }
            set { _angle = value; }
        }
        /// <summary>
        /// Returns formated coordinates into a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {

            return "X: " + _X.ToString() + " Y: " + _Y.ToString() + " Lat: " + _Lat + " Long: " + _Long;
        }
    }
}
