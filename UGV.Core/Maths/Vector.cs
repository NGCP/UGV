using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGV.Core.Maths
{
    /// <summary>
    /// A cartesian vector
    /// </summary>
    public class Vector
    {
        /// <summary>
        /// Direction of X Vector
        /// </summary>
        public double DirectionX;
        /// <summary>
        /// Direction of Y Vector
        /// </summary>
        public double DirectionY;
        /// <summary>
        /// Direction of Z Vector
        /// </summary>
        public double DirectionZ;
        /// <summary>
        /// Constructor of a cartesian vector
        /// </summary>
        /// <param name="DirectionX">Direction of X Vector</param>
        /// <param name="DirectionY">Direction of Y Vector</param>
        /// <param name="DirectionZ">Direction of Z Vector</param>
        public Vector(double DirectionX, double DirectionY, double DirectionZ)
        {
            this.DirectionX = DirectionX;
            this.DirectionY = DirectionY;
            this.DirectionZ = DirectionZ;
            Normalize();
        }
        /// <summary>
        /// String output of vector
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("<{0:N4}, {1:N4}, {2:N4}>", DirectionX, DirectionY, DirectionZ);
        }
        /// <summary>
        /// Rotate an Vector based on another Vector
        /// </summary>
        /// <param name="Axle">Axle of rotate</param>
        /// <param name="degree">degree of rotatein radian</param>
        public void Rotate(Vector Axle, double degree)
        {
            Normalize();
            double x = this.DirectionX;
            double y = this.DirectionY;
            double z = this.DirectionZ;
            double u = Axle.DirectionX;
            double v = Axle.DirectionY;
            double w = Axle.DirectionZ;
            double SinAngle = Math.Sin(degree);
            double CosAngle = Math.Cos(degree);
            double newX = u * (u * x + v * y + w * z) + (x * (v * v + w * w) - u * (v * y + w * z)) * CosAngle + (-w * y + v * z) * SinAngle;
            double newY = v * (u * x + v * y + w * z) + (y * (u * u + w * w) - v * (u * x + w * z)) * CosAngle + (w * x - u * z) * SinAngle;
            double newZ = w * (u * x + v * y + w * z) + (z * (u * u + v * v) - w * (u * x + v * y)) * CosAngle + (-v * x + u * y) * SinAngle;
            this.DirectionX = newX;
            this.DirectionY = newY;
            this.DirectionZ = newZ;
            Normalize();
        }
        /// <summary>
        /// Clone a Vector
        /// </summary>
        /// <returns></returns>
        public Vector Clone()
        {
            return new Vector(this.DirectionX, this.DirectionY, this.DirectionZ);
        }
        //cross product
        public Vector Cross(Vector V)
        {
            double vx = this.DirectionY * V.DirectionZ - this.DirectionZ * V.DirectionY;
            double vy = this.DirectionZ * V.DirectionX - this.DirectionX * V.DirectionZ;
            double vz = this.DirectionX * V.DirectionY - this.DirectionY * V.DirectionX;
            return new Vector(vx, vy, vz);
        }
        //dot product
        public double Dot(Vector V)
        {
            return this.DirectionX * V.DirectionX + this.DirectionY * V.DirectionY + this.DirectionZ * V.DirectionZ;
        }

        public double Absolute()
        {
            return Math.Sqrt(Math.Pow(this.DirectionX, 2) + Math.Pow(this.DirectionY, 2) + Math.Pow(this.DirectionZ, 2));
        }

        public double AngleBetweenVector(Vector V)
        {
            return Math.Acos(this.Dot(V) / (this.Absolute() * V.Absolute()));
        }
        /// <summary>
        /// Normalize the vector
        /// </summary>
        void Normalize()
        {
            double divider = Math.Sqrt(DirectionX * DirectionX + DirectionY * DirectionY + DirectionZ * DirectionZ);
            DirectionX /= divider;
            DirectionY /= divider;
            DirectionZ /= divider;
        }
    }
}
