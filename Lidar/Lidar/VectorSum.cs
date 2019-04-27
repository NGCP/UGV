using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObstacleDetection
{
    class VectorSum
    {
        private int count;
        private double angle;
        private double magnitude;
        public VectorSum(int newCount, double newAngle, double newMagnitude)
        {
            count = newCount;
            angle = newAngle;
            magnitude = newMagnitude;
        }

        public int getCount()
        {
            return count;
        }

        public double getAngle()
        {
            return angle;
        }

        public double getMagnitude()
        {
            return magnitude;
        }
    }
}
