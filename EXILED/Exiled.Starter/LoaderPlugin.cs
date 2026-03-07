// -----------------------------------------------------------------------
// <copyright file="LoaderPlugin.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Starter
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using PluginAPI.Core.Attributes;

    /// <summary>
    /// The Northwood PluginAPI Plugin class for the EXILED Loader.
    /// </summary>
    public class LoaderPlugin
    {
#pragma warning disable SA1401
        /// <summary>
        /// The config for the EXILED Loader.
        /// </summary>
        [PluginConfig]
        public static Config Config;
#pragma warning restore SA1401

        /// <summary>
        /// Called by PluginAPI when the plugin is enabled.
        /// </summary>
        [PluginEntryPoint("Exiled Loader", null, "Loads the EXILED Plugin Framework.", "Exiled-Official")]
        [PluginPriority(byte.MinValue)]
        public void Enable()
        {
            if (Config == null)
            {
                Error("Detected null config, EXILED will not be loaded.");
                return;
            }

            if (!Config.IsEnabled)
            {
                Info("EXILED is disabled on this server via config.");
                return;
            }

            Info($"Loading EXILED Version: {Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}");
            if (!Config.ShouldLoadOutdatedExiled &&
                !GameCore.Version.CompatibilityCheck(
                    (byte)AutoUpdateFiles.RequiredSCPSLVersion.Major,
                    (byte)AutoUpdateFiles.RequiredSCPSLVersion.Minor,
                    (byte)AutoUpdateFiles.RequiredSCPSLVersion.Revision,
                    GameCore.Version.Major,
                    GameCore.Version.Minor,
                    GameCore.Version.Revision,
                    GameCore.Version.BackwardCompatibility,
                    GameCore.Version.BackwardRevision))
            {
                ServerConsole.AddLog($"Exiled is outdated, please update to the latest version. Wait for release if still shows after update.\nSCP:SL: {GameCore.Version.VersionString} Exiled Supported Version: {AutoUpdateFiles.RequiredSCPSLVersion}", ConsoleColor.DarkRed);

                // return;
            }

            Paths.Reload(Config.ExiledDirectoryPath);

            Info($"Exiled root path set to: {Paths.Exiled}");

            Directory.CreateDirectory(Paths.Exiled);
            Directory.CreateDirectory(Paths.Configs);
            Directory.CreateDirectory(Paths.Plugins);
            Directory.CreateDirectory(Paths.Dependencies);

            string exiledApiPath = Path.Combine(Paths.Dependencies, "Exiled.API.dll");
            string exiledLoaderPath = Path.Combine(Paths.Exiled, "Exiled.Loader.dll");

            if (!File.Exists(exiledApiPath))
            {
                Error($"Exiled.API.dll was not found at {exiledApiPath}, Exiled won't be loaded!");
                return;
            }

            if (!File.Exists(exiledLoaderPath))
            {
                Error($"Exiled.Loader.dll was not found at {exiledLoaderPath}, Exiled won't be loaded!");
                return;
            }

            Assembly exiledApi = Assembly.Load(File.ReadAllBytes(exiledApiPath));
            Assembly exiledLoader = Assembly.Load(File.ReadAllBytes(exiledLoaderPath));

            Type loaderType = exiledLoader.GetType("Exiled.Loader.Loader");
            object loader = Activator.CreateInstance(loaderType);
            loader.GetType().GetMethod("Run").Invoke(loader, new[] { new Assembly[] { exiledApi } });
        }

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Info"/> level messages to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        private static void Info(string message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", Discord.LogLevel.Info, ConsoleColor.Cyan);

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Error"/> level messages to the game console.
        /// This should be used to send errors only.
        /// It's recommended to send any messages in the catch block of a try/catch as errors with the exception string.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        private static void Error(string message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", Discord.LogLevel.Error, ConsoleColor.DarkRed);

        /// <summary>
        /// Sends a log message to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="level">The message level of importance.</param>
        /// <param name="color">The message color.</param>
        private static void Send(string message, Discord.LogLevel level, ConsoleColor color = ConsoleColor.Gray)
        {
            SendRaw($"[{level.ToString().ToUpper()}] {message}", color);
        }

        /// <summary>
        /// Sends a raw log message to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="color">The <see cref="ConsoleColor"/> of the message.</param>
        private static void SendRaw(string message, ConsoleColor color) => ServerConsole.AddLog(message, color);

        /// <summary>
        /// A set of useful paths.
        /// </summary>
        public class Paths
        {
            static Paths() => Reload();

            /// <summary>
            /// Gets AppData path.
            /// </summary>
            public static string AppData { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            /// <summary>
            /// Gets or sets exiled directory path.
            /// </summary>
            public static string Exiled { get; set; }

            /// <summary>
            /// Gets or sets plugins path.
            /// </summary>
            public static string Plugins { get; set; }

            /// <summary>
            /// Gets or sets Dependencies directory path.
            /// </summary>
            public static string Dependencies { get; set; }

            /// <summary>
            /// Gets or sets the configuration folder path.
            /// </summary>
            public static string Configs { get; set; }

            /// <summary>
            /// Gets or sets individual configs directory path.
            /// </summary>
            public static string IndividualConfigs { get; set; }

            /// <summary>
            /// Gets or sets the loader configuration file path.
            /// </summary>
            public static string LoaderConfig { get; set; }

            /// <summary>
            /// Gets or sets individual translations directory path.
            /// </summary>
            public static string IndividualTranslations { get; set; }

            /// <summary>
            /// Reloads all paths.
            /// </summary>
            /// <param name="rootDirectory">The new root directory.</param>
            public static void Reload(string rootDirectory = null)
            {
                rootDirectory ??= Path.Combine(AppData, "EXILED");

                Exiled = rootDirectory;
                Plugins = Path.Combine(Exiled, "Plugins");
                Dependencies = Path.Combine(Plugins, "dependencies");
                Configs = Path.Combine(Exiled, "Configs");
                IndividualConfigs = Path.Combine(Configs, "Plugins");
                LoaderConfig = PluginAPI.Loader.AssemblyLoader.InstalledPlugins.FirstOrDefault(x => x.PluginName == "Exiled Loader")?.MainConfigPath;
                IndividualTranslations = Path.Combine(Configs, "Translations");
            }
        }
    }
}
