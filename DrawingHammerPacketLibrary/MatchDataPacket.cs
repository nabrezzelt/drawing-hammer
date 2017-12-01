using System;
using HelperLibrary.Networking.ClientServer.Packets;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class MatchDataPacket : BasePacket
    {
        public Match Match { get; set; }

        public MatchDataPacket(Match match, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Match = match;
        }
    }
}
