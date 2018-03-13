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

        /// <summary>
        /// Indicateds if the match is running
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Indicates whether a subround is currently running
        /// </summary>
        public bool IsSubRoundRunning { get; private set; }        

        /// <summary>
        /// Event, that is fired when the preparationtimer started
        /// </summary>
        public event EventHandler<PreparationTimerStartedEventArgs> PreparationTimeStarted;

        /// <summary>
        /// Event, that is fired when a new round started
        /// </summary>
        public event EventHandler<RoundStartedEventArgs> RoundStarted;
        
        /// <summary>
        /// Event, that is fired when the match started
        /// </summary>
        public event EventHandler MatchStarted;

        /// <summary>
        /// Event, that is fired when a subround started
        /// </summary>
        public event EventHandler SubRoundStarted;

        /// <summary>
        /// Event, that is fired when the preparation timer is finished
        /// </summary>
        public event EventHandler<PreparationTimeFinishedEventArgs> PreparationTimeFinished;

        /// <summary>
        /// Event, that is fired when a round is finished
        /// </summary>
        public event EventHandler RoundFinished;

        /// <summary>
        /// Event, that is fired when a subround is finished
        /// </summary>
        public event EventHandler<SubroundFinishedEventArgs> SubRoundFinished;

        /// <summary>
        /// Event, that is fired when the match is finished
        /// </summary>
        public event EventHandler MatchFinished;

        /// <summary>
        /// Event that is fired when the score of a player changed
        /// </summary>
        public event EventHandler<ScoreChangedEventArgs> ScoreChanged;

        /// <summary>
        /// Unique Id of this match
        /// </summary>
        public string MatchUid { get; }

        /// <summary>
        /// ClientId (DbId) of the user who created the match
        /// </summary>
        public int CreatorId { get; set; }

        /// <summary>
        /// Title of the match
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Rounds
        /// </summary>
        public int Rounds { get; }

        /// <summary>
        /// Maximum Players for this match
        /// </summary>
        public int MaxPlayers { get; }

        /// <summary>
        /// Roundlength in Seconds
        /// </summary>
        public int RoundLength { get; }

        /// <summary>
        /// Players of this match
        /// </summary>
        public ObservableCollection<Player> Players { get; }

        /// <summary>
        /// Player who have already drawed
        /// </summary>
        public List<Player> PlayedPlayers { get; }

        /// <summary>
        /// Already suggested words
        /// </summary>
        public ObservableCollection<Word> PickedWords { get; }

        /// <summary>
        /// Stores the 3 random words which the player should pick
        /// </summary>
        public ObservableCollection<Word> RandomWordsToPick { get; set; }

        /// <summary>
        /// Word that the current drawing player needs to draw
        /// </summary>
        public Word WordToDraw { get; set; }

        /// <summary>
        /// Remaining Time to draw.
        /// </summary>
        public int RemainingTime { get; private set; }

        /// <summary>
        /// Current Round
        /// </summary>
        public int CurrentRound { get; private set; }

        /// <summary>
        /// Strokes of the InkCanvas as byte-array
        /// </summary>
        public byte[] Strokes { get; set; }
        
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

            PreparationTimeFinished += Match_PreparationTimeFinished;
            SubRoundFinished += Match_SubRoundFinished;
        }

        private void Match_SubRoundFinished(object sender, EventArgs e)
        {            
            WordToDraw = null;
            RemainingTime = RoundLength;
            ResetHasGuessed();

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
                    IsRunning = false;
                    MatchFinished?.Invoke(this, EventArgs.Empty);
                    return;
                }

                CurrentRound++;

                RoundStarted?.Invoke(this, new RoundStartedEventArgs(CurrentRound));
            }

            player = GetPlayerWhoHasNotPlayed();
            StartPreparationTimer(player);
        }

        private void Match_PreparationTimeFinished(object sender, EventArgs e)
        {
            _currentPreparationTime = PreparationTime;

            var player = GetCurrentlyPreparingPlayer();
            player.Status = PlayerStatus.Drawing;

            StartSubRound();
        }

        private void StartSubRound()
        {
            _subRoundTimer.Start();
            IsSubRoundRunning = true;
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

        public Player GetCurrentlyDrawingPlayer()
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
            IsSubRoundRunning = false;
            SubRoundFinished?.Invoke(this, new SubroundFinishedEventArgs(WordToDraw, GetCurrentlyDrawingPlayer()));
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
            IsRunning = true;
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

        public Word GetRandomWordFromPreselectedWords()
        {
            Random random = new Random();

            int randomIndex = random.Next(RandomWordsToPick.Count);

            return RandomWordsToPick[randomIndex];
        }

        public void CalculateAndRaiseScore(string playerUid)
        {
            var successfulPlayers = GetSuccessfulGuessedPlayerCount();
            var player = Players.FirstOrDefault(p => p.Uid == playerUid);

            if (player != null && !player.HasGuessed)
            {
                var score = (Players.Count - 1 - successfulPlayers) * ScoreMultiplicator;
                //score = 2 - 1 - 0 * 100;
                Log.Debug($"Calculated: ({Players.Count} - 1 - {successfulPlayers}) * {ScoreMultiplicator} = {score}");

                player.Score += score;
                player.HasGuessed = true;
                ScoreChanged?.Invoke(this, new ScoreChangedEventArgs(player, score));
                //Raise score for drawing player            

                var drawingPlayer = GetCurrentlyDrawingPlayer();

                if (drawingPlayer != null)
                {
                    var drawingPlayerScore = (int)(score * DrawerScoreAdditionPercentage);
                    Log.Debug($"Drawing-Score: {drawingPlayerScore}");
                    drawingPlayer.Score += drawingPlayerScore;

                    ScoreChanged?.Invoke(this, new ScoreChangedEventArgs(drawingPlayer, drawingPlayerScore));
                }
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

        public bool EveryPlayerGuessedTheWord()
        {
            foreach (var player in Players)
            {
                if (!player.HasGuessed && player.Status != PlayerStatus.Drawing) return false;
            }

            return true;
        }
    }
}
