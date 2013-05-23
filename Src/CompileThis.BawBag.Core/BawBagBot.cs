namespace CompileThis.BawBag
{
    using System;
    using System.Net;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using JabbR.Client;
    using JabbRMessage = JabbR.Client.Models.Message;

    using NLog;

    using Raven.Client;
    using Raven.Client.Document;
    using Raven.Client.Indexes;

    using CompileThis.BawBag.Extensibility;
    using CompileThis.BawBag.Extensibility.Internal;
    using System.Collections.Generic;   

    public class BawBagBot
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly BawBagBotConfiguration _configuration;
        
        private IJabbRClient _client;
        private readonly IDocumentStore _store;
        private readonly IRandomNumberProvider _randomProvider;
        private readonly IInventoryManager _inventoryManager;
        private readonly ITextProcessor _textProcessor;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IDictionary<string, JabbR.Client.Models.Room> _rooms;
        private readonly Regex _botAddressedMatcher;

        private PluginManager _pluginManager;

        public BawBagBot()
            : this(BawBagBotConfiguration.FromConfigFile())
        { }

        public BawBagBot(BawBagBotConfiguration configuration)
        {
            Guard.NullParameter(configuration, () => configuration);

            _configuration = configuration;

            _store = new DocumentStore { ConnectionStringName = "RavenDB" };
            _randomProvider = new RandomNumberProvider();
            _inventoryManager = new InventoryManager(5, _store, _randomProvider);
            _textProcessor = new TextProcessor();
            _dateTimeProvider = new DefaultDateTimeProvider();

            _rooms = new Dictionary<string, JabbR.Client.Models.Room>();
            _botAddressedMatcher = new Regex("^@?" + _configuration.JabbrNick + "[,: ](.*)$", RegexOptions.IgnoreCase);
        }

        public async Task Start()
        {
            Log.Info("Starting BawBag");
            Log.Info("Connecting to '{0}' as '{1}'.", _configuration.JabbrUrl, _configuration.JabbrNick);

            _store.Initialize();
            IndexCreation.CreateIndexes(Assembly.GetExecutingAssembly(), _store);

            _pluginManager = new PluginManager();
            _pluginManager.Initialize(_configuration.PluginsDirectory, _store);

            ServicePointManager.DefaultConnectionLimit = 10;

            _client = new JabbRClient(_configuration.JabbrUrl);
            _client.MessageReceived += MessageReceived;

            Log.Info("Starting BawBag: Connecting");
            JabbR.Client.Models.LogOnInfo logOnInfo;
            try
            {
                logOnInfo = await _client.Connect(_configuration.JabbrNick, _configuration.JabbrPassword);
            }
            catch (Exception ex)
            {
                Log.Fatal("Failed to connect", ex);
                throw;
            }
            Log.Info("Starting BawBag: Connected");
            foreach (var room in logOnInfo.Rooms)
            {
                _rooms[room.Name] = room;
            }

            Log.Info("Started BawBag");

            foreach (var roomName in _configuration.Rooms)
            {
                if (_rooms.ContainsKey(roomName))
                {
                    continue;
                }

                await _client.JoinRoom(roomName);
                var room = await _client.GetRoomInfo(roomName);

                _rooms[roomName] = room;
            }
        }

        public async Task Stop()
        {
            _client.MessageReceived -= MessageReceived;

            foreach (var room in _configuration.Rooms)
            {
                await _client.LeaveRoom(room);
            }

            _client.Disconnect();
        }

        private void MessageReceived(JabbRMessage jabbrMessage, string roomName)
        {
            Guard.NullParameter(jabbrMessage, () => jabbrMessage);
            Guard.NullParameter(roomName, () => roomName);

            if (jabbrMessage.User.Name == _configuration.JabbrNick)
            {
                return;
            }

            var text = WebUtility.HtmlDecode(jabbrMessage.Content);
            var isBotAddressed = false;

            var addressMatch = _botAddressedMatcher.Match(text);
            if (addressMatch.Success)
            {
                isBotAddressed = true;
                text = addressMatch.Groups[1].Value.Trim();
            }

            using (var session = _store.OpenSession())
            {
                var message = new Message
                    {
                        Text = text,
                        Type = MessageType.Default,
                    };

                var context = new PluginContext
                    {
                        IsBotAddressed = isBotAddressed,
                        BotName = _configuration.JabbrNick,
                        Room = _rooms[roomName],
                        User = jabbrMessage.User,
                        RavenSession = session,
                        RandomProvider = _randomProvider,
                        InventoryManager = _inventoryManager,
                        TextProcessor = _textProcessor,
                        DateTimeProvider = _dateTimeProvider
                    };

                _pluginManager.ProcessMessage(message, context, _client);

                session.SaveChanges();
            }
        }
    }
}
