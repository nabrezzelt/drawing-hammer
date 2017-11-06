using DrawingHammerPacketLibrary;
using System.Collections.ObjectModel;

namespace DrawingHammerDesktopApp.ViewModel
{
    class GameBrowserViewModel : ViewModelBase
    {
        private ObservableCollection<Match> _matches;

        public ObservableCollection<Match> Matches => _matches;

        public GameBrowserViewModel()
        {
            _matches = new ObservableCollection<Match>
            {
                new Match("Faggot 8797", 4, 13, 4),
                new Match("Test kjshg", 4, 12, 4),
                new Match("Faggot alksjdlajlkdlasjd", 4, 48, 4),
                new Match("Test 132", 4, 9, 4)
        };
        }
    }
}
