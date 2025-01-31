using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Inventory Items/Calculator")]
public class SO_Calculator : SO_Item, iUsableItem
{
    [Header("APPLICATION PATH PROPERTIES")]
    [SerializeField] private string windowsApplicationPath = "calc.exe";
    [SerializeField] private string macApplicationPath = "/System/Applications/Calculator.app";
    
    [Header("APPLICATION NAME PROPERTIES")]
    [SerializeField] private string windowsApplicationName = "CalculatorApp";
    [SerializeField] private string macApplicationName = "Calculator";
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

    private void OpenCalculator(string applicationPath, string applicationProcessName)
    {
        if (Process.GetProcessesByName(applicationProcessName).Length > 0)
        {
            //TODO: Have a notification pop up with this window.
            UnityEngine.Debug.Log($"THE CALCULATOR IS ALREADY OPEN ON {Application.platform}");
        }
        else
        {
            Process.Start(applicationPath);
        }
    }
}
