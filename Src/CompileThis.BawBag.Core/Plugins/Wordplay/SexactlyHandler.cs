namespace CompileThis.BawBag.Plugins.Wordplay
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
                return MessageHandlerResult.NotHandled;
            }

            var text = Matcher.Replace(message.Text, "s$1");

            var response = new MessageResponse
                {
                    ResponseType = MessageHandlerResultResponseType.DefaultMessage,
                    ResponseText = text
                };

            return Handled(response);
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
