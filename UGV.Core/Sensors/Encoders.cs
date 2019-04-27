using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Ports;

namespace UGV.Core.Sensors
{
    /*This class reads the encoders controlled by the arduino, and calculates the degrees the wheels
     * have turned and at what speed. It also provides the current speed. */


    public class Encoders : Sensor<EncodersPackage>
    {
        /// <summary>
        /// The speed measured by the encoders in m/s
        /// </summary>
        public double speed { get; private set; }

        /// <summary>
        /// The degree of turn measured by the encoders in radians left of forward
        /// </summary>
        public double turn { get; private set; }

        private double rightDistance;
        private double leftDistance;
        private const double CIRCUMFRANCE = 68 * 2 * Math.PI;//mm
        private const double WIDTH = 365;//mm


        public override void Update(EncodersPackage package)
        {
            leftDistance = package.left;
            rightDistance = package.right;

            //The distance the vehicle has travelled in past 1/1000 of a second in mm
            //Note: mm/ms is m/s, so the following is in m/s
            speed = (rightDistance + leftDistance) * CIRCUMFRANCE / 2;
            

            //The angle the vehicle is turning at for the past 1/1000 of a second in rad
            //The value is positive when the vehicle is turning left and negative when turning right
            turn = (rightDistance - leftDistance) / (WIDTH * 2);
        }
    }

    public class EncodersPackage : SensorPackage
    {
        public double left, right;

    }
}