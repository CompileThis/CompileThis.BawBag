namespace CompileThis.BawBag.Plugins.RandomChoices
{
    using System;
    using System.Text.RegularExpressions;

    using CompileThis.BawBag.Extensibility;
    using CompileThis.BawBag.Jabbr;

    internal class RollHandler : MessageHandlerPluginBase
    {
        private static readonly Regex Matcher = new Regex(@"^\s*roll\s+(?<count>\d+)?d(?<sides>\d+)\s*$", RegexOptions.IgnoreCase);
        private static readonly Random RandomProvider = new Random();

        public RollHandler()
            : base("Roll", PluginPriority.Normal, continueProcessing: false, mustBeAddressed: true)
        { }

        protected override MessageHandlerResult ExecuteCore(Message message, IPluginContext context)
        {
            if (message.Type != MessageType.Default)
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

            return MessageHandlerResult.NotHandled;
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
