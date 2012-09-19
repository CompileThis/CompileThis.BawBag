namespace CompileThis.BawBag.Jabbr
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HtmlAgilityPack;
    using SignalR.Client.Hubs;

    using CompileThis.BawBag.Jabbr.Collections;
    using CompileThis.BawBag.Jabbr.ServerModels;

    public class JabbrClient : IJabbrClient
    {
        public event EventHandler<JoinedRoomEventArgs> JoinedRoom;
        public event EventHandler<MessageEventArgs> MessageReceived;
        public event EventHandler<LeftRoomEventArgs> LeftRoom;
        public event EventHandler<AddUserEventArgs> AddUser;

        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly HubConnection _connection;
        private readonly IHubProxy _chatHub;

        private readonly LookupList<string, Room> _rooms;
        private readonly LookupList<string, User> _users; 

        public JabbrClient(string url, IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;

            _connection = new HubConnection(url);
            _chatHub = _connection.CreateProxy("chat");

            _rooms = new LookupList<string, Room>(x => x.Name);
            _users = new LookupList<string, User>(x => x.Name);

            _chatHub.On<JabbrRoomSummary>("joinRoom", HandleJoinRoom);
            _chatHub.On<JabbrMessage, string>("addMessage", HandleAddMessage);
            _chatHub.On<JabbrUser, string>("leave", HandleLeaveMessage);
            _chatHub.On<JabbrUser, string, bool>("addUser", HandleAddUser);
            _chatHub.On<string, string, string>("sendMeMessage", HandleAddAction);
        }

        IReadOnlyLookupList<string, IRoom> IJabbrClient.Rooms
        {
            get { return _rooms; }
        }

        internal LookupList<string, Room> Rooms
        {
            get { return _rooms; }
        }

        IReadOnlyLookupList<string, IUser> IJabbrClient.Users
        {
            get { return _users; }
        }

        internal LookupList<string, User> Users
        {
            get { return _users; }
        }

        public async Task Connect(string username, string password)
        {
            await _connection.Start();

            var tcs = new TaskCompletionSource<object>();

            var logOnHandle = _chatHub.On<IEnumerable<JabbrRoomSummary>>("logOn", async summaries =>
                {
                    foreach (var summary in summaries)
                    {
                        var jabbrRoom = await _chatHub.Invoke<JabbrRoom>("GetRoomInfo", summary.Name);
                        jabbrRoom.Private = summary.Private;

                        var room = ServerModelConverter.ToRoom(jabbrRoom, _users);
                        _rooms.Add(room);
                    }

                    tcs.SetResult(null);
                });

            try
            {
                var joinSuccess = await _chatHub.Invoke<bool>("Join");
                if (!joinSuccess)
                {
                    await SetNick(username, password);
                }

                await tcs.Task;
            }
            finally
            {
                logOnHandle.Dispose();
            }
        }

        public async Task Disconnect()
        {
            await LogOut();
            _connection.Stop();
        }

        public Task JoinRoom(string roomName)
        {
            return _chatHub.Invoke("Send", string.Format("/join #{0}", roomName), "");
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

        public Task LeaveRoom(string room)
        {
            return _chatHub.Invoke("Send", string.Format("/leave #{0}", room), "");
        }

        private Task SetNick(string username, string password)
        {
            return _chatHub.Invoke("Send", String.Format("/nick {0} {1}", username, password), "");
        }

        private Task LogOut()
        {
            return _chatHub.Invoke("Send", "/logout", "");
        }

        protected virtual void OnJoinedRoom(JoinedRoomEventArgs e)
        {
            var handler = JoinedRoom;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnMessageReceived(MessageEventArgs e)
        {
            var handler = MessageReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnLeftRoom(LeftRoomEventArgs e)
        {
            var handler = LeftRoom;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnAddUser(AddUserEventArgs e)
        {
            var handler = AddUser;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void HandleJoinRoom(JabbrRoomSummary roomSummary)
        {
            Task.Factory.StartNew(async () =>
            {
                var jabbrRoom = await _chatHub.Invoke<JabbrRoom>("GetRoomInfo", roomSummary.Name);
                var room = ServerModelConverter.ToRoom(jabbrRoom, _users);

                _rooms.Add(room);

                OnJoinedRoom(new JoinedRoomEventArgs(room));
            });
        }

        private void HandleAddMessage(JabbrMessage jabbrMessage, string roomName)
        {
            Task.Factory.StartNew(() =>
                {
                    var room = _rooms[roomName];
                    var user = ServerModelConverter.ToUser(jabbrMessage.User, _users);

                    var message = new Message(CleanMessage(jabbrMessage.Content), jabbrMessage.Id, room, jabbrMessage.When, MessageType.Default, user);

                    OnMessageReceived(new MessageEventArgs(message));
                });
        }

        private void HandleLeaveMessage(JabbrUser jabbrUser, string roomName)
        {
            Task.Factory.StartNew(() =>
            {
                var room = _rooms[roomName];
                var user = ServerModelConverter.ToUser(jabbrUser, _users);

                room.Users.Remove(user.Name);

                if (room.Owners.Contains(user.Name))
                {
                    room.Owners.Remove(user.Name);
                }

                OnLeftRoom(new LeftRoomEventArgs(room, user));
            });
        }

        private void HandleAddUser(JabbrUser jabbrUser, string roomName, bool isOwner)
        {
            Task.Factory.StartNew(() =>
                {
                    var room = _rooms[roomName];
                    var user = ServerModelConverter.ToUser(jabbrUser, _users);

                    room.Users.Add(user);

                    if (isOwner)
                    {
                        room.Owners.Add(user);
                    }

                    OnAddUser(new AddUserEventArgs(room, user));
                });
        }

        private void HandleAddAction(string username, string content, string roomName)
        {
            Task.Factory.StartNew(() =>
            {
                var room = _rooms[roomName];
                var user = _users[username];

                var message = new Message(CleanMessage(content), string.Empty, room, _dateTimeProvider.GetNow(), MessageType.Action, user);

                OnMessageReceived(new MessageEventArgs(message));
            });
        }

        private string CleanMessage(string message)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(message);

            return doc.DocumentNode.InnerText;
        }
    }
}