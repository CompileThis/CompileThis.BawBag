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

    public class Message
    {
        private readonly string _text;
        private readonly MessageType _type;

        public Message(MessageType type, string text)
        {
            _type = type;
            _text = text;
        }

        public string Text
        {
            get { return _text; }
        }

        public MessageType Type
        {
            get { return _type; }
        }
    }

    public class JabbrEventContext
    {
        private readonly Room _room;
        private readonly User _user;

        internal JabbrEventContext(Room room, User user)
        {
            _room = room;
            _user = user;
        }

        public Room Room
        {
            get { return _room; }
        }

        public User User
        {
            get { return _user; }
        }
    }
}
