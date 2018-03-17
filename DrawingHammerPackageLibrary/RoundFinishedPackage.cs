using HelperLibrary.Networking.ClientServer.Packages;
using System;

namespace DrawingHammerPackageLibrary
{
    [Serializable]
    public class RoundFinishedPackage : BasePackage
    {
        public RoundFinishedPackage(string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {

        }
    }
}