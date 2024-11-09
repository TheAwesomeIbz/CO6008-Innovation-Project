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
    public class SCR_PlayerShooting : MonoBehaviour, iAttackable
    {
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

        [Header("CAMERA ZOOM PROPERTIES")]
        [SerializeField] float _lensOrthoSize = 5;
        [SerializeField] float _cameraZoom;
        [SerializeField] float _mouseZoomMultiplier = 0.25f;
        [SerializeField] float _cameraZoomThreshold = 1;
        CinemachineVirtualCamera virtualCamera;

        public Attackable DamageableTo => _damageableTo;

        protected void Start()
        {
            virtualCamera = Camera.main.VirtualCamera();
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

        protected void ShootingUpdate()
        {
            CameraZoomUpdate();
            if (dialogueEnabled) { return; }
            
            Vector3 screenToWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 relativeMousePosition = new Vector3(screenToWorldPoint.x, screenToWorldPoint.y, 0) - transform.position;

            if (_mouseCursor)
            {
                _mouseCursor.transform.position = new Vector3(screenToWorldPoint.x, screenToWorldPoint.y, 0);
            }

            if (_halfwayObject)
            {
                Vector3 position = (relativeMousePosition / 2) + transform.position;
                _halfwayObject.transform.position = new Vector3(
                    Mathf.Clamp(position.x, transform.position.x - _midpointThreshold, transform.position.x + _midpointThreshold),
                    Mathf.Clamp(position.y, transform.position.y - _midpointThreshold, transform.position.y + _midpointThreshold));
            }


            if (Input.GetMouseButton(0) && CanShoot)
            {
                float direction = Mathf.Atan2(relativeMousePosition.y, relativeMousePosition.x);
                _weaponProperties.SpawnBullet(transform, this, direction);
                ResetCooldown();
            }
        }

        private void CameraZoomUpdate()
        {
            float mouseScrollDelta = Input.mouseScrollDelta.y;
            _cameraZoom += mouseScrollDelta * _mouseZoomMultiplier;
            _cameraZoom = Mathf.Clamp(_cameraZoom, -_cameraZoomThreshold, _cameraZoomThreshold);

        virtualCamera.m_Lens.OrthographicSize = _lensOrthoSize + _cameraZoom;
        }

        void Update()
        {
            cooldown -= Time.deltaTime;
            cooldown = Mathf.Clamp(cooldown, 0, _weaponProperties.WeaponCooldown);
            ShootingUpdate();
        }

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

