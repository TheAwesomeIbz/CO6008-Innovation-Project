using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    public class SCR_Explosion : MonoBehaviour
    {
        public void OnAnimationFinished()
        {
            Destroy(gameObject);
        }
    }

}