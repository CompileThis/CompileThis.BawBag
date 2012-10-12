namespace CompileThis.BawBag.Extensibility
{
    public abstract class MessageHandlerPluginBase : PluginBase, IMessageHandlerPlugin
    {
        private readonly string _name;
        private readonly PluginPriority _priority;
        private readonly bool _continueProcessing;
        private readonly bool _mustBeAddressed;

        protected MessageHandlerPluginBase(string name, PluginPriority priority, bool continueProcessing, bool mustBeAddressed)
        {
            Guard.NullParameter(name, () => name);

            _name = name;
            _priority = priority;
            _continueProcessing = continueProcessing;
            _mustBeAddressed = mustBeAddressed;
        }

        public override string Name
        {
            get { return _name; }
        }

        public override PluginPriority Priority
        {
            get { return _priority; }
        }

        public override bool ContinueProcessing
        {
            get { return _continueProcessing; }
        }

        protected bool MustBeAddressed
        {
            get { return _mustBeAddressed; }
        }

        public MessageHandlerResult Execute(Message message, IPluginContext context)
        {
            Guard.NullParameter(message, () => message);
            Guard.NullParameter(context, () => context);

            if (MustBeAddressed && !context.IsBotAddressed)
            {
                return MessageHandlerResult.NotHandled;
            }

            return ExecuteCore(message, context);
        }

        public override void Initialize()
        { }

        protected abstract MessageHandlerResult ExecuteCore(Message message, IPluginContext context);

        protected MessageHandlerResult NotHandled()
        {
            return MessageHandlerResult.NotHandled;
        }

        protected MessageHandlerResult Handled(params MessageResponse[] responses)
        {
            return new MessageHandlerResult
                {
                    IsHandled = true,
                    Responses = responses
                };
        }

        protected MessageResponse Action(string text, params object[] arguments)
        {
            return new MessageResponse
                {
                    ResponseType = MessageHandlerResultResponseType.ActionMessage,
                    ResponseText = string.Format(text, arguments)
                };
        }

        protected MessageResponse Kick()
        {
            return new MessageResponse
            {
                ResponseType = MessageHandlerResultResponseType.Kick
            };
        }

        protected MessageResponse Message(string text, params object[] arguments)
        {
            return new MessageResponse
            {
                ResponseType = MessageHandlerResultResponseType.DefaultMessage,
                ResponseText = string.Format(text, arguments)
            };
        }
    }
}