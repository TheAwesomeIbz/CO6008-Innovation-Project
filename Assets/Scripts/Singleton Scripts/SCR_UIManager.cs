using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace UnityEngine.UI
{
    public class SCR_UIManager : MonoBehaviour
    {
        Transform _extensionObject;
        public T FindUIObject<T>() where T : MonoBehaviour
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                T childComponent = transform.GetChild(i).GetComponent<T>();
                if (childComponent != null) { return childComponent; }
            }
            return null;
        }


        private void Awake()
        {
            GetComponent<Canvas>().worldCamera = Camera.main;
            SceneManager.sceneLoaded += SceneLoaded;
        }
        private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            _extensionObject = GameObject.FindGameObjectWithTag("UI Extension")?.transform ?? null;
            if ( _extensionObject == null ) { return; }

            List<Transform> children = new List<Transform>();
            for (int i = 0; i < _extensionObject.transform.childCount; i++){
                children.Add(_extensionObject.transform.GetChild(i));
            }
            foreach (Transform child in children){
                child.SetParent(transform);
            }
           
            Destroy(_extensionObject.gameObject);
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
        }
    }
}


