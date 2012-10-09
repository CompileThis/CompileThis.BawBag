namespace CompileThis.BawBag.Plugins.Factoids
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    using CompileThis.BawBag.Extensibility;

    internal class EditFactiodsHandler : MessageHandlerPluginBase
    {
        private static readonly Regex AddFactoidExpression = new Regex(@"^\s*(?<trigger>.*)\s*<(?<type>is|reply|action)>\s*(?<response>.*?)\s*(?:<(?<options>cs)>\s*)*$", RegexOptions.IgnoreCase);
        private static readonly Regex ShowFactoidExpression = new Regex(@"^\s*show-factoid\s+(?<trigger>.*?)\s*$", RegexOptions.IgnoreCase);
        private static readonly Regex RemoveFactoidExpression = new Regex(@"^\s*remove-factoid\s+(?<trigger>.*?)\s+(?<index>\d+|\*)$", RegexOptions.IgnoreCase);

        public EditFactiodsHandler()
            : base("Edit Factoids", PluginPriority.High, continueProcessing: false, mustBeAddressed: true)
        { }

        protected override MessageHandlerResult ExecuteCore(Message message, IPluginContext context)
        {         
            var addMatch = AddFactoidExpression.Match(message.Text);
            if (addMatch.Success)
            {
                return AddFactiod(addMatch, context);
            }

            var showtMatch = ShowFactoidExpression.Match(message.Text);
            if (showtMatch.Success)
            {
                return ShowFactiod(showtMatch, context);
            }

            var removeMatch = RemoveFactoidExpression.Match(message.Text);
            if (removeMatch.Success)
            {
                return RemoveFactoid(removeMatch, context);
            }

            return NotHandled();
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
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

        private MessageHandlerResult AddFactiod(Match match, IPluginContext context)
        {
            var trigger = match.Groups["trigger"].Value;
            var type = match.Groups["type"].Value;
            var responseText = match.Groups["response"].Value;
            var options = match.Groups["options"].Captures.OfType<Capture>().Select(x => x.Value.ToUpperInvariant());

            var responseTrigger = context.TextProcessor.SimplifyText(trigger);
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

            if (factoid.Responses.Any(x => context.TextProcessor.SimplifiedEquals(x.Response, responseText)))
            {
                return Handled(new MessageResponse { ResponseType = MessageHandlerResultResponseType.DefaultMessage, ResponseText = string.Format("@{0}: I already had it that way!", context.User.Name) });
            }

            var response = new FactiodResponse
                {
                    Trigger = responseTrigger,
                    ResponseType = ToResponseType(type),
                    Response = responseText,
                    IsCaseSensitive = options.Any(x => x == "CS")
                };

            factoid.Responses.Add(response);

            var messageResponse = new MessageResponse
                {
                    ResponseType = MessageHandlerResultResponseType.DefaultMessage,
                    ResponseText = string.Format("OK @{0} factoid stored.", context.User.Name)
                };

            return Handled(messageResponse);

        }

        private MessageHandlerResult ShowFactiod(Match match, IPluginContext context)
        {
            var trigger = match.Groups["trigger"].Value;

            var factoidTrigger = context.TextProcessor.SimplifyText(trigger).ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(factoidTrigger))
            {
                return MessageHandlerResult.NotHandled;
            }

            var factoid = context.RavenSession.Query<Factoid>().SingleOrDefault(x => x.Trigger == factoidTrigger);
            if (factoid == null)
            {
                return Handled(new MessageResponse { ResponseType = MessageHandlerResultResponseType.DefaultMessage, ResponseText = string.Format("@{0}: no factoid is defined for '{1}'.", context.User.Name, trigger) });
            }

            var sb = new StringBuilder();
            sb.AppendFormat("@{0}: there are {1} responses defined for '{2}':", context.User.Name, factoid.Responses.Count, trigger);
            sb.AppendLine();
            var index = 1;
            foreach (var response in factoid.Responses)
            {
                sb.AppendFormat("{0}. {1}", index++, response.Response);
                sb.AppendLine();
            }

            return Handled(new MessageResponse {ResponseType = MessageHandlerResultResponseType.DefaultMessage, ResponseText = sb.ToString()});
        }

        private MessageHandlerResult RemoveFactoid(Match match, IPluginContext context)
        {
            var trigger = match.Groups["trigger"].Value;

            var factoidTrigger = context.TextProcessor.SimplifyText(trigger).ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(factoidTrigger))
            {
                return NotHandled();
            }

            var factoid = context.RavenSession.Query<Factoid>().SingleOrDefault(x => x.Trigger == factoidTrigger);
            if (factoid == null)
            {
                return Handled(new MessageResponse { ResponseType = MessageHandlerResultResponseType.DefaultMessage, ResponseText = string.Format("@{0}: no factoid is defined for '{1}'.", context.User.Name, trigger) });
            }

            var indexValue = match.Groups["index"].Value;
            if (indexValue == "*")
            {
                context.RavenSession.Delete(factoid);

                return Handled(new MessageResponse { ResponseType = MessageHandlerResultResponseType.DefaultMessage, ResponseText = string.Format("@{0}: removed factoid '{1}'.", context.User.Name, trigger) });
            }

            int index;
            int.TryParse(indexValue, out index);

            if (index < 1 || index > factoid.Responses.Count)
            {
                return Handled(new MessageResponse { ResponseType = MessageHandlerResultResponseType.DefaultMessage, ResponseText = string.Format("@{0}: invalid response number '{1}'.", context.User.Name, indexValue) });
            }

            factoid.Responses.RemoveAt(index - 1);

            return Handled(new MessageResponse { ResponseType = MessageHandlerResultResponseType.DefaultMessage, ResponseText = string.Format("@{0}: removed reponse {2} for factoid '{1}'.", context.User.Name, trigger, index) });
        }
    }
}
