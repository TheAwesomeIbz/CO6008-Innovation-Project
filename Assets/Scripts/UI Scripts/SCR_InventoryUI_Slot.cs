using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class SCR_InventoryUI_Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("INVENTORY SLOT PROPERTIES")]
        [SerializeField] private RawImage _iconTexture;
        private SCR_InventoryUI _inventoryUIReference;
        private SO_Item _item;
        private Button _button;
        public SO_Item Item => _item;
        void Start()
        {
            _inventoryUIReference = SCR_GeneralManager.UIManager.FindUIObject<SCR_InventoryUI>();
            _button = GetComponent<Button>();
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            _inventoryUIReference.OnSlotHovered(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _inventoryUIReference.OnSlotUnHovered();
        }

        /// <summary>
        /// Initialize the item within the slot and displays the proper graphics.
        /// </summary>
        /// <param name="item"></param>
        public void InitializeSlot(SO_Item item)
        {
            _item = item;
            _iconTexture.gameObject.SetActive(item != null);

            if (_button != null)
            {
                _button.interactable = item != null;
                if (item != null)
                {
                    _iconTexture.texture = item.SpriteIcon;
                }
            }
            
        }
    }
}
