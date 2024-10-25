using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_GeneralManager : MonoBehaviour
{
    private static SCR_GeneralManager _instance;

    public static SCR_PlayerInputManager PlayerInputManager => _instance.GetComponent<SCR_PlayerInputManager>();
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance);
        }
        else{
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
