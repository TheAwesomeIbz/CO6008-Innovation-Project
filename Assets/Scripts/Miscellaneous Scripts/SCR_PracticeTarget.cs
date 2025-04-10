using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    public class SCR_PracticeTarget : MonoBehaviour
    {
        [SerializeField] SCR_TutorialSceneManager tutorialSceneManager;
        CMP_HitboxComponent hitboxComponent;
        Animator targetAnimator;
        // Start is called before the first frame update
        void Awake()
        {
            hitboxComponent = GetComponent<CMP_HitboxComponent>();
            targetAnimator = GetComponent<Animator>();
        }

        private void HitboxComponent_OnZeroHPEvent(SCR_DamageCollider obj)
        {
            tutorialSceneManager?.UpdateTargets();
            gameObject.SetActive(false);
        }
        

        private void OnEnable()
        {
            if (hitboxComponent)
            {
                hitboxComponent.OnZeroHPEvent += HitboxComponent_OnZeroHPEvent;
                hitboxComponent.OnDamageEvent += HitboxComponentOnOnDamageEvent;
            }
            
        }

        private void HitboxComponentOnOnDamageEvent(SCR_DamageCollider obj)
        {
            StopAllCoroutines();
            StartCoroutine(DamageCoroutine());
            
            IEnumerator DamageCoroutine()
            {
                targetAnimator?.SetBool("isDamaged", true);
                yield return new WaitForSeconds(0.25f);
                targetAnimator?.SetBool("isDamaged", false);
            }
        }

        private void OnDisable()
        {
            hitboxComponent.OnZeroHPEvent -= HitboxComponent_OnZeroHPEvent;
            hitboxComponent.OnDamageEvent -= HitboxComponentOnOnDamageEvent;
        }
    }
}