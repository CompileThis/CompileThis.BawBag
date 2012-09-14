namespace CompileThis.BawBag.Jabbr
{
    using System;

    public class LeftRoomEventArgs : EventArgs
    {
        private readonly JabbrRoom _room;
        private readonly JabbrUser _user;

        public LeftRoomEventArgs(JabbrRoom room, JabbrUser user)
        {
            _room = room;
            _user = user;
        }

        public JabbrRoom Room
        {
            get { return _room; }
        }

        public JabbrUser User
        {
            get { return _user; }
        }
    }

    public class AddUserEventArgs : EventArgs
    {
        private readonly JabbrRoom _room;
        private readonly JabbrUser _user;

        public AddUserEventArgs(JabbrRoom room, JabbrUser user)
        {
            _room = room;
            _user = user;
        }

        public JabbrRoom Room
        {
            get { return _room; }
        }

        public JabbrUser User
        {
            get { return _user; }
        }
    }
}