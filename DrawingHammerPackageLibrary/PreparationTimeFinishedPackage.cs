using HelperLibrary.Networking.ClientServer.Packages;
using System;

namespace DrawingHammerPackageLibrary
{
    [Serializable]
    public class PreparationTimeFinishedPackage : BasePackage
    {
        public PreparationTimeFinishedPackage(string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
        }
    }
}
