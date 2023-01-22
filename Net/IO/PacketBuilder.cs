using Server.Net.Security;
using System.Text;

namespace Server.Net.IO
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
            //default method
            var cipherText = Encryption.EncryptDataWithAes(message); //encrypt message
            ms.Write(BitConverter.GetBytes(cipherText.Length));
            ms.Write(Encoding.ASCII.GetBytes(cipherText));

            //with encryption:
           // var cipherText = Encryption.EncryptDataWithAes(message, new byte[] {0, 0, 0}, out var iv);
           // ms.Write(Encoding.ASCII.GetBytes(iv)); //first write the IV
           // ms.Write(BitConverter.GetBytes(cipherText.Length)); //write the encrypted length of the original message
           // ms.Write(Encoding.ASCII.GetBytes(cipherText)); //write the encrypted version of the original message
        }

        public byte[] GetPacketBytes() => ms.ToArray();
    }
}
