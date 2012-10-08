namespace CompileThis.BawBag.Extensibility
{
    using System.Collections.Generic;
    using System.Linq;

    using CompileThis.BawBag.Jabbr;

    public class MessageHandlerResult
    {
        private static readonly MessageHandlerResult NotHandledInstance = new MessageHandlerResult {IsHandled = false, Responses = Enumerable.Empty<MessageResponse>()};

        public bool IsHandled { get; set; }
        public IEnumerable<MessageResponse> Responses { get; set; }

        internal async void Execute(IJabbrClient client, Room room)
        {
            Guard.NullParameter(client, () => client);
            Guard.NullParameter(room, () => room);

            if (!IsHandled)
            {
                return;
            }

            foreach (var response in Responses)
            {
                switch (response.ResponseType)
                {
                    case MessageHandlerResultResponseType.DefaultMessage:
                        await client.SendDefaultMessage(response.ResponseText, room.Name);
                        break;

                    case MessageHandlerResultResponseType.ActionMessage:
                        await client.SendActionMessage(response.ResponseText, room.Name);
                        break;

                    case MessageHandlerResultResponseType.Kick:
                        await client.Kick(response.ResponseText, room.Name);
                        break;
                }
            }
        }

        public static MessageHandlerResult NotHandled
        {
            get { return NotHandledInstance; }
        }
    }
}
