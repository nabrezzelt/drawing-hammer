using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DrawingHammerDesktopApp
{
    /// <summary>
    /// Interaktionslogik für ConnectionSettingsWindow.xaml
    /// </summary>
    public partial class ConnectionSettingsWindow
    {
        public ConnectionSettingsWindow()
        {
            InitializeComponent();

            TextBoxHost.Text = Properties.Settings.Default.Host;
            TextBoxPort.Text = Properties.Settings.Default.Port.ToString();
            CheckIfEnableSaveButton(new object(), null);
        }

        private void CheckIfEnableSaveButton(object sender, TextChangedEventArgs e)
        {
            if (TextBoxHost.Text != "" && TextBoxPort.Text != "" && int.TryParse(TextBoxPort.Text, out int _))
            {
                ButtonSave.IsEnabled = true;
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.IsConnectionConfigured = true;
            Properties.Settings.Default.Host = TextBoxHost.Text;
            Properties.Settings.Default.Port = Convert.ToInt32(TextBoxPort.Text);
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
