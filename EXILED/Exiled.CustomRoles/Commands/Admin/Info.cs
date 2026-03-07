// -----------------------------------------------------------------------
// <copyright file="Info.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.Commands.Admin
{
    using System;
    using System.Text;

    using API.Features;

    using CommandSystem;

    using Exiled.API.Features.Pools;

    using Permissions.Extensions;

    /// <summary>
    ///     The command to view info about a specific role.
    /// </summary>
    internal sealed class Info : ICommand
    {
        /// <inheritdoc />
        public string Command { get; set; } = "info";

        /// <inheritdoc />
        public string[] Aliases { get; set; } = { "i" };

        /// <inheritdoc />
        public string Description { get; set; } = "Информация про кастомную роль.";

        /// <summary>
        /// Gets or sets the usage message for the command.
        /// </summary>
        public string Usage { get; set; } = "info [Название/ID кастомной роли]";

        /// <summary>
        /// Gets or sets the error message displayed when a role is not found.
        /// </summary>
        public string ErrorNoRole { get; set; } = "{0} не идентификатор кастомной роли.";

        /// <summary>
        /// Gets or sets the first color used in the role information display.
        /// </summary>
        public string Color1 { get; set; } = "#E6AC00";

        /// <summary>
        /// Gets or sets the second color used in the role information display.
        /// </summary>
        public string Color2 { get; set; } = "#00D639";

        /// <summary>
        /// Gets or sets the third color used in the role information display.
        /// </summary>
        public string Color3 { get; set; } = "#05C4E8";

        /// <summary>
        /// Gets or sets the format string used to display role information.
        /// </summary>
        public string RoleInfoFormat { get; set; } = "- {0} ({1}) - {2}";

        /// <summary>
        /// Gets or sets the format string used to display role health information.
        /// </summary>
        public string RoleHealthFormat { get; set; } = "- Здоровье: {0}";

        /// <summary>
        /// Gets or sets the error message displayed when the user lacks permissions.
        /// </summary>
        public string NoPermissions { get; set; } = "Не хватает прав!";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("customroles.info"))
            {
                response = NoPermissions;
                return false;
            }

            if (arguments.Count < 1)
            {
                response = Usage;
                return false;
            }

            if ((!(uint.TryParse(arguments.At(0), out uint id) && CustomRole.TryGet(id, out CustomRole? role)) && !CustomRole.TryGet(arguments.At(0), out role)) || role is null)
            {
                response = string.Format(ErrorNoRole, arguments.At(0));
                return false;
            }

            StringBuilder builder = StringBuilderPool.Pool.Get().AppendLine();

            builder.Append("<color=").Append(Color1).Append(">-</color> <color=").Append(Color2).Append(">").Append(role.Name)
                .Append("</color> <color=").Append(Color3).Append(">(").Append(role.Id).Append(")</color>")
                .Append(string.Format(RoleInfoFormat, role.Name, role.Id, role.Description))
                .AppendLine(role.Role.ToString())
                .Append(string.Format(RoleHealthFormat, role.MaxHealth.ToString())).AppendLine();

            response = StringBuilderPool.Pool.ToStringReturn(builder);
            return true;
        }
    }
}