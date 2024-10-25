using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Level.Player
{
    public class SCR_PlayerMovement : MonoBehaviour
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

        [Header("PLAYER SPEED PROPERTIES")]
        [SerializeField] PlayerSpeedProperties _playerSpeedProperties;

        [Header("PLAYER JUMP PROPERTIES")]
        [SerializeField] PlayerJumpProperties _playerJumpProperties;
        void Start()
        {
            _inputManager = SCR_GeneralManager.PlayerInputManager;
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
        }

        // Update is called once per frame
        void Update()
        {
            JumpingPhysics();

            if (_inputManager.Horizontal.AxisValue != 0){
                AccelertatePlayer();
            }
            else{
                DecceleratePlayer();
            }
        }



        private void JumpingPhysics()
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
        private void AccelertatePlayer()
        {
            float maxHorizontalSpeed = _inputManager.Horizontal.AxisValue * _playerSpeedProperties.Speed * _playerSpeedProperties.SpeedMultiplier;
            _rigidbody2D.velocity = new Vector2(Mathf.Lerp(_rigidbody2D.velocity.x, maxHorizontalSpeed, Time.deltaTime * _playerSpeedProperties.Acceleration), _rigidbody2D.velocity.y);
        }
        private void DecceleratePlayer()
        {
            _rigidbody2D.velocity = new Vector2(Mathf.Lerp(_rigidbody2D.velocity.x, 0, Time.deltaTime * _playerSpeedProperties.Deceleration), _rigidbody2D.velocity.y);
        }


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

