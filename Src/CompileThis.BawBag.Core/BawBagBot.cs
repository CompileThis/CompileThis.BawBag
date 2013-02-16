namespace CompileThis.BawBag
{
    using System;
    using System.Net;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using CompileThis.BawBag.Extensibility;
    using CompileThis.BawBag.Extensibility.Internal;
    using CompileThis.BawBag.Jabbr;

    using NLog;

    using Raven.Client;
    using Raven.Client.Document;
    using Raven.Client.Indexes;

    public class BawBagBot
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly BawBagBotConfiguration _configuration;
        
        private IJabbrClient _client;
        private readonly IDocumentStore _store;
        private readonly IRandomNumberProvider _randomProvider;
        private readonly IInventoryManager _inventoryManager;
        private readonly ITextProcessor _textProcessor;
        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly Regex _botAddressedMatcher;

        private PluginManager _pluginManager;

        public BawBagBot()
            : this(BawBagBotConfiguration.FromConfigFile())
        { }

        public BawBagBot(BawBagBotConfiguration configuration)
        {
            Guard.NullParameter(configuration, () => configuration);

            _configuration = configuration;

            _store = new DocumentStore { Url = configuration.RavenUrl, DefaultDatabase = configuration.RavenDatabase };
            _randomProvider = new RandomNumberProvider();
            _inventoryManager = new InventoryManager(5, _store, _randomProvider);
            _textProcessor = new TextProcessor();
            _dateTimeProvider = new DefaultDateTimeProvider();

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

            _client = await JabbrManager.Connect(new Uri(_configuration.JabbrUrl), _configuration.JabbrNick, _configuration.JabbrPassword);
            _client.MessageReceived += MessageReceived;

            await _client.Connect();

            Log.Info("Started BawBag");

            foreach (var roomName in _configuration.Rooms)
            {
                if (!_client.Rooms.Contains(roomName))
                {
                    await _client.JoinRoom(roomName);
                }
            }
        }

        public async Task Stop()
        {
            _client.MessageReceived -= MessageReceived;

            foreach (var room in _client.Rooms)
            {
                await _client.LeaveRoom(room.Name);
            }

            await _client.Disconnect();
        }

        private void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Guard.NullParameter(sender, () => sender);
            Guard.NullParameter(e, () => e);

            if (e.Context.User.Name == _configuration.JabbrNick)
            {
                return;
            }

            var text = WebUtility.HtmlDecode(e.Message.Text);
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
                        Type = e.Message.Type,
                    };

                var context = new PluginContext
                    {
                        IsBotAddressed = isBotAddressed,
                        BotName = _configuration.JabbrNick,
                        Room = e.Context.Room,
                        User = e.Context.User,
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
