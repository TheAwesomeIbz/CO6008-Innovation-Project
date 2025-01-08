using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace UnityEngine.UI
{
    public class SCR_UIManager : MonoBehaviour
    {
        [SerializeField] List<Transform> extensionObjects;

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
            Transform extensionObject = GameObject.FindGameObjectWithTag("UI Extension")?.transform ?? null;
            if (extensionObject == null ) { return; }

            if (extensionObjects.Count > 0)
            {
                for (int i = 0; i < extensionObjects.Count; i++)
                {
                    Destroy(extensionObjects[i].gameObject);
                }
            }
            

            extensionObject.GetComponent<Canvas>().worldCamera = Camera.main;
            extensionObjects = new List<Transform>();
            for (int i = 0; i < extensionObject.childCount; i++){
                extensionObjects.Add(extensionObject.GetChild(i));
            }
            foreach (Transform child in extensionObjects)
            {
                child.SetParent(transform);
            }
           
            Destroy(extensionObject.gameObject);
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
        }
    }
}


