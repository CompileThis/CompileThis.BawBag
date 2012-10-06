namespace CompileThis.BawBag.Extensibility
{
    public abstract class PluginBase : IPlugin
    {
        public abstract bool ContinueProcessing { get; }
        public abstract string Name { get; }
        public abstract PluginPriority Priority { get; }
        
        public abstract void Initialize();
    }
}