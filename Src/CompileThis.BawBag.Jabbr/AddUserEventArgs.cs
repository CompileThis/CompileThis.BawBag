namespace CompileThis.BawBag.Jabbr
{
    using System;

    public class AddUserEventArgs : EventArgs
    {
        private readonly Room _room;
        private readonly User _user;

        public AddUserEventArgs(Room room, User user)
        {
            Guard.NullParameter(room, () => room);
            Guard.NullParameter(user, () => user);

            _room = room;
            _user = user;
        }

        public Room Room
        {
            get { return _room; }
        }

        public User User
        {
            get { return _user; }
        }
    }
}