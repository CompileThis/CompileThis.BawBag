namespace CompileThis.BawBag
{
    using CompileThis.BawBag.Jabbr;

    public class MessageContext
    {
        public bool IsBotAddressed { get; set; }
        public string Content { get; set; }
        public Room Room { get; set; }
        public MessageType Type { get; set; }
        public User User { get; set; }
    }
}