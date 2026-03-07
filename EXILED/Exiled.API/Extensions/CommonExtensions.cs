// -----------------------------------------------------------------------
// <copyright file="CommonExtensions.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using PlayerRoles;

    using UnityEngine;

    using Random = UnityEngine.Random;

    /// <summary>
    /// A set of extensions for common things.
    /// </summary>
    public static class CommonExtensions
    {
        /// <summary>
        /// Gets a random value from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="enumerable"><see cref="IEnumerable{T}"/> to be used to get a random value.</param>
        /// <typeparam name="T">Type of <see cref="IEnumerable{T}"/> elements.</typeparam>
        /// <returns>Returns a random value from <see cref="IEnumerable{T}"/>.</returns>
        public static T GetRandomValue<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable is null)
                return default;

            T[] array = enumerable.ToArray();
            return array.Length == 0 ? default : array[Random.Range(0, array.Length)];
        }

        /// <summary>
        /// Gets a random value from an <see cref="IEnumerable{T}"/> that matches the provided condition.
        /// </summary>
        /// <param name="enumerable"><see cref="IEnumerable{T}"/> to be used to get a random value.</param>
        /// <typeparam name="T">Type of <see cref="IEnumerable{T}"/> elements.</typeparam>
        /// <param name="condition">The condition to require.</param>
        /// <returns>Returns a random value from <see cref="IEnumerable{T}"/>.</returns>
        public static T GetRandomValue<T>(this IEnumerable<T> enumerable, System.Func<T, bool> condition)
        {
            if (enumerable is null)
                return default;

            T[] array = enumerable.Where(condition).ToArray();
            return array.Length == 0 ? default : array.GetRandomValue();
        }

        /// <summary>
        /// Modify the curve with the amount used.
        /// </summary>
        /// <param name="curve">The AnimationCurve to modify.</param>
        /// <param name="amount">The multiplier number.</param>
        /// <returns>The new modfied curve.</returns>
        public static AnimationCurve Multiply(this AnimationCurve curve, float amount)
        {
            Keyframe[] keys = curve.keys;
            for (int i = 0; i < keys.Length; i++)
                keys[i].value *= amount;

            curve.keys = keys;
            return curve;
        }

        /// <summary>
        /// Modify the curve with the amount used.
        /// </summary>
        /// <param name="curve">The AnimationCurve to mofify.</param>
        /// <param name="amount">The add number.</param>
        /// <returns>The new modfied curve.</returns>
        public static AnimationCurve Add(this AnimationCurve curve, float amount)
        {
            Keyframe[] keys = curve.keys;
            for (int i = 0; i < keys.Length; i++)
                keys[i].value += amount;

            curve.keys = keys;
            return curve;
        }

        /// <summary>
        /// Adds an action for OnCollisionEnter event of specified gameObject.
        /// </summary>
        /// <param name="gameObject">GameObject to attach action.</param>
        /// <param name="action">Action on collision.</param>
        /// <param name="owner">GameObject that will be ignored in collision.</param>
        /// <param name="fuseDelay">Delay before collision may be proceeded.</param>
        public static void AttachActionOnCollision(this GameObject gameObject, Action action, Player owner = null, float fuseDelay = 0.15f)
        {
            gameObject.AddComponent<Exiled.API.Features.Components.CollisionHandler>().Init((owner ?? Server.Host).GameObject, action, fuseDelay);
        }

        /// <summary>
        /// Заменяет вспомогательные теги в тексте (в основном для кесси).
        /// </summary>
        /// <param name="text">Искомый текст.</param>
        /// <returns>Новый текст.</returns>
        public static string ReplaceVars(this string text) => text
            .Replace("{classd}", Player.List.Count(x => x.Role.Team == Team.ClassD).ToString())
            .Replace("{scps}", Player.List.Count(x => x.Role.Team == Team.SCPs).ToString())
            .Replace("{scpsno079}", Player.List.Count(x => x.Role.Team == Team.SCPs && x.Role.Type != RoleTypeId.Scp079).ToString())
            .Replace("{scientists}", Player.List.Count(x => x.Role.Team == Team.Scientists).ToString())

            .Replace("{mtf}", Player.List.Count(x => x.Role.Team == Team.FoundationForces && x.Role.Type != RoleTypeId.FacilityGuard).ToString())
            .Replace("{guards}", Player.List.Count(x => x.Role.Type == RoleTypeId.FacilityGuard).ToString())
            .Replace("{foundationforces}", Player.List.Count(x => x.Role.Team == Team.FoundationForces).ToString())

            .Replace("{ci}", Player.List.Count(x => x.Role.Team == Team.ChaosInsurgency).ToString())
            .Replace("{human}", Player.List.Count(x => x.IsHuman).ToString());
    }
}
