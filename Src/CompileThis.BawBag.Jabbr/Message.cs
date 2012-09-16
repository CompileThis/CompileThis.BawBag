namespace CompileThis.BawBag.Jabbr
{
    using System;

    public class Message
    {
        private readonly string _content;
        private readonly string _id;
        private readonly IRoom _room;
        private readonly DateTimeOffset _timestamp;
        private readonly MessageType _type;
        private readonly IUser _user;

        public Message(string content, string id, IRoom room, DateTimeOffset timestamp, MessageType type, IUser user)
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

        public IRoom Room
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

        public IUser User
        {
            get { return _user; }
        }
    }
}
