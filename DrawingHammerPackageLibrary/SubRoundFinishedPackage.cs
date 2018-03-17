using System;
using HelperLibrary.Networking.ClientServer.Packages;

namespace DrawingHammerPackageLibrary
{
    [Serializable]
    public class SubRoundFinishedPackage : BasePackage
    {
        public SubRoundFinishedPackage(string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {

        }
    }
}