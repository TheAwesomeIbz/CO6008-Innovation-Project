using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace Entities
{
    /// <summary>
    /// Damage collider object that handles and deals with collision and damage with objects with hitboxes
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class SCR_DamageCollider : MonoBehaviour
    {

        SettingsInformation settings;
        [Header("ATTACKING PROPERTIES")]
        [SerializeField] protected Attackable _damageableTo;
        [SerializeField] protected int _attack = 5;
        [SerializeField] [Range(0, 1)] protected float _attackPercentage;
        [SerializeField] protected bool _attackCalculatedByPercentage;
        [SerializeField] [Range(0, 2)] protected float _stunTimer;
        [field: SerializeField] public bool DodgeableCollider { get; protected set; }

        /// <summary>
        /// Amount of time an entity should be stunned for once hit with this collider
        /// </summary>
        public float StunTimer => _stunTimer;

        /// <summary>
        /// Returns whether the attack should be calculated by a percentage value or fixed value
        /// </summary>
        public bool AttackCalculatedByPercentage => _attackCalculatedByPercentage;

        [Header("KNOCKBACK PROPERTIES")]
        [SerializeField] protected float _knockbackDirection;
        [SerializeField] protected float _knockbackMagnitude;

        /// <summary>
        /// Returns constant attack value
        /// </summary>
        public int Attack => _damageableTo == Attackable.PLAYER ?
            Mathf.RoundToInt(_attack * (0.5f + ((float)settings.GameMode * 0.5f))) :
            Mathf.RoundToInt((2 * _attack) / (float)(1 + (int)settings.GameMode) );
            

        /// <summary>
        /// Returns attack percentage float from 0 to 1
        /// </summary>
        public float AttackPercentage => _attackPercentage;

        /// <summary>
        /// Returns direction that the damaged object should be launched towards
        /// </summary>
        public float KnockbackDirection => _knockbackDirection;

        /// <summary>
        /// Returns the magnitude of knockback that the damaged object should receive
        /// </summary>
        public float KnockbackMagnitude => _knockbackMagnitude;

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            //Only collide with object if it has a hitbox component
            //Apply and add necessary properties on damage collider from hitbox or relevant objects
            if (collision.GetType(out CMP_HitboxComponent hitbox) == null) { return; }

            //Ensure that the damageable object cannot be damaged by its own collider
            if (_damageableTo == hitbox.DamageableBy) { return; }
            hitbox.DealDamage(this);
        }


        protected virtual IEnumerator Start()
        {
            settings = SCR_GeneralManager.Instance.Settings;
            GetComponent<Collider2D>().isTrigger = true;
            yield return null;
        }
    }

    
}

