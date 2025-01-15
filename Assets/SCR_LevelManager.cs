using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Overworld;

public class SCR_LevelManager : MonoBehaviour
{
    [field: Header("LEVEL MANAGER PROPERTIES")]
    [SerializeField] private List<LevelData> levelInformation;

    [Header("LEVEL TRANSITION PROPERTIES")]
    [SerializeField] private Vector3 previousPlayerOverworldPosition;
    [SerializeField] private LevelData currentLevelData;
    
    public List<LevelData> GetLevelInformation => levelInformation;

    /// <summary>
    /// Populates level data list from disk or any source
    /// </summary>
    /// <param name="levelInformation"></param>
    public void LoadLevelInformation(List<LevelData> levelInformation) => this.levelInformation = levelInformation;

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

    public void OnLevelTransition(LevelData leveldata, SCR_PlayerOverworldMovement playerOverworldMovement)
    {
        currentLevelData = leveldata;
        previousPlayerOverworldPosition = playerOverworldMovement.transform.position;
    }

    public void OnOverworldTransition()
    {
        SCR_PlayerOverworldMovement playerOverworldMovement = FindObjectOfType<SCR_PlayerOverworldMovement>();
        playerOverworldMovement.transform.position = previousPlayerOverworldPosition;
        Collider2D[] colliders = Physics2D.OverlapPointAll(previousPlayerOverworldPosition);
        foreach (Collider2D collider in colliders)
        {
            SCR_GraphNode graphNode = collider.GetComponent<SCR_GraphNode>();
            if (graphNode) { playerOverworldMovement.SetGraphNode(graphNode); break; }
        }
    }

    public void OnLevelCompleted()
    {
        LevelData levelData = FindLevelByID(currentLevelData.LevelID);
        if (levelData == null)
        {
            levelInformation.Add(currentLevelData);
        }
        else
        {
             
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
