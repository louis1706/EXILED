// -----------------------------------------------------------------------
// <copyright file="Permissions.cs" company="ExMod Team">
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

    /// <summary>
    /// The reload permissions command.
    /// </summary>
    public class Permissions : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "permissions";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new[] { "perms" };

        /// <inheritdoc/>
        public string Description { get; set; } = "Reload permissions.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("ee.reloadpermissions"))
            {
                response = "You can't reload permissions, you don't have \"ee.reloadpermissions\" permission.";
                return false;
            }

            Exiled.Permissions.Extensions.Permissions.Reload();
            Server.OnReloadedPermissions();

            response = "Permissions have been reloaded successfully!";
            return true;
        }
    }
}