using DrawingHammerPacketLibrary;
using DrawingHammerPacketLibrary.Enums;
using DrawingHammerServer.Exceptions;
using HelperLibrary.Database;
using HelperLibrary.Logging;
using HelperLibrary.Networking;
using HelperLibrary.Networking.ClientServer;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Sockets;
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
        private static AuthenticationManager _authenticationManager;

        private static ObservableCollection<Match> _matches;

        private static void Main(string[] args)
        {
            _settingsManager = new SettingsManager(SettingsPath);

            InitializeSettingsFile();            

            InitializeDatabaseConnection();            
            Log.Info("");

            string ip = _settingsManager.GetStartupIp() != "" ? _settingsManager.GetStartupIp() : NetworkUtilities.GetThisIPv4Adress();

            _server = new DrawingHammerServer(new X509Certificate2(CertificatePath, CertificatePassword), ip, 9999);
            
            _server.ClientConnected += OnClientConnected;
            _server.ClientDisconnected += OnClientDisconnected;
            _server.PacketReceived += OnPacketReceived;
            _server.Start();

            _authenticationManager = new AuthenticationManager();

            _matches = new ObservableCollection<Match>();

            StartCommandLineHandler();
        }

        private static void InitializeSettingsFile()
        {
            if (File.Exists(SettingsPath))
                return;

            _settingsManager.InitializeSettingsFile();
            Log.Info(".ini-File initialized. Please setup your database login!");
            Console.ReadLine();
            Environment.Exit(0);
        }

        private static void StartCommandLineHandler()
        {
            while (true)
            {
                Console.WriteLine();
                Console.Write("DrawingHammer>");
                var input = Console.ReadLine();                
                string[] args = input?.Split(' ');

                switch (input)
                {
                    #region lookup account
                    case string command when command.StartsWith("lookup account "):

                        break;
                    #endregion
                    #region account list
                    case string command when command.StartsWith("account list "):
                        break;
                    #endregion
                    #region account delete                    
                    case string command when command.StartsWith("account delete "):
                        break;
                    #endregion
                    #region account create
                    case string command when command.StartsWith("account create "):                        
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
                        break;
                    #endregion
                    #region account reset password
                    case string command when command.StartsWith("account reset password "):
                        if (args.Length != 5)
                        {
                            Log.Info("Paramenters are missing!");
                            Log.Info("To create a new account write: account reset password [username] [new_password]");
                            continue;
                        }
                        break;
                    #endregion
                    #region match list
                    case string command when command.StartsWith("match list"):
                        Log.Info("Found " + _matches.Count + " matches:");
                        foreach (var match in _matches)
                        {
                            Log.Info(match.Title);
                        }
                        break;
                    #endregion
                    #region help                        
                    case string command when command == "help" || command == "h":
                        Log.Info("Available Commands:");
                        Log.Info("");
                        Log.Info("Command                                          | Explanation");
                        Log.Info("--------------------------------------------------------------");
                        Log.Info("lookup account [username]                        | Find a account.");
                        Log.Info("account list [username]                          | List all existing accounts");
                        Log.Info("account delete [username]                        | Delete a existing account");                        
                        Log.Info("account create [username] [password]             | Create a new account");
                        Log.Info("account reset password [username] [new_password] | Reset the password of a account");
                        Log.Info("exit                                             | Shutdown the server");
                        Log.Info("help|h                                           | Shows this helptext");
                        break;
                    #endregion

                    #region exit
                    case string command when command == "exit":
                        Log.Error("Shutting down the server...");
                        Environment.Exit(0);
                        break;
                    #endregion
                    default:
                        if (input != "")
                        {
                            Log.Info("'" + input + "' is no command!");
                        }
                        break;
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

                case RegistrationPacket p:
                    HandleOnRegistrationRequest(p, e.SenderTcpClient);
                    break;

                case RequestGamelistPacket p:
                    HandleOnGamelistRequest(p);
                    break;

                case CreateMatchPacket p:
                    HandleCreateMatchPacket(p);
                    break;
            }
        }

        private static void HandleCreateMatchPacket(CreateMatchPacket packet)
        {
            var client = (DrawingHammerClientData) _server.GetClientByUid(packet.SenderUid);

            Match match = packet.Match;
            match.CreatorId = client.User.Id;

            _matches.Add(match);
            Log.Debug("Match created with title: " + match.Title);

            _server.Router.DistributePacket(
                new CreateMatchPacket(match, Router.ServerWildcard, Router.AllAuthenticatedWildCard),
                new[] {client.Uid});
            Log.Debug("All clients notified recording new match.");

            client.SendDataPacketToClient(new MatchCreatedPacket(match, Router.ServerWildcard, client.Uid));
            Log.Debug("User notified for own created match.");
        }

        private static void HandleOnGamelistRequest(RequestGamelistPacket packet)
        {
            var client  = _server.GetClientByUid(packet.SenderUid);
            client.SendDataPacketToClient(new GameListPacket(_matches, Router.ServerWildcard, client.Uid));
        }

        private static void HandleOnRegistrationRequest(RegistrationPacket packet, TcpClient senderTcpClient)
        {
            var client = _server.GetClientByTcpClient(senderTcpClient);
            client.Uid = packet.SenderUid;

            try
            {               
                UserManager.CreateUser(packet.Username, packet.Password);
                Log.Info("User successfull created with username: " + packet.Username);

                client.SendDataPacketToClient(new RegistrationResultPacket(RegistrationResult.Ok, Router.ServerWildcard, packet.SenderUid));
            }
            catch (UserAlreadyExitsException)
            {
                Log.Info("Registration failed. User with username '" + packet.Username + "' already exitsts");
                client.SendDataPacketToClient(new RegistrationResultPacket(RegistrationResult.UsernameAlreadyExists, Router.ServerWildcard, packet.SenderUid));
            }
            catch (UsernameTooLongException)
            {
                Log.Info("Registration failed. User '" + packet.Username + "' is too long.");
                client.SendDataPacketToClient(new RegistrationResultPacket(RegistrationResult.UsernameTooLong, Router.ServerWildcard, packet.SenderUid));
            }
            catch (UsernameTooShortException)
            {
                Log.Info("Registration failed. User '" + packet.Username + "' is too short.");
                client.SendDataPacketToClient(new RegistrationResultPacket(RegistrationResult.UsernameTooShort, Router.ServerWildcard, packet.SenderUid));
            }            
        }

        private static void HandleOnAuthenticationRequest(AuthenticationPacket packet, TcpClient senderTcpClient)
        {
            var client = (DrawingHammerClientData) _server.GetClientByTcpClient(senderTcpClient);
            client.Uid = packet.SenderUid;

            var authResult = _authenticationManager.IsValid(packet.Username, packet.Password);

            if (authResult.Result)
            {
                Log.Info("Authenticationcredentials are valid!");
                client.Authenticated = true;
                client.User = authResult.User;
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
