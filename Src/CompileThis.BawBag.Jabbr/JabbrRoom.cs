namespace CompileThis.BawBag.Jabbr
{
    using System.Collections.Generic;

    public class JabbrRoom
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public bool Private { get; set; }
        public string Topic { get; set; }
        public bool Closed { get; set; }
        public string Welcome { get; set; }
        public IEnumerable<JabbrUser> Users { get; set; }
        public IEnumerable<string> Owners { get; set; }
    }
}
