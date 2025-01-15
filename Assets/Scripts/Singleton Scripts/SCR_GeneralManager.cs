using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SCR_GeneralManager : MonoBehaviour
{
    const int C_MaximumPlayTime = 3599940;
    [field : SerializeField] public PlayerData PlayerData { get; private set; }

    public static SCR_GeneralManager Instance;

    public static SCR_PlayerInputManager PlayerInputManager => Instance.GetComponent<SCR_PlayerInputManager>();
    
    public static SCR_InventoryManager InventoryManager => Instance.GetComponent<SCR_InventoryManager>();

    public static SCR_UIManager UIManager => Instance.GetComponentInChildren<SCR_UIManager>();

    public static SCR_LevelManager LevelManager => Instance.GetComponent<SCR_LevelManager>();

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

    public void LoadPlayerData(PlayerData playerData)
    {
        PlayerData = playerData;


        if (playerData == null) { return; }
        if (playerData.RecentPlayerPosition == null) { return; }

        Overworld.SCR_PlayerOverworldMovement playerOverworldMovement = FindObjectOfType<Overworld.SCR_PlayerOverworldMovement>();
        if (playerOverworldMovement == null) { return; }
        playerOverworldMovement.transform.position = new Vector3(playerData.RecentPlayerPosition[0], playerData.RecentPlayerPosition[1]);
        playerOverworldMovement.Start();
    }

    private void Update()
    {
        if (PlayerData != null && PlayerData.PlayTime < C_MaximumPlayTime)
        {
            PlayerData.PlayTime += Time.deltaTime;
        }
        
        

        if (Input.GetKeyDown(KeyCode.F2)){
            SavingOperations.SaveInformation();
            System.Diagnostics.Process.Start(Application.persistentDataPath);
        }

        if (Input.GetKeyDown(KeyCode.F3)){
            SavingOperations.LoadInformation();
        }
    }

}
