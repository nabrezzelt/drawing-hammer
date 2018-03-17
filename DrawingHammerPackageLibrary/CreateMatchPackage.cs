using HelperLibrary.Networking.ClientServer.Packages;
using System;

namespace DrawingHammerPackageLibrary
{
    [Serializable]
    public class CreateMatchPackage : BasePackage
    {
        public MatchData MatchData { get; }

        public CreateMatchPackage(MatchData matchData, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            MatchData = matchData;
        }
    }
}
