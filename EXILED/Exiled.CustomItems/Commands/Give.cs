// -----------------------------------------------------------------------
// <copyright file="Give.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.Permissions.Extensions;
    using RemoteAdmin;

    /// <summary>
    /// The command to give a player an item.
    /// </summary>
    internal sealed class Give : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "give";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "g" };

        /// <inheritdoc/>
        public string Description { get; set; } = "Дает кастомный предмет.";

        /// <summary>
        /// Gets or sets message displayed when the user does not have the required permission.
        /// </summary>
        public string PermissionRequiredMessage { get; set; } = "Не хватает прав!";

        /// <summary>
        /// Gets or sets message displayed when the user uses the command incorrectly.
        /// </summary>
        public string UsageMessage { get; set; } = "give <Название/ID кастомного предмета> [Никнейм/ID/SteamID игрока или */all для выдачи всем]";

        /// <summary>
        /// Gets or sets message displayed when the specified item is not found.
        /// </summary>
        /// <remarks>
        /// The {0} placeholder will be replaced with the item name.
        /// </remarks>
        public string ItemNotFoundMessage { get; set; } = "Кастомный предмет {0} не найден!";

        /// <summary>
        /// Gets or sets message displayed when the specified player is not found.
        /// </summary>
        public string PlayerNotFoundMessage { get; set; } = "Игрок не найден.";

        /// <summary>
        /// Gets or sets message displayed when the player is not eligible to receive the item.
        /// </summary>
        public string NotEligibleMessage { get; set; } = "Вы не можете получить кастомный предмет!";

        /// <summary>
        /// Gets or sets message displayed when the item is successfully given to a player.
        /// </summary>
        /// <remarks>
        /// The {0} placeholder will be replaced with the item name, {1} with the player's nickname, and {2} with the player's ID.
        /// </remarks>
        public string ItemGivenMessage { get; set; } = "{0} дан игроку {1} ({2})";

        /// <summary>
        /// Gets or sets message displayed when the item is successfully given to all players.
        /// </summary>
        /// <remarks>
        /// The {0} placeholder will be replaced with the item name, and {1} with the number of players.
        /// </remarks>
        public string ItemGivenToAllMessage { get; set; } = "Кастомный предмет {0} дан ({1} игрокам)";

        /// <summary>
        /// Gets or sets message displayed when a player is not eligible to receive the item.
        /// </summary>
        public string PlayerNotEligibleMessage { get; set; } = "Игрок не можете получить кастомный предмет!";

        /// <summary>
        /// Gets or sets message displayed when a player cannot be found by their identifier.
        /// </summary>
        /// <remarks>
        /// The {0} placeholder will be replaced with the identifier.
        /// </remarks>
        public string PlayerNotFoundByIdentifierMessage { get; set; } = "Невозможно найти игрока: {0}.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("customitems.give"))
            {
                response = PermissionRequiredMessage;
                return false;
            }

            if (arguments.Count == 0)
            {
                response = UsageMessage;
                return false;
            }

            if (!CustomItem.TryGet(arguments.At(0), out CustomItem? item))
            {
                response = string.Format(ItemNotFoundMessage, arguments.At(0));
                return false;
            }

            if (arguments.Count == 1)
            {
                if (sender is PlayerCommandSender playerCommandSender)
                {
                    Player player = Player.Get(playerCommandSender.SenderId);

                    if (!CheckEligible(player))
                    {
                        response = NotEligibleMessage;
                        return false;
                    }

                    item?.Give(player);
                    response = string.Format(ItemGivenMessage, item?.Name, player.Nickname, player.UserId);
                    return true;
                }

                response = PlayerNotFoundMessage;
                return false;
            }

            string identifier = string.Join(" ", arguments.Skip(1));

            switch (identifier)
            {
                case "*":
                case "all":
                    List<Player> eligiblePlayers = Player.List.Where(CheckEligible).ToList();
                    foreach (Player ply in eligiblePlayers)
                        item?.Give(ply);

                    response = string.Format(ItemGivenToAllMessage, item?.Name, eligiblePlayers.Count);
                    return true;
                default:
                    if (Player.Get(identifier) is not { } player)
                    {
                        response = string.Format(PlayerNotFoundByIdentifierMessage, identifier);
                        return false;
                    }

                    if (!CheckEligible(player))
                    {
                        response = PlayerNotEligibleMessage;
                        return false;
                    }

                    item?.Give(player);
                    response = string.Format(ItemGivenMessage, item?.Name, player.Nickname, player.UserId);
                    return true;
            }
        }

        /// <summary>
        /// Checks if the player is eligible to receive custom items.
        /// </summary>
        private bool CheckEligible(Player player) => player.IsAlive && !player.IsCuffed && (player.Items.Count < 8);
    }
}