namespace CompileThis.BawBag.Jabbr
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using SignalR.Client.Hubs;

    public interface IJabbrClient
    {
        Task SendMessage(string room, string message);
        Task SendAction(string room, string action);
    }

    public class JabbrClient : IJabbrClient
    {
        public event EventHandler<LogOnEventArgs> LoggedOn;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<ActionReceivedEventArgs> ActionReceived;

        private readonly HubConnection _connection;
        private readonly IHubProxy _chatHub;

        public JabbrClient(string url)
        {
             _connection = new HubConnection(url);
             _chatHub = _connection.CreateProxy("chat");

             _chatHub.On<IEnumerable<JabbrRoomSummary>>("logOn", HandleLogOn);
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

        public async Task SendMessage(string room, string message)
        {
            throw new NotImplementedException();
        }

        public async Task SendAction(string room, string action)
        {
            throw new NotImplementedException();
        }

        private Task SetNick(string username, string password)
        {
            return _chatHub.Invoke("Send", String.Format("/nick {0} {1}", username, password), "");
        }

        private Task LogOut()
        {
            return _chatHub.Invoke("Send", "/logout", "");
        }

        protected virtual void OnLoggedOn(LogOnEventArgs e)
        {
            var handler = LoggedOn;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void HandleLogOn(IEnumerable<JabbrRoomSummary> roomSummaries)
        {
            Task.Factory.StartNew(async () =>
                {
                    var rooms = new List<JabbrRoom>();

                    foreach (var summary in roomSummaries)
                    {
                        var room = await _chatHub.Invoke<JabbrRoom>("GetRoomInfo", summary.Name);
                        rooms.Add(room);
                    }

                    var e = new LogOnEventArgs();
                    OnLoggedOn(e);
                });
        }
    }
}
