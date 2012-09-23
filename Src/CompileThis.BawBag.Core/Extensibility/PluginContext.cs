namespace CompileThis.BawBag.Extensibility
{
    using System;

    using Raven.Client;

    using CompileThis.BawBag.Jabbr;
    
    public interface IPluginContext
    {
        bool IsBotAddressed { get; }
        Room Room { get; }
        User User { get; }
        IDocumentSession RavenSession { get; }
        Random RandomProvider { get; }
    }

    internal class PluginContext : IPluginContext
    {
        public bool IsBotAddressed { get; set; }
        public Room Room { get; set; }
        public User User { get; set; }
        public IDocumentSession RavenSession { get; set; }
        public Random RandomProvider { get; set; }
    }

    public class Message
    {
        public string Text { get; set; }
        public MessageType Type { get; set; }
    }
}
