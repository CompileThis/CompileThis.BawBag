namespace CompileThis.BawBag.Jabbr
{
    using System;
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

    public class JabbrUser
    {
        public string Hash { get; set; }
        public string Name { get; set; }
        public JabbrUserStatus Status { get; set; }
    }

    public enum JabbrUserStatus
    {
        Active = 0,
        Inactive = 1,
        Offline = 2
    }

    public class JabbrMessage
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public DateTimeOffset When { get; set; }
        public JabbrUser User { get; set; }
    }
}
