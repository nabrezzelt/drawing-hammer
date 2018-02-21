using System;
using System.Collections.Generic;
using HelperLibrary.Networking.ClientServer.Packets;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class PickWordsPacket : BasePacket
    {
        public IList<Word> WordsToSelect { get; set; }

        public PickWordsPacket(IList<Word> wordsToSelect, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            WordsToSelect = wordsToSelect;
        }
    }
}