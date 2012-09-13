namespace CompileThis.BawBag
{
    using System.Collections.ObjectModel;

    internal class RoomCollection : KeyedCollection<string, Room>
    {
        protected override string GetKeyForItem(Room item)
        {
            return item.Name;
        }
    }
}
