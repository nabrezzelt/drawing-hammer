using System;
using HelperLibrary.Networking.ClientServer.Packages;

namespace DrawingHammerPackageLibrary
{
    [Serializable]    
    public class WordGuessCorrectPackage : BasePackage
    {
        public string PlayerUid { get; }                

        public WordGuessCorrectPackage(string playerUid, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            PlayerUid = playerUid;            
        }
    }
}