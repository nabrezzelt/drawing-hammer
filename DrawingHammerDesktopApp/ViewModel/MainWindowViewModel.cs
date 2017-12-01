using System.Collections.ObjectModel;
using DrawingHammerPacketLibrary;

namespace DrawingHammerDesktopApp.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string MyUsername { get; set; }
        public string MatchTitle { get; set; }
        public int CurrentRound { get; set; }
        public int Rounds { get; set; }
        public int RemainingTime { get; set; }
        public ObservableCollection<Player> Players { get; set; }
    }
}
