namespace CompileThis.BawBag.Jabbr
{
    using System;

    public class MessageEventArgs : EventArgs
    {
        private readonly ReceivedMessage _message;

        public MessageEventArgs(ReceivedMessage message)
        {
            _message = message;
        }

        public ReceivedMessage Message
        {
            get { return _message; }
        }
    }
}
