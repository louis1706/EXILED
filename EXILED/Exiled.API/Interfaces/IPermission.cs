// -----------------------------------------------------------------------
// <copyright file="IPermission.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces
{
    using Enums;

    /// <summary>
    /// Represents interface of all objects with required permissions.
    /// </summary>
    public interface IPermission
    {
        /// <summary>
        /// Gets or sets a value indicating whether gets or sets required permissions for object.
        /// </summary>
        public abstract KeycardPermissions Permissions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IPermission object is opened or closed.
        /// </summary>
        public bool IsOpen { get; set; }
    }
}