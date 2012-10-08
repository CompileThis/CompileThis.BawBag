namespace CompileThis.BawBag.Extensibility.Internal
{
    using System.Collections.Generic;
    using System.Linq;

    using Raven.Client;

    using CompileThis.BawBag.Jabbr;

    internal class InventoryManager : IInventoryManager
    {
        private readonly int _inventoryCapacity;
        private readonly IDocumentStore _documentStore;
        private readonly IRandomNumberProvider _randomNumberProvider;

        public InventoryManager(int inventoryCapacity, IDocumentStore documentStore, IRandomNumberProvider randomNumberProvider)
        {
            Guard.NullParameter(documentStore, () => documentStore);
            Guard.NullParameter(randomNumberProvider, () => randomNumberProvider);

            _inventoryCapacity = inventoryCapacity;
            _documentStore = documentStore;
            _randomNumberProvider = randomNumberProvider;
        }

        public bool AddItem(Room room, InventoryItem item, out InventoryItem droppedItem)
        {
            Guard.NullParameter(room, () => room);
            Guard.NullParameter(item, () => item);

            return AddItem(room.Name, item, out droppedItem);
        }

        public bool AddItem(string roomName, InventoryItem item, out InventoryItem droppedItem)
        {
            Guard.NullParameter(roomName, () => roomName);
            Guard.NullParameter(item, () => item);

            using (var session = _documentStore.OpenSession())
            {
                var inventory = session.Query<Inventory>().Customize(x => x.WaitForNonStaleResults()).SingleOrDefault(x => x.Room == roomName);
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

                    return true;
                }

                if (inventory.Items.Count >= _inventoryCapacity)
                {
                    var itemIndex = _randomNumberProvider.Next(_inventoryCapacity);
                    droppedItem = inventory.Items[itemIndex];
                    inventory.Items.RemoveAt(itemIndex);
                }

                inventory.Items.Add(item);
                session.SaveChanges();

                return false;
            }
        }

        public IEnumerable<InventoryItem> GetInventory(Room room)
        {
            Guard.NullParameter(room, () => room);

            return GetInventory(room.Name);
        }

        public IEnumerable<InventoryItem> GetInventory(string roomName)
        {
            Guard.NullParameter(roomName, () => roomName);

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
            Guard.NullParameter(room, () => room);

            return RemoveRandomItem(room.Name);
        }

        public InventoryItem RemoveRandomItem(string roomName)
        {
            Guard.NullParameter(roomName, () => roomName);

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
}
