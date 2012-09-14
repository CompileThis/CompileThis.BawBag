namespace CompileThis.BawBag
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using NLog;

    using CompileThis.BawBag.Jabbr;

    internal class MessageHandlerManager
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly IJabbrClient _client;
        private readonly IEnumerable<IMessageHandler> _handlers;

        public MessageHandlerManager(IJabbrClient client)
        {
            _client = client;

            _handlers = GetHandlers();
        }

        public void HandleMessage(Message message)
        {
            foreach (var handler in _handlers)
            {
                try
                {
                    var result = handler.Execute(message);

                    ExecuteResult(result, message.Room.Name, message.User.Name);

                    var continueProcessing = (!result.IsHandled || handler.ContinueProcessing);
                    if (!continueProcessing)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Log.WarnException(string.Format("Failed to execute handler '{0}' for message '{1}' - {2}.", handler.Name, message.Text, ex.Message), ex);
                }
            }
        }

        private async void ExecuteResult(MessageHandlerResult result, string roomName, string userName)
        {
            if (!result.IsHandled)
            {
                return;
            }

            foreach (var response in result.Responses)
            {
                switch (response.ResponseType)
                {
                    case MessageHandlerResultResponseType.Message:
                        await _client.SendMessage(roomName, response.ResponseText);
                        break;

                    case MessageHandlerResultResponseType.Action:
                        await _client.SendAction(roomName, response.ResponseText);
                        break;

                    case MessageHandlerResultResponseType.Kick:
                        await _client.Kick(response.ResponseText, roomName);
                        break;
                }
            }
        }

        private IEnumerable<IMessageHandler> GetHandlers()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var handlerTypes = assembly.GetTypes().Where(x => x.IsClass && typeof(IMessageHandler).IsAssignableFrom(x));

            var handlerInstances = handlerTypes.Select(x => (IMessageHandler)Activator.CreateInstance(x));
            var orderedInstances = handlerInstances.OrderBy(x => x.Priority).ThenBy(x => x.Name).ToList();

            return orderedInstances;
        }
    }
}
