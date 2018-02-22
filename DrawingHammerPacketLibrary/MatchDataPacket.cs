using HelperLibrary.Networking.ClientServer.Packets;
using System;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class MatchDataPacket : BasePacket
    {
        public MatchData MatchData { get; set; }

        public MatchDataPacket(MatchData matchData, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            MatchData = matchData;
        }
    }
}
