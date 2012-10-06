namespace CompileThis.BawBag.Extensibility
{
    public interface  IMessageHandlerPlugin : IPlugin
    {
        MessageHandlerResult Execute(Message message, IPluginContext context);
    }
}