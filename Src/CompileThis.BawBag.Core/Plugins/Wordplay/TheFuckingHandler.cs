﻿namespace CompileThis.BawBag.Plugins.Wordplay
{
    using System.Text.RegularExpressions;

    using CompileThis.BawBag.Extensibility;

    internal class TheFuckingHandler : MessageHandlerPluginBase
    {
        private static readonly Regex Matcher = new Regex("(the) (fucking)", RegexOptions.IgnoreCase);

        public TheFuckingHandler()
            : base("The Fucking", PluginPriority.Normal, continueProcessing: false, mustBeAddressed: false)
        { }

        protected override MessageHandlerResult ExecuteCore(Message message, IPluginContext context)
        {
            if (context.IsBotAddressed || message.Type != MessageType.Default)
            {
                return NotHandled();
            }

            if (!Matcher.IsMatch(message.Text) || context.RandomProvider.Next(2) == 0)
            {
                return NotHandled();
            }

            var text = Matcher.Replace(message.Text, "$2 $1");

            return Handled(Message(text));
        }
    }
}
