using HelperLibrary.FileSystem;

namespace DrawingHammerServer
{
    class SettingsManager
    {
        private readonly IniFile _iniFile;

        private const string DatabaseSettingsSection = "Database Settings",
            GeneralSettingsSection = "General Settings",
            SslCertificateSection = "Certificate Settings";

        private const string
            DatabaseHostKey = "DatabaseHost",
            DatabaseUserKey = "DatabaseUser",
            DatabasePasswordKey = "DatabasePassword",
            DatabasenameKey = "Databasename",
            StartupIpKey = "StartupIp",
            SslCertificatePathKey = "SslCertificatePath",
            SslCertificatePasswordKey = "SslCertificatePassword";

        public SettingsManager(string iniFilePath)
        {
            _iniFile = IniFile.OpenFile(iniFilePath);
        }

        public void InitializeSettingsFile()
        {
            _iniFile.WriteValue(DatabaseHostKey, "", DatabaseSettingsSection);
            _iniFile.WriteValue(DatabaseUserKey, "", DatabaseSettingsSection);
            _iniFile.WriteValue(DatabasePasswordKey, "", DatabaseSettingsSection);
            _iniFile.WriteValue(DatabasenameKey, "", DatabaseSettingsSection);
            _iniFile.WriteValue(StartupIpKey, "", GeneralSettingsSection);
            _iniFile.WriteValue(SslCertificatePathKey, "", SslCertificateSection);
            _iniFile.WriteValue(SslCertificatePasswordKey, "", SslCertificateSection);
        }

        public string GetDatabaseHost()
        {
            return _iniFile.ReadValue(DatabaseHostKey, DatabaseSettingsSection);
        }

        public string GetDatabaseUser()
        {
            return _iniFile.ReadValue(DatabaseUserKey, DatabaseSettingsSection);
        }

        public string GetDatabasePassword()
        {
            return _iniFile.ReadValue(DatabasePasswordKey, DatabaseSettingsSection);
        }

        public string GetDatabasename()
        {
            return _iniFile.ReadValue(DatabasenameKey, DatabaseSettingsSection);
        }

        public string GetStartupIp()
        {
            return _iniFile.ReadValue(StartupIpKey, GeneralSettingsSection);
        }

        public string GetSslCertificatePath()
        {
            return _iniFile.ReadValue(SslCertificatePathKey, SslCertificateSection);
        }

        public string GetSslCertificatePassword()
        {
            return _iniFile.ReadValue(SslCertificatePasswordKey, SslCertificateSection);
        }
    }
}
