using System;
using HelperLibrary.Networking.ClientServer.Packages;

namespace DrawingHammerPackageLibrary
{
    [Serializable]
    public class RoundStartedPackage : BasePackage
    {
        public int RoundNumber { get; }

        public RoundStartedPackage(int roundNumber, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            RoundNumber = roundNumber;
        }
    }
}