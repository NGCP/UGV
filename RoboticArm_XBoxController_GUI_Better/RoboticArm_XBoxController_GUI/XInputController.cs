using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.XInput;
using System.Drawing;

namespace RoboticArm_XBoxController_GUI
{
    class XInputController
    {
        #region Properties
        // properties for control of the arm
        Controller controller;
        Gamepad gamepad;
        public bool connected = false;
        public int deadband = 2500;
        public Point leftThumb, rightThumb = new Point(0, 0);
        public float leftTrigger, rightTrigger;
        #endregion

        #region Constructor
        public XInputController()
        {
            controller = new Controller(UserIndex.One);
            connected = controller.IsConnected;
        }
        #endregion

        #region Methods
        // Call this method to update all class values
        public void Update()
        {
            if (!connected)
                return;

            gamepad = controller.GetState().Gamepad;

            leftThumb.X = (int)((Math.Abs((float)gamepad.LeftThumbX) < deadband) ? 0 : (float)gamepad.LeftThumbX / short.MinValue * -100);
            leftThumb.Y = (int)((Math.Abs((float)gamepad.LeftThumbY) < deadband) ? 0 : (float)gamepad.LeftThumbY / short.MaxValue * 100);
            rightThumb.X = (int)((Math.Abs((float)gamepad.RightThumbX) < deadband) ? 0 : (float)gamepad.RightThumbX / short.MaxValue * 100);
            rightThumb.Y = (int)((Math.Abs((float)gamepad.RightThumbY) < deadband) ? 0 : (float)gamepad.RightThumbY / short.MaxValue * 100);

            leftTrigger = gamepad.LeftTrigger;
            rightTrigger = gamepad.RightTrigger;
        }
        #endregion
    }
}
