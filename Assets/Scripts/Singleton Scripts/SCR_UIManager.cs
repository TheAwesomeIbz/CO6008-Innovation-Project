using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace UnityEngine.UI
{
    public class SCR_UIManager : MonoBehaviour
    {
        [SerializeField] List<Transform> _extensionObjects;

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

            if (_extensionObjects.Count > 0)
            {
                for (int i = 0; i < _extensionObjects.Count; i++)
                {
                    Destroy(_extensionObjects[i].gameObject);
                }
            }

            SCR_GeneralManager.UIManager.GetComponent<Canvas>().worldCamera = Camera.main;
            extensionObject.GetComponent<Canvas>().worldCamera = Camera.main;
            _extensionObjects = new List<Transform>();
            for (int i = 0; i < extensionObject.childCount; i++){
                _extensionObjects.Add(extensionObject.GetChild(i));
            }
            foreach (Transform child in _extensionObjects)
            {
                child.SetParent(transform);
                child.SetSiblingIndex(0);
                child.transform.localScale = Vector3.one;
                child.transform.localPosition = Vector3.zero;
            }
           
            Destroy(extensionObject.gameObject);
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
        }
    }
}


