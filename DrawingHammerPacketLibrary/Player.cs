using System;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class Player : ViewModelBase
    {
        public int Id { get; set; }

        public string Uid { get; set; }

        public string Username { get; set; }

        public int Score { get; set; }

        public Player(int id, string uid, string username, int score)
        {
            Id = id;                              
            Uid = uid;
            Username = username;
            Score = score;
        }
    }
}