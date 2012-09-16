namespace CompileThis.BawBag.Jabbr
{
    using System;

    public class JoinedRoomEventArgs : EventArgs
    {
        private readonly IRoom _room;

        public JoinedRoomEventArgs(IRoom room)
        {
            _room = room;
        }

        public IRoom Room
        {
            get { return _room; }
        }
    }
}
