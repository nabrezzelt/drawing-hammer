using System;
using HelperLibrary.Networking.ClientServer.Packages;

namespace DrawingHammerPackageLibrary
{
    [Serializable]    
    public class WordGuessPackage : BasePackage
    {
        public string GuessedWord { get; }
        public string PlayerUid { get; }
        public string MatchUid { get; }

        public WordGuessPackage(string guessedWord, string matchUid, string playerUid, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            GuessedWord = guessedWord;
            PlayerUid = playerUid;
            MatchUid = matchUid;
        }
    }
}