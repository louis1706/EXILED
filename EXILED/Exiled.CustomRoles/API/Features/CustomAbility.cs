// -----------------------------------------------------------------------
// <copyright file="CustomAbility.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.API.Interfaces;
    using YamlDotNet.Serialization;

    /// <summary>
    ///     The custom ability base class.
    /// </summary>
    public abstract class CustomAbility : IAbstractResolvable
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CustomAbility" /> class.
        /// </summary>
        public CustomAbility() => Init();

        /// <summary>
        ///     Gets a list of all registered custom abilities.
        /// </summary>
        public static HashSet<CustomAbility> Registered { get; } = new();

        /// <summary>
        ///     Gets all players who have this ability.
        /// </summary>
        [YamlIgnore]
        public HashSet<Player> Players { get; } = new();

        /// <summary>
        ///     Gets or sets the name of the ability.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        ///     Gets or sets the description of the ability.
        /// </summary>
        public abstract string Description { get; set; }

        /// <summary>
        ///     Gets a <see cref="CustomAbility" /> by name.
        /// </summary>
        /// <param name="name">The name of the ability to get.</param>
        /// <returns>The ability, or <see langword="null" /> if it doesn't exist.</returns>
        public static CustomAbility? Get(string name) => Registered?.FirstOrDefault(r => r.Name == name);

        /// <summary>
        ///     Gets a <see cref="CustomAbility" /> by type.
        /// </summary>
        /// <param name="type">The type of the ability to get.</param>
        /// <returns>The type, or <see langword="null" /> if it doesn't exist.</returns>
        public static CustomAbility? Get(Type type) => Registered?.FirstOrDefault(r => r.GetType() == type);

        /// <summary>
        ///     Tries to get a <see cref="CustomAbility" /> by type.
        /// </summary>
        /// <param name="type">The type of the ability to get.</param>
        /// <param name="customAbility">The custom ability.</param>
        /// <returns>True if the ability exists, otherwise false.</returns>
        public static bool TryGet(Type type, out CustomAbility? customAbility)
        {
            customAbility = Get(type);

            return customAbility is not null;
        }

        /// <summary>
        ///     Tries to get a <see cref="CustomAbility" /> by name.
        /// </summary>
        /// <param name="name">The name of the ability to get.</param>
        /// <param name="customAbility">The custom ability.</param>
        /// <returns>True if the ability exists.</returns>
        /// <exception cref="ArgumentNullException">If the name is <see langword="null" /> or an empty string.</exception>
        public static bool TryGet(string name, out CustomAbility? customAbility)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            customAbility = Get(name);

            return customAbility is not null;
        }

        /// <summary>
        ///     Checks to see if the specified player has this ability.
        /// </summary>
        /// <param name="player">The <see cref="Player" /> to check.</param>
        /// <returns>True if the player has this ability.</returns>
        public virtual bool Check(Player player) => player is not null && Players.Contains(player);

        /// <summary>
        ///     Adds this ability to the player.
        /// </summary>
        /// <param name="player">The <see cref="Player" /> to give the ability to.</param>
        public void AddAbility(Player player)
        {
            Log.Debug($"Added {Name} to {player.Nickname}");
            Players.Add(player);
            AbilityAdded(player);
        }

        /// <summary>
        ///     Removes this ability from the player.
        /// </summary>
        /// <param name="player">The <see cref="Player" /> to remove this ability from.</param>
        public void RemoveAbility(Player player)
        {
            Log.Debug($"Removed {Name} from {player.Nickname}");
            Players.Remove(player);
            AbilityRemoved(player);
        }

        /// <summary>
        ///     Initializes this ability.
        /// </summary>
        public void Init() => SubscribeEvents();

        /// <summary>
        ///     Destroys this ability.
        /// </summary>
        public void Destroy() => UnsubscribeEvents();

        /// <summary>
        ///     Loads the internal event handlers for the ability.
        /// </summary>
        protected virtual void SubscribeEvents()
        {
        }

        /// <summary>
        ///     Unloads the internal event handlers for the ability.
        /// </summary>
        protected virtual void UnsubscribeEvents()
        {
        }

        /// <summary>
        ///     Called when the ability is first added to the player.
        /// </summary>
        /// <param name="player">The <see cref="Player" /> using the ability.</param>
        protected virtual void AbilityAdded(Player player)
        {
        }

        /// <summary>
        ///     Called when the ability is being removed.
        /// </summary>
        /// <param name="player">The <see cref="Player" /> using the ability.</param>
        protected virtual void AbilityRemoved(Player player)
        {
        }
    }
}