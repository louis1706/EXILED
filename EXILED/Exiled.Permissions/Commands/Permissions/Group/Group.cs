// -----------------------------------------------------------------------
// <copyright file="Group.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Permissions.Commands.Permissions.Group
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using API.Features;
    using CommandSystem;

    using Exiled.API.Features.Pools;

    using Extensions;

    /// <summary>
    /// Handles commands about permissions groups.
    /// </summary>
    public class Group : ParentCommand
    {
        /// <inheritdoc/>
        public override string Command { get; } = "groups";

        /// <inheritdoc/>
        public override string[] Aliases { get; set; } = new[] { "grps" };

        /// <inheritdoc/>
        public override string Description { get; set; } = "Handles commands about permissions groups.";

        /// <inheritdoc/>
        protected override IEnumerable<Type> CommandsToRegister()
        {
            yield return typeof(Add);
            yield return typeof(Remove);
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("ep.groupinfo"))
            {
                response = "You can't see group information, you don't have \"ep.groupinfo\" permission.";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "- EP GROUPS <NAME>";
                return false;
            }

            if (!Permissions.Groups.ContainsKey(arguments.At(0)))
            {
                response = $"Group {arguments.At(0)} does not exist.";
                return false;
            }

            Permissions.Groups.TryGetValue(arguments.At(0), out Features.Group group);

            StringBuilder stringBuilder = StringBuilderPool.Pool.Get();

            stringBuilder.AppendLine($"Group: {arguments.At(0)}");

            if (group is null)
            {
                stringBuilder.AppendLine($"Group is null.");
                response = stringBuilder.ToString();
                return false;
            }

            stringBuilder.AppendLine($"Default: {group.IsDefault}");

            if (group.Inheritance.Count != 0)
            {
                stringBuilder.AppendLine("Inheritance: ");

                foreach (string inheritance in group.Inheritance)
                    stringBuilder.AppendLine("- " + inheritance);
            }

            if (group.Inheritance.Count != 0)
            {
                stringBuilder.AppendLine("Permissions: ");

                foreach (string permission in group.Permissions)
                    stringBuilder.AppendLine($"- {permission}");
            }

            response = StringBuilderPool.Pool.ToStringReturn(stringBuilder);
            return true;
        }
    }
}