using DrawingHammerPacketLibrary;
using HelperLibrary.Networking.ClientServer;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DrawingHammerDesktopApp
{
    /// <summary>
    /// Interaktionslogik für CreateNewGameWindow.xaml
    /// </summary>
    public partial class CreateNewGameWindow
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
            InvokeGui(() =>
            {
                switch (e.Packet)
                {
                    case MatchCreatedPacket _:
                        NotifyForCreatedMatch();
                        break;
                }
            });
        }

        private void NotifyForCreatedMatch()
        {
            StatusSnackbar.MessageQueue.Enqueue("Match sucessfully created.");
            Close();
        }

        private void OnConnectionLost(object sender, EventArgs e)
        {
            StatusSnackbar.MessageQueue.Enqueue("Connection lost!");
        }

        private void CheckToEnableCreateButton(object sender, TextChangedEventArgs e)
        {
            ButtonCreate.IsEnabled = TextBoxGameName.Text != "";
        }

        private void Create(object sender, RoutedEventArgs e)
        {
            SendCreateMassage(TextBoxGameName.Text, Convert.ToInt32(SliderRounds.Value), Convert.ToInt32(SliderPlayers.Value), Convert.ToInt32(SliderRoundLength.Value));
        }

        private void SendCreateMassage(string matchTitle, int maxRounds, int maxPlayers, int roundLength)
        {
            _client.SendPacketToServer(new CreateMatchPacket(new MatchData(matchTitle, maxRounds, maxPlayers, roundLength), App.Uid, Router.ServerWildcard));
        }

        private void InvokeGui(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, action);
        }
    }
}
