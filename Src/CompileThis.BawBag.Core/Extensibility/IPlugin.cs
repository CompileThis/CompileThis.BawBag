namespace CompileThis.BawBag.Extensibility
{
    using Raven.Client;

    public interface IPlugin
    {
        bool ContinueProcessing { get; }
        string Name { get; }
        PluginPriority Priority { get; }

        void Initialize(IDocumentStore documentStore);
    }
}
