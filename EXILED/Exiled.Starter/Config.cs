// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Starter
{
    using System.ComponentModel;
    using System.IO;

    /// <summary>
    /// The configs of the loader.
    /// </summary>
    public sealed class Config
    {
        /// <summary>
        /// Gets or sets a value indicating whether the plugin is enabled or not.
        /// </summary>
        [Description("Whether or not this plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether debug messages should be displayed in the console or not.
        /// </summary>
        [Description("Whether or not debug messages should be shown in the console.")]
        public bool Debug { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether outdated Exiled versions should be loaded or not.
        /// </summary>
        [Description("Indicates whether outdated Exiled versions should be loaded or not.")]
        public bool ShouldLoadOutdatedExiled { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether outdated plugins should be loaded or not.
        /// </summary>
        [Description("Indicates whether outdated plugins should be loaded or not.")]
        public bool ShouldLoadOutdatedPlugins { get; set; } = true;

        /// <summary>
        /// Gets or sets the Exiled directory path from which plugins will be loaded.
        /// </summary>
        [Description("The Exiled directory path from which plugins will be loaded.")]
        public string ExiledDirectoryPath { get; set; } = Path.Combine(LoaderPlugin.Paths.AppData, "EXILED");

        /// <summary>
        /// Gets or sets a value indicating whether the command translation config display type.
        /// </summary>
        [Description("Should the loader replace 'default' tags with real values in command translations.")]
        public bool PrintFullCommandProps { get; set; } = false;
    }
}
