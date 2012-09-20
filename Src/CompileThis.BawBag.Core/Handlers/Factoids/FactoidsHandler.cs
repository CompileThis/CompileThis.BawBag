namespace CompileThis.BawBag.Handlers.Factoids
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    internal class FactoidsHandler : IMessageHandler
    {
        private static readonly Regex WhitespaceExpression = new Regex(@"\s+");
        private static readonly Regex SimplifyTextExpression = new Regex(@"[^\sa-zA-Z0-9']+");
        private static readonly Regex WhoExpression = new Regex(@"\$\{who\}", RegexOptions.IgnoreCase);
        private static readonly Regex SomeoneExpression = new Regex(@"\$\{someone\}", RegexOptions.IgnoreCase);

        private static readonly Random RandomProvider = new Random();

        public string Name
        {
            get { return "Factoids"; }
        }

        public int Priority
        {
            get { return 202; }
        }

        public bool ContinueProcessing
        {
            get { return false; }
        }

        public MessageHandlerResult Execute(MessageContext message, MessageHandlerContext context)
        {
            var responseTrigger = ProcessText(message.Content);
            var factoidTrigger = responseTrigger.ToUpperInvariant();

            var factoid = context.RavenSession.Query<Factoid>().SingleOrDefault(x => x.Trigger == factoidTrigger);
            if (factoid == null)
            {
                return MessageHandlerResult.NotHandled;
            }

            var matchingResponses = factoid.Responses.Where(x => !x.IsCaseSensitive || x.Trigger == responseTrigger).ToList();
            var index = RandomProvider.Next(matchingResponses.Count);

            var factoidResponse = matchingResponses[index];
            var responseText = ProcessResponseText(factoidResponse.Response, message);

            MessageResponse response;

            switch (factoidResponse.ResponseType)
            {
                case FactoidResponseType.Is:
                    response = new MessageResponse
                        {
                            ResponseType = MessageHandlerResultResponseType.DefaultMessage,
                            ResponseText = string.Format("{0} is {1}", message.Content, responseText)
                        };
                    break;

                case FactoidResponseType.Action:
                    response = new MessageResponse
                    {
                        ResponseType = MessageHandlerResultResponseType.ActionMessage,
                        ResponseText = responseText
                    };
                    break;

                case FactoidResponseType.Reply:
                    response = new MessageResponse
                    {
                        ResponseType = MessageHandlerResultResponseType.DefaultMessage,
                        ResponseText = responseText
                    };
                    break;

                default:
                    throw new Exception("Unknown response type.");
            }

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

        private static string ProcessText(string text)
        {
            return SimplifyTextExpression.Replace(WhitespaceExpression.Replace(text, " "), "").Trim();
        }

        private static string ProcessResponseText(string text, MessageContext message)
        {
            text = WhoExpression.Replace(text, "@" + message.User.Name);
            text = SomeoneExpression.Replace(text, m =>
                {
                    var index = RandomProvider.Next(message.Room.Users.Count);
                    var userName = message.Room.Users[index];

                    return "@" + userName;
                });

            return text;
        }
    }
}
