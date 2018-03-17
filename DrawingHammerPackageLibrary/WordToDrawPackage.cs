using System;
using HelperLibrary.Networking.ClientServer.Packages;

namespace DrawingHammerPackageLibrary
{
    [Serializable]
    public class WordToDrawPackage : BasePackage
    {
        public Word WordToDraw { get; }

        public WordToDrawPackage(Word wordToDraw, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            WordToDraw = wordToDraw;
        }
    }
}