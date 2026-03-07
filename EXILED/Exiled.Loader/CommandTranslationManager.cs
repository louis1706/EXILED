// -----------------------------------------------------------------------
// <copyright file="CommandTranslationManager.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using YamlDotNet.Core;

    /// <summary>
    /// Used to handle plugin commands translations.
    /// </summary>
    public static class CommandTranslationManager
    {
        /// <summary>
        /// Default value for translated command.
        /// </summary>
        public const string DefaultValue = "default";

        /// <summary>
        /// Loads all of the plugin's translations.
        /// </summary>
        /// <param name="rawTranslations">The raw translations to be loaded.</param>
        /// <returns>Returns a dictionary of loaded translations.</returns>
        public static SortedDictionary<string, CommandTranslation> Load(string rawTranslations)
        {
            try
            {
                Log.Info($"Loading commands translations...");

                Dictionary<string, CommandTranslation> rawDeserializedTranslations = Loader.Deserializer.Deserialize<Dictionary<string, CommandTranslation>>(rawTranslations) ?? new Dictionary<string, CommandTranslation>();
                SortedDictionary<string, CommandTranslation> deserializedTranslations = new(StringComparer.Ordinal);
                void LoadCommand(ICommand command)
                {
                    if (command == null)
                        return;

                    string commandName = command.GetType().FullName!;

                    if (command is CommandHandler parentCommand)
                    {
                        foreach (ICommand childCommand in parentCommand.AllCommands)
                            LoadCommand(childCommand);
                    }

                    CommandTranslation translation = command.LoadCommandTranslation(rawDeserializedTranslations);
                    if (deserializedTranslations.ContainsKey(commandName))
                        return; // Так происходит, если команда представлена в нескольких инстанциях.

                    deserializedTranslations.Add(commandName, translation);
                }

                foreach (ICommand command in Loader.Plugins.SelectMany(x => x.Commands.Select(y => y.Value.Item1)))
                {
                    try
                    {
                        LoadCommand(command);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"An error has occurred while loading command translation! \n {e}");
                    }
                }

                Log.Info("Plugin command translations loaded successfully!");

                DictionaryPool<string, CommandTranslation>.Pool.Return(rawDeserializedTranslations);
                return deserializedTranslations;
            }
            catch (Exception exception)
            {
                Log.Error($"An error has occurred while loading commands translations!\n{exception}");

                return null;
            }
        }

        /// <summary>
        /// Loads the translations of a plugin based on the actual distribution.
        /// </summary>
        /// <param name="command">Command to load translations into.</param>
        /// <param name="rawTranslations">The raw translations to check whether or not the plugin already has a translation config.</param>
        /// <returns>The <see cref="CommandTranslation"/> of the desired command.</returns>
        public static CommandTranslation LoadCommandTranslation(this ICommand command, Dictionary<string, CommandTranslation> rawTranslations)
        {
            rawTranslations ??= Loader.Deserializer.Deserialize<Dictionary<string, CommandTranslation>>(Read()) ??
                                DictionaryPool<string, CommandTranslation>.Pool.Get();

            Type commandType = command.GetType();
            string commandName = commandType.FullName ?? "N/D";
            List<PropertyInfo> props = commandType.GetProperties().Where(x => x.CanWrite && x.PropertyType == typeof(string)).ToList();

            if (!rawTranslations.TryGetValue(commandName, out CommandTranslation translation))
            {
                Log.Warn($"Command {commandName} doesn't have default translations, generating...");
                translation = new CommandTranslation()
                {
                    Properties = props.ToDictionary(x => x.Name, _ => DefaultValue),
                };
            }
            else
            {
                Dictionary<string, string> translationProperties = translation.Properties;
                foreach (PropertyInfo propertyInfo in props)
                {
                    if (translationProperties.TryGetValue(propertyInfo.Name, out string configValue))
                    {
                        if (configValue == DefaultValue)
                        {
                            if (LoaderPlugin.Config.PrintFullCommandProps)
                                translationProperties[propertyInfo.Name] = propertyInfo.GetValue(command) as string;
                            continue;
                        }

                        propertyInfo.SetValue(command, configValue);
                    }
                    else
                    {
                        translationProperties[propertyInfo.Name] = LoaderPlugin.Config.PrintFullCommandProps ? propertyInfo.GetValue(command) as string : DefaultValue;
                    }
                }

                List<string> validPropertyNames = props.Select(x => x.Name).ToList();
                foreach (string configuredPropsKey in translationProperties.Keys.Where(x => !validPropertyNames.Contains(x)).ToList())
                    translationProperties.Remove(configuredPropsKey);
            }

            return translation;
        }

        /// <summary>
        /// Reads, loads, and saves plugin translations.
        /// </summary>
        /// <returns>Returns a value indicating if the reloading process has been completed successfully or not.</returns>
        public static bool Reload() => Save(Load(Read()));

        /// <summary>
        /// Saves default distribution translations.
        /// </summary>
        /// <param name="translations">The translations to be saved, already serialized in yaml format.</param>
        /// <returns>Returns a value indicating whether the translations have been saved successfully or not.</returns>
        public static bool SaveDefaultCommandTranslation(string translations)
        {
            try
            {
                File.WriteAllText(Paths.CommandTranslations, translations ?? string.Empty);

                return true;
            }
            catch (Exception exception)
            {
                Log.Error($"An error has occurred while saving translations to {Paths.CommandTranslations} path:\n{exception}");

                return false;
            }
        }

        /// <summary>
        /// Saves plugin translations.
        /// </summary>
        /// <param name="translations">The translations to be saved.</param>
        /// <returns>Returns a value indicating whether the translations have been saved successfully or not.</returns>
        public static bool Save(SortedDictionary<string, CommandTranslation> translations)
        {
            try
            {
                if (translations is null || translations.Count == 0)
                    return false;

                return SaveDefaultCommandTranslation(Loader.Serializer.Serialize(translations));
            }
            catch (YamlException yamlException)
            {
                Log.Error($"An error has occurred while serializing translations:\n{yamlException}");

                return false;
            }
        }

        /// <summary>
        /// Read all plugin translations.
        /// </summary>
        /// <returns>Returns the read translations.</returns>
        public static string Read()
        {
            try
            {
                if (File.Exists(Paths.CommandTranslations))
                    return File.ReadAllText(Paths.CommandTranslations);
            }
            catch (Exception exception)
            {
                Log.Error($"An error has occurred while reading translations from {Paths.CommandTranslations} path:\n{exception}");
            }

            return string.Empty;
        }

        /// <summary>
        /// Has all information about translated command.
        /// </summary>
        public class CommandTranslation
        {
            /// <summary>
            /// Gets or sets dictionary of property names and their translations.
            /// </summary>
            public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
        }
    }
}
