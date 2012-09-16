namespace CompileThis.BawBag.Jabbr
{
    using System;

    public class MessageEventArgs : EventArgs
    {
        private readonly Message _message;

        public MessageEventArgs(Message message)
        {
            _message = message;
        }

        public Message Message
        {
            get { return _message; }
        }
    }
}
