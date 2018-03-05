using DrawingHammerPacketLibrary.Enums;
using HelperLibrary.Cryptography;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private const int PreparationTime = 10;

        private const int ScoreMultiplicator = 100;

        private const double DrawerScoreAdditionPercentage = 0.3;


        public event EventHandler<PreparationTimerStartedEventArgs> PreparationTimeStarted;

        public event EventHandler<RoundStartedEventArgs> RoundStarted;

        public event EventHandler MatchStarted;

        public event EventHandler SubRoundStarted;

        public event EventHandler<PreparationTimeFinishedEventArgs> PreparationTimeFinished;

        public event EventHandler RoundFinished;

        public event EventHandler SubRoundFinished;

        public event EventHandler MatchFinished;

        public event EventHandler<ScoreChangedEventArgs> ScoreChanged;

        public string MatchUid { get; set; }

        public int CreatorId { get; set; }

        public string Title { get; set; }

        public int Rounds { get; set; }

        public int MaxPlayers { get; set; }

        public int RoundLength { get; set; }

        public ObservableCollection<Player> Players { get; set; }

        public List<Player> PlayedPlayers { get; set; }

        public ObservableCollection<Word> PickedWords { get; set; }

        /// <summary>
        /// Stores the 3 random words witch the player should pick
        /// </summary>
        public ObservableCollection<Word> RandomWordsToPick { get; set; }

        public Word WordToDraw { get; set; }

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
            RoundLength = roundLength;

            Players = new ObservableCollection<Player>();
            PlayedPlayers = new List<Player>();
            PickedWords = new ObservableCollection<Word>();
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

        public Player GetCurrentlyPreparingPlayer()
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
                StopSubRoundTimer();
            }
        }

        public void StopSubRoundTimer()
        {
            _subRoundTimer.Stop();
            SubRoundFinished?.Invoke(this, EventArgs.Empty);
        }

        private void PreparationTimerTicked(object sender, ElapsedEventArgs e)
        {
            _currentPreparationTime--;

            if (_currentPreparationTime <= 0)
            {
               StopPreparationTimer();
            }
        }

        public void StopPreparationTimer()
        {
            _preparationTimer.Stop();
            PreparationTimeFinished?.Invoke(this, new PreparationTimeFinishedEventArgs(GetCurrentlyPreparingPlayer()));
        }

        public void StartMatch()
        {
            var player = GetPlayerWhoHasNotPlayed();

            MatchStarted?.Invoke(this, EventArgs.Empty);
            RoundStarted?.Invoke(this, new RoundStartedEventArgs(CurrentRound));

            StartPreparationTimer(player);
        }

        private void StartPreparationTimer(Player player)
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

        public Word GetRandomWord()
        {
            Random random = new Random();
            
            int randomIndex = random.Next(RandomWordsToPick.Count);

            return RandomWordsToPick[randomIndex];            
        }

        public void CalculateAndRaiseScore(string playerUid)
        {
            var successfulPlayers = GetSuccessfulGuessedPlayerCount();

            var score = (Players.Count - 1 - successfulPlayers) * ScoreMultiplicator;
            //score = 2 - 1 - 0 * 100;
            Log.Info($"Calculated: ({Players.Count} - 1 - {successfulPlayers}) * {ScoreMultiplicator} = {score}");
            var player = Players.FirstOrDefault(p => p.Uid == playerUid);

            if (player != null)
            {
                player.Score += score;
                player.HasGuessed = true;
                ScoreChanged?.Invoke(this, new ScoreChangedEventArgs(player, score));
            }

            //Raise score for drawing player            

            var drawingPlayer = GetCurrentlyDrawingPlayer();

            if (drawingPlayer != null)
            {
                var drawingPlayerScore = (int)(score * DrawerScoreAdditionPercentage);
                Log.Info($"Drawing-Score: {drawingPlayerScore}");
                drawingPlayer.Score += drawingPlayerScore;

                ScoreChanged?.Invoke(this, new ScoreChangedEventArgs(drawingPlayer, drawingPlayerScore));
            }            
        }

        private int GetSuccessfulGuessedPlayerCount()
        {
            int count = 0;

            foreach (var player in Players)
            {
                if (player.HasGuessed)
                    count++;
            }

            return count;
        }

        private void ResetHasGuessed()
        {
            foreach (var player in Players)
            {
                player.HasGuessed = false;
            }
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
    }
}
