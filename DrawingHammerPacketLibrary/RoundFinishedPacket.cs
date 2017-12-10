using HelperLibrary.Networking.ClientServer.Packets;
using System;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class RoundFinishedPacket : BasePacket
    {
        public RoundFinishedPacket(string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {

        }
    }
}