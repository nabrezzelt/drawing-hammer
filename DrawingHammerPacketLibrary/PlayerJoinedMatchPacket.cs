using HelperLibrary.Networking.ClientServer.Packets;
using System;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class PlayerJoinedMatchPacket : BasePacket
    {
        public string MatchUid { get; set; }

        public Player Player { get; set; }

        public PlayerJoinedMatchPacket(string matchUid, Player player, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {            
            MatchUid = matchUid;
            Player = player;
        }
    }
}
