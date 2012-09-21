namespace CompileThis.BawBag.Extensibility
{
    public interface IPlugin
    {
        string Name { get; }
    }

    public enum PluginPriority
    {
        High,
        Normal,
        Low,
    }

    public abstract class PluginBase : IPlugin
    {
        public string Name
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}
