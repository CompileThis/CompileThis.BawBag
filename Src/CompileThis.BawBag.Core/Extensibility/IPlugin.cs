namespace CompileThis.BawBag.Extensibility
{
    public interface IPlugin
    {
        bool ContinueProcessing { get; }
        string Name { get; }
        PluginPriority Priority { get; }

        void Initialize();
    }

    public interface  IMessageHandlerPlugin : IPlugin
    {
        MessageHandlerResult Execute(Message message, IPluginContext context);
    }

    public enum PluginPriority
    {
        High = 0,
        Normal = 1,
        Low = 2,
        Lowest = 3
    }

    public abstract class PluginBase : IPlugin
    {
        public abstract bool ContinueProcessing { get; }
        public abstract string Name { get; }
        public abstract PluginPriority Priority { get; }
        
        public abstract void Initialize();
    }

    public abstract class MessageHandlerPluginBase : PluginBase, IMessageHandlerPlugin
    {
        private readonly string _name;
        private readonly PluginPriority _priority;
        private readonly bool _continueProcessing;
        private readonly bool _mustBeAddressed;

        protected MessageHandlerPluginBase(string name, PluginPriority priority, bool continueProcessing, bool mustBeAddressed)
        {
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
            if (MustBeAddressed && !context.IsBotAddressed)
            {
                return MessageHandlerResult.NotHandled;
            }

            return ExecuteCore(message, context);
        }

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
    }

}
