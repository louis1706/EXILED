// -----------------------------------------------------------------------
// <copyright file="Spawning.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using Mirror;

    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.FirstPersonControl.Spawnpoints;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RoleSpawnpointManager"/> delegate.
    /// Adds the <see cref="Handlers.Player.Spawning"/> event.
    /// Fix for spawning in void.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Spawning))]
    [HarmonyPatch(typeof(RoleSpawnpointManager), nameof(RoleSpawnpointManager.SetPosition))]
    internal static class Spawning
    {
        private static bool Prefix(ReferenceHub hub, PlayerRoleBase newRole)
        {
            if (newRole is not IFpcRole fpcRole)
            {
                CallEventForNonFpc(hub, newRole);
                return false;
            }

            ISpawnpointHandler spawnpointHandler = fpcRole.SpawnpointHandler;

            if (spawnpointHandler == null || !spawnpointHandler.TryGetSpawnpoint(out Vector3 position, out float horizontalRot) || !newRole.ServerSpawnFlags.HasFlag(RoleSpawnFlags.UseSpawnpoint))
            {
                position = hub.transform.position;
                horizontalRot = fpcRole.FpcModule.MouseLook?.CurrentHorizontal ?? 0.0f;
            }

            Player player = Player.Get(hub);

            SpawningEventArgs ev = new SpawningEventArgs(player, position, horizontalRot, newRole);
            Handlers.Player.OnSpawning(ev);

            hub.transform.position = ev.Position;
            if (fpcRole.FpcModule.MouseLook != null)
            {
                fpcRole.FpcModule.MouseLook.CurrentHorizontal = ev.HorizontalRotation;
            }

            return false;
        }

        private static void CallEventForNonFpc(ReferenceHub hub, PlayerRoleBase newRole)
        {
            Player player = Player.Get(hub);
            if (player == null)
                return;
            if (player.IsVerified || player.IsNPC)
            {
                SpawningEventArgs ev = new SpawningEventArgs(player, player.Position, 0.0f, newRole);
                Handlers.Player.OnSpawning(ev);
            }
        }
    }
}
