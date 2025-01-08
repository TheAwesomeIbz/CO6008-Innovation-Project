using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_LevelComplete : MonoBehaviour
{
    [Header("LEVEL UI PROPERTIES")]
    [SerializeField] GameObject _levelCompleteContentObject;
    string _sceneName;
    void Start()
    {
        _levelCompleteContentObject.SetActive(false);
    }

    public void DisplayUI(string sceneName)
    {
        _levelCompleteContentObject.SetActive(true);
        _sceneName = sceneName;
    }
    // Update is called once per frame
    void Update()
    {
        if (!_levelCompleteContentObject.activeInHierarchy) { return; }

        if (Input.anyKeyDown && !string.IsNullOrEmpty(_sceneName))
        {
            SCR_GeneralManager.UIManager.FindUIObject<UI_LoadScenes>().LoadScene(new UI_LoadScenes.TransitionProperties { 
                SceneName = _sceneName,
                OnSceneLoaded = OnSceneLoaded
            });
            _sceneName = "";
        }
    }

    private void OnSceneLoaded()
    {
        _levelCompleteContentObject.SetActive(false);
    }
}
