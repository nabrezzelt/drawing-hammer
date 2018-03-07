using System;
using HelperLibrary.Networking.ClientServer.Packets;

namespace DrawingHammerPacketLibrary
{
    /// <summary>
    /// Package-Class to tell requesting player that he cant join a match
    /// </summary>
    [Serializable]
    public class MatchJoinFailedPacket : BasePacket
    {
        public string Reason { get; }

        public MatchJoinFailedPacket(string reason, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Reason = reason;
        }
    }
}
