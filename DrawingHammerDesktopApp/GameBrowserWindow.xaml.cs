﻿using DrawingHammerDesktopApp.ViewModel;
using DrawingHammerPacketLibrary;
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
            _client.PacketReceived += OnPacketReceived;

            RequestGameList();
        }

        private async void RequestGameList()
        {
            await Task.Run(() =>
            {
                _client.EnqueueDataForWrite(new RequestGamelistPacket(App.Uid, Router.ServerWildcard));
                Log.Debug("Matchlist requested.");
            });
        }

        private void OnPacketReceived(object sender, PacketReceivedEventArgs e)
        {
            switch (e.Packet)
            {
                case GameListPacket p:
                    HandleOnGameListReceived(p);
                    break;                
                case CreateMatchPacket p:
                    AddMatchToList(p.MatchData);
                    break;
                case PlayerJoinedMatchPacket p:
                    HandleOnPlayerChangedMatch(p);
                    break;
                case MatchJoinFailedPacket p:
                    HandleOnJoinMatchFailed(p);
                    break;
                case PlayerLeftMatchPacket p:
                    HandleOnPlayerLeftMatch(p);
                    break;
            }
        }

        private void HandleOnPlayerLeftMatch(PlayerLeftMatchPacket packet)
        {
           InvokeGui(() =>
           {
               var match = _viewModel.GetMatch(packet.MatchUid);

               var playerToRemove = match?.Players
                   .FirstOrDefault(player => player.Uid == packet.PlayerUid);

               if (playerToRemove != null)
               {
                   match.Players.Remove(playerToRemove);
               }
           });
        }

        private void HandleOnJoinMatchFailed(MatchJoinFailedPacket packet)
        {
            InvokeGui(() =>
            {
                StatusSnackbar.MessageQueue.Enqueue($"Could not join this match (Reason: {packet.Reason})");
            });
        }

        private void HandleOnPlayerChangedMatch(PlayerJoinedMatchPacket packet)
        {
            InvokeGui(() =>
            {                
                if (packet.Player.Uid == App.Uid)
                {
                    _mainWindow.MatchJoined(packet.MatchUid);
                    _matchJoined = true;
                    Close();
                }

                var match = _viewModel.GetMatch(packet.MatchUid);
                match.Players.Add(packet.Player);                                
            });            
        }

        private void AddMatchToList(MatchData matchData)
        {
            InvokeGui(() =>
            {
                _viewModel.Matches.Add(matchData);
            });
        }

        private void HandleOnGameListReceived(GameListPacket packet)
        {
            InvokeGui(() =>
            {
                _viewModel.Matches = packet.Matches;
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
                    _client.EnqueueDataForWrite(new JoinMatchPacket(selectedMatch.MatchUid, App.Uid, Router.ServerWildcard));
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
