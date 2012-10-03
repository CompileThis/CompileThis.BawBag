namespace CompileThis.BawBag.Plugins.Inventory
{
    using System;
    using System.Linq;
    using CompileThis.BawBag.Extensibility;
    using System.Text.RegularExpressions;
    using CompileThis.BawBag.Jabbr;

    class EditInventoryHandler : MessageHandlerPluginBase
    {
        public EditInventoryHandler()
            : base("Edit Inventory", PluginPriority.High, continueProcessing: false, mustBeAddressed: false)
        { }

        protected override MessageHandlerResult ExecuteCore(Message message, IPluginContext context)
        {
            Match match = null;
            if (message.Type == MessageType.Action)
            {
                match = GetFirstMatch(message.Text,
                                      new Regex(@"^\s*puts\s+(?<item>.+?)\s+in\s+@?" + context.BotName + "\b?.*$"));
            }
            else if (message.Type == MessageType.Default)
            {
                
            }

            if (match == null)
            {
                return NotHandled();
            }

            var item = new InventoryItem
                {
                    Value = match.Groups["item"].Value,
                    CreatedBy = context.User.Name,
                    CreatedOn = DateTime.Now
                };

            InventoryItem droppedItem;
            var success = context.InventoryManager.AddItem(context.Room, item, out droppedItem);

            return
                Handled(new MessageResponse
                    {
                        ResponseType = MessageHandlerResultResponseType.DefaultMessage,
                        ResponseText = "Bibble"
                    });
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

        private static Match GetFirstMatch(string text, params Regex[] regexes)
        {
            return regexes.Select(regex => regex.Match(text)).FirstOrDefault(match => match.Success);
        }
    }
}
