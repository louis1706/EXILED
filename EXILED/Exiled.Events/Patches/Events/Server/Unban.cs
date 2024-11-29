// -----------------------------------------------------------------------
// <copyright file="Unban.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Attributes;
    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Server;
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="BanHandler.RemoveBan"/>
    /// to add <see cref="Handlers.Server.Unbanning"/> and <see cref="Handlers.Server.Unbanned"/> events.
    /// </summary>
    [HarmonyPatch(typeof(BanHandler), nameof(BanHandler.RemoveBan))]
    [EventPatch(typeof(Handlers.Server), nameof(Handlers.Server.Unbanning))]
    [EventPatch(typeof(Handlers.Server), nameof(Handlers.Server.Unbanned))]
    internal class Unban
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(UnbanningEventArgs));

            Label continueLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // id
                new(OpCodes.Ldarg_0),

                // type
                new(OpCodes.Ldarg_1),

                // true
                new(OpCodes.Ldc_I4_1),

                // UnbanningEventArgs ev = new(string, BanHandler.BanType, true);
                new(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(UnbanningEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Server.OnUnbanning(ev);
                new(OpCodes.Call, AccessTools.Method(typeof(Handlers.Server), nameof(Handlers.Server.OnUnbanning))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(UnbanningEventArgs), nameof(UnbanningEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),

                new(OpCodes.Ret),

                // id = ev.TargetId;
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(continueLabel),
                new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(UnbanningEventArgs), nameof(UnbanningEventArgs.TargetId))),
                new(OpCodes.Starg_S, 0),
            });

            newInstructions.InsertRange(newInstructions.Count - 1, new CodeInstruction[]
            {
                // id
                new(OpCodes.Ldarg_0),

                // type
                new(OpCodes.Ldarg_1),

                // UnbannedEventArgs ev2 = new(string, BanHandler.BanType);
                new(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(UnbannedEventArgs))[0]),

                // Handlers.Server.OnUnbanned(ev2);
                new(OpCodes.Call, AccessTools.Method(typeof(Handlers.Server), nameof(Handlers.Server.OnUnbanned))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}