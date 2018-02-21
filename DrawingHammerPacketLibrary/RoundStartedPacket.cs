using System;
using HelperLibrary.Networking.ClientServer.Packets;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class RoundStartedPacket : BasePacket
    {
        public int RoundNumber { get; }

        public RoundStartedPacket(int roundNumber, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            RoundNumber = roundNumber;
        }
    }
}