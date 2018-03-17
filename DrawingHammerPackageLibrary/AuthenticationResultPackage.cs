using System;
using DrawingHammerPackageLibrary.Enums;
using HelperLibrary.Networking.ClientServer.Packages;

namespace DrawingHammerPackageLibrary
{
    [Serializable]
    public class AuthenticationResultPackage : BasePackage
    {
        public AuthenticationResult Result { get; }

        public AuthenticationResultPackage(AuthenticationResult result, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Result = result;
        }
    }
}
