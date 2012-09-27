namespace CompileThis.BawBag
{
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using NLog;

    using Raven.Client;
    using Raven.Client.Document;

    using CompileThis.BawBag.Jabbr;
    using CompileThis.BawBag.Extensibility;

    public class BawBagBot
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly BawBagBotConfiguration _configuration;
        
        private readonly IJabbrClient _client;
        private readonly IDocumentStore _store;
        private readonly IRandomNumberProvider _randomProvider;

        private readonly Regex _botAddressedMatcher;

        private PluginManager _pluginManager;

        public BawBagBot()
            : this(BawBagBotConfiguration.FromConfigFile())
        { }

        public BawBagBot(BawBagBotConfiguration configuration)
        {
            _configuration = configuration;

            _client = new JabbrClient(configuration.JabbrUrl, new DefaultDateTimeProvider());
            _store = new DocumentStore { Url = configuration.RavenUrl, DefaultDatabase = configuration.RavenDatabase };
            _randomProvider = new RandomNumberProvider();

            _botAddressedMatcher = new Regex("^@?" + _configuration.JabbrNick + "[,: ](.*)$", RegexOptions.IgnoreCase);
        }

        public async Task Start()
        {
            Log.Info("Starting BawBag");

            _client.MessageReceived += MessageReceived;

            Log.Info("Connecting to '{0}' as '{1}'.", _configuration.JabbrUrl, _configuration.JabbrNick);

            _store.Initialize();

            _pluginManager = new PluginManager();
            _pluginManager.Initialize(_configuration.PluginsDirectory);

            await _client.Connect(_configuration.JabbrNick, _configuration.JabbrPassword);

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
                        Room = e.Context.Room,
                        User = e.Context.User,
                        RavenSession = session,
                        RandomProvider = _randomProvider
                    };

                _pluginManager.ProcessMessage(message, context, _client);

                session.SaveChanges();
            }
        }
    }
}
