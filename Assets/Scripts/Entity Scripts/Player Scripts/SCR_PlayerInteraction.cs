using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Player
{
    public class SCR_PlayerInteraction : MonoBehaviour
    {

        /// <summary>
        /// Trigger Interaction used to determine whether the player has interacted with an interactable object
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetType(out iInteractable interactable) == null) { return; }

            if (interactable.Interactable){
                interactable.Interact(GetComponentInParent<SCR_PlayerMovement>());
            }
            
        }

    }
}
