using Entities;
using Entities.Boss;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityEngine.UI
{
    public class SCR_BossUI : MonoBehaviour
    {
        [Header("BOSS UI PROPERTIES")]
        [SerializeField] Transform _bossHealthBar;
        SCR_BossEntity _bossEntity;
        void Start()
        {
            _bossEntity = FindObjectOfType<SCR_BossEntity>();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateHealthBar();
        }

        private void UpdateHealthBar()
        {
            if (_bossHealthBar == null || _bossEntity == null) { 
                gameObject.SetActive(false);
                return; 
            }

            CMP_HealthComponent bossHealthComponent = _bossEntity.BossHealthComponent;
            _bossHealthBar.localScale = new Vector3(bossHealthComponent.HP / (float)bossHealthComponent.MaxHP, _bossHealthBar.localScale.y);

        }
    }

    
}
