using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class SCR_DamageCollider : MonoBehaviour
    {
        [SerializeField] private int _attack = 5;
        [SerializeField] private DamageColliderVariant _damageColliderVariant;
        //TODO : call the damage interface on the objects in contact with the collider
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetType(out CMP_HitboxComponent hitbox) == null) { return; }

            hitbox.DealDamage(_attack);

        }

        private bool CanDamageHitbox(CMP_HitboxComponent collision)
        {
            switch (collision.ColliderVariant)
            {
                case HitboxColliderVariant.DAMAGEABLE_BY_PLAYER:
                    return _damageColliderVariant == DamageColliderVariant.DAMAGABLE_TO_PLAYER || _damageColliderVariant == DamageColliderVariant.DAMAGABLE_TO_ALL_ENTITIES;
                case HitboxColliderVariant.DAMAGEABLE_BY_ENEMEIES:
                    return _damageColliderVariant == DamageColliderVariant.DAMAGEABLE_TO_ENEMIES || _damageColliderVariant == DamageColliderVariant.DAMAGABLE_TO_ALL_ENTITIES;
                case HitboxColliderVariant.DAMAGEABLE_BY_ALL_ENTITIES:
                    return true;
                    default: return false;
            }
        }
    }

    public enum DamageColliderVariant
    {
        DAMAGABLE_TO_PLAYER,
        DAMAGEABLE_TO_ENEMIES,
        DAMAGABLE_TO_ALL_ENTITIES
    }
}

