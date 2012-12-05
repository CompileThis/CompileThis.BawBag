namespace CompileThis.BawBag.Jabbr
{
    public class JabbrEventContext
    {
        private readonly Room _room;
        private readonly User _user;

        internal JabbrEventContext(Room room, User user)
        {
            this._room = room;
            this._user = user;
        }

        public Room Room
        {
            get { return this._room; }
        }

        public User User
        {
            get { return this._user; }
        }
    }
}