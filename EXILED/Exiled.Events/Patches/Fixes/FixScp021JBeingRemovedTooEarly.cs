// -----------------------------------------------------------------------
// <copyright file="FixScp021JBeingRemovedTooEarly.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using HarmonyLib;
    using InventorySystem;
    using InventorySystem.Items.MarshmallowMan;
    using InventorySystem.Items.Usables;
    using Mirror;

    using static HarmonyLib.AccessTools;

#pragma warning disable SA1313
    /// <summary>
    /// Patches <see cref="MarshmallowEffect.Enabled" />'s setter.
    /// Fix for the.
    /// </summary>
    [HarmonyPatch(typeof(MarshmallowEffect), nameof(MarshmallowEffect.Enabled))]
    internal class FixScp021JBeingRemovedTooEarly
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = -3;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerDropEverything)))) + offset;
            List<Label> labels = newInstructions[index].labels;
            newInstructions.RemoveRange(index, 4);
            newInstructions.InsertRange(
                index,
                new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0).WithLabels(labels),
                    new(OpCodes.Call, Method(typeof(FixScp021JBeingRemovedTooEarly), nameof(FixScp021JBeingRemovedTooEarly.HelperMethod))),
                    new(OpCodes.Ret),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void HelperMethod(MarshmallowEffect __instance)
        {
            if (!Player.TryGet(__instance.Hub, out Player player))
                return;

            player.DropAllAmmo();

            foreach (API.Features.Items.Item value in player.Items)
            {
                if (value.Base is Scp021J scp021J && scp021J.ActivationReady)
                {
                    Log.Info("CorrectlyPrevent Scp021J being removed");
                    continue;
                }

                value.Base.ServerDropItem(true);
            }
        }
    }
}