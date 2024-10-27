using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class SCR_DamageCollider : MonoBehaviour
    {
        [SerializeField] private int _attack = 5;
        //TODO : call the damage interface on the objects in contact with the collider
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetType(out CMP_HitboxComponent hitbox) == null) { return; }

            hitbox.DealDamage(_attack);
        }
    }

    public enum DamageColliderVariant
    {
        DAMAGABLE_TO_PLAYER,
        DAMAGEABLE_TO_ENEMIES,
        DAMAGABLE_TO_ALL_ENTITIES
    }
}

