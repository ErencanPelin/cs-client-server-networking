using System.Security.Cryptography;
using System.Diagnostics.CodeAnalysis;

namespace Server.Net.Security
{
    //keys should be exchanged over port 443 (TLS) Transport layer security to ensure secure key transmission
    //once keys are verified, once keys are agreed upon, the program can encrypt data using the relevant public/private symmetric keys
    //using the AES encryption algorithm so that all data sent between clients and the server is completely encrypted
    public class KeyExchange
    {
        private ECDiffieHellmanCng server;
        public byte[] PublicKey { get; private set; }
        public byte[] PrivateKey { get; private set; }

        public KeyExchange()
        {
            // create a new ECDiffieHellmanCng object
            server = new ECDiffieHellmanCng
            {
                KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash,
                HashAlgorithm = CngAlgorithm.Sha256
            };

            // generate the public and private keys
            PublicKey = server.PublicKey.ToByteArray();
            PrivateKey = server.Key.Export(CngKeyBlobFormat.EccPrivateBlob);
        }
    }
}
