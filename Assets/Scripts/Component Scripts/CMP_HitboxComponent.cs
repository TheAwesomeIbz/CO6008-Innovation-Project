using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CMP_HitboxComponent : MonoBehaviour
    {
        public event System.Action<SCR_DamageCollider> OnDamageEvent;
        public event System.Action<SCR_DamageCollider> OnZeroHPEvent;

        [Header("DAMAGEABLE COMPONENT PROPERTIES")]
        [SerializeField] Attackable _damageableBy;
        [SerializeField] CMP_HealthComponent _healthComponent;
        [SerializeField] GameObject _sourceGameobject;

        [Header("KNOCKBACKABLE PROPERTIES")]
        [SerializeField] Rigidbody2D _rigidbody2D;
        [SerializeField] bool _knockbackable;
        
        public Attackable DamageableBy => _damageableBy;

        void Start()
        {
            _healthComponent = GetComponentInParent<CMP_HealthComponent>() ?? GetComponent<CMP_HealthComponent>();
            _rigidbody2D = GetComponentInParent<Rigidbody2D>() ?? GetComponent<Rigidbody2D>();
            GetComponent<BoxCollider2D>().isTrigger = true;
        }

        public void DealDamage(SCR_DamageCollider damageCollider)
        {
            if (_healthComponent == null) {
                _healthComponent = transform.root.gameObject.AddComponent<CMP_HealthComponent>();
                Debug.LogWarning($"<color=yellow>THERE IS NO HEALTH COMPONENT ATTACHED TO {transform?.name.ToUpper()} or {transform.parent?.name.ToUpper()}.\nHEALTH WILL NOT BE INFUENCED AT ALL.</color>");
                return; 
            }

            if (_knockbackable && _rigidbody2D)
            {
                if (damageCollider.KnockbackMagnitude != 0) {
                    int localScale = damageCollider.transform.localScale.x > 0 ? 1 : -1;
                    _rigidbody2D.velocity += new Vector2(localScale *
                        Mathf.Cos(damageCollider.KnockbackDirection * Mathf.Deg2Rad),
                        Mathf.Sin(damageCollider.KnockbackDirection * Mathf.Deg2Rad)) * Mathf.Abs(damageCollider.KnockbackMagnitude);
                }
            }

            if (_healthComponent.HP <= 0) { return; }

            switch (damageCollider.GetAttackType)
            {
                case SCR_DamageCollider.AttackType.CONSTANT:
                    _healthComponent.LoseHP(damageCollider.Attack);
                    break;
                case SCR_DamageCollider.AttackType.PERCENTAGE:
                    _healthComponent.LoseHP(Mathf.RoundToInt(damageCollider.AttackPercentage * _healthComponent.MaxHP));
                    break;
            }
            
            if (_healthComponent.HP == 0) {
                OnZeroHPEvent?.Invoke(damageCollider);
            }
            else{
                OnDamageEvent?.Invoke(damageCollider);
            }
            Debug.Log($"{transform.name.ToUpper()} TOOK {damageCollider.Attack} DAMAGE!\nHP LEFT : {_healthComponent.HP}");
        }

    }
}
