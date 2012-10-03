namespace CompileThis.BawBag.Extensibility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Raven.Client;

    using CompileThis.BawBag.Jabbr;
    

    public interface IInventoryManager
    {
        bool AddItem(Room room, InventoryItem item, out InventoryItem droppedItem);
        bool AddItem(string roomName, InventoryItem item, out InventoryItem droppedItem);
        IEnumerable<InventoryItem> GetInventory(Room room);
        IEnumerable<InventoryItem> GetInventory(string roomName);
        InventoryItem RemoveRandomItem(Room room);
        InventoryItem RemoveRandomItem(string roomName);
    }

    internal class InventoryManager : IInventoryManager
    {
        private readonly int _inventoryCapacity;
        private readonly IDocumentStore _documentStore;
        private readonly IRandomNumberProvider _randomNumberProvider;

        public InventoryManager(int inventoryCapacity, IDocumentStore documentStore, IRandomNumberProvider randomNumberProvider)
        {
            _inventoryCapacity = inventoryCapacity;
            _documentStore = documentStore;
            _randomNumberProvider = randomNumberProvider;
        }

        public bool AddItem(Room room, InventoryItem item, out InventoryItem droppedItem)
        {
            return AddItem(room.Name, item, out droppedItem);
        }

        public bool AddItem(string roomName, InventoryItem item, out InventoryItem droppedItem)
        {
            using (var session = _documentStore.OpenSession())
            {
                var inventory = session.Query<Inventory>().SingleOrDefault(x => x.Room == roomName);
                if (inventory == null)
                {
                    inventory = new Inventory
                    {
                        Room = roomName,
                        Items = new List<InventoryItem>()
                    };

                    session.Store(inventory);
                }

                droppedItem = null;

                if (inventory.Items.Any(x => x.Value == item.Value))
                {
                    session.SaveChanges();

                    return false;
                }

                if (inventory.Items.Count >= _inventoryCapacity)
                {
                    var itemIndex = _randomNumberProvider.Next(_inventoryCapacity);
                    droppedItem = inventory.Items[itemIndex];
                    inventory.Items.RemoveAt(itemIndex);
                }

                inventory.Items.Add(item);
                session.SaveChanges();

                return true;
            }
        }

        public IEnumerable<InventoryItem> GetInventory(Room room)
        {
            return GetInventory(room.Name);
        }

        public IEnumerable<InventoryItem> GetInventory(string roomName)
        {
            using (var session = _documentStore.OpenSession())
            {
                var inventory = session.Query<Inventory>().SingleOrDefault(x => x.Room == roomName);
                if (inventory == null)
                {
                    inventory = new Inventory
                        {
                            Room = roomName,
                            Items = new List<InventoryItem>()
                        };

                    session.Store(inventory);
                    session.SaveChanges();
                }

                return inventory.Items;
            }
        }

        public InventoryItem RemoveRandomItem(Room room)
        {
            return RemoveRandomItem(room.Name);
        }

        public InventoryItem RemoveRandomItem(string roomName)
        {
            using (var session = _documentStore.OpenSession())
            {
                var inventory = session.Query<Inventory>().SingleOrDefault(x => x.Room == roomName);
                if (inventory == null)
                {
                    inventory = new Inventory
                        {
                            Room = roomName,
                            Items = new List<InventoryItem>()
                        };

                    session.Store(inventory);
                    session.SaveChanges();

                    return null;
                }

                if (inventory.Items.Count == 0)
                {
                    return null;
                }

                var itemIndex = _randomNumberProvider.Next(inventory.Items.Count);

                var item = inventory.Items[itemIndex];
                inventory.Items.RemoveAt(itemIndex);

                session.SaveChanges();

                return item;
            }
        }
    }

    public class Inventory
    {
        public string Id { get; set; }
        public string Room { get; set; }
        public List<InventoryItem> Items { get; set; }
    }

    public class InventoryItem
    {
        public string Value { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
