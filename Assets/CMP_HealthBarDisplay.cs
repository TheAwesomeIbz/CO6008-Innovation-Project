using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UI;

namespace Entities
{
    public class CMP_HealthBarDisplay : MonoBehaviour
    {
        [Header("HEALTH BAR PROPERTIES")]
        [SerializeField] CMP_HitboxComponent hitboxComponent;
        [SerializeField] RawImage canvasObject, healthObject;


        void Start()
        {
            canvasObject.gameObject.SetActive(false);

            if (!hitboxComponent)
                hitboxComponent = GetComponentInParent<CMP_HitboxComponent>();

            if (hitboxComponent)
                hitboxComponent.OnDamageEvent += HitboxComponent_OnDamageEvent;
            
        }

        private void HitboxComponent_OnDamageEvent(SCR_DamageCollider obj)
        {
            StopAllCoroutines();
            StartCoroutine(DisplayUpdatedHP());
            canvasObject.gameObject.SetActive(true);

            IEnumerator DisplayUpdatedHP()
            {
                healthObject.transform.localScale = new Vector3(hitboxComponent.HealthComponent.HealthDecimal, healthObject.transform.localScale.y);
                yield return new WaitForSeconds(0.5f);
                canvasObject.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            hitboxComponent.OnDamageEvent -= HitboxComponent_OnDamageEvent;
        }
    }

}