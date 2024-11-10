using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Entities.Player
{
    public class SCR_PlayerMovement : MonoBehaviour
    {
        public const int MaximumPlayerSpeed = 50;
        [Header("PLAYER MOVEMENT PROPERTIES")]
        [SerializeField] PlayerLevel _playerLevel;

        [Header("PLAYER GRID MOVEMENT PROPERTIES")]
        [SerializeField] private bool _currentlyMoving;
        [SerializeField] private float _gridSpeed;

        [Header("PLAYER SPEED PROPERTIES")]
        [SerializeField] PlayerSpeedProperties _playerSpeedProperties;

        [Header("PLAYER JUMP PROPERTIES")]
        [SerializeField] PlayerJumpProperties _playerJumpProperties;

        SCR_PlayerInteraction _playerInteraction;
        CMP_HitboxComponent _HitboxComponent;
        SCR_PlayerInputManager _inputManager;
        Rigidbody2D _rigidbody2D;
        BoxCollider2D _boxCollider2D;
        /// <summary>
        /// Get the Current Player level
        /// </summary>
        public PlayerLevel PlayerLevel => _playerLevel;
        void Start()
        {
            _inputManager = SCR_GeneralManager.PlayerInputManager;
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _boxCollider2D = GetComponent<BoxCollider2D>();

            _playerInteraction = GetComponentInChildren<SCR_PlayerInteraction>();
            _HitboxComponent = GetComponentInChildren<CMP_HitboxComponent>();
            _playerInteraction.gameObject.SetActive(false);

            _HitboxComponent.OnDamageEvent += OnDamageEvent;
            _HitboxComponent.OnZeroHPEvent += OnZeroHPEvent;
        }

        /// <summary>
        /// Called when the player's HP reaches zero
        /// </summary>
        /// <param name="damageCollider"></param>
        private void OnZeroHPEvent(SCR_DamageCollider damageCollider)
        {
            //TODO : Disable player collider and display game over screen
        }

        /// <summary>
        /// Called when the player received damage from any damage collider
        /// </summary>
        /// <param name="damageCollider"></param>
        private void OnDamageEvent(SCR_DamageCollider damageCollider)
        {
            StopCoroutine(StunTimerCoroutine());
            StopCoroutine(SpriteFlickerCoroutine());

            if (damageCollider.StunTimer > 0)
            {
                StartCoroutine(StunTimerCoroutine());
            }
            StartCoroutine(SpriteFlickerCoroutine());

            IEnumerator StunTimerCoroutine()
            {
                SCR_PlayerInputManager.PlayerControlsEnabled = false;
                _HitboxComponent.gameObject.SetActive(false);
                yield return new WaitForSeconds(damageCollider.StunTimer);
                _HitboxComponent.gameObject.SetActive(true);
                SCR_PlayerInputManager.PlayerControlsEnabled = true;
            }
            IEnumerator SpriteFlickerCoroutine()
            {
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                for (int i = 0; i < 4; i++)
                {
                    spriteRenderer.enabled = false;
                    yield return new WaitForSeconds(0.125f);
                    spriteRenderer.enabled = true;
                    yield return new WaitForSeconds(0.125f);
                }
            }
        }

        void Update()
        {
            PlayerMovementUpdate();
            PlayerInteractionUpdate();
        }


        #region Player Interaction

        /// <summary>
        /// Update method used to trigger player interaction
        /// </summary>
        private void PlayerInteractionUpdate()
        {

            if (_inputManager.Vertical.PressedThisFrame() && _inputManager.Vertical.AxisValue > 0){
                StartCoroutine(InteractionColliderCoroutine());
            }

            IEnumerator InteractionColliderCoroutine()
            {
                _playerInteraction.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                _playerInteraction.gameObject.SetActive(false);
                yield return new WaitForSeconds(0.1f);

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
            Collider2D groundCollider = Physics2D.OverlapPoint(_boxCollider2D.bounds.center - new Vector3(0, _boxCollider2D.bounds.extents.y * 1.1f), GlobalMasks.GroundLayerMask);
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

        private void OnDisable()
        {
            _HitboxComponent.OnDamageEvent -= OnDamageEvent;
            _HitboxComponent.OnZeroHPEvent -= OnZeroHPEvent;
        }

        #region PLAYER MOVEMENT PROPERTY OBJECTS
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

        #endregion

    }

    public enum PlayerLevel
    {
        WHOLE_LEVEL,
        INTEGER_LEVEL,
        RATIONAL_LEVEL,
        REAL_LEVEL
    }
}

