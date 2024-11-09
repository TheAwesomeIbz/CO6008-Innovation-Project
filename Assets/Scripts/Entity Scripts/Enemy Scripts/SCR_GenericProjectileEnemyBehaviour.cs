using System.Collections;
using System.Collections.Generic;
using Entities.Player;
using UnityEngine;

namespace Entities.Enemies
{
    public class SCR_ProjectileEnemyBehaviour : SCR_EnemyBehaviour
    {
        [Header("PROJECTILE ENEMY PROPERTY")]
        [SerializeField] SO_WeaponProperties weaponProperties;
        [SerializeField] [Range(0.125f, 1)] protected float _randomCooldownOffset = 0.125f;
        SCR_PlayerMovement _playerMovementReference;
        private float _outputDirection;
        private float _cooldown;


        protected override void OnPlayerDetected(SCR_PlayerMovement playerMovement)
        {
            _playerMovementReference = playerMovement;
            base.OnPlayerDetected(playerMovement);
        }
        protected override void PlayerSpottedUpdate()
        {
            if (!_playerMovementReference) { return; }

            Vector2 relativePosition =  _playerMovementReference.transform.position - transform.position;
            _outputDirection = Mathf.Atan2(relativePosition.y, relativePosition.x);


            if (_cooldown < 0)
            {
                weaponProperties.SpawnBullet(transform, this, _outputDirection);
                _cooldown = weaponProperties.WeaponCooldown + Random.Range(0, _randomCooldownOffset);
            }
            
            
            base.PlayerSpottedUpdate();
        }

        protected override void Update()
        {
            _cooldown -= _cooldown >= 0 ? Time.deltaTime : 0;
            base.Update();
        }

    }
}
