using HelperLibrary.Networking.ClientServer.Packages;
using System;

namespace DrawingHammerPackageLibrary
{
    [Serializable]
    public class RegistrationPackage : BasePackage
    {
        public string Username { get; }
        public string Password { get; }

        public RegistrationPackage(string username, string password, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Username = username;
            Password = password;
        }
    }
}
