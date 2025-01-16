using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace UnityEngine.UI
{
    public class SCR_UIManager : MonoBehaviour
    {
        List<Transform> _extensionObjects;

        /// <summary>
        /// Find UI Object with a specified type, which can access any object if it exists in the UI Manager Hierarchy.
        /// </summary>
        /// <typeparam name="T">The type of the UI Object, which must be a monobehaviour</typeparam>
        /// <returns>The instance of the object if it exist, otherwise null</returns>
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
            _extensionObjects = new List<Transform>();
        }

        /// <summary>
        /// Method added to delegate every time a scene is loaded.
        /// </summary>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            Transform extensionObject = GameObject.FindGameObjectWithTag("UI Extension")?.transform ?? null;

            //Initialised the camera of the extension object and normal UI manager
            SCR_GeneralManager.UIManager.GetComponent<Canvas>().worldCamera = Camera.main;
            

            if (extensionObject == null ) { return; }
            extensionObject.GetComponent<Canvas>().worldCamera = Camera.main;

            //If any extension objects already exist, then delete them all
            if (_extensionObjects.Count > 0)
            {
                for (int i = 0; i < _extensionObjects.Count; i++)
                {
                    Destroy(_extensionObjects[i].gameObject);
                }
            }

            //Initialises a new list, and adds all new extension objects to them
            _extensionObjects = new List<Transform>();
            for (int i = 0; i < extensionObject.childCount; i++){
                _extensionObjects.Add(extensionObject.GetChild(i));
            }

            //Format each child extension object
            foreach (Transform child in _extensionObjects) { FormatExtensionRectTransform(child); }
            Destroy(extensionObject.gameObject);
        }

        /// <summary>
        /// Formats all RectTransform and default Transform properties, so that they work on any device.
        /// </summary>
        /// <param name="child"></param>
        private void FormatExtensionRectTransform(Transform child)
        {
            child.SetParent(transform);
            child.SetSiblingIndex(0);
            RectTransform rectTransform = child.GetComponent<RectTransform>();
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;

            child.transform.localScale = Vector3.one;
            child.transform.localPosition = Vector3.zero;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
        }
    }
}


