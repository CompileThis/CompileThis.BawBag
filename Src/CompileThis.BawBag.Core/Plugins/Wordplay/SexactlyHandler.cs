﻿namespace CompileThis.BawBag.Plugins.Wordplay
{
    using System;
    using System.Text.RegularExpressions;

    using CompileThis.BawBag.Extensibility;
    using CompileThis.BawBag.Jabbr;

    internal class SexactlyHandler : MessageHandlerPluginBase
    {
        private static readonly Regex Matcher = new Regex(@"\b(ex.*?)\b", RegexOptions.IgnoreCase);

        public SexactlyHandler()
            : base("Sexactly", PluginPriority.Normal, continueProcessing: false, mustBeAddressed: false)
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

            var text = Matcher.Replace(message.Text, m =>
                {
                    var capture = m.Captures[0].Value;
                    if (capture[0] == 'e')
                    {
                        return "s" + capture;
                    }

                    if (capture[1] == 'X')
                    {
                        return "SE" + capture.Substring(1);
                    }

                    return "Se" + capture.Substring(1);
                });

            return Handled(Message(text));
        }
    }
}
