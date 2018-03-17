using System;
using HelperLibrary.Networking.ClientServer.Packages;

namespace DrawingHammerPackageLibrary
{
    [Serializable]
    public class DrawingAreaChangedPackage : BasePackage
    {
        public byte[] Strokes { get; }

        public string MatchUid { get; }

        public DrawingAreaChangedPackage(byte[] strokes, string matchUid, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Strokes = strokes;
            MatchUid = matchUid;
        }
    }
}