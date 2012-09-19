namespace CompileThis.BawBag.Jabbr
{
    using System;

    public class ReceivedMessage
    {
        private readonly string _content;
        private readonly string _id;
        private readonly Room _room;
        private readonly DateTimeOffset _timestamp;
        private readonly MessageType _type;
        private readonly User _user;

        internal ReceivedMessage(string content, string id, Room room, DateTimeOffset timestamp, MessageType type, User user)
        {
            _content = content;
            _id = id;
            _room = room;
            _timestamp = timestamp;
            _type = type;
            _user = user;
        }

        public string Content
        {
            get { return _content; }
        }

        public string Id
        {
            get { return _id; }
        }

        public Room Room
        {
            get { return _room; }
        }

        public DateTimeOffset Timestamp
        {
            get { return _timestamp; }
        }

        public MessageType Type
        {
            get { return _type; }
        }

        public User User
        {
            get { return _user; }
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

    public class MessageContext
    {
        private readonly Room _room;
        private readonly User _user;

        internal MessageContext(Room room, User user)
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
