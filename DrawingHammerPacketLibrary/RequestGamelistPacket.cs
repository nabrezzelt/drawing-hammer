using HelperLibrary.Networking.ClientServer.Packets;

namespace DrawingHammerPacketLibrary
{
    public class RequestGamelistPacket : BasePacket
    {
        protected RequestGamelistPacket(string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {

        }
    }
}
