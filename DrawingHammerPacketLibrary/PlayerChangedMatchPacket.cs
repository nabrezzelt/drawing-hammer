using System;
using DrawingHammerPacketLibrary.Enums;
using HelperLibrary.Networking.ClientServer.Packets;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class PlayerChangedMatchPacket : BasePacket
    {
        public MatchChangeType ChangeType { get; set; }

        public string MatchUid { get; set; }

        public Player Player { get; set; }

        public PlayerChangedMatchPacket(MatchChangeType changeType, string matchUid, Player player, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            ChangeType = changeType;
            MatchUid = matchUid;
            Player = player;
        }
    }
}
