using System;
using HelperLibrary.Networking.ClientServer.Packets;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class PickedWordPacket : BasePacket
    {
        public Word PickedWord { get; set; }
        public string MatchUid { get; set; }

        public PickedWordPacket(Word pickedWord, string matchUid, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            PickedWord = pickedWord;
            MatchUid = matchUid;
        }
    }
}