using Entities.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class SCR_StatusCollectable : BaseCollectable
    {
        [Header("POWERUP PROPERTIES")]
        [SerializeField] StatusCollectable StatusCollectables;

        [Header("HEALTH COLLECTIBLE PROPERTIES")]
        [SerializeField] bool _recoverPercentage;
        [SerializeField] [Range(0.1f, 1)] float _percentageAmount;
        [SerializeField] [Range(0, 500)] int _fixedRecoveryAmount = 10;

        [Header("DAMAGE COLLECTABLE PROPERTIES")]
        [SerializeField][Range(1, 5)] float _damageMultiplier = 2;

        [Header("DAMAGE COLLECTABLE PROPERTIES")]
        [SerializeField][Range(1, 5)] float _knockbackMultiplier = 2;

        [Header("DAMAGE COLLECTABLE PROPERTIES")]
        [SerializeField][Range(1, 5)] float _agilityMultiplier = 3;

        protected override void OnPlayerCollided(SCR_PlayerMovement playerMovement)
        {
            ApplyHealthRecovery(playerMovement);
            ApplyDamagePowerup(playerMovement);
            ApplyKnockbackPowerup(playerMovement);
            ApplyAgilityPowerup(playerMovement);

        }

        /// <summary>
        /// Recover player's health depending on whether it is a percentage or fixed amount
        /// </summary>
        /// <param name="playerMovement"></param>
        private void ApplyHealthRecovery(SCR_PlayerMovement playerMovement)
        {
            if (!StatusCollectables.HasFlag(StatusCollectable.HEALTH)) { return; }

            if (_recoverPercentage)
            {
                playerMovement.HitboxComponent.RecoverHealth(_percentageAmount);
            }
            else
            {
                playerMovement.HitboxComponent.RecoverHealth(_fixedRecoveryAmount);
            }
            Debug.Log("Health has been recovered");
        }

        private void ApplyDamagePowerup(SCR_PlayerMovement playerMovement)
        {
            if (!StatusCollectables.HasFlag(StatusCollectable.DAMAGE)) { return; }
            playerMovement.DamagePowerupProperty.SetPowerupMultiplier(_damageMultiplier);
        }

        private void ApplyKnockbackPowerup(SCR_PlayerMovement playerMovement)
        {
            if (!StatusCollectables.HasFlag(StatusCollectable.KNOCKBACK)) { return; }
            playerMovement.KnockbackPowerupProperty.SetPowerupMultiplier(_knockbackMultiplier);
        }

        private void ApplyAgilityPowerup(SCR_PlayerMovement playerMovement)
        {
            if (!StatusCollectables.HasFlag(StatusCollectable.AGILITY)) { return; }
            playerMovement.AgilityPowerupProperty.SetPowerupMultiplier(_agilityMultiplier);
        }

        /// <summary>
        /// Flag enum containing what powerups should be activated on collision
        /// </summary>
        [Flags] enum StatusCollectable
        {
            None = 0,
            HEALTH = 1,
            DAMAGE = 2,
            KNOCKBACK = 4,
            AGILITY = 8,
        }
    }


    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class BaseCollectable : MonoBehaviour
    {
        /// <summary>
        /// Abstract method requiring implementation for when the player object collides with a collectible object.
        /// </summary>
        /// <param name="playerMovement"></param>
        protected abstract void OnPlayerCollided(SCR_PlayerMovement playerMovement);
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetType(out SCR_PlayerMovement playerMovement) != null) { 
                OnPlayerCollided(playerMovement); 
            }
        }
    }
}

