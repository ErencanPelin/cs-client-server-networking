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

        public event Action onClientConnect; //invoked when the client initially joins the server
        public event Action onMessageReceive; //invoked when this client receives a message from the server
        public event Action onClientDisconnect; //invoked when the clinet initially leaves the server
        public event Action onClientFailedToConnect; //invoked when the user fails to join the server
        public event Action onClientKicked; //invoked when the user is removed from the connection, or the server is shut down
        public event Action<string> onMessageSendFailed; //invoked when a message fails to reach the server
        public Server()
        {
            client = new TcpClient();
        }

        public void ConnectToServer(string username)
        {
            if (!client.Connected)
            {
                try
                {
                    client.Connect("127.0.0.1", 5430);
                }
                catch (SocketException e)
                {
                    onClientFailedToConnect.Invoke();
                    return; //stop execution to stop other errors from occuring
                }

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
                    try
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
                    } catch (System.IO.IOException e)
                    {
                        onClientKicked.Invoke();
                        return;
                    }
                }
            });
        }

        public void SendMessageToServer(string message)
        {
            var packet = new PacketBuilder();
            packet.SetOpCode(5);
            packet.WriteMessage(message);
            try
            {
                client.Client.Send(packet.GetPacketBytes());
            }
            catch (SocketException e)
            {
                onMessageSendFailed?.Invoke(message);
                return;
            }
        }
    }
}
