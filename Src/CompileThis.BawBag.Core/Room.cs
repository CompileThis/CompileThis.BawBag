namespace CompileThis.BawBag
{
    using System;

    public class Room
    {
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
        public string Topic { get; set; }
        public int UserCount { get; set; }

        public UserCollection Users { get; set; }
        public string[] Owners { get; set; }

        public bool IsRoomOperator(User user)
        {
            return IsRoomOperator(user.Id);
        }

        public bool IsRoomOperator(string userId)
        {
            throw new NotImplementedException();
        }

        public bool IsInRoom(User user)
        {
            return IsInRoom(user.Id);
        }

        public bool IsInRoom(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
