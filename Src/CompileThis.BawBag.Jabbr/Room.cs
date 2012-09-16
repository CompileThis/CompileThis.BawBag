namespace CompileThis.BawBag.Jabbr
{
    using CompileThis.BawBag.Jabbr.Collections;

    internal class Room : IRoom
    {
        private readonly LookupList<string, User> _users;
        private readonly LookupList<string, User> _owners;
 
        internal Room()
        {
            _users = new LookupList<string, User>(x => x.Name);
            _owners = new LookupList<string, User>(x => x.Name);
        }

        IReadOnlyLookupList<string, IUser> IRoom.Owners { get { return _owners; } }
        IReadOnlyLookupList<string, IUser> IRoom.Users { get { return _users; } }

        public bool IsClosed { get; set; }
        public bool IsPrivate { get; set; }
        public string Name { get; set; }
        public LookupList<string, User> Owners { get { return _owners; } }
        public string Topic { get; set; }
        public int UserCount { get; set; }
        public LookupList<string, User> Users { get { return _users; } }
        public string Welcome { get; set; }
    }
}
