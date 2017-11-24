using System;
using HelperLibrary.Networking.ClientServer.Packets;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class MatchJoinFailedPacket : BasePacket
    {
        public string Reason { get; set; }

        public MatchJoinFailedPacket(string reason, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Reason = reason;
        }
    }
}
