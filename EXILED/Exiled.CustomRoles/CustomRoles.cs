// -----------------------------------------------------------------------
// <copyright file="CustomRoles.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles
{
    using Events;
    using Exiled.API.Features;

    using Player = Exiled.Events.Handlers.Player;
    using Server = Exiled.Events.Handlers.Server;

    /// <summary>
    ///     Handles all custom role API functions.
    /// </summary>
    public class CustomRoles : Plugin<Config>
    {
        private PlayerHandler playerHandler = null!;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CustomRoles" /> class.
        /// </summary>
        public CustomRoles()
        {
        }

        /// <summary>
        ///     Gets a static reference to the plugin's instance.
        /// </summary>
        public static CustomRoles? Instance { get; private set; }

        /// <inheritdoc />
        public override void OnEnabled()
        {
            Instance = this;
            playerHandler = new PlayerHandler();

            Server.WaitingForPlayers += playerHandler.OnWaitingForPlayers;

            Player.Spawned += playerHandler.OnSpawned;

            Player.ChangingRole += playerHandler.OnChangingRole;
            Player.SendingRole += playerHandler.OnSendingRole;
            Player.ChangedNickname += playerHandler.OnChangedNickname;

            base.OnEnabled();
        }

        /// <inheritdoc />
        public override void OnDisabled()
        {
            Server.WaitingForPlayers -= playerHandler.OnWaitingForPlayers;

            Player.Spawned -= playerHandler.OnSpawned;

            Player.ChangingRole -= playerHandler.OnChangingRole;
            Player.SendingRole -= playerHandler.OnSendingRole;
            Player.ChangedNickname -= playerHandler.OnChangedNickname;

            Instance = null;
            base.OnDisabled();
        }
    }
}