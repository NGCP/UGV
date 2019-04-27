using System;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;
using Emgu.CV.Util;
using System.Net.Sockets;
using System.Threading;

namespace CameraCapture
{
    public partial class CameraCapture : Form
    {

        private VideoCapture _capture = null;
        private Mat _frame;
        private Mat hsv;
        private Mat maskRed, maskTemp, maskFinal;
        private Mat filteredRed;
        private Mat greyCircles;
        private bool _captureInProgress;
        private VectorOfVectorOfPoint maxContour = null;
        private VectorOfVectorOfPoint contours = null;
        private Point center;
        private int targetX;
        private int targetY;



        private MCvScalar lowerBound;
        private MCvScalar upperBound;
        private int hueMin = 0;
        private int satMin = 0;
        private int valMin = 0;
        private int hueMax = 255;
        private int satMax = 255;
        private int valMax = 255;
        private double dp = 1.5;
        private double minDist = 100.0;
        private bool found = false;
        private bool foundball = false;

        public CameraCapture()
        {
            InitializeComponent();
            _frame = new Mat();
            hsv = new Mat();
            maskRed = new Mat();
            maskTemp = new Mat();
            maskFinal = new Mat();
            filteredRed = new Mat();
            greyCircles = new Mat();
            contours = new VectorOfVectorOfPoint();
            maxContour = new VectorOfVectorOfPoint();
            center = new Point(0, 0);

            


            CvInvoke.UseOpenCL = false;
            try
            {
                _capture = new VideoCapture();
                _capture.ImageGrabbed += hsvMethod;

            }
            catch (NullReferenceException excpt)
            {
                MessageBox.Show(excpt.Message);
            }

        }           

        void hsvMethod(object sender, EventArgs e)
        {
            Mat resized_frame = new Mat();
            Mat resized_maskFinal = new Mat();

            //Capture frame from video feed
            if (_capture != null && _capture.Ptr != IntPtr.Zero)
            {
                _capture.Retrieve(_frame, 0);

                generateMask();

                //Find all contours in HSV range
                CvInvoke.FindContours(maskFinal, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

                // Find the largest contour
                if(largestContour() > 50)//MaxContour_trackBar1.Value)
                {
                    found = true;
                }     
                else
                {
                    found = false;
                }

                // Get center of contour
                findCenter(maxContour[0],0.2);


                CvInvoke.CvtColor(_frame, greyCircles, ColorConversion.Bgr2Gray);


                if (maxContour.Size != 0 && maxContour[0].Size != 0 && found)
                {
                    foundball = true;
                }
                else
                    foundball = false;
                #region circle detection
                    double cannyThreshold = 180.0;
                double circleAccumulatorThreshold = 120;
                CircleF[] circles = CvInvoke.HoughCircles(greyCircles, HoughType.Gradient, dp, minDist, cannyThreshold, circleAccumulatorThreshold, 5);
                #endregion

                #region draw circles
                Mat circleImage = new Mat(_frame.Size, DepthType.Cv8U, 3);
                circleImage.SetTo(new MCvScalar(0));
                foreach (CircleF circle in circles)
                    CvInvoke.Circle(_frame, Point.Round(circle.Center), (int)circle.Radius, new Bgr(Color.Brown).MCvScalar, 2);

                //circleImageBox.Image = circleImage;
                #endregion



                //Draw on frame
                CvInvoke.DrawContours(_frame, maxContour, -1, new MCvScalar(0.0, 255.0, 0.0));
                CvInvoke.Circle(_frame, center, 5, new MCvScalar(255.0, 0.0, 0.0), 5);

                targetX = center.X;
                targetY = center.Y;

                //TargetX_textBox1.Text = center.X.ToString();
                //TargetY_textBox2.Text = center.Y.ToString();
                Thread demoThread = new Thread(new ThreadStart(this.ThreadProcSafe));
                demoThread.Start();

                CvInvoke.Resize(maskFinal, resized_maskFinal, new Size(imageBox1.Width, imageBox1.Height), 0, 0, Inter.Linear);    //This resizes the image to the size of Imagebox1 
                CvInvoke.Resize(_frame, resized_frame, new Size(imageBox2.Width, imageBox2.Height), 0, 0, Inter.Linear);    //This resizes the image to the size of Imagebox1 
                imageBox1.Image = resized_maskFinal;    // black and white image
                imageBox2.Image = resized_frame;         // color image
                sendData();
            }
        }

        public int largestContour()
        {
            int maxVal = 0;
            for (int i = 0; i < contours.Size; i++)
            {
                if (contours[i].Size > maxVal)
                {
                    maxVal = contours[i].Size;
                    if (maxContour.Size != 0)
                        maxContour.Clear();
                    maxContour.Push(contours[i]);
                }
            }

            return maxVal;
        }

        public void findCenter(VectorOfPoint maxContourTemp,double error)
        {
            // Get center of contour
            int avgX = 0, avgY = 0;
            for (int i = 0; i < maxContourTemp.Size; i++)
            {
                avgX += maxContourTemp[i].X;
                avgY += maxContourTemp[i].Y;
            }
            avgX /= maxContourTemp.Size;
            avgY /= maxContourTemp.Size;
            bool notskip = true;
            //Set center of contour
            center = new Point(avgX, avgY);
            //double firstDist = eucDist(maxContourTemp[0], center);
            //for (int i = 1; i < maxContour.Size; i++)
            //{
            //    // Check if each point distance is within the allowable error range
            //    if (Math.Abs(eucDist(maxContourTemp[i], center) - firstDist) / firstDist > error)
            //    {
            //        found = false;
            //        notskip = false; 
            //    }
            //}
            //if(notskip)
            //    found = true;
        }



        public double eucDist(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(p1.X - p2.X), 2) + Math.Pow(Math.Abs(p1.Y - p2.Y), 2));
        }
        public void generateMask()
        {

            //Convert BGR to HSV
            CvInvoke.CvtColor(_frame, hsv, ColorConversion.Bgr2Hsv);
            // Isolate Color range of interest, smooth, and convert to b/w
            lowerBound = new MCvScalar(hueMin, satMin, valMin);
            upperBound = new MCvScalar(hueMax, satMax, valMax);
            CvInvoke.InRange(hsv, new ScalarArray(lowerBound), new ScalarArray(upperBound), maskRed);
            CvInvoke.GaussianBlur(maskRed, filteredRed, new Size(25, 25), 0.0);
            CvInvoke.Threshold(filteredRed, maskFinal, 150.0, 255.0, ThresholdType.Binary);

        }

        private void ReleaseData()
        {
            if (_capture != null)
            {
                //_capture.ImageGrabbed -= hsvMethod;
                _capture.ImageGrabbed -= hsvMethod;
                _capture.Pause();
                _capture.Dispose();
            }    
        }

        private void captureButton_Click(object sender, EventArgs e)
        {
            if (_capture != null)
            {
                if (_captureInProgress)
                {  //stop the capture
                    captureButton.Text = "Start Capture";
                    _capture.Pause();
                }
                else
                {
                    //start the capture
                    captureButton.Text = "Stop";
                    _capture.Start();
                }

                _captureInProgress = !_captureInProgress;
            }
        }

        #region trackbars       
        private void HueMin_trackBar1_ValueChanged(object sender, EventArgs e)
        {
            hueMin = HueMin_trackBar1.Value;
        }

        private void HueMax_trackBar6_ValueChanged(object sender, EventArgs e)
        {
            hueMax = HueMax_trackBar6.Value;
        }

        private void SatMin_trackBar5_ValueChanged(object sender, EventArgs e)
        {
            satMin = SatMin_trackBar5.Value;
        }

        private void SatMax_trackBar4_ValueChanged(object sender, EventArgs e)
        {
            satMax = SatMax_trackBar4.Value;
        }

        private void dp_trackBar2_ValueChanged(object sender, EventArgs e)
        {
            dp = (double)dp_trackBar2.Value / 100.0;
            dp_textBox2.Text = dp.ToString();
        }

        private void minDist_trackBar1_ValueChanged(object sender, EventArgs e)
        {
            minDist = (double)minDist_trackBar1.Value;
            minDist_textBox1.Text = minDist.ToString();
        }

        private void valMax_trackBar1_ValueChanged(object sender, EventArgs e)
        {
            valMax = valMax_trackBar1.Value;

        }

        private void ValMin_trackBar3_ValueChanged(object sender, EventArgs e)
        {
            valMin = ValMin_trackBar3.Value;
        }

        #endregion

        void sendData()
        {
            string reciever_ip = "127.0.0.1";
            int udp_port = 8008;
            UdpClient udp = new UdpClient();
            udp.Connect(reciever_ip, udp_port);

            int msgSize = sizeof(int) + sizeof(int);
            byte[] SendX = BitConverter.GetBytes(targetX);
            byte[] SendY = BitConverter.GetBytes(targetY);
            byte[] SendBallfound = BitConverter.GetBytes(found);
            byte[] sendmsg = new byte[msgSize];

            Buffer.BlockCopy(SendX, 0, sendmsg, 0, SendX.Length);
            Buffer.BlockCopy(SendY, 0, sendmsg, SendX.Length, SendY.Length);
            udp.Send(sendmsg, sendmsg.Length);

        }




        private void button1_Click(object sender, EventArgs e)
        {
            //Thread demoThread = new Thread(new ThreadStart(this.ThreadProcSafe));
            //demoThread.Start();
        }

        private void CameraCapture_Load(object sender, EventArgs e)
        {
            HueMin_trackBar1.Value = Properties.Settings.Default.Hue_Min;
            HueMax_trackBar6.Value = Properties.Settings.Default.Hue_Max;
            SatMin_trackBar5.Value = Properties.Settings.Default.Sat_Min;
            SatMax_trackBar4.Value = Properties.Settings.Default.Sat_Max;
            ValMin_trackBar3.Value = Properties.Settings.Default.Value_Min;
            valMax_trackBar1.Value = Properties.Settings.Default.Value_Max;
        }

        private void CameraCapture_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Hue_Min = HueMin_trackBar1.Value;
            Properties.Settings.Default.Hue_Max = HueMax_trackBar6.Value;
            Properties.Settings.Default.Sat_Min = SatMin_trackBar5.Value;
            Properties.Settings.Default.Sat_Max = SatMax_trackBar4.Value;
            Properties.Settings.Default.Value_Min = ValMin_trackBar3.Value;
            Properties.Settings.Default.Value_Max = valMax_trackBar1.Value;
            Properties.Settings.Default.Save();
        }


        // This method is executed on the worker thread and makes 
        // a thread-safe call on the TextBox control. 
        private void ThreadProcSafe()
        {
            ThreadHelperClass.SetText(this, TargetX_textBox1, center.X.ToString());
            ThreadHelperClass.SetText(this, TargetY_textBox2, center.Y.ToString());
            ThreadHelperClass.SetText(this, ballfound,foundball.ToString());
        }


    }



    // ThreadHelper class is needed to avoid throwing the "cross-thread operation not valid" exception
    // This class also may prevent the camera capture error exception
    public static class ThreadHelperClass
    {
        delegate void SetTextCallback(Form f, Control ctrl, string text);
        /// <summary>
        /// Set text property of various controls
        /// </summary>
        /// <param name="form">The calling form</param>
        /// <param name="ctrl"></param>
        /// <param name="text"></param>
        public static void SetText(Form form, Control ctrl, string text)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (ctrl.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                form.Invoke(d, new object[] { form, ctrl, text });
            }
            else
            {
                ctrl.Text = text;
            }
        }
    }


}
