using System.IO;
using System.Net.Sockets;
using HelperLibrary.Networking.ClientServer;

namespace DrawingHammerServer
{
    public class DrawingHammerClientData : BaseClientData
    {
        public int Score;

        public DrawingHammerClientData(int score, Server serverInstance, TcpClient client, Stream stream) : base(serverInstance, client, stream)
        {
            Score = score;
        }

        public DrawingHammerClientData(Server serverInstance, TcpClient client, Stream stream) : base(serverInstance, client, stream)
        {
            Score = 0;
        }
    }
}