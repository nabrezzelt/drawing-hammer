using System;
using HelperLibrary.Networking.ClientServer.Packets;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    [ToString]
    public class ScoreChangedPacket : BasePacket
    {
        public string PlayerUid { get; set; }

        public int RaisedScore { get; set; }

        public ScoreChangedPacket(string playerUid, int raisedScore, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            PlayerUid = playerUid;
            RaisedScore = raisedScore;
        }
    }
}