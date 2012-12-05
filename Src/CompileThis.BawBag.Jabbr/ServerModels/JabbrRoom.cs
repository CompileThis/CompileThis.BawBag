namespace CompileThis.BawBag.Jabbr.ServerModels
{
    using System.Collections.Generic;

    internal class JabbrRoom
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
