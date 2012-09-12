namespace CompileThis.BawBag
{
    public interface IMessageHandler
    {
        int Priority { get; }
        bool ContinueProcessing { get; }
        bool Execute();
        void Initialize();
    }
}
