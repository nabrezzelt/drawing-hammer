using HelperLibrary.Networking.ClientServer.Packets;
using System;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class AuthenticationPacket : BasePacket
    {
        public string Username { get; }
        public string Password { get; }

        public AuthenticationPacket(string username, string password, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Username = username;
            Password = password;
        }
    }
}
