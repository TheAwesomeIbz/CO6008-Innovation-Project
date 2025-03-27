using Entities.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Level
{
    public class SCR_TextTrigger : MonoBehaviour
    {
        [SerializeField]
        [TextArea(2, 2)] string textDescription;
        [SerializeField] TextMeshProUGUI displayTextObject;
        [SerializeField] RawImage canvasImage;
        [SerializeField] bool displayedInitially;

        Func<Color, float, Color> SetColorFunction = (Color currentColor, float alphaValue) => {
            return new Color(currentColor.r, currentColor.g, currentColor.b, alphaValue);
        };

        private void Start()
        {
            
            if (!displayedInitially)
            {
                displayTextObject.color = SetColorFunction(displayTextObject.color, 0);
                canvasImage.color = SetColorFunction(canvasImage.color, 0);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.GetType(out SCR_PlayerMovement playerMovement)) { return; }

            displayTextObject.text = textDescription;

            if (displayTextObject.color.a == 1) { return; }

            LeanTween.cancel(gameObject);   
            LeanTween.value(0, 1, 0.5f).setOnUpdate((float value) =>
            {
                displayTextObject.color = SetColorFunction(displayTextObject.color, value);
                canvasImage.color = SetColorFunction(canvasImage.color, value);
            });
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.GetType(out SCR_PlayerMovement playerMovement)) { return; }

            LeanTween.cancel(gameObject);
            LeanTween.value(1, 0, 0.5f).setOnUpdate((float value) =>
            {
                displayTextObject.color = SetColorFunction(displayTextObject.color, value);
                canvasImage.color =  SetColorFunction(canvasImage.color, value);
            });
        }
    }

}