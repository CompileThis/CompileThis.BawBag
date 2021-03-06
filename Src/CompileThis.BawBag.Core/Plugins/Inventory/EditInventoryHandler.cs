﻿namespace CompileThis.BawBag.Plugins.Inventory
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    using CompileThis.BawBag.Extensibility;

    internal class EditInventoryHandler : MessageHandlerPluginBase
    {
        public EditInventoryHandler()
            : base("Edit Inventory", PluginPriority.High, continueProcessing: false, mustBeAddressed: false)
        { }

        protected override MessageHandlerResult ExecuteCore(Message message, IPluginContext context)
        {
            Match match = null;
            if (message.Type == MessageType.Action)
            {
                match = GetFirstMatch(
                    message.Text,
                    new Regex(@"^\s*puts\s+(?<item>.+?)\s+in\s+@?" + context.BotName + @"\b?.*$", RegexOptions.IgnoreCase),
                    new Regex(@"^\s*gives\s+@?" + context.BotName + @"\s+(?<item>.+?)\.?\s*$", RegexOptions.IgnoreCase),
                    new Regex(@"^\s*gives\s+(?<item>.+?)\s+to\s+@?" + context.BotName + @"\b?.*$", RegexOptions.IgnoreCase));
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
            var isDuplicate = context.InventoryManager.AddItem(context.Room, item, out droppedItem, context.TextProcessor);
            if (isDuplicate)
            {
                return Handled(Message("No thanks, @{0}, I've already got one.", context.User.Name));
            }

            if (droppedItem == null)
            {
                return Handled(Action("now contains {0}.", item.Value));
            }

            return Handled(Action("drops {0} and takes {1}.", droppedItem.Value, item.Value));
        }

        private static Match GetFirstMatch(string text, params Regex[] regexes)
        {
            return regexes.Select(regex => regex.Match(text)).FirstOrDefault(match => match.Success);
        }
    }
}
