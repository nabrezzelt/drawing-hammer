using System;
using HelperLibrary.Networking.ClientServer.Packets;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class DrawingAreaChangedPacket : BasePacket
    {
        public byte[] Strokes { get; set; }

        public string MatchUid { get; set; }

        public DrawingAreaChangedPacket(byte[] strokes, string matchUid, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Strokes = strokes;
            MatchUid = matchUid;
        }
    }
}