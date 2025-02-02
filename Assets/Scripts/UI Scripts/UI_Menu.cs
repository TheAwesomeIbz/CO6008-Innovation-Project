using Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
    public class UI_Menu : MonoBehaviour
    {
        [Header("MENU UI PROPERTIES")]
        [SerializeField] private GameObject parentGameObject;
        [SerializeField] private Transform parentButtonObject;
        private List<Button> buttons;

        [field: Header("MENU UI STATE")]
        [field : SerializeField] public bool MenuEnabled { get; private set; }


        [Header("CONTROL UI PROPERTIES")]
        [SerializeField] private GameObject parentControlGameObject;

        [Header("INVENTORY UI PROPERTIES")]
        [SerializeField] private SCR_InventoryUI inventoryUI;

        void Start()
        {
            MenuEnabled = false;
            parentGameObject.SetActive(false);
            parentControlGameObject.gameObject.SetActive(false);

            buttons = new List<Button>();
            foreach (Transform child in parentButtonObject){
                buttons.Add(child.GetComponent<Button>());
            }
        }
        
        /// <summary>
        /// Set all buttons to be interactable or not
        /// </summary>
        /// <param name="state">The state of the button's intertactability</param>
        public void SetButtonInteractability(bool state)
        {
            foreach (Button button in buttons)
            {
                button.interactable = state;
            }
        }
        
        private bool MenuButtonPressed() { return Input.GetKeyDown(KeyCode.Escape) && SCR_PlayerInputManager.PlayerControlsEnabled; }

        private void ToggleMenu()
        {
            MenuEnabled = !MenuEnabled;
            parentGameObject.SetActive(MenuEnabled);

            SCR_PlayerInputManager.PlayerControlsEnabled = !MenuEnabled;
        }

        void Update()
        {
            if (!MenuButtonPressed()) { return; }
            ToggleMenu();

        }


        public void OnReturnButtonPressed()
        {
            ToggleMenu();
        }

        public void OnControlsButtonPressed()
        {
            parentControlGameObject.gameObject.SetActive(true);
        }

        public void OnReturnFromControlsButtonPressed()
        {
            parentControlGameObject.gameObject.SetActive(false);
        }

        public void OnInventoryButtonPressed()
        {
            inventoryUI.OpenInventory();
            SetButtonInteractability(false);
        }

        public void OnReturnFromInventory()
        {
            SetButtonInteractability(true);
        }

        public void OnSettingsButtonPressed()
        {
            SetButtonInteractability(false);
            SCR_GeneralManager.UIManager.FindUIObject<UI_SettingsUI>().DisplaySettingsUI(true);
        }

        public void OnSaveGameButtonPressed()
        {
            SetButtonInteractability(false);

            System.Action onYesChoice = () =>
            {
                SavingOperations.SaveInformation();
                
            };

            ChoiceDialogueObject.ChoiceOption yesChoice = new ChoiceDialogueObject.ChoiceOption("YES", 
                DialogueObject.CreateDialogue($"{SCR_GeneralManager.Instance.PlayerData.PlayerName}'s data has been saved successfully!"), onYesChoice);
            ChoiceDialogueObject.ChoiceOption noChoice = new ChoiceDialogueObject.ChoiceOption("NO", null, null);

            string text = $"Do you wish to save {SCR_GeneralManager.Instance.PlayerData.PlayerName}'s progress?";

            ChoiceDialogueObject dialogueObject = new ChoiceDialogueObject(new ChoiceDialogueObject.ChoiceOption[] { yesChoice, noChoice }, true, "", text);
            ChoiceDialogueObject[] dialogue = new ChoiceDialogueObject[] { dialogueObject };
            SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(dialogue, () => { SetButtonInteractability(true); }, false);
        }

        public void OnQuitGameButtonPressed()
        {
            System.Action warpToTitleScreen = () =>
            {
                SCR_GeneralManager.UIManager.FindUIObject<UI_LoadScene>().LoadScene(new UI_LoadScene.TransitionProperties
                {
                    SceneName = "Title Scene",
                    EnablePlayerControls = true,
                });
            };

            System.Action onYesChoice = () =>
            {
                SavingOperations.SaveInformation();
                warpToTitleScreen();
            };

            System.Action onCancelChoice = () => {
                SetButtonInteractability(true);
            };


            SetButtonInteractability(false);


            DialogueObject[] onYesChoiceText = new DialogueObject[] { new DialogueObject($"{SCR_GeneralManager.Instance.PlayerData.PlayerName}'s data has been saved successfully!").SetOnSentenceFinishedAction(onYesChoice) };
            ChoiceDialogueObject.ChoiceOption yesChoice = new ChoiceDialogueObject.ChoiceOption("YES", onYesChoiceText, null);
            ChoiceDialogueObject.ChoiceOption noChoice = new ChoiceDialogueObject.ChoiceOption("NO", null, warpToTitleScreen);
            ChoiceDialogueObject.ChoiceOption cancelChoice = new ChoiceDialogueObject.ChoiceOption("CANCEL", null, onCancelChoice);

            string text = $"Do you wish to save {SCR_GeneralManager.Instance.PlayerData.PlayerName}'s progress before quitting?";

            ChoiceDialogueObject dialogueObject = new ChoiceDialogueObject(new ChoiceDialogueObject.ChoiceOption[] { yesChoice, noChoice, cancelChoice }, true, "", text);
            ChoiceDialogueObject[] dialogue = new ChoiceDialogueObject[] { dialogueObject };
            SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(dialogue, null, false);
        }
    }
}
