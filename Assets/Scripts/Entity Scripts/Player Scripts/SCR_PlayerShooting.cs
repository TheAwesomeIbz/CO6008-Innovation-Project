using Cinemachine;
using Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Entities.Player
{
    public class SCR_PlayerShooting : MonoBehaviour
    {
        SCR_PlayerInputManager _inputManager;

        [Header("PLAYER SHOOTING PROPERTIES")]
        [SerializeField] SO_WeaponProperties _weaponProperties;
        [SerializeField] Attackable _damageableTo;
        [SerializeField] GameObject _mouseCursor;
        [SerializeField] GameObject _halfwayObject;
        [SerializeField] [Range(1, 4)] float _midpointThreshold = 1;

        [Header("COOLDOWN PROPERTIES")]
        [SerializeField] float cooldown;
        [SerializeField] bool CanShoot => cooldown <= 0;

        [SerializeField] bool dialogueEnabled;

        [Header("CAMERA PROPERTIES")]
        [SerializeField] bool _trackPlayerCursor;
        [SerializeField] Vector3 relativeMousePosition;
        [SerializeField] float _cameraZoom;
        [SerializeField] float _mouseZoomMultiplier = 0.25f;
        [SerializeField] float _cameraZoomThreshold = 1;
        CinemachineVirtualCamera _virtualCamera;
        float _lensOrthoSize;

        public Attackable DamageableTo => _damageableTo;

        protected void Start()
        {
            _virtualCamera = Camera.main.VirtualCamera();
            _inputManager = SCR_GeneralManager.PlayerInputManager;
            _trackPlayerCursor = true;
            _lensOrthoSize = _virtualCamera.m_Lens.OrthographicSize;
        }

        private void OnEnable()
        {
            SCR_DialogueNPC.OnDialogueStart += OnDialogueStart;
            SCR_DialogueManager.OnDialogueEnd += OnDialogueEnd;
        }

        private void OnDialogueStart(SCR_DialogueNPC obj)
        {
            dialogueEnabled = true;
            _halfwayObject.transform.position = (transform.position + obj.transform.position) / 2;
        }

        private void OnDialogueEnd()
        {
            dialogueEnabled = false;
        }

        /// <summary>
        /// Update method called to register the cooldown and shooting functionality
        /// </summary>
        protected void ShootingUpdate()
        {
            if (_weaponProperties == null) { return; }
            if (dialogueEnabled) { return; }

            cooldown -= Time.deltaTime;
            cooldown = Mathf.Clamp(cooldown, 0, _weaponProperties.WeaponCooldown);

            if (_inputManager.LeftClick.IsPressed() && CanShoot)
            {
                float direction = Mathf.Atan2(relativeMousePosition.y, relativeMousePosition.x);
                _weaponProperties.SpawnBullet(new SO_WeaponProperties.BulletProperties
                {
                    ShootingObject = transform,
                    InputDirection = direction,

                });
                ResetCooldown();
            }
        }

        /// <summary>
        /// Update method called to register mouse movement and input
        /// </summary>
        private void MouseCursorUpdate()
        {
            if (_virtualCamera == null) { return; } 
            if (!_trackPlayerCursor) { return; }

            float mouseScrollDelta = Input.mouseScrollDelta.y;
            _cameraZoom += mouseScrollDelta * _mouseZoomMultiplier;
            _cameraZoom = Mathf.Clamp(_cameraZoom, -_cameraZoomThreshold, _cameraZoomThreshold);

            relativeMousePosition = _inputManager.CursorWorldPoint - transform.position;

            if (_mouseCursor)
            {
                _mouseCursor.transform.position = new Vector3(_inputManager.CursorWorldPoint.x, _inputManager.CursorWorldPoint.y, 0);
            }

            if (_halfwayObject)
            {
                Vector3 position = (relativeMousePosition / 2) + transform.position;
                _halfwayObject.transform.position = new Vector3(
                    Mathf.Clamp(position.x, transform.position.x - _midpointThreshold, transform.position.x + _midpointThreshold),
                    Mathf.Clamp(position.y, transform.position.y - _midpointThreshold, transform.position.y + _midpointThreshold));
            }

            _virtualCamera.m_Lens.OrthographicSize = _lensOrthoSize + _cameraZoom;
        }

        void Update()
        {
            MouseCursorUpdate();
            ShootingUpdate();
        }

        /// <summary>
        /// Self explanatory ¯\_(ツ)_/¯
        /// </summary>
        void ResetCooldown()
        {
            cooldown = _weaponProperties.WeaponCooldown;
        }
        private void OnDisable()
        {
            SCR_DialogueNPC.OnDialogueStart -= OnDialogueStart;
            SCR_DialogueManager.OnDialogueEnd -= OnDialogueEnd;
        }
    }
}

