using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Net.Sockets;

namespace ObstacleDetection
{
    class Program
    {
        static void Main(string[] args)
        {
            const int GET_NUM = 10;
            // reduce start and end step to avoid detecting wheels
            const int start_step = 140; // min 0
            const int end_step = 930; // max 1080

            //UDP Information
            string receiver_ip = "127.0.0.1";
            int udp_port = 8008;


            //Serial Information 
            int baudrate = 115200;
            string port_name = "COM4";

            //Distance Information
            double maxDistance = 5000;

            var file = new IniFile("Settings.ini");
            var value = file.Read("IPAddress", "Lidar");
            if (value != null && value.ToString().Length > 0)
            {
                receiver_ip = value.ToString();
            }
            else
            {
                Console.WriteLine("Error reading INI File, using default value.");
            }

            value = file.Read("IPPort", "Lidar");
            if (value != null && value.ToString().Length > 0)
            {
                udp_port = int.Parse(value.ToString());
            }
            else
            {
                Console.WriteLine("Error reading INI File, using default value.");
            }

            value = file.Read("LidarBaud", "Lidar");
            if (value != null && value.ToString().Length > 0)
            {
                baudrate = int.Parse(value.ToString());
            }
            else
            {
                Console.WriteLine("Error reading INI File, using default value.");
            }

            value = file.Read("LidarPort", "Lidar");
            if (value != null && value.ToString().Length > 0)
            {
                port_name = value.ToString();
            }
            else
            {
                Console.WriteLine("Error reading INI File, using default value.");
            }

            value = file.Read("MaxDistance", "Lidar");
            if (value != null && value.ToString().Length > 0)
            {
                maxDistance = double.Parse(value.ToString());
            }
            else
            {
                Console.WriteLine("Error reading INI File, using default value.");
            }

            try
            {
                //Open Serial Port
                SerialPort urg = new SerialPort(port_name, baudrate);
                urg.NewLine = "\n\n";
                urg.Open();

                //Open UDp port
                UdpClient udp = new UdpClient();
                udp.Connect(receiver_ip, udp_port);

                //Set Lidar to Measurement Mode
                urg.Write(SCIP_Writer.setSCIP2Mode());
                urg.ReadLine(); //ignore SCIP echo back
                while (true)
                {
                    //Console.WriteLine("Loop: " + count++);
                    string test;
                    urg.Write(SCIP_Writer.requestSensorData(start_step, end_step));
                    test = urg.ReadLine(); // ignore echo back

                    //Receive data from the lidar
                    List<double> distances = new List<double>();
                    long time_stamp = 0;
                    for (int i = 0; i < GET_NUM; ++i)
                    {
                        string receive_data = urg.ReadLine();
                        if (!SCIP_Reader.getSensorData(receive_data, ref time_stamp, ref distances))
                        {
                            //Console.WriteLine(receive_data);
                            break;
                        }

                        if (distances.Count == 0)
                        {
                            //Console.WriteLine(receive_data);
                            continue;
                        }
                    }

                    double[] distanceArray = distances.ToArray();

                    //Parse Objects from lidar data
                    ObstacleDetector od = new ObstacleDetector(0, distances, maxDistance);
                    VectorSum vectorSum = od.getVectorSum();

                    Console.Write("count: {0,10}", vectorSum.getCount());
                    Console.Write("   Angle: {0,10}", vectorSum.getAngle() * 180 / Math.PI);
                    Console.WriteLine("   Mag:   {0}", vectorSum.getMagnitude());

                    //Send objects to behavior
                    int msgSize = 2 * sizeof(double) + sizeof(int);

                    byte[] count_bytes = BitConverter.GetBytes(vectorSum.getCount());
                    byte[] magnitude_bytes = BitConverter.GetBytes(vectorSum.getMagnitude());
                    byte[] angle_bytes = BitConverter.GetBytes(vectorSum.getAngle() - Math.PI / 2);
                    byte[] sendBytes = new byte[msgSize];

                    Buffer.BlockCopy(count_bytes, 0, sendBytes, 0, count_bytes.Length);
                    Buffer.BlockCopy(magnitude_bytes, 0, sendBytes, count_bytes.Length, magnitude_bytes.Length);
                    Buffer.BlockCopy(angle_bytes, 0, sendBytes, count_bytes.Length + magnitude_bytes.Length, angle_bytes.Length);

                    udp.Send(sendBytes, sendBytes.Length);
                }
                urg.Write(SCIP_Writer.turnOffLaser()); // stop measurement mode
                urg.ReadLine(); // ignore echo back

                urg.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine("Press any key.");
                Console.ReadKey();
            }
        }
    }
}