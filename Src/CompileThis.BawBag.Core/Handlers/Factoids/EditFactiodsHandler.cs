﻿namespace CompileThis.BawBag.Handlers.Factoids
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    internal class EditFactiodsHandler : IMessageHandler
    {
        private static readonly Regex WhitespaceExpression = new Regex(@"\s+");
        private static readonly Regex SimplifyTextExpression = new Regex(@"[^\sa-zA-Z0-9']+");
        private static readonly Regex AddFactoidExpression = new Regex(@"^\s*(?<trigger>.*)\s*<(?<type>is|reply|action)>\s*(?<response>.*?)\s*(?:<(?<options>cs)>\s*)*$", RegexOptions.IgnoreCase);
        private static readonly Regex DisplayFactoidExpression = new Regex(@"^\s*show-factoid\s+(?<trigger>.*?)\s*$", RegexOptions.IgnoreCase);
        private static readonly Regex RemoveFactoidExpression = new Regex(@"^\s*remove-factoid\s+(?<trigger>.*?)\s+(?<index>\d+|\*)$", RegexOptions.IgnoreCase);

        public string Name
        {
            get { return "Edit-Factiods"; }
        }

        public int Priority
        {
            get { return 201; }
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
            
            var addMatch = AddFactoidExpression.Match(message.Content);
            if (addMatch.Success)
            {
                return AddFactiod(addMatch, message, context);
            }

            var displaytMatch = DisplayFactoidExpression.Match(message.Content);
            if (displaytMatch.Success)
            {
                return DisplayFactiod(displaytMatch, message, context);
            }

            var removeMatch = RemoveFactoidExpression.Match(message.Content);
            if (removeMatch.Success)
            {
                return RemoveFactoid(removeMatch, message, context);
            }

            return MessageHandlerResult.NotHandled;
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        private static string ProcessText(string text)
        {
            return SimplifyTextExpression.Replace(WhitespaceExpression.Replace(text, " "), "").Trim();
        }

        private static FactoidResponseType ToResponseType(string responseTypeText)
        {
            switch (responseTypeText.ToUpperInvariant())
            {
                case "IS":
                    return FactoidResponseType.Is;

                case "REPLY":
                    return FactoidResponseType.Reply;

                case "ACTION":
                    return FactoidResponseType.Action;
            }

            throw new ArgumentException(string.Format("Unknown response type '{0}'.", responseTypeText), "responseTypeText");
        }

        private static MessageHandlerResult AddFactiod(Match match, MessageContext message, MessageHandlerContext context)
        {
            var trigger = match.Groups["trigger"].Value;
            var type = match.Groups["type"].Value;
            var responseText = match.Groups["response"].Value;
            var options = match.Groups["options"].Captures.OfType<Capture>().Select(x => x.Value.ToUpperInvariant());

            var responseTrigger = ProcessText(trigger);
            var factoidTrigger = responseTrigger.ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(factoidTrigger))
            {
                return MessageHandlerResult.NotHandled;
            }

            var factoid = context.RavenSession.Query<Factoid>().SingleOrDefault(x => x.Trigger == factoidTrigger);
            if (factoid == null)
            {
                factoid = new Factoid { Trigger = factoidTrigger, Responses = new List<FactiodResponse>(1) };
                context.RavenSession.Store(factoid);
            }

            if (factoid.Responses.Any(x => ProcessText(x.Response).ToUpperInvariant() == ProcessText(responseText).ToUpperInvariant()))
            {
                return new MessageHandlerResult
                    {
                        IsHandled = true,
                        Responses = new[] {new MessageResponse {ResponseType = MessageHandlerResultResponseType.DefaultMessage, ResponseText = string.Format("@{0}: I already had it that way!", message.User.Name)}}
                    };
            }

            var response = new FactiodResponse
                {
                    Trigger = responseTrigger,
                    ResponseType = ToResponseType(type),
                    Response = responseText,
                    IsCaseSensitive = options.Any(x => x == "CS")
                };

            factoid.Responses.Add(response);

            return new MessageHandlerResult
            {
                IsHandled = true,
                Responses = new[] { new MessageResponse { ResponseType = MessageHandlerResultResponseType.DefaultMessage, ResponseText = string.Format("OK @{0} factoid stored.", message.User.Name) } }
            };
        }

        private static MessageHandlerResult DisplayFactiod(Match match, MessageContext message, MessageHandlerContext context)
        {
            var trigger = match.Groups["trigger"].Value;

            var factoidTrigger = ProcessText(trigger).ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(factoidTrigger))
            {
                return MessageHandlerResult.NotHandled;
            }

            var factoid = context.RavenSession.Query<Factoid>().SingleOrDefault(x => x.Trigger == factoidTrigger);
            if (factoid == null)
            {
                return new MessageHandlerResult
                    {
                        IsHandled = true,
                        Responses = new[] {new MessageResponse {ResponseType = MessageHandlerResultResponseType.DefaultMessage, ResponseText = string.Format("@{0}: no factoid is defined for '{1}'.", message.User.Name, trigger)}}
                    };
            }

            var sb = new StringBuilder();
            sb.AppendFormat("@{0}: there are {1} responses defined for '{2}':", message.User.Name, factoid.Responses.Count, trigger);
            sb.AppendLine();
            var index = 1;
            foreach (var response in factoid.Responses)
            {
                sb.AppendFormat("{0}. {1}", index++, response.Response);
                sb.AppendLine();
            }

            return new MessageHandlerResult
                {
                    IsHandled = true,
                    Responses = new[] {new MessageResponse {ResponseType = MessageHandlerResultResponseType.DefaultMessage, ResponseText = sb.ToString()}}
                };
        }

        private static MessageHandlerResult RemoveFactoid(Match match, MessageContext message, MessageHandlerContext context)
        {
            var trigger = match.Groups["trigger"].Value;

            var factoidTrigger = ProcessText(trigger).ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(factoidTrigger))
            {
                return MessageHandlerResult.NotHandled;
            }

            var factoid = context.RavenSession.Query<Factoid>().SingleOrDefault(x => x.Trigger == factoidTrigger);
            if (factoid == null)
            {
                return new MessageHandlerResult
                {
                    IsHandled = true,
                    Responses = new[] { new MessageResponse { ResponseType = MessageHandlerResultResponseType.DefaultMessage, ResponseText = string.Format("@{0}: no factoid is defined for '{1}'.", message.User.Name, trigger) } }
                };
            }

            var indexValue = match.Groups["index"].Value;
            if (indexValue == "*")
            {
                context.RavenSession.Delete(factoid);

                return new MessageHandlerResult
                {
                    IsHandled = true,
                    Responses = new[] { new MessageResponse { ResponseType = MessageHandlerResultResponseType.DefaultMessage, ResponseText = string.Format("@{0}: removed factoid '{1}'.", message.User.Name, trigger) } }
                };
            }

            var index = -1;
            int.TryParse(indexValue, out index);

            if (index < 1 || index > factoid.Responses.Count)
            {
                return new MessageHandlerResult
                {
                    IsHandled = true,
                    Responses = new[] { new MessageResponse { ResponseType = MessageHandlerResultResponseType.DefaultMessage, ResponseText = string.Format("@{0}: invalid response number '{1}'.", message.User.Name, indexValue) } }
                };
            }

            factoid.Responses.RemoveAt(index - 1);

            return new MessageHandlerResult
            {
                IsHandled = true,
                Responses = new[] { new MessageResponse { ResponseType = MessageHandlerResultResponseType.DefaultMessage, ResponseText = string.Format("@{0}: removed reponse {2} for factoid '{1}'.", message.User.Name, trigger, index) } }
            };
        }
    }
}