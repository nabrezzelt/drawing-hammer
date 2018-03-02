using System;
using HelperLibrary.Networking.ClientServer.Packets;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class WordToDrawPacket : BasePacket
    {
        public Word WordToDraw { get; set; }

        public WordToDrawPacket(Word wordToDraw, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            WordToDraw = wordToDraw;
        }
    }
}