using DrawingHammerServer.Exceptions;
using HelperLibrary.Database;
using HelperLibrary.Logging;
using HelperLibrary.Networking.ClientServer;
using HelperLibrary.Networking.ClientServer.Packets;
using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using DrawingHammerPacketLibrary;

namespace DrawingHammerServer
{
    class Program
    {       
        private const string SettingsPath = "settings.ini";
        private const string CertificatePath = "certificate.pfx";
        private const string CertificatePassword = "password";

        private static DrawingHammerServer _server;        
        private static SettingsManager _settingsManager;
        private static AuthenticationManager _authenticationManager;

        private static void Main(string[] args)
        {
            CheckIfSettingFileExists();

            _settingsManager = new SettingsManager(SettingsPath);

            InitializeDatabaseConnection();            
            Log.Info("");
            
            _server = new DrawingHammerServer(new X509Certificate2(CertificatePath, CertificatePassword), 9999);
            
            _server.ClientConnected += OnClientConnected;
            _server.ClientDisconnected += OnClientDisconnected;
            _server.PacketReceived += OnPacketReceived;
            _server.Start();

            _authenticationManager = new AuthenticationManager();

            StartCommandLineHandler();
        }

        private static void CheckIfSettingFileExists()
        {
            if (!File.Exists(SettingsPath))
            {
                _settingsManager.InitializeSettingsFile();
                Log.Info(".ini-File initialized. Please set your database connection credentials!");
                Environment.Exit(0);
            }
        }

        private static void StartCommandLineHandler()
        {
            while (true)
            {
                Console.WriteLine();
                Console.Write("DrawingHammer>");
                var input = Console.ReadLine();

                // ReSharper disable once PossibleNullReferenceException
                if (input.StartsWith("account create "))
                {
                    string[] args = input.Split(' ');

                    if (args.Length != 4)
                    {
                        Log.Info("Paramenters are missing!");
                        Log.Info("To create a new account write: account create [username] [password]");
                        continue;                        
                    }

                    string username = args[2];
                    string password = args[3];

                    try
                    {
                        UserManager.CreateUser(username, password);
                        Log.Info("Account '" + username.ToLower() + "' created!");
                    }
                    catch (UserAlreadyExitsException)
                    {
                        Log.Info("User with this username '" + username.ToLower() + "' already exits!");
                    }
                    catch (UsernameTooLongException)
                    {
                        Log.Info("Username is too long. The maxium length is " + UserManager.MaxUsernameLength);
                    }
                }
                else if (input == "help" || input == "h")
                {
                    Log.Info("Available Commands:");
                    Log.Info("");
                    Log.Info("Command        | Explanation");
                    Log.Info("account create   Create a new Account");
                    Log.Info("help|h           Shows this helptext");
                }
                else
                {
                    Log.Info("'" + input + "' is no command!");
                }
            }
        }

        private static void InitializeDatabaseConnection()
        {
            MySQLDatabaseManager dbManager = MySQLDatabaseManager.GetInstance();
            dbManager.SetConnectionString(
                _settingsManager.GetDatabaseHost(),
                _settingsManager.GetDatabaseUser(),
                _settingsManager.GetDatabasePassword(),
                _settingsManager.GetDatabasename());
            dbManager.Connect();
            Log.Info("Database connection successfully initialized!");
        }

        private static void OnPacketReceived(object sender, PacketReceivedEventArgs e)
        {
            var packet = e.Packet;
            Log.Debug("Packet recived of type: " + nameof(e.Packet));

            switch (packet)
            {
                case AuthenticationPacket p:
                    HandleOnAuthenticationRequest(p, e.SenderTcpClient);
                    break;

                case BasePacket p:                    
                    break;                
            }
        }

        private static void HandleOnAuthenticationRequest(AuthenticationPacket packet, TcpClient senderTcpClient)
        {
            var client = _server.GetClientByTcpClient(senderTcpClient);
            client.Uid = packet.SenderUid;

            if (_authenticationManager.IsValid(packet.Username, packet.Password))
            {
                Log.Info("Authenticationcredentials are valid!");
                client.SendDataPacketToClient(new AuthenticationResultPacket(AuthenticationResult.Ok, Router.ServerWildcard, packet.SenderUid));
            }
            else
            {
                Log.Info("Authenticationcredentials are not valid!");
                client.SendDataPacketToClient(new AuthenticationResultPacket(AuthenticationResult.Failed, Router.ServerWildcard, packet.SenderUid));
            }                
        }

        private static void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Console.WriteLine("Client disconnected with Uid: {0}", e.ClientUid);
            //ToDo: Von allen Matches usw. entfernen und an die anderen Clients syncen.
        }

        private static void OnClientConnected(object sender, ClientConnectedEventArgs e)
        {
            Console.WriteLine("Client connected from IP: {0}", e.Client.TcpClient.Client.RemoteEndPoint);
        }
    }
}
