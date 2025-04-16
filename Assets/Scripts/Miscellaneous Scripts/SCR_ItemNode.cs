using System;
using System.Collections;
using System.Collections.Generic;
using Dialogue;
using UnityEngine;
using UnityEngine.UI;

namespace Overworld
{
    public class SCR_ItemNode : SCR_GraphNode, iInteractable
    {
        [Header("ITEM NODE PROPERTIES")]
        [SerializeField] private SO_Item overworldItem;
        [SerializeField] Sprite normalTexture, collectedTexture;
        [SerializeField] private bool mustCollectItem;
        SpriteRenderer spriteRenderer;
        private bool ItemAlreadyExists => SCR_GeneralManager.Instance.CollectedItems.Contains(name);
        public bool Interactable => !ItemAlreadyExists;
        SCR_DescriptiveObject descriptiveObject;

        public void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            descriptiveObject = GetComponent<SCR_DescriptiveObject>();
            spriteRenderer.sprite = Interactable ? normalTexture : collectedTexture;
            
        }
        
        public void Interact(object playerObject)
        {
            SCR_GeneralManager.Instance.CollectedItems.Add(name);
            SCR_GeneralManager.InventoryManager.AddItemWithDialogue(overworldItem);
            spriteRenderer.sprite = collectedTexture;
        }

        public override void OnPlayerLanded(SCR_PlayerOverworldMovement playerOverworldMovement)
        {
            if (Interactable){
                SCR_GeneralManager.UIManager.FindUIObject<UI_PlayerInputDisplay>().DisplayUI();
            }
            
        }

        public override bool ConditionalNode()
        {
            if (mustCollectItem)
            {
                return SCR_GeneralManager.InventoryManager.Inventory.Contains(overworldItem);
            }
            
            return true;
        }

        void Update()
        {
            if (!Interactable && descriptiveObject) { Destroy(descriptiveObject); }
            

        }
    }
}
