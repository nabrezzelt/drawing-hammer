using HelperLibrary.Networking.ClientServer;
using System;
using System.Net.Sockets;

namespace DrawingHammerServer
{
    public class DrawingHammerServer : Server
    {
        public DrawingHammerServer(int port) : base(port)
        { }

        protected override BaseClientData HandleNewConnectedClient(TcpClient connectedClient)
        {
            return new DrawingHammerClient(this, connectedClient);
        }
    }
}
