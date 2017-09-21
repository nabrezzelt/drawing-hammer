using DrawingHammerDataLib;
using DrawingHammerDataLib.Packets;
using HelperLibrary.Networking;
using System;
using System.Collections.Generic;
using System.IO;
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
        public const int PORT = 6666;
        public const string SERVER_UID = BasePacket.SERVER;

        private static TcpListener Listener { get; private set; }      
        public static List<ClientData> Clients { get; private set; }


        static void Main(string[] args)
        {
            Console.Title = "drawing-hammer | Server";

            Clients = new List<ClientData>();

            StartServer();
            StartCommandLineHandler(); 
        }

        private static void StartServer()
        {
            Log.INFO("Starting server on " + Network.GetThisIPv4Adress());

            Listener = new TcpListener(new IPEndPoint(IPAddress.Parse(Network.GetThisIPv4Adress()), PORT));

            Log.INFO("Server started. Waiting for new Client connections...");

            Thread listenForNewClients = new Thread(ListenForNewClients);
            listenForNewClients.Start();
        }

        private static void ListenForNewClients()
        {
            Listener.Start();

            while (true)
            {
                TcpClient connectedClient = Listener.AcceptTcpClient();

                Clients.Add(new ClientData(connectedClient));
                Log.INFO("New Client connected (IP: " + connectedClient.Client.RemoteEndPoint.ToString() + ")");
                //ToDo: Eventhandler erstellen wenn Client connected!
            }
        }

        public static void DataIn(object tcpClient)
        {
            TcpClient client = (TcpClient)tcpClient;
            NetworkStream clientStream = client.GetStream();
            try
            {
                while (true)
                {
                    byte[] buffer; //Daten
                    byte[] dataSize = new byte[4]; //Länge

                    int readBytes = clientStream.Read(dataSize, 0, 4);

                    while (readBytes != 4)
                    {
                        readBytes += clientStream.Read(dataSize, readBytes, 4 - readBytes);
                    }
                    var contentLength = BitConverter.ToInt32(dataSize, 0);

                    buffer = new byte[contentLength];
                    readBytes = 0;
                    while (readBytes != buffer.Length)
                    {
                        readBytes += clientStream.Read(buffer, readBytes, buffer.Length - readBytes);
                    }

                    //Daten sind im Buffer-Array
                    Router.DistributePacket((BasePacket)BasePacket.Deserialize(buffer), client);
                }
            }
            catch (ObjectDisposedException)
            {
                //Connection lost/closed to client.
                return;
            }
            catch (IOException)
            {
                ClientData disconnectedClient = Router.GetClientFromList(client);
                Log.INFO("Client disconnected with UID: " + disconnectedClient.UID);
                Clients.Remove(disconnectedClient);
                Log.INFO("Client removed from list.");

                //ToDo: Eventhandler erstellen wenn Client disconnected!
                //NotifyForDisconnectedOutputClient(disconnectedClient.UID);
            }
        }

        private static void StartCommandLineHandler()
        {
            while (true)
            {
                Console.Write("PiServer>");
                string input = Console.ReadLine();
                switch (input)
                {                    
                    case "h":
                    case "?":
                    case "help":                        
                        Log.INFO("exit                - Shutdown the server");
                        Log.INFO("");
                        Log.INFO("h|?|help            - For this commandlist.");
                        break;

                    case "exit":
                        Log.WARN("Going down for shutdown now...!");
                        Environment.Exit(0);
                        break;
                    default:
                        if (input != String.Empty)
                            Log.WARN("'" + input + "' not fount. Try 'help' to get a list of all available commands.");
                        break;
                }
            }
        }
    }
}
