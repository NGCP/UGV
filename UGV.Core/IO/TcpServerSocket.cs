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
    /// <summary>
    /// Message Argumens
    /// </summary>
    public class MessageArgs : EventArgs
    {
        /// <summary>
        /// Message body
        /// </summary>
        public string Message;
        /// <summary>
        /// Construct a message argument
        /// </summary>
        /// <param name="Message">Message Body</param>
        public MessageArgs(string Message)
        {
            this.Message = Message;
        }
    }
    /// <summary>
    /// Tcp Server
    /// </summary>
    public class TcpServerSocket : Link
    {
        #region Properties
        /// <summary>
        /// IP address of Tcp Server
        /// </summary>
        public IPAddress ipAddress { get; private set; }
        /// <summary>
        /// IP address of Tcp Server
        /// </summary>
        public int Port { get; private set; }
        /// <summary>
        /// Manual Rest Event for Async Listener
        /// </summary>
        ManualResetEvent allDone = new ManualResetEvent(false);
        /// <summary>
        /// Flag for terminate threads and loop
        /// </summary>
        volatile bool TerminateFlag;
        /// <summary>
        /// Socket if connection
        /// </summary>
        Socket ConnectionSocket;
        /// <summary>
        /// If Tcp is active
        /// </summary>
        bool Active = false;
        #endregion Properties

        #region Construtor

        /// Construct a Tcp Server Socket
        /// </summary>
        /// <param name="Port">Port Number</param>
        public TcpServerSocket(IPAddress ipAddress, int Port)
        {
            this.TerminateFlag = true;
            this.Port = Port;
            this.ipAddress = ipAddress;
        }
        #endregion Construtor

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
        /// Stop a Tcp Server from listening to connection
        /// </summary>
        public override void Stop()
        {
            TerminateFlag = true;
            allDone.Set();
        }
        /// <summary>
        /// Start a connection of Tcp Server Socket
        /// </summary>
        public override void Start()
        {
            //prevent multiple start
            if (!TerminateFlag) return;
            Thread workThread = new Thread(StartWork);
            workThread.Start();
        }
        /// <summary>
        /// Start a connection of Tcp Server Socket
        /// </summary>
        void StartWork()
        {
            //set Terminate flag to be false
            TerminateFlag = false;
            Active = true;
            // Data buffer for incoming data.
            byte[] buffer = new Byte[1024];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Port);
            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);
                while (!TerminateFlag)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();
                    // Start an asynchronous socket to listen for connections.
                    Console.WriteLine("Waiting for a new connection...");
                    Console.WriteLine("IP = " + ipAddress.ToString() + " : " + Port);
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Active = false;
                Console.WriteLine(e.ToString());
            }
            Active = false;
        }
        /// <summary>
        /// Add a message to server queue that will be send to client
        /// </summary>
        /// <param name="Message"></param>
        public override void Send(byte[] Message)
        {
            SendMessage(ConnectionSocket, Encoding.ASCII.GetString(Message));
        }
        #endregion Control Methods

        #region Private Functions
        /// <summary>
        /// Callback function to accpect an incoming connection
        /// </summary>
        /// <param name="ar"></param>
        void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();
            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            ConnectionSocket = listener.EndAccept(ar);
            // Create the state object.
            TcpStateObject state = new TcpStateObject();
            state.workSocket = ConnectionSocket;
            ConnectionSocket.BeginReceive(state.buffer, 0, TcpStateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }
        /// <summary>
        /// Read a Callback send from client
        /// </summary>
        /// <param name="ar"></param>
        void ReadCallback(IAsyncResult ar)
        {
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            TcpStateObject state = (TcpStateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(ar);

            Console.WriteLine("Callback read on server: " + bytesRead);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                //flag and trigger the package event

                // All the data has been read from the client. 
                //trigger new message event
                if (PackageReceived != null)
                    PackageReceived(Encoding.ASCII.GetBytes(state.sb.ToString()));
                //keep read is allowed so
                if (!TerminateFlag)
                {
                    //establish new message and read more
                    TcpStateObject newState = new TcpStateObject();
                    newState.workSocket = handler;
                    try
                    {
                        handler.BeginReceive(newState.buffer, 0, TcpStateObject.BufferSize, 0,
                            new AsyncCallback(ReadCallback), newState);
                    }
                    catch (SocketException)
                    { }
                }


            }
        }
        /// <summary>
        /// Send a message back to client
        /// </summary>
        /// <param name="handler">Sockect object</param>
        /// <param name="data">data content</param>
        void SendMessage(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }
        /// <summary>
        /// Async send a callback to client
        /// </summary>
        /// <param name="ar"></param>
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;
                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                //keep read is allowed so
                if (!TerminateFlag)
                {
                    //establish new message and read more
                    TcpStateObject newState = new TcpStateObject();
                    newState.workSocket = handler;
                    handler.BeginReceive(newState.buffer, 0, TcpStateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), newState);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
        #endregion Private Functions
    }

    // State object for reading client data asynchronously
    public class TcpStateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }
}
