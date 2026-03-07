// -----------------------------------------------------------------------
// <copyright file="ButtonSetting.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.UserSettings
{
    using System;
    using System.Diagnostics;

    using Exiled.API.Interfaces;
    using global::UserSettings.ServerSpecific;

    /// <summary>
    /// Represents a button setting.
    /// </summary>
    public class ButtonSetting : SettingBase, IWrapper<SSButton>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonSetting"/> class.
        /// </summary>
        /// <param name="id"><inheritdoc cref="SettingBase.Id"/></param>
        /// <param name="label"><inheritdoc cref="SettingBase.Label"/></param>
        /// <param name="buttonText"><inheritdoc cref="Text"/></param>
        /// <param name="holdTime"><inheritdoc cref="HoldTime"/></param>
        /// <param name="hintDescription"><inheritdoc cref="SettingBase.HintDescription"/></param>
        /// <param name="header"><inheritdoc cref="SettingBase.Header"/></param>
        /// <param name="onChanged"><inheritdoc cref="SettingBase.OnChanged"/></param>
        public ButtonSetting(int id, string label, string buttonText, float holdTime = 0.0f, string hintDescription = null, HeaderSetting header = null, Action<Player, SettingBase> onChanged = null)
            : base(new SSButton(id, label, buttonText, holdTime, hintDescription), header, onChanged)
        {
            Base = (SSButton)base.Base;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonSetting"/> class.
        /// </summary>
        /// <param name="settingBase">A <see cref="SSButton"/> instance.</param>
        internal ButtonSetting(SSButton settingBase)
            : base(settingBase)
        {
            Base = settingBase;

            if (OriginalDefinition != null && OriginalDefinition.Is(out ButtonSetting setting))
            {
                Text = setting.Text;
                HoldTime = setting.HoldTime;
            }
        }

        /// <inheritdoc/>
        public new SSButton Base { get; }

        /// <summary>
        /// Gets the last press time.
        /// </summary>
        public Stopwatch LastPress => Base.SyncLastPress;

        /// <summary>
        /// Gets or sets the button text.
        /// </summary>
        public string Text
        {
            get => Base.ButtonText;
            set => Base.ButtonText = value;
        }

        /// <summary>
        /// Gets or sets the hold time in seconds.
        /// </summary>
        public float HoldTime
        {
            get => Base.HoldTimeSeconds;
            set => Base.HoldTimeSeconds = value;
        }

        /// <summary>
        /// Sends updated values to clients.
        /// </summary>
        /// <param name="text"><inheritdoc cref="Text"/></param>
        /// <param name="holdTime"><inheritdoc cref="HoldTime"/></param>
        /// <param name="overrideValue">If false, sends fake values.</param>
        /// <param name="filter">Who to send the update to.</param>
        public void UpdateSetting(string text, float holdTime, bool overrideValue = true, Predicate<Player> filter = null)
        {
            filter ??= _ => true;
            Base.SendButtonUpdate(text, holdTime, overrideValue, hub => filter(Player.Get(hub)));
        }

        /// <summary>
        /// Returns a representation of this <see cref="ButtonSetting"/>.
        /// </summary>
        /// <returns>A string in human-readable format.</returns>
        public override string ToString()
        {
            return base.ToString() + $" ={Text}= -{HoldTime}- /{LastPress}/";
        }

        /// <summary>
        /// Represents a config for ButtonSetting.
        /// </summary>
        public class ButtonConfig : SettingConfig<ButtonSetting>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ButtonConfig"/> class.
            /// </summary>
            /// <param name="label"/><inheritdoc cref="Label"/>
            /// <param name="buttonText"><inheritdoc cref="ButtonText"/></param>
            /// <param name="headerName"><inheritdoc cref="HeaderName"/></param>
            /// <param name="holdTime"><inheritdoc cref="HoldTime"/></param>
            /// <param name="hintDescription"><inheritdoc cref="HintDescription"/></param>
            /// <param name="headerDescription"><inheritdoc cref="HeaderDescription"/></param>
            /// <param name="headerPaddling"><inheritdoc cref="HeaderPaddling"/></param>
            public ButtonConfig(string label, string buttonText, string headerName = null, float holdTime = 0.0f, string hintDescription = null, string headerDescription = null, bool headerPaddling = false)
            {
                Label = label;
                ButtonText = buttonText;
                HoldTime = holdTime;
                HintDescription = hintDescription;
                HeaderName = headerName;
                HeaderDescription = headerDescription;
                HeaderPaddling = headerPaddling;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ButtonConfig"/> class.
            /// </summary>
            public ButtonConfig()
            {
            }

            /// <summary>
            /// Gets or sets label of a ButtonConfig.
            /// </summary>
            public string Label { get; set; }

            /// <summary>
            /// Gets or sets ButtonText of a ButtonConfig.
            /// </summary>
            public string ButtonText { get; set; }

            /// <summary>
            /// Gets or sets HoldTime of a ButtonConfig.
            /// </summary>
            public float HoldTime { get; set; }

            /// <summary>
            /// Gets or sets HintDescription of a ButtonConfig.
            /// </summary>
            public string HintDescription { get; set; }

            /// <summary>
            /// Gets or sets HeaderName of a ButtonConfig.
            /// </summary>
            public string HeaderName { get; set; }

            /// <summary>
            /// Gets or sets HeaderDescription of a ButtonConfig.
            /// </summary>
            public string HeaderDescription { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether HeaderPaddling is needed.
            /// </summary>
            public bool HeaderPaddling { get; set; }

            /// <summary>
            /// Creates a ButtonSetting instanse.
            /// </summary>
            /// <returns>ButtonSetting.</returns>
            public override ButtonSetting Create() => new(++IdIncrementor, Label, ButtonText, HoldTime, HintDescription, HeaderName == null ? null : new HeaderSetting(HeaderName, HeaderDescription, HeaderPaddling));
        }
    }
}