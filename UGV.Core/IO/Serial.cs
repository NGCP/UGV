using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace UGV.Core.IO
{
    /// <summary>
    /// Extended method for the List class
    /// </summary>
    public static class ExtendedList
    {
        public static int IndexOfSeq<T>(this List<T> list, T[] array) where T : IComparable
        {
            for (int i = 0; i <= list.Count - array.Length; i++)
            {
                if (list[i].CompareTo(array[0]) != 0) continue;
                bool confirm = true;
                //check for ending sequence
                for (int s = 1; s < array.Length; s++)
                    if (list[i + s].CompareTo(array[s]) != 0)
                        confirm = false;
                if (confirm)
                    return i;
            }
            return -1;
        }
    }
    /// <summary>
    /// An Serial based sensor
    /// </summary>
    public class Serial : Link
    {
        public int count = 0;

        /// <summary>
        /// Package mode of link
        /// </summary>
        public enum PackageModes
        {
            /// <summary>
            /// Use EscapeToken to determine end of package
            /// </summary>
            UseEscapeToken,
            /// <summary>
            /// Use FindPackageEnd Function to extract the package
            /// </summary>
            UseFunction,
            /// <summary>
            /// true if recieving FPGA data, false if not
            /// </summary>
            UseFPGA
        }

        /// <summary>
        /// Amount of byte limit max of a single package include escape tokens
        /// </summary>
        public int PackageMaxSizeLimit = 65536;
        /// <summary>
        /// User defined token of ending a package
        /// </summary>
        public byte[] EscapeToken = { (byte)'\n' };
        /// <summary>
        /// User defined function to find the end of package 
        /// </summary>
        public Func<byte[], int> FindPackageEnd = (b) => b.Length;
        /// <summary>
        /// Package Mode of link
        /// </summary>
        public PackageModes PackageMode = PackageModes.UseEscapeToken;
        /// <summary>
        /// Serial port of COM
        /// </summary>
        SerialPort COM;
        /// <summary>
        /// Timer to read data from buffer used if data data recived event will not work on the current OS
        /// </summary>
        System.Timers.Timer read;
        /// <summary>
        /// byte sequence of message income
        /// </summary>
        List<byte> message;
        /// <summary>
        /// Construct a Serial based sensor object
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        public Serial(string portName, int baudRate)
        {
            message = new List<byte>();
            COM = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
            COM.DataReceived += new SerialDataReceivedEventHandler(OnDataReceived);
        }

        public Serial(string portName, int baudRate, int checkTime)
        {
            message = new List<byte>();
            COM = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
            read = new System.Timers.Timer();
            read.Interval = checkTime;
            read.Elapsed += new System.Timers.ElapsedEventHandler(OnTick);

        }
        /// <summary>
        /// If IO is active
        /// </summary>
        /// <returns></returns>
        public override bool IsActive()
        {
            //check object
            if (COM == null)
                return false;
            //check port name
            if (!SerialPort.GetPortNames().Contains(COM.PortName))
                return false;
            //check port open
            return COM.IsOpen;
        }
        /// <summary>
        /// Start read serial communication
        /// </summary>
        public override void Start()
        {
            COM.Open();

            if (read != null)
            {
                read.Start();
            }
        }
        /// <summary>
        /// Stop read serial communication
        /// </summary>
        public override void Stop()
        {
            COM.Close();

            if (read != null)
            {
                read.Stop();
            }
        }
        /// <summary>
        /// Send a serial data package
        /// </summary>
        /// <param name="bytes"></param>
        public override void Send(byte[] bytes)
        {
            //try send
            try
            {
                COM.Write(bytes, 0, bytes.Length);
            }
            catch (Exception)
            {
                //if send error try to restart
                try
                {
                    Start();
                }
                catch (Exception)
                {
                    //but some times, serial is moody
                }
            }
        }
        /// <summary>
        /// Callback for data received
        /// </summary>
        void OnDataReceived(object sender, SerialDataReceivedEventArgs args)
        {
            //count++;
            while (COM.BytesToRead > 0)
            {
                //read byte from port
                byte ByteRead = (byte)COM.ReadByte();
                //add onto list
                message.Add(ByteRead);
                //remove first byte if message exceed size limit
                if (message.Count > PackageMaxSizeLimit)
                    message.RemoveAt(0);
                //find package end
                int loc = -1;
                if (PackageMode == PackageModes.UseFPGA)
                {
                    // if first byte is start of header
                    if (message[0] == (byte)0x01)
                    {
                        // find eof
                        loc = message.IndexOf(0x04);
                        // if eof detected
                        if (loc > 0)
                        {
                            int eof = loc + 1;
                            //fetch package
                            if (message.Count == 9)
                            {
                                byte[] package = new byte[eof];
                                try
                                {
                                    int messageLength = eof;
                                    for (int i = 0; i < messageLength; i++)
                                        package[i] = message[i];
                                    //take off read package
                                    message.RemoveRange(0, messageLength);
                                    //do action of package received
                                    if (PackageReceived != null)
                                        PackageReceived(package);
                                    count++;
                                }
                                catch (IndexOutOfRangeException) { }
                            }
                            // if found end of file, but message is too short, then clear message
                            else if ((loc != -1) && (eof < 9))
                                message.Clear();
                            // if found end of file, but message is too long, then start clearing message until a correct message is found
                            // probably dont need to check loc now that i realize it
                            else if ((loc != -1) && (eof > 9))
                                message.RemoveAt(0);


                        }


                    }
                    else message.RemoveAt(0);
                }
                // else if not using FPGA
                else
                {
                    if (PackageMode == PackageModes.UseEscapeToken)
                    {
                        //if eof detected
                        loc = message.IndexOfSeq<byte>(EscapeToken);

                    }
                    else if (PackageMode == PackageModes.UseFunction)
                    {
                        //if eof detected
                        loc = FindPackageEnd(message.ToArray());
                    }
                    //notify package if found
                    if (loc > -1)
                    {
                        //fetch package
                        byte[] package = new byte[loc];
                        try
                        {
                            for (int i = 0; i < loc; i++)
                                package[i] = message[i];
                            //take off read package
                            message.RemoveRange(0, loc + EscapeToken.Length);
                            //do action of package received
                            if (PackageReceived != null)
                                PackageReceived(package);
                            count++;
                        }
                        catch (IndexOutOfRangeException) { }
                    }
                }
            }
        }
        //reading a package
        void OnTick(object sender, System.Timers.ElapsedEventArgs e)
        {
            read.Enabled = false;

            while (COM.BytesToRead > 0)
            {
                //read byte from port
                byte ByteRead = (byte)COM.ReadByte();
                //add onto list
                message.Add(ByteRead);
                //remove first byte if message exceed size limit
                if (message.Count > PackageMaxSizeLimit)
                    message.RemoveAt(0);
                //find package end
                int loc = -1;
                if (PackageMode == PackageModes.UseEscapeToken)
                {
                    //if eof detected
                    loc = message.IndexOfSeq<byte>(EscapeToken);

                }
                else if (PackageMode == PackageModes.UseFunction)
                {
                    //if eof detected
                    loc = FindPackageEnd(message.ToArray());
                }
                //notify package if found
                if (loc > -1)
                {
                    //fetch package
                    byte[] package = new byte[loc];
                    for (int i = 0; i < loc; i++)
                        package[i] = message[i];
                    //take off read package
                    message.RemoveRange(0, loc + EscapeToken.Length);
                    //do action of package received
                    if (PackageReceived != null)
                        PackageReceived(package);
                    count++;
                }
            }

            read.Enabled = true;
        }
    }
}
