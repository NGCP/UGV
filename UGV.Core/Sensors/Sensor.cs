using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGV.Core.Sensors
{
    public abstract class Sensor<T> where T:SensorPackage
    {
        /// <summary>
        /// Determine whether the sensor is calibrated
        /// </summary>
        protected bool Calibrated = false;
        /// <summary>
        /// Last Update time of Sensor
        /// </summary>
        public DateTime LastUpdateTime;
        /// <summary>
        /// Update the sensor data 
        /// </summary>
        /// <param name="package">Package of the sensor data</param>
        public abstract void Update(T package);
    }

    public abstract class SensorPackage
    {
        /// <summary>
        /// Determine if the sensor package is valid
        /// </summary>
        public bool Valid = true;  
    }
}
