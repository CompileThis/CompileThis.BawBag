namespace CompileThis.BawBag.Plugins.Factoids
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using CompileThis.BawBag.Extensibility;

    internal class FactoidsHandler : MessageHandlerPluginBase
    {
        private static readonly Regex WhitespaceExpression = new Regex(@"\s+");
        private static readonly Regex SimplifyTextExpression = new Regex(@"[^\sa-zA-Z0-9']+");
        private static readonly Regex WhoExpression = new Regex(@"\$\{who\}", RegexOptions.IgnoreCase);
        private static readonly Regex SomeoneExpression = new Regex(@"\$\{someone\}", RegexOptions.IgnoreCase);

        public FactoidsHandler()
            : base("Factoids", PluginPriority.Low, continueProcessing: false, mustBeAddressed: false)
        { }

        protected override MessageHandlerResult ExecuteCore(Message message, IPluginContext context)
        {
            var responseTrigger = ProcessText(message.Text);
            var factoidTrigger = responseTrigger.ToUpperInvariant();

            var factoid = context.RavenSession.Query<Factoid>().SingleOrDefault(x => x.Trigger == factoidTrigger);
            if (factoid == null)
            {
                return NotHandled();
            }

            var matchingResponses = factoid.Responses.Where(x => !x.IsCaseSensitive || x.Trigger == responseTrigger).ToList();
            var index = context.RandomProvider.Next(matchingResponses.Count);

            var factoidResponse = matchingResponses[index];
            var responseText = ProcessResponseText(factoidResponse.Response, context);

            MessageResponse response;

            switch (factoidResponse.ResponseType)
            {
                case FactoidResponseType.Is:
                    response = new MessageResponse
                        {
                            ResponseType = MessageHandlerResultResponseType.DefaultMessage,
                            ResponseText = string.Format("{0} is {1}", message.Text, responseText)
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

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

        private static string ProcessText(string text)
        {
            return WhitespaceExpression.Replace(SimplifyTextExpression.Replace(text, ""), " ").Trim();
        }

        private static string ProcessResponseText(string text, IPluginContext context)
        {
            text = WhoExpression.Replace(text, "@" + context.User.Name);
            text = SomeoneExpression.Replace(text, m =>
                {
                    var index = context.RandomProvider.Next(context.Room.Users.Count);
                    var userName = context.Room.Users[index].Name;

                    return "@" + userName;
                });

            return text;
        }
    }
}
