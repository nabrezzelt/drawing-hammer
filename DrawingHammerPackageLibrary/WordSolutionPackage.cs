using System;
using HelperLibrary.Networking.ClientServer.Packages;

namespace DrawingHammerPackageLibrary
{
    [Serializable]
    public class WordSolutionPackage : BasePackage
    {
        public Word Word { get; }

        public WordSolutionPackage(Word word, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Word = word;
        }
    }
}