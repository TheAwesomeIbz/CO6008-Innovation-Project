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
        // Start is called before the first frame update
        void Awake()
        {
            hitboxComponent = GetComponent<CMP_HitboxComponent>();
            
        }

        private void HitboxComponent_OnZeroHPEvent(SCR_DamageCollider obj)
        {
            tutorialSceneManager?.UpdateTargets();
            gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            if (hitboxComponent)
            hitboxComponent.OnZeroHPEvent += HitboxComponent_OnZeroHPEvent;
        }

        private void OnDisable()
        {
            hitboxComponent.OnZeroHPEvent -= HitboxComponent_OnZeroHPEvent;
        }
    }
}