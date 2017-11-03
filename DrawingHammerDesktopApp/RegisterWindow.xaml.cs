using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DrawingHammerPacketLibrary;
using DrawingHammerPacketLibrary.Enums;
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
            _client.PacketReceived += OnPacketReceived;
        }

        private void OnPacketReceived(object sender, PacketReceivedEventArgs e)
        {
            switch (e.Packet)
            {
                case RegistrationResultPacket packet:
                    HandleRegisterResult(packet);
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

        private void HandleRegisterResult(RegistrationResultPacket packet)
        {
            InvokeGui(() =>
            {
                btnRegister.Visibility = Visibility.Visible;
                pbRegistering.Visibility = Visibility.Collapsed;
            });
            
            switch (packet.Result)
            {
                case RegistrationResult.Ok:
                    InvokeGui(async () =>
                    {
                        StatusSnackbar.MessageQueue.Enqueue("Account successfully created - you can login now!");
                        await TaskDelay();
                        Close();
                    });
                    break;
                case RegistrationResult.Failed:
                    InvokeGui(() =>
                    {
                        StatusSnackbar.MessageQueue.Enqueue("Registration failed!");
                    });
                    break;
                case RegistrationResult.UsernameTooLong:
                    InvokeGui(() =>
                    {
                        StatusSnackbar.MessageQueue.Enqueue("Your username it so long");
                    });
                    break;
                case RegistrationResult.UsernameTooShort:
                    InvokeGui(() =>
                    {
                        StatusSnackbar.MessageQueue.Enqueue("Your username is to short!");
                    });
                    break;
                case RegistrationResult.UsernameAlreadyExists:
                    InvokeGui(() =>
                    {
                        StatusSnackbar.MessageQueue.Enqueue("A user with this name already exitsts. Please choose another one.");
                    });
                    break;                
            }
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
            btnRegister.Visibility = Visibility.Collapsed;
            pbRegistering.Visibility = Visibility.Visible;

            SendRegisterMessage(tbUsername.Text, tbPassword.Password);
        }

        private async void SendRegisterMessage(string username, string password)
        {            
            await Task.Run(() =>
            {
                _client.SendPacketToServer(new RegistrationPacket(
                    username,
                    password,
                    App.Uid,
                    Router.ServerWildcard
                    ));
            });
        }

        private void RegisterWhenEnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && btnRegister.IsEnabled)
            {
                Register(sender, e);
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
