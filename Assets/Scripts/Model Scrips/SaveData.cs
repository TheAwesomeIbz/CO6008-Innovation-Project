using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SaveData 
{
    public PlayerData PlayerData;
    public List<LevelData> LevelInformation;
    public List<SO_Item> InventoryInformation;

    public SaveData(PlayerData playerData, List<LevelData> levelInformation, List<SO_Item> inventoryInformation)
    {
        PlayerData = playerData;
        LevelInformation = levelInformation;
        InventoryInformation = inventoryInformation;
    }
}

[System.Serializable]
public class PlayerData
{
    public string PlayerName;
    public string[] DateStarted;
    public string[] DateLastSaved;
    public float PlayTime;
    public string RecentSceneName;
    public float[] RecentPlayerPosition;

    public PlayerData(string PlayerName)
    {
        this.PlayerName = PlayerName;
        DateStarted = new string[] { string.Format("{0:00}", DateTime.Now.Day), string.Format("{0:00}", DateTime.Now.Month), DateTime.Now.Year.ToString(), string.Format("{0:00}", DateTime.Now.Hour) + ":" + string.Format("{0:00}", DateTime.Now.Minute) };
    }

    public PlayerData UpdatedPlayerData()
    {
        PlayTime = SCR_GeneralManager.Instance.PlayerData.PlayTime;
        DateLastSaved = new string[] { string.Format("{0:00}", DateTime.Now.Day), string.Format("{0:00}", DateTime.Now.Month), DateTime.Now.Year.ToString(), string.Format("{0:00}", DateTime.Now.Hour) + ":" + string.Format("{0:00}", DateTime.Now.Minute) };

        Overworld.SCR_PlayerOverworldMovement overworldMovement = UnityEngine.Object.FindObjectOfType<Overworld.SCR_PlayerOverworldMovement>();

        if (overworldMovement == null)
        {
            return this;
        }
        RecentSceneName = SceneManager.GetActiveScene().name;
        RecentPlayerPosition = new float[2] { overworldMovement.transform.position.x, overworldMovement.transform.position.y };

        return this;
    }
}


public static class SavingOperations
{
    public static string SaveDataPath => Application.persistentDataPath + "/SaveData.math";
    public static void SaveInformation()
    {
        SaveData saveData = new SaveData(SCR_GeneralManager.Instance.PlayerData.UpdatedPlayerData(), 
            SCR_GeneralManager.LevelManager.GetLevelInformation,
            SCR_GeneralManager.InventoryManager.Inventory);
        File.WriteAllText(SaveDataPath, JsonUtility.ToJson(saveData));
    }

    public static SaveData LoadInformation()
    {
        try
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(SaveDataPath));

            SCR_GeneralManager.Instance.LoadPlayerData(saveData.PlayerData);
            SCR_GeneralManager.LevelManager.LoadLevelInformation(saveData.LevelInformation);
            SCR_GeneralManager.InventoryManager.LoadInventoryInformation(saveData.InventoryInformation);
            return saveData;
        }
        catch
        {
            return null;
        }
        

    }
}
