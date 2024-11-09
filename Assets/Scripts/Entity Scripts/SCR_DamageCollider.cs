using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace Entities
{
    [RequireComponent(typeof(Collider2D))]
    public class SCR_DamageCollider : MonoBehaviour
    {
        [Header("ATTACKING PROPERTIES")]
        [SerializeField] private Attackable _damageableTo;
        [SerializeField] private AttackType _attackType;
        [SerializeField] private int _attack = 5;
        [SerializeField] [Range(0, 1)] private float _attackPercentage;

        public AttackType GetAttackType => _attackType;

        [Header("KNOCKBACK PROPERTIES")]
        [SerializeField] private float _knockbackDirection;
        [SerializeField] private float _knockbackMagnitude;

        /// <summary>
        /// Returns constant attack value
        /// </summary>
        public int Attack => _attack;

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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //Only collide with object if it has a hitbox component
            //Apply and add necessary properties on damage collider from hitbox or relevant objects
            if (collision.GetType(out CMP_HitboxComponent hitbox) == null) { return; }

            if (transform.GetType(out SCR_Projectile projectile) != null)
            {
                _knockbackDirection = projectile.weaponProperties.KnockbackDirection;
                _knockbackMagnitude = projectile.weaponProperties.KnockbackMagnitude;
            }

            //Ensure that the damageable object cannot be damaged by its own collider
            if (_damageableTo == hitbox.DamageableBy) { return; }
            hitbox.DealDamage(this);
        }

        public void InitializeCollider(Transform shootingObject, Attackable damageColliderVariant)
        {
            _damageableTo = damageColliderVariant;
        }

        private void Start()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        public enum AttackType
        {
            CONSTANT,
            PERCENTAGE,
        }
    }

    
}

