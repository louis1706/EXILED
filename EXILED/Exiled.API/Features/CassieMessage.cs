// -----------------------------------------------------------------------
// <copyright file="CassieMessage.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Useful class to save cassie message configs in a cleaner way.
    /// </summary>
    public class CassieMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CassieMessage"/> class.
        /// </summary>
        public CassieMessage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CassieMessage"/> class.
        /// </summary>
        /// <param name="message">The cassie message.</param>
        /// <param name="subtitles">The cassie subtitles.</param>
        /// <param name="isHeld">Indicates whether C.A.S.S.I.E has to hold the message.</param>
        /// <param name="isNoisy">Indicates whether C.A.S.S.I.E has to make noises or not during the message.</param>
        /// <param name="isSubtitles">Indicates whether C.A.S.S.I.E has to make subtitles.</param>
        public CassieMessage(string message, string subtitles, bool isHeld = false, bool isNoisy = true, bool isSubtitles = true)
        {
            Message = message;
            Subtitles = subtitles;
            IsHeld = isHeld;
            IsNoisy = isNoisy;
            IsSubtitles = isSubtitles;
        }

        /// <summary>
        /// Gets or sets the cassie message.
        /// </summary>
        [Description("The cassie message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the cassie subtitles.
        /// </summary>
        [Description("The cassie subtitles")]
        public string Subtitles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether C.A.S.S.I.E has to hold the message.
        /// </summary>
        [Description("Indicates whether C.A.S.S.I.E has to hold the message")]
        public bool IsHeld { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether C.A.S.S.I.E has to make noises or not during the message.
        /// </summary>
        [Description("Indicates whether C.A.S.S.I.E has to make noises or not during the message")]
        public bool IsNoisy { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether C.A.S.S.I.E has to make subtitles.
        /// </summary>
        [Description("Indicates whether C.A.S.S.I.E has to make subtitles")]
        public bool IsSubtitles { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the cassie should be sended or not.
        /// </summary>
        [Description("Indicates whether the cassie should be sended or not")]
        public bool Show { get; set; }
    }
}
