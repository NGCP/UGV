using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObstacleDetection
{
    class ObstacleDetector
    {
        private List<double> stepDistances;
        private List<Vector2d> vectors;
        private int startStep;

        //Objects that are farther than the MAX_DISTANCE
        //Will not be considered when forming an obstacle 
        private double maxDistance;

        public ObstacleDetector(int startingStep, List<double> distanceList, double maxDistance)
        {
            //this.maxDistance = maxDistance; just doing a test change back if i forget
            this.maxDistance = .3; //use code above 
            stepDistances = distanceList;
            startStep = startingStep; // start step should be 140 End step should be 930
            vectors = new List<Vector2d>();
        }

        /**
        * Takes a list of distance vectors and creates
        * calculates the vectorSum 
        **/
        public VectorSum getVectorSum()
        {
            //vector solution
            /*
            Vector2d sum = new Vector2d(0, 0);
            int count = 0;
            int step;
            for (int i = 0; i < stepDistances.Count; i++)
            {
                step = i + startStep;

                //Ignore Data that is greater than maxDistance or is infinity
                if (stepDistances[i] * 0.001 <= maxDistance && stepDistances[i] != 1)
                {
                    double magnitude = stepDistances[i] * .001;
                    double angle = stepToRadians(step);
                    Vector2d newVector = new Vector2d(1 / magnitude, angle);

                    sum += newVector;
                    count++;
                }

            }
            return new VectorSum(count, sum.getAngle(), sum.getMagnitude());*/

            //Case based solution
            int count = 0;
            int step;
            double Beta = .1; // Scaling Factor
            int ClosestRange = 0; //Closest Range to object
            double[] RVect = new double[] { 0, 0 };
            double RMagn;
            double RAngle;
            for (int i = 0; i < stepDistances.Count; i++)
            {
                step = i + startStep;
                //Ignore Data that is greater than maxDistance or is infinity
                if (stepDistances[i] * 0.001 <= maxDistance && stepDistances[i] != 1 && stepDistances[i] >= 200)
                {
                    double magnitude = stepDistances[i] * .001;
                    double angle = stepToRadians(step);
                    if (magnitude < ClosestRange)
                    {
                        RVect[0] -= 100000 * Math.Cos(angle);
                        RVect[1] -= 100000 * Math.Sin(angle);
                    }
                    else if (ClosestRange <= magnitude)
                    {
                        RMagn = Beta * ((1 / (this.maxDistance + ClosestRange)) - 1 / (magnitude)) * ((1 / magnitude) * 1 / magnitude);
                        RVect[0] += RMagn * Math.Cos(angle);
                        RVect[1] -= RMagn * Math.Sin(angle);
                    }
                    count++;
                }
            }
            RMagn = Math.Sqrt(Math.Pow(RVect[0], 2) + Math.Pow(RVect[1], 2));
            RAngle = (Math.Atan2(RVect[1], RVect[0]) * 180 / Math.PI) + 90;
            if (RAngle > 360)
            {
                RAngle = RAngle - 360;
            }
            else if (RAngle < -360)
            {
                RAngle = RAngle + 360;
            }
            return new VectorSum(count, (RAngle) * Math.PI / 180, RMagn);
        }

        //Set the max distance in milimieters
        public void setMaxDistance(double newMax)
        {
            maxDistance = newMax;
        }

        /**
        * Converts a step from the lidar into a degree.
        * Note that each step on the the Hokuyo UTM-30LX lidar
        * is separated by .25 degrees. The lidar has a 270 degree
        * range of detection. Step 0 occurs at angle 225 degrees.  
        **/
        private double stepToRadians(int step)
        {
            double startAngle = 225;
            double angle = startAngle + step * .25f;
            if (angle >= 360)
            {
                angle = angle - 360;
            }
            angle *= Math.PI / 180;
            if (angle > Math.PI)
                angle -= Math.PI * 2;
            return angle;
        }
    }
}