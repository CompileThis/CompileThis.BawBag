namespace CompileThis.BawBag.Handlers.Wordplay
{
    using System;
    using System.Text.RegularExpressions;

    using CompileThis.BawBag.Jabbr;

    internal class SexactlyHandler : IMessageHandler
    {
        private static readonly Regex Matcher = new Regex(@"\b(ex.*?)\b", RegexOptions.IgnoreCase);
        private static readonly Random RandomProvider = new Random();

        public string Name
        {
            get { return "Sexactly"; }
        }

        public int Priority
        {
            get { return 105; }
        }

        public bool ContinueProcessing
        {
            get { return false; }
        }

        public MessageHandlerResult Execute(MessageContext message, MessageHandlerContext context)
        {
            if (message.IsBotAddressed || message.Type != MessageType.Default)
            {
                return MessageHandlerResult.NotHandled;
            }

            if (!Matcher.IsMatch(message.Content) || RandomProvider.Next(2) == 0)
            {
                return MessageHandlerResult.NotHandled;
            }

            var text = Matcher.Replace(message.Content, "s$1");

            var response = new MessageResponse
                {
                    ResponseType = MessageHandlerResultResponseType.DefaultMessage,
                    ResponseText = text
                };

            return new MessageHandlerResult
                {
                    IsHandled = true,
                    Responses = new[] {response}
                };
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
