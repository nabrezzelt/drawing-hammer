using System;
using HelperLibrary.Cryptography;
using System.Collections.Generic;
using System.Timers;


namespace DrawingHammerPacketLibrary
{
    public class Match
    {
        public EventHandler<EventArgs> RoundFinished;
        public EventHandler<EventArgs> MatchFinished;

        public readonly string Uid; 
        public string Title;
        public int Rounds;
        public int MaxPlayers;
        public int RoundLengh;

        public List<Player> Players;
        public List<Word> GuessedWords;

        public int CurrentTime;
        public int CurrentRound;

        //private Timer timer;

        public Match(string title, int rounds, int maxPlayers, int roundLengh)
        {
            Uid = HashManager.GenerateSecureRandomToken();

            Title = title;
            Rounds = rounds;
            MaxPlayers = maxPlayers;
            RoundLengh = roundLengh;

            Players = new List<Player>();
            CurrentTime = 0;
            CurrentRound = 1;

            //timer.Interval = 1000;
            //timer.Elapsed += TimerTicked;
        }

        //private void TimerTicked(object sender, ElapsedEventArgs elapsedEventArgs)
        //{
            
        //}

        //public void Start()
        //{
        //    timer.Start();
        //}

        //public void Stop()
        //{
        //    timer.Stop();
        //}
    }
}
