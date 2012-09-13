namespace CompileThis.BawBag
{
    using System.Collections.Generic;

    using JabbR.Client;

    using CompileThis.BawBag.Handlers;
    using JabbR.Client.Models;

    internal class MessageHandlerManager
    {
        private readonly JabbRClient _client;
        private readonly List<IMessageHandler> _handlers;
        
        public MessageHandlerManager(JabbRClient client)
        {
            _client = client;

            _handlers = new List<IMessageHandler> {new ChooseHandler()};
        }

        public void HandleMessage(Message message)
        {
            foreach (var handler in _handlers)
            {
                var result = handler.Execute(message);
                ExecuteResult(result, message.Room.Name);

                var continueProcessing = (!result.IsHandled || handler.ContinueProcessing);
                if (!continueProcessing)
                {
                    break;
                }
            }
        }

        private void ExecuteResult(MessageHandlerResult result, string roomName)
        {
            if (!result.IsHandled)
            {
                return;
            }

            switch (result.ResponseType)
            {
                case MessageHandlerResultResponseType.Message:
                    _client.Send(result.ResponseText, roomName);
                    break;

                case MessageHandlerResultResponseType.Action:
                    _client.Send(string.Format("/me {0}", result.ResponseText), roomName);
                    break;
            }
        }
    }
}
