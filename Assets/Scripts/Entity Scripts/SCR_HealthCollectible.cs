using Entities.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class SCR_HealthCollectible : BaseCollectible
    {
        [Header("HEALTH COLLECTIBLE PROPERTIES")]
        [SerializeField] bool _recoverPercentage;
        [SerializeField] [Range(0.1f, 1)] float _percentageAmount;
        [SerializeField] [Range(0, 500)] int _fixedRecoveryAmount;

        protected override void OnPlayerCollided(SCR_PlayerMovement playerMovement)
        {
            if (_recoverPercentage){
                playerMovement.HitboxComponent.RecoverHealth(_percentageAmount);
            }
            else{
                playerMovement.HitboxComponent.RecoverHealth(_fixedRecoveryAmount);
            }
            Debug.Log("Health has been recovered");
        }

    }


    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class BaseCollectible : MonoBehaviour
    {
        protected abstract void OnPlayerCollided(SCR_PlayerMovement playerMovement);
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetType(out SCR_PlayerMovement playerMovement) != null) { 
                OnPlayerCollided(playerMovement); 
            }
        }
    }
}

