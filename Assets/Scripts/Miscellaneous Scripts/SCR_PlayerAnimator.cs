using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Player
{
    public class SCR_PlayerAnimator : MonoBehaviour
    {

        SCR_PlayerMovement playerMovement;
        SCR_PlayerShooting playerShooting;
        Animator animator;

        [SerializeField] RuntimeAnimatorController animator16BitController, animator32BitController;

        const string isMoving = "isMoving";
        const string isShooting = "isShooting";
        const string isStunned = "isStunned";
        const string isDead = "isDead";
        const string isDodging = "isDodging";
        void Start()
        {
            animator = GetComponent<Animator>();
            playerMovement = GetComponent<SCR_PlayerMovement>();
            playerShooting = GetComponent<SCR_PlayerShooting>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!animator) { return; }
            animator.runtimeAnimatorController = playerMovement.PlayerLevel < PlayerLevel.REAL_LEVEL ? animator16BitController : animator32BitController;

            animator.SetBool(isMoving, playerMovement.IsMoving);
            animator.SetBool(isShooting, !playerShooting.CanShoot);
            animator.SetBool(isDead, playerMovement.HitboxComponent.HealthComponent.HP < 1);

            if (playerMovement.PlayerLevel >= PlayerLevel.REAL_LEVEL)
            {
                animator.SetBool(isDodging, playerMovement.IsDodging);
            }
            

        }
    }

}