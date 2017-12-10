using System;
using HelperLibrary.Networking.ClientServer.Packets;

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
