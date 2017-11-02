﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using HelperLibrary.Networking.ClientServer;

namespace DrawingHammerDesktopApp
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SslClient _client;

        public MainWindow()
        {
            InitializeComponent();

            //if (!Properties.Settings.Default.IsConnectionConfigured)
            //{
                ConnectionSettingsWindow settingsWindow = new ConnectionSettingsWindow();
                settingsWindow.ShowDialog();
            //}

            _client = new SslClient(Properties.Settings.Default.Host, true);

            _client.ConnectionLost += OnConnectionLost;
            _client.ConnectionSucceed += OnConnectionSucceed;
            _client.PacketReceived += OnPacketReceived;

            Connect();

            IsEnabled = false;
        }

        private async void Connect()
        {
            await Task.Run(() =>
            {
                _client.Connect(Properties.Settings.Default.Host, Properties.Settings.Default.Port);
            });
        }

        private void OnPacketReceived(object sender, PacketReceivedEventArgs e)
        {
            
        }

        private void OnConnectionSucceed(object sender, EventArgs e)
        {
            InvokeGui(() =>
            {
                IsEnabled = true;

                MessageBox.Show("Connected!");

                LoginWindow loginWindow = new LoginWindow(_client);
                loginWindow.ShowDialog();
            });
        }

        private void OnConnectionLost(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void SetUsername(string username)
        {
            throw new NotImplementedException();
        }

        private void InvokeGui(Action action)
        {           
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, action);           
        }
    }
}
