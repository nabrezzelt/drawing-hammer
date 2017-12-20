using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DrawingHammerDesktopApp.ViewModel;
using DrawingHammerPacketLibrary;
using DrawingHammerPacketLibrary.Enums;
using HelperLibrary.Networking.ClientServer;

namespace DrawingHammerDesktopApp
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SslClient _client;

        public MainWindow()
        {            
            //if (!Properties.Settings.Default.IsConnectionConfigured)
            //{
                ConnectionSettingsWindow settingsWindow = new ConnectionSettingsWindow();
                settingsWindow.ShowDialog();
            //}

            _client = new SslClient(Properties.Settings.Default.Host, true);

            _client.ConnectionLost += OnConnectionLost;
            _client.ConnectionSucceed += OnConnectionSucceed;
            _client.PacketReceived += OnPacketReceived;

            Connect();

            DataContext = new MainWindowViewModel();

            InitializeComponent();
        }

        private async void Connect()
        {
            await Task.Run(() =>
            {
                _client.Connect(Properties.Settings.Default.Host, Properties.Settings.Default.Port);
            });
        }

        private void OnPacketReceived(object sender, PacketReceivedEventArgs e)
        {
            switch (e.Packet)
            {
                case AuthenticationResultPacket p:
                    HandleAuthenticationPacket(p);
                    break;
                case MatchDataPacket p:
                    HandleMatchDataPacket(p);
                    break;
                case PlayerJoinedMatchPacket p:
                    HandleOnPlayerJoinedMatch(p);
                    break;

                #region TimerEvents
                case MatchFinishedPacket p:
                    MessageBox.Show(p.GetType().Name);
                    break;
                case SubRoundStartedPacket p:
                    StartTimer();
                    break;
                case SubRoundFinishedPacket p:
                    StopTimer();
                    break;
                case RoundStartedPacket p:
                    ChangeRoundNumber(p.RoundNumber);
                    MessageBox.Show(p.GetType().Name + "started" + p.RoundNumber);
                    break;
                case RoundFinishedPacket p:
                    MessageBox.Show(p.GetType().Name);
                    break;
                case PreparationTimeFinishedPacket p:
                    MessageBox.Show(p.GetType().Name);
                    break;
                case PreparationTimeStartedPacket p:
                    MessageBox.Show(p.GetType().Name);
                    break;
                    #endregion
            }
        }

        private void StartTimer()
        {
            InvokeGui(() =>
            {
                var vm = (MainWindowViewModel)DataContext;

                vm.StartTimer();
            });
        }

        private void StopTimer()
        {
            InvokeGui(() =>
            {
                var vm = (MainWindowViewModel)DataContext;

                vm.ResetTimer();
            });
        }

        private void ChangeRoundNumber(int roundNumber)
        {
            InvokeGui(() =>
            {
                var vm = (MainWindowViewModel)DataContext;

                vm.CurrentRound = roundNumber;
            });
        }

        private void HandleOnPlayerJoinedMatch(PlayerJoinedMatchPacket packet)
        {
            InvokeGui(() =>
            {
                var vm = (MainWindowViewModel) DataContext;

                vm.Players.Add(packet.Player);
            });
        }

        private void HandleAuthenticationPacket(AuthenticationResultPacket packet)
        {
            if (packet.Result == AuthenticationResult.Ok)
            {
                InvokeGui(() =>
                {
                    var vm = (MainWindowViewModel) DataContext;
                    vm.MyUsername = App.Username;
                });
            }                
        }

        private void HandleMatchDataPacket(MatchDataPacket packet)
        {
            InvokeGui(() =>
            {
                var vm = (MainWindowViewModel) DataContext;

                vm.Rounds = packet.MatchData.Rounds;
                vm.CurrentRound = packet.MatchData.CurrentRound;
                vm.RemainingTime = packet.MatchData.RemainingTime;
                vm.MatchTitle = packet.MatchData.Title;
                vm.Players = packet.MatchData.Players;
            });
        }

        private void OnConnectionSucceed(object sender, EventArgs e)
        {
            InvokeGui(() =>
            {
                IsEnabled = true;
               
                LoginWindow loginWindow = new LoginWindow(_client);
                loginWindow.ShowDialog();

                GameBrowserWindow gameBrowser = new GameBrowserWindow(_client, this);
                gameBrowser.ShowDialog();

                ProgressBarLoading.Visibility = Visibility.Collapsed;
            });
        }

        private void OnConnectionLost(object sender, EventArgs e)
        {
            
        }

        private void InvokeGui(Action action)
        {           
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, action);           
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        public void MatchJoined(string matchUid)
        {
            _client.SendPacketToServer(new RequestMatchDataPacket(matchUid, App.Uid, Router.ServerWildcard));
        }
    }
}
