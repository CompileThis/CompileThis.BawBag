namespace CompileThis.BawBag.Extensibility.Internal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using CompileThis.BawBag.Jabbr;

    internal class PluginManager
    {
        private IEnumerable<IMessageHandlerPlugin> _messageHandlers;

        public void ProcessMessage(Message message, IPluginContext context, IJabbrClient client)
        {
            Guard.NullParameter(message, () => message);
            Guard.NullParameter(context, () => context);
            Guard.NullParameter(client, () => client);

            foreach (var messageHandler in _messageHandlers)
            {
                var result = messageHandler.Execute(message, context);
                result.Execute(client, context.Room);

                var continueProcessing = (!result.IsHandled || messageHandler.ContinueProcessing);
                if (!continueProcessing)
                {
                    break;
                }
            }
        }

        public void Initialize(string pluginsDirectory)
        {
            Guard.NullParameter(pluginsDirectory, () => pluginsDirectory);

            var absolutePluginsDirectory = Path.Combine(Assembly.GetEntryAssembly().Location, pluginsDirectory);

            var assembly = Assembly.GetExecutingAssembly();

            var handlerTypes = assembly.GetTypes().Where(x => x.IsClass && !x.IsAbstract && typeof(IPlugin).IsAssignableFrom(x));

            var messageHandlers = (
                    from handlerType in handlerTypes
                    where typeof(IMessageHandlerPlugin).IsAssignableFrom(handlerType)
                    select (IMessageHandlerPlugin)Activator.CreateInstance(handlerType)
                ).ToList();

            _messageHandlers = messageHandlers.OrderBy(x => x.Priority).ThenBy(x => x.Name).ToList();
        }
    }
}
