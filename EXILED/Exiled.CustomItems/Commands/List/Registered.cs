// -----------------------------------------------------------------------
// <copyright file="Registered.cs" company="ExMod Team">
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

    using Exiled.API.Features.Pools;
    using Exiled.CustomItems.API.Features;
    using Exiled.Permissions.Extensions;

    /// <inheritdoc/>
    internal sealed class Registered : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "registered";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "r", "reg" };

        /// <inheritdoc/>
        public string Description { get; set; } = "Получает все зарегистрированные кастомные предметы.";

        /// <summary>
        /// Gets or sets the message displayed when a user does not have the required permission.
        /// </summary>
        public string NoPermissionMessage { get; set; } = "Не хватает прав!";

        /// <summary>
        /// Gets or sets the message displayed when there are no custom items on the server.
        /// </summary>
        public string NoCustomItemsMessage { get; set; } = "На сервере нет кастомных предметов.";

        /// <summary>
        /// Gets or sets the header format for displaying custom items, where {0} is the number of custom items.
        /// </summary>
        public string CustomItemsHeader { get; set; } = "[Кастомные предметы ({0})]";

        /// <summary>
        /// Gets or sets the format for displaying a single custom item, where {0} is the item ID, {1} is the item name, and {2} is the item type.
        /// </summary>
        public string CustomItemFormat { get; set; } = "[{0}. {1} ({2})]";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("customitems.list.registered"))
            {
                response = NoPermissionMessage;
                return false;
            }

            if (CustomItem.Registered.Count == 0)
            {
                response = NoCustomItemsMessage;
                return false;
            }

            StringBuilder message = StringBuilderPool.Pool.Get().AppendLine();

            message.Append(string.Format(CustomItemsHeader, CustomItem.Registered.Count));

            foreach (CustomItem customItem in CustomItem.Registered.OrderBy(item => item.Id))
                message.Append(string.Format(CustomItemFormat, customItem.Id, customItem.Name, customItem.Type)).AppendLine();

            response = StringBuilderPool.Pool.ToStringReturn(message);
            return true;
        }
    }
}