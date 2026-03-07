// -----------------------------------------------------------------------
// <copyright file="Reload.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.Reload
{
    using System;
    using System.Collections.Generic;

    using API.Features;
    using CommandSystem;

    /// <summary>
    /// The reload command.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Reload : ParentCommand
    {
        /// <inheritdoc/>
        public override string Command { get; } = "reload";

        /// <inheritdoc/>
        public override string[] Aliases { get; set; } = new[] { "rld" };

        /// <inheritdoc/>
        public override string Description { get; set; } = "Reload plugins, configs, gameplay configs, remote admin configs, translations, permissions or all of them.";

        /// <inheritdoc/>
        protected override IEnumerable<Type> CommandsToRegister()
        {
            yield return typeof(Configs);
            yield return typeof(Translations);
            yield return typeof(Plugins);
            yield return typeof(GamePlay);
            yield return typeof(RemoteAdmin);
            yield return typeof(Permissions);
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.At(0).ToLower() != "all")
                return base.ExecuteParent(arguments, sender, out response);

            foreach (ICommand child in Commands.Values)
            {
                bool done = child.Execute(arguments, sender, out string localResponse);
                sender.Respond(localResponse, done);
            }

            response = "Executed all reloads.";
            return false;
        }
    }
}