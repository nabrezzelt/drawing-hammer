using System.Collections.Generic;
using HelperLibrary.Networking.ClientServer.Packets;

namespace DrawingHammerPacketLibrary
{
    public class GameListPacket : BasePacket
    {
        public List<Match> Matches; 

        public GameListPacket(List<Match> matches, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Matches = matches; 
        }

        public GameListPacket(string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Matches = new List<Match>();
        }
    }
}