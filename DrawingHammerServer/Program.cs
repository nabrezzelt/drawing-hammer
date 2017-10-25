using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace DrawingHammerServer
{
    class Program
    {
        private static DrawingHammerServer _server;

        private static void Main(string[] args)
        {
            _server = new DrawingHammerServer(6666);

            _server.ClientConnected += OnClientConnected;
            _server.PacketReceived += OnPacketReceived;
            _server.ClientDisconnected += OnClientDisconnected;


        }

        private static void OnClientDisconnected(object sender, HelperLibrary.Networking.ClientServer.ClientDisconnectedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void OnPacketReceived(object sender, HelperLibrary.Networking.ClientServer.PacketReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void OnClientConnected(object sender, HelperLibrary.Networking.ClientServer.ClientConnectedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
