using HelperLibrary.Networking.ClientServer.Packets;
using System;
using HelperLibrary.Networking.ClientServer;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class AuthenticationPacket : BasePacket
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public AuthenticationPacket(string username, string password, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Username = username;
            Password = password;
        }
    }
}
