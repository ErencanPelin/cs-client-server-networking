namespace Server.Net.Security
{
    /// <summary>
    /// Client's public key is shared and checked against server private key and visa versa to decrypt messages
    /// </summary>
    public static class Encryption
    {
        private static string privateKey;
        private static string publicKey;

        public static string Encrypt(string text)
        {
            throw new NotImplementedException();
        }

        public static string Decrypt(string cipherText) 
        {
            throw new NotImplementedException();
        }
    }
}
