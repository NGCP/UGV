using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGCP.UGV
{
    /// <summary>
    /// The SystemState class describes the state of the UGV at any given moment
    /// </summary>
    public class SystemState
    {
        #region Private Properties

        private double _lat;
        private double _long;
        private double _bearing;
        private double _targetX;
        private double _targetY;
        private float _velocity;

        #endregion

        #region Public Methods

        public double Lat
        {
            get { return _lat; }
            set { _lat = value; }
        }
        public double Long
        {
            get { return _long; }
            set { _long = value; }
        }
        public double Bearing
        {
            get { return _bearing; }
            set { _bearing = value; }
        }
        public double TargetX
        {
            get { return _targetX; }
            set { _targetX = value; }
        }
        public double TargetY
        {
            get { return _targetY; }
            set { _targetY = value; }
        }
        public float Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }
        #endregion
    }
}
