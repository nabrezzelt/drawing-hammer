using System;
using HelperLibrary.Networking.ClientServer.Packages;

namespace DrawingHammerPackageLibrary
{
    [Serializable]
    public class RequestMatchDataPackage : BasePackage
    {
        public string MatchUid { get; }

        public RequestMatchDataPackage(string matchUid, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            MatchUid = matchUid;
        }
    }
}
