using System.Security.Cryptography;

namespace Client.Net.Security
{
    //each client should have their own key exchange object
    //keys should be exchanged over port 443 (TLS) Transport layer security to ensure secure key transmission
    //once keys are verified, once keys are agreed upon, the program can encrypt data using the relevant public/private symmetric keys
    //using the AES encryption algorithm so that all data sent between clients and the server is completely encrypted
    public class KeyExchange
    {
        private ECDiffieHellmanCng client;
        public byte[] PublicKey { get; private set; }
        public byte[] PrivateKey { get; private set; }

        public KeyExchange()
        {
            // create a new ECDiffieHellmanCng object
            client = new ECDiffieHellmanCng();
            client.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
            client.HashAlgorithm = CngAlgorithm.Sha256;

            // generate the public and private keys
            PublicKey = client.PublicKey.ToByteArray();
            PrivateKey = client.Key.Export(CngKeyBlobFormat.EccPrivateBlob);
        }

        public byte[] SignData(byte[] data)
        {
            // create a new ECDsaCng object
            using (ECDsaCng aliceSigner = new ECDsaCng(CngKey.Import(PrivateKey, CngKeyBlobFormat.EccPrivateBlob)))
            {
                // sign the data using ECDSA
                return aliceSigner.SignData(data);
            }
        }

        public byte[] CalculateSharedSecretKey(byte[] receivedPublicKey)
        {
            // create a new ECDiffieHellmanCng object to import the received public key
            using (ECDiffieHellmanCng bobExchange = new(CngKey.Import(receivedPublicKey, CngKeyBlobFormat.EccPublicBlob)))
            {
                // calculate the shared secret key
                return bobExchange.DeriveKeyMaterial(CngKey.Import(receivedPublicKey, CngKeyBlobFormat.EccPublicBlob));
            }
        }
    }
}
