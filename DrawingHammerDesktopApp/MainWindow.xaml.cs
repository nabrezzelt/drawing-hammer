using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DrawingHammerDesktopApp.ViewModel;
using DrawingHammerPacketLibrary;
using DrawingHammerPacketLibrary.Enums;
using HelperLibrary.Logging;
using HelperLibrary.Networking.ClientServer;
using MaterialDesignThemes.Wpf;

namespace DrawingHammerDesktopApp
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SslClient _client;
        private MainWindowViewModel _viewModel;
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
            _viewModel = (MainWindowViewModel)DataContext;

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
                    SetDrawingPlayerToGuessing();
                    MessageBox.Show(p.GetType().Name);
                    break;
                case SubRoundStartedPacket p:
                    StartTimer();
                    break;
                case SubRoundFinishedPacket p:
                    StopTimer();
                    ClearDrawingAreaAndWord();
                    break;
                case RoundStartedPacket p:
                    ChangeRoundNumber(p.RoundNumber);
                    break;
                case PreparationTimeFinishedPacket p:
                    SetPreparingPlayerToDrawing();
                    break;
                case PreparationTimeStartedPacket p:
                    SetDrawingPlayerToGuessing();
                    SetPlayerToPreparing(p.PreparingPlayer);
                    break;
                #endregion

                #region Wordhandling
                case PickWordsPacket p:
                    HandlePickingWords(p);
                    break;
                case WordToDrawPacket p:
                    HandleOnWordPicked(p);
                    break;
                    #endregion
            }
        }

        private void ClearDrawingAreaAndWord()
        {
            InvokeGui(() =>
            {
                DrawingArea.Strokes.Clear();
                _viewModel.WordToDraw = null;
            });
        }

        private void HandleOnWordPicked(WordToDrawPacket packet)
        {
            InvokeGui(() =>
            {
                if (DialogHostPickWords.IsOpen)
                {
                    DialogHostPickWords.IsOpen = false;
                }

                _viewModel.WordToDraw = packet.WordToDraw;
                //ToDo: Enable Drawingarea (maybe clear inkcanvas)
            });
        }

        private void HandlePickingWords(PickWordsPacket packet)
        {
            InvokeGui(() =>
            {
                _viewModel.Words = new ObservableCollection<Word>(packet.WordsToSelect);

                DialogHostPickWords.IsOpen = true;
            });
        }

        private void SetPreparingPlayerToDrawing()
        {
            InvokeGui(() =>
            {
                foreach (var player in _viewModel.Players)
                {
                    if (player.Status == PlayerStatus.Preparing)
                    {
                        player.Status = PlayerStatus.Drawing;
                    }
                }
            });
        }

        private void SetDrawingPlayerToGuessing()
        {
            InvokeGui(() =>
            {
                foreach (var player in _viewModel.Players)
                {
                    if (player.Status == PlayerStatus.Drawing)
                    {
                        player.Status = PlayerStatus.Guessing;
                    }
                }
            });
        }

        private void SetPlayerToPreparing(Player preparingPlayer)
        {
            InvokeGui(() =>
            {
                foreach (var player in _viewModel.Players)
                {
                    if (player.Uid == preparingPlayer.Uid)
                    {
                        player.Status = PlayerStatus.Preparing;
                    }
                }
            });
        }

        private void StartTimer()
        {
            InvokeGui(() =>
            {                
                _viewModel.StartTimer();
            });
        }

        private void StopTimer()
        {
            InvokeGui(() =>
            {
                _viewModel.ResetTimer();
            });
        }

        private void ChangeRoundNumber(int roundNumber)
        {
            InvokeGui(() =>
            {
                _viewModel.CurrentRound = roundNumber;
            });
        }

        private void HandleOnPlayerJoinedMatch(PlayerJoinedMatchPacket packet)
        {
            InvokeGui(() =>
            {
                _viewModel.Players.Add(packet.Player);
                Log.Warn($"Player {packet.Player.Username} joind with status: {packet.Player.Status}");
            });
        }

        private void HandleAuthenticationPacket(AuthenticationResultPacket packet)
        {
            if (packet.Result == AuthenticationResult.Ok)
            {
                InvokeGui(() =>
                {
                    _viewModel.MyUsername = App.Username;
                });
            }
        }

        private void HandleMatchDataPacket(MatchDataPacket packet)
        {
            InvokeGui(() =>
            {
                _viewModel.Rounds = packet.MatchData.Rounds;
                _viewModel.CurrentRound = packet.MatchData.CurrentRound;
                _viewModel.RemainingTime = packet.MatchData.RemainingTime;
                _viewModel.RoundLength = packet.MatchData.RoundLength;
                _viewModel.Players = packet.MatchData.Players;
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
            _viewModel.MatchUid = matchUid;
            _client.SendPacketToServer(new RequestMatchDataPacket(matchUid, App.Uid, Router.ServerWildcard));
        }

        private void DialogHostPickWords_OnDialogClosing(object sender, DialogClosingEventArgs e)
        {
            if(e.Parameter == null)
                return;            

            var word = (Word) e.Parameter;
            
            _client.SendPacketToServer(new PickedWordPacket(new Word(word.Id, word.Value), _viewModel.MatchUid, App.Uid, Router.ServerWildcard));
        }
    }
}
