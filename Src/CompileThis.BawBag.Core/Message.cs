using CompileThis.BawBag.Jabbr;

namespace CompileThis.BawBag
{
    public class Message
    {
        public bool IsBotAddressed { get; set; }
        public string Text { get; set; }
        public MessageType Type { get; set; }
        public JabbrUser User { get; set; }
        public JabbrRoom Room { get; set; }
    }
}