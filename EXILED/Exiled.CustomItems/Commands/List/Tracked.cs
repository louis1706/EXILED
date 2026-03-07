// -----------------------------------------------------------------------
// <copyright file="Tracked.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.Commands.List
{
    using System;
    using System.Linq;
    using System.Text;

    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.CustomItems.API.Features;
    using Exiled.Permissions.Extensions;
    using RemoteAdmin;

    /// <inheritdoc/>
    internal sealed class Tracked : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; set; } = "insideinventories";

        /// <inheritdoc/>
        public string[] Aliases { get; set; } = { "ii", "inside", "inv", "inventories" };

        /// <inheritdoc/>
        public string Description { get; set; } = "Получает все предметы которые лежат в инвенторях игроков.";

        /// <summary>
        /// Gets or sets the message displayed when a user does not have the required permission.
        /// </summary>
        public string NoPermissionMessage { get; set; } = "Не хватает прав!";

        /// <summary>
        /// Gets or sets the message displayed when no custom items are found.
        /// </summary>
        public string NoCustomItemsMessage { get; set; } = "Кастомные предметы не найдены.";

        /// <summary>
        /// Gets or sets the format for the title of the custom items list, where {0} is the count of items.
        /// </summary>
        public string TitleFormat { get; set; } = "[Custom items inside inventories ({0})]";

        /// <summary>
        /// Gets or sets the format for a single custom item in the list, where:
        /// {0} is the item ID,
        /// {1} is the item name,
        /// {2} is the item type,
        /// {3} is the count of items.
        /// </summary>
        public string ItemFormat { get; set; } = "[{0}. {1} ({2}) {{ {3} }}]";

        /// <summary>
        /// Gets or sets the format for a serial number in the list, where {0} is the serial number.
        /// </summary>
        public string SerialFormat { get; set; } = "{0}. ";

        /// <summary>
        /// Gets or sets the message displayed when an item has no owner.
        /// </summary>
        public string NoOwnerMessage { get; set; } = "Никто";

        /// <summary>
        /// Gets or sets the format for an item owner, where:
        /// {0} is the owner's nickname,
        /// {1} is the owner's user ID,
        /// {2} is the owner's ID,
        /// {3} is the owner's role.
        /// </summary>
        public string OwnerFormat { get; set; } = "{0} ({1}) ({2}) [{3}]";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("customitems.list.insideinventories") && sender is PlayerCommandSender playerSender && !playerSender.FullPermissions)
            {
                response = NoPermissionMessage;
                return false;
            }

            StringBuilder message = StringBuilderPool.Pool.Get();

            int count = 0;

            foreach (CustomItem customItem in CustomItem.Registered)
            {
                if (customItem.TrackedSerials.Count == 0)
                    continue;

                message.AppendLine()
                    .AppendFormat(ItemFormat, customItem.Id, customItem.Name, customItem.Type, customItem.TrackedSerials.Count)
                    .AppendLine();

                count += customItem.TrackedSerials.Count;

                foreach (int insideInventory in customItem.TrackedSerials)
                {
                    Player owner = Player.List.FirstOrDefault(player => player.Inventory.UserInventory.Items.Any(item => item.Key == insideInventory));

                    message.AppendFormat(SerialFormat, insideInventory);

                    if (owner is null)
                        message.AppendLine(NoOwnerMessage);
                    else
                        message.AppendFormat(OwnerFormat, owner.Nickname, owner.UserId, owner.Id, owner.Role).AppendLine();
                }
            }

            if (message.Length == 0)
                message.Append(NoCustomItemsMessage);
            else
                message.Insert(0, Environment.NewLine + string.Format(TitleFormat, count) + Environment.NewLine);

            response = StringBuilderPool.Pool.ToStringReturn(message);
            return true;
        }
    }
}