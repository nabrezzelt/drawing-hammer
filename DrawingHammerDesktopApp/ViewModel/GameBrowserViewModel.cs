using DrawingHammerPacketLibrary;
using System.Collections.ObjectModel;

namespace DrawingHammerDesktopApp.ViewModel
{
    public class GameBrowserViewModel : ViewModelBase
    {
        private ObservableCollection<Match> _matches;

        public ObservableCollection<Match> Matches
        {
            get => _matches;
            set
            {
                _matches = value;
                OnPropertyChanged();
            }
        }

        public GameBrowserViewModel()
        {
            _matches = new ObservableCollection<Match>(); 
        }

        public Match GetMatch(string matchUid)
        {
            foreach (var match in Matches)
            {
                if (match.Uid == matchUid)
                {
                    return match;
                }
            }

            return null;
        }
    }
}
