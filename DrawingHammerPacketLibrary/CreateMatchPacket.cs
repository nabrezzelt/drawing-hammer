using HelperLibrary.Networking.ClientServer.Packets;
using System;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class CreateMatchPacket : BasePacket
    {
        public MatchData MatchData { get; }

        public CreateMatchPacket(MatchData matchData, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            MatchData = matchData;
        }
    }
}
