using HelperLibrary.Networking.ClientServer.Packages;
using System;

namespace DrawingHammerPackageLibrary
{
    [Serializable]
    public class MatchFinishedPackage : BasePackage
    {
        public string MatchUid { get; }

        public MatchFinishedPackage(string matchUid, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            MatchUid = matchUid;
        }
    }
}