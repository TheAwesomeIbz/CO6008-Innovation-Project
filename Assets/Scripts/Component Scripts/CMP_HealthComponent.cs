using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Entities
{
    public class CMP_HealthComponent : MonoBehaviour
    {
        [Header("HP PROPERTIES")]
        [SerializeField] int _HP = 100;
        [SerializeField] int _maxHP = 100;
        public int HP => _HP;
        public int MaxHP => _maxHP;

        public bool IsHalfHP => _HP <= _maxHP / 2;

        private void Start()
        {
            _maxHP = _HP;
        }

        /// <summary>
        /// Recover amount of HP
        /// </summary>
        /// <param name="hpGain">Amount gained</param>
        public void GainHP(int hpGain)
        {
            _HP = Mathf.Clamp(_HP + Mathf.Abs(hpGain), 0, _maxHP);
        }

        /// <summary>
        /// Lose amount of HP
        /// </summary>
        /// <param name="hpGain">Amount lost</param>
        public void LoseHP(int hpGain)
        {
            _HP = Mathf.Clamp(_HP - Mathf.Abs(hpGain), 0, _maxHP);
        }
    }

}
