using System;
using DrawingHammerPackageLibrary;
using System.Collections.ObjectModel;
using System.Timers;
using MaterialDesignThemes.Wpf;

namespace DrawingHammerDesktopApp.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string MyUsername { get; set; }     
        
        public string MatchUid {get; set;}

        public int CurrentRound { get; set; }

        public int Rounds { get; set; }

        public int RemainingTime { get; set; }       
        
        public int RoundLength { get; set;  }

        public ObservableCollection<Player> Players { get; set; }

        public bool CanDraw { get; set; }

        public bool CanGuess { get; set; }

        public Word WordToDraw { get; set; }

        public ObservableCollection<Word> Words { get; set; }

        public ObservableCollection<Guess> Guesses { get; set; }

        public SnackbarMessageQueue MessageQueue { get; }

        private readonly Timer _roundTimer;

        public MainWindowViewModel()
        {
            Guesses = new ObservableCollection<Guess>();
            Players = new ObservableCollection<Player>();

            _roundTimer = new Timer(1000);
            _roundTimer.Elapsed += _roundTimer_Elapsed;

            MessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(5000));
        }

        private void _roundTimer_Elapsed(object sender, ElapsedEventArgs e)
        {    
            RemainingTime--;

            if (RemainingTime <= 0)
            {
                _roundTimer.Stop();
                RemainingTime = RoundLength;                
            }
        }

        public void ResetTimer()
        {            
            _roundTimer.Stop();
            RemainingTime = RoundLength;
        }

        public void StartTimer()
        {
            _roundTimer.Start();
        }

        public void Reset()
        {
            ResetTimer();
            WordToDraw = null;
            Rounds = 0;
            CurrentRound = 0;
            RemainingTime = 0;
            Players.Clear();
            Guesses.Clear();
            MatchUid = String.Empty;             
        }
    }
}
