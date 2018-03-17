using System;
using HelperLibrary.Networking.ClientServer.Packages;

namespace DrawingHammerPackageLibrary
{
    [Serializable]
    public class SubRoundStartedPackage : BasePackage
    {
        public SubRoundStartedPackage(string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {

        }
    }
}