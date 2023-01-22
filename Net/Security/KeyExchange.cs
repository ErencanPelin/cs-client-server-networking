using System.Security.Cryptography;
using System;

namespace Client.Net.Security
{
    //each client should have their own key exchange object
    //keys should be exchanged over port 443 (TLS) Transport layer security to ensure secure key transmission
    //once keys are verified, once keys are agreed upon, the program can encrypt data using the relevant public/private symmetric keys
    //using the AES encryption algorithm so that all data sent between clients and the server is completely encrypted
    public class KeyExchange
    {
        private ECDiffieHellmanCng client;
        public byte[] PublicKey { get; private set; } //given to the server who uses it to encrypt messages for this client
        public byte[] PrivateKey { get; private set; } //used to decrypt messages sent by the server - remember the server uses this clients public key to encrypt messages

        /*attackers or man-in-the-middle attacks may be able to gain access to the message, but because they lack the corresponding private
        *key, they won't be able to decrypt and steal the message contents - keeping confidentiality under CIA
        */

        public KeyExchange()
        {
            // create a new ECDiffieHellmanCng object
            client = new()
            {
                KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash,
                HashAlgorithm = CngAlgorithm.Sha256
            };

            // generate the public and private keys
            PublicKey = client.PublicKey.ToByteArray(); //stored as a byte array to easier transmission
            PrivateKey = client.Key.Export(CngKeyBlobFormat.EccPrivateBlob);
        }

        public byte[] SignPublicKey(byte[] publicKey)
        {
            throw new NotImplementedException();
        }

        public bool VerifySignature(byte[] signature)
        {
            throw new NotImplementedException();
        }
    }
}