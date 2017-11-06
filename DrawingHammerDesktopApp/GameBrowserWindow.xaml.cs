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
            InitializeComponent();

            DataContext = new GameBrowserViewModel();

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
            }
        }

        private void HandleOnGameListReceived(GameListPacket packet)
        {
            //InvokeGui(() =>
            //{
                
            //});


        }

        private void OnConnectionLost(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CreateNewGame(object sender, RoutedEventArgs e)
        {
            CreateNewGameWindow createGameWindow = new CreateNewGameWindow();
            createGameWindow.ShowDialog();
        }

        private void InvokeGui(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, action);
        }
    }
}
