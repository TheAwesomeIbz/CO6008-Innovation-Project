using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Inventory Items/Complex Root Calculator")]
    public class SO_ComplexExponentiationCalculator : SO_Item, iUsableItem
    {
        public void UseItem()
        {
            if (System.Diagnostics.Process.GetProcessesByName("Complex Root Calculator").Length > 0)
            {
                System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("Complex Root Calculator");
                foreach (System.Diagnostics.Process process in processes)
                {
                    process.Kill();
                }
            }
            else
            {
                string processName = Application.dataPath + "/Scripts/Python Scripts/Complex Root Calculator.exe";
                System.Diagnostics.Process.Start(processName);
            }

        }
    }
}