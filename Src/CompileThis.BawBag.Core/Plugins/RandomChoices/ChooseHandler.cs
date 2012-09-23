namespace CompileThis.BawBag.Plugins.RandomChoices
{
    using System.Collections.Generic;
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    using CompileThis.BawBag.Extensibility;

    internal class ChooseHandler : MessageHandlerPluginBase
    {
        private static readonly Regex Matcher = new Regex("^(?:choose )?(.*?) (?:or (.*?))+[.?]?$");

        public ChooseHandler()
            : base("Choose", PluginPriority.Normal, continueProcessing: false, mustBeAddressed: true)
        { }

        protected override MessageHandlerResult ExecuteCore(Message message, IPluginContext context)
        {
            var match = Matcher.Match(message.Text);
            if (!match.Success)
            {
                return NotHandled();
            }

            var values = new List<string>();
            values.Add(match.Groups[1].Value.Trim());
            values.AddRange(match.Groups[2].Captures.OfType<Capture>().Select(x => x.Value.Trim()));

            var options = (from v in values
                           where v.Length > 0 && !v.Equals("or", StringComparison.InvariantCultureIgnoreCase)
                           select v).ToList();

            if (options.Count == 0)
            {
                var messageResponse = new MessageResponse {ResponseType = MessageHandlerResultResponseType.DefaultMessage, ResponseText = string.Format("{0}: confuse BawBag, receive kicking...", context.User.Name)};
                var kickResponse = new MessageResponse {ResponseType = MessageHandlerResultResponseType.Kick};

                return Handled(messageResponse, kickResponse);
            }
            
            var index = context.RandomProvider.Next(0, options.Count);
            var text = string.Format("@{0}, {1}", context.User.Name, options[index]);

            var response = new MessageResponse
                {
                    ResponseType = MessageHandlerResultResponseType.DefaultMessage,
                    ResponseText = text
                };

            return Handled(response);
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
