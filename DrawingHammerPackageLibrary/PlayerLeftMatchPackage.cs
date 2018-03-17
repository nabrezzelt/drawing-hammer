using System;
using HelperLibrary.Networking.ClientServer.Packages;

namespace DrawingHammerPackageLibrary
{
    [Serializable]
    public class PlayerLeftMatchPackage : BasePackage
    {
        public string MatchUid { get; }

        public string PlayerUid { get; }

        public PlayerLeftMatchPackage(string matchUid, string playerUid, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            MatchUid = matchUid;
            PlayerUid = playerUid;
        }
    }
}