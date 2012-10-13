namespace CompileThis.BawBag.Plugins.Karma
{
    using System.Linq;
    using System.Text.RegularExpressions;

    using CompileThis.BawBag.Extensibility;

    internal class ApplyKarmaHandler : MessageHandlerPluginBase
    {
        public ApplyKarmaHandler()
            : base("apply-karma", PluginPriority.High, true, false)
        { }

        private static readonly Regex AssignKarmaExpression = new Regex(@"(?<nick>[a-zA-Z0-9]+)(?<quantity>[+]{2,})", RegexOptions.IgnoreCase);
        private static readonly Regex RetractKarmaExpression = new Regex(@"(?<nick>[a-zA-Z0-9]+)(?<quantity>[-]{2,})", RegexOptions.IgnoreCase);

        protected override MessageHandlerResult ExecuteCore(Message message, IPluginContext context)
        {
            var assignMatches = AssignKarmaExpression.Matches(message.Text);
            foreach (var match in assignMatches.Cast<Match>())
            {
                var nick = match.Groups["nick"].Value;
                var amount = match.Groups["quantity"].Length - 1;

                var karma = new Karma
                    {
                        CreatedBy = context.User.Name,
                        CreatedOn = context.DateTimeProvider.GetUtcNow(),
                        Name = nick,
                        Quantity = amount
                    };

                context.RavenSession.Store(karma);
            }

            var retractMatches = RetractKarmaExpression.Matches(message.Text);
            foreach (var match in retractMatches.Cast<Match>())
            {
                var nick = match.Groups["nick"].Value;
                var amount = -1 * (match.Groups["quantity"].Length - 1);

                var karma = new Karma
                {
                    CreatedBy = context.User.Name,
                    CreatedOn = context.DateTimeProvider.GetUtcNow(),
                    Name = nick,
                    Quantity = amount
                };

                context.RavenSession.Store(karma);
            }

            return NotHandled();
        }

        public override void Initialize(Raven.Client.IDocumentStore documentStore)
        {
            base.Initialize(documentStore);

            documentStore.ExecuteIndex(new KarmaTotals());
        }
    }
}
