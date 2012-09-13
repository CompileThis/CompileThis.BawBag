namespace CompileThis.BawBag
{
    using System.Collections.Generic;
    using System.Linq;

    public class MessageHandlerResult
    {
        private static readonly MessageHandlerResult NotHandledInstance = new MessageHandlerResult {IsHandled = false, Responses = Enumerable.Empty<MessageResponse>()};

        public bool IsHandled { get; set; }
        public IEnumerable<MessageResponse> Responses { get; set; }

        public static MessageHandlerResult NotHandled
        {
            get { return NotHandledInstance; }
        }
    }
}
