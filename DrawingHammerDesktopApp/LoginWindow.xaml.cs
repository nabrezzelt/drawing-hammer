using DrawingHammerPacketLibrary;
using HelperLibrary.Networking.ClientServer;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DrawingHammerPacketLibrary.Enums;

namespace DrawingHammerDesktopApp
{
    /// <summary>
    /// Interaktionslogik für LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private bool LoggedIn { get; set; }

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
                StackPanelButtons.Visibility = Visibility.Visible;
                StackPanelLoggingIn.Visibility = Visibility.Collapsed;
            });

            
            switch (packet.Result)
            {
                case AuthenticationResult.Ok:
                    LoggedIn = true;
                    InvokeGui(async () =>
                    {                        
                        StatusSnackbar.MessageQueue.Enqueue("Login was successfull.");
                        ButtonLogin.IsEnabled = false;
                        ButtonRegister.IsEnabled = false;

                        await TaskDelay(1500);
                        Close();
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
            SendLoginMessage(TextBoxUsername.Text, TextBoxPassword.Password);

            InvokeGui(() =>
            {                
                StackPanelButtons.Visibility = Visibility.Collapsed;
                StackPanelLoggingIn.Visibility = Visibility.Visible;
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
            if (TextBoxPassword.Password != "" && TextBoxUsername.Text != "")
            {
                ButtonLogin.IsEnabled = true;
            }
            else
            {
                ButtonLogin.IsEnabled = false;
            }
        }

        private void LoginWhenEnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && ButtonLogin.IsEnabled)
            {
                Login(sender, e);
            }
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (!LoggedIn)
            {
                Environment.Exit(0);
            }            
        }        

        private void InvokeGui(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, action);
        }

        private async Task TaskDelay(int seconds = 3000)
        {
            await Task.Delay(seconds);
        }
    }
}
