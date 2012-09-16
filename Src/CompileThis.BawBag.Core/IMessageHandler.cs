namespace CompileThis.BawBag
{
    public interface IMessageHandler
    {
        string Name { get; }
        int Priority { get; }
        bool ContinueProcessing { get; }
        MessageHandlerResult Execute(MessageContext message);
        void Initialize();
    }
}
