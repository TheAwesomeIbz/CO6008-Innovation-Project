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
        [SerializeField] protected Attackable _damageableTo;
        [SerializeField] protected AttackType _attackType;
        [SerializeField] protected int _attack = 5;
        [SerializeField] [Range(0, 1)] protected float _attackPercentage;
        [SerializeField] [Range(0, 2)] protected float _stunTimer;
        public float StunTimer => _stunTimer;

        public AttackType GetAttackType => _attackType;

        [Header("KNOCKBACK PROPERTIES")]
        [SerializeField] protected float _knockbackDirection;
        [SerializeField] protected float _knockbackMagnitude;

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
            GetComponent<Collider2D>().isTrigger = true;
            yield return null;
        }

        public enum AttackType
        {
            CONSTANT,
            PERCENTAGE,
        }
    }

    
}

