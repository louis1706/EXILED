// -----------------------------------------------------------------------
// <copyright file="CustomHealthStat.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using PlayerRoles;
    using PlayerStatsSystem;

    /// <summary>
    /// A custom version of <see cref="HealthStat"/> which allows the player's max amount of health to be changed.
    /// </summary>
    public class CustomHealthStat : HealthStat
    {
        private float customMaxValue;

        /// <inheritdoc/>
        public override float MaxValue => customMaxValue == default ? base.MaxValue : CustomMaxValue;

        /// <summary>
        /// Gets or sets the maximum amount of health the player will have.
        /// </summary>
        public float CustomMaxValue
        {
            get
            {
                if (Hub.playerStats.TryGetModule(out MaxHealthStat maxHealthStat))
                    return maxHealthStat.CurValue + HumanRole.DefaultMaxHealth;
                return customMaxValue;
            }

            set
            {
                customMaxValue = value;
                if (Hub.playerStats.TryGetModule(out MaxHealthStat maxHealthStat))
                    maxHealthStat.CurValue = value - HumanRole.DefaultMaxHealth;
            }
        }
    }
}