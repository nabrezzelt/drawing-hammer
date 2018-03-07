using HelperLibrary.Networking.ClientServer.Packets;
using System;
using DrawingHammerPacketLibrary.Enums;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class RegistrationResultPacket : BasePacket
    {
        public RegistrationResult Result { get; }
        
        public RegistrationResultPacket(RegistrationResult result, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Result = result;
        }
    }
}
