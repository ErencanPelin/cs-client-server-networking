using Client.Net.IO;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Client.Net
{
    public class Server
    {
        private TcpClient client;
        public PacketReader packetReader;

        public event Action onClientConnect;
        public event Action onMessageReceive;
        public event Action onClientDisconnect;

        public Server()
        {
            client = new TcpClient();
        }

        public void ConnectToServer(string username)
        {
            if (!client.Connected)
            {
                client.Connect("127.0.0.1", 5430);
                packetReader = new PacketReader(client.GetStream());

                if (!string.IsNullOrEmpty(username))
                {
                    var connectPkt = new PacketBuilder();
                    connectPkt.SetOpCode(0); //create our packet, and send it off to the server
                    connectPkt.WriteMessage(username);
                    client.Client.Send(connectPkt.GetPacketBytes());
                }

                ReadPackets();
            }
        }

        private void ReadPackets()
        {
            Task.Run(() => 
            {
                while(true)
                {
                    var opCode = packetReader.ReadByte();

                    switch (opCode)
                    {
                        case 1:
                            onClientConnect?.Invoke();
                            break;
                        case 5: 
                            onMessageReceive?.Invoke();
                            break;
                        case 10:
                            onClientDisconnect?.Invoke();
                            break;
                        default:
                            Console.WriteLine("");
                            break;
                    }
                }
            });
        }

        public void SendMessageToServer(string message)
        {
            var packet = new PacketBuilder();
            packet.SetOpCode(5);
            packet.WriteMessage(message);
            client.Client.Send(packet.GetPacketBytes());
        }
    }
}
