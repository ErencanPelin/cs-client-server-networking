using System.IO;
using System.Security.Cryptography;
using System;

namespace Client.Net.Security
{
    public static class Encryption
    {
        /* plain text msg is encrypted with AES, the IV is sent back to the caller so that it can be attached
         * as plain text to the start of the message. when decrypting the message on the server side - 
         * the server will first take the plain text IV from the start of the message and use it
         * to decrypt the rest of the message.
         * Key is currently hard coded in as there is no secure key exchange algorithm implemented - since this would require an SSL implementation with certificate verification
         * this ensure a unique IV for every message sent.
        */

        public static string EncryptDataWithAes(string plainText, out byte[] IV, byte[] key)
        {
            using Aes aesAlgorithm = Aes.Create();
            aesAlgorithm.Key = key;
            aesAlgorithm.GenerateIV(); //create a new IV for this packet/message
            IV = aesAlgorithm.IV; //pass it out the function so we can attach it to the messagw

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

        public static string DecryptDataWithAes(string cipherText, byte[] IV, byte[] key)
        {
            using Aes aesAlgorithm = Aes.Create();
            aesAlgorithm.Key = key;
            aesAlgorithm.IV = IV;

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