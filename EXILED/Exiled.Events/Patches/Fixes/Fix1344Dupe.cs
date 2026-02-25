// -----------------------------------------------------------------------
// <copyright file="Fix1344Dupe.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using HarmonyLib;
    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Usables.Scp1344;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches the <see cref="Scp1344Item.OnPlayerInventoryDropped"/> method.
    /// Fixes the dupe where 2 copies of SCP-1344 can be created when a player dies.
    /// Bug not reported to NW yet (rare in vanilla servers).
    /// </summary>
    [HarmonyPatch(typeof(Scp1344Item), nameof(Scp1344Item.OnPlayerInventoryDropped))]
    public class Fix1344Dupe
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            int offset = 1;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ret) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // this.OwnerInventory.UserInventory.Items
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp1344Item), nameof(Scp1344Item.OwnerInventory))),
                new(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory.UserInventory))),
                new(OpCodes.Ldfld, Field(typeof(InventoryInfo), nameof(InventoryInfo.Items))),

                // this.ItemSerial
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp1344Item), nameof(Scp1344Item.ItemSerial))),

                // if (!this.OwnerInventory.UserInventory.Items.ContainsKey(this.ItemSerial) return;
                new(OpCodes.Callvirt, Method(typeof(Dictionary<ushort, ItemBase>), nameof(Dictionary<ushort, ItemBase>.ContainsKey))),
                new(OpCodes.Brfalse_S, returnLabel),
            });

            offset = -1;
            index = newInstructions.FindIndex(x => x.OperandIs(Method(typeof(Scp1344Item), nameof(Scp1344Item.ActivateFinalEffects)))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // if (status != Scp1344Status.Activating) return;
                new CodeInstruction(OpCodes.Ldloc_0),
                new(OpCodes.Ldc_I4_2),
                new(OpCodes.Beq_S, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}