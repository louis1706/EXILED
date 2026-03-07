// -----------------------------------------------------------------------
// <copyright file="BoneTypeExtensions.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using UnityEngine;

    /// <summary>
    /// Содержит методы для определения точки попадания в игрока.
    /// </summary>
    public static class BoneTypeExtensions
    {
        /// <summary>
        /// Определяет точку попадания в игрока по конкретному хитбоксу.
        /// </summary>
        /// <param name="target">Игрок, попадание в которого рассчитывется.</param>
        /// <param name="hitboxIdentity">Хитбокс, попадание в которого.</param>
        /// <returns>Точная зона попадания в игрока.</returns>
        public static BoneType GetByMassCenter(this Player target, HitboxIdentity hitboxIdentity)
        {
            BoneType boneType;
            Vector3 boneLocalMassCenter = target.Transform.InverseTransformPoint(hitboxIdentity.CenterOfMass);
            switch (hitboxIdentity._dmgMultiplier)
            {
                case HitboxType.Headshot:
                    boneType = BoneType.Head;
                    break;
                case HitboxType.Body:
                    if (IsArms(boneLocalMassCenter))
                        boneType = IsRight(boneLocalMassCenter) ? BoneType.RightHand : BoneType.LeftHand;
                    else
                        boneType = BoneType.Body;
                    break;
                case HitboxType.Limb:
                    boneType = IsRight(boneLocalMassCenter) ? BoneType.RightLeg : BoneType.LeftLeg;
                    break;
                default:
                    boneType = BoneType.Unknown;
                    break;
            }

            Log.Debug($"[{nameof(GetByMassCenter)}] [{hitboxIdentity._dmgMultiplier}] at {boneLocalMassCenter}: {boneType}");
            return boneType;
        }

        private static bool IsArms(Vector3 point) => point.z <= -0.2 || Math.Abs(point.x) > 0.1;

        private static bool IsRight(Vector3 point) => point.x > 0;
    }
}
