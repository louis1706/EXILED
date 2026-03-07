// -----------------------------------------------------------------------
// <copyright file="UseAbility.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace Exiled.CustomRoles.Commands.User
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using API;
    using API.Features;
    using CommandSystem;
    using Exiled.API.Features;

    /// <summary>
    ///     Handles the using of custom role abilities.
    /// </summary>
    [CommandHandler(typeof(ClientCommandHandler))]
    public class UseAbility : ICommand
    {
        /// <inheritdoc />
        public string Command { get; set; } = "ability";

        /// <inheritdoc />
        public string[] Aliases { get; set; } = { "a" };

        /// <inheritdoc />
        public string Description { get; set; } = "Использует спецспособность";

        /// <summary>
        /// Gets or sets the response when no custom role is found.
        /// </summary>
        public string NoCustomRoleResponse { get; set; } = "У вас нет спецролей со спецспособностями";

        /// <summary>
        /// Gets or sets the response when no abilities detected.
        /// </summary>
        public string NoAbilitiesResponse { get; set; } = "У вашей спецроли нет спецспособностей!";

        /// <summary>
        /// Gets or sets the response when an invalid ability number is detected.
        /// </summary>
        public string InvalidAbilityNumberResponse { get; set; } = "Такая способность не существует!";

        /// <summary>
        /// Gets or sets the response when invalid arguments detected.
        /// </summary>
        public string InvalidArgumentResponse { get; set; } = "{0} не является числом!";

        /// <summary>
        /// Gets or sets the response when an ability is already used.
        /// </summary>
        public string AbilityUsedResponse { get; set; } = "Способность {0} успешно использована!";

        /// <inheritdoc />
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get((CommandSender)sender);

            if (!player.TryGetCustomRole(out CustomRole role))
            {
                response = NoCustomRoleResponse;
                return false;
            }

            if (arguments.Count == 0)
            {
                response = string.Format(CustomRoles.Instance!.Config.UseAbilityResponse, RoleInfo.GetAbilitiesInfo(player, role));
                return false;
            }

            if (role.CustomAbilities == null)
            {
                response = NoAbilitiesResponse;
                return false;
            }

            List<ActiveAbility> activeAbilities = new();
            foreach (CustomAbility? ability in role.CustomAbilities)
            {
                if (ability is Scp079ActiveAbility scp079ActiveAbility)
                {
                    if (!scp079ActiveAbility.IsAvailable(player))
                        continue;

                    activeAbilities.Add(scp079ActiveAbility);
                }
                else if (ability is ActiveAbility activeAbility)
                {
                    activeAbilities.Add(activeAbility);
                }
            }

            if (activeAbilities.IsEmpty())
            {
                response = NoAbilitiesResponse;
                return false;
            }

            int abilityNumber = 0;
            if (arguments.Count > 0)
            {
                if (!int.TryParse(arguments.At(0), out abilityNumber))
                {
                    response = string.Format(InvalidArgumentResponse, arguments.At(0));
                    return false;
                }
            }

            if (activeAbilities.Count < abilityNumber)
            {
                response = InvalidAbilityNumberResponse;
                return false;
            }

            if (!activeAbilities[abilityNumber - 1].CanUseAbility(player, out string res))
            {
                response = res;
                player.ShowHint(response, 5);
                return false;
            }

            activeAbilities[abilityNumber - 1].UseAbility(player);
            response = string.Format(AbilityUsedResponse, activeAbilities[abilityNumber - 1].Name);
            return false;
        }
    }
}