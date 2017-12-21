﻿using System;

namespace DrawingHammerPacketLibrary
{
    public class PreparationTimerStartedEventArgs : EventArgs
    {
        public Player Player { get; }

        public PreparationTimerStartedEventArgs(Player player)
        {
            Player = player;            
        }
    }
}