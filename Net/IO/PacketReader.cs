using System.Net.Sockets;
using System.Text;

namespace Server.Net.IO
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

            return Encoding.ASCII.GetString(buffer); //convert the bytes back into a string and return it
        }
    }
}
