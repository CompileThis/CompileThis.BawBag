namespace CompileThis.BawBag.Jabbr
{
    using System;
    using System.Threading.Tasks;

    using CompileThis.Collections.Generic;

    public interface IJabbrClient
    {
        event EventHandler<MessageReceivedEventArgs> MessageReceived;

        event EventHandler<LeftRoomEventArgs> UserLeftRoom;

        event EventHandler<AddUserEventArgs> UserJoinedRoom;

        IReadOnlyLookupList<string, Room> Rooms { get; }

        IReadOnlyLookupList<string, User> Users { get; }

        Task Connect(string username, string password);

        Task Disconnect();

        Task<Room> JoinRoom(string room);

        Task LeaveRoom(string room);

        Task SendDefaultMessage(string text, string roomName);

        Task SendActionMessage(string text, string roomName);

        Task Kick(string username, string room);
    }
}