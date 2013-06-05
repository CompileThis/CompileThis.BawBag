namespace CompileThis.BawBag.Extensibility
{
  using System.Collections.Generic;
  using System.Linq;

  using JabbR.Client;

  public class MessageHandlerResult
  {
    private static readonly MessageHandlerResult NotHandledInstance = new MessageHandlerResult
                                                                          {
                                                                            IsHandled = false,
                                                                            Responses = Enumerable.Empty<MessageResponse>()
                                                                          };

    public static MessageHandlerResult NotHandled
    {
      get { return NotHandledInstance; }
    }

    public bool IsHandled { get; set; }

    public IEnumerable<MessageResponse> Responses { get; set; }

    internal async void Execute(IJabbRClient client, Room room)
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
            await client.Send(response.ResponseText, room.Name);
            break;

          case MessageHandlerResultResponseType.ActionMessage:
            await client.Send("/me " + response.ResponseText, room.Name);
            break;

          case MessageHandlerResultResponseType.Kick:
            await client.Kick(response.ResponseText, room.Name);
            break;
        }
      }
    }
  }
}
