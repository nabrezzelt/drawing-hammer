using DrawingHammerPacketLibrary;
using System.Collections.ObjectModel;
using System.Timers;

namespace DrawingHammerDesktopApp.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string MyUsername { get; set; }
        public string MatchTitle { get; set; }
        public string MatchUid {get; set;}
        public int CurrentRound { get; set; }
        public int Rounds { get; set; }
        public int RemainingTime { get; set; }        
        public int RoundLength { get; set;  }
        public ObservableCollection<Player> Players { get; set; }

        private readonly Timer _roundTimer;

        public MainWindowViewModel()
        {
            Players = new ObservableCollection<Player>();

            _roundTimer = new Timer(1000);
            _roundTimer.Elapsed += _roundTimer_Elapsed;
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
    }
}
