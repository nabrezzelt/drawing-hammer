using HelperLibrary.Networking.ClientServer.Packets;
using System;
using System.Collections.ObjectModel;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class GameListPacket : BasePacket
    {
        public ObservableCollection<Match> Matches; 

        public GameListPacket(ObservableCollection<Match> matches, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Matches = matches; 
        }

        public GameListPacket(string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Matches = new ObservableCollection<Match>();
        }
    }
}