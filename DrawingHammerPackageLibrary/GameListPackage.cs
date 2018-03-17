using HelperLibrary.Networking.ClientServer.Packages;
using System;
using System.Collections.ObjectModel;

namespace DrawingHammerPackageLibrary
{
    [Serializable]
    public class GameListPackage : BasePackage
    {
        public ObservableCollection<MatchData> Matches { get; }

        public GameListPackage(ObservableCollection<MatchData> matches, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Matches = matches; 
        }

        public GameListPackage(string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Matches = new ObservableCollection<MatchData>();
        }
    }
}