using System;
using HelperLibrary.Networking.ClientServer.Packets;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class PlayerLeftMatchPacket : BasePacket
    {
        public string MatchUid { get; }

        public string PlayerUid { get; }

        public PlayerLeftMatchPacket(string matchUid, string playerUid, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            MatchUid = matchUid;
            PlayerUid = playerUid;
        }
    }
}