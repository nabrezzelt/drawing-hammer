using HelperLibrary.Networking.ClientServer.Packets;
using System;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class CreateMatchPacket : BasePacket
    {
        public Match Match;

        public CreateMatchPacket(Match match, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Match = match;
        }
    }
}
