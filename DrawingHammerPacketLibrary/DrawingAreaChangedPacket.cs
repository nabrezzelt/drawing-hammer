using System;
using HelperLibrary.Networking.ClientServer.Packets;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class DrawingAreaChangedPacket : BasePacket
    {
        public byte[] Strokes { get; }

        public string MatchUid { get; }

        public DrawingAreaChangedPacket(byte[] strokes, string matchUid, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Strokes = strokes;
            MatchUid = matchUid;
        }
    }
}