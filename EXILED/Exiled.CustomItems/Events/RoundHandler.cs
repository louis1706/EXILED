// -----------------------------------------------------------------------
// <copyright file="RoundHandler.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.Events
{
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.Handlers;

    /// <summary>
    /// Event Handlers for the CustomItem API.
    /// </summary>
    internal sealed class RoundHandler
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRoundStarted"/>
        public void OnRoundStarted()
        {
            foreach (CustomItem customItem in CustomItem.Registered)
                customItem?.SpawnAll();
        }

        /// <inheritdoc cref="Server.OnWaitingForPlayers"/>
        public void OnWaitingForPlayers()
        {
            foreach (CustomItem? customItem in CustomItem.Registered)
                customItem.TrackedSerials.Clear();
        }
    }
}