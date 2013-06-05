namespace CompileThis.BawBag
{
  using System.Collections.Generic;

  public class Room
  {
    private readonly List<User> _users;

    private readonly Dictionary<string, User> _usersDictionary; 

    internal Room()
    {
      _users = new List<User>();
      _usersDictionary = new Dictionary<string, User>();
    }

    public string Name { get; internal set; }

    public bool IsPrivate { get; internal set; }

    public IReadOnlyList<User> Users
    {
      get
      {
        return _users;
      }
    }

    public User GetUserByName(string name)
    {
      return _usersDictionary[name];
    }

    internal static Room Create(JabbR.Client.Models.Room jabbrRoom)
    {
      var room = new Room
               {
                 Name = jabbrRoom.Name,
                 IsPrivate = jabbrRoom.Private,
               };

      foreach (var jabbrUser in jabbrRoom.Users)
      {
        room.AddUser(User.Create(jabbrUser));
      }

      return room;
    }

    internal void AddUser(User user)
    {
      _users.Add(user);
      _usersDictionary.Add(user.Name, user);
    }

    internal void RemoveUser(string name)
    {
      var user = _usersDictionary[name];

      _usersDictionary.Remove(name);
      _users.Remove(user);
    }
  }
}
