using HelperLibrary.Networking.ClientServer.Packets;
using System;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class MatchCreatedPacket : BasePacket
    {
        public MatchCreatedPacket(string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
        }
    }
}
