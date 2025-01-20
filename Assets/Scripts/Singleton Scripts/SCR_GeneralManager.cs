using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class SCR_GeneralManager : MonoBehaviour
{
    public static SCR_GeneralManager Instance;

    public static SCR_PlayerInputManager PlayerInputManager => Instance.GetComponent<SCR_PlayerInputManager>();
    
    public static SCR_InventoryManager InventoryManager => Instance.GetComponent<SCR_InventoryManager>();

    public static SCR_UIManager UIManager => Instance.GetComponentInChildren<SCR_UIManager>();

    public static SCR_LevelManager LevelManager => Instance.GetComponent<SCR_LevelManager>();

    /// <summary>
    /// Maximum play time allowed for the player in any given save.
    /// </summary>
    public const int C_MaximumPlayTime = 3599940;
    [field: Header("PLAYER SAVE PROPERTIES")]
    [field: SerializeField] public PlayerData PlayerData { get; private set; }
    [SerializeField] private float _currentSessionTime;
    public float CurrentSessionTime => _currentSessionTime;
    UI_LoadScenes _loadScenes;

    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else{
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _loadScenes = UIManager.FindUIObject<UI_LoadScenes>();
    }

    private void Update()
    {
        UpdateCurrentSessionTime();

        if (Input.GetKeyDown(KeyCode.F2) && FindObjectOfType<Overworld.SCR_PlayerOverworldMovement>())
        {
            SavingOperations.SaveInformation();
            if (Input.GetKey(KeyCode.LeftShift)) { System.Diagnostics.Process.Start(Application.persistentDataPath); }
        }

        if (Input.GetKeyDown(KeyCode.F3) && FindObjectOfType<Overworld.SCR_PlayerOverworldMovement>())
        {
            SavingOperations.LoadInformation();
        }
    }

    /// <summary>
    /// Sets the current session time to 0
    /// </summary>
    public void ResetCurrentSessionTime() => _currentSessionTime = 0;

    /// <summary>
    /// Updates the current session time every frame depending on the current game state
    /// </summary>
    private void UpdateCurrentSessionTime()
    {
        if (PlayerData == null || PlayerData.PlayTime >= C_MaximumPlayTime) { return; }
        if (_loadScenes != null && _loadScenes.Loading) { return; }

        _currentSessionTime += Time.deltaTime;
    }

    /// <summary>
    /// Loads the current player data to the Game Manager
    /// </summary>
    /// <param name="playerData"></param>
    public void LoadPlayerData(PlayerData playerData)
    {
        PlayerData = playerData;

        if (playerData == null) { return; }
        if (playerData.RecentPlayerPosition == null) { return; }

        Overworld.SCR_PlayerOverworldMovement playerOverworldMovement = FindObjectOfType<Overworld.SCR_PlayerOverworldMovement>();
        if (playerOverworldMovement == null) { return; }
        playerOverworldMovement.transform.position = new Vector3(playerData.RecentPlayerPosition[0], playerData.RecentPlayerPosition[1]);
        playerOverworldMovement.Start();

        if (PlayerData.SavableChoices.Count == 0) { return; }
        Overworld.SCR_ChoiceDialogueNode[] choiceDialogueNodes = FindObjectsOfType<Overworld.SCR_ChoiceDialogueNode>();
        foreach (Overworld.SCR_ChoiceDialogueNode choiceDialogueNode in choiceDialogueNodes)
        {
            Dialogue.SavableChoice savableChoice = PlayerData.SavableChoices.Find(ch => ch.ChoiceID == choiceDialogueNode.SavableChoice.ChoiceID);
            choiceDialogueNode.SavableChoice.SetChoice(savableChoice.SelectedChoice, savableChoice.TimeTakenToSelect);
        }


    }
}
