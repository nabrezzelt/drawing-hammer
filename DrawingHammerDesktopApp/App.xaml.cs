using HelperLibrary.Cryptography;

namespace DrawingHammerDesktopApp
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App
    {
        public static string Uid { get; private set; }
        public static string Username { get; set; }

        public App()
        {
            Uid = HashManager.GenerateSecureRandomToken(64);            
        }        
    }
}
