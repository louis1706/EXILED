// -----------------------------------------------------------------------
// <copyright file="CollisionHandler.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Components
{
    using System;

    using Features;
    using InventorySystem.Items.ThrowableProjectiles;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// Collision Handler for grenades.
    /// </summary>
    public class CollisionHandler : MonoBehaviour
    {
        private bool initialized;
        private float activableTime;
        private Action onCollisionAction;

        /// <summary>
        /// Gets the thrower of the grenade.
        /// </summary>
        public GameObject Owner { get; private set; }

        /// <summary>
        /// Inits the <see cref="CollisionHandler"/> object.
        /// </summary>
        /// <param name="owner">The grenade owner.</param>
        /// <param name="onCollisionAction">Action on collision.</param>
        /// <param name="fuseDelay">Delay before onCollisionAction may be executed by collision.</param>
        public void Init(GameObject owner, Action onCollisionAction, float fuseDelay = 0.15f)
        {
            Owner = owner;
            initialized = true;
            this.onCollisionAction = onCollisionAction;
            activableTime = (float)NetworkTime.time + fuseDelay;
        }

        private void OnCollisionEnter(Collision collision)
        {
            try
            {
                if (!initialized)
                    return;
                if (activableTime > NetworkTime.time)
                    return;

                if (Owner == null)
                    Log.Error($"Owner is null!");
                if (collision.gameObject == null)
                    Log.Error("pepehm");
                if (collision.collider.gameObject == Owner || collision.collider.gameObject.TryGetComponent<EffectGrenade>(out _))
                    return;

                onCollisionAction.Invoke();
            }
            catch (Exception exception)
            {
                Log.Error($"{nameof(OnCollisionEnter)} error:\n{exception}");
                Destroy(this);
            }
        }
    }
}
