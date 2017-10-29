using System.Windows;

namespace DrawingHammerDesktopApp
{
    /// <summary>
    /// Interaktionslogik für RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void CheckForEnableRegisterButton(object sender, RoutedEventArgs routedEventArgs)
        {
            if (tbPassword.Password != "" && tbPassword.Password == tbPasswordRepeat.Password && tbUsername.Text != "")
            {
                btnRegister.IsEnabled = true;
            }
            else
            {
                btnRegister.IsEnabled = false;
            }
        }

        private void Register(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Register...");
        }
    }
}
