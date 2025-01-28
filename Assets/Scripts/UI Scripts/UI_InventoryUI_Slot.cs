using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class UI_InventoryUI_Slot : UI_DescriptiveObject, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("INVENTORY SLOT PROPERTIES")]
        [SerializeField] private RawImage _iconTexture;
        private SO_Item _item;
        private Button _button;
        public SO_Item Item => _item;
        protected override void Start()
        {
            _button = GetComponent<Button>();
            base.Start();
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

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (Item == null) { return; }
            base.OnPointerEnter(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (Item == null) { return; }
            base.OnPointerExit(eventData);
        }
        public override void OnOptionUsed()
        {
            if (Item == null) { return; }
            if (Item is iUsableItem)
            {
                iUsableItem iUsableItem = (iUsableItem)Item;
                iUsableItem.UseItem();
            }
        }

        private void Update()
        {
            if (Item == null) { return; }
            header = Item.name;
            description = Item.SpriteDescription;
            usageDescription = Item is iUsableItem ? defaultUsageString : string.Empty;
        }
    }
}
