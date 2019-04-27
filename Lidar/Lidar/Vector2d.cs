using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObstacleDetection
{
    public class Vector2d
    {
        public double angle;
        public double magnitude;

        public Vector2d(double newAngle, double newMagnitude)
        {
            angle = newAngle;
            magnitude = newMagnitude;
        }

        public double getAngle()
        {
            return angle;
        }

        public double getMagnitude()
        {
            return magnitude;
        }

        public double getX()
        {
            return magnitude * Math.Cos(angle);
        }

        public double getY()
        {
            return magnitude * Math.Sin(angle);
        }

        public void setAngle(double x, double y)
        {
            angle = Math.Atan2(y, x);
        }

        public void setMagnitude(double x, double y)
        {
            magnitude = Math.Sqrt(x * x + y * y);
        }

        public static Vector2d operator +(Vector2d v1, Vector2d v2)
        {
            Vector2d vec = new Vector2d(0, 0);
            double x = v1.getX() + v2.getX();
            double y = v1.getY() + v2.getY();
            vec.setAngle(x, y);
            vec.setMagnitude(x, y);
            return vec;
        }

        public static Vector2d operator -(Vector2d v1, Vector2d v2)
        {
            Vector2d vec = new Vector2d(0, 0);
            double x = v1.getX() - v2.getX();
            double y = v1.getY() - v2.getY();
            vec.setAngle(x, y);
            vec.setMagnitude(x, y);
            return vec;
        }
    }
}
