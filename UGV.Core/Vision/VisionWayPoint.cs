using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGV.Core.Vision
{
    /// <summary>
    /// Class to store spatial information for specific waypoints
    /// </summary>
    public class VisionWayPoint
    {
        private float _X;
        private float _Y;
        private int _ID;
        private int _Next;
        private int _Flags;

        /// <summary>
        /// Default constructor sets all data members to 0
        /// </summary>
        public VisionWayPoint()
        {
            _X = 0;
            _Y = 0;
            _ID = 0;
            _Next = 0;
            _Flags = 0;
        }

        /// <summary>
        /// X coordinate for specific target relative to LIDAR
        /// </summary>
        public float X
        {
            get { return _X; }
            set { _X = value; }
        }
        /// <summary>
        /// Y coordinate for specific target: (positive = right)(negative = left)
        /// </summary>
        public float Y
        {
            get { return _Y; }
            set { _Y = value; }
        }
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        public int Next
        {
            get { return _Next; }
            set { _Next = value; }
        }
        public int Flags
        {
            get { return _Flags; }
            set { _Flags = value; }
        }
    }
}
