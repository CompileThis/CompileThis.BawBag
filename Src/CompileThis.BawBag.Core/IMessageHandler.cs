namespace CompileThis.BawBag
{
    public interface IMessageHandler
    {
        int Priority { get; }
        bool ContinueProcessing { get; }
        MessageHandlerResult Execute(Message message);
        void Initialize();
    }
}
