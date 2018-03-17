using System;
using HelperLibrary.Networking.ClientServer.Packages;

namespace DrawingHammerPackageLibrary
{
    [Serializable]
    public class LeaveMatchPackage : BasePackage
    {
        public string MatchUid { get; }

        public LeaveMatchPackage(string matchUid, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            MatchUid = matchUid;
        }
    }
}