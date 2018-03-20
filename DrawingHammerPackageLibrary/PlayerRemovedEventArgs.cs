using System;

namespace DrawingHammerPackageLibrary
{
    public class PlayerRemovedEventArgs : EventArgs
    {
        public string PlayerUid { get; }

        public PlayerRemovedEventArgs(string playerUid)
        {
            PlayerUid = playerUid;
        }
    }
}