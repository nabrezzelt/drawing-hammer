using HelperLibrary.Networking.ClientServer.Packages;
using System;

namespace DrawingHammerPackageLibrary
{
    /// <summary>
    /// Package-Class to tell players that someone joined a match
    /// </summary>
    [Serializable]
    public class PlayerJoinedMatchPackage : BasePackage
    {
        public string MatchUid { get; }

        public Player Player { get; }

        public PlayerJoinedMatchPackage(string matchUid, Player player, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {            
            MatchUid = matchUid;
            Player = player;
        }
    }
}
