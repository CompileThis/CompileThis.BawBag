namespace CompileThis.BawBag.Plugins.Factoids
{
    using System;
    using System.Linq;

    using CompileThis.BawBag.Extensibility;

    internal class FactoidsHandler : MessageHandlerPluginBase
    {
        public FactoidsHandler()
            : base("Factoids", PluginPriority.Low, continueProcessing: false, mustBeAddressed: false)
        { }

        protected override MessageHandlerResult ExecuteCore(Message message, IPluginContext context)
        {
            var responseTrigger = context.TextProcessor.SimplifyText(message.Text);
            var factoidTrigger = responseTrigger.ToUpperInvariant();

            var factoid = context.RavenSession.Query<Factoid>().SingleOrDefault(x => x.Trigger == factoidTrigger);
            if (factoid == null)
            {
                return NotHandled();
            }

            var matchingResponses = factoid.Responses.Where(x => !x.IsCaseSensitive || x.Trigger == responseTrigger).ToList();
            var index = context.RandomProvider.Next(matchingResponses.Count);

            var factoidResponse = matchingResponses[index];
            var responseText = context.TextProcessor.FormatPluginResponse(factoidResponse.Response, context);

            MessageResponse response;

            switch (factoidResponse.ResponseType)
            {
                case FactoidResponseType.Is:
                    response = Message("{0} is {1}", message.Text, responseText);
                    break;

                case FactoidResponseType.Action:
                    response = Action(responseText);
                    break;

                case FactoidResponseType.Reply:
                    response = Message(responseText);
                    break;

                default:
                    throw new Exception("Unknown response type.");
            }

            return Handled(response);
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
