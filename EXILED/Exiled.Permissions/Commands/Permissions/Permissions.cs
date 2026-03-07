// -----------------------------------------------------------------------
// <copyright file="Permissions.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Permissions.Commands.Permissions
{
    using System;
    using System.Collections.Generic;

    using API.Features;
    using CommandSystem;

    /// <summary>
    /// Handles commands about permissions.
    /// </summary>
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Permissions : ParentCommand
    {
        /// <inheritdoc/>
        public override string Command { get; } = "permissions";

        /// <inheritdoc/>
        public override string[] Aliases { get; set; } = new[] { "ep" };

        /// <inheritdoc/>
        public override string Description { get; set; } = "Handles commands about permissions";

        /// <inheritdoc/>
        protected override IEnumerable<Type> CommandsToRegister()
        {
            yield return typeof(Reload);
            yield return typeof(Group.Group);
            yield return typeof(Add);
            yield return typeof(Remove);
        }
    }
}