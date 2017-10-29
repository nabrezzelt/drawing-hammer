using System.Windows;

namespace DrawingHammerDesktopApp
{
    /// <summary>
    /// Interaktionslogik für LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void ShowRegisterWindow(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.ShowDialog();
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Logging in...");
        }

        private void CheckForEnablingLoginButton(object sender, RoutedEventArgs routedEventArgs)
        {
            if (tbPassword.Password != "" && tbUsername.Text != "")
            {
                btnLogin.IsEnabled = true;
            }
            else
            {
                btnLogin.IsEnabled = false;
            }
        }
    }
}
