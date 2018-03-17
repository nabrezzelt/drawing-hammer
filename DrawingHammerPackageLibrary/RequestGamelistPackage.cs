using System;
using HelperLibrary.Networking.ClientServer.Packages;

namespace DrawingHammerPackageLibrary
{
    [Serializable]
    public class RequestGamelistPackage : BasePackage
    {
        public RequestGamelistPackage(string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {

        }
    }
}
