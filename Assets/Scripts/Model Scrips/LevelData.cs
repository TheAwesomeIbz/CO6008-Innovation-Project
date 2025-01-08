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
    [field: SerializeField] public float LevelCompletedTime { get; private set; } = -1;
    [field: SerializeField]
    public LevelCollectible[] LevelCollectibles { get; private set; } = new LevelCollectible[]
    {
        new LevelCollectible(0),
        new LevelCollectible(1),
        new LevelCollectible(2)
    };

    /// <summary>
    /// Determines if level is completed if a level completed time is not its default value (-1).
    /// </summary>
    public bool LevelCompleted => LevelCompletedTime != -1;


    /// <summary>
    /// Model class containing information about collectible
    /// </summary>
    [System.Serializable]
    public class LevelCollectible
    {
        public int CollectibleID;
        public bool Collected = false;

        public LevelCollectible(int collectibleID, bool collected = false)
        {
            CollectibleID = collectibleID;
            Collected = collected;

        }
    }
}
