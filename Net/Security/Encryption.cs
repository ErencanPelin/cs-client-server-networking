using System.IO;
using System.Security.Cryptography;
using System;

namespace Client.Net.Security
{
    public static class Encryption
    {
        public static string EncryptDataWithAes(string plainText, out string key, out string iv)
        {
            using Aes aesAlgorithm = Aes.Create();
            //this implementation will change when paired with the key exchange algorithm
            key = Convert.ToBase64String(aesAlgorithm.Key);
            iv = Convert.ToBase64String(aesAlgorithm.IV);

            // Create encryptor object
            ICryptoTransform encryptor = aesAlgorithm.CreateEncryptor();

            byte[] encryptedData;

            //Encryption will be done in a memory stream through a CryptoStream object
            using (MemoryStream ms = new())
            {
                using CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write);
                using (StreamWriter sw = new(cs))
                {
                    sw.Write(plainText);
                }
                encryptedData = ms.ToArray();
            }

            return Convert.ToBase64String(encryptedData);
        }

        public static string DecryptDataWithAes(string cipherText, string key, string iv)
        {
            using Aes aesAlgorithm = Aes.Create();
            //this implementation is subject to change when the key exchange algorithm is complete
            aesAlgorithm.Key = Convert.FromBase64String(key);
            aesAlgorithm.IV = Convert.FromBase64String(iv);

            // Create decryptor object
            ICryptoTransform decryptor = aesAlgorithm.CreateDecryptor();

            byte[] cipher = Convert.FromBase64String(cipherText);

            //Decryption will be done in a memory stream through a CryptoStream object
            using MemoryStream ms = new(cipher);
            using CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read);
            using StreamReader sr = new(cs);
            return sr.ReadToEnd();
        }
    }
}