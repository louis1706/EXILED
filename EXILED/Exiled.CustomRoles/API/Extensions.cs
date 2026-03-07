// -----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.API
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Exiled.API.Features;

    using Features;

    /// <summary>
    ///     A collection of API methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets a <see cref="IReadOnlyDictionary{TKey, TValue}"/> of all players that have custom role.
        /// </summary>
        public static IReadOnlyDictionary<Player, CustomRole> PlayerToCustomRoles => InternalPlayerToCustomRoles;

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> of all players that have custom role.
        /// </summary>
        internal static Dictionary<Player, CustomRole> InternalPlayerToCustomRoles { get; } = new();

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing all temp cache-roles for setting position.
        /// </summary>
        internal static Dictionary<Player, CustomRole> ToChangeRolePlayers { get; } = new();

        /// <summary>
        /// Gets a <see cref="HashSet{TKey}"/> containing all players that should receive customrole inventory.
        /// </summary>
        internal static HashSet<Player> AssignInventoryPlayers { get; } = new();

        /// <summary>
        ///     Gets a <see cref="ReadOnlyCollection{T}" /> of the player's current custom roles.
        /// </summary>
        /// <param name="player">The <see cref="Player" /> to check for roles.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}" /> of all current custom roles.</returns>
        [Obsolete("Для ФЛХ используйте GetCustomRole!!!", true)]
        public static ReadOnlyCollection<CustomRole> GetCustomRoles(this Player player)
        {
            List<CustomRole> roles = new();

            if (TryGetCustomRole(player, out CustomRole role))
            {
                roles.Add(role);
            }

            return roles.AsReadOnly();
        }

        /// <summary>
        /// Checks whether the player has any custom role assigned.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns><c>true</c> if the player has at least one custom role; otherwise, <c>false</c>.</returns>
        public static bool HasAnyCustomRole(this Player player)
        {
            if (player == null)
                return false;

            foreach (CustomRole role in CustomRole.Registered)
            {
                if (role.Check(player))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Registers an <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/>s.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check for role.</param>
        /// <returns>A target <see cref="CustomRole"/> (can be null).</returns>
        public static CustomRole? GetCustomRole(this Player player)
        {
            if (player != null && PlayerToCustomRoles.TryGetValue(player, out CustomRole? role))
                return role;
            return null;
        }

        /// <summary>
        /// Gets a <see cref="CustomRole"/> of the player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check for role.</param>
        /// <param name="customRole">A target <see cref="CustomRole"/>.</param>
        /// <returns>A boolean indicating whether or not a custom role was found.</returns>
        public static bool TryGetCustomRole(this Player player, out CustomRole customRole)
        {
            return (customRole = GetCustomRole(player) !) is not null;
        }

        /// <summary>
        /// Gets a value indicating whether or not player has any <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check for role.</param>
        /// <returns>A boolean indicating whether or not player has <see cref="CustomRole"/>.</returns>
        public static bool HasCustomRole(this Player player)
        {
            return player.GetCustomRole() is not null;
        }

        /// <summary>
        /// Gets a specific <see cref="CustomRole"/> by type of the player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check for role.</param>
        /// <typeparam name="T">The specified <see cref="CustomRole"/> type.</typeparam>
        /// <returns>A target <see cref="CustomRole"/> (can be null).</returns>
        public static T? GetCustomRole<T>(this Player player)
            where T : CustomRole
        {
            if (player != null && PlayerToCustomRoles.TryGetValue(player, out CustomRole? role) && role is T typedRole)
                return typedRole;

            return null;
        }

        /// <summary>
        /// Gets a specific <see cref="CustomRole"/> by type of the player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check for role.</param>
        /// <param name="customRole">A target <see cref="CustomRole"/>.</param>
        /// <typeparam name="T">The specified <see cref="CustomRole"/> type.</typeparam>
        /// <returns>A boolean indicating whether or not a custom role was found.</returns>
        public static bool TryGetCustomRole<T>(this Player player, out T customRole)
            where T : CustomRole
        {
            return (customRole = GetCustomRole<T>(player) !) is not null;
        }

        /// <summary>
        /// Gets a value indicating whether or not player has a specific <see cref="CustomRole"/> by type.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check for role.</param>
        /// <typeparam name="T">The specified <see cref="CustomRole"/> type.</typeparam>
        /// <returns>A boolean indicating whether or not player has specific <see cref="CustomRole"/>.</returns>
        public static bool HasCustomRole<T>(this Player player)
            where T : CustomRole
        {
            return player.GetCustomRole<T>() is not null;
        }

        /// <summary>
        ///     Registers an <see cref="IEnumerable{T}" /> of <see cref="CustomRole" />s.
        /// </summary>
        /// <param name="customRoles"><see cref="CustomRole" />s to be registered.</param>
        public static void Register(this IEnumerable<CustomRole> customRoles)
        {
            if (customRoles is null)
                throw new ArgumentNullException(nameof(customRoles));

            foreach (CustomRole customRole in customRoles)
                customRole.TryRegister();
        }

        /// <summary>
        ///     Registers a <see cref="CustomRole" />.
        /// </summary>
        /// <param name="role"><see cref="CustomRole" /> to be registered.</param>
        public static void Register(this CustomRole role) => role.TryRegister();

        /// <summary>
        ///     Unregisters an <see cref="IEnumerable{T}" /> of <see cref="CustomRole" />s.
        /// </summary>
        /// <param name="customRoles"><see cref="CustomRole" />s to be unregistered.</param>
        public static void Unregister(this IEnumerable<CustomRole> customRoles)
        {
            if (customRoles is null)
                throw new ArgumentNullException(nameof(customRoles));

            foreach (CustomRole customRole in customRoles)
                customRole.TryUnregister();
        }

        /// <summary>
        ///     Unregisters a <see cref="CustomRole" />.
        /// </summary>
        /// <param name="role"><see cref="CustomRole" /> to be unregistered.</param>
        public static void Unregister(this CustomRole role) => role.TryUnregister();
    }
}
