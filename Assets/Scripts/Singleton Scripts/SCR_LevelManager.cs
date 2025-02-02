using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Overworld;
using Level;
using System;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine.UI;

/// <summary>
/// Manager class responsible for handling level data and information
/// </summary>
public class SCR_LevelManager : MonoBehaviour
{
    [field: Header("LEVEL MANAGER PROPERTIES")]
    [SerializeField] private List<LevelData> levelInformation;
    [SerializeField] private bool levelBegan;

    [Header("LEVEL TRANSITION PROPERTIES")]
    [SerializeField] private string previousSceneName;
    [SerializeField] private Vector3 previousPlayerOverworldPosition;
    [SerializeField] private LevelData currentLevelData;

    
    public string GetPreviousSceneName => previousSceneName ?? "Overworld Scene";
    public List<LevelData> GetLevelInformation => levelInformation;
    public LevelData GetCurrentLevelData => currentLevelData;
    private UI_LoadScene loadScenes;

    public bool LevelFirstCompleted { get; private set; }   
    private SCR_LevelNode cachedLevelNode;
    void Start()
    {
        loadScenes = SCR_GeneralManager.UIManager.FindUIObject<UI_LoadScene>();
        SavingOperations.OnSaveDataLoaded += OnSaveDataLoaded;
    }

    /// <summary>
    /// Populates level data list from disk or any source
    /// </summary>
    private void OnSaveDataLoaded(SaveData saveData)
    {
        levelInformation = saveData.LevelInformation;
        OnOverworldSceneLoaded();
    }

    /// <summary>
    /// Finds level by ID within the level data list
    /// </summary>
    /// <param name="id"></param>
    /// <returns>A level data object (if exists) with the corresponding ID</returns>
    public LevelData FindLevelByID(string id) => levelInformation?.Find(pr => pr.LevelID == id) ?? null;

    /// <summary>
    /// Finds all level nodes within a scene, and initializes their values, if they exist within the level data array.
    /// </summary>
    /// <remarks>
    /// Should be called ideally when a scene loads in the overworld
    /// </remarks>
    public void InitializeLevels()
    {
        SCR_LevelNode[] allLevelNodes = FindObjectsOfType<SCR_LevelNode>();

        foreach (SCR_LevelNode levelNode in allLevelNodes)
        {
            LevelData existingData = levelInformation?.Find(pr => pr.LevelID == levelNode.LevelData.LevelID);
            if (existingData == null) { continue; }
            levelNode.InitializeLevelNode(existingData);
        }

    }

    /// <summary>
    /// Called on the transition from the overworld into the level scene.
    /// </summary>
    /// <param name="leveldata"></param>
    /// <param name="playerOverworldMovement"></param>
    public void OnTransitionToLevel(LevelData leveldata, SCR_PlayerOverworldMovement playerOverworldMovement)
    {
        currentLevelData = leveldata;
        CachePlayerProperties(playerOverworldMovement);
    }

    public void OnSceneLoaded()
    {
        SCR_LevelCollectable[] levelCollectables = FindObjectsOfType<SCR_LevelCollectable>();
        foreach (SCR_LevelCollectable levelCollectable in levelCollectables)
        {
            bool collectibleAlreadyExists = currentLevelData.LevelCollectablesObtained.Contains(levelCollectable.name);
            levelCollectable.gameObject.SetActive(!collectibleAlreadyExists);
        }
        levelBegan = true;
        LevelFirstCompleted = false;
        cachedLevelNode = null;
    }
    
    public void CachePlayerProperties(SCR_PlayerOverworldMovement playerOverworldMovement)
    {
        previousSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        previousPlayerOverworldPosition = playerOverworldMovement.transform.position;
    }

    /// <summary>
    /// Called on the first frame in the overworld scene scene that transitioned from the level scene.
    /// </summary>
    /// <remarks>Sets the player's properties</remarks>
    public void OnOverworldSceneLoaded()
    {
        LoadPlayerProperties();
        LoadLevelProperties();
    }

    public void OnTransitionFinished()
    {
        cachedLevelNode?.OnLevelCompleted();
        cachedLevelNode = null;
    }

    private void LoadPlayerProperties()
    {
        SCR_PlayerOverworldMovement playerOverworldMovement = FindObjectOfType<SCR_PlayerOverworldMovement>();
        if (playerOverworldMovement == null) { return; }
        playerOverworldMovement.transform.position = previousPlayerOverworldPosition;
        Collider2D[] colliders = Physics2D.OverlapPointAll(previousPlayerOverworldPosition);
        foreach (Collider2D collider in colliders)
        {
            SCR_GraphNode graphNode = collider.GetComponent<SCR_GraphNode>();
            if (graphNode) { playerOverworldMovement.SetGraphNode(graphNode); break; }
        }

        previousPlayerOverworldPosition = Vector3.zero;
        previousSceneName = "";
    }

    private void LoadLevelProperties()
    {
        SCR_LevelNode[] levelNodes = FindObjectsOfType<SCR_LevelNode>();
        if (levelNodes == null) { return; }

        if (LevelFirstCompleted){
            cachedLevelNode = Array.Find(levelNodes, lvlNode => lvlNode.LevelData.LevelID == currentLevelData.LevelID);
        }
        

        foreach (SCR_LevelNode levelNode in levelNodes)
        {   
            LevelData existingLevelData = SCR_GeneralManager.LevelManager.levelInformation.Find(lvl => lvl.LevelID == levelNode.LevelData.LevelID);
            if (existingLevelData != null) { 
                levelNode.UpdateLevelData(existingLevelData); 
            }
        }

        
        
    }

    /// <summary>
    /// Called on interaction with goal post when level is completed.
    /// </summary>
    public void OnLevelCompleted()
    {
        levelBegan = false;
        LevelData levelData = FindLevelByID(currentLevelData.LevelID);

        //If there is no level data present, that means it doesnt exist in the list, so append to level information
        if (levelData == null)
        {
            levelInformation.Add(currentLevelData);
            LevelFirstCompleted = true;
            return;
        }

        //Find instance of level data in list and update it
        for (int i = 0; i < levelInformation.Count; i++)
        {
            if (levelInformation[i].LevelID == currentLevelData.LevelID)
            {
                levelInformation[i] = currentLevelData;
            }
        }
    }

    private void UpdateLevelTimer()
    {
        if (!levelBegan) { return; }
        if (!SCR_PlayerInputManager.PlayerControlsEnabled) { return; }
        if (loadScenes?.Loading ?? false) { return; }

        currentLevelData?.UpdateLevelTime();
    }
    private void Update()
    {
        UpdateLevelTimer();

    }

    private void OnDisable()
    {
        SavingOperations.OnSaveDataLoaded -= OnSaveDataLoaded;
    }
}
