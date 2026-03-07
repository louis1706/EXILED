// -----------------------------------------------------------------------
// <copyright file="ParentCommand.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using CommandSystem;
    using NorthwoodLib.Pools;

    /// <summary>
    /// Представляет удобный класс для создания родительских команд.
    /// </summary>
    public abstract class ParentCommand : CommandHandler, ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParentCommand"/> class.
        /// </summary>
        protected ParentCommand()
        {
            LoadGeneratedCommands();
        }

        /// <inheritdoc/>
        public abstract string Command { get; }

        /// <inheritdoc/>
        public abstract string[] Aliases { get; set; }

        /// <inheritdoc/>
        public abstract string Description { get; set; }

        /// <inheritdoc/>
        public override sealed void LoadGeneratedCommands()
        {
            foreach (Type commandType in CommandsToRegister())
            {
                if (commandType.GetInterface(nameof(ICommand)) != typeof(ICommand))
                {
                    Log.Error($"Invalid command type provided for parent command {Command}: {commandType.FullName}");
                    continue;
                }

                ICommand command = (ICommand)Activator.CreateInstance(commandType);
                RegisterCommand(command);
            }
        }

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            return arguments.Count != 0 && TryGetCommand(arguments.Array![arguments.Offset], out ICommand command)
                ? command.Execute(new ArraySegment<string>(arguments.Array, arguments.Offset + 1, arguments.Count - 1), sender, out response)
                : ExecuteParent(arguments, sender, out response);
        }

        /// <summary>
        /// Gets HashSet of subcommands to register.
        /// </summary>
        /// <returns>IEnumerable of commands to register.</returns>
        protected abstract IEnumerable<Type> CommandsToRegister();

        /// <summary>
        /// Executes parent comand without subcommands.
        /// </summary>
        /// <param name="arguments">Passed args.</param>
        /// <param name="sender">Command sender.</param>
        /// <param name="response">Response for user.</param>
        /// <returns>Was command executed succesfully or not.</returns>
        protected virtual bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            StringBuilder message = StringBuilderPool.Shared.Rent($"{Command}:\n");
            foreach (ICommand command in AllCommands)
                message.AppendFormat("- {0}\n<i>{1}</i>\n\n", command.Command, command.Description);

            response = StringBuilderPool.Shared.ToStringReturn(message);
            return false;
        }
    }
}
