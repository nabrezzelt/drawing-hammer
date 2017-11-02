using HelperLibrary.Networking.ClientServer.Packets;
using System;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class RegistrationResultPacket : BasePacket
    {
        public RegistrationResultPacket(string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
        }
    }
}
