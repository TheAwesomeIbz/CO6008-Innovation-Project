using System.Collections;
using System.Collections.Generic;
using Dialogue;
using UnityEngine;

namespace UnityEngine
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Inventory Items/URL Item")]
    public class SO_UrlItem : SO_Item, iUsableItem
    {
        [Header("URL PROPERTIES")]
        [SerializeField] private string[] URLs;
        
        
        
        public void UseItem()
        {
            if (!InternetAccessible())
            {
                SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(DialogueObject.CreateDialogue(
                    "There is no valid internet connection present in order to use this item.", "Please re-examine your current connection and try again."));
                return;
            }

            foreach (string url in URLs)
            {
                Application.OpenURL(url);
            }
        }

        private bool InternetAccessible()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
    }
}
