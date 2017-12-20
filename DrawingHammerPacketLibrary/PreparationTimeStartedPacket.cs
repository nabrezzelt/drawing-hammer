using System;
using HelperLibrary.Networking.ClientServer.Packets;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class PreparationTimeStartedPacket : BasePacket
    {
        public PreparationTimeStartedPacket(string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {

        }
    }
}