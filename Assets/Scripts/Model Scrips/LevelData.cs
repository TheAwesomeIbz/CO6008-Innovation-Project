using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Model class containing level data. This can also be used for bosses.
/// </summary>
/// <remarks>
/// This will be the format in which data will be stored and loaded from disk.
/// </remarks>
[System.Serializable]
public class LevelData
{
    [field: Header("LEVEL DATA PROPERTIES")]
    [field: SerializeField] public string LevelID { get; private set; }
    [field: SerializeField] public float LevelCompletedTime { get; private set; } = 0;
    [field: SerializeField]
    public List<string> LevelCollectablesObtained { get; private set; } = new List<string>();

    /// <summary>
    /// Determines if level is completed if a level completed time is not its default value (0).
    /// </summary>
    public bool LevelCompleted => LevelCompletedTime > 0;

    public void UpdateLevelTime() {  LevelCompletedTime += Time.deltaTime; }

}
