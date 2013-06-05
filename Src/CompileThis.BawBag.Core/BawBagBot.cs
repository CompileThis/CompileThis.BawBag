namespace CompileThis.BawBag
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net;
  using System.Reflection;
  using System.Text.RegularExpressions;
  using System.Threading.Tasks;

  using CompileThis.BawBag.Extensibility;
  using CompileThis.BawBag.Extensibility.Internal;

  using JabbR.Client;

  using NLog;

  using Raven.Client;
  using Raven.Client.Document;
  using Raven.Client.Indexes;

  using JabbRMessage = JabbR.Client.Models.Message;
  using JabbRRoom = JabbR.Client.Models.Room;
  using JabbRUser = JabbR.Client.Models.User;

  public class BawBagBot
  {
    private static readonly Logger Log = LogManager.GetCurrentClassLogger();

    private readonly BawBagBotConfiguration _configuration;

    private readonly IDocumentStore _store;
    private readonly IRandomNumberProvider _randomProvider;
    private readonly IInventoryManager _inventoryManager;
    private readonly ITextProcessor _textProcessor;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IDictionary<string, Room> _rooms;
    private readonly Regex _botAddressedMatcher;

    private IJabbRClient _client;
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

      _rooms = new Dictionary<string, Room>();
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

      _client = new JabbRClient(_configuration.JabbrUrl) { AutoReconnect = true };

      _client.MessageReceived += MessageReceived;
      _client.MeMessageReceived += MeMessageReceived;
      _client.UserJoined += UserJoined;
      _client.UserLeft += UserLeft;

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
      foreach (var roomName in logOnInfo.Rooms.Select(r => r.Name))
      {
        var jabbrRoom = await _client.GetRoomInfo(roomName);
        var room = Room.Create(jabbrRoom);
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
        var jabbrRoom = await _client.GetRoomInfo(roomName);
        var room = Room.Create(jabbrRoom);
        _rooms[room.Name] = room;
      }
    }

    public async Task Stop()
    {
      _client.MessageReceived -= MessageReceived;
      _client.MeMessageReceived -= MeMessageReceived;

      foreach (var room in _configuration.Rooms)
      {
        await _client.LeaveRoom(room);
      }

      _client.Disconnect();
    }

    private void MeMessageReceived(string userName, string content, string roomName)
    {
      Guard.NullParameter(userName, () => userName);
      Guard.NullParameter(content, () => content);
      Guard.NullParameter(roomName, () => roomName);

      Log.Trace("Received ACTION message: {0}, {1}, {2}", userName, roomName, content);

      if (userName == _configuration.JabbrNick)
      {
        return;
      }

      var room = _rooms[roomName];
      var user = room.GetUserByName(userName);

      this.MessageReceivedCore(MessageType.Action, content, user, room);
    }

    private void MessageReceived(JabbRMessage jabbrMessage, string roomName)
    {
      Guard.NullParameter(jabbrMessage, () => jabbrMessage);
      Guard.NullParameter(roomName, () => roomName);

      Log.Trace("Received DEFAULT message: {0}, {1}, {2}", jabbrMessage.User.Name, roomName, jabbrMessage.Content);

      if (jabbrMessage.User.Name == _configuration.JabbrNick)
      {
        return;
      }

      var room = _rooms[roomName];
      var user = room.GetUserByName(jabbrMessage.User.Name);

      this.MessageReceivedCore(MessageType.Default, jabbrMessage.Content, user, room);
    }

    private void MessageReceivedCore(MessageType type, string content, User user, Room room)
    {
      var text = WebUtility.HtmlDecode(content);
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
              Type = type,
            };

        var context = new PluginContext
            {
              IsBotAddressed = isBotAddressed,
              BotName = _configuration.JabbrNick,
              Room = room,
              User = user,
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

    private void UserJoined(JabbRUser user, string roomName, bool isOwner)
    {
      var room = _rooms[roomName];
      room.AddUser(User.Create(user));
    }

    private void UserLeft(JabbRUser user, string roomName)
    {
      var room = _rooms[roomName];
      room.RemoveUser(user.Name);
    }
  }
}
