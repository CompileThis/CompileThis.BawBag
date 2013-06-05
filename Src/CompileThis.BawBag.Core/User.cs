namespace CompileThis.BawBag
{
  using System;

  public class User
  {
    public string Name { get; internal set; }

    public string Hash { get; internal set; }

    public bool IsActive { get; internal set; }

    public bool IsAdmin { get; internal set; }

    public bool IsAfk { get; internal set; }

    public DateTime LastActivity { get; internal set; }

    public string Note { get; internal set; }

    public static User Create(JabbR.Client.Models.User jabbrUser)
    {
      return new User
               {
                 IsActive = jabbrUser.Active,
                 Hash = jabbrUser.Hash,
                 IsAdmin = jabbrUser.IsAdmin,
                 IsAfk = jabbrUser.IsAfk,
                 LastActivity = jabbrUser.LastActivity,
                 Name = jabbrUser.Name,
                 Note = jabbrUser.Note
               };
    }
  }
}
