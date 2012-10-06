namespace CompileThis.BawBag.Extensibility.Internal
{
    using System.Collections.Generic;

    internal class Inventory
    {
        public string Id { get; set; }
        public string Room { get; set; }
        public List<InventoryItem> Items { get; set; }
    }
}