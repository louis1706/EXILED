// -----------------------------------------------------------------------
// <copyright file="ISettingHandler.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Interfaces
{
    using UserSettings;

    /// <summary>
    /// Represents a config of ServerSpecific keybinds.
    /// </summary>
    public interface ISettingHandler
    {
        /// <summary>
        /// Creating a SettingBase Instanse.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="setting">Setting.</param>
        void Handle(Player player, SettingBase setting);
    }
}