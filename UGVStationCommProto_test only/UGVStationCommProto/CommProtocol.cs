using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NGCP;

namespace NGCP.UGV
{
    public class CommProtocol
    {
        private static float increment = 0;

        #region Public Properties

        ///<summary>
        /// Link call backs
        ///</summary>
        ///
        public bool LinkCallback(Comnet.ABSPacket packet, Comnet.CallBack callback)
        {
            //link the call backs to the CommProtocol library node
            return Node.LinkCallback(packet,callback);
        }

        /// <summary>
        /// Local Node id
        /// </summary>
        public int NodeID { get; private set; }


        #endregion Public Properties

        #region Private Properties

        /// <summary>
        /// Node object of Commprotocol
        /// </summary>
        Comnet.Comms Node;

        #endregion Private Properties

        #region Constructor

        /// <summary>
        /// Constructor of protonet
        /// </summary>
        /// <param name="NodeID"></param>
        public CommProtocol(int NodeID)
        {
            //node id of protonet
            this.NodeID = NodeID;
            //Create a protonet node at specified node id
            Node = new Comnet.Comms((byte)NodeID);
            //load encryption key from text or file
            //Node.LoadKey("NGCP Project 2016");
            //Node.LoadKeyFromFile("fileName.txt");

                       
        }

        #endregion Constructor

        #region Public Method

        /// <summary>
        /// Init, Add connection, and Send
        /// </summary>
        

        //Initalize will connect to xbee and if connection is success returns true
        public bool initializeConnection( String PortName, int Baud) 
        {
            try {         
               return Node.InitConnection(Comnet.Network.TransportProtocol.ZIGBEE_LINK, PortName,"", (uint)Baud);
            }
            catch (Exception e)
            {
                Console.WriteLine("Some thing went wrong with init the xbee");    
                Console.WriteLine(e.GetType());
                return false;
            }                                 
        }

        /// <summary>
        /// Add a pair of an address id and the actual address
        /// </summary>
        public bool addAddress(int id, String address)
        {
            try
            {
                return Node.AddAddress((byte)id, address, 0);
            }
            catch (Exception e)
            {
                Console.WriteLine("Some thing went wrong with adding one of the xbee address");
                Console.WriteLine(e.GetType());
                return false;
            }

        }

        /// <summary>
        /// Send a state
        /// </summary>
        /// <param name="state"></param>
        public void SendState(UGVState state, byte id = 1)
        {
            double timestamp = (DateTime.Now - UnixTime).TotalMilliseconds;
            increment += 0.0001f;
            //id, longitude, latitude, altitude, x_speed, y_speed, z_speed
            //VehicleGlobalPosition vgp = new NGCP.VehicleGlobalPosition((byte)NodeID, (float)state.Longitude, (float)state.Latitude,0);
            VehicleGlobalPosition vgp = new NGCP.VehicleGlobalPosition((byte)7, (float)-117.0f+increment, (float)34.0f+increment, 0);

            Node.Send(vgp, id);
        }

        public void sendMode(UGV.DriveMode mode, byte id = 1)
        {
            NGCP.VehicleSystemStatus vss = new NGCP.VehicleSystemStatus((byte)NodeID, (byte)mode);
            Node.Send(vss, id);            
        }

        public void sendDriveState(UGV.DriveState state, byte id = 1)
        {
            NGCP.VehicleSystemStatus vss = new NGCP.VehicleSystemStatus((byte)NodeID, 0, (byte)state);
            Node.Send(vss, id);
            
        }

        public void SendArmPosition(int position1, int position2, int position3, int position4, byte id = 1)
        {
            NGCP.ArmPosition ap = new NGCP.ArmPosition(position1, position2, position3, position4);
            Node.Send(ap, id);
        }

        public void start()
        {
            Node.Run();
        }

        static DateTime UnixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion Public Method
    }
}
