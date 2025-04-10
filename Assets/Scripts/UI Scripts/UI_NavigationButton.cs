using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace UnityEngine.UI
{

    public class UI_NavigationButton : MonoBehaviour
    {
        public static event Action<int> OnButtonPressed;
        [SerializeField] TextMeshProUGUI buttonTextDisplay;

        Button buttonObject;

        private void Awake()
        {
            buttonTextDisplay = GetComponentInChildren<TextMeshProUGUI>();
            buttonObject = GetComponent<Button>();
        }


        public void SetButtonSelected(bool buttonSelected, bool questionAnswered)
        {
            buttonObject.image.color = buttonSelected ? Color.black : Color.white;
            buttonTextDisplay.color = buttonSelected ? Color.white : (questionAnswered ? Color.green : Color.red);
        }
        
        public void OnNavigationButtonPressed() => OnButtonPressed?.Invoke(transform.GetSiblingIndex());
        public void InitialiseButton(string text) => buttonTextDisplay.text = text;
    }
}
