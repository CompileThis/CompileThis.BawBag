namespace CompileThis.BawBag.Jabbr
{
    using System;

    public class AddUserEventArgs : EventArgs
    {
        private readonly IRoom _room;
        private readonly IUser _user;

        public AddUserEventArgs(IRoom room, IUser user)
        {
            _room = room;
            _user = user;
        }

        public IRoom Room
        {
            get { return _room; }
        }

        public IUser User
        {
            get { return _user; }
        }
    }
}