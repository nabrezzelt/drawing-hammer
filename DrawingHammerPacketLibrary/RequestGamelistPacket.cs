using System;
using HelperLibrary.Networking.ClientServer.Packets;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class RequestGamelistPacket : BasePacket
    {
        public RequestGamelistPacket(string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {

        }
    }
}
