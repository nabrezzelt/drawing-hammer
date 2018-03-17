using System;

namespace DrawingHammerPackageLibrary
{
    public class PreparationTimeFinishedEventArgs : EventArgs
    {
        public Player PreparingPlayer { get; }

        public PreparationTimeFinishedEventArgs(Player preparingPlayer)
        {
            PreparingPlayer = preparingPlayer;
        }
    }
}