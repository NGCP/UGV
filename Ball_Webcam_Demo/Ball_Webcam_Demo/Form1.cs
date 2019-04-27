using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing.Imaging;

namespace Ball_Webcam_Demo
{
    public partial class Form1 : Form
    {
        // private VideoCapture_capture = null;
        // video stream form webcam
        private VideoCapture _capture = null;
        private bool findPayload;
        private bool ballfound;
        private double angle;
        private Mat _frame;
        private Mat hsv;
        private Mat maskRed, maskBlue, maskYellow, maskTemp, maskFinal;
        private Mat filteredRed, filteredBlue, filteredYellow;
        private VectorOfVectorOfPoint maxContour = null;
        private VectorOfVectorOfPoint contours = null;
        private Point center;

        //private Image<Bgr, byte> tempImg;
        private MCvScalar lowerBoundRed;
        private MCvScalar upperBoundRed;
        private MCvScalar lowerBoundBlue;
        private MCvScalar upperBoundBlue;
        private MCvScalar lowerBoundYellow;
        private MCvScalar upperBoundYellow;
        public Form1()
        {
            InitializeComponent();

            

            _frame = new Mat();
            maxContour = new VectorOfVectorOfPoint();

            //Red HSV
            lowerBoundRed = new MCvScalar(0, 50, 30);
            upperBoundRed = new MCvScalar(15, 255, 255);

            // Blue HSV?
            lowerBoundBlue = new MCvScalar(142, 50, 30);
            upperBoundBlue = new MCvScalar(170, 255, 255);

            // Yellow HSV?
            lowerBoundYellow = new MCvScalar(33, 50, 30);
            upperBoundYellow = new MCvScalar(45, 255, 255);

            findPayload = false;
            ballfound = false;

            // Cv specific variables
            hsv = new Mat();
            maskRed = new Mat();
            filteredRed = new Mat();
            maskBlue = new Mat();
            filteredBlue = new Mat();
            maskYellow = new Mat();
            filteredYellow = new Mat();
            maskTemp = new Mat();
            maskFinal = new Mat();
            contours = new VectorOfVectorOfPoint();

            CvInvoke.UseOpenCL = false;
            try
            {
                _capture = new VideoCapture();
                _capture.Start();
                _capture.ImageGrabbed += hsvMethod;
            }
            catch (NullReferenceException excpt)
            {
                MessageBox.Show(excpt.Message);
            }
        }

        void hsvMethod(object sender, EventArgs e)
        {
            //Capture frame from video feed
            _capture.Retrieve(_frame, 0);

            generateMask();

            //Find all contours in HSV range
            CvInvoke.FindContours(maskFinal, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            // Find the largest contour
            largestContour();

            //Check if contour is ball shaped (10 pixel error)
            if (maxContour.Size != 0 && maxContour[0].Size != 0 && isBallShaped(maxContour[0], 10.20))
            {
                ballfound = true;
                angle = calcangle(center);
                textBox1.Text = "True";
                textBox2.Text = angle.ToString() ;//325,240
                //textBox2.Text = center.ToString();
                 CvInvoke.Line(_frame, center, new Point(center.X + 1, center.Y + 1), new MCvScalar(0.0, 0.0, 255.0), 3);
            }
            else
            {
                ballfound = false;
                textBox1.Text = "False";
                textBox2.Text = "No Contour";
            }

            //Draw on frame
            CvInvoke.DrawContours(_frame, maxContour, -1, new MCvScalar(0.0, 255.0, 0.0));
            imageBox1.Image = _frame;
            sendData();

            // Dispose all variables
            //disposeAllVars();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }

        public bool isBallShaped(VectorOfPoint maxContour, double error)
        {
            // Get center of contour
            int avgX = 0, avgY = 0;
            for (int i = 0; i < maxContour.Size; i++)
            {
                avgX += maxContour[i].X;
                avgY += maxContour[i].Y;
            }
            avgX /= maxContour.Size;
            avgY /= maxContour.Size;

            //Set center of contour
            center = new Point(avgX, avgY);

            double firstDist = eucDist(maxContour[0], center);

            for (int i = 1; i < maxContour.Size; i++)
            {
                // Check if each point distance is within the allowable error range
                if (Math.Abs(eucDist(maxContour[i], center) - firstDist) / firstDist > error) return false;
            }
            return true;
        }

        public void generateMask()
        {
            if (findPayload)
            {
                //Convert BGR to HSV
                CvInvoke.CvtColor(_frame, hsv, ColorConversion.Bgr2Hsv);
                // Isolate Color range of interest, smooth, and convert to b/w
                //Red
                CvInvoke.InRange(hsv, new ScalarArray(lowerBoundRed), new ScalarArray(upperBoundRed), maskRed);
                CvInvoke.GaussianBlur(maskRed, filteredRed, new Size(25, 25), 0.0);
                CvInvoke.Threshold(filteredRed, maskRed, 150.0, 255.0, ThresholdType.Binary);

                //Blue
                CvInvoke.InRange(hsv, new ScalarArray(lowerBoundBlue), new ScalarArray(upperBoundBlue), maskBlue);
                CvInvoke.GaussianBlur(maskBlue, filteredBlue, new Size(25, 25), 0.0);
                CvInvoke.Threshold(filteredBlue, maskBlue, 150.0, 255.0, ThresholdType.Binary);

                //Yellow
                CvInvoke.InRange(hsv, new ScalarArray(lowerBoundYellow), new ScalarArray(upperBoundYellow), maskYellow);
                CvInvoke.GaussianBlur(maskYellow, filteredYellow, new Size(25, 25), 0.0);
                CvInvoke.Threshold(filteredYellow, maskYellow, 150.0, 255.0, ThresholdType.Binary);

                CvInvoke.BitwiseOr(maskRed, maskBlue, maskTemp);
                CvInvoke.BitwiseOr(maskTemp, maskYellow, maskFinal);
            }
            else
            {
                //Convert BGR to HSV
                CvInvoke.CvtColor(_frame, hsv, ColorConversion.Bgr2Hsv);
                // Isolate Color range of interest, smooth, and convert to b/w
                CvInvoke.InRange(hsv, new ScalarArray(lowerBoundRed), new ScalarArray(upperBoundRed), maskRed);
                CvInvoke.GaussianBlur(maskRed, filteredRed, new Size(25, 25), 0.0);
                CvInvoke.Threshold(filteredRed, maskFinal, 150.0, 255.0, ThresholdType.Binary);
            }
        }

        //Set the maximum contour
        public void largestContour()
        {
            int maxVal = maxContour_trackBar1.Value;
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
        }

        // Euclidian distance between two points
        public double eucDist(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(p1.X - p2.X), 2) + Math.Pow(Math.Abs(p1.Y - p2.Y), 2));
        }
        public double calcangle(Point p1)
        {
            Point centerx = new Point { X = 360, Y = 480 };


            double a, b, c, theta;
            a = p1.X - centerx.X;//85
            b = p1.Y - centerx.Y;//-120
            c = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));//147
            theta = Math.Acos(a / c);
            theta = theta * (180 / Math.PI);
            theta = theta-90;


            return theta;
        }
        void sendData()
        {
            string reciever_ip = "127.0.0.1";
            int udp_port = 8008;
            UdpClient udp = new UdpClient();
            udp.Connect(reciever_ip, udp_port);

            int msgSize = sizeof(bool) + sizeof(double);
            byte[] found = BitConverter.GetBytes(ballfound);
            byte[] targetangle = BitConverter.GetBytes(angle);
            byte[] sendmsg = new byte[msgSize];

            Buffer.BlockCopy(found, 0, sendmsg, 0, found.Length);
            Buffer.BlockCopy(targetangle, 0, sendmsg, found.Length, targetangle.Length);
            udp.Send(sendmsg, sendmsg.Length);

        }
    }
}
