using System;

namespace DrawingHammerPackageLibrary
{
    public class SubroundFinishedEventArgs : EventArgs
    {
        public Word LastWord { get; }

        public Player LastDrawingPlayer { get; }

        public SubroundFinishedEventArgs(Word lastWord, Player lastDrawingPlayer)
        {
            LastWord = lastWord;
            LastDrawingPlayer = lastDrawingPlayer;
        }
    }
}