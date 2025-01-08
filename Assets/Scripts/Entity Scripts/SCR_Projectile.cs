using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{

    /// <summary>
    /// Projectile class that parses damage information which also inherits from the base damage collider. Only applied to projectile-based objects
    /// </summary>
    public class SCR_Projectile : SCR_DamageCollider
    {
        [Header("PROJECTILE PROPERTIES")]
        [SerializeField] bool _impenetrable;
        protected override IEnumerator Start()
        {
            yield return new WaitForSeconds(2);
            Destroy(gameObject);
        }

        /// <summary>
        /// Initialize the projectile with weapon properties and bullet properties, to initialize its _bossSpeed, direction and collider type on instantiation
        /// </summary>
        /// <param name="weaponProperties">Information about the weapon</param>
        /// <param name="bulletProperties">Information about the bullet and projectile(s)</param>
        public void InitializeProjectile(SO_WeaponProperties weaponProperties, SO_WeaponProperties.BulletProperties bulletProperties)
        {
            DodgeableCollider = weaponProperties.DodgeableBullet;
            SetupColliderVariant(bulletProperties.ShootingObject);
            Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();

            _knockbackMagnitude = weaponProperties.KnockbackMagnitude;

            transform.localScale = new Vector3(transform.localScale.x * Mathf.Sign(bulletProperties.ShootingObject.transform.localScale.x), transform.localScale.y);
            rigidbody2D.velocity = new Vector2(Mathf.Cos(bulletProperties.InputDirection), Mathf.Sin(bulletProperties.InputDirection)) * bulletProperties.BulletMagnitude;
        }

        /// <summary>
        /// Initialize the collider variant of the damage collider.
        /// </summary>
        /// <remarks>Dependant on the properties of the originObject parameter</remarks>
        /// <param name="originObject">The object that the projectile is shot from</param>
        private void SetupColliderVariant(Transform originObject)
        {
            if (originObject.GetComponent<Player.SCR_PlayerShooting>())
            {
                _damageableTo = Attackable.ENEMIES;
            }
            else
            {
                _damageableTo = Attackable.PLAYER;
            }
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (GetComponent<Collider2D>().IsTouchingLayers(GlobalMasks.BoundaryLayerMask) && _impenetrable)
            {
                Destroy(gameObject);
            }
            base.OnTriggerEnter2D(collision);

            if (collision.GetType(out CMP_HitboxComponent hitboxComponent) && hitboxComponent.DamageableBy != _damageableTo)
            {
                Destroy(gameObject);
            }
        }

    }
}
