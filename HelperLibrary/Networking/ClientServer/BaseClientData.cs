using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using HelperLibrary.Logging;
using HelperLibrary.Networking.ClientServer.Packets;

namespace HelperLibrary.Networking.ClientServer
{
    public class BaseClientData
    {
        public string Uid;
        public bool Authenticated;
        public readonly TcpClient TcpClient;
        public readonly Stream ClientStream;
        private readonly Thread _clientThread;                
        private Server _serverInstance;

        private readonly ConcurrentQueue<byte[]> _pendingDataToWrite = new ConcurrentQueue<byte[]>();
        private bool _sendingData;

        protected BaseClientData(Server serverInstance, TcpClient client, Stream stream)
        {
            Uid = "";

            _serverInstance = serverInstance;

            TcpClient = client;
            ClientStream = stream;

            //Starte für jeden Client nach dem Verbinden einen seperaten Thread in dem auf neue eingehende Nachrichten gehört/gewartet wird.
            _clientThread = new Thread(_serverInstance.DataIn);
            _clientThread.Start(this);
        }       

        [Obsolete("Use BaseClientData.EnqueueDataForWrite(object packet) instead!")]
        public void SendDataPacketToClient(object packet)
        {
            //var packetBytes = SerializePacket(packet);

            //var length = packetBytes.Length;
            //var lengthBytes = BitConverter.GetBytes(length);
            //ClientStream.Write(lengthBytes, 0, 4); //Senden der Länge/Größe des Textes
            //ClientStream.Write(packetBytes, 0, packetBytes.Length); //Senden der eingentlichen Daten/des Textes    
            
            EnqueueDataForWrite(packet);

            Log.Debug($"Packet of type {packet.GetType().Name} will be send to: {Uid}");
        }

        public void EnqueueDataForWrite(object packet)
        {
            var packetBytes = SerializePacket(packet);

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

        public virtual byte[] SerializePacket(object packet)
        {
            byte[] packetBytes = BasePacket.Serialize(packet);
            return packetBytes;
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
                // maybe handle exception then
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
                // handle exception                
                Log.Debug(ex.ToString());
            }

            WriteData(); 
        }
    }
}