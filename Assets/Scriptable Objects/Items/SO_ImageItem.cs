using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Inventory Items/Image Item")]
    public class SO_ImageItem : SO_Item, iUsableItem
    {
        [Header("IMAGE ITEM PROPERTIES")]
        [SerializeField] private Texture imageGraphic;
        [SerializeField] private string imageCaption;
        
        public Texture ImageGraphic => imageGraphic;
        public string ImageCaption => imageCaption;

        public void UseItem()
        {
            SCR_GeneralManager.UIManager.FindUIObject<UI_Menu>().InventoryUI.OnGraphicItemPressed(this);
        }
    }
}
