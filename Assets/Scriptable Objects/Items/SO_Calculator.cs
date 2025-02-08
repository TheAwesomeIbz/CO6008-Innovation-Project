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
        Process[] allCalculatorProcesses = Process.GetProcessesByName(applicationProcessName);
        if (allCalculatorProcesses.Length > 0)
        {
            foreach (Process pr in allCalculatorProcesses) { pr.Kill(); }
        }
        else
        {
            Process.Start(applicationPath);
        }
    }
}
