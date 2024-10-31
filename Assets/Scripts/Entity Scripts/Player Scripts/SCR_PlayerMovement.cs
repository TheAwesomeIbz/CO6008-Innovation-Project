using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Entities.Player
{
    public class SCR_PlayerMovement : MonoBehaviour, iDamagableStatistic
    {
        [Header("PLAYER MOVEMENT PROPERTIES")]
        [SerializeField] PlayerLevel _playerLevel;
        /// <summary>
        /// Get the Current Player level
        /// </summary>
        public PlayerLevel PlayerLevel => _playerLevel;

        

        SCR_PlayerInputManager _inputManager;
        Rigidbody2D _rigidbody2D;
        BoxCollider2D _boxCollider2D;
        

        [Header("PLAYER GRID MOVEMENT PROPERTIES")]
        [SerializeField] private bool _currentlyMoving;
        [SerializeField] private float _gridSpeed;

        [Header("PLAYER SPEED PROPERTIES")]
        [SerializeField] PlayerSpeedProperties _playerSpeedProperties;

        [Header("PLAYER JUMP PROPERTIES")]
        [SerializeField] PlayerJumpProperties _playerJumpProperties;

        [Header("PLAYER ATTACK PROPERTIES")]
        [SerializeField] private bool _playerAttacking;
        [SerializeField] private float _colliderEnabledTime = 0.25f;
        [SerializeField] private float _colliderCooldownTime = 0.25f;
        SCR_DamageCollider _damageCollider;
        SCR_PlayerInteraction _playerInteraction;

        [Header("PLAYER STATISTICS")]
        [SerializeField] int _HP;
        [SerializeField] int _maxHP;
        public int HP { get => _HP; set => _HP = value; }
        void Start()
        {
            _inputManager = SCR_GeneralManager.PlayerInputManager;
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _boxCollider2D = GetComponent<BoxCollider2D>();

            _damageCollider = GetComponentInChildren<SCR_DamageCollider>();
            _playerInteraction = GetComponentInChildren<SCR_PlayerInteraction>();
            _damageCollider.gameObject.SetActive(false);
            _playerInteraction.gameObject.SetActive(false);

            _maxHP = _HP;
        }

        // Update is called once per frame
        void Update()
        {
            PlayerMovementUpdate();
            PlayerMeleeUpdate();
            PlayerInteractionUpdate();
        }

        #region Player Attacking

        private void PlayerMeleeUpdate()
        {
            //Do not attack if player is already attacking
            if (_playerAttacking) { return; }
            if (_inputManager.Attack.PressedThisFrame())
            {
                StartCoroutine(DamageColliderCoroutine());
            }

            IEnumerator DamageColliderCoroutine()
            {
                _playerAttacking = true;
                _damageCollider.gameObject.SetActive(true);
                yield return new WaitForSeconds(_colliderEnabledTime);
                _damageCollider.gameObject.SetActive(false);
                yield return new WaitForSeconds(_colliderCooldownTime);

                _playerAttacking = false;
            }
        }

        #endregion

        #region Player Interaction

        /// <summary>
        /// Update method used to trigger player interaction
        /// </summary>
        private void PlayerInteractionUpdate()
        {
            //Do not trigger if player is already attacking
            if (_playerAttacking) { return; }

            if (_inputManager.Vertical.PressedThisFrame() && _inputManager.Vertical.AxisValue > 0){
                StartCoroutine(InteractionColliderCoroutine());
            }

            IEnumerator InteractionColliderCoroutine()
            {
                _playerInteraction.gameObject.SetActive(true);
                yield return new WaitForSeconds(_colliderEnabledTime);
                _playerInteraction.gameObject.SetActive(false);
                yield return new WaitForSeconds(_colliderCooldownTime);

            }
        }
        #endregion



        #region Player Movement
        /// <summary>
        /// Method thats called every frame containing the logic and state for the player movmement.
        /// </summary>
        /// <remarks>
        /// Dependant on the enum state of _playerLevel
        /// </remarks>
        private void PlayerMovementUpdate()
        {
            switch (_playerLevel)
            {
                case PlayerLevel.WHOLE_LEVEL:
                    if (_inputManager.Horizontal.AxisValue < 0) { return; }
                    IntegerMovement(1);
                    break;

                case PlayerLevel.INTEGER_LEVEL:
                    IntegerMovement((int)_inputManager.Horizontal.AxisValue);
                    break;

                case PlayerLevel.RATIONAL_LEVEL:
                    RationalMovement();
                    break;

                case PlayerLevel.REAL_LEVEL:
                    RationalMovement();
                    JumpingPhysicsUpdate();
                    break;
            }
        }


        
        /// <summary>
        /// Method called every frame regarding jumping physics for the player character
        /// </summary>
        private void JumpingPhysicsUpdate()
        {
            Collider2D groundCollider = Physics2D.OverlapPoint(_boxCollider2D.bounds.center - new Vector3(0, _boxCollider2D.bounds.extents.y * 1.25f));
            _rigidbody2D.gravityScale = _playerJumpProperties.NormalGravity;

            if (groundCollider == null) { return; }

            if (_inputManager.Jump.PressedThisFrame()){
                _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _playerJumpProperties.JumpForce);
            }

            if (_inputManager.Jump.ReleasedThisFrame()){
                _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _playerJumpProperties.JumpDecay * _rigidbody2D.velocity.y);
            }

            
        }

        /// <summary>
        /// Grid-Based movement for player within early _playerState values. Calls local coroutine for grid movement only if player isn't currently moving.
        /// </summary>
        /// <param name="direction">Direction that the player is going</param>
        private void IntegerMovement(int direction)
        {

            if (_currentlyMoving) { return; }
            if (_inputManager.Horizontal.IsPressed()){
                StartCoroutine(GridMovement(direction));
            }

            IEnumerator GridMovement(int direction)
            {
                _currentlyMoving = true;
                Vector2 targetPosition = transform.position + (Vector3.right * direction);
                transform.localScale = direction < 0 ? new Vector3(-1, 1) : Vector3.one;

                while (Mathf.Abs(targetPosition.x - transform.position.x) > 0.01f)
                {
                    transform.position = Vector3.Lerp(transform.position, new Vector3(targetPosition.x, transform.position.y), _gridSpeed * Time.deltaTime);
                    yield return new WaitForEndOfFrame();
                }
                transform.position = new Vector3(Mathf.RoundToInt(targetPosition.x), transform.position.y);
                _currentlyMoving = false;
            }
        }

        /// <summary>
        /// Player moves with smooth acceleration and deceleration
        /// </summary>
        private void RationalMovement()
        {
            if (_inputManager.Horizontal.AxisValue != 0)
            {
                float maxHorizontalSpeed = _inputManager.Horizontal.AxisValue * _playerSpeedProperties.Speed * _playerSpeedProperties.SpeedMultiplier;
                _rigidbody2D.velocity = new Vector2(Mathf.Lerp(_rigidbody2D.velocity.x, maxHorizontalSpeed, Time.deltaTime * _playerSpeedProperties.Acceleration), _rigidbody2D.velocity.y);
            }
            else
            {
                _rigidbody2D.velocity = new Vector2(Mathf.Lerp(_rigidbody2D.velocity.x, 0, Time.deltaTime * _playerSpeedProperties.Deceleration), _rigidbody2D.velocity.y);
            }

            if (_inputManager.Horizontal.AxisValue > 0) { transform.localScale = Vector3.one; }
            else if (_inputManager.Horizontal.AxisValue < 0) { transform.localScale = new Vector3(-1, 1);}
        }

        #endregion

        [Serializable] class PlayerSpeedProperties
        {
            public float Speed;
            public float SpeedMultiplier;

            public float Acceleration;
            [Range(0, 1)] public float Deceleration = 0.75f;
        }

        [Serializable] class PlayerJumpProperties
        {
            public int JumpForce = 10;

            public int NormalGravity = 4;
            [Range(0,1)] public float JumpDecay = 0.5f;
        }

    }

    public enum PlayerLevel
    {
        WHOLE_LEVEL,
        INTEGER_LEVEL,
        RATIONAL_LEVEL,
        REAL_LEVEL
    }
}

