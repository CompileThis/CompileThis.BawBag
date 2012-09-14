namespace CompileThis.BawBag.Jabbr
{
    using System;

    public class JabbrMessage
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public DateTimeOffset When { get; set; }
        public JabbrUser User { get; set; }
    }
}