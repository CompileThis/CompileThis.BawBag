namespace CompileThis.BawBag
{
    using System.Threading.Tasks;

    using JabbR.Client;
    using JabbR.Client.Models;

    using NLog;

    public class BawBagBot
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly BawBagBotConfiguration _configuration;
        private readonly JabbRClient _client;

        public BawBagBot(BawBagBotConfiguration configuration)
        {
            _configuration = configuration;
            _client = new JabbRClient(configuration.Url);
        }

        public async Task Start()
        {
            _client.MessageReceived += MessageReceived;
            _client.MeMessageReceived += MeMessageReceived;

            await _client.Connect(_configuration.Name, _configuration.Password);
            await JoinRooms();
        }

        public async Task Stop()
        {
            _client.MessageReceived -= MessageReceived;
            _client.MeMessageReceived -= MeMessageReceived;

            await LeaveRooms();

            _client.Disconnect();
        }

        private async Task JoinRooms()
        {
            var rooms = _configuration.Rooms;
            Log.Debug("Joining {0} room(s).", rooms.Length);

            foreach (var room in rooms)
            {
                await _client.JoinRoom(room);
            }
        }

        private async Task LeaveRooms()
        {
            var rooms = _configuration.Rooms;
            Log.Debug("Leaving {0} room(s).", rooms.Length);

            foreach (var room in rooms)
            {
                await _client.LeaveRoom(room);
            }
        }

        private void MessageReceived(Message message, string room)
        {
            //throw new NotImplementedException();
        }

        private void MeMessageReceived(string name, string text, string room)
        {
            //throw new NotImplementedException();
        }
    }
}
