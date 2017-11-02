using DrawingHammerPacketLibrary;
using HelperLibrary.Networking.ClientServer;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

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
            _client.PacketReceived += OnPacketReceived;
        }

        private void OnPacketReceived(object sender, PacketReceivedEventArgs e)
        {
            switch (e.Packet)
            {
                case AuthenticationResultPacket p:
                    HandleAuthenticationResult(p);
                    break;
            }
        }

        private void HandleAuthenticationResult(AuthenticationResultPacket packet)
        {
            InvokeGui(() =>
            {
                spButtons.Visibility = Visibility.Visible;
                spLoggingIn.Visibility = Visibility.Collapsed;
            });

            
            switch (packet.Result)
            {
                case AuthenticationResult.Ok:
                    InvokeGui(() =>
                    {
                        StatusSnackbar.MessageQueue.Enqueue("Login was successfull.");
                        //ToDo: Close(); and check why connection is killed when this windows closed
                    });
                    break;
                case AuthenticationResult.Failed:
                    InvokeGui(() =>
                    {
                        StatusSnackbar.MessageQueue.Enqueue("Login failed. Check username and/or password and try it again.");
                    });
                    break;                
            }            
        }

        private void OnConnectionLost(object sender, EventArgs e)
        {
            InvokeGui(() =>
            {
                StatusSnackbar.MessageQueue.Enqueue("Connection lost!");
            });
            
        }

        private void ShowRegisterWindow(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow(_client);
            registerWindow.ShowDialog();
        }

        private void Login(object sender, RoutedEventArgs routedEventArgs)
        {           
            
            InvokeGui(() =>
            {
                StatusSnackbar.MessageQueue.Enqueue("Logging in...");
                btnLogin.IsEnabled = false;
            });

            SendLoginMessage(tbUsername.Text, tbPassword.Password);

            InvokeGui(() =>
            {
                btnLogin.IsEnabled = true;

                spButtons.Visibility = Visibility.Collapsed;
                spLoggingIn.Visibility = Visibility.Visible;
            });            
        }

        private async void SendLoginMessage(string username, string password)
        {            
            App.Username = username;

            await Task.Run(() =>
            {                
                var loginPacket = new AuthenticationPacket(
                    username, 
                    password, 
                    App.Uid, 
                    Router.ServerWildcard);
                
                _client.SendPacketToServer(loginPacket);
            });
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

        private void OnClosing(object sender, CancelEventArgs e)
        {
            Environment.Exit(0);   
        }

        private void InvokeGui(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, action);
        }
    }
}
