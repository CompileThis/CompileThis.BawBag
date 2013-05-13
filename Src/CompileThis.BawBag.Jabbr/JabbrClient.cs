namespace CompileThis.BawBag.Jabbr
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Security;
    using System.Threading.Tasks;

    using CompileThis.BawBag.Jabbr.ServerModels;
    using CompileThis.Collections.Generic;

    using HtmlAgilityPack;

    using Microsoft.AspNet.SignalR.Client.Hubs;

    using NLog;

    public class JabbrClient : IJabbrClient
    {
        private const string AuthEndpoint = "account/login";
        private const string JabbrCookieName = "jabbr.userToken";
        private const string UserNameParamName = "username";
        private const string PasswordParamName = "password";

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly Uri _url;
        private readonly HubConnection _connection;

        private readonly IList<IDisposable> _subscriptionHandles; 

        private readonly LookupList<string, Room> _rooms;
        private readonly LookupList<string, User> _users;

        private IHubProxy _chatHub;
        private string _userName;
        private string _password;

        public JabbrClient(Uri url)
            : this(url, new DefaultDateTimeProvider())
        { }

        public JabbrClient(Uri url, IDateTimeProvider dateTimeProvider)
        {
            Guard.NullParameter(url, () => url);
            Guard.NullParameter(dateTimeProvider, () => dateTimeProvider);

            _url = url;
            _connection = new HubConnection(url.AbsoluteUri);
            _dateTimeProvider = dateTimeProvider;
            _subscriptionHandles = new List<IDisposable>();

            _connection.Error += ConnectionOnError;

            _rooms = new LookupList<string, Room>(x => x.Name);
            _users = new LookupList<string, User>(x => x.Name);
        }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public event EventHandler<LeftRoomEventArgs> UserLeftRoom;

        public event EventHandler<AddUserEventArgs> UserJoinedRoom;

        public IReadOnlyLookupList<string, Room> Rooms
        {
            get { return _rooms; }
        }

        public IReadOnlyLookupList<string, User> Users
        {
            get { return _users; }
        }

        public async Task Connect(string userName, string password)
        {
            Log.Info("Connecting to JabbR.");

            _userName = userName;
            _password = password;
            _chatHub = _connection.CreateHubProxy("chat");

            Log.Debug("Authenticating");

            await AuthenticateConnection(_connection, _url, userName, password);
            Log.Debug("Authenticated");

            _subscriptionHandles.Add(_chatHub.On<JabbrMessage, string>("addMessage", HandleAddMessage));
            _subscriptionHandles.Add(_chatHub.On<string, string, string>("sendMeMessage", HandleAddAction));
            _subscriptionHandles.Add(_chatHub.On<JabbrUser, string, bool>("addUser", UserJoinedHandler));
            _subscriptionHandles.Add(_chatHub.On<JabbrUser, string>("leave", UserLeftHandler));

            await _connection.Start();
            Log.Info("Started");
            var tcs = new TaskCompletionSource<object>();
            var disposable = new DisposableWrapper();

            var logOnHandle = _chatHub.On<IEnumerable<JabbrRoomSummary>>(
                "logOn",
                async summaries =>
                {
                    Log.Trace("Entered event handler.");

                    foreach (var summary in summaries)
                    {
                        Log.Info("Registering room '{0}'.", summary.Name);

                        var jabbrRoom =
                            await _chatHub.Invoke<JabbrRoom>("GetRoomInfo", summary.Name).ConfigureAwait(false);
                        jabbrRoom.Private = summary.Private;
                        var room = ServerModelConverter.ToRoom(jabbrRoom, this, _users);

                        _rooms.Add(room);

                        Log.Info("Registered room '{0}'.", room.Name);
                    }

                    Log.Trace("Completed event handler.");

                    tcs.SetResult(null);
                    disposable.Dispose();
                });

            disposable.Disposable = logOnHandle;
            await _chatHub.Invoke("Join").ConfigureAwait(false);
            await tcs.Task.ConfigureAwait(false);

            Log.Info("Connected to JabbR.");
        }

        public async Task Disconnect()
        {
            await LogOut();
            _connection.Disconnect();
            _chatHub = null;
        }

        public async Task<Room> JoinRoom(string roomName)
        {
            Guard.NullParameter(roomName, () => roomName);

            var tcs = new TaskCompletionSource<Room>();
            var disposable = new DisposableWrapper();

            var joinHandle = _chatHub.On<JabbrRoomSummary>(
                "joinRoom",
                async summary =>
                {
                    Log.Info("Joining room '{0}'.", summary.Name);

                    try
                    {
                        var jabbrRoom =
                            await _chatHub.Invoke<JabbrRoom>("GetRoomInfo", summary.Name).ConfigureAwait(false);
                        var room = ServerModelConverter.ToRoom(jabbrRoom, this, _users);

                        _rooms.Add(room);

                        tcs.SetResult(room);
                        Log.Info("Joined room '{0}'.", room.Name);
                    }
                    catch (Exception ex)
                    {
                        Log.ErrorException("Failed to join room.", ex);
                    }
                    finally
                    {
                        disposable.Dispose();
                    }
                });

            disposable.Disposable = joinHandle;

            await _chatHub.Invoke("Send", string.Format("/join #{0}", roomName), string.Empty).ConfigureAwait(false);

            return await tcs.Task.ConfigureAwait(false);
        }

        public Task SendDefaultMessage(string text, string roomName)
        {
            return _chatHub.Invoke("Send", text, roomName);
        }

        public Task SendActionMessage(string text, string roomName)
        {
            return _chatHub.Invoke("Send", string.Format("/me {0}", text), roomName);
        }

        public Task Kick(string username, string room)
        {
            return _chatHub.Invoke("Send", string.Format("/kick {0}", username), room);
        }

        public Task LeaveRoom(string room)
        {
            return _chatHub.Invoke("Send", string.Format("/leave #{0}", room), string.Empty);
        }

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            var handler = MessageReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnUserLeftRoom(LeftRoomEventArgs e)
        {
            var handler = UserLeftRoom;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnUserJoinedRoom(AddUserEventArgs e)
        {
            var handler = UserJoinedRoom;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private static async Task AuthenticateConnection(IHubConnection connection, Uri url, string userName, string password)
        {
            var authenticationUrl = new Uri(url, AuthEndpoint);

            var cookieContainer = new CookieContainer();
            var httpHandler = new HttpClientHandler { UseCookies = true, CookieContainer = cookieContainer };

            var formContent =
                new FormUrlEncodedContent(
                    new[]
                        {
                            new KeyValuePair<string, string>(UserNameParamName, userName),
                            new KeyValuePair<string, string>(PasswordParamName, password)
                        });

            var client = new HttpClient(httpHandler);

            var response = await client.PostAsync(authenticationUrl, formContent);
            response.EnsureSuccessStatusCode();

            var cookies = cookieContainer.GetCookies(url);
            if (cookies == null || cookies[JabbrCookieName] == null)
            {
                throw new SecurityException("Didn't get a cookie from JabbR! Ensure your User Name/Password are correct");
            }

            connection.CookieContainer = cookieContainer;
        }

        private static string CleanMessage(string message)
        {
            return message;
            /* I don't know why I did this
            var doc = new HtmlDocument();
            doc.LoadHtml(message);

            return doc.DocumentNode.InnerText;
            */
        }

        private Task LogOut()
        {
            return _chatHub.Invoke("Send", "/logout", string.Empty);
        }

        private void HandleAddMessage(JabbrMessage jabbrMessage, string roomName)
        {
            Task.Factory.StartNew(() =>
            {
                var room = _rooms[roomName];
                var user = ServerModelConverter.ToUser(jabbrMessage.User, _users);

                var message = new ReceivedMessage(MessageType.Default, CleanMessage(jabbrMessage.Content), jabbrMessage.Id, jabbrMessage.When);
                var context = new JabbrEventContext(room, user);

                OnMessageReceived(new MessageReceivedEventArgs(message, context));
            });
        }

        private async void ConnectionOnError(Exception exception)
        {
            Log.InfoException("Connection error", exception);

            foreach (var subscriptionHandle in _subscriptionHandles)
            {
                subscriptionHandle.Dispose();
            }

            await this.Disconnect();
            await this.Connect(_userName, _password);
        }

        private void UserJoinedHandler(JabbrUser jabbrUser, string roomName, bool isOwner)
        {
            Task.Factory.StartNew(() =>
            {
                var room = _rooms[roomName];
                var user = ServerModelConverter.ToUser(jabbrUser, _users);

                room.AddUser(user);

                if (isOwner)
                {
                    room.AddOwner(user);
                }

                OnUserJoinedRoom(new AddUserEventArgs(room, user));
            });
        }

        private void UserLeftHandler(JabbrUser jabbrUser, string roomName)
        {
            Task.Factory.StartNew(() =>
            {
                var room = _rooms[roomName];
                var user = ServerModelConverter.ToUser(jabbrUser, _users);

                room.RemoveUser(user);

                if (room.Owners.Contains(user.Name))
                {
                    room.RemoveOwner(user);
                }

                OnUserLeftRoom(new LeftRoomEventArgs(room, user));
            });
        }

        private void HandleAddAction(string username, string content, string roomName)
        {
            Task.Factory.StartNew(() =>
            {
                var room = _rooms[roomName];
                var user = _users[username];

                var message = new ReceivedMessage(MessageType.Action, CleanMessage(content), null, _dateTimeProvider.GetNow());
                var context = new JabbrEventContext(room, user);

                OnMessageReceived(new MessageReceivedEventArgs(message, context));
            });
        }
    }
}