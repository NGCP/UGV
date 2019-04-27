using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGV.Core.Maths
{
    /// <summary>
    /// PID controller
    /// </summary>
    public class PID
    {
        /// <summary>
        /// Sampling period of pid control system
        /// </summary>
        double SamplingPeriod;
        /// <summary>
        /// Calculated Gains
        /// </summary>
        double K1, K2, K3;
        /// <summary>
        /// delay state memory
        /// </summary>
        double R1, R2, R3;
        /// <summary>
        /// Limit of output memory
        /// </summary>
        double Limit;
        /// <summary>
        /// Constructor of a PID controller
        /// </summary>
        /// <param name="SamplingFrequency">sampling frequency of PID in Hz</param>
        /// <param name="Limit">Limit of output memory, 0 for no limit</param>
        public PID(double SamplingFrequency, double Limit = 0)
        {
            this.SamplingPeriod = 1.0 / SamplingFrequency;
            this.Limit = Limit;
        }
        /// <summary>
        /// Constructor of a PID controller
        /// </summary>
        /// <param name="SamplingFrequency">sampling frequency of PID in Hz</param>
        /// <param name="Kp">Propotional Gain</param>
        /// <param name="Ki">Integrate Gain</param>
        /// <param name="Kd">Devritivational Gain</param>
        /// <param name="Limit">Limit of output memory, 0 for no limit</param>
        public PID(double SamplingFrequency, double Kp, double Ki, double Kd, double Limit = 0)
        {
            this.SamplingPeriod = 1.0 / SamplingFrequency;
            K1 = Kp + Ki * SamplingPeriod + Kd / SamplingPeriod;
            K2 = Kp + 2 * Kd / SamplingPeriod;
            K3 = Kd / SamplingPeriod;
            this.Limit = Limit;
        }
        /// <summary>
        /// PID gain of the control system
        /// </summary>
        /// <param name="Kp">Propotional Gain</param>
        /// <param name="Ki">Integrate Gain</param>
        /// <param name="Kd">Devritivational Gain</param>
        /// <param name="Limit">Limit of output memory, 0 for no limit</param>
        public void SetPID(double Kp, double Ki, double Kd, double Limit = 0)
        {
            K1 = Kp + Ki * SamplingPeriod + Kd / SamplingPeriod;
            K2 = Kp + 2 * Kd / SamplingPeriod;
            K3 = Kd / SamplingPeriod;
            this.Limit = Limit;
        }
        /// <summary>
        /// Feed a input to PID and get a output
        /// </summary>
        /// <param name="Input">input of sensor</param>
        /// <returns></returns>
        public double Feed(double ExpectedValue, double Input)
        {
            double Error = ExpectedValue - Input;
            R3 += K1 * Error - K2 * R1 + K3 * R2;
            R2 = R1;
            R1 = Error;
            if (Limit > 0)
                R3 = Math.Min(Math.Max(R3, -Limit), Limit);
            return R3;
        }
        /// <summary>
        /// Feed a input to PID and get a output
        /// </summary>
        /// <param name="Error">Error of control: expected value - feed back</param>
        /// <returns></returns>
        public double Feed(double Error)
        {
            R3 += K1 * Error - K2 * R1 + K3 * R2;
            R2 = R1;
            R1 = Error;
            if (Limit > 0)
                R3 = Math.Min(Math.Max(R3, -Limit), Limit);
            return R3;
        }
    }
}
