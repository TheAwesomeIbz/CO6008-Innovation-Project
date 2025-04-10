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
        [SerializeField] private GameObject _panelObject;
        [SerializeField] private Transform inventorySlotParent;
        [SerializeField] private UI_Menu menu;

        SCR_InventoryManager _inventoryManager;
        UI_InventoryUI_Slot[] _inventoryUISlots;

        [Header("INVENTORY GRAPHIC IMAGE DISPLAY")] 
        [SerializeField] private GameObject parentGraphicGameObject;
        [SerializeField] private TextMeshProUGUI imageText;
        [SerializeField] private RawImage image;
        [SerializeField] private TextMeshProUGUI imageCaption;

        

        void Start()
        {
            InventoryEnabled = false;
            _inventoryManager = SCR_GeneralManager.InventoryManager;

            _inventoryUISlots = new UI_InventoryUI_Slot[16];
            gameObject.SetActive(false);
            parentGraphicGameObject.SetActive(false);
        }

        /// <returns>Whether the inventory button can be pressed under varying conditions</returns>

        /// <summary>
        /// Every time the menu button is pressed under the proper conditions, then this will display all the Inventory items
        /// </summary>
        public void UpdateUI()
        {
            for (int i = 0; i < _inventoryUISlots.Length; i++)
            {
                _inventoryUISlots[i] = inventorySlotParent.GetChild(i).GetComponent<UI_InventoryUI_Slot>();

                SO_Item inventoryItem = i < _inventoryManager.Inventory.Count ? _inventoryManager.Inventory[i] : null;
                _inventoryUISlots[i].InitializeSlot(inventoryItem);
            }
        }


        /// <summary>
        /// Method responsible for opening the inventory.
        /// </summary>
        public void OpenInventory()
        {
            InventoryEnabled = true;
            gameObject.SetActive(true);
            UpdateUI();
        }

        public void OnReturnButtonPressed()
        {
            InventoryEnabled = false;
            gameObject.SetActive(false);
        }


        public void OnGraphicItemPressed(SO_ImageItem item)
        {
            imageText.text = item.name;
            imageCaption.text = item.ImageCaption;
            image.texture = item.ImageGraphic;
            parentGraphicGameObject.SetActive(true);
        }

        public void OnGraphicReturnButtonPressed()
        {
            parentGraphicGameObject.SetActive(false);
        }

        
    }
}
