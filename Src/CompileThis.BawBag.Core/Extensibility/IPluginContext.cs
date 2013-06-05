namespace CompileThis.BawBag.Extensibility
{
    using Raven.Client;

    public interface IPluginContext
    {
        bool IsBotAddressed { get; }

        string BotName { get; }

        Room Room { get; }

        User User { get; }

        IDocumentSession RavenSession { get; }

        IRandomNumberProvider RandomProvider { get; }

        IInventoryManager InventoryManager { get; }

        ITextProcessor TextProcessor { get; }

        IDateTimeProvider DateTimeProvider { get; }
    }
}