﻿using System;
using System.Collections.Generic;
using HelperLibrary.Cryptography;
using System.Collections.ObjectModel;
using System.Timers;
using HelperLibrary.Logging;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class Match : ViewModelBase
    {
        /// <summary>
        /// Time to prepare and select a word in seconds
        /// </summary>
        public const int MaxPreparationTime = 10;

        public event EventHandler PreparationTimeStarted;

        public event EventHandler RoundStarted;

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

        public Match(string title, int rounds, int maxPlayers, int roundLength)
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
            _currentPreparationTime = MaxPreparationTime;

            _preparationTimer = new Timer(1000);
            _preparationTimer.Elapsed += PreparationTimerTicked;

            _subRoundTimer = new Timer(1000);
            _subRoundTimer.Elapsed += SubRoundTimerTicked;

            PreparationTimeFinished += Match_PreparationTimeFinished;
            SubRoundFinished += Match_SubRoundFinished;
        }


        //public Match(string uid, string title, int rounds, int maxPlayers, int roundLength)
        //{
        //    MatchUid = uid;

        //    Title = title;
        //    Rounds = rounds;
        //    MaxPlayers = maxPlayers;
        //    RoundLength = roundLength;

        //    Players = new ObservableCollection<Player>();
        //    PlayedPlayers = new List<Player>();
        //    GuessedWords = new ObservableCollection<Word>();
        //    RemainingTime = RoundLength;
        //    CurrentRound = 1;
        //    _currentPreparationTime = MaxPreparationTime;

        //    _preparationTimer = new Timer(1000);
        //    _preparationTimer.Elapsed += PreparationTimerTicked;

        //    PreparationTimeFinished += Match_PreparationTimeFinished;


        //    _subRoundTimer = new Timer(1000);
        //    _subRoundTimer.Elapsed += SubRoundTimerTicked;            
        //}

        private void Match_PreparationTimeFinished(object sender, EventArgs e)
        {
            StartSubRound();
        }

        private void Match_SubRoundFinished(object sender, EventArgs e)
        {
            StartRound();
        }

        public void StartMatch()
        {
            MatchStarted?.Invoke(this, EventArgs.Empty);

            StartRound();
        }

        public void StartRound()
        {   
            if (CurrentRound < Rounds + 1)
            {
                if (PlayedPlayers.Count < Players.Count)
                {                    
                    StartPreparationTimer();
                    var playerToPlay = GetPlayerWhoHasNotPlayed();
                    Log.Warn($"Player has played or should be play now: {playerToPlay.Username}");
                    //ToDo: Notifiy "playerToPlay" to draw a word.
                    PlayedPlayers.Add(playerToPlay);
                }
                else
                {
                    RoundFinished?.Invoke(this, EventArgs.Empty); //Roundennummer mit an client/Event senden.
                    Log.Warn(DateTime.Now + " Round finished");
                    PlayedPlayers.Clear();
                    CurrentRound++;
                    StartRound();
                }
            }
            else
            {
                MatchFinished?.Invoke(this, EventArgs.Empty);
                Log.Warn(DateTime.Now + " Match finished");
            }
        }

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


        public void StartPreparationTimer()
        {           
            _preparationTimer.Start();
            PreparationTimeStarted?.Invoke(this, EventArgs.Empty);
            Log.Warn(DateTime.Now + " PreparationTimer started!");
        }
        
        private void SubRoundTimerTicked(object sender, ElapsedEventArgs e)
        {
            RemainingTime--;

            if (RemainingTime <= 0)
            {
                RemainingTime = RoundLength;
                _subRoundTimer.Stop();
                Log.Warn(DateTime.Now + " SubRound finished!");
                SubRoundFinished?.Invoke(this, EventArgs.Empty);
            }
        }

        private void PreparationTimerTicked(object sender, ElapsedEventArgs e)
        {
            _currentPreparationTime--;

            if (_currentPreparationTime <= 0)
            {
                _currentPreparationTime = MaxPreparationTime;
                _preparationTimer.Stop();
                Log.Warn(DateTime.Now + " PreparationTimer finished!");
                PreparationTimeFinished?.Invoke(this, EventArgs.Empty);                
            }
        }

        public void StartSubRound()
        {
            _subRoundTimer.Start();
            SubRoundStarted?.Invoke(this, EventArgs.Empty);
            Log.Warn(DateTime.Now + " SubRoundTimer started!");
        }
    }
}
