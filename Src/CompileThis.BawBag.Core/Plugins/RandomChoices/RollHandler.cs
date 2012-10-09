namespace CompileThis.BawBag.Plugins.RandomChoices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using CompileThis.BawBag.Extensibility;
    using CompileThis.BawBag.Jabbr;

    internal class RollHandler : MessageHandlerPluginBase
    {
        private static readonly Regex Matcher = new Regex(@"^\s*rolls\s+(?<count>\d+)?d(?<sides>\d+)\s*$", RegexOptions.IgnoreCase);

        public RollHandler()
            : base("Roll", PluginPriority.Normal, continueProcessing: false, mustBeAddressed: false)
        { }

        protected override MessageHandlerResult ExecuteCore(Message message, IPluginContext context)
        {
            if (message.Type != MessageType.Action)
            {
                return MessageHandlerResult.NotHandled;
            }

            var match = Matcher.Match(message.Text);
            if (!match.Success)
            {
                return NotHandled();
            }

            var countText = match.Groups["count"].Value;
            var sidesText = match.Groups["sides"].Value;

            if (countText == "")
            {
                countText = "1";
            }

            var count = int.Parse(countText);
            var sides = int.Parse(sidesText);

            if (count > 10)
            {
                return Handled(Message(string.Format("@{0}: that's too many dice for my tiny bothands!", context.User.Name)));
            }

            if (sides > 20)
            {
                return Handled(Message(string.Format("@{0}: the numbers are too tiny to read!", context.User.Name)));
            }

            if (count == 1)
            {
                var result = context.RandomProvider.Next(1, sides + 1);
                var singleResponse = string.Format("@{0}: you got {1}.", context.User.Name, result);

                return Handled(Message(singleResponse));
            }

            var results = new List<int>();
            for (var index = 0; index < count; ++index)
            {
                var result = context.RandomProvider.Next(1, sides + 1);
                results.Add(result);
            }

            var response = string.Format("@{0}: you got {1} for a total of {2}.", context.User.Name, string.Join(", ", results), results.Sum());

            return Handled(Message(response));
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
