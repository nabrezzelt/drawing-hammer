using DrawingHammerDesktopApp.ViewModel;
using DrawingHammerPackageLibrary;
using HelperLibrary.Logging;
using HelperLibrary.Networking.ClientServer;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace DrawingHammerDesktopApp
{
    /// <summary>
    /// Interaktionslogik für GameBrowserWindow.xaml
    /// </summary>
    public partial class GameBrowserWindow
    {
        private readonly MainWindow _mainWindow;
        private readonly SslClient _client;
        private bool _matchJoined;

        private readonly GameBrowserViewModel _viewModel;

        public GameBrowserWindow(SslClient client, MainWindow mainWindow)
        {
            DataContext = new GameBrowserViewModel();

            _viewModel = (GameBrowserViewModel) DataContext;

            InitializeComponent();

            _mainWindow = mainWindow;
            _client = client;

            _client.ConnectionLost += OnConnectionLost;
            _client.PackageReceived += OnPackageReceived;

            RequestGameList();
        }

        private async void RequestGameList()
        {
            await Task.Run(() =>
            {
                _client.EnqueueDataForWrite(new RequestGamelistPackage(App.Uid, Router.ServerWildcard));
                Log.Debug("Matchlist requested.");
            });
        }

        private void OnPackageReceived(object sender, PackageReceivedEventArgs e)
        {
            switch (e.Package)
            {
                case GameListPackage p:
                    HandleOnGameListReceived(p);
                    break;                
                case CreateMatchPackage p:
                    AddMatchToList(p.MatchData);
                    break;
                case PlayerJoinedMatchPackage p:
                    HandleOnPlayerChangedMatch(p);
                    break;
                case MatchJoinFailedPackage p:
                    HandleOnJoinMatchFailed(p);
                    break;
                case PlayerLeftMatchPackage p:
                    HandleOnPlayerLeftMatch(p);
                    break;
                case MatchFinishedPackage p:
                    HandleOnMatchFinished(p);
                    break;             
            }
        }

        private void HandleOnMatchFinished(MatchFinishedPackage package)
        {
            InvokeGui(() =>
            {
                var match = _viewModel.Matches.FirstOrDefault(m => m.MatchUid == package.MatchUid);

                if (match != null)
                    match.IsFinished = true;
            });
        }

        private void HandleOnPlayerLeftMatch(PlayerLeftMatchPackage package)
        {
            RequestGameList();
        }

        private void HandleOnJoinMatchFailed(MatchJoinFailedPackage package)
        {
            InvokeGui(() =>
            {
                StatusSnackbar.MessageQueue.Enqueue($"Could not join this match (Reason: {package.Reason})");
            });
        }

        private void HandleOnPlayerChangedMatch(PlayerJoinedMatchPackage package)
        {
            InvokeGui(() =>
            {                
                if (package.Player.Uid == App.Uid)
                {
                    _mainWindow.MatchJoined(package.MatchUid);
                    _matchJoined = true;
                    Close();
                }

                var match = _viewModel.GetMatch(package.MatchUid);
                match.Players.Add(package.Player);                                
            });            
        }

        private void AddMatchToList(MatchData matchData)
        {
            InvokeGui(() =>
            {
                _viewModel.Matches.Add(matchData);
            });
        }

        private void HandleOnGameListReceived(GameListPackage package)
        {
            InvokeGui(() =>
            {
                _viewModel.Matches = package.Matches;
            });          
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

        private void ListViewGameList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonJoin.IsEnabled = ListViewGamelist.SelectedItem != null;
        }

        private void ListViewGamelist_OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListViewGamelist.SelectedItem != null)
            {
                JoinMatch(sender, e);
            }
        }

        private void JoinMatch(object sender, RoutedEventArgs e)
        {            
            InvokeGui(async () =>
            {
                MatchData selectedMatch = (MatchData) ListViewGamelist.SelectedItem;
                await Task.Run(() =>
                {
                    _client.EnqueueDataForWrite(new JoinMatchPackage(selectedMatch.MatchUid, App.Uid, Router.ServerWildcard));
                });
            });           
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {            
            if(!_matchJoined)
                Environment.Exit(0);
        }
    }
}
