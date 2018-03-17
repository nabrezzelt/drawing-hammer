using System;
using System.Collections.Generic;
using HelperLibrary.Networking.ClientServer.Packages;

namespace DrawingHammerPackageLibrary
{
    [Serializable]
    public class PickWordsPackage : BasePackage
    {
        public IList<Word> WordsToSelect { get; }

        public PickWordsPackage(IList<Word> wordsToSelect, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            WordsToSelect = wordsToSelect;
        }
    }
}