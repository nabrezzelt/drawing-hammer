using System;

namespace DrawingHammerPacketLibrary
{
    public class RoundStartedEventArgs : EventArgs
    {
        public int RoundNumber { get; }

        public RoundStartedEventArgs(int roundNumber)
        {
            RoundNumber = roundNumber;
        }
    }
}