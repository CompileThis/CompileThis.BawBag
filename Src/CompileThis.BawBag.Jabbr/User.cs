namespace CompileThis.BawBag.Jabbr
{
    using System;

    public class User
    {
        internal User()
        { }

        public string AfkNote { get; internal set; }
        public string Country { get; internal set; }
        public string Flag { get; internal set; }
        public string Id { set; internal get; }
        public string Name { get; internal set; }
        public bool IsActive { get; internal set; }
        public bool IsAdmin { get; internal set; }
        public bool IsAfk { get; internal set; }
        public DateTime LastActivity { get; internal set; }
        public string Note { get; internal set; }
        public UserStatus Status { get; internal set; }
    }
}