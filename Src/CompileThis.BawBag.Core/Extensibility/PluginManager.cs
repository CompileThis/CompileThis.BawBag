﻿namespace CompileThis.BawBag.Extensibility
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
            pluginsDirectory = Path.Combine(Assembly.GetEntryAssembly().Location, pluginsDirectory);

            var assembly = Assembly.GetExecutingAssembly();

            var handlerTypes = assembly.GetTypes().Where(x => x.IsClass && !x.IsAbstract && typeof(IPlugin).IsAssignableFrom(x));

            var messageHandlers = new List<IMessageHandlerPlugin>();
            foreach (var handlerType in handlerTypes)
            {
                if (typeof(IMessageHandlerPlugin).IsAssignableFrom(handlerType))
                {
                    messageHandlers.Add((IMessageHandlerPlugin)Activator.CreateInstance(handlerType));
                }
            }

            _messageHandlers = messageHandlers.OrderBy(x => x.Priority).ThenBy(x => x.Name).ToList();
        }
    }
}