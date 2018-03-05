using System;
using HelperLibrary.Networking.ClientServer.Packets;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    [ToString]
    public class WordGuessPacket : BasePacket
    {
        public string GuessedWord { get; set; }
        public string PlayerUid { get; set; }
        public string MatchUid { get; set; }

        public WordGuessPacket(string guessedWord, string matchUid, string playerUid, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            GuessedWord = guessedWord;
            PlayerUid = playerUid;
            MatchUid = matchUid;
        }
    }
}