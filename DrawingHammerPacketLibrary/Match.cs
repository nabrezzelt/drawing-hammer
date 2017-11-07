using HelperLibrary.Cryptography;
using System.Collections.ObjectModel;

namespace DrawingHammerPacketLibrary
{
    public class Match : ViewModelBase
    {
        //public EventHandler<EventArgs> RoundFinished;
        //public EventHandler<EventArgs> MatchFinished;
        private string _title;
        private int _rounds;
        private int _maxPlayers;
        private int _roundLengh;
        private ObservableCollection<Player> _players;
        private ObservableCollection<Word> _guessedWords;
        private int _currentTime;
        private int _currentRound;
        private string _uid;

        public string Uid
        {
            get => _uid;
            set
            {
                _uid = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public int Rounds
        {
            get => _rounds;
            set
            {
                _rounds = value;
                OnPropertyChanged();
            }
        }

        public int MaxPlayers
        {
            get => _maxPlayers;
            set
            {
                _maxPlayers = value;
                OnPropertyChanged();
            }
        }

        public int RoundLengh
        {
            get => _roundLengh;
            set
            {
                _roundLengh = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Player> Players
        {
            get => _players;
            set
            {
                _players = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Word> GuessedWords
        {
            get => _guessedWords;
            set
            {
                _guessedWords = value;
                OnPropertyChanged();
            }
        }

        public int CurrentTime
        {
            get => _currentTime;
            set
            {
                _currentTime = value;
                OnPropertyChanged();
            }
        }

        public int CurrentRound
        {
            get => _currentRound;
            set
            {
                _currentRound = value;
                OnPropertyChanged();
            }
        }

        public Match(string title, int rounds, int maxPlayers, int roundLengh)
        {
            _uid = HashManager.GenerateSecureRandomToken();

            Title = title;
            Rounds = rounds;
            MaxPlayers = maxPlayers;
            RoundLengh = roundLengh;

            Players = new ObservableCollection<Player>();
            CurrentTime = 0;
            CurrentRound = 1;
        }
    }
}
