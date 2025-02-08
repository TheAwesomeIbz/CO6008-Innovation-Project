using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI.Title
{
    public class UI_DescriptiveContinueButton : UI_DescriptiveObject
    {
        SaveData saveData;

        protected override void Start()
        {
            saveData = SCR_GeneralManager.UIManager.FindUIObject<UI_TitleUI>().SaveData;
            base.Start();
        }

        void Update()
        {
            if (string.IsNullOrEmpty(saveData?.PlayerData.PlayerName ?? "")) { return; }

            header = $"Continue with {saveData.PlayerData.PlayerName}";
            description =
                $"Last saved on <color=green>{saveData.PlayerData.DateLastSaved}</color>\n" +
                $"First started on <color=green>{saveData.PlayerData.DateStarted}</color>";
        }
    }
}
