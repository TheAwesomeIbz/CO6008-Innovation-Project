using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SCR_GeneralManager : MonoBehaviour
{
    private static SCR_GeneralManager _instance;

    public static SCR_PlayerInputManager PlayerInputManager => _instance.GetComponent<SCR_PlayerInputManager>();
    
    public static SCR_InventoryManager InventoryManager => _instance.GetComponent<SCR_InventoryManager>();

    public static SCR_UIManager UIManager => _instance.GetComponentInChildren<SCR_UIManager>();

    public static SCR_LevelManager LevelManager => _instance.GetComponent<SCR_LevelManager>();

    private void Awake()
    {
        
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance);
        }
        else{
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
