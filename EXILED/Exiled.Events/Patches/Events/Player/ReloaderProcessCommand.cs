// -----------------------------------------------------------------------
// <copyright file="ReloaderProcessCommand.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pools;
using Exiled.Events.EventArgs.Player;
using HarmonyLib;
using InventorySystem.Items.Autosync;
using InventorySystem.Items.Firearms.Modules;

using static HarmonyLib.AccessTools;

namespace Exiled.Events.Patches.Events.Player
{
    using Exiled.Events.Attributes;

    using Player = Exiled.Events.Handlers.Player;

    [EventPatch(typeof(Player), nameof(Player.ReloadingWeapon))]
    [HarmonyPatch(typeof(AnimationToggleableReloaderModule), nameof(AnimationToggleableReloaderModule.StartReloading))]
    internal static class ReloaderProcessCommand
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder player = generator.DeclareLocal(typeof(API.Features.Player));
            LocalBuilder firearm = generator.DeclareLocal(typeof(Firearm));

            Label ret = generator.DefineLabel();
            Label cont = generator.DefineLabel();

            // Index is correct, player and firearm exist, skips reload and continues to no action.
            newInstructions.InsertRange(0, new[]
            {
                // player = Player.Get(this.Item.Owner);
                new CodeInstruction(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AnimatorReloaderModuleBase), nameof(AnimatorReloaderModuleBase.Item))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ModularAutosyncItem), nameof(ModularAutosyncItem.Owner))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Isinst, typeof(API.Features.Player)),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, player.LocalIndex),
                new(OpCodes.Brfalse_S, cont),
                new(OpCodes.Ldstr, "Player"),
                new(OpCodes.Call, Method(typeof(Log), nameof(Log.Warn), new[] { typeof(string) })),

                // firearm = Firearm.Get(this.ItemSerial);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AnimatorReloaderModuleBase), nameof(AnimatorReloaderModuleBase.ItemSerial))),
                new(OpCodes.Call, GetDeclaredMethods(typeof(API.Features.Items.Item)).First(x => !x.IsGenericMethod && x.Name is nameof(API.Features.Items.Item.Get) && x.GetParameters().Length is 1 && x.GetParameters()[0].ParameterType == typeof(ushort))),
                new(OpCodes.Isinst, typeof(Firearm)),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, firearm.LocalIndex),
                new(OpCodes.Brfalse_S, cont),
                new(OpCodes.Ldstr, "Firearm"),
                new(OpCodes.Call, Method(typeof(Log), nameof(Log.Warn), new[] { typeof(string) })),

                // ReloadingWeaponEventArgs ev = new(player, firearm);
                // Player.OnReloadingWeapon(ev);
                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Ldloc_S, player.LocalIndex),
                new(OpCodes.Ldloc_S, firearm.LocalIndex),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ReloadingWeaponEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnReloadingWeapon))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReloadingWeaponEventArgs), nameof(ReloadingWeaponEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, ret),
                new CodeInstruction(OpCodes.Ldstr, "Continue").WithLabels(cont),
                new(OpCodes.Call, Method(typeof(Log), nameof(Log.Warn), new[] { typeof(string) })),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);
            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void thing(AnimatorReloaderModuleBase header)
        {
            Log.Warn(header.Firearm.Owner is null);
            Log.Warn(header.Item.Owner is null);
            Log.Warn(header.ItemSerial);
        }
    }
}