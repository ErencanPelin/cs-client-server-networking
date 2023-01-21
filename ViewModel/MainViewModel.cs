using Client.Core;
using Client.Model;
using Client.Net;
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

            server.onClientConnect += ClientConnected;
            server.onMessageReceive += MessageReceived;
            server.onClientDisconnect += ClientDisconnected;

            ConnectCommand = new RelayCommand(o => server.ConnectToServer(Username!.Trim().Trim()), o => !string.IsNullOrEmpty(Username));
            SendCommand = new RelayCommand(o => { server.SendMessageToServer(Message.Trim()); Message = ""; }, o => !string.IsNullOrEmpty(Message));
        }

        private void ClientDisconnected()
        {
            var guid = server.packetReader.ReadMessage();
            var user = Users.Where((o) => o.GUID == guid).First();
            Application.Current.Dispatcher.Invoke(() => { Users.Remove(user); });
        }

        private void MessageReceived()
        {
            var msg = server.packetReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => { Messages.Add(msg); });
        }

        private void ClientConnected()
        {
            var username = server.packetReader.ReadMessage();
            var guid = server.packetReader.ReadMessage();
            var user = new UserModel
            {
                Username = username,
                GUID = guid,
            };

            if (!Users.Any((o) => o.GUID == user.GUID))
                Application.Current.Dispatcher.Invoke(() => Users.Add(user)); //since we're doing this on a separate thread
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
