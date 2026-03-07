// -----------------------------------------------------------------------
// <copyright file="Spawn.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.Commands
{
    using System;

    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.Permissions.Extensions;
    using UnityEngine;

    /// <summary>
    /// The command to spawn a specific item.
    /// </summary>
    internal sealed class Spawn : ICommand
    {
        /// <summary>
        /// Gets or sets message displayed when the user has insufficient permissions to execute the command.
        /// </summary>
        public string InsufficientPermissionsMessage { get; set; } = "Не хватает прав!";

        /// <summary>
        /// Gets or sets message displayed when the user provides invalid arguments for the command.
        /// </summary>
        public string InvalidArgumentsMessage { get; set; } = "spawn [Название/ID кастомного предмета] [Никнейм/SteamID игока]\nspawn [Название/ID кастомного предмета] [X] [Y] [Z]";

        /// <summary>
        /// Gets or sets message displayed when the user tries to spawn an invalid custom item.
        /// </summary>
        public string InvalidCustomItemMessage { get; set; } = " {0} is not a valid custom item.";

        /// <summary>
        /// Gets or sets message displayed when the target player is dead.
        /// </summary>
        public string PlayerIsDeadMessage { get; set; } = "Игрок мертв!";

        /// <summary>
        /// Gets or sets message displayed when the user provides invalid coordinates for the spawn location.
        /// </summary>
        public string InvalidCoordinatesMessage { get; set; } = "Невозможно получить координату (попробуй писать через , а не .)";

        /// <summary>
        /// Gets or sets message displayed when the system is unable to find a valid spawn location.
        /// </summary>
        public string UnableToFindLocationMessage { get; set; } = "Невозможно найти локацию для спавна.";

        /// <summary>
        /// Gets or sets message displayed when the spawn is successful.
        /// </summary>
        public string SpawnSuccessMessage { get; set; } = "{0} ({1}) заспавнился на позиции {2}.";

        /// <inheritdoc/>
        public string Command { get; set; } = "spawn";

        /// <inheritdoc/>
        public string[] Aliases { get; set; } = { "sp" };

        /// <inheritdoc/>
        public string Description { get; set; } = "Спавнит кастомный предмет.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("customitems.spawn"))
            {
                response = InsufficientPermissionsMessage;
                return false;
            }

            if (arguments.Count < 2)
            {
                response = InvalidArgumentsMessage;
                return false;
            }

            if (!CustomItem.TryGet(arguments.At(0), out CustomItem? item))
            {
                response = string.Format(InvalidCustomItemMessage, arguments.At(0));
                return false;
            }

            Vector3 position;

            if (Player.Get(arguments.At(1)) is Player player)
            {
                if (player.IsDead)
                {
                    response = PlayerIsDeadMessage;
                    return false;
                }

                position = player.Position;
            }
            else if (arguments.Count > 3)
            {
                if (!float.TryParse(arguments.At(1), out float x) || !float.TryParse(arguments.At(2), out float y) || !float.TryParse(arguments.At(3), out float z))
                {
                    response = InvalidCoordinatesMessage;
                    return false;
                }

                position = new Vector3(x, y, z);
            }
            else
            {
                response = UnableToFindLocationMessage;
                return false;
            }

            item?.Spawn(position);

            response = string.Format(SpawnSuccessMessage, item?.Name, item?.Type, position);
            return true;
        }
    }
}
