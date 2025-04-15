using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Inventory Items/Complex Root Calculator")]
    public class SO_ComplexExponentiationCalculator : SO_Item, iUsableItem
    {
        [SerializeField]
        private string macApplicationPath, macApplicationName, windowsApplicationPath, windowsApplicationName; 
        public void UseItem()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXEditor:
                    OpenCalculator(macApplicationPath, macApplicationName);
                    break;
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    OpenCalculator(windowsApplicationPath, windowsApplicationName);
                    break;
            }
            
            

        }

        private void OpenCalculator(string applicationPath, string applicationName)
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
                string processName = Application.dataPath + applicationPath;
                System.Diagnostics.Process.Start(processName);
            }
        }
    }3
}