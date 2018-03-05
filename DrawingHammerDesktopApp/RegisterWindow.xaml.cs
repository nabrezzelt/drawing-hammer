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
    public partial class RegisterWindow
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
            InvokeGui(() =>
            {
                switch (e.Packet)
                {
                    case RegistrationResultPacket packet:
                        HandleRegisterResult(packet);
                        break;
                }
            });
        }

        private void OnConnectionLost(object sender, EventArgs e)
        {
            StatusSnackbar.MessageQueue.Enqueue("Connection lost!");
        }

        private void HandleRegisterResult(RegistrationResultPacket packet)
        {
            ButtonRegister.Visibility = Visibility.Visible;
            ProgressBarRegistering.Visibility = Visibility.Collapsed;

            switch (packet.Result)
            {
                case RegistrationResult.Ok:
                    Task.Run(async () =>
                    {
                        StatusSnackbar.MessageQueue.Enqueue("Account successfully created - you can login now!");
                        ButtonRegister.IsEnabled = false;

                        await TaskDelay(2000);
                        Close();
                    });
                    break;
                case RegistrationResult.Failed:
                    StatusSnackbar.MessageQueue.Enqueue("Registration failed!");
                    break;
                case RegistrationResult.UsernameTooLong:
                    StatusSnackbar.MessageQueue.Enqueue("Your username it so long");
                    break;
                case RegistrationResult.UsernameTooShort:
                    StatusSnackbar.MessageQueue.Enqueue("Your username is to short!");
                    break;
                case RegistrationResult.UsernameAlreadyExists:
                    StatusSnackbar.MessageQueue.Enqueue("A user with this name already exitsts. Please choose another one.");
                    break;
            }
        }

        private void CheckForEnableRegisterButton(object sender, RoutedEventArgs routedEventArgs)
        {
            if (TextBoxPassword.Password != "" && TextBoxPassword.Password == TextBoxPasswordRepeat.Password && TextBoxUsername.Text != "")
            {
                ButtonRegister.IsEnabled = true;
            }
            else
            {
                ButtonRegister.IsEnabled = false;
            }
        }

        private void Register(object sender, RoutedEventArgs e)
        {
            ButtonRegister.Visibility = Visibility.Collapsed;
            ProgressBarRegistering.Visibility = Visibility.Visible;

            SendRegisterMessage(TextBoxUsername.Text, TextBoxPassword.Password);
        }

        private void SendRegisterMessage(string username, string password)
        {
            _client.SendPacketToServer(new RegistrationPacket(
                username,
                password,
                App.Uid,
                Router.ServerWildcard
                ));
        }

        private void RegisterWhenEnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && ButtonRegister.IsEnabled)
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
