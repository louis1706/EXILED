// -----------------------------------------------------------------------
// <copyright file="Give.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.Commands.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using API;
    using API.Features;
    using CommandSystem;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Permissions.Extensions;
    using PlayerRoles;
    using RemoteAdmin;

    /// <summary>
    ///     The command to give a role to player(s).
    /// </summary>
    internal sealed class Give : ICommand
    {
        /// <inheritdoc />
        public string Command { get; set; } = "give";

        /// <inheritdoc />
        public string[] Aliases { get; set; } = { "g" };

        /// <inheritdoc />
        public string Description { get; set; } = "Gives the specified custom role to the indicated player(s).";

        /// <summary>
        /// Gets or sets the message displayed when the user does not have the required permission.
        /// </summary>
        public string NoPermissionMessage { get; set; } = "Не хватает прав!";

        /// <summary>
        /// Gets or sets the message displayed when the specified custom role is not found.
        /// </summary>
        /// <remarks>
        /// The message contains a placeholder for the role name, which is replaced with the actual role name when the message is displayed.
        /// </remarks>
        public string NoRoleFoundMessage { get; set; } = "Кастомная роль {0} не найдена!";

        /// <summary>
        /// Gets or sets the message displayed when the specified player is not found.
        /// </summary>
        public string PlayerNotFoundMessage { get; set; } = "Игрок не найден.";

        /// <summary>
        /// Gets or sets the message displayed when a custom role is successfully given to a player.
        /// </summary>
        /// <remarks>
        /// The message contains placeholders for the role name and the player's nickname, which are replaced with the actual values when the message is displayed.
        /// </remarks>
        public string RoleGivenMessage { get; set; } = "Кастомная роль {0} дана игроку {1}.";

        /// <summary>
        /// Gets or sets the message displayed when a custom role is successfully given to all players.
        /// </summary>
        /// <remarks>
        /// The message contains a placeholder for the role name, which is replaced with the actual role name when the message is displayed.
        /// </remarks>
        public string AllPlayersRoleGivenMessage { get; set; } = "Кастомная роль {0} дана всем игрокам.";

        /// <summary>
        /// Gets or sets the error message displayed when a player is not found.
        /// </summary>
        /// <remarks>
        /// The message contains a placeholder for the player's nickname or ID, which is replaced with the actual value when the message is displayed.
        /// </remarks>
        public string PlayerNotFoundErrorMessage { get; set; } = "Игрок {0} не найден";

        /// <summary>
        /// Gets or sets the usage message displayed when the command is used incorrectly.
        /// </summary>
        /// <remarks>
        /// The message contains a placeholder for the command name, which is replaced with the actual command name when the message is displayed.
        /// </remarks>
        public string UsageMessage { get; set; } = "{0} <Название/ID кастомной роли> [Никнейм/ID/SteamID игрока или all/*]";

        /// <inheritdoc />
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("customroles.give"))
            {
                response = NoPermissionMessage;
                return false;
            }

            if (arguments.Count == 0)
            {
                response = string.Format(UsageMessage, Command);
                return false;
            }

            if (!CustomRole.TryGet(arguments.At(0), out CustomRole? role) || role is null)
            {
                response = string.Format(NoRoleFoundMessage, arguments.At(0));
                return false;
            }

            if (arguments.Count == 1)
            {
                if (sender is PlayerCommandSender playerCommandSender)
                {
                    Player player = Player.Get(playerCommandSender);

                    TryAddRole(player, role);
                    response = string.Format(RoleGivenMessage, role.Name, player.Nickname);
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
                    List<Player> players = ListPool<Player>.Pool.Get(Player.List);

                    foreach (Player player in players)
                        TryAddRole(player, role);

                    response = string.Format(AllPlayersRoleGivenMessage, role.Name);
                    ListPool<Player>.Pool.Return(players);
                    return true;
                default:
                    if (Player.Get(identifier) is not { } ply)
                    {
                        response = string.Format(PlayerNotFoundErrorMessage, identifier);
                        return false;
                    }

                    TryAddRole(ply, role);
                    response = string.Format(RoleGivenMessage, role.Name, ply.Nickname);
                    return true;
            }
        }

        private void TryAddRole(Player player, CustomRole customRole)
        {
            player.GetCustomRole()?.RemoveRole(player);

            customRole.AddRole(player, SpawnReason.ForceClass, RoleSpawnFlags.All);
        }
    }
}