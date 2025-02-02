using Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace UnityEngine.UI
{
    public sealed class UI_DescriptionUI : MonoBehaviour
    {
        SCR_PlayerInputManager playerInputManager;
        [SerializeField] private Transform _parentObject;
        IDescriptive descriptiveObject;
        private TextMeshProUGUI _objectHeader;
        private TextMeshProUGUI _objectDescription;
        private TextMeshProUGUI _objectUsageText;

        private bool dialogueManagerEnabled;

        Vector2 centerPivot = new Vector2(0.5f, 0.5f);
        const float offsetValue = 0.5625f;
        RectTransform rectTransform;

        void Start()
        {
            _objectHeader = _parentObject.GetChild(0).GetComponent<TextMeshProUGUI>();
            _objectDescription = _parentObject.GetChild(1).GetComponent<TextMeshProUGUI>();
            _objectUsageText = _parentObject.GetChild(2).GetComponent<TextMeshProUGUI>();
            playerInputManager = SCR_GeneralManager.PlayerInputManager;
            rectTransform = _parentObject.GetComponent<RectTransform>();

            SCR_DialogueManager.OnDialogueStartEvent += OnDialogueStartEvent;
            SCR_DialogueManager.OnDialogueEndEvent += OnDialogueEndEvent;
        }

        private void OnDialogueEndEvent()
        {
            dialogueManagerEnabled = false;
        }

        private void OnDialogueStartEvent(DialogueObject[] obj)
        {
            dialogueManagerEnabled = true;
        }

        public void SetDescriptiveObject(IDescriptive descriptiveObject) => this.descriptiveObject = descriptiveObject;

        /// <summary>
        /// Sets the Hover UI object text and description. DIsplays graphic whether item is usable or not
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="itemDescription"></param>
        /// <param name="itemUsable"></param>
        private void SetUIInformation(IDescriptive descriptive)
        {
            _objectHeader.text = descriptive.Header;
            _objectDescription.text = descriptive.Description;
            _objectUsageText.text = descriptive.UsageDesctiption;

            if (Input.GetMouseButtonDown(0) && !string.IsNullOrWhiteSpace(descriptive.UsageDesctiption))
            {
                descriptive.OnOptionUsed();
            }
        }


        /// <summary>
        /// Sets the parent object anchored position to the world cursor point of the mouse
        /// </summary>
        private void SetMousePosition()
        {
            _parentObject.transform.position = SCR_GeneralManager.PlayerInputManager.CursorWorldPoint;
           
        }


        private void FindAllContacts()
        {
            if (EventSystems.EventSystem.current.IsPointerOverGameObject()) { 
                return; 
            }

            IDescriptive descriptive = null;

            foreach (Collider2D collider in playerInputManager.CollidedWithMouse)
            {
                if (collider == null) { continue; }
                descriptive = collider.GetComponent<IDescriptive>();
            }
            descriptiveObject = descriptive;
        }

        private void UpdatePosition()
        {
            Vector2 offset = new Vector2(-offsetValue, offsetValue);

            if (Input.mousePosition.x > Screen.width / 4)
            {
                offset.x = offsetValue;
            }
            if (Input.mousePosition.y < Screen.height / 4)
            {
                offset.y = -offsetValue;
            }

            rectTransform.pivot = centerPivot + offset;


        }

        void Update()
        {
            if (dialogueManagerEnabled) { 
                _parentObject.gameObject.SetActive(false);
                return; 
            }

            UpdatePosition();
            FindAllContacts();
            if (descriptiveObject != null && descriptiveObject.GameObject.activeInHierarchy)
            {
                SetMousePosition();
                SetUIInformation(descriptiveObject);
                _parentObject.gameObject.SetActive(true);
            }
            else { _parentObject.gameObject.SetActive(false); }
        }

        private void OnDisable()
        {
            SCR_DialogueManager.OnDialogueStartEvent -= OnDialogueStartEvent;
            SCR_DialogueManager.OnDialogueEndEvent -= OnDialogueEndEvent;
        }
        public interface IDescriptive
        {
            public GameObject GameObject { get; }
            public string Header { get; }
            public string Description { get; }
            public string UsageDesctiption { get; }
            public void OnOptionUsed();
        }
    }
}
