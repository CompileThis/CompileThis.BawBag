namespace CompileThis.BawBag
{
    using System;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using HtmlAgilityPack;

    using NLog;

    using Raven.Client;
    using Raven.Client.Document;

    using CompileThis.BawBag.Jabbr;

    public class BawBagBot
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly BawBagBotConfiguration _configuration;
        
        private readonly IJabbrClient _client;
        private readonly IDocumentStore _store;

        private readonly Regex _botAddressedMatcher;

        private MessageHandlerManager _messageManager;

        public BawBagBot()
            : this(BawBagBotConfiguration.FromConfigFile())
        { }

        public BawBagBot(BawBagBotConfiguration configuration)
        {
            _configuration = configuration;

            _client = new JabbrClient(configuration.JabbrUrl, new DefaultDateTimeProvider());
            _store = new DocumentStore { Url = configuration.RavenDbUrl };

            _botAddressedMatcher = new Regex("^@?" + _configuration.JabbrNick + "[,: ](.*)$", RegexOptions.IgnoreCase);
        }

        public async Task Start()
        {
            Log.Info("Starting BawBag");

            _client.MessageReceived += MessageReceived;

            Log.Info("Connecting to '{0}' as '{1}'.", _configuration.JabbrUrl, _configuration.JabbrNick);

            _store.Initialize();
            _messageManager = new MessageHandlerManager(_client, _store, _configuration.JabbrNick);

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

        private void MessageReceived(object sender, MessageEventArgs e)
        {
            
            var text = WebUtility.HtmlDecode(CleanMessage(e.Message.Content));
            var isBotAddressed = false;

            var addressMatch = _botAddressedMatcher.Match(text);
            if (addressMatch.Success)
            {
                isBotAddressed = true;
                text = addressMatch.Groups[1].Value.Trim();
            }

            var message = new MessageContext
                {
                    IsBotAddressed = isBotAddressed,
                    Content = text,
                    Room = e.Message.Room,
                    Type = e.Message.Type,
                    User = e.Message.User
                };

            _messageManager.HandleMessage(message);
        }

        private string CleanMessage(string message)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(message);

            return doc.DocumentNode.InnerText;
        }
    }
}
