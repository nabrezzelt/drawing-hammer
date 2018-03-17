using System;
using HelperLibrary.Networking.ClientServer.Packages;

namespace DrawingHammerPackageLibrary
{
    [Serializable]
    public class PickedWordPackage : BasePackage
    {
        public Word PickedWord { get; }
        public string MatchUid { get; }

        public PickedWordPackage(Word pickedWord, string matchUid, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            PickedWord = pickedWord;
            MatchUid = matchUid;
        }
    }
}