namespace CompileThis.BawBag.Extensibility.Internal
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents an inventory for a single chat room.
    /// </summary>
    internal class Inventory
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the room.
        /// </summary>
        /// <value>
        /// The room.
        /// </value>
        public string Room { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public List<InventoryItem> Items { get; set; }
    }
}