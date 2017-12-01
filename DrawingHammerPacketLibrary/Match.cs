using System;
using HelperLibrary.Cryptography;
using System.Collections.ObjectModel;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class Match : ViewModelBase
    {                
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
        }
    }
}
