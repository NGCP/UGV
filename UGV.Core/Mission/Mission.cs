using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGV.Core.Navigation
{
    /// <summary>
    /// Mission of UGV
    /// </summary>
    public abstract class Mission
    {
        /// <summary>
        /// UGV Class
        /// </summary>
        protected UGV ugv;

        /// <summary>
        /// Sleep Time each cycle
        /// </summary>
        protected const int SleepTime = 20;


        /// <summary>
        /// Constructor of Mission
        /// </summary>
        /// <param name="ugv"></param>
        public Mission(UGV ugv)
        {
            this.ugv = ugv;
            Reset();
        }

        /// <summary>
        /// Reset this mission
        /// </summary>
        public virtual void Reset() { }

        /// <summary>
        /// Do the work of mission
        /// </summary>
        public abstract UGV.DriveState DoWork();
    }
}
