using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class SCR_ConstantDamageCollider : MonoBehaviour
    {
        [Header("ATTACKING PROPERTIES")]
        [SerializeField] protected Attackable _damageableTo;
        [SerializeField] List<CMP_HitboxComponent> _hitboxComponents;
        
        
        [field : Header("DAMAGE COLLIDER FIELDS")]
        [field: SerializeField] [field : Range(0, 359)] public float KnockbackDirection { get; private set; }
        [field: SerializeField] [field: Range(0, 10)] public float KnockbackMagnitude { get; private set; }
        [SerializeField] [Range(25, 200)] private float _damageRate = 25;
        float _damageCounter;
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            //Only collide with object if it has a hitbox component
            //Apply and add necessary properties on damage collider from hitbox or relevant objects
            if (collision.GetType(out CMP_HitboxComponent hitbox) == null) { return; }

            //Ensure that the damageable object cannot be damaged by its own collider
            if (_damageableTo == hitbox.DamageableBy) { return; }
            _hitboxComponents.Add(hitbox);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetType(out CMP_HitboxComponent hitbox) == null) { return; }
            _hitboxComponents.Remove(hitbox);
        }
        void Update()
        {
            _damageCounter -= Time.deltaTime * _damageRate;
            if (_damageCounter < 0 && _hitboxComponents.Count > 0) {
                foreach (CMP_HitboxComponent hitboxComponent in _hitboxComponents){
                    hitboxComponent?.DealDamage(this);
                }
                _damageCounter = 0.25f;
            }
            
        }
    }

}