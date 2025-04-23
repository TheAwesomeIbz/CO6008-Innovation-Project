using Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Inventory Items/Complex Root Calculator")]
    public class SO_ComplexExponentiationCalculator : SO_Item, iUsableItem
    {
        [SerializeField]
        private string macApplicationName, windowsApplicationName; 
        public void UseItem()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXEditor:
                    OpenCalculator(macApplicationName);
                    break;
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    OpenCalculator(windowsApplicationName);
                    break;
            }
        }

        private void OpenCalculator(string applicationName)
        {
            try
            {
                if (System.Diagnostics.Process.GetProcessesByName(applicationName).Length > 0)
                {
                    System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(applicationName);
                    foreach (System.Diagnostics.Process process in processes)
                    {
                        process.Kill();
                    }
                }
                else
                {
                    string processName = Application.dataPath + $"/External Material/{applicationName}";
                    System.Diagnostics.Process.Start(processName);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"An unexpected error occured. {ex.Message}");
                System.Diagnostics.Process.Start(Application.dataPath + "/External Material");
            }
            
        }
    }
}