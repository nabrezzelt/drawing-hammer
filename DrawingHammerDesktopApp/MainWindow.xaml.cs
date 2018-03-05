﻿using DrawingHammerDesktopApp.ViewModel;
using DrawingHammerPacketLibrary;
using DrawingHammerPacketLibrary.Enums;
using HelperLibrary.Logging;
using HelperLibrary.Networking.ClientServer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;


namespace DrawingHammerDesktopApp
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly SslClient _client;
        private readonly MainWindowViewModel _viewModel;
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

            SetPenSize(5, 5);
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
                case SubRoundStartedPacket _:
                    StartTimer();
                    EnableCorrectArea();
                    break;
                case SubRoundFinishedPacket _:
                    StopTimer();
                    ClearDrawingAreaAndWord();
                    ResetCorrectGuessesInPlayerList();
                    ClearGuessList();
                    break;
                case RoundStartedPacket p:
                    ChangeRoundNumber(p.RoundNumber);
                    break;
                case PreparationTimeFinishedPacket _:
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
                case WordGuessPacket p:
                    HandleOnWordGuessedByOtherPlayer(p);
                    break;
                case WordGuessCorrectPacket p:
                    HandleOnWordGuessCorrect(p);
                    break;
                case ScoreChangedPacket p:
                    HandleOnScoreChanged(p);
                    break;
                #endregion

                #region DrawingArea
                case DrawingAreaChangedPacket p:
                    HandleOnDrawingAreaChanged(p);
                    break;                    
                #endregion
            }
        }

        private void EnableCorrectArea()
        {
            InvokeGui(() =>
            {
                foreach (var player in _viewModel.Players)
                {
                    if (player.Status == PlayerStatus.Drawing && player.Uid == App.Uid)
                    {
                        _viewModel.CanDraw = true;
                        return;
                    }
                }

                _viewModel.CanGuess = true;
            });            
        }

        private void HandleOnScoreChanged(ScoreChangedPacket packet)
        {
            InvokeGui(() =>
            {
                var player = _viewModel.Players.FirstOrDefault(p => p.Uid == packet.PlayerUid);

                if (player != null)
                {
                    player.Score += packet.RaisedScore;
                }
            });            
        }

        private void ClearGuessList()
        {
            InvokeGui(() =>
            {
                _viewModel.Guesses.Clear();
                TextBoxGuess.Clear();
            });
        }

        private void ResetCorrectGuessesInPlayerList()
        {
            InvokeGui(() =>
            {
                foreach (var player in _viewModel.Players)
                {
                    player.HasGuessed = false;
                }
            });
        }

        private void HandleOnWordGuessCorrect(WordGuessCorrectPacket packet)
        {
            InvokeGui(() =>
            {
                var player = GetPlayerByUid(packet.PlayerUid);

                if (player != null)
                {
                    _viewModel.Guesses.Add(new Guess(player.Username, String.Empty, true));
                }
            });                     
        }

        private void HandleOnWordGuessedByOtherPlayer(WordGuessPacket packet)
        {
            InvokeGui(() =>
            {
                var player = GetPlayerByUid(packet.PlayerUid);

                if (player != null)
                {
                    _viewModel.Guesses.Add(new Guess(player.Username, packet.GuessedWord, true));
                }
            });            
        }

        private void HandleOnDrawingAreaChanged(DrawingAreaChangedPacket packet)
        {
            InvokeGui(() =>
            {
                DrawingArea.Strokes = new StrokeCollection(new MemoryStream(packet.Strokes));
            });            
        }

        private void ClearDrawingAreaAndWord()
        {
            InvokeGui(() =>
            {
                DrawingArea.Strokes.Clear();
                _viewModel.WordToDraw = null;
                _viewModel.CanDraw = false;
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

                    if (player.Uid == App.Uid)
                    {
                        //I have to draw
                        _viewModel.CanDraw = true;
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

                        if (player.Uid == App.Uid)
                        {
                            //I have drawed
                            _viewModel.CanDraw = false;                            
                        }
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
       
        private void SetEraser(object sender, RoutedEventArgs e)
        {
            DrawingArea.EditingMode = InkCanvasEditingMode.EraseByPoint;
            SetPenSize(5, 5);
        }

        private void SetColor(object sender, RoutedEventArgs e)
        {
            var btn = (Button) sender;
            // ReSharper disable once PossibleNullReferenceException
            var color = (Color) ColorConverter.ConvertFromString(btn.Tag.ToString());

            DrawingArea.DefaultDrawingAttributes.Color = color;
            DrawingArea.EditingMode = InkCanvasEditingMode.Ink;
            SetPenSize(5, 5);
        }

        private void SetPenSize(int height, int width)
        {
            DrawingArea.DefaultDrawingAttributes.Height = height;
            DrawingArea.DefaultDrawingAttributes.Width = width;
        }

        private void DrawingArea_OnChanged(object sender, RoutedEventArgs e)
        {
            if (_viewModel.CanDraw)
            {
                var strokeMemoryStream = new MemoryStream();
                DrawingArea.Strokes.Save(strokeMemoryStream);                                

                _client.SendPacketToServer(new DrawingAreaChangedPacket(strokeMemoryStream.ToArray(), _viewModel.MatchUid, App.Uid, Router.ServerWildcard));
            }            
        }

        private void TextBoxGuess_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !String.IsNullOrWhiteSpace(TextBoxGuess.Text))
            {
                var guessedWord = TextBoxGuess.Text;

                _client.SendPacketToServer(new WordGuessPacket(guessedWord, _viewModel.MatchUid, App.Uid, App.Uid, Router.ServerWildcard));
            }
        }

        private Player GetPlayerByUid(string playerUid)
        {
            return _viewModel.Players.FirstOrDefault(player => player.Uid == playerUid);
        }
    }
}
