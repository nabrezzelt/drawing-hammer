using System;
using HelperLibrary.Networking.ClientServer.Packets;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class LeaveMatchPacket : BasePacket
    {
        public string MatchUid { get; }

        public LeaveMatchPacket(string matchUid, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            MatchUid = matchUid;
        }
    }
}