// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Permissions
{
    using System.ComponentModel;
    using System.IO;

    using API.Interfaces;

    using Exiled.API.Features;

    using YamlDotNet.Serialization;

    /// <inheritdoc cref="IConfig"/>
    public sealed class Config : IConfig
    {
        /// <summary>
        /// Gets a value indicating whether the debug should be shown or not.
        /// </summary>
        [Description("Indicates whether the debug should be shown or not")]
        public bool ShouldDebugBeShown { get; private set; }

        /// <summary>
        /// Gets the full permissions path.
        /// </summary>
        [Description("The full permissions path")]
        [YamlIgnore]
        public string FullPath => Path.Combine(Paths.Configs, FileName);

        /// <summary>
        /// Gets the permissions name.
        /// </summary>
        [Description("The permissions file name")]
        public string FileName { get; private set; } = "permissions.yml";

        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <inheritdoc/>
        public bool Debug { get; set; }
    }
}