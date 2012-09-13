namespace CompileThis.BawBag
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class JabbrTypeConverter
    {
        public static Room ConvertRoom(JabbR.Client.Models.Room jabbrRoom)
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

        public static User ConvertUser(JabbR.Client.Models.User jabbrUser)
        {
            return new User
                {
                    Id = jabbrUser.Hash,
                    Name = jabbrUser.Name,
                    Status = ConvertUserStatus(jabbrUser.Status)
                };
        }

        public static UserStatus ConvertUserStatus(JabbR.Client.Models.UserStatus jabbrStatus)
        {
            switch (jabbrStatus)
            {
                case JabbR.Client.Models.UserStatus.Active:
                    return UserStatus.Active;

                case JabbR.Client.Models.UserStatus.Inactive:
                    return UserStatus.Inactive;

                case JabbR.Client.Models.UserStatus.Offline:
                    return UserStatus.Offline;
            }

            throw new ArgumentException(string.Format("Unknown JabbR status '{0}'.", jabbrStatus));
        }

        public static UserCollection ConvertUsers(IEnumerable<JabbR.Client.Models.User> jabbrUsers)
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