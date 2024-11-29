// -----------------------------------------------------------------------
// <copyright file="InteractingElevator.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using Interactables.Interobjects;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ElevatorManager" />.
    /// Adds the <see cref="Handlers.Player.InteractingElevator" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.InteractingElevator))]
    [HarmonyPatch(typeof(ElevatorChamber), nameof(ElevatorChamber.ServerInteract))]
    internal class InteractingElevator
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            // InteractingElevatorEventArgs ev = new(Player.Get(referenceHub), elevatorChamber, true);
            //
            // Handlers.Player.OnInteractingElevator(ev);
            //
            // if (!ev.IsAllowed)
            //     continue;
            newInstructions.InsertRange(0, new CodeInstruction[]
                {
                    // Player.Get(referenceHub)
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // elevatorChamber
                    new(OpCodes.Ldarg_0),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // InteractingElevatorEventArgs ev = new(Player, ElevatorChamber, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingElevatorEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnInteractingElevator(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnInteractingElevator))),

                    // if (!ev.IsAllowed)
                    //     continue;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingElevatorEventArgs), nameof(InteractingElevatorEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}