namespace CompileThis.BawBag.Jabbr.ServerModels
{
    using System;
    using CompileThis.BawBag.Jabbr.Collections;

    internal static class ServerModelConverter
    {
        public static Room ToRoom(JabbrRoom jabbrRoom, IJabbrClient client, LookupList<string, User> users)
        {
            var room = new Room(client)
                {
                    Name = jabbrRoom.Name,
                    IsClosed = jabbrRoom.Closed,
                    IsPrivate = jabbrRoom.Private,
                    Topic = jabbrRoom.Topic,
                    UserCount = jabbrRoom.Count,
                    Welcome = jabbrRoom.Welcome
                };

            foreach (var jabbrUser in jabbrRoom.Users)
            {
                var user = ToUser(jabbrUser, users);
                room.AddUser(user);
            }

            foreach (var username in jabbrRoom.Owners)
            {
                var user = users[username];
                room.AddOwner(user);
            }

            return room;
        }

        public static User ToUser(JabbrUser jabbrUser, LookupList<string, User> users)
        {
            var user = users.GetValueOrDefault(jabbrUser.Name);
            if (user == null)
            {
                user = new User {Name = jabbrUser.Name};
                users.Add(user);
            }

            user.AfkNote = jabbrUser.AfkNote;
            user.Country = jabbrUser.Country;
            user.Flag = jabbrUser.Flag;
            user.Id = jabbrUser.Hash;
            user.IsActive = jabbrUser.Active;
            user.IsAdmin = jabbrUser.IsAdmin;
            user.IsAfk = jabbrUser.IsAfk;
            user.LastActivity = jabbrUser.LastActivity;
            user.Note = jabbrUser.Node;
            user.Status = ToUserStatus(jabbrUser.Status);

            return user;
        }

        public static UserStatus ToUserStatus(JabbrUserStatus jabbrUserStatus)
        {
            switch (jabbrUserStatus)
            {
                case JabbrUserStatus.Active:
                    return UserStatus.Active;

                case JabbrUserStatus.Inactive:
                    return UserStatus.Inactive;

                case JabbrUserStatus.Offline:
                    return UserStatus.Offline;
            }

            throw new ArgumentException(string.Format("Unknown JabbR user status '{0}'.", jabbrUserStatus), "jabbrUserStatus");
        }
    }
}
