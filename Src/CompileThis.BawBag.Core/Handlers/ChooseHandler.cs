using System.Collections.Generic;

namespace CompileThis.BawBag.Handlers
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    internal class ChooseHandler : IMessageHandler
    {
        private static readonly Regex Matcher = new Regex("^choose (.*?) (?:or (.*?))+[.?]?$");
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

            var values = new List<string>();
            values.Add(match.Groups[1].Value.Trim());
            values.AddRange(match.Groups[2].Captures.OfType<Capture>().Select(x => x.Value.Trim()));

            var options = (from v in values
                           where v.Length > 0 && !v.Equals("or", StringComparison.InvariantCultureIgnoreCase)
                           select v).ToList();

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
            
            var index = RandomProvider.Next(0, options.Count);
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
