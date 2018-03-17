using HelperLibrary.Networking.ClientServer.Packages;
using System;

namespace DrawingHammerPackageLibrary
{
    [Serializable]
    public class MatchCreatedPackage : BasePackage
    {
        public MatchCreatedPackage(string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
        }
    }
}
