namespace CompileThis.BawBag.Jabbr
{
    using System;

    public class ActionReceivedEventArgs : EventArgs
    {
        private readonly JabbrUser _user;
        private readonly JabbrRoom _room;
        private readonly string _actionText;

        public ActionReceivedEventArgs(JabbrUser user, JabbrRoom room, string actionText)
        {
            _user = user;
            _room = room;
            _actionText = actionText;
        }

        public JabbrUser User
        {
            get { return _user; }
        }

        public JabbrRoom Room
        {
            get { return _room; }
        }

        public string ActionText
        {
            get { return _actionText; }
        }
    }
}
