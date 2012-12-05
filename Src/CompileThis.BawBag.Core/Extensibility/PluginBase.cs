namespace CompileThis.BawBag.Extensibility
{
    using Raven.Client;

    public abstract class PluginBase : IPlugin
    {
        public abstract bool ContinueProcessing { get; }

        public abstract string Name { get; }

        public abstract PluginPriority Priority { get; }

        public abstract void Initialize(IDocumentStore documentStore);
    }
}