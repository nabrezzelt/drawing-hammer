using System;
using System.Collections.ObjectModel;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class MatchData : ViewModelBase
    {
        public string MatchUid { get; set; }

        public int CreatorId { get; set; }

        public string Title { get; set; }

        public int Rounds { get; set; }

        public int MaxPlayers { get; set; }

        public int RoundLength { get; set; }

        public ObservableCollection<Player> Players { get; set; }

        public int RemainingTime { get; set; }

        public int CurrentRound { get; set; }

        public MatchData(Match match)
        {
            MatchUid = match.MatchUid;
            CreatorId = match.CreatorId;

            Title = match.Title;
            Rounds = match.Rounds;
            MaxPlayers = match.MaxPlayers;
            RoundLength = match.RoundLength;

            Players = match.Players;
            RemainingTime = match.RemainingTime;
            CurrentRound = match.CurrentRound;
        }
        
        public MatchData(string title, int rounds, int maxPlayers, int roundLength)
        {
            MatchUid = "";
            CreatorId = 0;
            Title = title;
            Rounds = rounds;
            MaxPlayers = maxPlayers;
            RoundLength = roundLength;

            Players = new ObservableCollection<Player>();
            RemainingTime = RoundLength;
            CurrentRound = 1;            
        }

        public MatchData(string title, int creatorId, int rounds, int maxPlayers, int roundLength, ObservableCollection<Player> players, int remainingTime, int currentRound)
        {
            MatchUid = "";
            CreatorId = creatorId;
            Title = title;
            Rounds = rounds;
            MaxPlayers = maxPlayers;
            RoundLength = roundLength;

            Players = players;
            RemainingTime = remainingTime;
            CurrentRound = currentRound;
        }
    }
}
