// -----------------------------------------------------------------------
// <copyright file="PassiveAbility.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.API.Features
{
    /// <summary>
    ///     The base class for passive (always active) abilities.
    /// </summary>
    public abstract class PassiveAbility : CustomAbility
    {
        /// <summary>
        ///     Gets or sets a value indicating whether ability will be dispayed in .roleinfo or not.
        /// </summary>
        public virtual bool IsHidden { get; set; } = false;
    }
}