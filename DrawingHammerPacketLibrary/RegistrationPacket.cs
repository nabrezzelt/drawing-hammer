using HelperLibrary.Networking.ClientServer.Packets;
using System;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class RegistrationPacket : BasePacket
    {
        public readonly string Username;
        public readonly string Password;

        public RegistrationPacket(string username, string password, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Username = username;
            Password = password;
        }
    }
}
