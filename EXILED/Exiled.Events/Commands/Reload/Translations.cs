// -----------------------------------------------------------------------
// <copyright file="Translations.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.Reload
{
    using System;

    using CommandSystem;

    using Exiled.Permissions.Extensions;

    using Loader;

    /// <summary>
    /// The reload translations command.
    /// </summary>
    public class Translations : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "translations";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public string Description { get; set; } = "Reload plugin translations.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("ee.reloadtranslations"))
            {
                response = "You can't reload translations, you don't have \"ee.reloadtranslations\" permission.";
                return false;
            }

            bool haveBeenReloaded = TranslationManager.Reload();

            Handlers.Server.OnReloadedTranslations();

            response = "Plugin translations have been reloaded successfully!";
            return haveBeenReloaded;
        }
    }
}
