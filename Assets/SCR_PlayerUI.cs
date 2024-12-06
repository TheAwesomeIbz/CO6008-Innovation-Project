using Entities;
using Entities.Player;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace UnityEngine.UI
{
    public class SCR_PlayerUI : MonoBehaviour
    {
        [Header("HEALTH BAR PROPERTIES")]
        [SerializeField] Transform _healthBar;
        [SerializeField] TextMeshProUGUI _healthText;

        [Header("WEAPON PROPERTIES")]
        [SerializeField] Transform _weaponUIParent;
        [SerializeField] Transform _weaponUpgradeUIParent;

        UIObject _weaponUI;
        UIObject _weaponUpgradeUI;

        [Header("POWERUP PROPERTIES")]
        [SerializeField] Transform _damageUIParent;
        [SerializeField] Transform _knockbackUIParent;
        [SerializeField] Transform _agilityUIParent;

        UIObject _damageUI;
        UIObject _knockbackUI;
        UIObject _agilityUI;



        SCR_PlayerMovement _playerMovementReference;

        void Start()
        {
            _playerMovementReference = FindObjectOfType<SCR_PlayerMovement>();

            _weaponUI = new UIObject(_weaponUIParent);
            _weaponUpgradeUI = new UIObject(_weaponUpgradeUIParent);

            _damageUI = new UIObject(_damageUIParent);
            _knockbackUI = new UIObject(_knockbackUIParent);
            _agilityUI = new UIObject(_agilityUIParent);

            _weaponUpgradeUI.SetElementActivity(false);
            _damageUI.SetElementActivity(false);
            _knockbackUI.SetElementActivity(false);
            _agilityUI.SetElementActivity(false);

            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void OnSceneChanged(Scene arg0, Scene arg1)
        {
            _playerMovementReference = FindObjectOfType<SCR_PlayerMovement>();
        }

        /// <summary>
        /// Update function called every frame to update Player UI
        /// </summary>
        private void UpdateHealthBar()
        {
            if (_healthBar == null) { return; }
            
            CMP_HealthComponent playerHealthComponent = _playerMovementReference.HitboxComponent.HealthComponent;
            _healthBar.localScale = new Vector3(playerHealthComponent.HP / (float)playerHealthComponent.MaxHP, _healthBar.localScale.y);

            if (_healthText == null) { return ; }
            _healthText.text = $"{playerHealthComponent.HP} / {playerHealthComponent.MaxHP}";

        }

        /// <summary>
        /// Called in Update() to display current player powerups in UI
        /// </summary>
        private void UpdatePowerups()
        {
            _damageUI.SetElementActivity(_playerMovementReference.DamagePowerupProperty.PowerupActive);
            if (_playerMovementReference.DamagePowerupProperty.PowerupActive)
            {
                _damageUI.SetElementText($"DAMAGE ({Mathf.RoundToInt(_playerMovementReference.DamagePowerupProperty.PowerupDuration)}s)");
            }

            _knockbackUI.SetElementActivity(_playerMovementReference.KnockbackPowerupProperty.PowerupActive);
            if (_playerMovementReference.KnockbackPowerupProperty.PowerupActive)
            {
                _knockbackUI.SetElementText($"KNOCKBACK ({Mathf.RoundToInt(_playerMovementReference.KnockbackPowerupProperty.PowerupDuration)}s)");
            }

            _agilityUI.SetElementActivity(_playerMovementReference.AgilityPowerupProperty.PowerupActive);
            if (_playerMovementReference.AgilityPowerupProperty.PowerupActive)
            {
                _agilityUI.SetElementText($"AGILITY ({Mathf.RoundToInt(_playerMovementReference.AgilityPowerupProperty.PowerupDuration)}s)");
            }
        }

        /// <summary>
        /// Called in Update() to display current player weapons and powerups in UI
        /// </summary>
        private void UpdateWeaponUI()
        {
            _weaponUI.SetElementText(_playerMovementReference.WeaponProperties?.name ?? "");
        }

        // Update is called once per frame
        void Update()
        {
            if (_playerMovementReference == null) { return; }
            UpdateHealthBar();
            UpdatePowerups();
            UpdateWeaponUI();
        }

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        [System.Serializable] class UIObject
        {
            public GameObject ElementParent;

            private RawImage ElementIcon;
            private TextMeshProUGUI ElementText;

            public void SetElementActivity(bool activity) => ElementParent?.SetActive(activity);
            public void SetElementText(string text) => ElementText.text = text; 
            public void SetElementIcon(Texture2D texture2D) => ElementIcon.texture = texture2D;
            public UIObject(Transform transform)
            {
                ElementParent = transform.gameObject;
                ElementIcon = transform.GetComponentInChildren<RawImage>();
                ElementText = transform.GetComponentInChildren<TextMeshProUGUI>();
            }
        }

    }
}

