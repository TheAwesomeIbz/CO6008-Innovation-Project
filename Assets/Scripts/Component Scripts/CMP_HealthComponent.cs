using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Entities
{
    public class CMP_HealthComponent : MonoBehaviour
    {
        [Header("HP PROPERTIES")]
        [SerializeField] int _HP;
        [SerializeField] int _maxHP;
        public int HP => _HP;
        public int MaxHP => _maxHP;

        private void Start()
        {
            _maxHP = _HP;
        }

        public void GainHP(int hpGain)
        {
            _HP = Mathf.Clamp(_HP + Mathf.Abs(hpGain), 0, _maxHP);
        }

        public void LoseHP(int hpGain)
        {
            _HP = Mathf.Clamp(_HP - Mathf.Abs(hpGain), 0, _maxHP);
        }
    }

}
