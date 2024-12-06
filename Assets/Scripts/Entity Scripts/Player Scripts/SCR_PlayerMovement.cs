using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Entities.Player
{
    public class SCR_PlayerMovement : MonoBehaviour, iDodgeable
    {
        public const int MaximumPlayerSpeed = 15;
        [Header("PLAYER MOVEMENT PROPERTIES")]
        [SerializeField] PlayerLevel _playerLevel;
        [SerializeField] bool _moveIn2DSpace;

        [Header("PLAYER GRID MOVEMENT PROPERTIES")]
        [SerializeField] private bool _currentlyMoving;
        [SerializeField] private float _gridSpeed;
        [SerializeField] [Range(1,5)] private int _gridDisplacement = 1;

        [Header("PLAYER SPEED PROPERTIES")]
        [SerializeField] PlayerSpeedProperties _playerSpeedProperties;

        [Header("PLAYER DODGE PROPERTIES")]
        [SerializeField] PlayerDodgeProperties _playerDodgeProperties;

        [Header("PLAYER POWERUP PROPERTIES")]
        [SerializeField] PlayerPowerupProperty _damagePowerupProperty;
        [SerializeField] PlayerPowerupProperty _knockbackPowerupProperty;
        [SerializeField] PlayerPowerupProperty _agilityPowerupProperty;

        public PlayerPowerupProperty DamagePowerupProperty => _damagePowerupProperty;
        public PlayerPowerupProperty KnockbackPowerupProperty => _knockbackPowerupProperty;
        public PlayerPowerupProperty AgilityPowerupProperty => _agilityPowerupProperty;

        SCR_PlayerInteraction _playerInteraction;
        public CMP_HitboxComponent HitboxComponent { get; private set; }
        SCR_PlayerInputManager _inputManager;
        public Rigidbody2D Rigidbody2D { get; private set; }
        public BoxCollider2D BoxCollider2D { get; private set; }
        public bool IsDodging => _playerDodgeProperties.IsDodging;
        public SO_WeaponProperties WeaponProperties { get; private set; }
        /// <summary>
        /// Get the Current Player level
        /// </summary>
        public PlayerLevel PlayerLevel => _playerLevel;
        void Start()
        {
            _inputManager = SCR_GeneralManager.PlayerInputManager;
            Rigidbody2D = GetComponent<Rigidbody2D>();
            BoxCollider2D = GetComponent<BoxCollider2D>();

            _playerInteraction = GetComponentInChildren<SCR_PlayerInteraction>();
            HitboxComponent = GetComponentInChildren<CMP_HitboxComponent>();
            WeaponProperties = GetComponent<SCR_PlayerShooting>().WeaponProperties;
            //_playerInteraction.gameObject.SetActive(false);

            HitboxComponent.OnDamageEvent += OnDamageEvent;
            HitboxComponent.OnZeroHPEvent += OnZeroHPEvent;
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

            if (damageCollider?.StunTimer > 0)
            {
                StartCoroutine(StunTimerCoroutine());
            }
            StartCoroutine(SpriteFlickerCoroutine());

            IEnumerator StunTimerCoroutine()
            {
                SCR_PlayerInputManager.PlayerControlsEnabled = false;
                HitboxComponent.gameObject.SetActive(false);
                yield return new WaitForSeconds(damageCollider.StunTimer);
                HitboxComponent.gameObject.SetActive(true);
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
            DamagePowerupProperty.PowerupUpdate();
            AgilityPowerupProperty.PowerupUpdate();
            KnockbackPowerupProperty.PowerupUpdate();

            PlayerMovementUpdate();
            //PlayerInteractionUpdate();
        }


        #region Player Interaction

        /// <summary>
        /// Update method used to trigger player interaction
        /// </summary>
        //private void PlayerInteractionUpdate()
        //{

        //    if (_inputManager.Submit.PressedThisFrame())
        //    {
        //        StartCoroutine(InteractionColliderCoroutine());
        //    }

        //    IEnumerator InteractionColliderCoroutine()
        //    {
        //        _playerInteraction.gameObject.SetActive(true);
        //        yield return new WaitForSeconds(0.1f);
        //        _playerInteraction.gameObject.SetActive(false);
        //        yield return new WaitForSeconds(0.1f);

        //    }
        //}
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
            Vector2 directionVector = _inputManager.Axis2D.AxisValue;
            switch (_playerLevel)
            {
                case PlayerLevel.WHOLE_LEVEL:
                    if (directionVector.x < 0) { return; }
                    IntegerMovement(1);
                    break;

                case PlayerLevel.INTEGER_LEVEL:
                    IntegerMovement((int)directionVector.x);
                    break;

                case PlayerLevel.RATIONAL_LEVEL:
                    RationalMovement();
                    break;

                case PlayerLevel.REAL_LEVEL:
                    RationalMovement();
                    DodgingPhysics();
                    break;
            }
        }

        /// <summary>
        /// Grid-Based movement for player within early _playerState values. Calls local coroutine for grid movement only if player isn't currently moving.
        /// </summary>
        /// <param name="direction">Direction that the player is going</param>
        private void IntegerMovement(int direction)
        {
            Rigidbody2D.velocity = Vector2.zero;
            if (_currentlyMoving) { return; }
            if (_inputManager.Axis2D.IsPressed()){
                StopAllCoroutines();
                StartCoroutine(GridMovement(direction));
            }

            IEnumerator GridMovement(int direction)
            {
                _currentlyMoving = true;
                Vector2 currentPosition = transform.position;
                Vector2 targetPosition = transform.position + (Vector3.right * direction * _gridDisplacement);
                transform.localScale = direction < 0 ? new Vector3(-1, 1) : Vector3.one;

                RaycastHit2D boundaryRaycast = Physics2D.Raycast(currentPosition, new Vector2(direction, 0), _gridDisplacement + 1, GlobalMasks.BoundaryLayerMask);

                while (Mathf.Abs(targetPosition.x - transform.position.x) > 0.1f && boundaryRaycast.collider == null)
                {
                    transform.position = Vector3.Lerp(transform.position, new Vector3(targetPosition.x, transform.position.y), _gridSpeed * Time.deltaTime);
                    boundaryRaycast = Physics2D.Raycast(currentPosition, new Vector2(direction, 0), _gridDisplacement, GlobalMasks.BoundaryLayerMask);
                    yield return new WaitForEndOfFrame();
                }

                if (boundaryRaycast.collider != null) {
                    transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), transform.position.y);
                    _currentlyMoving = false;
                    yield break; 
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
            Vector2 directionVector = _inputManager.Axis2D.AxisValue;
            if (!_moveIn2DSpace) { directionVector.y = 0; }
            if (_playerDodgeProperties.IsDodging) { return; }
            if (_inputManager.Axis2D.IsPressed())
            {
                Vector2 MaxSpeed = directionVector * _playerSpeedProperties.Speed * _agilityPowerupProperty.PowerupMultiplier;
               
                Rigidbody2D.velocity = Vector2.Lerp(Rigidbody2D.velocity, MaxSpeed, Time.deltaTime * _playerSpeedProperties.Acceleration);   
            }
            else
            {
                Rigidbody2D.velocity = Vector2.Lerp(Rigidbody2D.velocity, Vector2.zero, Time.deltaTime * _playerSpeedProperties.Deceleration * _agilityPowerupProperty.PowerupMultiplier);
            }

            if (directionVector.x > 0) { transform.localScale = Vector3.one; }
            else if (directionVector.x < 0) { transform.localScale = new Vector3(-1, 1);}
        }

        /// <summary>
        /// Player dodges in direction that is pressed with a given impulse and delay
        /// </summary>
        private void DodgingPhysics()
        {
            Vector2 directionVector = _inputManager.Axis2D.AxisValue;
            if (!_moveIn2DSpace) {  directionVector.y = 0; }
            if (_inputManager.Dodge.PressedThisFrame() && directionVector.magnitude != 0 && !_playerDodgeProperties.IsDodging)
            {
                StartCoroutine(DodgeCoroutine());
            }


            IEnumerator DodgeCoroutine()
            {
                _playerDodgeProperties.IsDodging = true;

                float timer = 0;
                while (timer < _playerDodgeProperties.DodgeDelay)
                {

                    Rigidbody2D.velocity = directionVector * _playerDodgeProperties.DodgeImpulse;
                    timer += Time.deltaTime;
                    yield return null;
                }
                Rigidbody2D.velocity = directionVector * _playerSpeedProperties.Speed;

                yield return new WaitForSeconds(0.25f);
                _playerDodgeProperties.IsDodging = false;
                
            }
        }

        #endregion

        private void OnDisable()
        {
            HitboxComponent.OnDamageEvent -= OnDamageEvent;
            HitboxComponent.OnZeroHPEvent -= OnZeroHPEvent;
        }

        #region PLAYER MOVEMENT PROPERTY OBJECTS
        [Serializable] class PlayerSpeedProperties
        {
            public float Speed = 5;
            public float SpeedMultiplier = 1;

            public float Acceleration = 2;
            [Range(0, 1)] public float Deceleration = 0.75f;
        }

        [Serializable] class PlayerDodgeProperties
        {
            [Range(5, 20)] public int DodgeImpulse = 10;
            public bool IsDodging;
            [Range(0.25f, 2)] public float DodgeDelay = 0.5f;
        }

        /// <summary>
        /// Property class used to store information about an abstract player powerup
        /// </summary>
        /// <remarks>Used for damage, agility etc.</remarks>
        [Serializable] public class PlayerPowerupProperty
        {
            const int PowerupDurationConstant = 30;
            [Range(1,2)] private float _powerupMultiplier = 1;
            [SerializeField] private float _powerupDuration;
            public float PowerupDuration => _powerupDuration;
            public float PowerupMultiplier => _powerupMultiplier;
            public bool PowerupActive => _powerupDuration > 0;

            /// <summary>
            /// Sets the multiplier of the powerup
            /// </summary>
            /// <param name="multiplier"></param>
            public void SetPowerupMultiplier(float multiplier) {
                _powerupMultiplier = multiplier;
                _powerupDuration = PowerupDurationConstant;
            }
            /// <summary>
            /// Function called in Update() to update the property duration timer every frame
            /// </summary>
            public void PowerupUpdate() => _powerupDuration -= _powerupDuration >= 0 ? Time.deltaTime : 0;
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

