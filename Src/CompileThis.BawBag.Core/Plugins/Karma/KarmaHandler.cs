namespace CompileThis.BawBag.Plugins.Karma
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Raven.Abstractions.Indexing;
    using Raven.Client.Indexes;

    using CompileThis.BawBag.Extensibility;

    internal class KarmaHandler :  MessageHandlerPluginBase
    {
        private static readonly Regex GetKarmaExpression = new Regex(@"^\s*karma\s+@?(?<nick>[a-zA-Z0-9-]+)[.?]?\s*$", RegexOptions.IgnoreCase);

        public KarmaHandler()
            : base("karma", PluginPriority.Normal, continueProcessing: false, mustBeAddressed: true)
        { }

        protected override MessageHandlerResult ExecuteCore(Message message, IPluginContext context)
        {
            var getKaramMatch = GetKarmaExpression.Match(message.Text);
            if (getKaramMatch.Success)
            {
                return GetKarma(getKaramMatch.Groups["nick"].Value, context);
            }

            return NotHandled();
        }

        private MessageHandlerResult GetKarma(string nick, IPluginContext context)
        {
            var result = context.RavenSession.Query<KarmaTotal, KarmaTotals>().SingleOrDefault(x => x.Name == nick);

            var quantity = (result == null) ? 0 : result.Quantity;

            return Handled(Message("@{0}: {1} = {2}", context.User.Name, nick, quantity));
        }
    }

    internal class Karma
    {
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
    }

    internal class KarmaTotal
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
    }

    internal class KarmaTotals : AbstractIndexCreationTask<Karma, KarmaTotal>
    {
        public KarmaTotals()
        {
            Map = karmas => from karma in karmas
                            select new {karma.Name, karma.Quantity};

            Reduce = results => from result in results
                                group result by result.Name
                                into g
                                select new {Name = g.Key, Quantity = g.Sum(x => x.Quantity)};

            Sort(x => x.Quantity, SortOptions.Int);
        }
    }
}
