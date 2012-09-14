﻿using System;
using System.Text.RegularExpressions;

namespace CompileThis.BawBag
{
    using System.Threading.Tasks;

    using NLog;

    using CompileThis.BawBag.Jabbr;

    public class BawBagBot
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly BawBagBotConfiguration _configuration;
        private readonly IJabbrClient _client;
        private readonly MessageHandlerManager _messageManager;

        public BawBagBot(BawBagBotConfiguration configuration)
        {
            _configuration = configuration;
            _client = new JabbrClient(configuration.Url);
            _messageManager = new MessageHandlerManager(_client);
        }

        public async Task Start()
        {
            Log.Info("Starting BawBag");

            _client.MessageReceived += MessageReceived;
            _client.ActionReceived += ActionReceived;
            _client.LoggedOn += LoggedOn;

            Log.Trace("Connecting to server.");

            await _client.Connect(_configuration.Name, _configuration.Password);

            Log.Info("Started BawBag");
        }

        private void LoggedOn(object sender, EventArgs e)
        {
            foreach (var roomName in _configuration.Rooms)
            {
                if (!_client.Rooms.Contains(roomName))
                {
                    _client.JoinRoom(roomName);
                }
            }
        }

        public async Task Stop()
        {
            _client.MessageReceived -= MessageReceived;
            _client.ActionReceived -= ActionReceived;
            _client.LoggedOn -= LoggedOn;

            await _client.Disconnect();
        }

        private void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var text = e.Message.Content;
            var isBotAddressed = false;

            var addressMatch = Regex.Match(text, "^@?BawBag[,: ](.*)$", RegexOptions.IgnoreCase);
            if (addressMatch.Success)
            {
                isBotAddressed = true;
                text = addressMatch.Groups[1].Value.Trim();
            }

            var message = new Message
                {
                    IsBotAddressed = isBotAddressed,
                    Room = e.Room,
                    Text = text,
                    Type = MessageType.Default,
                    User = e.Message.User
                };

            _messageManager.HandleMessage(message);
        }

        private void ActionReceived(object sender, ActionReceivedEventArgs e) //(string name, string text, string roomName)
        {
            var message = new Message
            {
                IsBotAddressed = false,
                Room = e.Room,
                Text = e.Content,
                Type = MessageType.Action,
                User = e.User
            };

            _messageManager.HandleMessage(message);
        }
    }
}
