// -----------------------------------------------------------------------
// <copyright file="PluginManager.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.PluginManager
{
    using System;
    using System.Collections.Generic;

    using API.Features;
    using CommandSystem;

    /// <summary>
    /// The plugin manager.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class PluginManager : ParentCommand
    {
        /// <inheritdoc/>
        public override string Command { get; } = "pluginmanager";

        /// <inheritdoc/>
        public override string[] Aliases { get; set; } = new[] { "plymanager", "plmanager", "pmanager", "plym" };

        /// <inheritdoc/>
        public override string Description { get; set; } = "Manage plugin. Enable, disable and show plugins.";

        /// <inheritdoc/>
        protected override IEnumerable<Type> CommandsToRegister()
        {
            yield return typeof(Show);
            yield return typeof(Enable);
            yield return typeof(Disable);
            yield return typeof(Patches);
        }
    }
}
