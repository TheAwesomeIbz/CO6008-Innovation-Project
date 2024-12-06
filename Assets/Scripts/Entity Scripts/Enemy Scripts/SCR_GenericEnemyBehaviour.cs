using Entities.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Enemies
{
    /// <summary>
    /// Base Enemy class that generic enemies would inherit from. This class contains basic functionality for movement and shooting
    /// </summary>
    /// <remarks>(NOT a parent class to custom enemies and/or boss objects)</remarks>
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class SCR_EnemyBehaviour : MonoBehaviour
    {
        protected Rigidbody2D _rigidbody2D;
        protected BoxCollider2D _boxCollider2D;

        [Header("BASE ENEMY PROPERTIES")]
        [SerializeField] protected Attackable _damageableTo;
        [SerializeField] protected CMP_HitboxComponent _hitboxComponent;
        [SerializeField] protected bool _constantlyShooting;
        protected SCR_EntityShooting _entityShooting;

        [Header("ENEMY SPEED PROPERTIES")]
        [SerializeField] [Range(3,10)] protected float _enemySpeed = 5;
        [SerializeField] protected float _acceleration = 1.25f;
        [SerializeField] protected float _decleration = 0.125f;

        [Header("ENEMY PATROLLING PROPERTIES")]
        [SerializeField] protected bool _enemyRandomRoaming;
        [SerializeField] protected bool _enemyChasesPlayer;
        private Vector3 _defaultPosition;
        private Vector3 _randomPosition;
        private float _enemyRandomMovementTimer;
        bool _randomMovementFlag;

        [Header("PLAYER DETECTION PROPERTIES")]
        [SerializeField] protected bool _canDetectPlayer;
        [SerializeField] protected SCR_PlayerMovement _playerMovementReference;
       
        
        private SCR_PlayerDetectionTrigger playerDetectionTrigger;

        protected virtual void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _entityShooting = GetComponent<SCR_EntityShooting>();
            _defaultPosition = transform.position;
            _randomPosition = _defaultPosition + new Vector3(Random.Range(-5, 5), Random.Range(-5, 5));

            _hitboxComponent = GetComponentInChildren<CMP_HitboxComponent>() ?? GetComponent<CMP_HitboxComponent>();
            if (_hitboxComponent) { _hitboxComponent.OnZeroHPEvent += OnZeroHP; }
                
        }
        protected virtual void Update()
        {
            if (_constantlyShooting)
            {
                PlayerSpottedUpdate();
                return;
            }

            if (_playerMovementReference)
            {
                PlayerSpottedUpdate();
            }
            else
            {
                DefaultBehaviourUpdate();
            }
        }

        /// <summary>
        /// Virtual function called in update that executes whenever the player hasn't been spotted and performs default behaviour.
        /// </summary>
        /// <remarks>Functionality can overriden in inherited classes</remarks>
        protected virtual void DefaultBehaviourUpdate()
        {

            if (_enemyRandomRoaming)
            {
                if ((transform.position - _randomPosition).sqrMagnitude > 2)
                {
                    Vector2 maximumSpeed = (_randomPosition - transform.position).normalized * _enemySpeed / 2;
                    _rigidbody2D.velocity = Vector3.Lerp(_rigidbody2D.velocity, maximumSpeed, Time.deltaTime * _acceleration / 2);
                }
                else
                {
                    if ((transform.position - _defaultPosition).sqrMagnitude < 25)
                    {
                        _randomPosition = _defaultPosition + new Vector3(Random.Range(-5, 5), Random.Range(-5, 5));
                    }
                }
            }
            else
            {
                _rigidbody2D.velocity = Vector2.Lerp(_rigidbody2D.velocity, Vector2.zero, Time.deltaTime * _decleration);
            }
        }

        /// <summary>
        /// Virtual function called in update that executes whenever the player has been spotted.
        /// </summary>
        /// <remarks>Functionality can overriden in inherited classes</remarks>
        protected virtual void PlayerSpottedUpdate()
        {
            _entityShooting?.EntityShootingUpdate(_playerMovementReference?.transform ?? null);

            if (!_playerMovementReference) { return; }
            if (!_enemyChasesPlayer) { return; }
            _enemyRandomMovementTimer -= Time.deltaTime;
            if (_enemyRandomMovementTimer < 0)
            {
                _enemyRandomMovementTimer = _enemyRandomMovementTimer = Random.Range(0.1f, 1.5f);
                _randomMovementFlag = !_randomMovementFlag;
            }
            Vector2 maximumVelocity = (_playerMovementReference.transform.position - transform.position).normalized * _enemySpeed;
            if (_randomMovementFlag)
            {
                _rigidbody2D.velocity = Vector3.Lerp(_rigidbody2D.velocity, -maximumVelocity / 2, Time.deltaTime * _acceleration);
            }
            else
            {
                _rigidbody2D.velocity = Vector3.Slerp(_rigidbody2D.velocity, maximumVelocity, Time.deltaTime * _acceleration);
            }
        }

        /// <summary>
        /// Virtual function called when the player has been detected.
        /// </summary>
        /// <param name="playerMovement">The input player object detected</param>
        protected virtual void OnPlayerDetected(SCR_PlayerMovement playerMovement, bool playerDetected)
        {
            if (!_canDetectPlayer) { return; }

            
            _playerMovementReference = playerDetected ? playerMovement : null;
            _rigidbody2D.velocity = Vector2.zero;
            Debug.Log($"{playerMovement.name} WAS SPOTTED BY {name}");
        }

        private void OnEnable()
        {
            playerDetectionTrigger = GetComponentInChildren<SCR_PlayerDetectionTrigger>();
            if (playerDetectionTrigger == null) { return; }
            playerDetectionTrigger.OnPlayerDetected += OnPlayerDetected;
        }


        /// <summary>
        /// Virtual function that is called when the enemy reaches 0 HP
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void OnZeroHP(SCR_DamageCollider obj)
        {
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            if (_hitboxComponent)
            {
                _hitboxComponent.OnZeroHPEvent -= OnZeroHP;
            }
            if (playerDetectionTrigger == null) { return; }
            playerDetectionTrigger.OnPlayerDetected -= OnPlayerDetected;
        }
    }
}

