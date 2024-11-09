using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Enemies
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class SCR_EnemyBehaviour : MonoBehaviour, iAttackable
    {
        protected Rigidbody2D _rigidbody2D;
        protected BoxCollider2D _boxCollider2D;

        [Header("BASE ENEMY PROPERTIES")]
        [SerializeField] protected Attackable _damageableTo;
        [SerializeField] protected EnemyBehaviour _enemyBehaviour;
        [SerializeField] [Range(3,10)] protected float _enemySpeed = 5;
        protected bool _grounded;
        protected bool _touchingLedge;
        protected bool _touchingWall;

        [Header("PLAYER DETECTION PROPERTIES")]
        [SerializeField] protected bool _canDetectPlayer;
        [SerializeField] protected bool _playerSpotted;
        [SerializeField] [Range(4, 20)] protected int _playerDetectionRange;
       
        
        private CircleCollider2D _circleCollider2D;
        private SCR_PlayerDetectionTrigger playerDetectionTrigger;

        public Attackable DamageableTo => _damageableTo;

        void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _circleCollider2D = GetComponentInChildren<CircleCollider2D>();
        }

        

        private void ColliderUpdate()
        {
            RaycastHit2D groundRaycast = Physics2D.Raycast(_boxCollider2D.bounds.center, Vector2.down, _boxCollider2D.bounds.center.y * 1.25f, GlobalMasks.GroundLayerMask);
            _grounded = groundRaycast.collider != null;

            Vector2 edgeCenter = _boxCollider2D.bounds.center + new Vector3(_boxCollider2D.bounds.extents.x * 1.25f * Mathf.Sign(transform.localScale.x), 0);
            RaycastHit2D ledgeRaycast = Physics2D.Raycast(edgeCenter, Vector2.down, _boxCollider2D.bounds.center.y * 1.25f, GlobalMasks.GroundLayerMask);
            _touchingLedge = ledgeRaycast.collider == null && _grounded;

            RaycastHit2D wallRaycast = Physics2D.Raycast(_boxCollider2D.bounds.center, new Vector2(Mathf.Sign(transform.localScale.x), 0), _boxCollider2D.bounds.extents.x * 1.25f, GlobalMasks.GroundLayerMask);
            _touchingWall = wallRaycast.collider != null;

        }

        private void LinearPatrolUpdate()
        {
            _rigidbody2D.velocity = new Vector2(Mathf.Lerp(_rigidbody2D.velocity.x, _enemySpeed * Mathf.Sign(transform.localScale.x), Time.deltaTime * 5), _rigidbody2D.velocity.y);
            if (_touchingLedge || _touchingWall)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y);
            }
        }

        private void RandomPatrolUpdate()
        {
            
        }

        

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

            PlayerDetectionUpdate();

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

        protected virtual void PlayerSpottedUpdate()
        {

        }

        protected virtual void StaticBehaviourUpdate()
        {

        }

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

        protected enum EnemyBehaviour
        {
            ENEMY_STATIC,
            ENEMY_PATROL_LINEAR,
            ENEMY_PATROL_RANDOM
        }
    }
}

