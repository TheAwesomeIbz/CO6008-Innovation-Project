using System;
using System.Collections;
using System.Collections.Generic;
using Dialogue;
using Entities;
using UnityEngine;

namespace Level
{
    public class SCR_ShootingObject : SCR_EntityShooting
    {
        [Header("CONSTANT SHOOTING PROPERTIES")] 
        [SerializeField] [Range(0, 10)]
        private float shootingBurstCooldown;

        private float randomInitialisationCooldown;

        [SerializeField] private bool destroyable;
        [SerializeField] private bool randomisePeriod = true;
        CMP_HitboxComponent hitboxComponent;

        private ShootingState shootingState;
        public CMP_HitboxComponent HitboxComponent => hitboxComponent;

        [Header("SPRITE PROPERTIES")]
        [SerializeField] Sprite defaultSprite;
        [SerializeField] Sprite destroyableSprite;
        private SpriteRenderer objectSpriteRenderer;
        
        private new ParticleSystem particleSystem;
        private SpriteRenderer particleSystemSpriteRenderer;
        private Color particleColor;
        

        protected override void Awake()
        {
            base.Awake();
            if (randomisePeriod) {
                randomInitialisationCooldown = UnityEngine.Random.Range(0.5f, 1.5f);
            }
            
            if (!hitboxComponent)
                hitboxComponent = GetComponentInChildren<CMP_HitboxComponent>();

            if (hitboxComponent && destroyable) {
                hitboxComponent.OnZeroHPEvent += OnZeroHealth;
            }
            else
            {
                CMP_HealthBarDisplay healthBarDisplay = GetComponentInChildren<CMP_HealthBarDisplay>();
                Destroy(healthBarDisplay.gameObject);
            }
            
            shootingState = ShootingState.NOTHING;

            
            
            if (weaponProperties)
            {
                objectSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
                particleSystem = weaponProperties.ParticleSystem;
                particleSystemSpriteRenderer = weaponProperties.SpriteRenderer;
            }
            
            
            
            SCR_DialogueManager.OnDialogueStartEvent += SCR_DialogueManagerOnOnDialogueStartEvent;
            SCR_DialogueManager.OnDialogueEndEvent += SCR_DialogueManagerOnOnDialogueEndEvent;
        }

        private void SCR_DialogueManagerOnOnDialogueEndEvent()
        {
            shootingState = ShootingState.NOTHING;
        }

        private void SCR_DialogueManagerOnOnDialogueStartEvent(DialogueObject[] obj)
        {
            StopAllCoroutines();
            shootingState = ShootingState.DISABLED;
            
        }

        IEnumerator ShootingBurstCoroutine()
        {
            float randomTime = randomisePeriod ? UnityEngine.Random.Range(0, shootingBurstCooldown) : shootingBurstCooldown;
            shootingState = ShootingState.SHOOTING;
            yield return new WaitForSeconds(shootingBurstCooldown + randomTime);
            shootingState = ShootingState.PAUSED;
            yield return new WaitForSeconds(shootingBurstCooldown);
            shootingState = ShootingState.NOTHING;
        }
        protected override void Update()
        {
            if (weaponProperties)
            {
                objectSpriteRenderer.sprite = destroyable ? destroyableSprite : defaultSprite;
                particleSystemSpriteRenderer.color = destroyable ? Color.red : Color.white;
                ParticleSystem.MainModule settings = particleSystem.main;
                settings.startColor = new ParticleSystem.MinMaxGradient( destroyable ? Color.red : Color.white );
            }

            if (!SCR_GeneralManager.LevelManager.playerMovement) { return; }
            if (shootingState == ShootingState.DISABLED) { return; }
            
            Vector3 distance = transform.position - SCR_GeneralManager.LevelManager.playerMovement.transform.position;
            if (distance.sqrMagnitude > 550) { return; }
            
            if (randomInitialisationCooldown >= 0)
            {
                randomInitialisationCooldown -= Time.deltaTime;
                return;
            }
            
            if (shootingState == ShootingState.NOTHING)
            {
               StopAllCoroutines();
               StartCoroutine(ShootingBurstCoroutine());
            }

            if (shootingState == ShootingState.SHOOTING)
            {
                EntityShootingUpdate(SCR_GeneralManager.LevelManager.playerMovement.transform);
                base.Update();
            }
            
        }

        private void OnDestroy()
        {
            hitboxComponent.OnZeroHPEvent -= OnZeroHealth;
            SCR_DialogueManager.OnDialogueStartEvent -= SCR_DialogueManagerOnOnDialogueStartEvent;
            SCR_DialogueManager.OnDialogueEndEvent -= SCR_DialogueManagerOnOnDialogueEndEvent;
        }

        private void OnZeroHealth(SCR_DamageCollider damageCollider)
        {
            if (destroyable){
                Destroy(gameObject);
            }
        }

        
        
        enum ShootingState
        {
            NOTHING,
            SHOOTING,
            PAUSED,
            DISABLED,
        }
    }
}
