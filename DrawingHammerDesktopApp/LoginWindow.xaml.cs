using System;
using System.Windows;
using System.Windows.Input;
using HelperLibrary.Networking.ClientServer;

namespace DrawingHammerDesktopApp
{
    /// <summary>
    /// Interaktionslogik für LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly SslClient _client;
        public LoginWindow(SslClient client)
        {
            InitializeComponent();

            _client = client;

            _client.ConnectionLost += OnConnectionLost;
            _client.ConnectionSucceed += OnConnectionSucceed;
            _client.PacketReceived += OnPacketReceived;
        }

        private void OnPacketReceived(object sender, PacketReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnConnectionSucceed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnConnectionLost(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ShowRegisterWindow(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow(_client);
            registerWindow.ShowDialog();
        }

        private void Login(object sender, RoutedEventArgs routedEventArgs)
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

        private void LoginWhenEnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && btnLogin.IsEnabled)
            {
                Login(sender, e);
            }
        }
    }
}
