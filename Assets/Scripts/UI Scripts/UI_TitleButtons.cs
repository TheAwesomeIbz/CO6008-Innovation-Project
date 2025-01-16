using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Title
{
    public class UI_TitleButtons : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("TITLE BUTTON PROPERTIES")]
        [SerializeField] [TextArea(2,2)] protected string buttonDescription;
        UI_TitleUI _titleUI;

        public void SetDescription(string description) => buttonDescription = description;
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (!_titleUI) { return; }
            _titleUI.UpdateHoverUI(name, buttonDescription);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_titleUI) { return; }
            _titleUI.UpdateHoverUI("", "");
        }

        // Start is called before the first frame update
        void Start()
        {
            _titleUI = SCR_GeneralManager.UIManager.FindUIObject<UI_TitleUI>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
