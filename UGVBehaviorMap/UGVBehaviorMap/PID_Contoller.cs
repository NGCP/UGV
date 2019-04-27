using System;
using System.Diagnostics;
using System.IO;

namespace NGCP.UGV
{
    /// <summary>
    /// A (P)roportional, (I)ntegral, (D)erivative Controller
    /// </summary>
    /// <remarks>
    /// The controller should be able to control any process with a
    /// measureable value, a known ideal value and an input to the
    /// process that will affect the measured value.
    /// </remarks>
    /// <see cref="https://en.wikipedia.org/wiki/PID_controller"/>
    public class PID_Contoller
    {
        private double processVariable = 0;

        private Stopwatch sw = new Stopwatch();



        #region Properties
        /// <summary>
        /// The derivative term is proportional to the rate of
        /// change of the error
        /// </summary>
        public double Kd { get; set; } = 0;

        /// <summary>
        /// The integral term is proportional to both the magnitude
        /// of the error and the duration of the error
        /// </summary>
        public double Ki { get; set; } = 0;

        /// <summary>
        /// The proportional term produces an output value that
        /// is proportional to the current error value
        /// </summary>
        /// <remarks>
        /// Tuning theory and industrial practice indicate that the
        /// proportional term should contribute the bulk of the output change.
        /// </remarks>
        public double Kp { get; set; } = 0;

        /// <summary>
        /// The max output value the control device can accept.
        /// </summary>
        public double OutputMax { get; private set; } = 0;

        /// <summary>
        /// The minimum ouput value the control device can accept.
        /// </summary>
        public double OutputMin { get; private set; } = 0;

        /// <summary>
        /// Adjustment made by considering the accumulated error over time
        /// </summary>
        /// <remarks>
        /// An alternative formulation of the integral action, is the
        /// proportional-summation-difference used in discrete-time systems
        /// </remarks>
        public double IntegralTerm { get; private set; } = 0;


        /// <summary>
        /// The current value
        /// </summary>
        public double ProcessVariable
        {
            get { return processVariable; }
            set
            {
                ProcessVariableLast = processVariable;
                processVariable = value;
            }
        }

        /// <summary>
        /// The last reported value (used to calculate the rate of change)
        /// </summary>
        public double ProcessVariableLast { get; private set; } = 0;

        /// <summary>
        /// The desired value
        /// </summary>
        public double SetPoint { get; set; } = 0;
        #endregion

        #region Contructor
        public PID_Contoller(double Kp, double Ki, double Kd, double outMax, double outMin)
        {
            this.Kp = Kp;
            this.Ki = Ki;
            this.Kd = Kd;
            this.OutputMax = outMax;
            this.OutputMin = outMin;
        }
        #endregion

        #region Methods
        /// <summary>
        /// The controller output
        /// </summary>
        /// <param name="timeSinceLastUpdate">timespan of the elapsed time
        /// since the previous time that ControlVariable was called</param>
        /// <returns>Value of the variable that needs to be controlled</returns>
        public double ControlVariable(TimeSpan timeSinceLastUpdate)
        {
            double error = SetPoint - ProcessVariable;
            Console.WriteLine("The error term value is {0}", error);

            // integral term calculation
            IntegralTerm += (Ki * error * timeSinceLastUpdate.TotalSeconds);
            IntegralTerm = Clamp(IntegralTerm);
            Console.WriteLine("The integral term value is {0}", IntegralTerm);

            // derivative term calculation
            double dInput = processVariable - ProcessVariableLast;

            double derivativeTerm = Kd * (dInput / timeSinceLastUpdate.TotalSeconds);

            Console.WriteLine("The derivative term value is {0}", derivativeTerm);

            // proportional term calcullation
            double proportionalTerm = Kp * error;
            Console.WriteLine("The proportional term value is {0}", proportionalTerm);

            double output = proportionalTerm + IntegralTerm + derivativeTerm;               // changed (-) dterm --> (+) dterm

            output = Clamp(output);

            return output;
        }


        // record values to text file-----------------------------------------------------------------
        string file_path = "C:\\Users\\UGV\\Desktop\\PID_test_values";

       // System.IO.File.
       



        /// <summary>
        /// Limit a variable to the set OutputMax and OutputMin properties
        /// </summary>
        /// <returns>
        /// A value that is between the OutputMax and OutputMin properties
        /// </returns>
        /// <remarks>
        /// Inspiration from http://stackoverflow.com/questions/3176602/how-to-force-a-number-to-be-in-a-range-in-c
        /// </remarks>
        private double Clamp(double variableToClamp)
        {
            if (variableToClamp <= OutputMin) { return OutputMin; }
            if (variableToClamp >= OutputMax) { return OutputMax; }
            return variableToClamp;
        }
        #endregion
    }
}
