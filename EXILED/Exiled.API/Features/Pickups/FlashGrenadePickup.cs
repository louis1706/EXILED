// -----------------------------------------------------------------------
// <copyright file="FlashGrenadePickup.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using Enums;
    using InventorySystem.Items;
    using InventorySystem.Items.ThrowableProjectiles;
    using Items;
    using Projectiles;

    /// <summary>
    /// A wrapper class for dropped Flashbang Pickup.
    /// </summary>
    internal class FlashGrenadePickup : GrenadePickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlashGrenadePickup"/> class.
        /// </summary>
        /// <param name="pickupBase">.</param>
        internal FlashGrenadePickup(TimedGrenadePickup pickupBase)
            : base(pickupBase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlashGrenadePickup"/> class.
        /// </summary>
        internal FlashGrenadePickup()
            : base(ItemType.GrenadeFlash)
        {
        }

        /// <summary>
        /// Gets or sets the minimum duration of player can take the effect.
        /// </summary>
        public float MinimalDurationEffect { get; set; }

        /// <summary>
        /// Gets or sets the additional duration of the <see cref="EffectType.Blinded"/> effect.
        /// </summary>
        public float AdditionalBlindedEffect { get; set; }

        /// <summary>
        /// Gets or sets the how mush the flash grenade going to be intensified when explode at <see cref="RoomType.Surface"/>.
        /// </summary>
        public float SurfaceDistanceIntensifier { get; set; }

        /// <inheritdoc/>
        internal override void ReadItemInfo(Item item)
        {
            base.ReadItemInfo(item);
            if (item is FlashGrenade flashGrenadeitem)
            {
                MinimalDurationEffect = flashGrenadeitem.MinimalDurationEffect;
                AdditionalBlindedEffect = flashGrenadeitem.AdditionalBlindedEffect;
                SurfaceDistanceIntensifier = flashGrenadeitem.SurfaceDistanceIntensifier;
                FuseTime = flashGrenadeitem.FuseTime;
            }
        }

        /// <inheritdoc/>
        internal override void WriteProjectileInfo(Projectile projectile)
        {
            if (projectile is FlashbangProjectile flashbangProjectile)
            {
                flashbangProjectile.MinimalDurationEffect = MinimalDurationEffect;
                flashbangProjectile.AdditionalBlindedEffect = AdditionalBlindedEffect;
                flashbangProjectile.SurfaceDistanceIntensifier = SurfaceDistanceIntensifier;
                flashbangProjectile.FuseTime = FuseTime;
            }
        }

        /// <inheritdoc/>
        protected override void InitializeProperties(ItemBase itemBase)
        {
            base.InitializeProperties(itemBase);
            if (itemBase is ThrowableItem throwable && throwable.Projectile is FlashbangGrenade flashGrenade)
            {
                MinimalDurationEffect = flashGrenade._minimalEffectDuration;
                AdditionalBlindedEffect = flashGrenade._additionalBlurDuration;
                SurfaceDistanceIntensifier = flashGrenade._surfaceZoneDistanceIntensifier;
            }
        }
    }
}
