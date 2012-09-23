namespace CompileThis.BawBag.Plugins.RandomChoices
{
    using System;
    using System.Linq;

    using CompileThis.BawBag.Extensibility;

    public class DickLottoHandler : MessageHandlerPluginBase
    {
        private static readonly Random RandomProvider = new Random();

        private DateTimeOffset _lastLotto;

        public DickLottoHandler()
            : base("DickLotto", PluginPriority.Normal, continueProcessing: false, mustBeAddressed: true)
        {
            _lastLotto = DateTimeOffset.MinValue;
        }

        protected override MessageHandlerResult ExecuteCore(Message message, IPluginContext context)
        {
            if (!message.Text.Equals("dicklotto", StringComparison.OrdinalIgnoreCase))
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

            var dickableUsers = (from u in context.Room.Users
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

        public override void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
