using System;
using HelperLibrary.Networking.ClientServer.Packets;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class SubRoundStartedPacket : BasePacket
    {
        public SubRoundStartedPacket(string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {

        }
    }
}