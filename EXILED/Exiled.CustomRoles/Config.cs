// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles
{
    using System.ComponentModel;

    using API.Features;

    using Commands.User;

    using Exiled.API.Features;
    using Exiled.API.Interfaces;

    /// <summary>
    ///     The plugin's config.
    /// </summary>
    public class Config : IConfig
    {
        /// <inheritdoc />
        [Description("Whether or not the plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        ///     Gets or sets a value indicating whether debug messages should be printed to the console.
        /// </summary>
        /// <returns><see cref="bool" />.</returns>
        [Description("Whether or not debug messages should be shown.")]
        public bool Debug { get; set; } = false;

        /// <summary>
        ///     Gets or sets the hint that is shown when someone gets a <see cref="CustomRole" />.
        /// </summary>
        [Description("The hint that is shown when someone gets a custom role.")]
        public Hint GotRoleHint { get; set; } = new("You have spawned as a {0}\n{1}", 6);

        /// <summary>
        ///     Gets or sets  the hint that is shown when someone used a <see cref="CustomAbility" />.
        /// </summary>
        [Description("The hint that is shown when someone used a custom ability.")]
        public Hint UsedAbilityHint { get; set; } = new("Ability {0} has been activated.\n{1}", 5);

        /// <summary>
        ///     Gets or sets  the hint that is shown when someone used a <see cref="CustomAbility" />.
        /// </summary>
        [Description("The hint that is shown when someone's custom ability is ready.")]
        public Hint AbilityReadyHint { get; set; } = new("Ability {0} is ready.\n{1}", 5);

        /// <summary>
        ///     Gets or sets  the hint that is shown when someone used a cooldowned <see cref="CustomAbility" />.
        /// </summary>
        [Description("The hint that is shown when someone tries to use cooldowned ability. Also used in console respond. {0} - remaining cooldown, {1} - ability name")]
        public Hint AbilityOnCooldownHint { get; set; } = new("Способность на перезарядке!\nПодождите ещё {0} секунд перед использованием.", 5);

        /// <summary>
        ///     Gets or sets  the hint that is shown when someone tries to use <see cref="CustomAbility" /> without required energy.
        /// </summary>
        [Description("The hint that is shown when someone tries to use ability without required energy. Also used in console respond. {0} - current energy, {1} - required energy")]
        public Hint InsufficientEnergyHint { get; set; } = new("Недостаточно энергии!\nУ вас {0}/{1}", 5);

        /// <summary>
        ///     Gets or sets  the hint that is shown when someone tries to use <see cref="CustomAbility" /> without required level.
        /// </summary>
        [Description("The hint that is shown when someone tries to use ability without required level. Also used in console respond. {0} - current level, {1} - required level")]
        public Hint InsufficientLevelHint { get; set; } = new("Недостаточный уровень!\nУ вас {0}/{1}", 5);

        /// <summary>
        ///     Gets or sets  the hint that is shown when someone tries to use <see cref="CustomAbility" /> with too high level.
        /// </summary>
        [Description("The hint that is shown when someone tries to use ability with too high level. Also used in console respond. {0} - current level, {1} - required level")]
        public Hint RedundantLevelHint { get; set; } = new("Избыточный уровень!\nУ вас {0}/{1}", 5);

        /// <summary>
        ///     Gets or sets response of <see cref="RoleInfo" />.
        /// </summary>
        [Description("Формат ответа команды RoleInfo. 0 - Название, 1 - айди, 2 - описание, 3 - способности")]
        public string RoleInfoResponse { get; set; } = "\n<b>Ваша особая роль: <color=red>{0}</color></b>\n" +
                                                       "<b>ID: <color=red>{1}</color></b>\n" +
                                                       "{2}\n" +
                                                       "<b><color=green>{3}</b></color>\n" + // Теги будут влиять на заголовки разделов
                                                       "<color=yellow><i>Для активации способности напишите команду .ability</i></color>\n\n" +
                                                       "<b><color=red>Вы можете сделать активацию способности по клавише!</color></b>\n" +
                                                       "Для этого напишите команду cmdbind КЛАВИША .ability НОМЕР_СПОСОБНОСТИ";

        /// <summary>
        ///     Gets or sets response of <see cref="UseAbility" />.
        /// </summary>
        [Description("Формат ответа команды RoleInfo. 0 - способности")]
        public string UseAbilityResponse { get; set; } = "\n<b><color=red>Укажите номер способности.</color></b>\n" +
                                                         "<b>Список:</b>\n" +
                                                         "\n{0}\n" +
                                                         "<b><color=yellow>Вы можете сделать активацию способности по клавише!</color></b>\n" +
                                                         "<color=yellow>Для этого напишите команду cmdbind КЛАВИША .ability НОМЕР_СПОСОБНОСТИ</color>";

        /// <summary>
        ///     Gets or sets format for displaing <see cref="PassiveAbility" /> in <see cref="RoleInfo" /> command.
        /// </summary>
        [Description("Формат отображения строки способности. 0 - номер в разделе, 1 - название, 2 - описание. Следует помнить, что эта строка вставляется в раздел 3 выше")]
        public string PassiveAbilityLineFormat { get; set; } = " - <color=gray><i>#{0} {1}\n{2}</i></color>";

        /// <summary>
        ///     Gets or sets format for displaing <see cref="ActiveAbility" /> in <see cref="RoleInfo" /> and
        ///     <see cref="UseAbility" /> commands.
        /// </summary>
        [Description("Формат отображения строки способности. 0 - номер в разделе, 1 - название, 2 - описание, 3 - длительность (заменяется на 'Мгновенно', если равно нулю), 4 - КД. Следует помнить, что эта строка вставляется в раздел 3 выше")]
        public string ActiveAbilityLineFormat { get; set; } = " - <color=gray><i>#{0} {1}\n{2}</i></color>";

        /// <summary>
        ///     Gets or sets format for displaing <see cref="Scp079ActiveAbility" /> in <see cref="RoleInfo" /> and
        ///     <see cref="UseAbility" /> commands.
        /// </summary>
        [Description("Формат отображения строки способности. 0 - номер в разделе, 1 - название, 2 - описание, 3 - длительность (заменяется на 'Мгновенно', если равно нулю), 4 - КД, 5 - требуемый уровень, 6 - требуемая энергия. Следует помнить, что эта строка вставляется в раздел 3 выше")]
        public string Active079AbilityLineFormat { get; set; } = " - <color=gray><i>#{0} {1}\n{2}</i></color>";

        /// <summary>
        ///     Gets or sets ability line format.
        /// </summary>
        [Description("Формат названия раздела способностей. {0} - 'Пассивные способности:'/'Активные способности:'")]
        public string AbilityBlockFormat { get; set; } = "<b>{0}</b>";

        /// <summary>
        ///     Gets or sets a value indicating whether fuck these docs.
        /// </summary>
        [Description("Формат названия раздела способностей. {0} - 'Пассивные способности:'/'Активные способности:'")]
        public bool HideUnavailableHighLevelAbilities { get; set; } = false;

        /// <summary>
        ///     Gets or sets customroles nickname display to spectators.
        /// </summary>
        [Description("Задержка до синхронизации имён кастомных ролей и наблюдателей.")]
        public float CustomRolesSpectatorDisplayDelay { get; set; } = 2;
    }
}