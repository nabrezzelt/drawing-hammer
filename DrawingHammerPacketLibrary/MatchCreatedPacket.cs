using HelperLibrary.Networking.ClientServer.Packets;
using System;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class MatchCreatedPacket : BasePacket
    {
        public Match Match;
        public MatchCreatedPacket(Match match, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Match = match;
        }
    }
}
