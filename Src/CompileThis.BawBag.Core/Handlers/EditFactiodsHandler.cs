namespace CompileThis.BawBag.Handlers
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    internal class EditFactiodsHandler : IMessageHandler
    {
        private static readonly Regex WhitespaceExpression = new Regex(@"\s+");
        private static readonly Regex SimplifyTextExpression = new Regex(@"[^\sa-zA-Z0-9']+");

        public string Name
        {
            get { return "Edit-Factiods"; }
        }

        public int Priority
        {
            get { return 201; }
        }

        public bool ContinueProcessing
        {
            get { return false; }
        }

        public MessageHandlerResult Execute(MessageContext message, MessageHandlerContext context)
        {
            var session = context.RavenSession;

            var factiodTrigger = ProcessText(message.Content);
            var factiod = session.Query<Factoid>().SingleOrDefault(x => x.Trigger == factiodTrigger);

            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new System.NotImplementedException();
        }

        private static string ProcessText(string text)
        {
            return SimplifyTextExpression.Replace(WhitespaceExpression.Replace(text, " "), "").Trim().ToLower();
        }   
    }

    internal class Factoid
    {
        public int Id { get; set; }
        public string Trigger { get; set; }
    }

    internal class FactiodResponse
    {

    }
}
