// -----------------------------------------------------------------------
// <copyright file="ItemAddedEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System.Diagnostics;

    using API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Interfaces;

    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;

    /// <summary>
    /// Contains all information after adding an item to a player's inventory.
    /// </summary>
    public class ItemAddedEventArgs : IItemEvent, IPickupEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemAddedEventArgs"/> class.
        /// </summary>
        /// <param name="referenceHub">The <see cref="ReferenceHub"/> the item was added to.</param>
        /// <param name="itemBase">The added <see cref="ItemBase"/>.</param>
        /// <param name="pickupBase">The <see cref="ItemPickupBase"/> the <see cref="ItemBase"/> originated from, or <see langword="null"/> if the item was not picked up.</param>
        public ItemAddedEventArgs(ReferenceHub referenceHub, ItemBase itemBase, ItemPickupBase pickupBase)
        {
            Player = Player.Get(referenceHub);
            Item = Item.Get(itemBase);
            Pickup = Pickup.Get(pickupBase);
            Log.Assert(Item != null, $"ItemAddedEventArgs ctor: Item is null! Base: '{itemBase}'");
            Log.Assert(Player != null, $"ItemAddedEventArgs ctor: Player is null! Base: '{referenceHub} {new StackTrace()}'");
        }

        /// <summary>
        /// Gets the player that had the item added.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the item that was added.
        /// </summary>
        public Item Item { get; }

        /// <summary>
        /// Gets the pickup that the item originated from or <see langword="null"/> if the item was not picked up.
        /// </summary>
        public Pickup Pickup { get; }
    }
}