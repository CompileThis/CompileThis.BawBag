namespace CompileThis.BawBag.Jabbr
{
    using System;
    using System.Threading.Tasks;
    using CompileThis.BawBag.Jabbr.Collections;

    public interface IJabbrClient
    {
        event EventHandler<EventArgs> LoggedOn;
        event EventHandler<JoinedRoomEventArgs> JoinedRoom;
        event EventHandler<MessageEventArgs> MessageReceived;
        event EventHandler<LeftRoomEventArgs> LeftRoom;
        event EventHandler<AddUserEventArgs> AddUser;

        IReadOnlyLookupList<string, IRoom> Rooms { get; }
        IReadOnlyLookupList<string, IUser> Users { get; }

        Task Connect(string username, string password);
        Task Disconnect();

        Task JoinRoom(string room);
        Task LeaveRoom(string room);

        Task SendMessage(string room, string message);
        Task SendAction(string room, string action);
        Task Kick(string username, string room);
    }
}