using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class SCR_DamageCollider : MonoBehaviour
    {
        //TODO : call the damage interface on the objects in contact with the collider
        private void OnTriggerEnter2D(Collider2D collision)
        {
            print($"{collision} with 2D");
        }
        void Start()
        {

        }

        void Update()
        {

        }
    }
}

