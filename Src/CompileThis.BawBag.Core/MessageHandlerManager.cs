namespace CompileThis.BawBag
{
    using System.Collections.Generic;

    using CompileThis.BawBag.Handlers;
    using CompileThis.BawBag.Jabbr;

    internal class MessageHandlerManager
    {
        private readonly IJabbrClient _client;
        private readonly List<IMessageHandler> _handlers;

        public MessageHandlerManager(IJabbrClient client)
        {
            _client = client;

            _handlers = new List<IMessageHandler> {new ChooseHandler()};
        }

        public void HandleMessage(Message message)
        {
            foreach (var handler in _handlers)
            {
                var result = handler.Execute(message);
                ExecuteResult(result, message.Room.Name, message.User.Name);

                var continueProcessing = (!result.IsHandled || handler.ContinueProcessing);
                if (!continueProcessing)
                {
                    break;
                }
            }
        }

        private async void ExecuteResult(MessageHandlerResult result, string roomName, string userName)
        {
            if (!result.IsHandled)
            {
                return;
            }

            foreach (var response in result.Responses)
            {
                switch (response.ResponseType)
                {
                    case MessageHandlerResultResponseType.Message:
                        await _client.SendMessage(roomName, response.ResponseText);
                        break;

                    case MessageHandlerResultResponseType.Action:
                        //await _client.Send(string.Format("/me {0}", response.ResponseText), roomName);
                        break;

                    case MessageHandlerResultResponseType.Kick:
                        //await _client.Kick(userName, roomName);
                        break;
                }
            }
        }
    }
}
