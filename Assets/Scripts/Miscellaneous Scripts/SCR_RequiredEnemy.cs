using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using Level;
using UnityEngine;

public class SCR_RequiredEnemy : MonoBehaviour
{
    [SerializeField] private SCR_EnemyProgression enemyProgression;

    CMP_HitboxComponent hitboxComponent;
    private void Start()
    {
        hitboxComponent = GetComponentInChildren<CMP_HitboxComponent>();

        if (!hitboxComponent)
            hitboxComponent = GetComponent<CMP_HitboxComponent>();
        
        if (hitboxComponent)
            hitboxComponent.OnZeroHPEvent += HitboxComponentOnOnZeroHPEvent;
    }

    private void HitboxComponentOnOnZeroHPEvent(SCR_DamageCollider obj)
    {
        enemyProgression?.UpdateEnemyProgression();
    }

    private void OnDestroy()
    {
        hitboxComponent.OnZeroHPEvent -= HitboxComponentOnOnZeroHPEvent;
        
    }
}
