using HelperLibrary.Networking.ClientServer.Packets;
using System;

namespace DrawingHammerPacketLibrary
{
    /// <summary>
    /// Package-Class to tell players that someone joined a match
    /// </summary>
    [Serializable]
    public class PlayerJoinedMatchPacket : BasePacket
    {
        public string MatchUid { get; }

        public Player Player { get; }

        public PlayerJoinedMatchPacket(string matchUid, Player player, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {            
            MatchUid = matchUid;
            Player = player;
        }
    }
}
