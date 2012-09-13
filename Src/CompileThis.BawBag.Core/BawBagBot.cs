namespace CompileThis.BawBag
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using JabbR.Client;

    using NLog;
    using System.Text.RegularExpressions;

    public class BawBagBot
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly BawBagBotConfiguration _configuration;
        private readonly JabbRClient _client;
        private readonly MessageHandlerManager _messageManager;
        private readonly RoomCollection _rooms;

        public BawBagBot(BawBagBotConfiguration configuration)
        {
            _configuration = configuration;
            _client = new JabbRClient(configuration.Url);
            _messageManager = new MessageHandlerManager(_client);
            _rooms = new RoomCollection();
        }

        public async Task Start()
        {
            var joinedRooms = new HashSet<string>();

            _client.MessageReceived += MessageReceived;
            _client.MeMessageReceived += MeMessageReceived;

            var logOnInfo = await _client.Connect(_configuration.Name, _configuration.Password);
            foreach (var jabbrRoom in logOnInfo.Rooms)
            {
                joinedRooms.Add(jabbrRoom.Name);
            }

            foreach (var roomName in _configuration.Rooms)
            {
                if (!joinedRooms.Contains(roomName))
                {
                    await _client.JoinRoom(roomName);
                    joinedRooms.Add(roomName);
                }
            }

            foreach (var roomName in joinedRooms)
            {
                var jabbrRoom = await _client.GetRoomInfo(roomName);
                var room = JabbrTypeConverter.ConvertRoom(jabbrRoom);

                _rooms.Add(room);
            }
        }

        public async Task Stop()
        {
            _client.MessageReceived -= MessageReceived;
            _client.MeMessageReceived -= MeMessageReceived;

            _rooms.Clear();

            await LeaveRooms();

            try
            {
                await _client.LogOut();
            }
            catch (Exception ex)
            {
                Log.DebugException("Failed to sign out", ex);
            }

            _client.Disconnect();
        }

        private async Task LeaveRooms()
        {
            var rooms = _configuration.Rooms;
            Log.Debug("Leaving {0} room(s).", rooms.Length);

            foreach (var room in rooms)
            {
                Log.Trace("Leaving room '{0}'.", room);
                await _client.LeaveRoom(room);
                Log.Trace("Left room '{0}'.", room);
            }
        }

        private void MessageReceived(JabbR.Client.Models.Message jabbrMessage, string roomName)
        {
            var room = _rooms[roomName];
            var user = room.Users[jabbrMessage.User.Name];

            var text = jabbrMessage.Content;
            var isBotAddressed = false;

            var addressMatch = Regex.Match(text, "^@?BawBag[,:](.*)$", RegexOptions.IgnoreCase);
            if (addressMatch.Success)
            {
                isBotAddressed = true;
                text = addressMatch.Groups[1].Value;
            }

            var message = new Message
                {
                    IsBotAddressed = isBotAddressed,
                    Room = room,
                    Text = text,
                    Type = MessageType.Default,
                    User = user
                };

            _messageManager.HandleMessage(message);
        }

        private void MeMessageReceived(string name, string text, string roomName)
        {
            var room = _rooms[roomName];
            var user = room.Users[name];

            var message = new Message
            {
                IsBotAddressed = false,
                Room = room,
                Text = text,
                Type = MessageType.Default,
                User = user
            };

            _messageManager.HandleMessage(message);
        }
    }
}
