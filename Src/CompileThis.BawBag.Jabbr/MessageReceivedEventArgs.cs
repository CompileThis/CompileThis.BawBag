namespace CompileThis.BawBag.Jabbr
{
    using System;

    public class MessageReceivedEventArgs : EventArgs
    {
        private readonly ReceivedMessage _message;
        private readonly JabbrEventContext _context;

        internal MessageReceivedEventArgs(ReceivedMessage message, JabbrEventContext context)
        {
            Guard.NullParameter(message, () => message);
            Guard.NullParameter(context, () => context);

            _message = message;
            _context = context;
        }

        public ReceivedMessage Message
        {
            get { return _message; }
        }

        public JabbrEventContext Context
        {
            get { return _context; }
        }
    }
}
