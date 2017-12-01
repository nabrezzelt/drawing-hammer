﻿using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using DrawingHammerDesktopApp.ViewModel;
using DrawingHammerPacketLibrary;
using DrawingHammerPacketLibrary.Enums;
using HelperLibrary.Logging;
using HelperLibrary.Networking.ClientServer;

namespace DrawingHammerDesktopApp
{
    /// <summary>
    /// Interaktionslogik für GameBrowserWindow.xaml
    /// </summary>
    public partial class GameBrowserWindow : Window
    {
        private readonly MainWindow _mainWindow;
        private readonly SslClient _client;
        private bool _matchJoined = false;

        public GameBrowserWindow(SslClient client, MainWindow mainWindow)
        {
            DataContext = new GameBrowserViewModel();

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
                _client.SendPacketToServer(new RequestGamelistPacket(App.Uid, Router.ServerWildcard));
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
                    AddMatchToList(p.Match);
                    break;
                case PlayerChangedMatchPacket p:
                    HandleOnPlayerChangedMatch(p);
                    break;
                case MatchJoinFailedPacket p:
                    HandleOnJoinMatchFailed(p);
                    break;
            }
        }

        private void HandleOnJoinMatchFailed(MatchJoinFailedPacket packet)
        {
            InvokeGui(() =>
            {
                StatusSnackbar.MessageQueue.Enqueue($"Could not join this match (Reason: {packet.Reason})");
            });
        }

        private void HandleOnPlayerChangedMatch(PlayerChangedMatchPacket packet)
        {
            InvokeGui(() =>
            {
                if (packet.ChangeType == MatchChangeType.Joined)
                {
                    if (packet.Player.Uid == App.Uid)
                    {
                        _mainWindow.MatchJoined(packet.MatchUid);
                        _matchJoined = true;
                        Close();
                    }

                    var match = ((GameBrowserViewModel) DataContext).GetMatch(packet.MatchUid);
                    match.Players.Add(packet.Player);                   
                }
            });            
        }

        private void AddMatchToList(Match match)
        {
            InvokeGui(() =>
            {
                var vm = (GameBrowserViewModel) DataContext;
                vm.Matches.Add(match);
            });
        }

        private void HandleOnGameListReceived(GameListPacket packet)
        {
            InvokeGui(() =>
            {
                var vm = (GameBrowserViewModel)DataContext;
                vm.Matches = packet.Matches;
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

        private void lvGameList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnJoin.IsEnabled = lvGamelist.SelectedItem != null;
        }

        private void lvGamelist_OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvGamelist.SelectedItem != null)
            {
                JoinMatch(sender, e);
            }
        }

        private void JoinMatch(object sender, RoutedEventArgs e)
        {            
            InvokeGui(async () =>
            {
                Match selectedMatch = (Match) lvGamelist.SelectedItem;
                await Task.Run(() =>
                {
                    _client.SendPacketToServer(new JoinMatchPacket(selectedMatch.MatchUid, App.Uid, Router.ServerWildcard));
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
