using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
    public class UI_ScalableUI : MonoBehaviour
    {
        void Update()
        {
            if (SCR_GeneralManager.UIManager.FindUIObject<UI_SettingsUI>() == null) { return; }
            transform.localScale = Vector3.one * SCR_GeneralManager.UIManager.FindUIObject<UI_SettingsUI>().settingsInformation.UIScale;
        }
    }
}
