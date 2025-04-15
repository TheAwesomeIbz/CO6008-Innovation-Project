using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SettingsInformation
{
    public Settings.SettingOption GameMode;
    public Settings.SettingOption TextSpeed;
    public float UIScale;
    public float AudioVolume;

    public void SaveSettings()
    {
        File.WriteAllText(Application.persistentDataPath + "/Settings.config", JsonUtility.ToJson(this));
    }
}

[System.Serializable]
public class PersistentSettings
{
    public bool ExaminationCompleted = false;
    public bool AgreedToConditions = false;

    static string Path = Application.persistentDataPath + "/PersistentSettings.config";

    public void SaveSettings()
    {
        File.WriteAllText(Path, JsonUtility.ToJson(this));
    }

    public static PersistentSettings LoadSettings()
    {
        try
        {
            return JsonUtility.FromJson<PersistentSettings>(File.ReadAllText(Path));
        }
        catch
        {
            PersistentSettings settings = new PersistentSettings();
            settings.SaveSettings();
            return null;
        }
    }
}

public class Settings
{
    public static SettingsInformation DefaultSettings { get { return new SettingsInformation
    {
        GameMode = SettingOption.DEFAULT,
        TextSpeed = SettingOption.DEFAULT,
        AudioVolume = 0.5f,
        UIScale = 1f
    }; } }

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
        SettingsInformation = Settings.DefaultSettings;
        File.WriteAllText(settingsPath, JsonUtility.ToJson(SettingsInformation));
    }

}
