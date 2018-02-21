using HelperLibrary.Networking.ClientServer.Packets;
using System;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class MatchFinishedPacket : BasePacket
    {
        public MatchFinishedPacket(string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
        }
    }
}