using HelperLibrary.Networking.ClientServer.Packets;
using System;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class PreparationTimeFinishedPacket : BasePacket
    {
        public PreparationTimeFinishedPacket(string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
        }
    }
}
