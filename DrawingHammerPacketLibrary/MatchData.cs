using System;
using System.Collections.ObjectModel;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class MatchData : ViewModelBase
    {
        public string MatchUid { get; }

        public int CreatorId { get; }

        public string Title { get; }

        public int Rounds { get; }

        public int MaxPlayers { get; }

        public int RoundLength { get; }

        public ObservableCollection<Player> Players { get; }

        public int RemainingTime { get; }

        public int CurrentRound { get; }

        public byte[] Strokes { get; }

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

            Strokes = match.Strokes;
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
    }
}
