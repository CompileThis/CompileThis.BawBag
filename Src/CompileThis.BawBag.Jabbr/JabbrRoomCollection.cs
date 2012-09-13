namespace CompileThis.BawBag.Jabbr
{
    using System.Collections.ObjectModel;

    public class JabbrRoomCollection : KeyedCollection<string, JabbrRoom>
    {
        protected override string GetKeyForItem(JabbrRoom item)
        {
            return item.Name;
        }
    }
}
