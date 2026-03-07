// -----------------------------------------------------------------------
// <copyright file="Info.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.Commands
{
    using System;
    using System.Text;

    using CommandSystem;
    using Exiled.API.Features.Pools;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API.Features;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// The command to view info about a specific item.
    /// </summary>
    internal sealed class Info : ICommand
    {
        /// <summary>
        /// Gets or sets the permission required to execute the info command.
        /// </summary>
        public string Permission { get; set; } = "customitems.info";

        /// <summary>
        /// Gets or sets the message displayed when the user does not have the required permission.
        /// </summary>
        public string NoPermissionMessage { get; set; } = "Не хватает прав!";

        /// <summary>
        /// Gets or sets the usage message displayed when the user provides invalid arguments.
        /// </summary>
        public string UsageMessage { get; set; } = "info [Название/ID кастомного предмета]";

        /// <summary>
        /// Gets or sets the message displayed when the specified item is not found.
        /// </summary>
        /// <remarks>The {0} placeholder will be replaced with the item name or ID.</remarks>
        public string NotFoundMessage { get; set; } = "{0} не найден.";

        /// <summary>
        /// Gets or sets the first color used in the item information display.
        /// </summary>
        public string Color1 { get; set; } = "#E6AC00";

        /// <summary>
        /// Gets or sets the second color used in the item information display.
        /// </summary>
        public string Color2 { get; set; } = "#00D639";

        /// <summary>
        /// Gets or sets the third color used in the item information display.
        /// </summary>
        public string Color3 { get; set; } = "#05C4EB";

        /// <summary>
        /// Gets or sets the label used to display the spawn limit.
        /// </summary>
        public string SpawnLimitLabel { get; set; } = "- Лимит спавна: ";

        /// <summary>
        /// Gets or sets the label used to display the spawn points.
        /// </summary>
        /// <remarks>The {0} placeholder will be replaced with the number of spawn points.</remarks>
        public string SpawnPointsLabel { get; set; } = "[Локации ({0})]";

        /// <summary>
        /// Gets or sets the format used to display individual spawn points.
        /// </summary>
        /// <remarks>The {0} placeholder will be replaced with the spawn point name, {1} with the position, and {2} with the chance.</remarks>
        public string SpawnPointFormat { get; set; } = "{0} {1} Шанс: {2}%";

        /// <inheritdoc/>
        public string Command { get; set; } = "info";

        /// <inheritdoc/>
        public string[] Aliases { get; set; } = { "i" };

        /// <inheritdoc/>
        public string Description { get; set; } = "Дает информацию о кастомном предмете.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(Permission))
            {
                response = NoPermissionMessage;
                return false;
            }

            if (arguments.Count < 1)
            {
                response = UsageMessage;
                return false;
            }

            if (!(uint.TryParse(arguments.At(0), out uint id) && CustomItem.TryGet(id, out CustomItem? item)) &&
                !CustomItem.TryGet(arguments.At(0), out item))
            {
                response = string.Format(NotFoundMessage, arguments.At(0));
                return false;
            }

            StringBuilder message = StringBuilderPool.Pool.Get().AppendLine();

            message.Append("<color=").Append(Color1).Append(">-</color> <color=").Append(Color2).Append(">").Append(item?.Name).Append("</color> <color=").Append(Color3).Append(">(").Append(item?.Id).AppendLine(")</color>")
                .Append("- ").AppendLine(item?.Description)
                .AppendLine(item?.Type.ToString())
                .Append(SpawnLimitLabel).AppendLine(item?.SpawnProperties?.Limit.ToString()).AppendLine()
                .Append(string.Format(SpawnPointsLabel, item?.SpawnProperties?.DynamicSpawnPoints.Count + item?.SpawnProperties?.StaticSpawnPoints.Count));

            foreach (DynamicSpawnPoint spawnPoint in item?.SpawnProperties?.DynamicSpawnPoints!)
                message.Append(string.Format(SpawnPointFormat, spawnPoint.Name, spawnPoint.Position, spawnPoint.Chance)).AppendLine("%");

            foreach (StaticSpawnPoint spawnPoint in item.SpawnProperties.StaticSpawnPoints)
                message.Append(string.Format(SpawnPointFormat, spawnPoint.Name, spawnPoint.Position, spawnPoint.Chance)).AppendLine("%");

            response = StringBuilderPool.Pool.ToStringReturn(message);
            return true;
        }
    }
}