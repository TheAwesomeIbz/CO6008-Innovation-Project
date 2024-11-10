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
        [SerializeField] protected EnemyBehaviour _enemyBehaviour;
        [SerializeField] [Range(3,10)] protected float _enemySpeed = 5;
        [SerializeField] protected bool _grounded;
        [SerializeField] protected bool _touchingLedge;
        [SerializeField] protected bool _touchingWall;

        [Header("PLAYER DETECTION PROPERTIES")]
        [SerializeField] protected bool _canDetectPlayer;
        [SerializeField] protected bool _playerSpotted;
        [SerializeField] [Range(4, 20)] protected int _playerDetectionRange;
       
        
        private CircleCollider2D _circleCollider2D;
        private SCR_PlayerDetectionTrigger playerDetectionTrigger;

        void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _circleCollider2D = GetComponentInChildren<CircleCollider2D>();
        }

        /// <summary>
        /// Method called in update to update the colliders of the object to detect ground, walls etc.
        /// </summary>
        private void ColliderUpdate()
        {
            RaycastHit2D groundRaycast = Physics2D.Raycast(_boxCollider2D.bounds.center, Vector2.down, _boxCollider2D.bounds.extents.y * 1.25f, GlobalMasks.GroundLayerMask);
            _grounded = groundRaycast.collider != null;

            Vector2 ledgeCenter = _boxCollider2D.bounds.center + new Vector3(_boxCollider2D.bounds.extents.x * 1.25f * Mathf.Sign(transform.localScale.x), -_boxCollider2D.bounds.extents.y * 1.25f);
            Collider2D ledgeRaycast = Physics2D.OverlapPoint(ledgeCenter, GlobalMasks.GroundLayerMask);
            _touchingLedge = ledgeRaycast == null && _grounded;

            RaycastHit2D wallRaycast = Physics2D.Raycast(_boxCollider2D.bounds.center, new Vector2(Mathf.Sign(transform.localScale.x), 0), _boxCollider2D.bounds.extents.x * 1.25f, GlobalMasks.GroundLayerMask);
            _touchingWall = wallRaycast.collider != null;
        }

        /// <summary>
        /// Linearly patrolling enemy behaviour that only detects walls and ledges
        /// </summary>
        private void LinearPatrolUpdate()
        {
            _rigidbody2D.velocity = new Vector2(Mathf.Lerp(_rigidbody2D.velocity.x, _enemySpeed * Mathf.Sign(transform.localScale.x), Time.deltaTime * 5), _rigidbody2D.velocity.y);
            if (_touchingLedge || _touchingWall)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y);
            }
        }

        /// <summary>
        /// Random patrolling enemy behaviour that walks and turns at random, taking into account walls and ledges
        /// </summary>
        private void RandomPatrolUpdate()
        {
            
        }

        
        /// <summary>
        /// Method called in update to detect and scan for the player
        /// </summary>
        /// <remarks>Dynamically creates player detection zone and functionality and gameobjects if it doesn't exist already</remarks>
        private void PlayerDetectionUpdate()
        {
            if (!_canDetectPlayer) { return; }

            if (_circleCollider2D == null)
            {
                GameObject obj = new GameObject("playerDetectionZone");
                playerDetectionTrigger = obj.AddComponent<SCR_PlayerDetectionTrigger>();
                playerDetectionTrigger.InitializeCollider(_playerDetectionRange);
                obj.transform.parent = transform;
                _circleCollider2D = playerDetectionTrigger.GetComponent<CircleCollider2D>();
                obj.transform.localPosition = Vector3.zero;
                playerDetectionTrigger.OnPlayerDetected += OnPlayerDetected;
            }
        }

        

        protected virtual void Update()
        {
            
            ColliderUpdate();
            if (_playerSpotted)
            {
                PlayerSpottedUpdate();
                return;
            }
            else
            {
                PlayerDetectionUpdate();
            }

            switch (_enemyBehaviour)
            {
                case EnemyBehaviour.ENEMY_STATIC:
                    StaticBehaviourUpdate();
                    break;
                case EnemyBehaviour.ENEMY_PATROL_LINEAR:
                    LinearPatrolUpdate();
                    break;
                case EnemyBehaviour.ENEMY_PATROL_RANDOM:
                    RandomPatrolUpdate();
                    break;
            }
            
        }

        /// <summary>
        /// Virtual function called in update that executes whenever the player has been spotted.
        /// </summary>
        /// <remarks>Functionality can be implemented in inherited classes as it defaults to static behaviour</remarks>
        protected virtual void PlayerSpottedUpdate()
        {

        }

        /// <summary>
        /// Virtual function called in update that executes when enemy behaviour is static.
        /// </summary>
        /// <remarks>Functionality can be implemented in inherited classes as it defaults to no functionality</remarks>
        protected virtual void StaticBehaviourUpdate()
        {

        }

        /// <summary>
        /// Virtual function called when the player has been detected.
        /// </summary>
        /// <param name="playerMovement">The input player object detected</param>
        protected virtual void OnPlayerDetected(Player.SCR_PlayerMovement playerMovement)
        {
            if (!_canDetectPlayer) { return; }

            _playerSpotted = true;
            _rigidbody2D.velocity = Vector2.zero;
            Debug.Log($"{playerMovement.name} WAS SPOTTED BY {name}");
        }

        private void OnEnable()
        {
            playerDetectionTrigger = GetComponentInChildren<SCR_PlayerDetectionTrigger>();
            if (playerDetectionTrigger == null) { return; }
            playerDetectionTrigger.OnPlayerDetected += OnPlayerDetected;
        }



        private void OnDisable()
        {
            if (playerDetectionTrigger == null) { return; }
            playerDetectionTrigger.OnPlayerDetected -= OnPlayerDetected;
        }

        /// <summary>
        /// Values for the enemy behaviour
        /// </summary>
        protected enum EnemyBehaviour
        {
            ENEMY_STATIC,
            ENEMY_PATROL_LINEAR,
            ENEMY_PATROL_RANDOM
        }
    }
}

