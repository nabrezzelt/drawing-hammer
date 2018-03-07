using System;
using DrawingHammerPacketLibrary.Enums;
using HelperLibrary.Networking.ClientServer.Packets;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class AuthenticationResultPacket : BasePacket
    {
        public AuthenticationResult Result { get; }

        public AuthenticationResultPacket(AuthenticationResult result, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Result = result;
        }
    }
}
