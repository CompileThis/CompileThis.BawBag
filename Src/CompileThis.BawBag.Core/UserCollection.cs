namespace CompileThis.BawBag
{
    using System.Collections.ObjectModel;

    public class UserCollection : KeyedCollection<string, User>
    {
        protected override string GetKeyForItem(User item)
        {
            return item.Name;
        }
    }
}