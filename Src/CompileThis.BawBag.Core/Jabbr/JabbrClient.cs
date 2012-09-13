namespace CompileThis.BawBag.Jabbr
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using SignalR.Client.Hubs;

    public interface IJabbrClient
    {
        event EventHandler<EventArgs> LoggedOn;
        event EventHandler<JoinedRoomEventArgs> JoinedRoom;
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        event EventHandler<ActionReceivedEventArgs> ActionReceived;

        JabbrRoomCollection Rooms { get; }

        Task Connect(string username, string password);
        Task Disconnect();

        Task JoinRoom(string roomName);

        Task SendMessage(string room, string message);
        Task SendAction(string room, string action);
        Task Kick(string username, string room);
    }

    public class JabbrClient : IJabbrClient
    {
        public event EventHandler<EventArgs> LoggedOn;
        public event EventHandler<JoinedRoomEventArgs> JoinedRoom;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<ActionReceivedEventArgs> ActionReceived;

        private readonly HubConnection _connection;
        private readonly IHubProxy _chatHub;
        private readonly JabbrRoomCollection _rooms;

        public JabbrClient(string url)
        {
            _connection = new HubConnection(url);
            _chatHub = _connection.CreateProxy("chat");
            _rooms = new JabbrRoomCollection();

            _chatHub.On<IEnumerable<JabbrRoomSummary>>("logOn", HandleLogOn);
            _chatHub.On<JabbrRoomSummary>("joinRoom", HandleJoinRoom);
            _chatHub.On<JabbrMessage, string>("addMessage", HandleAddMessage);
        }

        public JabbrRoomCollection Rooms
        {
            get { return _rooms; }
        }

        public async Task Connect(string username, string password)
        {
            await _connection.Start();
            await SetNick(username, password);
        }

        public async Task Disconnect()
        {
            await LogOut();
            _connection.Stop();
        }

        public Task JoinRoom(string roomName)
        {
            return _chatHub.Invoke("Send", string.Format("/join {0}", roomName), "");
        }

        public Task SendMessage(string room, string message)
        {
            return _chatHub.Invoke("Send", message, room);
        }

        public Task SendAction(string room, string action)
        {
            return _chatHub.Invoke("Send", string.Format("/me {0}", action), room);
        }

        public Task Kick(string username, string room)
        {
            return _chatHub.Invoke("Send", string.Format("/kick {0}", username), room);
        }

        private Task SetNick(string username, string password)
        {
            return _chatHub.Invoke("Send", String.Format("/nick {0} {1}", username, password), "");
        }

        private Task LogOut()
        {
            return _chatHub.Invoke("Send", "/logout", "");
        }

        protected virtual void OnLoggedOn(EventArgs e)
        {
            var handler = LoggedOn;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnJoinedRoom(JoinedRoomEventArgs e)
        {
            var handler = JoinedRoom;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            var handler = MessageReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void HandleLogOn(IEnumerable<JabbrRoomSummary> roomSummaries)
        {
            Task.Factory.StartNew(async () =>
                {
                    foreach (var summary in roomSummaries)
                    {
                        var room = await _chatHub.Invoke<JabbrRoom>("GetRoomInfo", summary.Name);
                        _rooms.Add(room);
                    }

                    OnLoggedOn(EventArgs.Empty);
                });
        }

        private void HandleJoinRoom(JabbrRoomSummary roomSummary)
        {
            Task.Factory.StartNew(async () =>
            {
                var room = await _chatHub.Invoke<JabbrRoom>("GetRoomInfo", roomSummary);
                _rooms.Add(room);

                OnJoinedRoom(new JoinedRoomEventArgs(room));
            });
        }

        private void HandleAddMessage(JabbrMessage message, string roomName)
        {
            Task.Factory.StartNew(() =>
                {
                    var room = _rooms[roomName];
                    OnMessageReceived(new MessageReceivedEventArgs(message, room));
                });
        }
    }
}
