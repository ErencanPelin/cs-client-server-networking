using Client.Net.Security;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Client.Net.IO
{
    public class PacketReader : BinaryReader
    {
        private NetworkStream ns;

        public PacketReader(NetworkStream ns) : base(ns)
        {
            this.ns = ns;
        }

        public string ReadMessage()
        {
            var length = ReadInt32(); //create a suitable buffer
            byte[] buffer = new byte[length];
            ns.Read(buffer, 0, length); //read the bytes
            var cipherText = Encoding.ASCII.GetString(buffer); //read the string from the buffer
            var plainText = Encryption.DecryptDataWithAes(cipherText); //decrypt the data
            return plainText; //output plain text data
        }
    }
}
