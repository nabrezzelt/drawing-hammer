using HelperLibrary.Networking.ClientServer.Packets;
using System;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class AuthenticationPacket : BasePacket
    {
        public readonly string Username;
        public readonly string Password;

        public AuthenticationPacket(string username, string password, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Username = username;
            Password = password;
        }
    }
}
