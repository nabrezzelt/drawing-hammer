using DrawingHammerPackageLibrary.Enums;
using System;

namespace DrawingHammerPackageLibrary
{
    [Serializable]
    public class Player : ViewModelBase
    {
        public int Id { get; }

        public string Uid { get; }

        public string Username { get; }

        public int Score { get; set; }

        public PlayerStatus Status { get; set; } 
        
        public bool HasGuessed { get; set; }

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