namespace CompileThis.BawBag.Jabbr
{
    using System;

    internal class User : IUser
    {
        public string AfkNote { get; set; }
        public string Country { get; set; }
        public string Flag { get; set; }
        public string Id { set; get; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsAfk { get; set; }
        public DateTime LastActivity { get; set; }
        public string Note { get; set; }
        public UserStatus Status { get; set; }
    }
}