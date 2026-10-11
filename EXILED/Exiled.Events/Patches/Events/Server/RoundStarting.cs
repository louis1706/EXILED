// -----------------------------------------------------------------------
// <copyright file="RoundStarting.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Server;
    using GameCore;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CharacterClassManager.Init()" />.
    /// Adds the <see cref="Handlers.Server.RoundStarting" /> event.
    /// </summary>
    [HarmonyPatch]
    internal class RoundStarting
    {
        #pragma warning disable SA1600 // Elements should be documented
        public static Type PrivateType { get; internal set; }

        private static MethodInfo TargetMethod()
        {
            PrivateType = typeof(CharacterClassManager).GetNestedTypes(all)
                .FirstOrDefault(currentType => currentType.Name.Contains("Init"));
            if (PrivateType == null)
                throw new Exception("State machine type for Init not found.");
            MethodInfo moveNextMethod = PrivateType.GetMethod("MoveNext", all);

            if (moveNextMethod == null)
                throw new Exception("MoveNext method not found in the state machine type.");
            return moveNextMethod;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const string TimeLeft = "<timeLeft>5__3";
            const string OriginalTimeLeft = "<originalTimeLeft>5__2";
            const string TopPlayer = "<topPlayers>5__4";

            LocalBuilder ev = generator.DeclareLocal(typeof(RoundStartingEventArgs));
            Label skipForIsAllowed = generator.DefineLabel();
            Label skipForPreventingCountDown = generator.DefineLabel();
            Label skipidk = generator.DefineLabel();

            int offset = -4;
            int index = newInstructions.FindLastIndex(x => x.Calls(Method(typeof(CharacterClassManager), nameof(CharacterClassManager.ForceRoundStart)))) + offset;

            List<Label> labels = newInstructions[index].ExtractLabels();
            newInstructions.RemoveRange(index, 4);

            newInstructions[index].labels.Add(skipidk);

            newInstructions.InsertRange(index, new[]
            {
                // this.TimeLeft
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(labels),
                new(OpCodes.Ldfld, Field(PrivateType, TimeLeft)),

                // this.OriginalTimeLeft
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(PrivateType, OriginalTimeLeft)),

                // this.TopPlayer
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(PrivateType, TopPlayer)),

                // playerCount
                new(OpCodes.Ldloc_2),

                // RoundStartingEventArgs ev = new(short, short, int, int)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RoundStartingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Server.OnRoundStarting(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnRoundStarting))),

                // this.TimeLeft = ev.TimeLeft
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(RoundStartingEventArgs), nameof(RoundStartingEventArgs.TimeLeft))),
                new(OpCodes.Stfld, Field(PrivateType, TimeLeft)),

                // this.OriginalTimeLeft = ev.OriginalTimeLeft
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(RoundStartingEventArgs), nameof(RoundStartingEventArgs.OriginalTimeLeft))),
                new(OpCodes.Stfld, Field(PrivateType, OriginalTimeLeft)),

                // this.TopPlayer = ev.TopPlayer
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(RoundStartingEventArgs), nameof(RoundStartingEventArgs.TopPlayer))),
                new(OpCodes.Stfld, Field(PrivateType, TopPlayer)),

                // if (!ev.IsAllowed)
                //   skip;
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(RoundStartingEventArgs), nameof(RoundStartingEventArgs.PreventCountdown))),
                new(OpCodes.Brtrue_S, skipForPreventingCountDown),

                // if (!ev.IsAllowed)
                //   skip;
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(RoundStartingEventArgs), nameof(RoundStartingEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, skipidk),

                // if (this.TimeLeft <= 0) goto skipForPreventingCountDown;
                // else goto skipForIsAllowed;
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(PrivateType, TimeLeft)),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ble_Un_S, skipForPreventingCountDown),
                new(OpCodes.Br, skipidk),
            });

            newInstructions.FindLast(x => x.Calls(PropertySetter(typeof(RoundStart), nameof(RoundStart.NetworkTimer)))).labels.Add(skipForPreventingCountDown);

            offset = 2;
            index = newInstructions.FindLastIndex(x => x.LoadsConstant(-2)) + offset;
            newInstructions[index].labels.Add(skipForIsAllowed);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
    }
