using System;
using System.Collections;
using System.Collections.Generic;
using Dialogue;
using UnityEngine;

namespace Overworld
{
    public class SCR_ItemNode : SCR_GraphNode, iInteractable
    {
        [SerializeField] private SO_Item overworldItem;
        private bool ItemAlreadyExists => SCR_GeneralManager.Instance.CollectedItems.Contains(name);
        public bool Interactable => !ItemAlreadyExists;
        SCR_DescriptiveObject descriptiveObject;

        private void Start()
        {
            descriptiveObject = GetComponent<SCR_DescriptiveObject>();
        }
        public void Interact(object playerObject)
        {
            SCR_GeneralManager.Instance.CollectedItems.Add(name);
            SCR_GeneralManager.InventoryManager.AddItemWithDialogue(overworldItem);
        }

        void Update()
        {
            if (!Interactable && descriptiveObject) { Destroy(descriptiveObject); }
        }
    }
}
