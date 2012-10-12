namespace CompileThis.BawBag.Plugins.Quotes
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    using CompileThis.BawBag.Extensibility;
    using CompileThis.BawBag.Jabbr;

    internal class QuotesHandler : MessageHandlerPluginBase
    {
        private static readonly Regex AddQuoteExpression = new Regex(@"^\s*add-quote\s+@?(?<nick>\S+)\s+(?<text>.*?)\s*$", RegexOptions.IgnoreCase);
        private static readonly Regex GetQuoteExpression = new Regex(@"^\s*quote(?:\s+@?(?<nick>\S+))?\s*$", RegexOptions.IgnoreCase);

        public QuotesHandler()
            : base("Quotes", PluginPriority.Normal, continueProcessing: false, mustBeAddressed: true)
        { }

        protected override MessageHandlerResult ExecuteCore(Message message, IPluginContext context)
        {
            if (message.Type != MessageType.Default)
            {
                return NotHandled();
            }

            var addMatch = AddQuoteExpression.Match(message.Text);
            if (addMatch.Success)
            {
                return AddQuote(addMatch.Groups["nick"].Value, addMatch.Groups["text"].Value, context);
            }

            var getMatch = GetQuoteExpression.Match(message.Text);
            if (getMatch.Success)
            {
                var nickMatch = getMatch.Groups["nick"];
                return GetQuote(nickMatch.Success ? nickMatch.Value : null, context);
            }

            return NotHandled();
        }

        private MessageHandlerResult AddQuote(string nick, string text, IPluginContext context)
        {
            var quote = new Quote
                {
                    CreatedBy = context.User.Name,
                    CreatedOn = context.DateTimeProvider.GetUtcNow(),
                    Nick = nick,
                    Text = text
                };

            context.RavenSession.Store(quote);

            return Handled(Message("OK @{0}, quote stored.", context.User.Name));
        }

        private MessageHandlerResult GetQuote(string nick, IPluginContext context)
        {
            IQueryable<Quote> query = context.RavenSession.Query<Quote>().Customize(x => x.RandomOrdering());
            
            if (nick != null)
            {
                query = query.Where(x => x.Nick == nick);
            }

            var quote = query.FirstOrDefault();

            if (quote == null)
            {
                if (nick == null)
                {
                    return Handled(Message("@{0}: no quotes are defined.", context.User.Name));
                }

                return Handled(Message("@{0}: no quotes are defined for '{1}'.", context.User.Name, nick));
            }

            return Handled(Message("{0}: {1}", quote.Nick, quote.Text));
        }
    }

    internal class Quote
    {
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string Nick { get; set; }
        public string Text { get; set; }
    }
}
