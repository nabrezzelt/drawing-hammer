using System;
using HelperLibrary.Networking.ClientServer.Packets;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class WordGuessCorrectPacket : BasePacket
    {
        public string PlayerUid { get; set; }                

        public WordGuessCorrectPacket(string playerUid, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            PlayerUid = playerUid;            
        }
    }
}