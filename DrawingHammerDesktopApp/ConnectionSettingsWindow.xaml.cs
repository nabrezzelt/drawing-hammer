using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DrawingHammerDesktopApp
{
    /// <summary>
    /// Interaktionslogik für ConnectionSettingsWindow.xaml
    /// </summary>
    public partial class ConnectionSettingsWindow : Window
    {
        public ConnectionSettingsWindow()
        {
            InitializeComponent();

            tbHost.Text = Properties.Settings.Default.Host;
            tbPort.Text = Properties.Settings.Default.Port.ToString();
            CheckIfEnableSaveButton(new object(), null);
        }

        private void CheckIfEnableSaveButton(object sender, TextChangedEventArgs e)
        {
            if (tbHost.Text != "" && tbPort.Text != "" && int.TryParse(tbPort.Text, out int port))
            {
                btnSave.IsEnabled = true;
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.IsConnectionConfigured = true;
            Properties.Settings.Default.Host = tbHost.Text;
            Properties.Settings.Default.Port = Convert.ToInt32(tbPort.Text);
            Properties.Settings.Default.Save();

            Close();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if(!Properties.Settings.Default.IsConnectionConfigured)
                Environment.Exit(0);
        }
    }
}
