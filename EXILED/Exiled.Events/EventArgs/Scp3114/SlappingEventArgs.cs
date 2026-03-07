// -----------------------------------------------------------------------
// <copyright file="SlappingEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp3114
{
    using API.Features;
    using Exiled.API.Features.Roles;
    using Interfaces;

    /// <summary>
    /// Contains all information before SCP-3114 slaps.
    /// </summary>
    public class SlappingEventArgs : IScp3114Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SlappingEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        public SlappingEventArgs(Player player)
        {
            Player = player;
            Scp3114 = Player.Role.As<Scp3114Role>();
        }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; } = true;

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp3114Role Scp3114 { get; }
    }
}