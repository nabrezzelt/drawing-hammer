using System;
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
        public const int PreparationTime = 10;

        public event EventHandler PreparationTimeFinished;

        public event EventHandler RoundFinished;

        public event EventHandler MatchFinished;

        public string MatchUid { get; set; }

        public int CreatorId { get; set; }

        public string Title { get; set; }

        public int Rounds { get; set; }

        public int MaxPlayers { get; set; }

        public int RoundLength { get; set; }

        public ObservableCollection<Player> Players { get; set; }

        public ObservableCollection<Word> GuessedWords { get; set; }

        public int RemainingTime { get; set; }

        public int CurrentRound { get; set; }

        private int _currentPreparationTime;

        public Match(string title, int rounds, int maxPlayers, int roundLength)
        {
            MatchUid = HashManager.GenerateSecureRandomToken();

            Title = title;
            Rounds = rounds;
            MaxPlayers = maxPlayers;
            RoundLength = roundLength;

            Players = new ObservableCollection<Player>();
            RemainingTime = RoundLength;
            CurrentRound = 1;
            _currentPreparationTime = 0;

            _preparationTimer = new Timer(1000);
            _preparationTimer.Elapsed += PreparationTimerTicked;

            _roundTimer = new Timer(1000);
            _roundTimer.Elapsed += RoundTimer_Ticked;
        }

        public Match(string uid, string title, int rounds, int maxPlayers, int roundLength)
        {
            MatchUid = uid;

            Title = title;
            Rounds = rounds;
            MaxPlayers = maxPlayers;
            RoundLength = roundLength;

            Players = new ObservableCollection<Player>();
            RemainingTime = RoundLength;
            CurrentRound = 1;
            _currentPreparationTime = 0;

            _preparationTimer = new Timer(1000);
            _preparationTimer.Elapsed += PreparationTimerTicked;

            _roundTimer = new Timer(1000);
            _roundTimer.Elapsed += RoundTimer_Ticked;            
        }

        //ToDo: Needs to be checked, because is firing random events :x
        private void RoundTimer_Ticked(object sender, ElapsedEventArgs e)
        {
            RemainingTime--;

            if (RemainingTime == 0)
            {
                _roundTimer.Stop();
                RoundFinished?.Invoke(this, EventArgs.Empty);

                if (CurrentRound < Rounds)
                {
                    Log.Warn($"Round {CurrentRound} finished!");
                    CurrentRound++;
                    
                    StartPreparationTimer();
                }
                else
                {
                    MatchFinished?.Invoke(this, EventArgs.Empty);
                    Log.Warn("Match finished!");
                }
            }            
        }

        //ToDo: Needs to be checked, because is not working properly :x
        private void PreparationTimerTicked(object sender, ElapsedEventArgs e)
        {


            if (_currentPreparationTime <= PreparationTime)
            {
                _currentPreparationTime++;
            }
            else            
            {
                _preparationTimer.Stop();
                Log.Warn("PreparationTime finished!");
                PreparationTimeFinished?.Invoke(this, EventArgs.Empty);                
                StartRound();
            }

        }

        private readonly Timer _preparationTimer;

        private readonly Timer _roundTimer;

        public void StartPreparationTimer()
        {
            _preparationTimer.Start();
            Log.Warn("PreparationTimer started!");
        }

        public void StartRound()
        {
            _roundTimer.Start();
            Log.Warn("RoundTimer started!");
        }
    }
}
