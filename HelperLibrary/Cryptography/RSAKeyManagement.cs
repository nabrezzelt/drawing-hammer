﻿using System;
using System.Security.Cryptography;
using System.Text;

namespace HelperLibrary.Cryptography
{
    public class RsaKeyManagement
    {
        public readonly int MaxStringLength;

        public readonly KeySize KeySize;

        public readonly string PublicKey;

        public readonly string PrivateKey;

        public RsaKeyManagement(KeySize keySize = KeySize.Size2048)
        {
            var keyPair = CreateKeyPair(keySize);

            KeySize = keySize;
            PrivateKey = keyPair.Item1;
            PublicKey = keyPair.Item2;

            switch (keySize)
            {
                case KeySize.Size512:
                    MaxStringLength = 53;
                    break;
                case KeySize.Size1024:
                    MaxStringLength = 117;
                    break;
                case KeySize.Size2048:
                    MaxStringLength = 245;
                    break;
                case KeySize.Size4096:
                    MaxStringLength = 501;
                    break;
                case KeySize.Size8192:
                    MaxStringLength = 1013;
                    break;
                case KeySize.Size16384:
                    MaxStringLength = 2037;
                    break;                
            }
        }

        /// <summary>
        /// Generates a RSA-Key Pair - 1: Private Key, 2: Public Key
        /// </summary>
        /// <returns>1: Private Key, 2: Public Key</returns>
        private Tuple<string, string> CreateKeyPair(KeySize size)
        {
            CspParameters cspParams = new CspParameters { ProviderType = 1 };
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(Convert.ToInt32(size), cspParams);

            string publicKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(false));
            string privateKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(true));

            return new Tuple<string, string>(privateKey, publicKey);
        }

        /// <summary>
        /// Encrypt a string with the given PublicKey.
        /// </summary>
        /// <param name="message">Messagedata</param>
        /// <returns>Encrypted string as Byte-Array</returns>
        public byte[] EncryptString(string message)
        {
            if (message.Length <= MaxStringLength)
            {
                CspParameters cspParams = new CspParameters { ProviderType = 1 };
                RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(cspParams);

                rsaProvider.ImportCspBlob(Convert.FromBase64String(PublicKey));
                byte[] plainBytes = Encoding.UTF8.GetBytes(message);
                byte[] encryptedBytes = rsaProvider.Encrypt(plainBytes, false);

                return encryptedBytes;
            }

            throw new Exception("The string to encrypt must be less or equal to " + MaxStringLength);
        }

        /// <summary>
        /// Decrypt a string with the given PrivateKey.
        /// </summary>
        /// <param name="encryptedMessage">Messagedata</param>
        /// <returns>Decrypted string</returns>
        public string DecryptString(byte[] encryptedMessage)
        {
            CspParameters cspParams = new CspParameters { ProviderType = 1 };
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(cspParams);

            rsaProvider.ImportCspBlob(Convert.FromBase64String(PrivateKey));
            byte[] plainBytes = rsaProvider.Decrypt(encryptedMessage, false);
            string plainText = Encoding.UTF8.GetString(plainBytes, 0, plainBytes.Length);

            return plainText;
        }
    }
}
