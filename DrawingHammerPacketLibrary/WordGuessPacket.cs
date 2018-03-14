﻿using System;
using HelperLibrary.Networking.ClientServer.Packets;

namespace DrawingHammerPacketLibrary
{
    [Serializable]    
    public class WordGuessPacket : BasePacket
    {
        public string GuessedWord { get; }
        public string PlayerUid { get; }
        public string MatchUid { get; }

        public WordGuessPacket(string guessedWord, string matchUid, string playerUid, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            GuessedWord = guessedWord;
            PlayerUid = playerUid;
            MatchUid = matchUid;
        }
    }
}