using System;
using HelperLibrary.Networking.ClientServer.Packages;

namespace DrawingHammerPackageLibrary
{
    [Serializable]    
    public class ScoreChangedPackage : BasePackage
    {
        public string PlayerUid { get; }

        public int RaisedScore { get; }

        public ScoreChangedPackage(string playerUid, int raisedScore, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            PlayerUid = playerUid;
            RaisedScore = raisedScore;
        }
    }
}