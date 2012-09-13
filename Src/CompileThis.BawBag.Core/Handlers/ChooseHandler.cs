namespace CompileThis.BawBag.Handlers
{
    using System;
    using System.Linq;
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

            var match = Matcher.Match(message.Text);
            if (!match.Success)
            {
                return MessageHandlerResult.NotHandled;
            }

            var options = (from o in match.Groups.OfType<Group>()
                           let value = o.Value.Trim()
                           where o.Index > 0 && value.Length > 0 && !value.Equals("or", StringComparison.InvariantCultureIgnoreCase)
                           select value).ToList();

            if (options.Count == 0)
            {
                var messageResponse = new MessageResponse {ResponseType = MessageHandlerResultResponseType.Message, ResponseText = string.Format("{0}: confuse BawBag, receive kicking...", message.User.Name)};
                var kickResponse = new MessageResponse {ResponseType = MessageHandlerResultResponseType.Kick};

                return new MessageHandlerResult
                {
                    IsHandled = true,
                    Responses = new[] { messageResponse, kickResponse }
                };
            }
            
            var index = RandomProvider.Next(1, options.Count);
            var text = string.Format("@{0}, {1}", message.User.Name, options[index]);

            var response = new MessageResponse
                {
                    ResponseType = MessageHandlerResultResponseType.Message,
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
