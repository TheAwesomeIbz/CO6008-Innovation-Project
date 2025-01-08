using Entities.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

namespace Entities
{
    public class SCR_EntityShooting : MonoBehaviour
    {
        [Header("ENTITY SHOOTING PROPERTIES")]
        [SerializeField] SO_WeaponProperties weaponProperties;
        [SerializeField] [Range(0.001f, 1)] protected float _cooldownPeriod = 0.125f;
        [field: SerializeField] public ShootingVariant ShootingType;
        protected float _cooldown;

        [Header("LINEAR SHOOTING PROPERTIES")]
        [SerializeField][Range(0, 360)] int[] _angleOfShooting = { 0 };

        [Header("CIRCULAR SHOOTING PROPERTIES")]
        [SerializeField][Range(8, 100)] int _circularBullets = 8;

        [Header("CROSS SHOOTING PROPERTIES")]
        [SerializeField][Range(0, Mathf.PI / 2)] float _crossShootingOffset = 0;

        [Header("SPIRAL SHOOTING PROPERTIES")]
        [SerializeField][Range(1, 20)] int _spiralBullets = 8;
        [SerializeField] [Range(10, 100)] float _spiralSpeed = 20;
        float _spiralAngle = 0;

        
        /// <summary>
        /// Virtual function called in Update() to handle shooting physics and mechanics whenever the cooldown counter has disabled;
        /// </summary>
        public void EntityShootingUpdate(Transform targetObject)
        {
            if (weaponProperties == null) { return; }
            if (_cooldown > 0) { return; }

            switch (ShootingType)
            {
                case ShootingVariant.LINEAR:
                    LinearShooting();
                    break;
                case ShootingVariant.CIRCULAR:
                    CircularShooting();
                    break;
                case ShootingVariant.CROSS:
                    CrossShooting();
                    break;
                case ShootingVariant.SPIRAL:
                    SpiralShooting();
                    break;
                case ShootingVariant.CUSTOM:
                    if (!targetObject) { return; }
                    CustomShooting(targetObject);
                    break;
                default:
                    if (!targetObject) { return; }
                    DefaultShooting(targetObject);
                    break;
            }
        }

        /// <summary>
        /// Set shooting style for entity
        /// </summary>
        /// <param name="ShootingType"></param>
        public void SetShootingStyle(ShootingVariant ShootingType) => this.ShootingType = ShootingType;

        /// <summary>
        /// Set cooldown counter for entity
        /// </summary>
        /// <param name="cooldownPeriod"></param>
        public void SetCooldownPeriod(float cooldownPeriod) => _cooldownPeriod = cooldownPeriod;


        public void SetLinearShootingAngles(int[] angles) => _angleOfShooting = angles;
        private void DefaultShooting(Transform targetObject)
        {
            Vector2 relativePosition = targetObject.position - transform.position;
            float outputDirection = Mathf.Atan2(relativePosition.y, relativePosition.x);

            weaponProperties.SpawnBullet(new SO_WeaponProperties.BulletProperties
            {
                ShootingObject = transform,
                InputDirection = outputDirection
            });
            _cooldown = weaponProperties.WeaponCooldown + Random.Range(0, _cooldownPeriod);
        }

        private void LinearShooting()
        {
            if (_angleOfShooting.Length == 0) { return; }
            for (int i = 0; i < _angleOfShooting.Length; i++)
            {
                weaponProperties.SpawnBulletDefault(new SO_WeaponProperties.BulletProperties
                {
                    ShootingObject = transform,
                    InputDirection = _angleOfShooting[i] * Mathf.Deg2Rad
                });

            }
            _cooldown = _cooldownPeriod;
        }
        private void CircularShooting()
        {

            for (int i = 0; i < _circularBullets; i++)
            {
                float angle = i * (2 * Mathf.PI / _circularBullets);
                weaponProperties.SpawnBulletDefault(new SO_WeaponProperties.BulletProperties
                {
                    ShootingObject = transform,
                    InputDirection = angle
                });
                
            }
            _cooldown = _cooldownPeriod;
        }

        private void CrossShooting()
        {
            for (int i = 0; i < 4; i++)
            {
                float angle = i * (2 * Mathf.PI / 4) + _crossShootingOffset;
                weaponProperties.SpawnBulletDefault(new SO_WeaponProperties.BulletProperties
                {
                    ShootingObject = transform,
                    InputDirection = angle
                });

            }
            _cooldown = _cooldownPeriod;
        }

        private void SpiralShooting()
        {
            _spiralAngle += Time.deltaTime * _spiralSpeed; 
            if (_spiralAngle > 2 * Mathf.PI) { _spiralAngle = 0; }

            for (int i = 0; i < _spiralBullets; i++)
            {
                float angle = i * (2 * Mathf.PI / _spiralBullets) + _spiralAngle;
                weaponProperties.SpawnBulletDefault(new SO_WeaponProperties.BulletProperties
                {
                    ShootingObject = transform,
                    InputDirection = angle
                });

            }
            _cooldown = _cooldownPeriod;
        }


        /// <summary>
        /// CUstom implementation for shooting that can be overriden in inherited classes
        /// </summary>
        /// <param name="targetObject"></param>
        protected virtual void CustomShooting(Transform targetObject)
        {

        }

        protected virtual void Update()
        {
            _cooldown -= _cooldown >= 0 ? Time.deltaTime : 0;
        }

        public enum ShootingVariant
        {
            DEFAULT,
            LINEAR,
            CIRCULAR,
            CROSS,
            SPIRAL,
            CUSTOM
        }

    }

}
