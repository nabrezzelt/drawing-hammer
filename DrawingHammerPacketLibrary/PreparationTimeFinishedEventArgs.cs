using System;

namespace DrawingHammerPacketLibrary
{
    public class PreparationTimeFinishedEventArgs : EventArgs
    {
        public Player PreparingPlayer { get; set; }

        public PreparationTimeFinishedEventArgs(Player preparingPlayer)
        {
            PreparingPlayer = preparingPlayer;
        }
    }
}