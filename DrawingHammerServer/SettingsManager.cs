using System;
using HelperLibrary.FileSystem;

namespace DrawingHammerServer
{
    class SettingsManager
    {
        private readonly IniFile _iniFile;

        private const string DatabaseSettingsSection = "Database Settings",
            StartupSettingsSection = "Startup Settings",
            GeneralSettingsSection = "General Settings",
            SslCertificateSection = "Certificate Settings";

        private const string
            DatabaseHostKey = "DatabaseHost",
            DatabaseUserKey = "DatabaseUser",
            DatabasePasswordKey = "DatabasePassword",
            DatabasenameKey = "Databasename",

            StartupIpKey = "StartupIp",
            StartupPortKey = "StartupPort",

            SslCertificatePathKey = "SslCertificatePath",
            SslCertificatePasswordKey = "SslCertificatePassword",

            MinUsernameLengthKey = "MinUsernameLength",
            MaxUsernameLengthKey = "MaxUsernameLength";


        public SettingsManager(string iniFilePath)
        {
            _iniFile = IniFile.OpenFile(iniFilePath);
        }

        public void InitializeSettingsFile()
        {
            //Database
            _iniFile.WriteValue(DatabaseHostKey, "", DatabaseSettingsSection);
            _iniFile.WriteValue(DatabaseUserKey, "", DatabaseSettingsSection);
            _iniFile.WriteValue(DatabasePasswordKey, "", DatabaseSettingsSection);
            _iniFile.WriteValue(DatabasenameKey, "", DatabaseSettingsSection);

            //Startup
            _iniFile.WriteValue(StartupIpKey, "", StartupSettingsSection);
            _iniFile.WriteValue(StartupPortKey, "9999", StartupSettingsSection);

            //Certificate
            _iniFile.WriteValue(SslCertificatePathKey, "", SslCertificateSection);
            _iniFile.WriteValue(SslCertificatePasswordKey, "", SslCertificateSection);

            //General
            _iniFile.WriteValue(MinUsernameLengthKey, "6", GeneralSettingsSection);
            _iniFile.WriteValue(MaxUsernameLengthKey, "24", GeneralSettingsSection);
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
            return _iniFile.ReadValue(StartupIpKey, StartupSettingsSection);
        }

        public int GetStartupPort()
        {
            if (!int.TryParse(_iniFile.ReadValue(StartupPortKey, StartupSettingsSection), out int port) && port > 0)
            {
                throw new FormatException("The Port-Key in your Settings-File must be a int and larger than zero!");
            }

            return port;
        }        

        public string GetSslCertificatePath()
        {
            return _iniFile.ReadValue(SslCertificatePathKey, SslCertificateSection);
        }

        public string GetSslCertificatePassword()
        {
            return _iniFile.ReadValue(SslCertificatePasswordKey, SslCertificateSection);
        }

        public int GetMinUsernameLength()
        {
            if (!int.TryParse(_iniFile.ReadValue(MinUsernameLengthKey, GeneralSettingsSection), out int minLength) && minLength > 0)
            {
                throw new FormatException("The MinUsernameLength-Key in your Settings-File must be a int and larger or equal to 1!");
            }

            return minLength;
        }

        public int GetMaxUsernameLength()
        {
            if (!int.TryParse(_iniFile.ReadValue(MaxUsernameLengthKey, GeneralSettingsSection), out int maxLength) && maxLength <= 64)
            {
                throw new FormatException("The MaxUsernameLength-Key in your Settings-File must be a int and smaller or equal to 64!");
            }

            return maxLength;            
        }
    }
}
