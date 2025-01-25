using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Settings;


[System.Serializable]
public class SettingsInformation
{
    public static SettingsInformation DefaultSettings = new SettingsInformation
    {
        GameMode = SettingOption.DEFAULT,
        TextSpeed = SettingOption.DEFAULT,
        AudioVolume = 0.5f,
        UIScale = 1f
    };

    public SettingOption GameMode;
    public SettingOption TextSpeed;
    public float UIScale;
    public float AudioVolume;

    public void SaveSettings()
    {
        File.WriteAllText(Application.persistentDataPath + "/Settings.config", JsonUtility.ToJson(this));
    }
}

public class Settings
{


    public SettingsInformation SettingsInformation;
    string settingsPath = Application.persistentDataPath + "/Settings.config";

    public enum SettingOption 
    {
        LOWEST,
        DEFAULT,
        HIGHEST
    }

    public Settings()
    {
        if (File.Exists(settingsPath))
        {
            try
            {
                SettingsInformation settings = JsonUtility.FromJson<SettingsInformation>(File.ReadAllText(settingsPath));
                SettingsInformation = settings;
            }
            catch{
                InitialiseSettings();
            }
        }
        else {
            InitialiseSettings();
        }
    }

    private void InitialiseSettings()
    {
        SettingsInformation = new SettingsInformation();
        File.WriteAllText(settingsPath, JsonUtility.ToJson(new SettingsInformation()));
    }

}
