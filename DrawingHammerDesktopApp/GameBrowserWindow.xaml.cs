using System.Windows;

namespace DrawingHammerDesktopApp
{
    /// <summary>
    /// Interaktionslogik für GameBrowserWindow.xaml
    /// </summary>
    public partial class GameBrowserWindow : Window
    {
        public GameBrowserWindow()
        {
            InitializeComponent();
        }

        private void CreateNewGame(object sender, RoutedEventArgs e)
        {
            CreateNewGameWindow createGameWindow = new CreateNewGameWindow();
            createGameWindow.ShowDialog();
        }
    }
}
