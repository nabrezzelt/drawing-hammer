using System;
using System.Windows;
using HelperLibrary.Networking.ClientServer;

namespace DrawingHammerDesktopApp
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SslClient _client;
        public MainWindow()
        {
            InitializeComponent();

            if (!Properties.Settings.Default.IsConnectionConfigured)
            {
                ConnectionSettingsWindow settingsWindow = new ConnectionSettingsWindow();
                settingsWindow.ShowDialog();
            }

            _client = new SslClient(Properties.Settings.Default.Host, true);

            _client.ConnectionLost += OnConnectionLost;
            _client.ConnectionSucceed += OnConnectionSucceed;
            _client.PacketReceived += OnPacketReceived;

            LoginWindow loginWindow = new LoginWindow(_client);
            loginWindow.ShowDialog();
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
    }
}
