using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class SCR_InventoryUI : MonoBehaviour
    {
        [field : Header("INVENTORY UI PROPERTIES")]
        [field : SerializeField] public bool InventoryEnabled { get; private set; }
        [SerializeField] GameObject _panelObject;
        [SerializeField] GameObject _title;
        [SerializeField] Transform _hoverParentObject;
        [SerializeField] RawImage _selectedItemObject;

        HoverUIObject _hoverUIObject;
        iUsableItem _usableItem;
        SCR_InventoryManager _inventoryManager;
        SCR_InventoryUI_Slot[] _inventoryUISlots;

        void Start()
        {
            _hoverUIObject = new HoverUIObject(_hoverParentObject);
            InventoryEnabled = false;
            _inventoryManager = SCR_GeneralManager.InventoryManager;

            _panelObject.SetActive(false);
            _title.SetActive(false);
            _hoverParentObject.gameObject.SetActive(false);
            _selectedItemObject.gameObject.SetActive(false);
            _inventoryUISlots = new SCR_InventoryUI_Slot[32];
        }
        void Update()
        {
            _hoverUIObject.SetMousePosition();
            InventoryButtonUpdate();
            InventorySlotUpdate();
        }


        /// <returns>Whether the inventory button can be pressed under varying conditions</returns>
        private bool InventoryButtonPressed()
        {
            return Input.GetKeyDown(KeyCode.F1);
        }

        /// <summary>
        /// Every time the menu button is pressed under the proper conditions, then this will display all the Inventory items
        /// </summary>
        public void UpdateUI()
        {
            for (int i = 0; i < _inventoryUISlots.Length; i++)
            {
                _inventoryUISlots[i] = _panelObject.transform.GetChild(i).GetComponent<SCR_InventoryUI_Slot>();

                SO_Item inventoryItem = i < _inventoryManager.Inventory.Count ? _inventoryManager.Inventory[i] : null;
                _inventoryUISlots[i].InitializeSlot(inventoryItem);
            }
        }

        /// <summary>
        /// Function called whenever a slot is hovered over within the Inventory Panel
        /// </summary>
        /// <param name="slot"></param>
        public void OnSlotHovered(SCR_InventoryUI_Slot slot)
        {
            if (!slot.Item) { return; }

            _usableItem = slot.Item as iUsableItem;
            _hoverParentObject.gameObject.SetActive(true);
            _hoverUIObject.SetItemText(slot.Item.SpriteName, slot.Item.SpriteDescription, _usableItem != null);
        }

        /// <summary>
        /// Function called whenever the mouse cursor has stopped hovering within the Inventory Panel
        /// </summary>
        public void OnSlotUnHovered()
        {
            _hoverParentObject.gameObject.SetActive(false);
            _usableItem = null;
        }


        /// <summary>
        /// Update() method that is responsible for enabling and disabling the inventory menu
        /// </summary>
        private void InventoryButtonUpdate()
        {
            if (!InventoryButtonPressed()) { return; }

            InventoryEnabled = !InventoryEnabled;
            _panelObject.SetActive(InventoryEnabled);
            _title.SetActive(InventoryEnabled);

            if (InventoryEnabled)
            {
                UpdateUI();
                
            }
            else
            {
                _hoverParentObject.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Update() method that is responsible for using usable items within the inventory menu
        /// </summary>
        private void InventorySlotUpdate()
        {
            if (!InventoryEnabled) { return; }

            if (Input.GetMouseButtonDown(1) && _usableItem != null)
            {
                _usableItem.UseItem();
            }
        }

        /// <summary>
        /// Local model class that dynamically allows for easy modification for the hover UI object
        /// </summary>
        [Serializable] class HoverUIObject
        {
            private Transform _parentObject;
            private TextMeshProUGUI _itemName;
            private TextMeshProUGUI _itemDescription;
            private GameObject _itemUsableText;

            /// <summary>
            /// Sets the Hover UI object text and description. DIsplays graphic whether item is usable or not
            /// </summary>
            /// <param name="itemName"></param>
            /// <param name="itemDescription"></param>
            /// <param name="itemUsable"></param>
            public void SetItemText(string itemName, string itemDescription, bool itemUsable)
            {
                _itemName.text = itemName;
                _itemDescription.text = itemDescription;
                _itemUsableText.SetActive(itemUsable);
            }

            public HoverUIObject(Transform parentObject)
            {
                _parentObject = parentObject;
                _itemName = parentObject.GetChild(0).GetComponent<TextMeshProUGUI>();
                _itemDescription = parentObject.GetChild(1).GetComponent<TextMeshProUGUI>();
                _itemUsableText = parentObject.GetChild(2).gameObject;
            }

            /// <summary>
            /// Sets the parent object anchored position to the world cursor point of the mouse
            /// </summary>
            public void SetMousePosition()
            {
                _parentObject.transform.position = SCR_GeneralManager.PlayerInputManager.CursorWorldPoint;
            }
        }
    }
}
