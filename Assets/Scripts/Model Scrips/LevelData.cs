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
    [field: SerializeField] public List<LevelCollectable> LevelCollectablesObtained { get; private set; } = new List<LevelCollectable>();

    /// <summary>
    /// Determines if level is completed if a level completed time is not its default value (0).
    /// </summary>
    public bool LevelCompleted => LevelCompletedTime > 0;

    public void UpdateLevelTime() {  LevelCompletedTime += Time.deltaTime; }

}

[System.Serializable]
public class LevelCollectable
{
    [field: SerializeField] [field: Range(0,2)] public int CollectableID { get; private set; }
    [field: SerializeField] public bool CollectableObtained { get; private set; }

    public void Collect() => CollectableObtained = true;

}