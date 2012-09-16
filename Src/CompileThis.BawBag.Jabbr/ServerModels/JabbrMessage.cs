namespace CompileThis.BawBag.Jabbr.ServerModels
{
    using System;

    internal class JabbrMessage
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public DateTimeOffset When { get; set; }
        public JabbrUser User { get; set; }
    }
}