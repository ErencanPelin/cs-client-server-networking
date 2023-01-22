using Client.Net.Security;
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
            var cipherText = Encryption.EncryptDataWithAes(message, new byte[] { 0, 0, 0 }, out var iv);
            var cipherLength = Encryption.EncryptDataWithAes(message.Length.ToString(), new byte[] { 0, 0, 0 }, out var iv);

            //default method
            ms.Write(BitConverter.GetBytes(message.Length));
            ms.Write(Encoding.ASCII.GetBytes(message));

            //with encryption:
            // ms.Write(Encoding.ASCII.GetBytes(iv)); //first write the IV
            // ms.Write(BitConverter.GetBytes(int.Parse(cipherLength))); //write the encrypted length of the original message
            // ms.Write(Encoding.ASCII.GetBytes(cipherText)); //write the encrypted version of the original message

        }

        public byte[] GetPacketBytes() => ms.ToArray();
    }
}
