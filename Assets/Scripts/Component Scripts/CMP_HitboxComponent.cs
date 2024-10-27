using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CMP_HitboxComponent : MonoBehaviour
    {
        public event System.Action OnDamageEvent;
        public event System.Action OnZeroHPEvent;

        [Header("DAMAGEABLE COMPONENT PROPERTIES")]
        [SerializeField] DamageColliderVariant _damageColliderVariant;
        [SerializeField] int _HP;
        [SerializeField] int _maxHP;
        [SerializeField] int _defence;
        public int HP => _HP;
        public int MaxHP => _maxHP;
        public int Defence => _defence;
        public DamageColliderVariant DamageColliderVariant => _damageColliderVariant;

        void Start()
        {
            if (_HP <= 0) { _HP = 20; }
            if (Defence <= 0) { _defence = 1; }

            _maxHP = _HP;
        }

        public void DealDamage(int damage)
        {
            _HP -= damage;

            if (_HP <= 0) {
                _HP = 0;
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
