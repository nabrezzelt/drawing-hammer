using System;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class Player : ViewModelBase
    {
        private int _id;
        private string _username;
        private string _score;
        private string _uid;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public string Uid
        {
            get => _uid;
            set
            {
                _uid = value;
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }        

        public string Score
        {
            get => _score;
            set
            {
                _score = value;
                OnPropertyChanged();
            }
        }

        public Player(int id, string uid, string username, string score)
        {
            _id = id;                              
            Uid = uid;
            Username = username;
            Score = score;
        }
    }
}