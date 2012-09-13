namespace CompileThis.BawBag.Handlers
{
    using System;
    using System.Text.RegularExpressions;

    internal class ChooseHandler : IMessageHandler
    {
        private static readonly Regex Matcher = new Regex("^choose (.*?) (?:or (.*?))[.?]?$");
        private static readonly Random RandomProvider = new Random();

        public int Priority
        {
            get { throw new NotImplementedException(); }
        }

        public bool ContinueProcessing
        {
            get { return false; }
        }

        public MessageHandlerResult Execute(Message message)
        {
            if (!message.IsBotAddressed)
            {
                return MessageHandlerResult.NotHandled;
            }

            var match = Matcher.Match(message.Text.Trim());
            if (!match.Success)
            {
                return MessageHandlerResult.NotHandled;
            }

            var optionCount = match.Groups.Count;
            var index = RandomProvider.Next(1, optionCount);

            var text = string.Format("@{0}, {1}", message.User.Name, match.Groups[index].Value);

            return new MessageHandlerResult
                {
                    IsHandled = true,
                    ResponseType = MessageHandlerResultResponseType.Message,
                    ResponseText = text
                };
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
