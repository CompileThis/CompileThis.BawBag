namespace CompileThis.BawBag.Jabbr.ServerModels
{
    using System;

    internal class JabbrUser
    {
        public string Name { get; set; }

        public string Hash { get; set; }

        public bool Active { get; set; }

        public JabbrUserStatus Status { get; set; }

        public string Node { get; set; }

        public string AfkNote { get; set; }

        public bool IsAfk { get; set; }

        public string Flag { get; set; }

        public string Country { get; set; }

        public DateTime LastActivity { get; set; }

        public bool IsAdmin { get; set; }
    }
}