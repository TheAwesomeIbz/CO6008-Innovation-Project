using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CMP_HitboxComponent : MonoBehaviour
    {
        public event System.Action OnDamageEvent;
        public event System.Action OnZeroHPEvent;

        [Header("DAMAGEABLE COMPONENT PROPERTIES")]
        [SerializeField] HitboxColliderVariant _colliderVariant;
        [SerializeField] CMP_HealthComponent _healthComponent;

        [Header("KNOCKBACKABLE PROPERTIES")]
        [SerializeField] Rigidbody2D _rigidbody2D;
        [SerializeField] bool _knockbackable;
        
        public HitboxColliderVariant ColliderVariant => _colliderVariant;

        void Start()
        {
            _healthComponent = GetComponentInParent<CMP_HealthComponent>() ?? GetComponent<CMP_HealthComponent>();
            _rigidbody2D = GetComponentInParent<Rigidbody2D>() ?? GetComponent<Rigidbody2D>();
        }

        public void DealDamage(SCR_DamageCollider damageCollider)
        {
            if (_healthComponent == null) {
                Debug.LogWarning($"<color=yellow>THERE IS NO HEALTH COMPONENT ATTACHED TO {transform?.name.ToUpper()} or {transform.parent?.name.ToUpper()}.\nHEALTH WILL NOT BE INFUENCED AT ALL.</color>");
                return; 
            }

            if (_knockbackable && _rigidbody2D)
            {
                int localScale = damageCollider.transform.localScale.x > 0 ? 1 : -1;
                _rigidbody2D.velocity = new Vector2(localScale * 
                    Mathf.Cos(damageCollider.KnockbackDirection * Mathf.Deg2Rad), 
                    Mathf.Sin(damageCollider.KnockbackDirection * Mathf.Deg2Rad)) * Mathf.Abs(damageCollider.KnockbackMagnitude);
            }

            if (_healthComponent.HP <= 0) { return; }
            
            _healthComponent.LoseHP(damageCollider.Attack);
            if (_healthComponent.HP == 0) {
                OnZeroHPEvent?.Invoke();
            }
            else{
                OnDamageEvent?.Invoke();
            }
            Debug.Log($"{transform.name.ToUpper()} TOOK {damageCollider.Attack} DAMAGE!\nHP LEFT : {_healthComponent.HP}");
        }

    }

    public enum HitboxColliderVariant
    {
        DAMAGEABLE_BY_PLAYER,
        DAMAGEABLE_BY_ENEMEIES,
        DAMAGEABLE_BY_ALL_ENTITIES
    }
}
