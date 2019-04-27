using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UGV.Core.IO
{
    public class TcpClientSocket : Link
    {
        #region Properties

        /// <summary>
        /// IP address of Tcp Client
        /// </summary>
        public IPAddress IpAddress { get; private set; }
        /// <summary>
        /// IP address of Tcp Client
        /// </summary>
        public int Port { get; private set; }
        /// <summary>
        /// ManualResetEvent instances signal completion for terminate
        /// </summary>
        ManualResetEvent TerminateFlag = new ManualResetEvent(false);
        /// <summary>
        /// Socket if connection
        /// </summary>
        Socket ConnectionSocket;
        /// <summary>
        /// If Tcp is active
        /// </summary>
        bool Active = false;
        #endregion Properties

        #region Contructor
        /// <summary>
        /// Construct a Tcp Client Socket
        /// </summary>
        /// <param name="Port">Port Number</param>
        public TcpClientSocket(IPAddress IpAddress, int Port)
        {
            this.TerminateFlag.Reset();
            this.IpAddress = IpAddress;
            this.Port = Port;
        }
        #endregion Contructor

        #region Control Methods
        /// <summary>
        /// If TCP is active
        /// </summary>
        /// <returns></returns>
        public override bool IsActive()
        {
            return Active;
        }
        /// <summary>
        /// Stop a Tcp Client from listening to connection
        /// </summary>
        public override void Stop()
        {
            TerminateFlag.Set();
        }
        /// <summary>
        /// Start a thread of
        /// </summary>
        public override void Start()
        {
            Thread workThread = new Thread(StartWork);
            workThread.Start();
        }
        /// <summary>
        /// Start a connection of Tcp Client Socket
        /// </summary>
        void StartWork()
        {
            //set Terminate flag
            TerminateFlag.Reset();
            Active = true;
            // Connect to a remote device.
            try
            {
                // Find out the local endpoint for the socket.
                //IPHostEntry ipHostInfo = Dns.Resolve(Host);
                IPAddress ipAddress = IpAddress;// ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, Port);
                // Create a TCP/IP socket.
                ConnectionSocket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);
                // Connect to the remote endpoint.
                ConnectionSocket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), ConnectionSocket);
                //wait until stop is called
                TerminateFlag.WaitOne();
                // Release the socket.
                ConnectionSocket.Shutdown(SocketShutdown.Both);
                ConnectionSocket.Close();
                Active = false;
            }
            catch (Exception e)
            {
                Active = false;
                Console.WriteLine(e.ToString());
            }
        }
        /// <summary>
        /// Add a message to server queue that will be send to server
        /// </summary>
        /// <param name="Message"></param>
        public override void Send(byte[] Message)
        {
            SendMessage(ConnectionSocket, Encoding.ASCII.GetString(Message));
        }
        #endregion Control Methods

        #region Private Functions
        /// <summary>
        /// Called when a new server is connected to this client
        /// </summary>
        /// <param name="ar"></param>
        void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;
                // Complete the connection.
                client.EndConnect(ar);
                // Create the state object.
                TcpStateObject state = new TcpStateObject();
                state.workSocket = client;
                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, TcpStateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Read a Callback send from client
        /// </summary>
        /// <param name="ar"></param>
        void ReadCallback(IAsyncResult ar)
        {
            try
            {
                String content = String.Empty;
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                TcpStateObject state = (TcpStateObject)ar.AsyncState;
                Socket client = state.workSocket;
                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);
                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    // Check for end-of-file tag. If it is not there, read 
                    // more data.
                    content = state.sb.ToString();
                    // All the data has been read from the client. 
                    //trigger new message event
                    if (PackageReceived != null)
                        PackageReceived(Encoding.ASCII.GetBytes(content));
                    //establish new message and read more
                    TcpStateObject newState = new TcpStateObject();
                    newState.workSocket = client;
                    client.BeginReceive(state.buffer, 0, TcpStateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void SendMessage(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            // Begin sending the data to the remote device.
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;
                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        #endregion Private Functions
    }
}
