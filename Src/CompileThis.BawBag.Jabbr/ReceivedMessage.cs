namespace CompileThis.BawBag.Jabbr
{
    using System;

    public class ReceivedMessage
    {
        private readonly string _text;
        private readonly string _id;
        private readonly DateTimeOffset _timestamp;
        private readonly MessageType _type;

        internal ReceivedMessage(MessageType type, string text, string id, DateTimeOffset timestamp)
        {
            Guard.NullParameter(text, () => text);

            _text = text;
            _id = id;
            _timestamp = timestamp;
            _type = type;
        }

        public string Text
        {
            get { return _text; }
        }

        public string Id
        {
            get { return _id; }
        }

        public DateTimeOffset Timestamp
        {
            get { return _timestamp; }
        }

        public MessageType Type
        {
            get { return _type; }
        }
    }
}
