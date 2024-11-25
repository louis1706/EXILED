// -----------------------------------------------------------------------
// <copyright file="LiftList.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using API.Features;

    using HarmonyLib;
    using Interactables.Interobjects;

    /// <summary>
    /// Patches <see cref="ElevatorManager.SpawnAllChambers"/>.
    /// </summary>
    [HarmonyPatch(typeof(ElevatorManager), nameof(ElevatorManager.SpawnAllChambers))]
    internal class LiftList
    {
        private static void Postfix()
        {
            Lift.ElevatorChamberToLift.Clear();

            foreach (ElevatorChamber lift in ElevatorChamber.AllChambers)
            {
                _ = new Lift(lift);
            }
        }
    }
}