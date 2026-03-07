// -----------------------------------------------------------------------
// <copyright file="CustomWeapon.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using CustomPlayerEffects;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.DamageHandlers;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Structs;
    using Exiled.Events.EventArgs.Item;
    using Exiled.Events.EventArgs.Player;
    using InventorySystem.Items.Firearms.Attachments;
    using InventorySystem.Items.Firearms.Attachments.Components;
    using InventorySystem.Items.Firearms.BasicMessages;
    using MEC;
    using PlayerRoles;
    using UnityEngine;

    /// <summary>
    ///     The Custom Weapon base class.
    /// </summary>
    public abstract class CustomWeapon : CustomItem
    {
        private readonly HashSet<Player> cooldownedPlayers = new();

        /// <inheritdoc />
        public override ItemType Type
        {
            get => base.Type;
            set
            {
                if (!value.IsWeapon(false) && value != ItemType.None)
                    throw new ArgumentOutOfRangeException($"{nameof(Type)}", value, "Invalid weapon type.");

                base.Type = value;
            }
        }

        /// <summary>
        /// Gets or sets value indicating what <see cref="Attachment" />s the weapon will have.
        /// </summary>
        public virtual AttachmentName[] Attachments { get; set; } = { };

        /// <summary>
        /// Gets or sets value indicating what <see cref="AttachmentName" />s the weapon wont have.
        /// </summary>
        public virtual AttachmentName[] BannedAttachments { get; set; } = { };

        /// <summary>
        ///     Gets or sets the weapon damage.
        /// </summary>
        public abstract float Damage { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating how big of a clip the weapon will have.
        /// </summary>
        public virtual byte ClipSize { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating how many ammo will be spent per shot.
        /// </summary>
        public virtual byte AmmoUsage { get; set; } = 1;

        /// <summary>
        ///     Gets or sets a value indicating whether firearm can be unloaded.
        /// </summary>
        public virtual bool CanUnload { get; set; } = true;

        /// <summary>
        ///     Gets or sets a value indicating whether firearm's attachments can be modified.
        /// </summary>
        public bool AllowAttachmentsChange { get; set; } = true;

        /// <summary>
        ///     Gets or sets a value indicating shot cooldown.
        /// </summary>
        [Description("Кулдаун на выстрелы. Работает только при ClipSize > 1 и FireCooldown > 0. -1 для отключения.")]
        public float FireCooldown { get; set; } = -1;

        /// <summary>
        ///     Gets or sets a value indicating message, displayed to players, trying to reload cooldowned weapon.
        /// </summary>
        [Description("Сообщение при попытке перезарядить оружие под кулдауном. {0} - кулдаун из конфига")]
        public string WeaponNotReady { get; set; } = "Оружие ещё не готово к выстрелу! Оно может стрелять только раз в {0} секунд.";

        /// <summary>
        ///     Gets or sets a value indicating damage multipliers by ArmorType and HitboxType.
        /// </summary>
        [Description("Множители урона в зависимости от брони и точки попадания. Словарь ТипБрони: (ЗонаПопадания: МножительУрона)")]
        public Dictionary<ItemType, Dictionary<HitboxType, float>> ArmorAndZoneDamageMultipliers { get; set; } = new()
        {
            [ItemType.None] = new Dictionary<HitboxType, float>
            {
                [HitboxType.Headshot] = 1,
                [HitboxType.Limb] = 1,
                [HitboxType.Body] = 1,
            },
            [ItemType.ArmorLight] = new Dictionary<HitboxType, float>
            {
                [HitboxType.Headshot] = 1,
            },
            [ItemType.ArmorCombat] = new Dictionary<HitboxType, float>
            {
                [HitboxType.Headshot] = 1,
            },
            [ItemType.ArmorHeavy] = new Dictionary<HitboxType, float>
            {
                [HitboxType.Headshot] = 1,
            },
        };

        /// <summary>
        ///     Gets or sets a value indicating  damage multipliers by target RoleTypeId.
        /// </summary>
        [Description("Множители урона для ролей. Словарь RoleType: МножительУрона")]
        public Dictionary<RoleTypeId, float> RoleDamageMultipliers { get; set; } = new()
        {
            { RoleTypeId.Scp096, 1 },
            { RoleTypeId.Scp173, 1 },
        };

        /// <summary>
        /// Gets or sets a value indicating whether firearm's will be reset after shot.
        /// </summary>
        [Description("Будет ли оружие убрано-возвращено в руки после выстрела")]
        public bool ForceResetWeaponOnShot { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether firearm's will be double shot.
        /// </summary>
        [Description("При использовании КД выстрелов, разрешать ли двойной")]
        public bool AllowDoubleShot { get; set; } = false;

        /// <inheritdoc />
        public override Item CreateItem()
        {
            Item item = base.CreateItem();

            if (item is Firearm firearm)
            {
                if (!Attachments.IsEmpty())
                    firearm.AddAttachment(Attachments);

                firearm.MagazineAmmo = firearm.MaxMagazineAmmo = ClipSize;
                firearm.AmmoDrain = AmmoUsage;
            }

            return item;
        }

        /// <inheritdoc />
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ReloadingWeapon += OnInternalReloading;
            Exiled.Events.Handlers.Player.Shooting += OnInternalShooting;
            Exiled.Events.Handlers.Player.Shot += OnInternalShot;
            Exiled.Events.Handlers.Player.Hurting += OnInternalHurting;
            Exiled.Events.Handlers.Player.UnloadingWeapon += OnInternalUnloading;
            Exiled.Events.Handlers.Item.ChangingAttachments += OnInternalChangingAttachments;

            base.SubscribeEvents();
        }

        /// <inheritdoc />
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ReloadingWeapon -= OnInternalReloading;
            Exiled.Events.Handlers.Player.Shooting -= OnInternalShooting;
            Exiled.Events.Handlers.Player.Shot -= OnInternalShot;
            Exiled.Events.Handlers.Player.Hurting -= OnInternalHurting;
            Exiled.Events.Handlers.Player.UnloadingWeapon -= OnInternalUnloading;
            Exiled.Events.Handlers.Item.ChangingAttachments -= OnInternalChangingAttachments;

            base.UnsubscribeEvents();
        }

        /// <summary>
        ///     Handles reloading for custom weapons.
        /// </summary>
        /// <param name="ev"><see cref="ReloadingWeaponEventArgs" />.</param>
        protected virtual void OnReloading(ReloadingWeaponEventArgs ev)
        {
        }

        /// <summary>
        /// Handles reloaded for custom weapons.
        /// </summary>
        /// <param name="ev"><see cref="ReloadedWeaponEventArgs"/>.</param>
        protected virtual void OnReloaded(ReloadedWeaponEventArgs ev)
        {
        }

        /// <summary>
        /// Handles shooting for custom weapons.
        /// </summary>
        /// <param name="ev"><see cref="ShootingEventArgs" />.</param>
        protected virtual void OnShooting(ShootingEventArgs ev)
        {
        }

        /// <summary>
        ///     Handles shot for custom weapons.
        /// </summary>
        /// <param name="ev"><see cref="ShotEventArgs" />.</param>
        protected virtual void OnShot(ShotEventArgs ev)
        {
        }

        /// <summary>
        ///     Handles hurting for custom weapons.
        /// </summary>
        /// <param name="ev"><see cref="HurtingEventArgs" />.</param>
        protected virtual void OnHurting(HurtingEventArgs ev)
        {
        }

        /// <summary>
        ///     Handles unloading for custom weapons.
        /// </summary>
        /// <param name="ev"><see cref="HurtingEventArgs" />.</param>
        protected virtual void OnUnloading(UnloadingWeaponEventArgs ev)
        {
        }

        private void OnInternalChangingAttachments(ChangingAttachmentsEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            IEnumerable<AttachmentIdentifier> newAttachments = ev.NewAttachmentIdentifiers.Except(ev.CurrentAttachmentIdentifiers);
            if (!AllowAttachmentsChange || newAttachments.Any(x => BannedAttachments.Contains(x.Name)))
                ev.IsAllowed = false;
        }

        private void OnInternalReloading(ReloadingWeaponEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            if (cooldownedPlayers.Contains(ev.Player))
            {
                ev.IsAllowed = false;
                ev.Player.ShowHint(string.Format(WeaponNotReady, FireCooldown));
                return;
            }

            Log.Debug($"{nameof(Name)}.{nameof(OnInternalReloading)}: Reloading weapon. Calling external reload event..");
            OnReloading(ev);

            Log.Debug($"{nameof(Name)}.{nameof(OnInternalReloading)}: External event ended. {ev.IsAllowed}");
        }

        private void OnInternalShooting(ShootingEventArgs ev)
        {
            if (!Check(ev.Player))
                return;

            if (cooldownedPlayers.Contains(ev.Player))
            {
                ev.IsAllowed = false;
                ev.Player.ShowHint(string.Format(WeaponNotReady, FireCooldown));
                Log.Debug($"Disallowed shot from cooldowned on {Name} player {ev.Player.Nickname}");
                return;
            }

            if (!AllowDoubleShot)
                cooldownedPlayers.Add(ev.Player);

            Timing.CallDelayed(FireCooldown, () =>
            {
                cooldownedPlayers.Remove(ev.Player);
                Log.Debug($"Cooldown of {Name} removed from player {ev.Player.Nickname}");
            });

            OnShooting(ev);
        }

        private void OnInternalShot(ShotEventArgs ev)
        {
            Item curItem = ev.Player.CurrentItem;
            if (!Check(curItem))
                return;

            OnShot(ev);
            if (ForceResetWeaponOnShot)
                Timing.RunCoroutine(ResetWeapon(ev.Player));
        }

        private IEnumerator<float> ResetWeapon(Player player)
        {
            Item curItem = player.CurrentItem;
            yield return Timing.WaitForSeconds(0.01f);
            player.CurrentItem = null;
            yield return Timing.WaitForSeconds(0.08f);
            player.CurrentItem = curItem;
        }

        private void OnInternalHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker is null || ev.Player is null || ev.Attacker == ev.Player || !Check(ev.Attacker.CurrentItem) || ev.DamageHandler == null)
                return;

            if (!ev.DamageHandler.CustomBase.BaseIs(out FirearmDamageHandler firearmDamageHandler))
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: Handler not firearm");
                return;
            }

            if (!Check(firearmDamageHandler.Item))
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: type != type");
                return;
            }

            ev.Amount = Damage;
            if (ev.Player.IsHuman && ArmorAndZoneDamageMultipliers.TryGetValue(ev.Player.CurrentArmor?.Type ?? ItemType.None, out Dictionary<HitboxType, float> dic) &&
                dic.TryGetValue(firearmDamageHandler.Hitbox, out float multiplier))
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: Found damage muptiplier for armor/hitbox {multiplier}");
                ev.Amount *= multiplier;
            }

            if (RoleDamageMultipliers.TryGetValue(ev.Player.Role.Type, out multiplier))
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: Found damage muptiplier for target role: {multiplier}");
                ev.Amount *= multiplier;
            }

            OnHurting(ev);
        }

        private void OnInternalUnloading(UnloadingWeaponEventArgs ev)
        {
            if (!Check(ev.Firearm))
                return;

            ev.IsAllowed = CanUnload;

            OnUnloading(ev);
        }
    }
}
