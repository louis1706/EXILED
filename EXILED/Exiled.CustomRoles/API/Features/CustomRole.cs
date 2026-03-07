// -----------------------------------------------------------------------
// <copyright file="CustomRole.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    using CustomItems.API.Features;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Pools;
    using Exiled.API.Features.Roles;
    using Exiled.API.Interfaces;
    using Exiled.Events.EventArgs.Player;
    using FLXLib.Spawns;
    using MEC;
    using Mirror;
    using PlayerRoles;
    using UnityEngine;
    using YamlDotNet.Serialization;

    using Extensions = Extensions;

    /// <summary>
    ///     The custom role base class.
    /// </summary>
    public abstract class CustomRole
    {
        /// <summary>
        ///     This var makes player skip base role replace by <see cref="ReplacesBaseRole" /> for one rolechange.
        /// </summary>
        public const string SkipBaseRoleReplaceKey = "skipRoleReplace";

        private static readonly Dictionary<uint, CustomRole?> IdLookupTable = new();

        /// <summary>
        ///     Gets a list of all registered custom roles.
        /// </summary>
        public static HashSet<CustomRole> Registered { get; } = new();

        /// <summary>
        ///     Gets or sets the custom RoleID of the role.
        /// </summary>
        public abstract uint Id { get; set; }

        /// <summary>
        ///     Gets or sets the max <see cref="Player.Health" /> for the role.
        /// </summary>
        public abstract int MaxHealth { get; set; }

        /// <summary>
        ///     Gets or sets the name of this role.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        ///     Gets or sets the description of this role.
        /// </summary>
        public abstract string Description { get; set; }

        /// <summary>
        ///     Gets or sets the CustomInfo of this role.
        /// </summary>
        public abstract string CustomInfo { get; set; }

        /// <summary>
        ///     Gets or sets the SpectatorText of this role.
        /// </summary>
        public abstract string SpectatorText { get; set; }

        /// <summary>
        ///     Gets all of the players currently set to this role.
        /// </summary>
        [YamlIgnore]
        public HashSet<Player> TrackedPlayers { get; } = new();

        /// <summary>
        ///     Gets or sets the <see cref="RoleTypeId" /> to spawn this role as.
        /// </summary>
        public virtual RoleTypeId Role { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the <see cref="RoleTypeId" /> role is always replaced by this CustomRole.
        /// </summary>
        public virtual bool ReplacesBaseRole { get; set; } = false;

        /// <summary>
        ///     Gets or sets a list of the roles custom abilities.
        /// </summary>
        public virtual List<CustomAbility>? CustomAbilities { get; set; } = new();

        /// <summary>
        ///     Gets or sets List {Dictionary {Item, Chance}}. Supports CustomItems by IDs. Chance can't be decimal.
        /// </summary>
        [Description("List {Dictionary {Item, Chance}}. Supports CustomItems by IDs. Chance can't be decimal. Max slots: 8")]
        public virtual List<Dictionary<string, short>> Inventory { get; set; } =
            new();

        /// <summary>
        ///     Gets or sets the starting ammo for the role.
        /// </summary>
        public virtual Dictionary<AmmoType, ushort> Ammo { get; set; } = new();

        /// <summary>
        ///     Gets or sets the possible spawn locations for this role.
        /// </summary>
        public virtual SpawnPositionSettings SpawnProperties { get; set; } = new();

        /// <summary>
        ///     Gets or sets a value indicating whether players keep this role when they die.
        /// </summary>
        public virtual bool KeepRoleOnDeath { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether players will receive a message for getting a custom item, when gaining it
        ///     through the inventory config for this role.
        /// </summary>
        public virtual bool DisplayCustomItemMessages { get; set; } = true;

        /// <summary>
        ///     Gets or sets a value indicating the <see cref="Player" />'s size.
        /// </summary>
        public virtual Vector3 Scale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets a value indicating the <see cref="Player"/>'s gravity.
        /// </summary>
        public virtual Vector3? Gravity { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> containing cached <see cref="string"/> and their  <see cref="Dictionary{TKey, TValue}"/> which is cached Role with FF multiplier.
        ///     Gets or sets a <see cref="Dictionary{TKey, TValue}" /> containing cached <see cref="string" /> and their
        ///     <see cref="Dictionary{TKey, TValue}" /> which is cached Role with FF multiplier.
        /// </summary>
        public virtual Dictionary<RoleTypeId, float> CustomRoleFFMultiplier { get; set; } = new();

        /// <summary>
        ///     Gets a <see cref="CustomRole" /> by ID.
        /// </summary>
        /// <param name="id">The ID of the role to get.</param>
        /// <returns>The role, or <see langword="null" /> if it doesn't exist.</returns>
        public static CustomRole? Get(uint id)
        {
            if (!IdLookupTable.ContainsKey(id))
                IdLookupTable.Add(id, Registered.FirstOrDefault(r => r.Id == id));
            return IdLookupTable[id];
        }

        /// <summary>
        /// Gets a <see cref="CustomRole" /> by type.
        /// </summary>
        /// <param name="t">The <see cref="Type" /> to get.</param>
        /// <returns>The role, or <see langword="null" /> if it doesn't exist.</returns>
        public static CustomRole? Get(Type t)
        {
            return Registered.FirstOrDefault(r => r.GetType() == t);
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/> by type.
        /// </summary>
        /// <param name="t">The <see cref="Type" /> to get.</param>
        /// <returns>The <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/>.</returns>
        public static IEnumerable<CustomRole> GetMany(Type t)
        {
            return Registered.Where(r => r.GetType() == t);
        }

        /// <summary>
        ///     Gets a <see cref="CustomRole" /> by name.
        /// </summary>
        /// <param name="name">The name of the role to get.</param>
        /// <returns>The role, or <see langword="null" /> if it doesn't exist.</returns>
        public static CustomRole? Get(string name)
        {
            return Registered.FirstOrDefault(r => r.Name == name);
        }

        /// <summary>
        /// Gets a <see cref="CustomRole"/> by type.
        /// </summary>
        /// <typeparam name="T">The specified <see cref="CustomRole"/> type.</typeparam>
        /// <returns>The role, or <see langword="null"/> if it doesn't exist.</returns>
        public static T? Get<T>()
            where T : CustomRole
        {
            return Registered.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/> by type.
        /// </summary>
        /// <typeparam name="T">The specified <see cref="CustomRole"/> type.</typeparam>
        /// <returns>The <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/>.</returns>
        public static IEnumerable<T> GetMany<T>()
            where T : CustomRole
        {
            return Registered.OfType<T>();
        }

        /// <summary>
        ///     Tries to get a <see cref="CustomRole" /> by <inheritdoc cref="Id" />.
        /// </summary>
        /// <param name="id">The ID of the role to get.</param>
        /// <param name="customRole">The custom role.</param>
        /// <returns>True if the role exists.</returns>
        public static bool TryGet(uint id, out CustomRole? customRole)
        {
            customRole = Get(id);

            return customRole is not null;
        }

        /// <summary>
        ///     Tries to get a <see cref="CustomRole" /> by name.
        /// </summary>
        /// <param name="name">The name of the role to get.</param>
        /// <param name="customRole">The custom role.</param>
        /// <returns>True if the role exists.</returns>
        /// <exception cref="ArgumentNullException">If the name is <see langword="null" /> or an empty string.</exception>
        public static bool TryGet(string name, out CustomRole? customRole)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            customRole = uint.TryParse(name, out uint id) ? Get(id) : Get(name);

            return customRole is not null;
        }

        /// <summary>
        ///     Tries to get a <see cref="CustomRole" /> by name.
        /// </summary>
        /// <param name="t">The <see cref="Type" /> of the role to get.</param>
        /// <param name="customRole">The custom role.</param>
        /// <returns>True if the role exists.</returns>
        /// <exception cref="ArgumentNullException">If the name is <see langword="null" /> or an empty string.</exception>
        public static bool TryGet(Type t, out CustomRole? customRole)
        {
            customRole = Get(t);

            return customRole is not null;
        }

        /// <summary>
        ///     Tries to get a <see cref="IReadOnlyCollection{T}" /> of the specified <see cref="Player" />'s
        ///     <see cref="CustomRole" />s.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <param name="customRoles">The custom roles the player has.</param>
        /// <returns>True if the player has custom roles.</returns>
        /// <exception cref="ArgumentNullException">If the player is <see langword="null" />.</exception>
        public static bool TryGet(Player player, out IReadOnlyCollection<CustomRole> customRoles)
        {
            if (player is null)
                throw new ArgumentNullException(nameof(player));

            List<CustomRole> tempList = ListPool<CustomRole>.Pool.Get();
            tempList.AddRange(Registered.Where(customRole => customRole.Check(player)) ?? Array.Empty<CustomRole>());

            customRoles = tempList.AsReadOnly();
            ListPool<CustomRole>.Pool.Return(tempList);

            return customRoles?.Count > 0;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomRole"/> by name.
        /// </summary>
        /// <param name="customRole">The custom role.</param>
        /// <typeparam name="T">The specified <see cref="CustomRole"/> type.</typeparam>
        /// <returns>True if the role exists.</returns>
        public static bool TryGet<T>(out T? customRole)
            where T : CustomRole
        {
            customRole = Get<T>();

            return customRole is not null;
        }

        /// <summary>
        ///     Registers all the <see cref="CustomRole" />'s present in the current assembly.
        /// </summary>
        /// <param name="skipReflection">
        ///     Whether or not reflection is skipped (more efficient if you are not using your custom item
        ///     classes as config objects).
        /// </param>
        /// <param name="overrideClass">The class to search properties for, if different from the plugin's config class.</param>
        /// <returns>
        ///     A <see cref="IEnumerable{T}" /> of <see cref="CustomRole" /> which contains all registered
        ///     <see cref="CustomRole" />'s.
        /// </returns>
        public static IEnumerable<CustomRole> RegisterRoles(bool skipReflection = false, object? overrideClass = null) => RegisterRoles(skipReflection, overrideClass, true, Assembly.GetCallingAssembly());

        /// <summary>
        ///     Registers all the <see cref="CustomRole" />'s present in the current assembly.
        /// </summary>
        /// <param name="skipReflection">
        ///     Whether or not reflection is skipped (more efficient if you are not using your custom item
        ///     classes as config objects).
        /// </param>
        /// <param name="overrideClass">The class to search properties for, if different from the plugin's config class.</param>
        /// <param name="inheritAttributes">Whether or not inherited attributes should be taken into account for registration.</param>
        /// <param name="assembly">Assembly which is calling this method.</param>
        /// <returns>
        ///     A <see cref="IEnumerable{T}" /> of <see cref="CustomRole" /> which contains all registered
        ///     <see cref="CustomRole" />'s.
        /// </returns>
        public static IEnumerable<CustomRole> RegisterRoles(bool skipReflection = false, object? overrideClass = null, bool inheritAttributes = true, Assembly? assembly = null)
        {
            List<CustomRole> roles = new();

            Log.Warn("Registering roles...");

            assembly ??= Assembly.GetCallingAssembly();

            foreach (Type type in assembly.GetTypes())
            {
                if (type.BaseType != typeof(CustomRole) && type.GetCustomAttribute(typeof(CustomRoleAttribute), inheritAttributes) is null)
                {
                    Log.Debug($"{type} base: {type.BaseType} -- {type.GetCustomAttribute(typeof(CustomRoleAttribute), inheritAttributes) is null}");
                    continue;
                }

                Log.Debug($"Getting attributed for {type}");
                foreach (Attribute attribute in type.GetCustomAttributes(typeof(CustomRoleAttribute), inheritAttributes).Cast<Attribute>())
                {
                    CustomRole? customRole = null;

                    if (!skipReflection && Server.PluginAssemblies.TryGetValue(assembly, out IPlugin<IConfig> plugin))
                    {
                        foreach (PropertyInfo property in overrideClass?.GetType().GetProperties() ?? plugin.Config.GetType().GetProperties())
                        {
                            if (property.PropertyType != type)
                                continue;

                            customRole = property.GetValue(overrideClass ?? plugin.Config) as CustomRole;
                            break;
                        }
                    }

                    customRole ??= (CustomRole)Activator.CreateInstance(type);

                    if (customRole.Role == RoleTypeId.None)
                        customRole.Role = ((CustomRoleAttribute)attribute).RoleTypeId;

                    if (customRole.TryRegister())
                        roles.Add(customRole);
                }
            }

            return roles;
        }

        /// <summary>
        ///     Registers all the <see cref="CustomRole" />'s present in the current assembly.
        /// </summary>
        /// <param name="targetTypes">The <see cref="IEnumerable{T}" /> of <see cref="Type" /> containing the target types.</param>
        /// <param name="isIgnored">A value indicating whether the target types should be ignored.</param>
        /// <param name="skipReflection">
        ///     Whether or not reflection is skipped (more efficient if you are not using your custom item
        ///     classes as config objects).
        /// </param>
        /// <param name="overrideClass">The class to search properties for, if different from the plugin's config class.</param>
        /// <returns>
        ///     A <see cref="IEnumerable{T}" /> of <see cref="CustomRole" /> which contains all registered
        ///     <see cref="CustomRole" />'s.
        /// </returns>
        public static IEnumerable<CustomRole> RegisterRoles(IEnumerable<Type> targetTypes, bool isIgnored = false, bool skipReflection = false, object? overrideClass = null)
        {
            List<CustomRole> roles = new();
            Assembly assembly = Assembly.GetCallingAssembly();

            foreach (Type type in assembly.GetTypes())
            {
                if (type.BaseType != typeof(CustomItem) ||
                    type.GetCustomAttribute(typeof(CustomRoleAttribute)) is null ||
                    (isIgnored == targetTypes.Contains(type)))
                {
                    continue;
                }

                foreach (Attribute attribute in type.GetCustomAttributes(typeof(CustomRoleAttribute), true).Cast<Attribute>())
                {
                    CustomRole? customRole = null;

                    if (!skipReflection && Server.PluginAssemblies.ContainsKey(assembly))
                    {
                        IPlugin<IConfig> plugin = Server.PluginAssemblies[assembly];

                        foreach (PropertyInfo property in overrideClass?.GetType().GetProperties() ??
                                                          plugin.Config.GetType().GetProperties())
                        {
                            if (property.PropertyType != type)
                                continue;

                            customRole = property.GetValue(overrideClass ?? plugin.Config) as CustomRole;
                        }
                    }

                    customRole ??= (CustomRole)Activator.CreateInstance(type);

                    if (customRole.Role == RoleTypeId.None)
                        customRole.Role = ((CustomRoleAttribute)attribute).RoleTypeId;

                    if (customRole.TryRegister())
                        roles.Add(customRole);
                }
            }

            return roles;
        }

        /// <summary>
        ///     Unregisters all the <see cref="CustomRole" />'s present in the current assembly.
        /// </summary>
        /// <returns>
        ///     A <see cref="IEnumerable{T}" /> of <see cref="CustomRole" /> which contains all unregistered
        ///     <see cref="CustomRole" />'s.
        /// </returns>
        public static IEnumerable<CustomRole> UnregisterRoles()
        {
            List<CustomRole> unregisteredRoles = new();

            foreach (CustomRole customRole in Registered)
            {
                customRole.TryUnregister();
                unregisteredRoles.Add(customRole);
            }

            return unregisteredRoles;
        }

        /// <summary>
        ///     Unregisters all the <see cref="CustomRole" />'s present in the current assembly.
        /// </summary>
        /// <param name="targetTypes">The <see cref="IEnumerable{T}" /> of <see cref="Type" /> containing the target types.</param>
        /// <param name="isIgnored">A value indicating whether the target types should be ignored.</param>
        /// <returns>
        ///     A <see cref="IEnumerable{T}" /> of <see cref="CustomRole" /> which contains all unregistered
        ///     <see cref="CustomRole" />'s.
        /// </returns>
        public static IEnumerable<CustomRole> UnregisterRoles(IEnumerable<Type> targetTypes, bool isIgnored = false)
        {
            List<CustomRole> unregisteredRoles = new();

            foreach (CustomRole customRole in Registered)
            {
                if (targetTypes.Contains(customRole.GetType()) == isIgnored)
                    continue;

                customRole.TryUnregister();
                unregisteredRoles.Add(customRole);
            }

            return unregisteredRoles;
        }

        /// <summary>
        ///     Unregisters all the <see cref="CustomRole" />'s present in the current assembly.
        /// </summary>
        /// <param name="targetRoles">The <see cref="IEnumerable{T}" /> of <see cref="CustomRole" /> containing the target roles.</param>
        /// <param name="isIgnored">A value indicating whether the target roles should be ignored.</param>
        /// <returns>
        ///     A <see cref="IEnumerable{T}" /> of <see cref="CustomRole" /> which contains all unregistered
        ///     <see cref="CustomRole" />'s.
        /// </returns>
        public static IEnumerable<CustomRole> UnregisterRoles(IEnumerable<CustomRole> targetRoles, bool isIgnored = false) => UnregisterRoles(targetRoles.Select(x => x.GetType()), isIgnored);

        /// <summary>
        ///     ResyncCustomRole Friendly Fire with Player (Append, or Overwrite).
        /// </summary>
        /// <param name="roleToSync"> <see cref="CustomRole" /> to sync with player. </param>
        /// <param name="player"> <see cref="Player" /> Player to add custom role to. </param>
        /// <param name="overwrite"> <see cref="bool" /> whether to force sync (Overwriting previous information). </param>
        public static void SyncPlayerFriendlyFire(CustomRole roleToSync, Player player, bool overwrite = false)
        {
            if (overwrite)
            {
                player.TryAddCustomRoleFriendlyFire(roleToSync.Name, roleToSync.CustomRoleFFMultiplier, overwrite);
                player.UniqueRole = roleToSync.Name;
            }
            else
            {
                player.TryAddCustomRoleFriendlyFire(roleToSync.Name, roleToSync.CustomRoleFFMultiplier);
            }
        }

        /// <summary>
        ///     Force sync CustomRole Friendly Fire with Player (Set to).
        /// </summary>
        /// <param name="roleToSync"> <see cref="CustomRole" /> to sync with player. </param>
        /// <param name="player"> <see cref="Player" /> Player to add custom role to. </param>
        public static void ForceSyncSetPlayerFriendlyFire(CustomRole roleToSync, Player player)
        {
            player.TrySetCustomRoleFriendlyFire(roleToSync.Name, roleToSync.CustomRoleFFMultiplier);
        }

        /// <summary>
        ///     Checks if the given player has this role.
        /// </summary>
        /// <param name="player">The <see cref="Player" /> to check.</param>
        /// <returns>True if the player has this role.</returns>
        public virtual bool Check(Player? player) => player is not null && TrackedPlayers.Contains(player);

        /// <summary>
        ///     Initializes this role manager.
        /// </summary>
        public virtual void Init()
        {
            IdLookupTable.Add(Id, this);
            SubscribeEvents();
        }

        /// <summary>
        ///     Destroys this role manager.
        /// </summary>
        public virtual void Destroy()
        {
            IdLookupTable.Remove(Id);
            UnsubscribeEvents();
        }

        /// <summary>
        /// Gives to a target <see cref="Player"/> items and ammos preset for current <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="player">Target <see cref="Player"/>.</param>
        public virtual void GivePreset(Player player)
        {
            try
            {
                player.TryGetSessionVariable("buffitems", out short itemsBuff);
                foreach (Dictionary<string, short> slot in Inventory)
                {
                    foreach (KeyValuePair<string, short> item in slot)
                    {
                        byte chance = (byte)Mathf.Clamp(item.Value + itemsBuff, 0, 100);
                        if (UnityEngine.Random.Range(0, 101) > chance)
                            continue;

                        try
                        {
                            if (CustomItem.TryGet(item.Key, out CustomItem? customItem))
                                customItem?.Give(player, DisplayCustomItemMessages);
                            else if (!uint.TryParse(item.Key, out _) && Enum.TryParse(item.Key, out ItemType itemType)) // Чтобы числа могли обозначать только кастомпредметы
                                player.AddItem(itemType);
                            else
                                Log.Error($"Error at adding items to custom role {Name} ({Id}). Wrong item: {item.Key}");
                        }
                        catch (Exception e)
                        {
                            Log.Error($"Failed to give item {item.Key} to {player}\nCustom role: {Name} ({Id})!\n{e}");
                        }

                        break;
                    }
                }

                foreach (KeyValuePair<AmmoType, ushort> ammo in Ammo)
                    player.SetAmmo(ammo.Key, ammo.Value);
            }
            catch (Exception e)
            {
                Log.Error($"Exception in customrole {Name} ({Id}) inventory delayed process for player {player}:\n{e}");
            }
        }

        /// <summary>
        /// Adds all <see cref="CustomRole"/> properties to a target <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player" /> to add the role to.</param>
        /// <param name="spawnReason">The <see cref="SpawnReason" /> to spawn player.</param>
        /// <param name="assignInventory">Should <see cref="Player"/> receive <see cref="CustomRole"/> inventory.</param>
        public virtual void AddProperties(Player player, SpawnReason spawnReason, bool assignInventory)
        {
            if (assignInventory)
            {
                player.ClearInventory();
                GivePreset(player);
            }

            Log.Debug($"{Name} ({Id}): Setting health values.");
            player.Health = MaxHealth;
            player.MaxHealth = MaxHealth;
            player.Scale = Scale;
            if (Gravity.HasValue && player.Role is FpcRole fpcRole)
                fpcRole.Gravity = Gravity.Value;

            Log.Debug($"{Name}: Setting player info");
            player.InfoArea &= ~PlayerInfoArea.Role;
            if (CustomInfo.ToLowerInvariant() != "none")
                player.CustomInfo = CustomInfo;

            if (Extensions.InternalPlayerToCustomRoles.TryGetValue(player, out CustomRole cr))
            {
                Log.Error($"player: {player} already has custom role in AddRole: cr is {cr.Name} ({cr.Id})");
                cr.TrackedPlayers.Remove(player);
                Extensions.InternalPlayerToCustomRoles.Remove(player);
            }

            TrackedPlayers.Add(player);
            Extensions.InternalPlayerToCustomRoles.Add(player, this);
            ShowMessage(player);
            player.UniqueRole = Name;
            player.TryAddCustomRoleFriendlyFire(Name, CustomRoleFFMultiplier);
        }

        /// <summary>
        ///     Handles setup of the role, including spawn location, inventory and registering event handlers and add FF rules.
        /// </summary>
        /// <param name="player">The <see cref="Player" /> to add the role to.</param>
        /// <param name="spawnReason">The <see cref="SpawnReason" /> to spawn player.</param>
        /// <param name="spawnFlags">The <see cref="RoleSpawnFlags" /> to spawn player.</param>
        public virtual void AddRole(Player player, SpawnReason spawnReason, RoleSpawnFlags spawnFlags) =>
            AddRole(player, spawnReason, spawnFlags, true);

        /// <summary>
        ///     Handles setup of the role, including spawn location, inventory and registering event handlers and add FF rules.
        /// </summary>
        /// <param name="player">The <see cref="Player" /> to add the role to.</param>
        /// <param name="spawnReason">The <see cref="SpawnReason" /> to spawn player.</param>
        /// <param name="spawnFlags">The <see cref="RoleSpawnFlags" /> to spawn player.</param>
        /// <param name="forceRole">Whether or not <see cref="Player"/> will be forced.</param>
        public virtual void AddRole(Player player, SpawnReason spawnReason, RoleSpawnFlags spawnFlags, bool forceRole)
        {
            Log.Debug($"{Name}: Adding role to {player.Nickname} with flags {spawnFlags}.");

            bool useSpawnpoint = spawnFlags.HasFlag(RoleSpawnFlags.UseSpawnpoint);
            bool assignInventory = spawnFlags.HasFlag(RoleSpawnFlags.AssignInventory);

            if (forceRole && Role != RoleTypeId.None)
            {
                RoleSpawnFlags flags = RoleSpawnFlags.None;
                if (!SpawnProperties.IsAny && useSpawnpoint)
                    flags |= RoleSpawnFlags.UseSpawnpoint;

                if (assignInventory)
                    Extensions.AssignInventoryPlayers.Add(player);

                Extensions.ToChangeRolePlayers[player] = this;
                player.Role.Set(Role, spawnReason, flags);

                Log.Debug($"{Name}: Set basic role (force) to {player.Nickname} with flags: {flags}.");
            }
            else
            {
                Log.Debug($"Спавним игрока {player.Nickname} по второму сценарию");

                if (SpawnProperties.IsAny && useSpawnpoint && NetworkServer.active && player.IsConnected)
                    player.Position = SpawnProperties.GetRandomPoint() + (Vector3.up * 1.5f);

                AddProperties(player, spawnReason, assignInventory);
                RoleAdded(player);
                Log.Debug($"{Name}: Set basic role (nonforce) to {player.Nickname}");
            }
        }

        /// <summary>
        ///     Returns player's nickname for spectators.
        /// </summary>
        /// <param name="player">Player to send text.</param>
        /// <returns>Spectator text of player.</returns>
        public string GetSpectatorText(Player player) =>
            string.IsNullOrEmpty(SpectatorText)
                ? player.CustomName
                : $"{player.CustomName} | {SpectatorText}";

        /// <summary>
        ///     Removes the role from a specific player and FF rules.
        /// </summary>
        /// <param name="player">The <see cref="Player" /> to remove the role from.</param>
        public virtual void RemoveRole(Player player)
        {
            Log.Debug($"{Name}: (before) Removing role from {player.Nickname}");

            if (!TrackedPlayers.Contains(player))
                return;
            Log.Debug($"{Name}: Removing role from {player.Nickname}");
            Extensions.InternalPlayerToCustomRoles.Remove(player);
            TrackedPlayers.Remove(player);
            if (CustomInfo.ToLowerInvariant() != "none")
                player.CustomInfo = string.Empty;
            player.InfoArea |= PlayerInfoArea.Role;
            player.Scale = Vector3.one;

            if (CustomAbilities is not null)
            {
                foreach (CustomAbility ability in CustomAbilities)
                {
                    ability.RemoveAbility(player);
                }
            }

            RoleRemoved(player);
            player.UniqueRole = string.Empty;
            player.TryRemoveCustomeRoleFriendlyFire(Name);
        }

        /// <summary>
        ///     Tries to add <see cref="RoleTypeId" /> to CustomRole FriendlyFire rules.
        /// </summary>
        /// <param name="roleToAdd"> Role to add. </param>
        /// <param name="ffMult"> Friendly fire multiplier. </param>
        public void SetFriendlyFire(RoleTypeId roleToAdd, float ffMult)
        {
            if (CustomRoleFFMultiplier.ContainsKey(roleToAdd))
            {
                CustomRoleFFMultiplier[roleToAdd] = ffMult;
            }
            else
            {
                CustomRoleFFMultiplier.Add(roleToAdd, ffMult);
            }
        }

        /// <summary>
        ///     Wrapper to call <see cref="SetFriendlyFire(RoleTypeId, float)" />.
        /// </summary>
        /// <param name="roleFF"> Role with FF to add even if it exists. </param>
        public void SetFriendlyFire(KeyValuePair<RoleTypeId, float> roleFF)
        {
            SetFriendlyFire(roleFF.Key, roleFF.Value);
        }

        /// <summary>
        ///     Tries to add <see cref="RoleTypeId" /> to CustomRole FriendlyFire rules.
        /// </summary>
        /// <param name="roleToAdd"> Role to add. </param>
        /// <param name="ffMult"> Friendly fire multiplier. </param>
        /// <returns> Whether the item was able to be added. </returns>
        public bool TryAddFriendlyFire(RoleTypeId roleToAdd, float ffMult)
        {
            if (CustomRoleFFMultiplier.ContainsKey(roleToAdd))
            {
                return false;
            }

            CustomRoleFFMultiplier.Add(roleToAdd, ffMult);
            return true;
        }

        /// <summary>
        ///     Tries to add <see cref="RoleTypeId" /> to CustomRole FriendlyFire rules.
        /// </summary>
        /// <param name="pairedRoleFF"> Role FF multiplier to add. </param>
        /// <returns> Whether the item was able to be added. </returns>
        public bool TryAddFriendlyFire(KeyValuePair<RoleTypeId, float> pairedRoleFF) => TryAddFriendlyFire(pairedRoleFF.Key, pairedRoleFF.Value);

        /// <summary>
        ///     Tries to add <see cref="RoleTypeId" /> to CustomRole FriendlyFire rules.
        /// </summary>
        /// <param name="ffRules"> Roles to add with friendly fire values. </param>
        /// <param name="overwrite"> Whether to overwrite current values if they exist. </param>
        /// <returns> Whether the item was able to be added. </returns>
        public bool TryAddFriendlyFire(Dictionary<RoleTypeId, float> ffRules, bool overwrite = false)
        {
            Dictionary<RoleTypeId, float> temporaryFriendlyFireRules = DictionaryPool<RoleTypeId, float>.Pool.Get();

            foreach (KeyValuePair<RoleTypeId, float> roleFF in ffRules)
            {
                if (overwrite)
                {
                    SetFriendlyFire(roleFF);
                }
                else
                {
                    if (!CustomRoleFFMultiplier.ContainsKey(roleFF.Key))
                    {
                        temporaryFriendlyFireRules.Add(roleFF.Key, roleFF.Value);
                    }
                    else
                    {
                        // Contained Key but overwrite set to false so we do not add any.
                        return false;
                    }
                }
            }

            if (!overwrite)
            {
                foreach (KeyValuePair<RoleTypeId, float> roleFF in temporaryFriendlyFireRules)
                {
                    TryAddFriendlyFire(roleFF);
                }
            }

            DictionaryPool<RoleTypeId, float>.Pool.Return(temporaryFriendlyFireRules);
            return true;
        }

        /// <summary>
        ///     Called after the role has been added to the player.
        /// </summary>
        /// <param name="player">The <see cref="Player" /> the role was added to.</param>
        public virtual void RoleAdded(Player player)
        {
            if (CustomAbilities is not null)
            {
                foreach (CustomAbility ability in CustomAbilities)
                    ability.AddAbility(player);
            }
        }

        /// <summary>
        ///     Tries to register this role.
        /// </summary>
        /// <returns>True if the role registered properly.</returns>
        internal bool TryRegister()
        {
            if (!CustomRoles.Instance!.Config.IsEnabled)
                return false;

            if (!Registered.Contains(this))
            {
                if (Registered.Any(r => r.Id == Id))
                {
                    Log.Error($"{Name} has tried to register with the same Role ID as another role: {Id}. It will not be registered!");

                    return false;
                }

                Registered.Add(this);
                Init();

                Log.Debug($"{Name} ({Id}) has been successfully registered.");

                return true;
            }

            Log.Error($"Couldn't register {Name} ({Id}) [{Role}] as it already exists.");

            return false;
        }

        /// <summary>
        ///     Tries to unregister this role.
        /// </summary>
        /// <returns>True if the role is unregistered properly.</returns>
        internal bool TryUnregister()
        {
            Destroy();

            if (!Registered.Remove(this))
            {
                Log.Warn($"Cannot unregister {Name} ({Id}) [{Role}], it hasn't been registered yet.");

                return false;
            }

            return true;
        }

        /// <summary>
        ///     Tries to add an item to the player's inventory by name.
        /// </summary>
        /// <param name="player">The <see cref="Player" /> to try giving the item to.</param>
        /// <param name="itemName">The name of the item to try adding.</param>
        /// <returns>Whether or not the item was able to be added.</returns>
        protected bool TryAddItem(Player player, string itemName)
        {
            if (CustomItem.TryGet(itemName, out CustomItem? customItem))
            {
                customItem?.Give(player);

                return true;
            }

            if (Enum.TryParse(itemName, out ItemType type))
            {
                if (type.IsAmmo())
                    player.Ammo[type] = 100;
                else
                    player.AddItem(type);

                return true;
            }

            Log.Warn($"{Name}: {nameof(TryAddItem)}: {itemName} is not a valid ItemType or Custom Item name.");

            return false;
        }

        /// <summary>
        ///     Called when the role is initialized to setup internal events.
        /// </summary>
        protected virtual void SubscribeEvents()
        {
            Log.Debug($"{Name}: Loading events.");

            Exiled.Events.Handlers.Player.ChangingRole += OnInternalChangingRole;

            if (ReplacesBaseRole)
                Exiled.Events.Handlers.Player.Spawned += OnInternalSpawned;
        }

        /// <summary>
        ///     Called when the role is destroyed to unsubscribe internal event handlers.
        /// </summary>
        protected virtual void UnsubscribeEvents()
        {
            foreach (Player player in TrackedPlayers)
                RemoveRole(player);

            Log.Debug($"{Name}: Unloading events.");

            Exiled.Events.Handlers.Player.ChangingRole -= OnInternalChangingRole;

            if (ReplacesBaseRole)
                Exiled.Events.Handlers.Player.Spawned -= OnInternalSpawned;
        }

        /// <summary>
        ///     Shows the spawn message to the player.
        /// </summary>
        /// <param name="player">The <see cref="Player" /> to show the message to.</param>
        protected virtual void ShowMessage(Player player) => player.ShowHint(string.Format(CustomRoles.Instance!.Config.GotRoleHint.Content, Name, Description), CustomRoles.Instance.Config.GotRoleHint.Duration);

        /// <summary>
        ///     Called after the role is removed from the player.
        /// </summary>
        /// <param name="player">The <see cref="Player" /> the role was removed from.</param>
        protected virtual void RoleRemoved(Player player)
        {
        }

        private void OnInternalChangingRole(ChangingRoleEventArgs ev)
        {
            if (Check(ev.Player))
            {
                RemoveRole(ev.Player);
            }
        }

        private void OnInternalSpawned(SpawnedEventArgs ev)
        {
            if (ev.Player.Role == Role && !ev.Player.HasCustomRole() && !Extensions.ToChangeRolePlayers.ContainsKey(ev.Player) && !ev.Player.SessionVariables.Remove(SkipBaseRoleReplaceKey))
            {
                try
                {
                    AddRole(ev.Player, SpawnReason.ForceClass, RoleSpawnFlags.All, false);
                }
                catch (Exception e)
                {
                    Log.Error($"[{nameof(CustomRole)}.{nameof(OnInternalChangingRole)}] [{Name}] Failed to add customRole-replacer of basic {Role}:\n{e}");
                }
            }
        }
    }
}
