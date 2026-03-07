// -----------------------------------------------------------------------
// <copyright file="BoneType.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Более подробная чем <see cref="HitboxType"/> точка попадания в игрока.
    /// </summary>
    public enum BoneType
    {
        /// <summary>
        /// Попадание в голову.
        /// </summary>
        Head = 0,

        /// <summary>
        /// Попадание в тело.
        /// </summary>
        Body = 1,

        /// <summary>
        /// Попадание в левую руку.
        /// </summary>
        LeftHand = 2,

        /// <summary>
        /// Попадание в правую руку.
        /// </summary>
        RightHand = 3,

        /// <summary>
        /// Попадание в левую ногу.
        /// </summary>
        LeftLeg = 4,

        /// <summary>
        /// Попадание в правую ногу.
        /// </summary>
        RightLeg = 5,

        /// <summary>
        /// Зону попадания не удалось определить.
        /// </summary>
        Unknown = 6,
    }
}
