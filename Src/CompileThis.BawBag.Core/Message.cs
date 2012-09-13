namespace CompileThis.BawBag
{
    public class Message
    {
        public bool IsBotAddressed { get; set; }
        public string Text { get; set; }
        public MessageType Type { get; set; }
        public User User { get; set; }
        public Room Room { get; set; }
    }
}