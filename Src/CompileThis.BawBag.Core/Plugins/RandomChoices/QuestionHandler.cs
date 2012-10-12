namespace CompileThis.BawBag.Plugins.RandomChoices
{
    using System;
    using System.Text.RegularExpressions;

    using CompileThis.BawBag.Extensibility;

    internal class QuestionHandler : MessageHandlerPluginBase
    {
        private static readonly Regex Matcher = new Regex(@"^\s*(?:is|are|will|can|should|has|does|did|will|would)\s+.+\?$");

        private static readonly string[] PositiveResponses = new[] {"yes", "absolutely", "without doubt"};
        private static readonly string[] NegativeResponses = new[] { "no", "oh fuck no", "no way" };

        public QuestionHandler()
            : base("Question", PluginPriority.Normal, continueProcessing: false, mustBeAddressed: true)
        { }

        protected override MessageHandlerResult ExecuteCore(Message message, IPluginContext context)
        {
            var match = Matcher.Match(message.Text);
            if (!match.Success)
            {
                return NotHandled();
            }
            
            var index = context.RandomProvider.Next(0, 2);
            string answerText;
            if (index == 0) // Negative
            {
                var answerIndex = context.RandomProvider.Next(0, NegativeResponses.Length);
                answerText = NegativeResponses[answerIndex];
            }
            else // Positive
            {
                var answerIndex = context.RandomProvider.Next(0, PositiveResponses.Length);
                answerText = PositiveResponses[answerIndex];
            }

            return Handled(Message("@{0}: {1}!", context.User.Name, answerText));
        }
    }
}
