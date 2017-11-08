using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DrawingHammerPacketLibrary;
using HelperLibrary.Networking.ClientServer;

namespace DrawingHammerDesktopApp
{
    /// <summary>
    /// Interaktionslogik für CreateNewGameWindow.xaml
    /// </summary>
    public partial class CreateNewGameWindow : Window
    {
        private readonly SslClient _client;

        public CreateNewGameWindow(SslClient client)
        {
            InitializeComponent();
            _client = client; 
            _client.ConnectionLost += OnConnectionLost;
            _client.PacketReceived += OnPacketReceived;
        }

        private void OnPacketReceived(object sender, PacketReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnConnectionLost(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CheckToEnableCreateButton(object sender, TextChangedEventArgs e)
        {
            btnCreate.IsEnabled = tbGameName.Text != "";
        }

        private void Create(object sender, RoutedEventArgs e)
        {
            var match = new Match(tbGameName.Text, Convert.ToInt32(sRounds.Value), Convert.ToInt32(sPlayers.Value), Convert.ToInt32(sRoundlength.Value));
            SendCreateMassage(match);

        }

        private async void SendCreateMassage(Match match)
        {
            await Task.Run(() =>
            {
                _client.SendPacketToServer(new CreateMatchPacket(match, App.Uid, Router.ServerWildcard));
            });
        }

        private void InvokeGui(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, action);
        }

        private async Task TaskDelay(int seconds = 3000)
        {
            await Task.Delay(seconds);
        }
    }
}
