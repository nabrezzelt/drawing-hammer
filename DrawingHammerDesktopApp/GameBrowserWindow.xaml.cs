using System;
using System.Windows;
using System.Windows.Threading;
using DrawingHammerDesktopApp.ViewModel;
using DrawingHammerPacketLibrary;
using HelperLibrary.Networking.ClientServer;

namespace DrawingHammerDesktopApp
{
    /// <summary>
    /// Interaktionslogik für GameBrowserWindow.xaml
    /// </summary>
    public partial class GameBrowserWindow : Window
    {
        private readonly SslClient _client;

        public GameBrowserWindow(SslClient client)
        {
            DataContext = new GameBrowserViewModel();

            InitializeComponent();            

            _client = client;

            _client.ConnectionLost += OnConnectionLost;
            _client.PacketReceived += OnPacketReceived;            
        }

        private void OnPacketReceived(object sender, PacketReceivedEventArgs e)
        {
            switch (e.Packet)
            {
                case GameListPacket p:
                    HandleOnGameListReceived(p);
                    break;
                case MatchCreatedPacket:
                    NotifyForCreatedMatch();
                    break;
                case CreateMatchPacket p:
                    AddMatchToList(p.Match);
                    break;
            }
        }

        private void NotifyForCreatedMatch()
        {
            InvokeGui(() =>
            {
                StatusSnackbar.MessageQueue.Enqueue("Match successfully created.");
            });
        }

        private void AddMatchToList(Match match)
        {
            var vm = (GameBrowserViewModel)DataContext;
            vm.Matches.Add(match);
        }

        private void HandleOnGameListReceived(GameListPacket packet)
        {
            //InvokeGui(() =>
            //{
                
            //});

            var vm = (GameBrowserViewModel) DataContext;
            vm.Matches = packet.Matches;
        }

    
        private void OnConnectionLost(object sender, EventArgs e)
        {
            InvokeGui(() =>
            {
               StatusSnackbar.MessageQueue.Enqueue("Connection lost!");
            });           
        }

        private void CreateNewGame(object sender, RoutedEventArgs e)
        {
            CreateNewGameWindow createGameWindow = new CreateNewGameWindow(_client);
            createGameWindow.ShowDialog();
        }

        private void InvokeGui(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, action);
        }
    }
}
