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
            var length = ReadInt32(); //read the length from the start of the packet
            byte[] iv = new byte[16]; //create buffer to store the IV
            byte[] buffer = new byte[length]; //create a suitable buffer
            ns.Read(iv, 0, 16); //read bytes for the IV - IV is 16 bytes long
            var IV = iv;
            ns.Read(buffer, 0, length); //read the bytes for the message
            var cipherText = Encoding.ASCII.GetString(buffer); //read the string from the buffer
            var plainText = Encryption.DecryptDataWithAes(cipherText, IV, Encoding.ASCII.GetBytes("b14ca5898a4e4133bbce2ea2315a1916")); //decrypt the data
            return plainText; //output plain text data
        }
    }
}
