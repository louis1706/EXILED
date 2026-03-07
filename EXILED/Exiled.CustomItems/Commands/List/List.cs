// -----------------------------------------------------------------------
// <copyright file="List.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.Commands.List
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Features;

    /// <summary>
    /// The command to list all installed items.
    /// </summary>
    internal sealed class List : ParentCommand
    {
        /// <inheritdoc/>
        public override string Command { get; } = "list";

        /// <inheritdoc/>
        public override string[] Aliases { get; set; } = { "s", "l", "show", "sh" };

        /// <inheritdoc/>
        public override string Description { get; set; } = "Gets a list of all currently registered custom items.";

        /// <inheritdoc/>
        protected override IEnumerable<Type> CommandsToRegister()
        {
            yield return typeof(Registered);
            yield return typeof(Tracked);
        }
    }
}