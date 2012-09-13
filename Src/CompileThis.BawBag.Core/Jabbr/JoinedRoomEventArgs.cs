namespace CompileThis.BawBag.Jabbr
{
    using System;

    public class JoinedRoomEventArgs : EventArgs
    {
        private readonly JabbrRoom _room;

        public JoinedRoomEventArgs(JabbrRoom room)
        {
            _room = room;
        }

        public JabbrRoom Room
        {
            get { return _room; }
        }
    }
}
