using Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SCR_GeneralManager : MonoBehaviour
{
    public static SCR_GeneralManager Instance;

    public static SCR_PlayerInputManager PlayerInputManager => Instance.GetComponent<SCR_PlayerInputManager>();
    
    public static SCR_InventoryManager InventoryManager => Instance.GetComponent<SCR_InventoryManager>();

    public static SCR_UIManager UIManager => Instance.GetComponentInChildren<SCR_UIManager>();

    public static SCR_LevelManager LevelManager => Instance.GetComponent<SCR_LevelManager>();
    public SettingsInformation Settings => UIManager.FindUIObject<UI_SettingsUI>().SettingsInformation;

    
    /// <summary>
    /// Maximum play time allowed for the player in any given save.
    /// </summary>
    public const int C_MaximumPlayTime = 3599940;
    [field: Header("PLAYER SAVE PROPERTIES")]
    [field: SerializeField] public PlayerData PlayerData { get; private set; }
    [field: SerializeField] public List<SavableChoice> Choices { get; private set; }
    [field: SerializeField] public List<string> CollectedItems { get; private set; }
    [SerializeField] private float _currentSessionTime;
    public float CurrentSessionTime => _currentSessionTime;
    UI_LoadScene _loadScenes;


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
        _loadScenes = UIManager.FindUIObject<UI_LoadScene>();
        SavingOperations.OnSaveDataLoaded += OnSaveDataLoaded;
    }

    /// <summary>
    /// Loads the current player data to the Game Manager
    /// </summary>
    /// <param name="playerData"></param>
    private void OnSaveDataLoaded(SaveData saveData)
    {
        PlayerData = saveData.PlayerData;
        Choices = saveData.Choices;
        CollectedItems = saveData.CollectedItems;

        if (PlayerData == null) { return; }
        if (PlayerData.RecentPlayerPosition == null) { return; }

        Overworld.SCR_PlayerOverworldMovement playerOverworldMovement = FindObjectOfType<Overworld.SCR_PlayerOverworldMovement>();
        if (playerOverworldMovement == null) { return; }
        playerOverworldMovement.transform.position = new Vector3(PlayerData.RecentPlayerPosition[0], PlayerData.RecentPlayerPosition[1]);
        playerOverworldMovement.Start();

        if (Choices.Count == 0) { return; }
        Overworld.SCR_QuizDialogueNode[] choiceDialogueNodes = FindObjectsOfType<Overworld.SCR_QuizDialogueNode>();
        foreach (Overworld.SCR_QuizDialogueNode choiceDialogueNode in choiceDialogueNodes)
        {
            SavableChoice savableChoice = Choices.Find(ch => ch.ChoiceID == choiceDialogueNode.SavableChoice.ChoiceID);
            choiceDialogueNode.SavableChoice.SetChoice(savableChoice.SelectedChoice, savableChoice.TimeTakenToSelect, savableChoice.CorrectAnswer);
        }

    }

    public void SetPlayerName(string playerName) => PlayerData.PlayerName = playerName;

    private void Update()
    {
        UpdateCurrentSessionTime();

        //if (Input.GetKeyDown(KeyCode.F1) && Input.GetKey(KeyCode.LeftShift))
        //{
        //    System.Diagnostics.Process.Start(Application.persistentDataPath);
        //}
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

}
