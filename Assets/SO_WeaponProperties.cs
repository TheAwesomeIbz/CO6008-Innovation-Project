using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{

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

        public float KnockbackMagnitude => _knockbackMagnitude;
        public float KnockbackDirection => _knockbackDirection;

        public GameObject BulletPrefab => _bulletPrefab;
        public float WeaponCooldown => _weaponCooldown;
        public int BulletAmount => _bulletAmount;
        public int BulletMagnitude => _bulletMagnitude;

        public void SpawnBullet(Transform shootingObject, iAttackable iAttackable, float inputDirection)
        {
            Rigidbody2D rigidbody2D = shootingObject.GetComponent<Rigidbody2D>();

            if (_bulletAmount == 1)
            {
                SCR_Projectile obj = Instantiate(_bulletPrefab, null).GetComponent<SCR_Projectile>();
                obj.transform.position = shootingObject.position + new Vector3(1f * Mathf.Sign(shootingObject.transform.localScale.x), 0);
                obj.GetRigidbody2D.velocity = new Vector2(Mathf.Cos(inputDirection), Mathf.Sin(inputDirection)) * _bulletMagnitude;
                obj.InitializeProjectile(this, shootingObject, iAttackable);

                if (rigidbody2D)
                {
                    rigidbody2D.velocity += new Vector2(-Mathf.Cos(inputDirection), Mathf.Sin(-inputDirection)) * _recoilImpulse;
                }
            }
            else
            {
                for (int i = 0; i < _bulletAmount; i++)
                {
                    float bulletMagnitude = _bulletMagnitudeRange + Random.Range(_bulletMagnitude - _bulletMagnitudeRange, _bulletMagnitude + _bulletMagnitudeRange);


                    SCR_Projectile obj = Instantiate(_bulletPrefab, null).GetComponent<SCR_Projectile>();
                    obj.transform.position = shootingObject.position + new Vector3(1f * Mathf.Sign(shootingObject.transform.localScale.x), 0);
                    float direction = Random.Range(inputDirection - _bulletSpreadRange, inputDirection + _bulletSpreadRange);
                    obj.GetRigidbody2D.velocity = new Vector2(Mathf.Cos(direction), Mathf.Sin(direction)) * bulletMagnitude;
                    obj.InitializeProjectile(this, shootingObject, iAttackable);
                }

                if (rigidbody2D)
                {
                    rigidbody2D.velocity = new Vector2(-Mathf.Cos(inputDirection), Mathf.Sin(-inputDirection)) * _recoilImpulse;
                }
            }

        }

    }

}