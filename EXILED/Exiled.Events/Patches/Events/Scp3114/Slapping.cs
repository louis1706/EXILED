// -----------------------------------------------------------------------
// <copyright file="Slapping.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp3114
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp3114;
    using Exiled.Events.Handlers;

    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp3114;
    using PlayerRoles.PlayableScps.Subroutines;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp3114Slap.DamagePlayers" />.
    /// Adds the <see cref="Handlers.Scp3114.Slapped" /> event.
    /// </summary>
    [EventPatch(typeof(Scp3114), nameof(Scp3114.Slapping))]
    [HarmonyPatch(typeof(ScpAttackAbilityBase<Scp3114Role>), nameof(ScpAttackAbilityBase<Scp3114Role>.ServerPerformAttack))]
    internal class Slapping
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();
            int index = newInstructions.FindIndex(instruction =>
                instruction.opcode == OpCodes.Stfld) + 1;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Player.Get(this.Owner)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ScpAttackAbilityBase<Scp3114Role>), nameof(ScpAttackAbilityBase<Scp3114Role>.Owner))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // SlappingEventArgs ev = new(player);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SlappingEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Scp3114.OnSlapping(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Scp3114), nameof(Handlers.Scp3114.OnSlapping))),

                // if (ev.IsAllowed)
                //     goto continueLabel;
                new(OpCodes.Callvirt, PropertyGetter(typeof(SlappingEventArgs), nameof(SlappingEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),

                // return;
                new(OpCodes.Ret),

                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
