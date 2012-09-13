namespace CompileThis.BawBag
{
    using System.Threading.Tasks;

    using NLog;

    using CompileThis.BawBag.Jabbr;

    public class BawBagBot
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly BawBagBotConfiguration _configuration;
        private readonly JabbrClient _client;
        private readonly MessageHandlerManager _messageManager;
        private readonly RoomCollection _rooms;

        public BawBagBot(BawBagBotConfiguration configuration)
        {
            _configuration = configuration;
            _client = new JabbrClient(configuration.Url);
            _messageManager = new MessageHandlerManager(_client);
            _rooms = new RoomCollection();
        }

        public async Task Start()
        {
            Log.Info("Starting BawBag");

            _client.MessageReceived += MessageReceived;
            _client.ActionReceived += ActionReceived;

            Log.Trace("Connecting to server.");

            await _client.Connect(_configuration.Name, _configuration.Password);

            Log.Info("Started BawBag");
        }

        public async Task Stop()
        {
            _client.MessageReceived -= MessageReceived;
            _client.ActionReceived -= ActionReceived;

            await LeaveRooms();
            await _client.Disconnect();
        }

        private async Task LeaveRooms()
        {
            var rooms = _configuration.Rooms;
            Log.Debug("Leaving {0} room(s).", rooms.Length);

            foreach (var room in rooms)
            {
                Log.Trace("Leaving room '{0}'.", room);
                //await _client.LeaveRoom(room);
                Log.Trace("Left room '{0}'.", room);
            }
        }

        private void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            //var room = _rooms[roomName];
            //var user = room.Users[jabbrMessage.User.Name];

            //var text = jabbrMessage.Content;
            //var isBotAddressed = false;

            //var addressMatch = Regex.Match(text, "^@?BawBag[,: ](.*)$", RegexOptions.IgnoreCase);
            //if (addressMatch.Success)
            //{
            //    isBotAddressed = true;
            //    text = addressMatch.Groups[1].Value.Trim();
            //}

            //var message = new Message
            //    {
            //        IsBotAddressed = isBotAddressed,
            //        Room = room,
            //        Text = text,
            //        Type = MessageType.Default,
            //        User = user
            //    };

            //_messageManager.HandleMessage(message);
        }

        private void ActionReceived(object sender, ActionReceivedEventArgs e) //(string name, string text, string roomName)
        {
            //var message = new Message
            //{
            //    IsBotAddressed = false,
            //    Room = e.Room,
            //    Text = e.ActionText,
            //    Type = MessageType.Default,
            //    User = e.User
            //};

            //_messageManager.HandleMessage(message);
        }
    }
}
