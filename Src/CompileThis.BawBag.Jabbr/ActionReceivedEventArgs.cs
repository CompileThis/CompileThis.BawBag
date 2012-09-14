namespace CompileThis.BawBag.Jabbr
{
    using System;

    public class ActionReceivedEventArgs : EventArgs
    {
        private readonly JabbrRoom _room;
        private readonly JabbrUser _user;
        private readonly string _content;
        
        public ActionReceivedEventArgs(JabbrRoom room, JabbrUser user, string content)
        {
            _room = room;
            _user = user;
            _content = content;
        }

        public JabbrRoom Room
        {
            get { return _room; }
        }

        public JabbrUser User
        {
            get { return _user; }
        }

        public string Content
        {
            get { return _content; }
        }
    }
}