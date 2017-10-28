using DrawingHammerServer.Exceptions;
using HelperLibrary.Database;
using HelperLibrary.Logging;
using HelperLibrary.Networking.ClientServer;
using HelperLibrary.Networking.ClientServer.Packets;
using System;
using System.Security.Cryptography.X509Certificates;

namespace DrawingHammerServer
{
    class Program
    {       
        private const string SettingsPath = "settings.ini";
        private const string CertificatePath = "certificate.pfx";
        private const string CertificatePassword = "password";

        private static DrawingHammerServer _server;        
        private static SettingsManager _settingsManager;

        private static void Main(string[] args)
        {
            _settingsManager = new SettingsManager(SettingsPath);

            InitializeDatabaseConnection();
            Log.Info("");
            
            _server = new DrawingHammerServer(new X509Certificate2(CertificatePath, CertificatePassword), 9999);
            
            _server.ClientConnected += OnClientConnected;
            _server.ClientDisconnected += OnClientDisconnected;
            _server.PacketReceived += OnPacketReceived;
            
            StartCommandLineHandler();
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
                case BasePacket p:
                    
                    break;                
            }
        }

        private static void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            
        }

        private static void OnClientConnected(object sender, ClientConnectedEventArgs e)
        {
            
        }
    }
}
