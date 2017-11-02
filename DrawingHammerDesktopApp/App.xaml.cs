using System.Windows;
using HelperLibrary.Cryptography;

namespace DrawingHammerDesktopApp
{    
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public static string Uid;
        public static string Username;

        public App()
        {
            Uid = HashManager.GenerateSecureRandomToken(64);
        }
    }
}
