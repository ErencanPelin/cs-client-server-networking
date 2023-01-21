using Server.Net.IO;
using Server.Net.Security;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public static class Program
    {
        static TcpListener _tcpListener;
        static List<Client> users;
        static KeyExchange keyExchange;

        static event Action<Client> onClientConnect; //invoked whenever a new client is connected
        static event Action<Client> onClientDisconnect; //invoked whenever a client disconnects
        static event Action<Client, string> onMessageReceived; //invoked when a message is sent to the server

        public static void Main(string[] args)
        {
            InitServer();

            //subscribe to events
            onClientConnect += OnClientConnect;
            onClientDisconnect += OnClientDisconnect;
            onMessageReceived += OnMessageReceived;

            Start();

            //start listening
            Log("Server has started on 127.0.0.1:5430.\nWaiting for a connection. . .\n");
            while (true)
            {
                var client = new Client(_tcpListener.AcceptTcpClient());
                onClientConnect.Invoke(client);
            }
        }

        //start any required services for the server to function
        private static void InitServer()
        {
            Console.Title = "Server";
            users = new List<Client>();

            keyExchange = new KeyExchange(); //create a new pair of public/private keys for this session
        }

        //open client/server port
        private static void Start()
        {
            _tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 5430); //start listening on port 5430
            _tcpListener.Start();
        }

        /// <summary>
        /// Logs a given message to the server console using formatting
        /// </summary>
        /// <param name="message">Message to be logged into server console</param>
        public static void Log(string message) => Console.WriteLine($"[{DateTime.Now}] {message}");

        /// <summary>
        /// Sends a message to all connected clients in the server
        /// </summary>
        private static void Broadcast(byte opCode = 1, params string[] stringParcels)
        {
            if (opCode < 1) throw new Exception("Broadcase opCode must be greater than 1");

            foreach (var user in users) //go through every connected client and create/send a packet to them
            {
                var packet = new PacketBuilder();
                packet.SetOpCode(opCode);
                foreach (var parcel in stringParcels)
                    packet.WriteMessage(parcel);
                user.client.Client.Send(packet.GetPacketBytes());
            }
        }

        /// <summary>
        /// Broadcasts a message from a client to all connected clients
        /// </summary>
        /// <param name="message">Message to be broadcast</param>
        public static void SendMessage(Client sender, string message) => onMessageReceived.Invoke(sender, message);

        /// <summary>
        /// Find and disconnect the user based on their UID
        /// </summary>
        /// <param name="guid">UID of the client to be disconnected</param>
        public static void DisconnectUser(string guid)
        {
            var disconnectedUser = users.Where(o => guid == o.GUID.ToString()).First();
            onClientDisconnect.Invoke(disconnectedUser);
        }

        //allows the message and sender to be processed however you want - such as adding /commands - although /commands should be on their own opCode
        private static void OnMessageReceived(Client sender, string message)
        {
            Broadcast(5, message); //forward the message to all connected clients
        }

        private static void OnClientConnect(Client connectingClient)
        {
            users.Add(connectingClient); //update user list
            //broadcast the list of current users to all connected users so they can update their user lists
            foreach (var user in users)
                Broadcast(1, user.Username, user.GUID.ToString());
            Broadcast(5, $"[{DateTime.Now}] {connectingClient.Username}: connected."); //send the connect message to the chat log
        }

        private static void OnClientDisconnect(Client diconnectingClient)
        {
            users.Remove(diconnectingClient); //update the users list
            Broadcast(10, diconnectingClient.GUID.ToString()); //let all users know to update their user lists
            Broadcast(5, $"[{DateTime.Now}] {diconnectingClient.Username}: disconnected."); //send the disconnect message to the chat log
        }
    }
}