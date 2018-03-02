using System;

namespace DrawingHammerPacketLibrary
{
    public class ScoreChangedEventArgs : EventArgs
    {        
        public Player Player { get; }

        public int RaisedScore { get; }

        public ScoreChangedEventArgs(Player player, int raisedScore)
        {
            Player = player;
            RaisedScore = raisedScore;
        }
    }
}