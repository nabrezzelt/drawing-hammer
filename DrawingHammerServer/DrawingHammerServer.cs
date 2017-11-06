using HelperLibrary.Networking.ClientServer;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace DrawingHammerServer
{
    public class DrawingHammerServer : SslServer
    {
        public DrawingHammerServer(X509Certificate2 certificate, string ip, int port) : base(certificate, ip, port) { }

        public override BaseClientData HandleNewConnectedClient(TcpClient connectedClient, Stream stream)
        {
            //Um eigene Werte hinzuzufügen (Score) wir die Standard Methode überschrieben
            return new DrawingHammerClientData(this, connectedClient, stream);
        }
    }
}
