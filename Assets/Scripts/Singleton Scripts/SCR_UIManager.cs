using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityEngine.UI
{
    public class SCR_UIManager : MonoBehaviour
    {
        public T FindUIObject<T>() where T : MonoBehaviour
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                T childComponent = transform.GetChild(i).GetComponent<T>();
                if (childComponent != null) { return childComponent; }
            }
            return null;
        }


        void Start()
        {
            GetComponent<Canvas>().worldCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}


