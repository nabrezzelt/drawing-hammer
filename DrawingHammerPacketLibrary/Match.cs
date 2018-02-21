﻿using DrawingHammerPacketLibrary.Enums;
using HelperLibrary.Cryptography;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Timers;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class Match : ViewModelBase
    {
        /// <summary>
        /// Time to prepare and select a word in seconds
        /// </summary>
        public const int PreparationTime = 10;

        public event EventHandler<PreparationTimerStartedEventArgs> PreparationTimeStarted;

        public event EventHandler<RoundStartedEventArgs> RoundStarted;

        public event EventHandler MatchStarted;

        public event EventHandler SubRoundStarted;

        public event EventHandler PreparationTimeFinished;

        public event EventHandler RoundFinished;

        public event EventHandler SubRoundFinished;

        public event EventHandler MatchFinished;

        public string MatchUid { get; set; }

        public int CreatorId { get; set; }

        public string Title { get; set; }

        public int Rounds { get; set; }

        public int MaxPlayers { get; set; }

        public int RoundLength { get; set; }

        public ObservableCollection<Player> Players { get; set; }

        public List<Player> PlayedPlayers { get; set; }

        public ObservableCollection<Word> GuessedWords { get; set; }

        public int RemainingTime { get; set; }

        public int CurrentRound { get; set; }

        private readonly Timer _preparationTimer;

        private readonly Timer _subRoundTimer;

        private int _currentPreparationTime;
        private bool _firstRound;

        public Match(string title, int rounds, int maxPlayers, int roundLength) //ToDo: Add roundLength
        {
            MatchUid = HashManager.GenerateSecureRandomToken();

            Title = title;
            Rounds = rounds;
            MaxPlayers = maxPlayers;
            RoundLength = 10;

            Players = new ObservableCollection<Player>();
            PlayedPlayers = new List<Player>();
            GuessedWords = new ObservableCollection<Word>();
            RemainingTime = RoundLength;
            CurrentRound = 1;
            _currentPreparationTime = PreparationTime;

            _preparationTimer = new Timer(1000);
            _preparationTimer.Elapsed += PreparationTimerTicked;

            _subRoundTimer = new Timer(1000);
            _subRoundTimer.Elapsed += SubRoundTimerTicked;
            
            PreparationTimeFinished += MatchPreparationTimeFinished;
            SubRoundFinished += Match_SubRoundFinished;

            SubRoundStarted += OnSubRoundStarted;
        }

        private void OnSubRoundStarted(object sender, EventArgs eventArgs)
        {
            ShowPlayerStatus();
        }


        private void Match_SubRoundFinished(object sender, EventArgs e)
        {
            RemainingTime = RoundLength;

            var player = GetCurrentlyDrawingPlayer();
            player.Status = PlayerStatus.Guessing;

            PlayedPlayers.Add(player);

            if (PlayedPlayers.Count == Players.Count)
            {
                //Runde Beendet
                PlayedPlayers.Clear();
                RoundFinished?.Invoke(this, EventArgs.Empty);

                
                if (CurrentRound == Rounds)
                {
                    MatchFinished?.Invoke(this, EventArgs.Empty);
                    return;                    
                }

                CurrentRound++;

                RoundStarted?.Invoke(this, new RoundStartedEventArgs(CurrentRound));
            }
            
            player = GetPlayerWhoHasNotPlayed();
            StartPreparationTimer(player);            
            //ToDo: set player to guessing!
            throw new NotImplementedException();
            //ToDo: Check here if other polayer have to play or complete round finished            
        }

        private void MatchPreparationTimeFinished(object sender, EventArgs e)
        {
            _currentPreparationTime = PreparationTime;

            var player = GetCurrentlyPreparingPlayer();
            player.Status = PlayerStatus.Drawing;

            StartSubRound();
        }

        private void StartSubRound()
        {
            _subRoundTimer.Start();
            SubRoundStarted?.Invoke(this, EventArgs.Empty);
        }

        private Player GetCurrentlyPreparingPlayer()
        {
            foreach (var player in Players)
            {
                if (player.Status == PlayerStatus.Preparing)
                {
                    return player;
                }
            }

            return null;
        }

        private Player GetCurrentlyDrawingPlayer()
        {
            foreach (var player in Players)
            {
                if (player.Status == PlayerStatus.Drawing)
                {
                    return player;
                }
            }

            return null;
        }

        private void SubRoundTimerTicked(object sender, ElapsedEventArgs e)
        {
            RemainingTime--;

            if (RemainingTime <= 0)
            {
                _subRoundTimer.Stop();
                SubRoundFinished?.Invoke(this, EventArgs.Empty);
            }
        }

        private void PreparationTimerTicked(object sender, ElapsedEventArgs e)
        {
            _currentPreparationTime--;

            if (_currentPreparationTime <= 0)
            {
                _preparationTimer.Stop();
                PreparationTimeFinished?.Invoke(this, EventArgs.Empty);
            }
        }

        public void StartMatch()
        {
            var player = GetPlayerWhoHasNotPlayed();

            MatchStarted?.Invoke(this, EventArgs.Empty);
            RoundStarted?.Invoke(this, new RoundStartedEventArgs(CurrentRound));

            StartPreparationTimer(player);
        }

        public void StartPreparationTimer(Player player)
        {
            player.Status = PlayerStatus.Preparing;

            PreparationTimeStarted?.Invoke(this, new PreparationTimerStartedEventArgs(player));            
            _preparationTimer.Start();
        }

        public string ShowPlayerStatus()
        {
            var statusResult = string.Empty;

            foreach (var player in Players)
            {
                statusResult += $"[{player.Username}] {player.Status} \n";                
            }

            return statusResult;
        }

        //private void MatchPreparationTimeFinished(object sender, EventArgs e)
        //{
        //    foreach (var player in Players)
        //    {
        //        if (player.Status == PlayerStatus.Preparing)
        //        {
        //            player.Status = PlayerStatus.Drawing;
        //        }
        //    }
        //    StartSubRound();
        //}

        //private void Match_SubRoundFinished(object sender, EventArgs e)
        //{
        //    foreach (var player in Players)
        //    {
        //        if (player.Status == PlayerStatus.Drawing)
        //        {
        //            player.Status = PlayerStatus.Guessing;
        //        }
        //    }

        //    StartRound();            
        //}

        //public void StartMatch()
        //{
        //    MatchStarted?.Invoke(this, EventArgs.Empty);

        //    _firstRound = true;
        //    StartRound();
        //}

        //public void StartRound()
        //{
        //    if (_firstRound)
        //    {
        //        RoundStarted?.Invoke(this, new RoundStartedEventArgs(CurrentRound));
        //        Log.Warn(DateTime.Now + " Round started");
        //        Log.Warn(this.ShowPlayerStatus());
        //        _firstRound = false;
        //    }

        //    if (CurrentRound < Rounds + 1)
        //    {
        //        if (PlayedPlayers.Count < Players.Count)
        //        {
        //            var playerToPlay = GetPlayerWhoHasNotPlayed();
        //            //ToDo: Notifiy "playerToPlay" to draw a word.
        //            Log.Warn($"Player should be play now: {playerToPlay.Username}");
        //            PlayedPlayers.Add(playerToPlay);                    
        //            StartPreparationTimer(playerToPlay);
        //            Log.Warn(this.ShowPlayerStatus());
        //        }
        //        else
        //        {
        //            RoundFinished?.Invoke(this, EventArgs.Empty); //Roundennummer mit an client/Event senden.
        //            Log.Warn(DateTime.Now + " Round finished");                    
        //            PlayedPlayers.Clear();
        //            CurrentRound++;

        //            if (CurrentRound <= Rounds)
        //            {
        //                RoundStarted?.Invoke(this, new RoundStartedEventArgs(CurrentRound));
        //                Log.Warn(DateTime.Now + " Round started");                        
        //                StartRound();
        //            }
        //            else
        //            {
        //                MatchFinished?.Invoke(this, EventArgs.Empty);
        //                Log.Warn(DateTime.Now + " Match finished");                        
        //            }
        //        }
        //    }
        //}

        private Player GetPlayerWhoHasNotPlayed()
        {
            foreach (var player in Players)
            {
                if (!PlayedPlayers.Contains(player))
                {
                    return player;
                }
            }

            return null;
        }

        //public void StartPreparationTimer(Player player)
        //{
        //    player.Status = PlayerStatus.Preparing;
        //    _preparationTimer.Start();
        //    PreparationTimeStarted?.Invoke(this, new PreparationTimerStartedEventArgs(player));
        //    Log.Warn(DateTime.Now + " PreparationTimer started!");            
        //}

        //private void SubRoundTimerTicked(object sender, ElapsedEventArgs e)
        //{
        //    RemainingTime--;

        //    if (RemainingTime <= 0)
        //    {
        //        RemainingTime = RoundLength;
        //        _subRoundTimer.Stop();
        //        Log.Warn(DateTime.Now + " SubRound finished!");                
        //        SubRoundFinished?.Invoke(this, EventArgs.Empty);                
        //    }
        //}

        //private void PreparationTimerTicked(object sender, ElapsedEventArgs e)
        //{
        //    _currentPreparationTime--;

        //    if (_currentPreparationTime <= 0)
        //    {
        //        _currentPreparationTime = MaxPreparationTime;
        //        _preparationTimer.Stop();
        //        Log.Warn(DateTime.Now + " PreparationTimer finished!");                
        //        PreparationTimeFinished?.Invoke(this, EventArgs.Empty);                
        //    }
        //}

        //public void StartSubRound()
        //{
        //    _subRoundTimer.Start();
        //    SubRoundStarted?.Invoke(this, EventArgs.Empty);
        //    Log.Warn(DateTime.Now + " SubRoundTimer started!");            
        //}        
    }
}
