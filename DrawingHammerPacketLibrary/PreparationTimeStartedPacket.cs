using System;
using HelperLibrary.Networking.ClientServer.Packets;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class PreparationTimeStartedPacket : BasePacket
    {
        public Player PreparingPlayer { get; }

        public PreparationTimeStartedPacket(Player preparingPlayer, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            PreparingPlayer = preparingPlayer;
        }
    }
}