using System;
using HelperLibrary.Networking.ClientServer.Packages;

namespace DrawingHammerPackageLibrary
{
    /// <summary>
    /// Package-Class to tell requesting player that he cant join a match
    /// </summary>
    [Serializable]
    public class MatchJoinFailedPackage : BasePackage
    {
        public string Reason { get; }

        public MatchJoinFailedPackage(string reason, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Reason = reason;
        }
    }
}
