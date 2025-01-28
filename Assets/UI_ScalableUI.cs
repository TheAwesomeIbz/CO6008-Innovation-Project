using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
    public class UI_ScalableUI : MonoBehaviour
    {
        [SerializeField] SettingsInformation settings;
        void Start()
        {
            settings = SCR_GeneralManager.UIManager.FindUIObject<UI_SettingsUI>().settingsInformation;
        }

        void Update()
        {
            transform.localScale = Vector3.one * settings.UIScale;
        }
    }
}
