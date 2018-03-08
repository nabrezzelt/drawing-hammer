using System;
using HelperLibrary.Networking.ClientServer.Packets;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class WordSolutionPacket : BasePacket
    {
        public Word Word { get; }

        public WordSolutionPacket(Word word, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Word = word;
        }
    }
}