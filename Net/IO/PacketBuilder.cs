using System;
using System.IO;
using System.Text;

namespace Client.Net.IO
{
    public class PacketBuilder
    {
        private MemoryStream ms;

        public PacketBuilder()
        {
            ms = new MemoryStream();
        }

        public void SetOpCode(byte opcode) => ms.WriteByte(opcode);

        public void WriteMessage(string message)
        {
            ms.Write(BitConverter.GetBytes(message.Length));
            ms.Write(Encoding.ASCII.GetBytes(message));
        }

        public byte[] GetPacketBytes() => ms.ToArray();
    }
}
