namespace CompileThis.BawBag.Handlers.RandomChoices
{
    using System;
    using System.Text.RegularExpressions;

    using CompileThis.BawBag.Jabbr;

    internal class RollHandler : IMessageHandler
    {
        private static readonly Regex Matcher = new Regex(@"^\s*roll\s+(?<count>\d+)?d(?<sides>\d+)\s*$", RegexOptions.IgnoreCase);
        private static readonly Random RandomProvider = new Random();

        public string Name
        {
            get { return "Roll"; }
        }

        public int Priority
        {
            get { return 107; }
        }

        public bool ContinueProcessing
        {
            get { return false; }
        }

        public MessageHandlerResult Execute(MessageContext message, MessageHandlerContext context)
        {
            if (!message.IsBotAddressed || message.Type != MessageType.Default)
            {
                return MessageHandlerResult.NotHandled;
            }

            var match = Matcher.Match(message.Content);
            if (!match.Success)
            {
                return MessageHandlerResult.NotHandled;
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

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
