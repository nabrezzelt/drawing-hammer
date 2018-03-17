using DrawingHammerPackageLibrary;
using System.Collections.ObjectModel;

namespace DrawingHammerDesktopApp.ViewModel
{
    public class GameBrowserViewModel : ViewModelBase
    {
        public ObservableCollection<MatchData> Matches { get; set; }

        public GameBrowserViewModel()
        {
            Matches = new ObservableCollection<MatchData>(); 
        }

        public MatchData GetMatch(string matchUid)
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
