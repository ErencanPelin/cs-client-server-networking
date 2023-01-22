using Client.Core;
using Client.Model;
using Client.Net;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Client.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Server server;

        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand ConnectCommand { get; set; }
        public RelayCommand SendCommand { get; set; }
        public string Username { get; set; }
        private string _message;

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<string>();

            server = new Server();

            server.onClientConnect += OnClientConnected;
            server.onMessageReceive += OnMessageReceived;
            server.onClientDisconnect += OnClientDisconnected;
            server.onClientFailedToConnect += OnClientFailedToConnect;
            server.onClientKicked += OnClientKicked;
            server.onMessageSendFailed += OnMessageSendFailed;

            ConnectCommand = new RelayCommand(o => server.ConnectToServer(Username!.Trim().Trim()), o => !string.IsNullOrEmpty(Username));
            SendCommand = new RelayCommand(o => { server.SendMessageToServer(opCode: 5, Message.Trim()); Message = ""; }, o => !string.IsNullOrEmpty(Message));
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #region Events
        private void OnClientDisconnected()
        {
            var guid = server.packetReader.ReadMessage();
            var user = Users.Where((o) => o.GUID == guid).First();
            Application.Current.Dispatcher.Invoke(() => { Users.Remove(user); }); //update user list
        }

        private void OnMessageReceived()
        {
            var msg = server.packetReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => { Messages.Add(msg); }); //update message history
        }

        private void OnClientConnected()
        {
            var username = server.packetReader.ReadMessage();
            var guid = server.packetReader.ReadMessage();
            var user = new UserModel
            {
                Username = username,
                GUID = guid,
            };

            if (!Users.Any((o) => o.GUID == user.GUID)) //update user list
                Application.Current.Dispatcher.Invoke(() => Users.Add(user)); //since we're doing this on a separate thread
        }

        private void OnClientFailedToConnect()
        {
            Console.WriteLine("Failed to find server or server refused connection");
            //do anything else here
        }

        private void OnMessageSendFailed(string message)
        {
            Console.WriteLine("Message failed to reach the server, check internet connection");
            Application.Current.Dispatcher.Invoke(() => { Messages.Add($"[{DateTime.Now}] Message: \"{message}\" failed to send. Check your network settings or the server might be temporarily shut down."); });
        }

        private void OnClientKicked()
        {
            Console.WriteLine("You were kicked from the server or the server was shutdown");
            //do anything else here
        }
        #endregion
    }
}
