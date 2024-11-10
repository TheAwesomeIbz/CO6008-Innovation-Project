using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// Scriptable object used to store information about projectiles and shoot aformentioned projectile(s) from a given entity with customisable attributes
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Objects/Weapon Property")]
    public class SO_WeaponProperties : ScriptableObject
    {
        [Header("SHOOTING PROPERTIES")]
        [SerializeField] GameObject _bulletPrefab;
        [SerializeField] [Range(5, 50)] int _bulletMagnitude;
        [SerializeField] [Range(0f, 3f)] float _weaponCooldown;
        [SerializeField] [Range(1, 30)] int _bulletAmount;

        [Header("RANDOM PROPERTIES")]
        [SerializeField] [Range(0f, 1)] float _bulletSpreadRange;
        [SerializeField] [Range(0, 10)] int _bulletMagnitudeRange;

        [Header("KNOCKBACK PROPERTIES")]
        [SerializeField] [Range(0, 30f)] int _recoilImpulse;
        [SerializeField] float _knockbackDirection;
        [SerializeField] int _knockbackMagnitude;

        /// <summary>
        /// Knockback dealt to colliding object
        /// </summary>
        public float KnockbackMagnitude => _knockbackMagnitude;

        /// <summary>
        /// Direction that the colliding object should be launched towards
        /// </summary>
        public float KnockbackDirection => _knockbackDirection;

        /// <summary>
        /// Amount of time in seconds that the shooting object should take to shoot again
        /// </summary>
        public float WeaponCooldown => _weaponCooldown;

        /// <summary>
        /// Dunamically spawns bullet with given direction and collider properties with specified speed
        /// </summary>
        /// <param name="bulletProperties">Flexible class used to add additonal parameters flexibly</param>
        public void SpawnBullet(BulletProperties bulletProperties)
        {
            if (_bulletAmount == 1)
            {
                SCR_Projectile obj = Instantiate(_bulletPrefab, null).GetComponent<SCR_Projectile>();
                obj.transform.position = bulletProperties.ShootingObject.position + new Vector3(Mathf.Sign(bulletProperties.ShootingObject.transform.localScale.x), 0);
                bulletProperties.BulletMagnitude = _bulletMagnitude;
                obj.InitializeProjectile(this, bulletProperties);
            }
            else
            {
                float defaultDirectionAngle = bulletProperties.InputDirection;
                for (int i = 0; i < _bulletAmount; i++)
                {
                    SCR_Projectile obj = Instantiate(_bulletPrefab, null).GetComponent<SCR_Projectile>();
                    obj.transform.position = bulletProperties.ShootingObject.position + new Vector3(Mathf.Sign(bulletProperties.ShootingObject.transform.localScale.x), 0);
                    float randomDirectionAngle = Random.Range(defaultDirectionAngle - _bulletSpreadRange, defaultDirectionAngle + _bulletSpreadRange);
                    bulletProperties.InputDirection = randomDirectionAngle;
                    bulletProperties.BulletMagnitude = _bulletMagnitudeRange + Random.Range(_bulletMagnitude - _bulletMagnitudeRange, _bulletMagnitude + _bulletMagnitudeRange);
                    obj.InitializeProjectile(this, bulletProperties);
                }
            }

            //Find rigidbody of shooting object, and if it exists, add recoil force
            Rigidbody2D rigidbody2D = bulletProperties.ShootingObject.GetComponent<Rigidbody2D>();
            if (rigidbody2D)
            {
                rigidbody2D.velocity += new Vector2(-Mathf.Cos(bulletProperties.InputDirection), Mathf.Sin(-bulletProperties.InputDirection)) * _recoilImpulse;
            }
        }

        /// <summary>
        /// Class parameter used to spawn bullets with 
        /// </summary>
        public class BulletProperties
        {
            public Transform ShootingObject;
            public float InputDirection;
            public float BulletMagnitude;

        }
    }

}