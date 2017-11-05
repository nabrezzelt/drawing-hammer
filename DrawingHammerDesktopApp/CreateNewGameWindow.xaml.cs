using System.Windows;
using System.Windows.Controls;

namespace DrawingHammerDesktopApp
{
    /// <summary>
    /// Interaktionslogik für CreateNewGameWindow.xaml
    /// </summary>
    public partial class CreateNewGameWindow : Window
    {
        public CreateNewGameWindow()
        {
            InitializeComponent();
        }

        private void CheckToEnableCreateButton(object sender, TextChangedEventArgs e)
        {
            btnCreate.IsEnabled = tbGameName.Text != "";
        }
    }
}
