namespace CompileThis.BawBag
{
    public class MessageHandlerResult
    {
        private static readonly MessageHandlerResult NotHandledInstance = new MessageHandlerResult {IsHandled = false};

        public bool IsHandled { get; set; }
        public MessageHandlerResultResponseType ResponseType { get; set; }
        public string ResponseText { get; set; }

        public static MessageHandlerResult NotHandled
        {
            get { return NotHandledInstance; }
        }
    }

    public enum MessageHandlerResultResponseType
    {
        Message = 0,
        Action = 1
    }
}
