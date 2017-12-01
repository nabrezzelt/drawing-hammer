using DrawingHammerPacketLibrary;
using System.Collections.ObjectModel;

namespace DrawingHammerDesktopApp.ViewModel
{
    public class GameBrowserViewModel : ViewModelBase
    {
        public ObservableCollection<Match> Matches { get; set; }

        public GameBrowserViewModel()
        {
            Matches = new ObservableCollection<Match>(); 
        }

        public Match GetMatch(string matchUid)
        {
            foreach (var match in Matches)
            {
                if (match.MatchUid == matchUid)
                {
                    return match;
                }
            }

            return null;
        }
    }
}
