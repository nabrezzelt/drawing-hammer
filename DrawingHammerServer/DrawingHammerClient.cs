using HelperLibrary.Networking.ClientServer;
using System.Net.Sockets;

namespace DrawingHammerServer
{
    class DrawingHammerClient : BaseClientData
    {
        public int Score;

        public DrawingHammerClient(int score,Server serverInstance, TcpClient client) : base(serverInstance,client)
        {

        }

        public DrawingHammerClient(Server serverInstance, TcpClient client) : base(serverInstance, client)
        {

        }
    }


}
