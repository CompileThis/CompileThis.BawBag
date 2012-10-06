namespace CompileThis.BawBag.Extensibility
{
    public interface IPlugin
    {
        bool ContinueProcessing { get; }
        string Name { get; }
        PluginPriority Priority { get; }

        void Initialize();
    }
}
