namespace CompileThis.BawBag.Jabbr
{
    using System;
    using System.Threading.Tasks;
    using CompileThis.BawBag.Jabbr.Collections;

    public interface IJabbrClient
    {
        event EventHandler<MessageEventArgs> MessageReceived;
        event EventHandler<LeftRoomEventArgs> UserLeftRoom;
        event EventHandler<AddUserEventArgs> UserJoinedRoom;

        IReadOnlyLookupList<string, Room> Rooms { get; }
        IReadOnlyLookupList<string, User> Users { get; }

        Task Connect(string username, string password);
        Task Disconnect();

        Task<Room> JoinRoom(string room);
        Task LeaveRoom(string room);

        Task SendMessage(string room, string message);
        Task SendAction(string room, string action);
        Task Kick(string username, string room);
    }
}