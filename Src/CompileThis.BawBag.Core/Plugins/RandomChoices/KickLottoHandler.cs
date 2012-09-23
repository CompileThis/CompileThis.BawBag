namespace CompileThis.BawBag.Plugins.RandomChoices
{
    using System;
    using System.Linq;

    using CompileThis.BawBag.Extensibility;

    public class KickLottoHandler : MessageHandlerPluginBase
    {
        private DateTimeOffset _lastLotto;

        public KickLottoHandler()
            : base("KickLotto", PluginPriority.Normal, continueProcessing: false, mustBeAddressed: true)
        {
            _lastLotto = DateTimeOffset.MinValue;
        }

        protected override MessageHandlerResult ExecuteCore(Message message, IPluginContext context)
        {
            if (!message.Text.Equals("kicklotto", StringComparison.OrdinalIgnoreCase))
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

                return Handled(response);
            }

            var kickableUsers = (from u in context.Room.Users
                                 where !u.IsAdmin && u.Name != "BawBag" && !context.Room.Owners.Contains(u.Name)
                                 select u).ToList();

            if (kickableUsers.Count == 0)
            {
                var response = new MessageResponse
                    {
                        ResponseType = MessageHandlerResultResponseType.DefaultMessage,
                        ResponseText = "There's no-one to kick you lonely bastard!"
                    };

                return Handled(response);
            }

            _lastLotto = now;

            var selectedIndex = context.RandomProvider.Next(0, kickableUsers.Count);
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

            return Handled(responseMessage, reponseKick);
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
