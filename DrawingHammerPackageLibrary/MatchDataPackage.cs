using HelperLibrary.Networking.ClientServer.Packages;
using System;

namespace DrawingHammerPackageLibrary
{
    [Serializable]
    public class MatchDataPackage : BasePackage
    {
        public MatchData MatchData { get; }

        public MatchDataPackage(MatchData matchData, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            MatchData = matchData;
        }
    }
}
