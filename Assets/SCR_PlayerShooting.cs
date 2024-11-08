using Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Entities.Player
{
    public class SCR_PlayerShooting : SCR_EntityShooting
    {
        [Header("PLAYER SHOOTING PROPERTIES")]
        [SerializeField] GameObject _mouseCursor;
        [SerializeField] GameObject _halfwayObject;
        [SerializeField] [Range(1, 4)] float _midpointThreshold = 1;

        bool dialogueEnabled;


        protected override void Start()
        {
            
            base.Start();
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

        protected override void ShootingUpdate()
        {
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
                _weaponProperties.SpawnBullet(transform, direction);
                ResetCooldown();
            }
        }

        private void OnDisable()
        {
            SCR_DialogueNPC.OnDialogueStart -= OnDialogueStart;
            SCR_DialogueManager.OnDialogueEnd -= OnDialogueEnd;
        }
    }
}


namespace Entities
{
    public class SCR_EntityShooting : MonoBehaviour
    {
        [Header("GENERAL SHOOTING PROPERTIES")]
        [SerializeField] protected SO_WeaponProperties _weaponProperties;

        protected float cooldown;
        protected bool CanShoot => cooldown <= 0;

        /// <summary>
        /// Calculate the angle of a given transform, relative to the current transform
        /// </summary>
        /// <param name="transform"></param>
        /// <returns>The angle the input transform makes to the current object</returns>
        protected float AngleRelativeTo(Transform transform)
        {
            Vector3 relativePosition = this.transform.position - transform.position;
            return Mathf.Atan2(relativePosition.y, relativePosition.x);
        }
        protected virtual void ShootingUpdate() { }
        protected void ResetCooldown()
        {
            cooldown = _weaponProperties.WeaponCooldown;
        }
        protected virtual void Start()
        {

        }

        void Update()
        {
            cooldown -= Time.deltaTime;
            cooldown = Mathf.Clamp(cooldown, 0, _weaponProperties.WeaponCooldown);
            ShootingUpdate();
        }
    }
}

