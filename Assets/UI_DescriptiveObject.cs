using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace UnityEngine.UI
{
    public class UI_DescriptiveObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, UI_DescriptionUI.IDescriptive
    {
        protected const string defaultUsageString = "LEFT CLICK TO SELECT OPTION";

        UI_DescriptionUI descriptionUI;
        [Header("DESCRIPTIVE PROPERTIES")]
        [SerializeField] protected string header;
        [SerializeField] [TextArea(2,2)] protected string description;
        [SerializeField] protected bool useDefaultUsageDescription;
        [SerializeField] protected string usageDescription;

        public string Header => header;

        public string Description => description;

        public string UsageDesctiption => usageDescription;

        public GameObject GameObject => gameObject;

        public virtual void OnOptionUsed()
        {

        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            descriptionUI.SetDescriptiveObject(this);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            descriptionUI.SetDescriptiveObject(null);
        }

        protected virtual void Start()
        {
            descriptionUI = SCR_GeneralManager.UIManager.FindUIObject<UI_DescriptionUI>();
            if (useDefaultUsageDescription){
                usageDescription = defaultUsageString;
            }
        }

    }
}

