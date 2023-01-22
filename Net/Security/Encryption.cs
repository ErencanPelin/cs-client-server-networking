using System.Security.Cryptography;
using System.Text;

namespace Server.Net.Security
{
    public static class Encryption
    {
        //plain text msg is encrypted with AES, the IV is sent back to the caller so that it can be attached
        //as plain text to the start of the message. when decrypting the message on the server side - 
        //the server will first take the plain text IV from the start of the message and use it
        //to decrypt the rest of the message.
        //The key will be the public key of the server and which will be obtained when the client first connects to the server
        //this ensure a unique IV for every message sent.
        public static string EncryptDataWithAes(string plainText, out byte[] IV, byte[] key = default)
        {
            using Aes aesAlgorithm = Aes.Create();
            aesAlgorithm.Key = Encoding.ASCII.GetBytes("b14ca5898a4e4133bbce2ea2315a1916");
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

        public static string DecryptDataWithAes(string cipherText, byte[] IV, byte[] key = default)
        {
            using Aes aesAlgorithm = Aes.Create();
            aesAlgorithm.Key = Encoding.ASCII.GetBytes("b14ca5898a4e4133bbce2ea2315a1916");
            Console.WriteLine(Encoding.ASCII.GetString(IV));
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
