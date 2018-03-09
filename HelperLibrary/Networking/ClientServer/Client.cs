using HelperLibrary.Logging;
using HelperLibrary.Networking.ClientServer.Packets;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace HelperLibrary.Networking.ClientServer
{
    public class Client
    {
        /// <summary>
        /// Fired when connection to server succeed.
        /// </summary>
        public event EventHandler ConnectionSucceed;

        /// <summary>
        /// Fired when connection to server is lost.
        /// </summary>
        public event EventHandler ConnectionLost;  
        
        /// <summary>
        /// Fired when client receives a packet.
        /// </summary>
        public event EventHandler<PacketReceivedEventArgs> PacketReceived;

        public bool IsConnected { get; set; }

        protected IPAddress ServerIp;
        protected int Port;
        protected TcpClient TcpClient;
        protected Stream ClientStream;

        private readonly ConcurrentQueue<byte[]> _pendingDataToWrite = new ConcurrentQueue<byte[]>();
        private bool _sendingData;
        /// <summary>
        /// Initialize the connection to the server.
        /// </summary>
        /// <param name="serverIP">Server IP</param>
        /// <param name="port">Port to connect</param>
        public void Connect(IPAddress serverIP, int port)
        {
            ServerIp = serverIP;
            Port = port;

            ConnectToServer();
            StartReceivingData();
            ConnectionSucceed?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc cref="Connect(IPAddress, int)"/>
        public void Connect(string serverIp, int port)
        {
            Connect(IPAddress.Parse(serverIp), port);
        }

        protected virtual void ConnectToServer()
        {
            TcpClient = new TcpClient();

            while (!TcpClient.Connected)
            {
                try
                {
                    Log.Info("Trying to connect to server at " + ServerIp + " on port " + Port + "...");

                    TcpClient.Connect(new IPEndPoint(ServerIp, Port));
                    ClientStream = TcpClient.GetStream();

                    IsConnected = true;
                    Log.Info("Connected");
                }
                catch (Exception e)
                {
                    Log.Error(e.Message + Environment.NewLine);                                        
                }                
            }
        }        

        private void StartReceivingData()
        {
            Log.Info("Starting incomming data handler...");
            Thread receiveDataThread = new Thread(HandleIncommingData);
            receiveDataThread.Start();
            Log.Info("Incomming data handler started.");
        }

        private void HandleIncommingData()
        {            
            try
            {
                while (true)
                {
                    byte[] buffer; //Daten
                    byte[] dataSize = new byte[4]; //Länge

                    int readBytes = ClientStream.Read(dataSize, 0, 4);

                    while (readBytes != 4)
                    {
                        readBytes += ClientStream.Read(dataSize, readBytes, 4 - readBytes);
                    }
                    var contentLength = BitConverter.ToInt32(dataSize, 0);

                    buffer = new byte[contentLength];
                    readBytes = 0;
                    while (readBytes != buffer.Length)
                    {
                        readBytes += ClientStream.Read(buffer, readBytes, buffer.Length - readBytes);
                    }

                    //Daten sind im Buffer-Array gespeichert
                    PacketReceived?.Invoke(this,
                        new PacketReceivedEventArgs(BasePacket.Deserialize(buffer), TcpClient));
                }
            }
            catch (IOException ex)
            {
                Log.Info(ex.Message);
                Log.Info("Server connection lost!");
                ConnectionLost?.Invoke(this, EventArgs.Empty);
            }
        }        

        /// <summary>
        /// Close the connection to the server.
        /// </summary>
        public void Disconnect()
        {
            if (!TcpClient.Connected)
                throw new InvalidOperationException("You're not connected!");

            ClientStream.Close();
            TcpClient.Close();
            Log.Info("Disconnected!");
        }

        /// <summary>
        /// Send a package to the server.
        /// </summary>
        /// <param name="packet">Packet to send</param>
        [Obsolete("Use EnqueueDataForWrite(BasePackage package) instead!")]
        public void SendPacketToServer(BasePacket packet)
        {
            EnqueueDataForWrite(packet);
        }
        /// <summary>
        /// Enqueue the packet in the message queue to send this to the server.
        /// </summary>
        /// <param name="packet">Package to send (must inherit <see cref="BasePacket"/>).</param>
        public void EnqueueDataForWrite(BasePacket packet)
        {
            if (!TcpClient.Connected)
                throw new InvalidOperationException("You're not connected!");

            var packetBytes = BasePacket.Serialize(packet);

            var length = packetBytes.Length;
            var lengthBytes = BitConverter.GetBytes(length);

            _pendingDataToWrite.Enqueue(lengthBytes);
            _pendingDataToWrite.Enqueue(packetBytes);

            lock (_pendingDataToWrite)
            {
                if (_sendingData)
                {
                    return;
                }

                _sendingData = true;
            }

            WriteData();
        }

        private void WriteData()
        {
            byte[] buffer;

            try
            {
                if (_pendingDataToWrite.Count > 0 && _pendingDataToWrite.TryDequeue(out buffer))
                {
                    ClientStream.BeginWrite(buffer, 0, buffer.Length, WriteCallback, ClientStream);
                }
                else
                {
                    lock (_pendingDataToWrite)
                    {
                        _sendingData = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                lock (_pendingDataToWrite)
                {
                    _sendingData = false;
                }
            }
        }

        private void WriteCallback(IAsyncResult ar)
        {
            try
            {
                ClientStream.EndWrite(ar);
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }

            WriteData();
        }
    }
}
