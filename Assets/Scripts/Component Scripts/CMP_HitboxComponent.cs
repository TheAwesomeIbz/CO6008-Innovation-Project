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

        public HitboxColliderVariant ColliderVariant => _colliderVariant;

        void Start()
        {
            _healthComponent = GetComponentInParent<CMP_HealthComponent>() ?? GetComponent<CMP_HealthComponent>();
        }

        public void DealDamage(int damage)
        {
            if (_healthComponent == null) {
                Debug.LogWarning($"<color=yellow>THERE IS NO HEALTH COMPONENT ATTACHED TO {transform.name.ToUpper()} or {transform.parent.name.ToUpper()}.\nHEALTH WILL NOT BE INFUENCED AT ALL.</color>");
                return; 
            }

            _healthComponent.LoseHP(damage);
            if (_healthComponent.HP == 0) {
                OnZeroHPEvent?.Invoke();
            }
            else{
                OnDamageEvent?.Invoke();
            }
        }
    }

    public enum HitboxColliderVariant
    {
        DAMAGEABLE_BY_PLAYER,
        DAMAGEABLE_BY_ENEMEIES,
        DAMAGEABLE_BY_ALL_ENTITIES
    }
}
