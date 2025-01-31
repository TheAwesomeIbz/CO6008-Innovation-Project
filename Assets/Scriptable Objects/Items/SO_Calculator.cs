using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Inventory Items/Calculator")]
public class SO_Calculator : SO_Item, iUsableItem
{
    public void UseItem()
    {
        if (Application.platform == RuntimePlatform.OSXPlayer)
        {
            //do something
        }

        else if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            //do something
        }


        string calculator = "CalculatorApp";

        if (System.Diagnostics.Process.GetProcessesByName(calculator).Length > 0)
        {
            Debug.Log($"CALC.EXE IS ALREADY OPEN ON {Application.platform}");
        }
        else
        {
            System.Diagnostics.Process.Start("calc.exe");
        }
    }


}
