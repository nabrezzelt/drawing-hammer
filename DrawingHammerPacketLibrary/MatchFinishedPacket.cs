using HelperLibrary.Networking.ClientServer.Packets;
using System;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class MatchFinishedPacket : BasePacket
    {
        public string MatchUid { get; }

        public MatchFinishedPacket(string matchUid, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            MatchUid = matchUid;
        }
    }
}