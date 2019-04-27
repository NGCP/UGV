using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGV.Core.Maths;

namespace UGV.Core.Sensors
{
    //This class handles the data from the Nav440
    public class Nav440 : Sensor<GnavPackage>
    {
        #region Public Property
        public double Roll, Pitch, Heading;
        public double Latitude, Longitude, Altitude;
        public double GroundSpeed;
        public bool GoodData = false;


        #endregion Public Property

        string debug;
public override void Update(GnavPackage RawData)
        {
            Roll = RawData.roll;
            Pitch = RawData.pitch;
            Heading = RawData.yaw;
            Longitude = RawData.longitudeGPS;
            Latitude = RawData.latitudeGPS;
            Altitude = RawData.altitudeGPS;
            //Taking the magnitude of the 3 velocity components
            GroundSpeed = Math.Pow(Math.Pow(RawData.nVel,2)+ Math.Pow(RawData.eVel,2)+ Math.Pow(RawData.dVel,2) , .5); 
            if (Latitude == 0 || Longitude == 0)
                GoodData = false;
            else
                GoodData = true;
            

            //time stamp
            this.LastUpdateTime = DateTime.Now;
        }       
    }
    //Raw data collection
    public class GnavPackage : SensorPackage
    {
        public double yaw, roll, pitch;
        public double xRateCorrected;
        public double yRateCorrected;
        public double zRateCorrected;
        public double xAccel;
        public double yAccel;
        public double zAccel;
        public double nVel;
        public double eVel;
        public double dVel;
        public double longitudeGPS;
        public double latitudeGPS;
        public double altitudeGPS;
        public double xRateTemp;
        public double timeitow;
        public double bitstatus;

    }
}

/*
namespace Nav440d
{
    class Nav440D
    {
        byte[] preamble = { 0x55, 0x55 };

        private bool debug;


        public double xRateCorrected { get; private set; }
        public double yRateCorrected { get; private set; }
        public double zRateCorrected { get; private set; }
        public double xAccel { get; private set; }
        public double yAccel { get; private set; }
        public double zAccel { get; private set; }
        public double nVel { get; private set; }
        public double eVel { get; private set; }
        public double dVel { get; private set; }
        public double longitudeGPS { get; private set; }
        public double latitudeGPS { get; private set; }
        public double altitudeGPS { get; private set; }
        public double xRateTemp { get; private set; }
        public double timeitow { get; private set; }
        public double bitstatus { get; private set; }

        void ping()
        {
            byte[] packet = new byte[4];
            packet[0] = preamble[0];
            packet[1] = preamble[1];
            packet[2] = 0x50;
            packet[3] = 0x4B;
            //nav.Send(packet);
        }


        public void getPacketRequest(byte type1, byte type2)
        {
            //sends the get packet request for a type of packet
            byte[] request = new byte[8];
            request[0] = preamble[0];
            request[1] = preamble[1];
            request[2] = 0x47;
            request[3] = 0x50;
            request[4] = type1;
            request[5] = type2;
            byte[] crc = calculateCRC(request);
            request[6] = crc[0];
            request[7] = crc[1];
            //nav.Send(request);
        }

        public void getID()
        {
            getPacketRequest(0x49, 0x44);
        }

        public void getData()
        {
            getPacketRequest(0x4E, 0x30);
            
        }


        byte[] calculateCRC(byte[] packet)
        {
            /*******************************************************************************
            * FUNCTION: calcCRC calculates a 2-byte CRC on serial data using
            Page 112 NAV440 User Manual
            7430‐0131‐01 Rev. F
            * CRC-CCITT 16-bit standard maintained by the ITU
            * (International Telecommunications Union).
            * ARGUMENTS: queue_ptr is pointer to queue holding area to be CRCed
            * startIndex is offset into buffer where to begin CRC calculation
            * num is offset into buffer where to stop CRC calculation
            * RETURNS: 2-byte CRC
            ******************************************************************************

            unsigned short calcCRC(QUEUE_TYPE* queue_ptr, unsigned int startIndex, unsigned int num) {
                unsigned int i = 0, j = 0;
                unsigned short crc = 0x1D0F; //non-augmented inital value equivalent to augmented initial value 0xFFFF
            for (i = 0; i < num; i += 1)
                {
                    crc ^= peekByte(queue_ptr, startIndex + i) << 8;
                    for (j = 0; j < 8; j += 1)
                    {
                        if (crc & 0x8000) crc = (crc << 1) ^ 0x1021;
                        else crc = crc << 1;
                    }
                }
                return crc;
            }

            ushort crc = 0x1D0F;
            for (int i = 2; i < packet.Length - 2; i++)
            {
                crc ^= (ushort)(packet[i] << 8);

                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x8000) == 0x1000)
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    else
                        crc = (ushort)(crc << 1);
                }
            }
            byte[] bytes = BitConverter.GetBytes(crc);

            return bytes;
        }
    }
}

 */
 