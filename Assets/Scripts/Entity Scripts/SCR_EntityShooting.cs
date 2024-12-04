using Entities.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class SCR_EntityShooting : MonoBehaviour
    {
        [Header("ENTITY SHOOTING PROPERTIES")]
        [SerializeField] SO_WeaponProperties weaponProperties;
        [SerializeField] [Range(0.125f, 1)] protected float _randomCooldownOffset = 0.125f;
        [SerializeField] ShootingType _shootingType;
        private float _outputDirection;
        private float _cooldown;
        /// <summary>
        /// Virtual function called in Update() to handle shooting physics and mechanics
        /// </summary>
        /// TODO : ADD ENTITY SHOOTING TYPES IN ENUM AND FUNCTIONALITY
        /// ALSO ADD THIS FUNCTIONALITY TO PLAYER SHOOTING PHYSICS
        public void EntityShootingUpdate(Transform targetObject)
        {
            if (!targetObject) { return; }
            if (weaponProperties == null) { return; }

            switch (_shootingType)
            {
                case ShootingType.CIRCULAR:
                    CircularShooting(targetObject);
                    break;
                case ShootingType.CROSS:
                    CrossShooting(targetObject);
                    break;
                case ShootingType.SPIRAL:
                    SpiralShooting(targetObject);
                    break;
                case ShootingType.ALTERNATING:
                    AlternatingShooting(targetObject);
                    break;
                case ShootingType.CUSTOM:
                    CustomShooting(targetObject);
                    break;
                default:
                    DefaultShooting(targetObject);
                    break;
            }
        }

        private void DefaultShooting(Transform targetObject)
        {
            Vector2 relativePosition = targetObject.position - transform.position;
            _outputDirection = Mathf.Atan2(relativePosition.y, relativePosition.x);

            if (_cooldown < 0)
            {
                weaponProperties.SpawnBullet(new SO_WeaponProperties.BulletProperties
                {
                    ShootingObject = transform,
                    InputDirection = _outputDirection
                });
                _cooldown = weaponProperties.WeaponCooldown + Random.Range(0, _randomCooldownOffset);
            }
        }

        private void CircularShooting(Transform targetObject)
        {

        }

        private void CrossShooting(Transform targetObject)
        {

        }

        private void SpiralShooting(Transform targetObject)
        {

        }

        private void AlternatingShooting(Transform targetObject)
        {

        }

        protected virtual void CustomShooting(Transform targetObject)
        {

        }

        protected void Update()
        {
            _cooldown -= _cooldown >= 0 ? Time.deltaTime : 0;
        }

        protected enum ShootingType
        {
            DEFAULT,
            CIRCULAR,
            CROSS,
            SPIRAL,
            ALTERNATING,
            CUSTOM
        }
    }

}
