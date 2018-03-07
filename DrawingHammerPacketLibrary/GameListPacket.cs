using HelperLibrary.Networking.ClientServer.Packets;
using System;
using System.Collections.ObjectModel;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class GameListPacket : BasePacket
    {
        public ObservableCollection<MatchData> Matches { get; }

        public GameListPacket(ObservableCollection<MatchData> matches, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Matches = matches; 
        }

        public GameListPacket(string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Matches = new ObservableCollection<MatchData>();
        }
    }
}