namespace CompileThis.BawBag
{
    using CompileThis.BawBag.Jabbr;

    public class MessageContext
    {
        public bool IsBotAddressed { get; set; }
        public string Content { get; set; }
        public IRoom Room { get; set; }
        public MessageType Type { get; set; }
        public IUser User { get; set; }
    }
}