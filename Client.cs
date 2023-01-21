using System.Net;
using System.Net.Sockets;
using Server.Net.IO;

namespace Server
{
    public class Client
    {
        public string Username { get; set; }
        public Guid GUID { get; set; }
        public TcpClient client { get; set; }

        private PacketReader packetReader;

        public Client(TcpClient client)
        {
            this.client = client;
            GUID = Guid.NewGuid();
            packetReader = new PacketReader(client.GetStream());

            var opCode = packetReader.ReadByte();
            Username = packetReader.ReadMessage();
            
            Program.Log($"{Username}({(client.Client.RemoteEndPoint as IPEndPoint)?.Address}) has connected!");

            Task.Run(() => { Process(); }); //start processing packets on a separate thread
        }

        private void Process()
        {
            while(true)
            {
                try
                {
                    var opCode = packetReader.ReadByte();
                    switch (opCode)
                    {
                        case 5:
                            var message = packetReader.ReadMessage();
                            Program.SendMessage(this, $"[{DateTime.Now}] {Username}: {message}");
                            Program.Log($"{Username}: {message}");
                            break;
                        default:
                            break;
                    }
                }
                catch
                {
                    Program.DisconnectUser(GUID.ToString());
                    Program.Log($"{Username} has disconnected");
                    client.Close();
                    break;
                }
            }
        }
    }
}
