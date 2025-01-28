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
        [SerializeField] RawImage _selectedItemObject;

        iUsableItem _usableItem;
        SCR_InventoryManager _inventoryManager;
        UI_InventoryUI_Slot[] _inventoryUISlots;

        void Start()
        {
            InventoryEnabled = false;
            _inventoryManager = SCR_GeneralManager.InventoryManager;

            _panelObject.SetActive(false);
            _title.SetActive(false);
            _selectedItemObject.gameObject.SetActive(false);
            _inventoryUISlots = new UI_InventoryUI_Slot[32];
        }
        void Update()
        {
            InventoryButtonUpdate();
            //InventorySlotUpdate();
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
                _inventoryUISlots[i] = _panelObject.transform.GetChild(i).GetComponent<UI_InventoryUI_Slot>();

                SO_Item inventoryItem = i < _inventoryManager.Inventory.Count ? _inventoryManager.Inventory[i] : null;
                _inventoryUISlots[i].InitializeSlot(inventoryItem);
            }
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
        }

        ///// <summary>
        ///// Update() method that is responsible for using usable items within the inventory menu
        ///// </summary>
        //private void InventorySlotUpdate()
        //{
        //    if (!InventoryEnabled) { return; }

        //    if (Input.GetMouseButtonDown(0) && SCR_PlayerInputManager.PlayerControlsEnabled && _usableItem != null)
        //    {
        //        _usableItem.UseItem();
        //    }
        ///}

        
    }
}
