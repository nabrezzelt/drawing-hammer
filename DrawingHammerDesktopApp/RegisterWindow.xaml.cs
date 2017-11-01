using System;
using System.Windows;
using System.Windows.Input;
using HelperLibrary.Networking.ClientServer;

namespace DrawingHammerDesktopApp
{
    /// <summary>
    /// Interaktionslogik für RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private readonly SslClient _client;
        public RegisterWindow(SslClient client)
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

        private void RegisterWhenEnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && btnRegister.IsEnabled)
            {
                Register(sender, e);
            }
        }
    }
}
