namespace CompileThis.BawBag.Handlers
{
    using System;
    using System.Text.RegularExpressions;

    internal class QuestionHandler : IMessageHandler
    {
        private static readonly Regex Matcher = new Regex(@"^(?:is|are|will|can|should) .*\?$");
        private static readonly Random RandomProvider = new Random();

        private static readonly string[] PositiveResponses = new[] {"yes", "absolutely", "without doubt"};
        private static readonly string[] NegativeResponses = new[] { "no", "oh fuck no", "no way" };

        public string Name
        {
            get { return "Question"; }
        }

        public int Priority
        {
            get { return 102; }
        }

        public bool ContinueProcessing
        {
            get { return false; }
        }

        public MessageHandlerResult Execute(MessageContext message, MessageHandlerContext context)
        {
            if (!message.IsBotAddressed)
            {
                return MessageHandlerResult.NotHandled;
            }

            var match = Matcher.Match(message.Content);
            if (!match.Success)
            {
                return MessageHandlerResult.NotHandled;
            }
            
            var index = RandomProvider.Next(0, 2);
            string answerText;
            if (index == 0) // Negative
            {
                var answerIndex = RandomProvider.Next(0, NegativeResponses.Length);
                answerText = NegativeResponses[answerIndex];
            }
            else // Positive
            {
                var answerIndex = RandomProvider.Next(0, PositiveResponses.Length);
                answerText = PositiveResponses[answerIndex];
            }

            var response = new MessageResponse
            {
                ResponseType = MessageHandlerResultResponseType.DefaultMessage,
                ResponseText = string.Format("@{0}, {1}!", message.User.Name, answerText)
            };

            return new MessageHandlerResult
            {
                IsHandled = true,
                Responses = new[] { response }
            };
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
