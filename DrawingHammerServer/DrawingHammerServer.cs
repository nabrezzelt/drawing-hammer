using DrawingHammerDataLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DrawingHammerServer
{
    class DrawingHammerServer
    {
        const int PORT = 6666;
        private static TcpListener Listener;
        private static List<ClientData> Clients;

        static void Main(string[] args)
        {
            Clients = new List<ClientData>();

            Console.Title = "drawing-hammer | Server";

            StartServer();
        }

        private static void StartServer()
        {
            Console.WriteLine("Starting server on " + Packet.GetThisIPv4Adress());

            Listener = new TcpListener(new IPEndPoint(IPAddress.Parse(Packet.GetThisIPv4Adress()), PORT));

            Console.WriteLine("Server started. Waiting for new Client connections...\n");

            //Thread listenForNewClients = new Thread(ListenForNewClients);
            //listenForNewClients.Start();
        }
    }
}
