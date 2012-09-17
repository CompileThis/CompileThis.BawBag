namespace CompileThis.BawBag
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using NLog;

    using CompileThis.BawBag.Jabbr;
using Raven.Client;

    internal class MessageHandlerManager
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly IJabbrClient _client;
        private readonly IDocumentStore _store;

        private readonly IEnumerable<IMessageHandler> _handlers;

        public MessageHandlerManager(IJabbrClient client, IDocumentStore store)
        {
            _client = client;
            _store = store;

            _handlers = GetHandlers();
        }

        public void HandleMessage(MessageContext message)
        {
            using (var session = _store.OpenSession())
            {
                foreach (var handler in _handlers)
                {
                    try
                    {

                        var result = handler.Execute(message, new MessageHandlerContext(session));

                        ExecuteResult(result, message.Room, message.User);

                        var continueProcessing = (!result.IsHandled || handler.ContinueProcessing);
                        if (!continueProcessing)
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WarnException(string.Format("Failed to execute handler '{0}' for message '{1}' - {2}.", handler.Name, message.Content, ex.Message), ex);
                    }
                }

                session.SaveChanges();
            }
        }

        private async void ExecuteResult(MessageHandlerResult result, IRoom room, IUser user)
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
                        await _client.SendMessage(room.Name, response.ResponseText);
                        break;

                    case MessageHandlerResultResponseType.Action:
                        await _client.SendAction(room.Name, response.ResponseText);
                        break;

                    case MessageHandlerResultResponseType.Kick:
                        await _client.Kick(response.ResponseText, room.Name);
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
