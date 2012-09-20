namespace CompileThis.BawBag.Handlers.RandomChoices
{
    using System;
    using System.Linq;

    public class DickLottoHandler : IMessageHandler
    {
        private static readonly Random RandomProvider = new Random();

        private DateTimeOffset _lastLotto;

        public DickLottoHandler()
        {
            _lastLotto = DateTimeOffset.MinValue;
        }

        public string Name
        {
            get { return "KickLotto"; }
        }

        public int Priority
        {
            get { return 109; }
        }

        public bool ContinueProcessing
        {
            get { return false; }
        }

        public MessageHandlerResult Execute(MessageContext message, MessageHandlerContext context)
        {
            if (!message.IsBotAddressed || !message.Content.Equals("dicklotto", StringComparison.OrdinalIgnoreCase))
            {
                return MessageHandlerResult.NotHandled;
            }

            var now = DateTimeOffset.Now;

            if (now - _lastLotto < TimeSpan.FromHours(1))
            {
                var response = new MessageResponse
                {
                    ResponseType = MessageHandlerResultResponseType.DefaultMessage,
                    ResponseText = "Only one buggering per hour bitch!"
                };

                return new MessageHandlerResult
                    {
                        IsHandled = true,
                        Responses = new[] {response}
                    };
            }

            var dickableUsers = (from u in message.Room.Users
                                 where u.Name != "BawBag"
                                 select u).ToList();

            _lastLotto = now;

            var selectedIndex = RandomProvider.Next(0, dickableUsers.Count);
            var selectedUser = dickableUsers[selectedIndex];

            var responseMessage = new MessageResponse
                {
                    ResponseType = MessageHandlerResultResponseType.DefaultMessage,
                    ResponseText = string.Format("Congratulations @{0}, you've just won the dicklotto!", selectedUser.Name)
                };

            var reponseAction = new MessageResponse
                {
                    ResponseType = MessageHandlerResultResponseType.ActionMessage,
                    ResponseText = string.Format("removes botpants and enthusiastically sodomises @{0}...", selectedUser.Name)
                };

            return new MessageHandlerResult
                {
                    IsHandled = true,
                    Responses = new[] {responseMessage, reponseAction}
                };
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
