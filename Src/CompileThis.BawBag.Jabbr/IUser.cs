namespace CompileThis.BawBag.Jabbr
{
    using System;

    public interface IUser
    {
        string AfkNote { get; }
        string Country { get; }
        string Flag { get; }
        string Id { get; }
        string Name { get; }
        bool IsActive { get; }
        bool IsAdmin { get; }
        bool IsAfk { get; }
        DateTime LastActivity { get; }
        string Note { get; }
        UserStatus Status { get; }
    }
}