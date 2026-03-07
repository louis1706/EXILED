// -----------------------------------------------------------------------
// <copyright file="Plugins.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.Reload
{
    using System;

    using CommandSystem;

    using Exiled.Events.Handlers;
    using Exiled.Permissions.Extensions;

    using Loader;

    /// <summary>
    /// The reload plugins command.
    /// </summary>
    public class Plugins : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "plugins";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new[] { "pl" };

        /// <inheritdoc/>
        public string Description { get; set; } = "Reloads all plugins.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("ee.reloadplugins"))
            {
                response = "You can't reload plugins, you don't have \"ee.reloadplugins\" permission.";
                return false;
            }

            sender.Respond("Reloading plugins...");

            Loader.ReloadPlugins();

            Server.OnReloadedPlugins();

            response = "Plugins have been reloaded successfully!";
            return true;
        }
    }
}