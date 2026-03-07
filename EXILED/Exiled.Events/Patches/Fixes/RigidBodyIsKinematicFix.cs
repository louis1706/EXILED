// -----------------------------------------------------------------------
// <copyright file="RigidBodyIsKinematicFix.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
#pragma warning disable SA1600
#pragma warning disable SA1313
#pragma warning disable SA1638
#pragma warning disable SA1649

namespace Exiled.Events.Patches.Fixes
{
    using HarmonyLib;
    using InventorySystem.Items.Pickups;

    /// <summary>
    /// Fixing RigidBody.isKinematic.
    /// </summary>
    [HarmonyPatch(typeof(PickupStandardPhysics), nameof(PickupStandardPhysics.ServerSendFreeze), MethodType.Getter)]
    internal static class RigidBodyIsKinematicFix
    {
        public static void Postfix(PickupStandardPhysics __instance, ref bool __result) => __result = __result || __instance.Rb.isKinematic;
    }
}
#pragma warning restore SA1600
#pragma warning restore SA1313
#pragma warning restore SA1638
#pragma warning restore SA1649