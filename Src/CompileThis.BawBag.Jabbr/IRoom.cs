namespace CompileThis.BawBag.Jabbr
{
    using CompileThis.BawBag.Jabbr.Collections;

    public interface IRoom
    {
        bool IsClosed { get; }
        bool IsPrivate { get; }
        string Name { get; }
        IReadOnlyLookupList<string, IUser> Owners { get; }
        string Topic { get; }
        int UserCount { get; }
        IReadOnlyLookupList<string, IUser> Users { get; }
        string Welcome { get; }
    }
}