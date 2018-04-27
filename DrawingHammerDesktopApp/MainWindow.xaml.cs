using DrawingHammerDesktopApp.ViewModel;
using DrawingHammerPackageLibrary;
using DrawingHammerPackageLibrary.Enums;
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
            ConnectionSettingsWindow settingsWindow = new ConnectionSettingsWindow();
            settingsWindow.ShowDialog();

            _client = new SslClient(Properties.Settings.Default.Host, true);

            _client.ConnectionLost += OnConnectionLost;
            _client.ConnectionSucceed += OnConnectionSucceed;
            _client.PackageReceived += OnPackageReceived;

            Connect();

            DataContext = new MainWindowViewModel();
            _viewModel = (MainWindowViewModel)DataContext;

            InitializeComponent();

            DialogHostConnecting.IsOpen = true;
            SetPenSize(5, 5);            
        }

        private async void Connect()
        {
            await Task.Run(() =>
            {
                _client.Connect(Properties.Settings.Default.Host, Properties.Settings.Default.Port);
            });
        }

        private void OnPackageReceived(object sender, PackageReceivedEventArgs e)
        {
            switch (e.Package)
            {
                #region Autentication
                case AuthenticationResultPackage p:
                    HandleAuthenticationPackage(p);
                    break;
                case MatchDataPackage p:
                    HandleMatchDataPackage(p);
                    break;
                #endregion

                #region MatchHandling
                case PlayerJoinedMatchPackage p:
                    HandleOnPlayerJoinedMatch(p);
                    break;
                case PlayerLeftMatchPackage p:
                    HandleOnPlayerLeftMatch(p);
                    break;
                #endregion

                #region TimerEvents
                case MatchFinishedPackage _:
                    SetDrawingPlayerToGuessing();
                    StopTimer();
                    ClearDrawingAreaAndWord();
                    ClearGuessList();
                    HandleOnMatchFinished();
                    break;
                case SubRoundStartedPackage _:
                    StartTimer();
                    EnableCorrectArea();
                    break;
                case SubRoundFinishedPackage _:
                    StopTimer();
                    ClearDrawingAreaAndWord();
                    ResetCorrectGuessesInPlayerList();
                    ClearGuessList();
                    DisableDrawingArea();
                    DisableGuessingArea();
                    break;
                case RoundStartedPackage p:
                    ChangeRoundNumber(p.RoundNumber);
                    break;
                case PreparationTimeFinishedPackage _:
                    SetPreparingPlayerToDrawing();
                    break;
                case PreparationTimeStartedPackage p:
                    SetDrawingPlayerToGuessing();
                    SetPlayerToPreparing(p.PreparingPlayer);
                    break;
                #endregion

                #region Wordhandling
                case PickWordsPackage p:
                    HandlePickingWords(p);
                    break;
                case WordToDrawPackage p:
                    HandleOnWordPicked(p);
                    break;
                case WordGuessPackage p:
                    HandleOnWordGuessedByOtherPlayer(p);
                    break;
                case WordGuessCorrectPackage p:
                    HandleOnWordGuessCorrect(p);
                    break;
                case ScoreChangedPackage p:
                    HandleOnScoreChanged(p);
                    break;
                case WordSolutionPackage p:
                    HandleOnWordSolution(p);
                    break;
                #endregion

                #region DrawingArea
                case DrawingAreaChangedPackage p:
                    HandleOnDrawingAreaChanged(p);
                    break;
                    #endregion
            }
        }

        private void HandleOnPlayerLeftMatch(PlayerLeftMatchPackage package)
        {
            InvokeGui(() =>
            {
                var playerToRemove = _viewModel.Players
                    .FirstOrDefault(player => player.Uid == package.PlayerUid);

                if (playerToRemove != null)
                {
                    _viewModel.Players.Remove(playerToRemove);
                }
            });            
        }

        private void DisableGuessingArea()
        {
            InvokeGui(() => { _viewModel.CanGuess = false; });
        }

        private void DisableDrawingArea()
        {
            InvokeGui(() => { _viewModel.CanDraw = false; });
        }

        private void HandleOnMatchFinished()
        {
            InvokeGui(() =>
            {                
                _viewModel.Players = new ObservableCollection<Player>(_viewModel.Players.OrderByDescending(player => player.Score));                                
                DialogHostMatchFinished.IsOpen = true;
            });
        }

        private void HandleOnWordSolution(WordSolutionPackage package)
        {
            InvokeGui(() =>
            {
                _viewModel.MessageQueue.Enqueue($"The word to guess was: '{package.Word.Value}'", "Ok", () => { });
            });
        }

        private void EnableCorrectArea()
        {
            InvokeGui(() =>
            {
                foreach (var player in _viewModel.Players)
                {
                    Log.Debug($"Iterating-Player-Uid: {player.Uid}, Status: {player.Status}");
                    Log.Debug($"My-Player-Uid:        {App.Uid}");

                    //I have to draw
                    if ((player.Status == PlayerStatus.Preparing || player.Status == PlayerStatus.Drawing) && player.Uid == App.Uid)
                    {
                        Log.Debug("I have to draw! Enable 'CanDraw', disable 'CanGuess'");
                        _viewModel.CanDraw = true;
                        _viewModel.CanGuess = false;
                        return;
                    }
                }

                _viewModel.CanDraw = false;
                _viewModel.CanGuess = true;
                Log.Debug("Someone else is drawing! Disable 'CanDraw', enable 'CanGuess'");
            });
        }

        private void HandleOnScoreChanged(ScoreChangedPackage package)
        {
            InvokeGui(() =>
            {
                Log.Debug($"Score changed: {package}");
                var player = GetPlayerByUid(package.PlayerUid);

                if (player != null)
                {
                    player.Score += package.RaisedScore;
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

        private void ClearDrawingAreaAndWord()
        {
            InvokeGui(() =>
            {
                DrawingArea.Strokes.Clear();
                _viewModel.WordToDraw = null;
            });
        }

        private void ResetCorrectGuessesInPlayerList()
        {
            InvokeGui(() =>
            {
                foreach (var player in _viewModel.Players)
                {
                    if (player.HasGuessed)
                    {
                        player.HasGuessed = false;
                    }
                }
            });
        }

        private void HandleOnWordGuessCorrect(WordGuessCorrectPackage package)
        {
            InvokeGui(() =>
            {
                var player = GetPlayerByUid(package.PlayerUid);

                if (player != null)
                {
                    _viewModel.Guesses.Add(new Guess(player.Username, String.Empty, true));
                    Log.Debug($"Guess correct - Package: {package} - Username-by-Match: {player.Username}");
                    player.HasGuessed = true;

                    ScrollGuessListToLastItem();
                }

                if (package.PlayerUid == App.Uid)
                {                    
                    _viewModel.CanGuess = false;
                }
            });
        }

        private void HandleOnWordGuessedByOtherPlayer(WordGuessPackage package)
        {
            InvokeGui(() =>
            {
                var player = GetPlayerByUid(package.PlayerUid);

                if (player != null)
                {
                    _viewModel.Guesses.Add(new Guess(player.Username, package.GuessedWord, false));
                    Log.Debug($"Other user guessed a word: {package}");

                    ScrollGuessListToLastItem();
                }
            });
        }

        private void HandleOnDrawingAreaChanged(DrawingAreaChangedPackage package)
        {
            InvokeGui(() =>
            {
                DrawingArea.Strokes = new StrokeCollection(new MemoryStream(package.Strokes));
            });
        }

        private void HandleOnWordPicked(WordToDrawPackage package)
        {
            InvokeGui(() =>
            {
                if (DialogHostPickWords.IsOpen)
                {
                    DialogHostPickWords.IsOpen = false;
                }

                _viewModel.WordToDraw = package.WordToDraw;
            });
        }

        private void HandlePickingWords(PickWordsPackage package)
        {
            InvokeGui(() =>
            {
                _viewModel.Words = new ObservableCollection<Word>(package.WordsToSelect);

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

        private void HandleOnPlayerJoinedMatch(PlayerJoinedMatchPackage package)
        {
            InvokeGui(() =>
            {
                if (_viewModel.MatchUid == package.MatchUid)
                {
                    _viewModel.Players.Add(package.Player);
                    Log.Warn($"Player {package.Player.Username} joind with status: {package.Player.Status}");
                }
            });
        }

        private void HandleAuthenticationPackage(AuthenticationResultPackage package)
        {
            if (package.Result == AuthenticationResult.Ok)
            {
                InvokeGui(() =>
                {
                    _viewModel.MyUsername = App.Username;
                });
            }
        }

        private void HandleMatchDataPackage(MatchDataPackage package)
        {
            InvokeGui(() =>
            {
                _viewModel.Rounds = package.MatchData.Rounds;
                _viewModel.CurrentRound = package.MatchData.CurrentRound;
                _viewModel.RemainingTime = package.MatchData.RemainingTime;
                _viewModel.RoundLength = package.MatchData.RoundLength;
                _viewModel.Players = package.MatchData.Players;

                if (package.MatchData.Strokes != null)
                    DrawingArea.Strokes = new StrokeCollection(new MemoryStream(package.MatchData.Strokes));

                foreach (var player in _viewModel.Players)
                {
                    if (player.Status == PlayerStatus.Drawing)
                    {
                        //If any players of the match is drawing, enable Guessing.
                        _viewModel.CanGuess = true;
                        return;
                    }
                }
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
                DialogHostConnecting.IsOpen = false;
            });
        }

        private void OnConnectionLost(object sender, EventArgs e)
        {
            InvokeGui(() =>
            {
                _viewModel.MessageQueue.Enqueue("Connection lost! Please reconnect");
            });
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
            _client.EnqueueDataForWrite(new RequestMatchDataPackage(matchUid, App.Uid, Router.ServerWildcard));
        }       

        private void ButtonSetEraser_OnClick(object sender, RoutedEventArgs e)
        {
            DrawingArea.EditingMode = InkCanvasEditingMode.EraseByPoint;
            SetPenSize(6, 6);
        }

        private void ButtonSetColor_OnClick(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            // ReSharper disable once PossibleNullReferenceException
            var color = (Color)ColorConverter.ConvertFromString(btn.Tag.ToString());

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
                SendCurrentDrawingAreaContent();                
            }
        }

        private void SendCurrentDrawingAreaContent()
        {
            var strokeMemoryStream = new MemoryStream();
            DrawingArea.Strokes.Save(strokeMemoryStream);

            _client.EnqueueDataForWrite(new DrawingAreaChangedPackage(strokeMemoryStream.ToArray(), _viewModel.MatchUid, App.Uid, Router.ServerWildcard));
        }

        private void TextBoxGuess_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !String.IsNullOrWhiteSpace(TextBoxGuess.Text))
            {
                var guessedWord = TextBoxGuess.Text;

                _client.EnqueueDataForWrite(new WordGuessPackage(guessedWord, _viewModel.MatchUid, App.Uid, App.Uid, Router.ServerWildcard));
                TextBoxGuess.Clear();
            }
        }

        private Player GetPlayerByUid(string playerUid)
        {
            return _viewModel.Players.FirstOrDefault(player => player.Uid == playerUid);
        }

        private void ScrollGuessListToLastItem()
        {
            if (_viewModel.Guesses.Count > 0)
                ListViewGuesses.ScrollIntoView(ListViewGuesses.Items[ListViewGuesses.Items.Count - 1]);
        }

        private void DialogHost_OnDialogClosing(object sender, DialogClosingEventArgs e)
        {
            if (e.Parameter.GetType() == typeof(Word))
            {
                if (e.Parameter == null)
                    return;

                var word = (Word)e.Parameter;

                _client.EnqueueDataForWrite(new PickedWordPackage(new Word(word.Id, word.Value), _viewModel.MatchUid, App.Uid, Router.ServerWildcard));
            }
            else //if(e.Parameter == "QuitMatch")
            {
                //Quit-Match clicked
                ProfileClipQuitMatch_OnClick(sender, e);

                _viewModel.Reset();

                var gameBrowser = new GameBrowserWindow(_client, this);
                gameBrowser.ShowDialog();

                ProgressBarLoading.Visibility = Visibility.Visible;
            }            
        }

        private void ButtonResetDrawingAreaContent_OnClick(object sender, RoutedEventArgs e)
        {
            DrawingArea.Strokes.Clear();
            SendCurrentDrawingAreaContent();
        }

        private void ProfileClipQuitMatch_OnClick(object sender, RoutedEventArgs e)
        {
            _client.EnqueueDataForWrite(new LeaveMatchPackage(_viewModel.MatchUid, App.Uid, Router.ServerWildcard));
            _viewModel.Reset();

            var gameBrowser = new GameBrowserWindow(_client, this);
            gameBrowser.ShowDialog();

            ProgressBarLoading.Visibility = Visibility.Visible;
        }

        private void DialogHostConnecting_OnDialogClosing(object sender, DialogClosingEventArgs eventargs)
        {
            if (eventargs.Parameter.ToString() == "ExitApplication")
            {
                Environment.Exit(0);
            }
        }
    }
}
