using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Model class that save data will be written in, containing persistent data for the player and the game.
/// </summary>
[Serializable]
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

/// <summary>
/// Model class containing information about important overworld player information
/// </summary>
[Serializable]
public class PlayerData
{
    /// <summary>
    /// Name of the player.
    /// </summary>
    public string PlayerName;

    /// <summary>
    /// Date in which the player started the game. Will be in the format DD-MM-YYY and HH:MM
    /// </summary>
    public string[] DateStarted;

    /// <summary>
    /// Date in which the player last saved the game. Will be in the format DD-MM-YYY and HH:MM
    /// </summary>
    public string[] DateLastSaved;

    /// <summary>
    /// The current play time of the save.
    /// </summary>
    public float PlayTime;

    /// <summary>
    /// The last scene in which the player saved in.
    /// </summary>
    public string RecentSceneName;

    /// <summary>
    /// The last position the player was in before saving. Formatted a Vector2 into a list of floats
    /// </summary>
    public float[] RecentPlayerPosition;

    public List<Dialogue.SavableChoice> SavableChoices;

    public PlayerData(string PlayerName)
    {
        this.PlayerName = PlayerName;
        SavableChoices = new List<Dialogue.SavableChoice>();
        DateStarted = new string[] { string.Format("{0:00}", DateTime.Now.Day), string.Format("{0:00}", DateTime.Now.Month), DateTime.Now.Year.ToString(), string.Format("{0:00}", DateTime.Now.Hour) + ":" + string.Format("{0:00}", DateTime.Now.Minute) };
    }

    /// <summary>
    /// Returns an updated version of the player data that should be used for saving player data
    /// </summary>
    /// <returns></returns>
    public PlayerData SavePlayerData()
    {
        PlayTime = SCR_GeneralManager.Instance.PlayerData.PlayTime + SCR_GeneralManager.Instance.CurrentSessionTime;
        SCR_GeneralManager.Instance.ResetCurrentSessionTime();
        DateLastSaved = new string[] { string.Format("{0:00}", DateTime.Now.Day), string.Format("{0:00}", DateTime.Now.Month), DateTime.Now.Year.ToString(), string.Format("{0:00}", DateTime.Now.Hour) + ":" + string.Format("{0:00}", DateTime.Now.Minute) };
        RecentSceneName = SceneManager.GetActiveScene().name;
        SavableChoices = SCR_GeneralManager.Instance.PlayerData.SavableChoices;
        Overworld.SCR_PlayerOverworldMovement overworldMovement = UnityEngine.Object.FindObjectOfType<Overworld.SCR_PlayerOverworldMovement>();

        if (overworldMovement == null) { return this; }
        RecentPlayerPosition = new float[2] { overworldMovement.transform.position.x, overworldMovement.transform.position.y };
        return this;
    }
}

/// <summary>
/// Static class responsible for holding functionality for saving and loading information from disk.
/// </summary>
public static class SavingOperations
{
    /// <summary>
    /// The exact directory in which save data is stored.
    /// </summary>
    public static string SaveDataPath => Application.persistentDataPath + "/Player.sav";

    /// <summary>
    /// Save information to the directory in a JSON format.
    /// </summary>
    public static void SaveInformation()
    {
        SaveData saveData = new SaveData(SCR_GeneralManager.Instance.PlayerData.SavePlayerData(), 
            SCR_GeneralManager.LevelManager.GetLevelInformation,
            SCR_GeneralManager.InventoryManager.Inventory);
        File.WriteAllText(SaveDataPath, JsonUtility.ToJson(saveData));

    }

    /// <summary>
    /// Retrieves save data from the directory and deserializes JSON data to a SaveData object if it exists.
    /// </summary>
    /// <returns>Save Data information if it exists or decodes properly, else nothing at all.</returns>
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
