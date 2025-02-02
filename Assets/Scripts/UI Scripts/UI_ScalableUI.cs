using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
    public class UI_ScalableUI : MonoBehaviour
    {
        private UI_SettingsUI cachedSettingsUI;
        void Update()
        {
            if (cachedSettingsUI is null)
            {
                cachedSettingsUI = SCR_GeneralManager.UIManager.FindUIObject<UI_SettingsUI>();
                return;
            }
            transform.localScale = Vector3.one * cachedSettingsUI.SettingsInformation.UIScale;
        }
    }
}
