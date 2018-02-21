using System;
using HelperLibrary.Networking.ClientServer.Packets;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class SubRoundFinishedPacket : BasePacket
    {
        public SubRoundFinishedPacket(string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {

        }
    }
}