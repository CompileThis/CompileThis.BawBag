namespace CompileThis.BawBag.Jabbr
{
    using System;

    public class MessageReceivedEventArgs : EventArgs
    {
        private readonly JabbrMessage _message;
        private readonly JabbrRoom _room;

        public MessageReceivedEventArgs(JabbrMessage message, JabbrRoom room)
        {
            _message = message;
            _room = room;
        }

        public JabbrMessage Message
        {
            get { return _message; }
        }

        public JabbrRoom Room
        {
            get { return _room; }
        }
    }
}
