using DrawingHammerDataLib.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DrawingHammerServer
{
    class ClientData
    {
        public TcpClient TcpClient { get; private set; }
        
        private Thread ClientThread;
        private NetworkStream ClientStream;
        
        public bool Registered;
        public string Uid;
     
        public ClientData(TcpClient client)
        {
            Uid = "";
            Registered = false;

            this.TcpClient = client;
            this.ClientStream = client.GetStream();

            //Starte für jeden Client nach dem Verbinden einen seperaten Thread in dem auf neue eingehende Nachrichten gehört/gewartet wird.
            ClientThread = new Thread(DrawingHammerServer.DataIn);
            ClientThread.Start(
                );           
        }        

        public void SendDataPacketToClient(object packet)
        {
            byte[] packetBytes = BasePacket.Serialize(packet);

            var length = packetBytes.Length;
            var lengthBytes = BitConverter.GetBytes(length);
            ClientStream.Write(lengthBytes, 0, 4); //Senden der Länge/Größe des Textes
            ClientStream.Write(packetBytes, 0, packetBytes.Length); //Senden der eingentlichen Daten/des Textes                                
}
    }
}
