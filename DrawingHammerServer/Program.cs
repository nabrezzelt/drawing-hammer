using DrawingHammerPacketLibrary;
using DrawingHammerPacketLibrary.Enums;
using DrawingHammerServer.Exceptions;
using HelperLibrary.Database;
using HelperLibrary.Database.Exceptions;
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
            
            Log.DisplaySelfCertDetails(new X509Certificate2(CertificatePath, CertificatePassword));

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
                Log.Info("");
                Console.Write("DrawingHammer>");
                var input = Console.ReadLine();                
                string[] args = input?.Split(' ');
                string username, password;
                User user;

                switch (input)
                {
                    #region lookup account
                    case var command when command.StartsWith("lookup account "):
                        if (args.Length != 3)
                        {
                            Log.Info("Paramenters are missing!");
                            Log.Info("To search a account write: lookup account [filter]");
                            continue;
                        }
                        var filteredUsers = UserManager.GetUsers(args[2]);
                        Log.Info(filteredUsers.Count + " account(s) found.");
                        Log.Info("");

                        filteredUsers.ForEach(u => Log.Info(u.Id + " " + u.Username + ", IsBanned:" + (u.IsBanned ? "True" : "False")));
                        break;
                    #endregion
                    #region account list
                    case var command when command == "account list":
                        var users = UserManager.GetUsers();
                        Log.Info(users.Count + " account(s) found.");
                        Log.Info("");

                        users.ForEach(u => Log.Info(u.Id + " " + u.Username + ", IsBanned:" + (u.IsBanned ? "True" : "False")));                        
                        break;
                    #endregion
                    #region account delete                    
                    case var command when command.StartsWith("account delete "):
                        if (args.Length != 3)
                        {
                            Log.Info("Paramenter is missing!");
                            Log.Info("To delete a account write: account delete [username]");
                            continue;
                        }

                        user = UserManager.GetUser(args[2]);
                        if (user == null)
                        {
                            Log.Info($"Account with username {args[2]} not found!");
                        }
                        else
                        {
                            UserManager.DeleteUser(user.Id);
                            Log.Info($"Account {user.Username} successfully deleted!");
                        }
                        break;
                    #endregion
                    #region account create
                    case var command when command.StartsWith("account create "):                        
                        if (args.Length != 4)
                        {
                            Log.Info("Paramenters are missing!");
                            Log.Info("To create a new account write: account create [username] [password]");
                            continue;
                        }

                        username = args[2];
                        password = args[3];

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
                        catch (UsernameTooShortException)
                        {
                            Log.Info("Username is too short. The minimum length is " + UserManager.MinUsernameLength);
                        }
                        break;
                    #endregion
                    #region account reset password
                    case var command when command.StartsWith("account reset password "):
                    
                        if (args.Length != 5)
                        {
                            Log.Info("Paramenters are missing!");
                            Log.Info("To create a new account write: account reset password [username] [new_password]");
                            continue;
                        }
                        username = args[3];
                        password = args[4];

                        user = UserManager.GetUser(username);
                        if (user == null)
                        {
                            Log.Info($"Account with username {username} not found!");
                        }
                        else
                        {
                            UserManager.ChangePassword(user.Id, password);
                            Log.Info($"Password of account {user.Username} successfully changed!");
                        }                        
                        break;                    

                    #endregion
                    #region match list
                    case var command when command.StartsWith("match list"):
                        Log.Info("Found " + _matches.Count + " matches:");
                        foreach (var match in _matches)
                        {
                            Log.Info(match.Title);
                        }
                        break;
                    #endregion
                    #region match player status
                    case var command when command.StartsWith("match player status"):                        
                        foreach (var match in _matches)
                        {
                            Log.Info(match.ShowPlayerStatus());
                        }
                        break;
                    #endregion

                    #region help                        
                    case var command when command == "help" || command == "h":
                        Log.Info("Available Commands:");
                        Log.Info("");
                        Log.Info("Command                                          | Explanation");
                        Log.Info("--------------------------------------------------------------");
                        Log.Info("lookup account [filter]                          | Find a account.");
                        Log.Info("account list                                     | List all existing accounts");
                        Log.Info("account delete [username]                        | Delete a existing account");                        
                        Log.Info("account create [username] [password]             | Create a new account");
                        Log.Info("account reset password [username] [new_password] | Reset the password of a account");
                        Log.Info("exit                                             | Shutdown the server");
                        Log.Info("help|h                                           | Shows this helptext");
                        break;
                    #endregion

                    #region exit
                    case var command when command == "exit":
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

            try
            {                
                dbManager.Connect();
            }
            catch (CouldNotConnectException e)
            {
                Log.Fatal(e.InnerException?.Message);
                Log.Info("Press <enter> to exit...");
                Console.ReadLine();
                Environment.Exit(1);
            }
            
            Log.Info("Database connection successfully initialized!");
        }

        private static void OnPacketReceived(object sender, PacketReceivedEventArgs e)
        {
            var packet = e.Packet;
            Log.Debug("Packet recived of type: " + e.Packet.GetType().Name);

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

                case JoinMatchPacket p:
                    HandleOnMatchJoin(p);                    
                    break;

                case RequestMatchDataPacket p:
                    HandleMatchDataRequest(p);
                    break;

                case PickedWordPacket p:
                    HandleOnPickedWord(p);
                    break;                    
            }
        }

        private static void HandleOnPickedWord(PickedWordPacket packet)
        {
            var match = GetMatchByUid(packet.MatchUid);

            Word word = null;            

            foreach (var randomWord in match.RandomWordsToPick)
            {
                if (randomWord.Id == packet.PickedWord.Id)
                {
                    word = packet.PickedWord;
                }
            }

            if (word == null)
            {
                word = match.GetRandomWord();
            }

            match.WordToDraw = word;
            match.PickedWords.Add(word);

            _server.Router.DistributePacket(new WordToDrawPacket(word, Router.ServerWildcard, match.GetCurrentlyPreparingPlayer().Uid));
        }

        private static void HandleMatchDataRequest(RequestMatchDataPacket packet)
        {
            var match = GetMatchByUid(packet.MatchUid);
            DrawingHammerClientData client = (DrawingHammerClientData)_server.GetClientByUid(packet.SenderUid);

            client.SendDataPacketToClient(new MatchDataPacket(new MatchData(match), Router.ServerWildcard, client.Uid));
        }

        private static void HandleOnMatchJoin(JoinMatchPacket packet)
        {
            Match match = GetMatchByUid(packet.MatchUid);
            DrawingHammerClientData client = (DrawingHammerClientData) _server.GetClientByUid(packet.SenderUid);

            if (match.Players.Count < match.MaxPlayers)
            {               
                Player player = new Player(client.User.Id, client.Uid, client.User.Username, 0);
                match.Players.Add(player);
                match.ShowPlayerStatus();

                _server.Router.DistributePacket(new PlayerJoinedMatchPacket(                    
                    match.MatchUid,
                    player,
                    Router.ServerWildcard,
                    Router.AllAuthenticatedWildCard));

                if (match.Players.Count > 1)
                {
                    StartMatch(match);
                }                
            }
            else
            {
                client.SendDataPacketToClient(new MatchJoinFailedPacket("Maximum player count reached!", packet.SenderUid, Router.ServerWildcard));
            }            
        }        

        private static void HandleCreateMatchPacket(CreateMatchPacket packet)
        {
            var client = (DrawingHammerClientData) _server.GetClientByUid(packet.SenderUid);

            Match match =
                new Match(packet.MatchData.Title, packet.MatchData.Rounds, packet.MatchData.MaxPlayers, packet.MatchData.RoundLength)
                {
                    CreatorId = client.User.Id
                };

            match.PreparationTimeStarted += MatchPreparationTimeStarted;
            match.PreparationTimeFinished += MatchPreparationTimeFinished;
            match.SubRoundStarted += Match_SubRoundStarted;
            match.SubRoundFinished += Match_SubRoundFinished;
            match.RoundStarted += Match_RoundStarted;
            match.RoundFinished += Match_RoundFinished;            
            match.MatchFinished += Match_MatchFinished;


            _matches.Add(match);
            Log.Debug("Match created with title: " + match.Title);

            _server.Router.DistributePacket(
                new CreateMatchPacket(new MatchData(match), Router.ServerWildcard, Router.AllAuthenticatedWildCard));
            Log.Debug("All clients notified recording new match.");  
            
            client.SendDataPacketToClient(new MatchCreatedPacket(Router.ServerWildcard, client.Uid));
        }

        private static void Match_RoundStarted(object sender, RoundStartedEventArgs e)
        {
            var match = (Match)sender;

            foreach (Player player in match.Players)
            {
                _server.Router.DistributePacket(new RoundStartedPacket(e.RoundNumber, Router.ServerWildcard, player.Uid));
            }
        }

        private static void MatchPreparationTimeStarted(object sender, PreparationTimerStartedEventArgs e)
        {
            var match = (Match) sender;

            var words = WordManager.GetWord(match.PickedWords);

            if (words.Count < 3)
            {
                match.PickedWords.Clear();
            }

            words = WordManager.GetWord(match.PickedWords);
            match.RandomWordsToPick = new ObservableCollection<Word>(words);            

            _server.Router.DistributePacket(new PickWordsPacket(words, Router.ServerWildcard, e.Player.Uid));

            foreach (Player player in match.Players)
            {
                _server.Router.DistributePacket(new PreparationTimeStartedPacket(e.Player, Router.ServerWildcard, player.Uid));
            }
        }

        private static void Match_SubRoundStarted(object sender, EventArgs e)
        {
            var match = (Match)sender;

            foreach (Player player in match.Players)
            {
                _server.Router.DistributePacket(new SubRoundStartedPacket(Router.ServerWildcard, player.Uid));
            }
        }

        private static void Match_SubRoundFinished(object sender, EventArgs e)
        {
            var match = (Match)sender;
            match.WordToDraw = null;

            foreach (Player player in match.Players)
            {
                _server.Router.DistributePacket(new SubRoundFinishedPacket(Router.ServerWildcard, player.Uid));
            }
        }

        private static void Match_RoundFinished(object sender, EventArgs e)
        {
            var match = (Match) sender;

            foreach (Player player in match.Players)
            {
                _server.Router.DistributePacket(new RoundFinishedPacket(Router.ServerWildcard, player.Uid));                            
            }
        }

        private static void MatchPreparationTimeFinished(object sender, PreparationTimeFinishedEventArgs e)
        {
            var match = (Match) sender;

            if (match.WordToDraw == null)
            {
                var randomWord = match.GetRandomWord();
                match.PickedWords.Add(randomWord);

                _server.Router.DistributePacket(new WordToDrawPacket(randomWord, Router.ServerWildcard, e.PreparingPlayer.Uid));
            }

            foreach (Player player in match.Players)
            {
                _server.Router.DistributePacket(new PreparationTimeFinishedPacket(Router.ServerWildcard, player.Uid));                
            }
        }

        private static void Match_MatchFinished(object sender, EventArgs e)
        {
            var match = (Match) sender;

            foreach (Player player in match.Players)
            {
                _server.Router.DistributePacket(new MatchFinishedPacket(Router.ServerWildcard, player.Uid));
                //ToDo: Notifiy players and show score overview
                Log.Debug("Notifiy players and show score overview");                
            }
        }

        private static void HandleOnGamelistRequest(RequestGamelistPacket packet)
        {
            var matchDataList = new ObservableCollection<MatchData>();
            foreach (var match in _matches)
            {
                matchDataList.Add(new MatchData(match));
            }

            var client  = _server.GetClientByUid(packet.SenderUid);
            client.SendDataPacketToClient(new GameListPacket(matchDataList, Router.ServerWildcard, client.Uid));
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

        private static Match GetMatchByUid(string matchUid)
        {
            foreach (var match in _matches)
            {
                if (match.MatchUid == matchUid)
                    return match;
            }

            return null;
        }

        private static void StartMatch(Match match)
        {
            match.StartMatch();            
        }
    }
}
