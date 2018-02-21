using System;
using DrawingHammerPacketLibrary.Enums;
using HelperLibrary.Logging;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class Player : ViewModelBase
    {
        private PlayerStatus _status;
        public int Id { get; set; }

        public string Uid { get; set; }

        public string Username { get; set; }

        public int Score { get; set; }

        public PlayerStatus Status { get; set; }
        //{
        //    get => _status;
        //    set
        //    {
        //        _status = value;
        //        Log.Info($"[{Username}] {Status.ToString()}");
        //    }
        //}

        public string StatusName => Status.ToString();

        public Player(int id, string uid, string username, int score)
        {
            Id = id;                              
            Uid = uid;
            Username = username;
            Score = score;
            Status = PlayerStatus.Guessing;
        }
    }
}