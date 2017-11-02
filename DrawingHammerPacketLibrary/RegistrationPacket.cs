using HelperLibrary.Networking.ClientServer.Packets;
using System;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class RegistrationPacket : BasePacket
    {
        public RegistrationPacket(string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
        }
    }
}
