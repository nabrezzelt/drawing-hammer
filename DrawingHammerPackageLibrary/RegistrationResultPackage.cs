using HelperLibrary.Networking.ClientServer.Packages;
using System;
using DrawingHammerPackageLibrary.Enums;

namespace DrawingHammerPackageLibrary
{
    [Serializable]
    public class RegistrationResultPackage : BasePackage
    {
        public RegistrationResult Result { get; }
        
        public RegistrationResultPackage(RegistrationResult result, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Result = result;
        }
    }
}
