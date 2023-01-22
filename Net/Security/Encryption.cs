using System.IO;
using System.Security.Cryptography;
using System;

namespace Client.Net.Security
{
    public static class Encryption
    {
        //plain text msg is encrypted with AES, the IV is sent back to the caller so that it can be attached
        //as plain text to the start of the message. when decrypting the message on the server side - 
        //the server will first take the plain text IV from the start of the message and use it
        //to decrypt the rest of the message.
        //The key will be the public key of the server and which will be obtained when the client first connects to the server
        //this ensure a unique IV for every message sent.
        public static string EncryptDataWithAes(string plainText, byte[] key, out string iv)
        {
            using Aes aesAlgorithm = Aes.Create();
            aesAlgorithm.Key = key;
            //this implementation will change when paired with the key exchange algorithm
            //   key = Convert.ToBase64String(aesAlgorithm.Key);
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