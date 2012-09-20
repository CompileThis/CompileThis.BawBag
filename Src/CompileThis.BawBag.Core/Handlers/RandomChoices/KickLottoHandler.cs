namespace CompileThis.BawBag.Handlers.RandomChoices
{
    using System;
    using System.Linq;

    public class KickLottoHandler : IMessageHandler
    {
        private static readonly Random RandomProvider = new Random();

        private DateTimeOffset _lastLotto;

        public KickLottoHandler()
        {
            _lastLotto = DateTimeOffset.MinValue;
        }

        public string Name
        {
            get { return "KickLotto"; }
        }

        public int Priority
        {
            get { return 103; }
        }

        public bool ContinueProcessing
        {
            get { return false; }
        }

        public MessageHandlerResult Execute(MessageContext message, MessageHandlerContext context)
        {
            if (!message.IsBotAddressed || !message.Content.Equals("kicklotto", StringComparison.OrdinalIgnoreCase))
            {
                return MessageHandlerResult.NotHandled;
            }

            var now = DateTimeOffset.Now;

            if (now - _lastLotto < TimeSpan.FromHours(1))
            {
                var response = new MessageResponse
                {
                    ResponseType = MessageHandlerResultResponseType.DefaultMessage,
                    ResponseText = "Only one lotto per hour bitch!"
                };

                return new MessageHandlerResult
                    {
                        IsHandled = true,
                        Responses = new[] {response}
                    };
            }

            var kickableUsers = (from u in message.Room.Users
                                 where !u.IsAdmin && u.Name != "BawBag" && !message.Room.Owners.Contains(u.Name)
                                 select u).ToList();

            if (kickableUsers.Count == 0)
            {
                var response = new MessageResponse
                    {
                        ResponseType = MessageHandlerResultResponseType.DefaultMessage,
                        ResponseText = "There's no-one to kick you lonely bastard!"
                    };

                return new MessageHandlerResult
                    {
                        IsHandled = true,
                        Responses = new[] {response}
                    };
            }

            _lastLotto = now;

            var selectedIndex = RandomProvider.Next(0, kickableUsers.Count);
            var selectedUser = kickableUsers[selectedIndex];

            var responseMessage = new MessageResponse
                {
                    ResponseType = MessageHandlerResultResponseType.DefaultMessage,
                    ResponseText = string.Format("Congratulations @{0}, you've just won the kicklotto!", selectedUser.Name)
                };

            var reponseKick = new MessageResponse
                {
                    ResponseType = MessageHandlerResultResponseType.Kick,
                    ResponseText = selectedUser.Name
                };

            return new MessageHandlerResult
                {
                    IsHandled = true,
                    Responses = new[] {responseMessage, reponseKick}
                };
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
