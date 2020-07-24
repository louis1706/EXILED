// -----------------------------------------------------------------------
// <copyright file="Left.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="ReferenceHub.OnDestroy"/>.
    /// Adds the <see cref="Player.Left"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.OnDestroy))]
    internal static class Left
    {
        private static void Prefix(ReferenceHub __instance)
        {
            try
            {
                if (__instance.characterClassManager.IsHost)
                    return;

                API.Features.Player player = API.Features.Player.Get(__instance.gameObject);

                if (player == null || string.IsNullOrEmpty(player.UserId))
                    return;

                var ev = new LeftEventArgs(player);

                API.Features.Log.Debug($"Player {ev.Player?.Nickname} ({ev.Player?.UserId}) disconnected");

                Player.OnLeft(ev);

                API.Features.Player.IdsCache.Remove(__instance.queryProcessor.PlayerId);
                API.Features.Player.UserIdsCache.Remove(__instance.characterClassManager.UserId);
                API.Features.Player.Dictionary.Remove(__instance.gameObject);
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.Left: {e}\n{e.StackTrace}");
            }
        }
    }
}
