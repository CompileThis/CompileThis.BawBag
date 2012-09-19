namespace CompileThis.BawBag.Jabbr
{
    using System;

    public class LeftRoomEventArgs : EventArgs
    {
        private readonly Room _room;
        private readonly User _user;

        public LeftRoomEventArgs(Room room, User user)
        {
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