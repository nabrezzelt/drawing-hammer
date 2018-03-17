using System;
using HelperLibrary.Networking.ClientServer.Packages;

namespace DrawingHammerPackageLibrary
{
    [Serializable]
    public class PreparationTimeStartedPackage : BasePackage
    {
        public Player PreparingPlayer { get; }

        public PreparationTimeStartedPackage(Player preparingPlayer, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            PreparingPlayer = preparingPlayer;
        }
    }
}