namespace CompileThis.BawBag.Plugins.Karma
{
    using System.Linq;
    using System.Text.RegularExpressions;

    using CompileThis.BawBag.Extensibility;

    internal class KarmaHandler : MessageHandlerPluginBase
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
}
