﻿namespace CompileThis.BawBag.Handlers
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    internal class FactoidsHandler : IMessageHandler
    {
        private static readonly Regex WhitespaceExpression = new Regex(@"\s+");
        private static readonly Regex SimplifyTextExpression = new Regex(@"[^\sa-zA-Z0-9']+");
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

            MessageResponse response;

            switch (factoidResponse.ResponseType)
            {
                case FactoidResponseType.Is:
                    response = new MessageResponse
                        {
                            ResponseType = MessageHandlerResultResponseType.Message,
                            ResponseText = string.Format("{0} is {1}", message.Content, factoidResponse.Response)
                        };
                    break;

                case FactoidResponseType.Action:
                    response = new MessageResponse
                    {
                        ResponseType = MessageHandlerResultResponseType.Action,
                        ResponseText = factoidResponse.Response
                    };
                    break;

                case FactoidResponseType.Reply:
                    response = new MessageResponse
                    {
                        ResponseType = MessageHandlerResultResponseType.Message,
                        ResponseText = factoidResponse.Response
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
    }
}