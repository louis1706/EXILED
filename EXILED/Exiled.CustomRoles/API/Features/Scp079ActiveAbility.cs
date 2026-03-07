// -----------------------------------------------------------------------
// <copyright file="Scp079ActiveAbility.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.API.Features
{
    using System;
    using System.ComponentModel;

    using Exiled.API.Features;
    using Exiled.API.Features.Roles;

    using PlayerRoles;

    /// <summary>
    ///     The base class for active (on-use) energy-using abilities for SCP-079.
    /// </summary>
    public abstract class Scp079ActiveAbility : ActiveAbility
    {
        /// <summary>
        ///     Gets or sets minimal SCP-079 level to use this ability.
        /// </summary>
        public abstract byte MinRequiredLevel { get; set; }

        /// <summary>
        ///     Gets or sets maximum SCP-079 level to use this ability.
        /// </summary>
        [Description("Уровень, с которого способность перестанет отображаться и станет недоступна для использования.")]
        public virtual byte MaxRequiredLevel { get; set; } = 10;

        /// <summary>
        ///     Gets or sets energy usage for ability.
        /// </summary>
        public abstract float EnergyUsage { get; set; }

        /// <inheritdoc />
        public override bool CanUseAbility(Player player, out string response)
        {
            if (!player.Role.Is(out Scp079Role scp079Role))
            {
                response = "Вы не SCP-079, чтобы использовать эту команду.";
                return false;
            }

            if (scp079Role.Level < MinRequiredLevel)
            {
                Hint hint = CustomRoles.Instance!.Config.InsufficientLevelHint;
                response = string.Format(hint.Content, scp079Role.Level + 1, MinRequiredLevel + 1);
                if (hint.Show)
                    player.ShowHint(response, hint.Duration);
                return false;
            }

            if (scp079Role.Level > MaxRequiredLevel)
            {
                Hint hint = CustomRoles.Instance!.Config.RedundantLevelHint;
                response = string.Format(hint.Content, scp079Role.Level + 1, MaxRequiredLevel + 1);
                if (hint.Show)
                    player.ShowHint(response, hint.Duration);
                return false;
            }

            if (scp079Role.Energy < EnergyUsage)
            {
                Hint hint = CustomRoles.Instance!.Config.InsufficientEnergyHint;
                response = string.Format(hint.Content, scp079Role.Energy, EnergyUsage);
                if (hint.Show)
                    player.ShowHint(response, hint.Duration);
                return false;
            }

            return base.CanUseAbility(player, out response);
        }

        /// <summary>
        /// Checks if current <see cref="Player"/> is avaible for something krisprs mmnt.
        /// </summary>
        /// <param name="player">Target <see cref="Player"/>.</param>
        /// <returns>skibidi ohio rizz.</returns>
        internal bool IsAvailable(Player player) =>
            player.Role.Is(out Scp079Role scp079Role) && scp079Role.Level <= MaxRequiredLevel && (!CustomRoles.Instance!.Config.HideUnavailableHighLevelAbilities || scp079Role.Level >= MinRequiredLevel);

        /// <inheritdoc />
        protected override void AbilityUsed(Player player)
        {
            if (player.Role.Is(out Scp079Role role))
                role.Energy -= EnergyUsage;
        }
    }
}
