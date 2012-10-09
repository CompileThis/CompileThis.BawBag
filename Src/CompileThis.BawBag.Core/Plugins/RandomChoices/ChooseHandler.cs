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
                return Handled(Message("{0}: confuse BawBag, receive kicking...", context.User.Name), Kick());
            }
            
            var index = context.RandomProvider.Next(0, options.Count);

            return Handled(Message("@{0}: {1}", context.User.Name, options[index]));
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
