using Entities.Player;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CMP_HitboxComponent : MonoBehaviour
    {
        /// <summary>
        /// Event called when the hitbox component is damaged by a valid collider;
        /// </summary>
        public event System.Action<SCR_DamageCollider> OnDamageEvent;

        /// <summary>
        /// Event called when the health component's HP reaches 0
        /// </summary>
        public event System.Action<SCR_DamageCollider> OnZeroHPEvent;

        [Header("DAMAGEABLE COMPONENT PROPERTIES")]
        [SerializeField] Attackable _damageableBy;
        [SerializeField] CMP_HealthComponent _healthComponent;
        iDodgeable _dodgeableInterface;

        [Header("KNOCKBACKABLE PROPERTIES")]
        [SerializeField] Rigidbody2D _rigidbody2D;
        [SerializeField] bool _knockbackable;

        /// <summary>
        /// Returns the attackable enum that the hitbox componenet is damageable by
        /// </summary>
        public Attackable DamageableBy => _damageableBy;

        public CMP_HealthComponent HealthComponent => _healthComponent;

        void Start()
        {
            _healthComponent = GetComponentInParent<CMP_HealthComponent>() ?? GetComponent<CMP_HealthComponent>();
            _rigidbody2D = GetComponentInParent<Rigidbody2D>() ?? GetComponent<Rigidbody2D>();
            _dodgeableInterface = GetComponentInParent<iDodgeable>();
            GetComponent<BoxCollider2D>().isTrigger = true;
        }

        /// <summary>
        /// Validate the health component on the current hitbox and instantiates a new component if it doesnt exist.
        /// </summary>
        private void ValidateHealthComponent()
        {
            if (_healthComponent == null)
            {
                _healthComponent = transform.root.gameObject.AddComponent<CMP_HealthComponent>();
                Debug.LogWarning($"<color=yellow>THERE IS NO HEALTH COMPONENT ATTACHED TO {transform?.name.ToUpper()} or {transform.parent?.name.ToUpper()}.\nHEALTH WILL NOT BE INFUENCED AT ALL.</color>");
            }
        }

        public void DealDamage(SCR_ConstantDamageCollider constantDamageCollider)
        {
            ValidateHealthComponent();
            if (_knockbackable && _rigidbody2D)
            {


                _rigidbody2D.velocity =  new Vector2(
                    Mathf.Cos(constantDamageCollider.KnockbackDirection * Mathf.Deg2Rad), 
                    Mathf.Sin(constantDamageCollider.KnockbackDirection * Mathf.Deg2Rad)) * 
                    Mathf.Abs(constantDamageCollider.KnockbackMagnitude);
            }

            _healthComponent.LoseHP(1);

            if (_healthComponent.HP == 0)
            {
                OnZeroHPEvent?.Invoke(null);
            }
            else
            {
                OnDamageEvent?.Invoke(null);
                Debug.Log($"{transform.name.ToUpper()} TOOK {1} DAMAGE!\nHP LEFT : {_healthComponent.HP}");
            }
        }

        /// <summary>
        /// Deals damage to health component and broadcasts events on damage
        /// </summary>
        /// <remarks>Dynamically creates component if it doesn't exist</remarks>
        /// <param name="damageCollider"></param>
        public void DealDamage(SCR_DamageCollider damageCollider)
        {
            ValidateHealthComponent();
            if (EntityDodgedSuccessfully(damageCollider)) { return; }

            //Apply standard knockback to object
            if (_knockbackable && _rigidbody2D)
            {
                Rigidbody2D damageColliderRigidbody = damageCollider.GetComponent<Rigidbody2D>();
                if (damageColliderRigidbody && damageCollider.KnockbackMagnitude != 0)
                {
                    _rigidbody2D.velocity = damageColliderRigidbody.velocity.normalized * Mathf.Abs(damageCollider.KnockbackMagnitude);
                }
                
            }

            if (_healthComponent.HP <= 0) { return; }

            int damage = damageCollider.AttackCalculatedByPercentage ? Mathf.RoundToInt(damageCollider.AttackPercentage * _healthComponent.MaxHP) : damageCollider.Attack;
            _healthComponent.LoseHP(damage);

            if (_healthComponent.HP == 0) {
                OnZeroHPEvent?.Invoke(damageCollider);
            }
            else{
                OnDamageEvent?.Invoke(damageCollider);
                //Debug.Log($"{transform.name.ToUpper()} TOOK {damageCollider.Attack} DAMAGE!\nHP LEFT : {_healthComponent.HP}");
            }
            
        }


        /// <summary>
        /// Determines whether an entity dodged from the current damage collider
        /// </summary>
        /// <param name="damageCollider"></param>
        /// <returns>Whether the entity successfully dodged</returns>
        private bool EntityDodgedSuccessfully(SCR_DamageCollider damageCollider)
        {
            return _dodgeableInterface != null && _dodgeableInterface.IsDodging && damageCollider.DodgeableCollider;
        }

        /// <summary>
        /// Recover a fixed amount of Health for the entity
        /// </summary>
        /// <param name="fixedAmount">Any integer amount</param>
        public void RecoverHealth(int fixedAmount)
        {
            ValidateHealthComponent();
            _healthComponent.GainHP(fixedAmount);
        }

        /// <summary>
        /// Recover a percentage amount of the Maximum Health for the entity
        /// </summary>
        /// <param name="percentage">Percentage from 0 to 1</param>
        public void RecoverHealth(float percentage)
        {
            ValidateHealthComponent();
            _healthComponent.GainHP(Mathf.RoundToInt(_healthComponent.MaxHP * percentage));
        }

    }
}
