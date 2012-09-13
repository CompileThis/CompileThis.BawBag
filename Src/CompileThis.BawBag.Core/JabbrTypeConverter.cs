namespace CompileThis.BawBag
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CompileThis.BawBag.Jabbr;

    internal static class JabbrTypeConverter
    {
        public static Room ConvertRoom(JabbrRoom jabbrRoom)
        {
            return new Room
                {
                    Name = jabbrRoom.Name,
                    IsPrivate = jabbrRoom.Private,
                    Topic = jabbrRoom.Topic,
                    UserCount = jabbrRoom.Count,
                    Users = ConvertUsers(jabbrRoom.Users),
                    Owners = (jabbrRoom.Owners ?? new string[]{}).ToArray()
                };
        }

        public static User ConvertUser(JabbrUser jabbrUser)
        {
            return new User
                {
                    Id = jabbrUser.Hash,
                    Name = jabbrUser.Name,
                    Status = ConvertUserStatus(jabbrUser.Status)
                };
        }

        public static UserStatus ConvertUserStatus(JabbrUserStatus jabbrStatus)
        {
            switch (jabbrStatus)
            {
                case JabbrUserStatus.Active:
                    return UserStatus.Active;

                case JabbrUserStatus.Inactive:
                    return UserStatus.Inactive;

                case JabbrUserStatus.Offline:
                    return UserStatus.Offline;
            }

            throw new ArgumentException(string.Format("Unknown JabbR status '{0}'.", jabbrStatus));
        }

        public static UserCollection ConvertUsers(IEnumerable<JabbrUser> jabbrUsers)
        {
            if (jabbrUsers == null)
            {
                return new UserCollection();
            }

            var users = new UserCollection();
            
            foreach (var jabbrUser in jabbrUsers)
            {
                var user = ConvertUser(jabbrUser);
                users.Add(user);
            }

            return users;
        }
    }
}