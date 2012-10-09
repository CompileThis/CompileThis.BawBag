namespace CompileThis.BawBag.Extensibility
{
    using System.Collections.Generic;
    using CompileThis.BawBag.Jabbr;

    public interface IInventoryManager
    {
        bool AddItem(Room room, InventoryItem item, out InventoryItem droppedItem, ITextProcessor textProcessor);
        bool AddItem(string roomName, InventoryItem item, out InventoryItem droppedItem, ITextProcessor textProcessor);
        IEnumerable<InventoryItem> GetInventory(Room room);
        IEnumerable<InventoryItem> GetInventory(string roomName);
        InventoryItem RemoveRandomItem(Room room);
        InventoryItem RemoveRandomItem(string roomName);
    }
}