using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGV.Core.Maths
{
    /// <summary>
    /// Collection of non-memory required math operations
    /// </summary>
    public static class Utility
    {
        #region Filter

        /// <summary>
        /// Calculate gaussian coefficient of x for x greater than expected value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="ExpectedValue">Expected value</param>
        /// <param name="Variance">variance of gaussian</param>
        /// <returns></returns>
        public static double GaussianLowPass(double x, double ExpectedValue, double Variance)
        {
            return x > ExpectedValue ? Gaussian(x, ExpectedValue, Variance) : 1.0;
        }

        /// <summary>
        /// Calculate gaussian coefficient of x for x less than expected value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="ExpectedValue">Expected value</param>
        /// <param name="Variance">variance of gaussian</param>
        /// <returns></returns>
        public static double GaussianHighPass(double x, double ExpectedValue, double Variance)
        {
            return x < ExpectedValue ? Gaussian(x, ExpectedValue, Variance) : 1.0;
        }

        /// <summary>
        /// Calculate gaussian coefficient of x
        /// </summary>
        /// <param name="x"></param>
        /// <param name="ExpectedValue">Expected value</param>
        /// <param name="Variance">variance of gaussian</param>
        /// <returns></returns>
        public static double Gaussian(double x, double ExpectedValue, double Variance)
        {
            return Math.Exp((x - ExpectedValue) * (x - ExpectedValue) / -2.0 / Variance);
        }

        #endregion Filter

        #region Angle

        /// <summary>
        /// Calculate average angle of two angle
        /// </summary>
        /// <param name="AngleA">Angle A in radian</param>
        /// <param name="AngleB">Angle B in radian</param>
        /// <returns></returns>
        public static double AverageAngle(double AngleA, double AngleB)
        {
            return AdjustAngle(AngleA, 0.5, AngleB, 0.5);
        }

        /// <summary>
        /// Adjust two angle based on each side's gain
        /// </summary>
        /// <param name="AngleA">Angle A in radian</param>
        /// <param name="GainA">Scale Gain of angle A</param>
        /// <param name="AngleB">Angle B in radian</param>
        /// <param name="GainB">Scale Gain of angle B</param>
        /// <returns></returns>
        public static double AdjustAngle(double AngleA, double GainA, double AngleB, double GainB)
        {
            //gain factor
            double GainFactor = (GainA + GainB) == 0 ? 0.5 : GainB / (GainA + GainB);
            //scale angle A and angle B
            while (AngleA > System.Math.PI)
                AngleA -= System.Math.PI * 2.0;
            while (AngleA < -System.Math.PI)
                AngleA += System.Math.PI * 2.0;
            while (AngleB > System.Math.PI)
                AngleB -= System.Math.PI * 2.0;
            while (AngleB < -System.Math.PI)
                AngleB += System.Math.PI * 2.0;
            //Calculate offset
            double OffsetAngle = AngleB - AngleA;
            //scale offset angle
            while (OffsetAngle > System.Math.PI)
                OffsetAngle -= System.Math.PI * 2.0;
            while (OffsetAngle < -System.Math.PI)
                OffsetAngle += System.Math.PI * 2.0;
            //calculate final angle
            double FinalAngle = AngleA + OffsetAngle * GainFactor;
            //scale final angle
            while (FinalAngle > System.Math.PI)
                FinalAngle -= System.Math.PI * 2.0;
            while (FinalAngle < -System.Math.PI)
                FinalAngle += System.Math.PI * 2.0;
            return FinalAngle;
        }

        #endregion Angle
    }
}
