namespace CompileThis.BawBag.Jabbr
{
    using System.Threading.Tasks;

    using CompileThis.BawBag.Jabbr.Collections;

    public class Room
    {
        private readonly IJabbrClient _client;

        private readonly LookupList<string, User> _users;
        private readonly LookupList<string, User> _owners;
 
        internal Room(IJabbrClient client)
        {
            _client = client;

            _users = new LookupList<string, User>(x => x.Name);
            _owners = new LookupList<string, User>(x => x.Name);
        }

        public bool IsClosed { get; internal set; }
        public bool IsPrivate { get; internal set; }
        public string Name { get; internal set; }
        public IReadOnlyLookupList<string, User> Owners { get { return _owners; } }
        public string Topic { get; internal set; }
        public int UserCount { get; internal set; }
        public IReadOnlyLookupList<string, User> Users { get { return _users; } }
        public string Welcome { get; internal set; }

        public Task Kick(User user)
        {
            return _client.Kick(user.Name, Name);
        }

        internal  void AddOwner(User owner)
        {
            _owners.Add(owner);
        }

        internal void AddUser(User user)
        {
            _users.Add(user);
        }

        internal  void RemoveOwner(User owner)
        {
            _owners.Remove(owner);
        }

        internal void RemoveUser(User user)
        {
            _users.Remove(user);
        }
    }
}
