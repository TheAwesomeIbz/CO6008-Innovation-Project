using System;
using System.Collections;
using System.Collections.Generic;
using Dialogue;
using TMPro;
using UnityEngine;

using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class UI_SolveCubicBossUI : MonoBehaviour
    {
        [Header("CUBIC BOSS UI PROPERTIES")] 
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private RawImage image;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Transform parentAnswerObjects;
        [SerializeField] private Button submitButton;
        
        [Header("QUESTION INDEX PROPERTIES")]
        [SerializeField] private List<Questions> questions;
        [SerializeField] private List<Answers> answers;
        [SerializeField] private List<string> inputAnswers;
        [SerializeField] private int currentQuestionIndex;
        
        private List<TextMeshProUGUI> answerObjects = new List<TextMeshProUGUI>();

        private const string bossID = "BOSS3";
        private float currentTime;

        [Header("BOSS COMPLETION PROPERTIES")]
        [SerializeField] SO_Item trophyItem;
        [SerializeField] SO_Calculator calculator;
        [SerializeField] SO_ComplexExponentiationCalculator complexExponentiationCalculator;
        
        [Header("UPLOAD INDEX PROPERTIES")]
        [SerializeField] SCR_UploadDataModule uploadDataModule;

        [Header("HELP UI PROPERTIES")]
        [SerializeField] private GameObject helpUIDisplay;
        [SerializeField] private TextMeshProUGUI titleUI;
        [SerializeField] private RawImage imageUI;
        [SerializeField] private Button[] interactableButtons;



        void Start()
        {
            foreach (Transform child in parentAnswerObjects)
            {
                answerObjects.Add(child.GetComponentInChildren<TextMeshProUGUI>());
                child.gameObject.SetActive(false);
            }


            currentQuestionIndex = 0;
            inputAnswers = new List<string>();
            currentTime = Time.time;
            UpdateQuestion();

            SCR_DialogueManager.OnDialogueStartEvent += SCR_DialogueManager_OnDialogueStartEvent;
            SCR_DialogueManager.OnDialogueEndEvent += SCR_DialogueManager_OnDialogueEndEvent;

        }

       
        void Update()
        {
            
            if (SCR_GeneralManager.PlayerInputManager.Submit.PressedThisFrame() && 
                currentQuestionIndex < questions.Count) {
                OnContinueButtonPressed();
            }

        }

        private void OnDestroy()
        {
            SCR_DialogueManager.OnDialogueStartEvent -= SCR_DialogueManager_OnDialogueStartEvent;
            SCR_DialogueManager.OnDialogueEndEvent -= SCR_DialogueManager_OnDialogueEndEvent;
        }

        private void SCR_DialogueManager_OnDialogueEndEvent()
        {
            if (currentQuestionIndex < questions.Count)
            {
                SetButtonActivity(true);
            }
            
        }

        private void SCR_DialogueManager_OnDialogueStartEvent(DialogueObject[] obj)
        {
            SetButtonActivity(false);
        }

        private void SetButtonActivity(bool buttonActivity)
        {
            foreach (Button button in interactableButtons)
            {
                button.interactable = buttonActivity;
            }
            submitButton.interactable = buttonActivity;
            inputField.interactable = buttonActivity;
        }
        /// <returns>Whether the input is empty or not</returns>
        private bool IsEmptyInput()
        {
            if (string.IsNullOrEmpty(inputField.text))
            {
                SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(DialogueObject.CreateDialogue(
                    "Please enter in an answer within this field before you continue."), OnDialogueEnd: () =>
                {
                    submitButton.interactable = true;
                    inputField.interactable = true;
                    EventSystem.current.SetSelectedGameObject(inputField.gameObject);
                });
                return true;
            }
            return false;
        }

        /// <returns>Whether the answer provided is valid or not</returns>
        private bool IsValidAnswer(string formattedAnswer)
        {
            bool validAnswer = answers[currentQuestionIndex].ValidAnswer(formattedAnswer) &&
                               !inputAnswers.Contains(formattedAnswer);
            ;
            if (!validAnswer)
            {
                SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(DialogueObject.CreateDialogue(
                    $"Unfortunately {inputField.text} is incorrect.", "Please recheck or redo your calculations again."), OnDialogueEnd: () =>
                {
                    submitButton.interactable = true;
                    inputField.interactable = true;
                    EventSystem.current.SetSelectedGameObject(inputField.gameObject);
                });
            }
            return validAnswer;
        }
        
        /// <summary>
        /// Called to continue on with answering the question
        /// </summary>
        public void OnContinueButtonPressed()
        {
            submitButton.interactable = false;
            inputField.interactable = false;
            
            if (IsEmptyInput()) { return;}
            
            string formattedAnswer = inputField.text
                .ToLower()
                .Replace(" ", "");
            
            if (!IsValidAnswer(formattedAnswer)) { return;}
            OnAnsweredSuccessfully(formattedAnswer);
        }

        
        private void OnAnsweredSuccessfully(string formattedAnswer)
        {
            inputAnswers.Add(formattedAnswer);
            answerObjects[currentQuestionIndex].text += formattedAnswer;
            inputField.text = "";
            currentQuestionIndex++;
            
            foreach (TextMeshProUGUI answerObject in answerObjects)
            {
                Transform answerObjectParent = answerObject.transform.parent;
                answerObjectParent.gameObject.SetActive(answerObjectParent.GetSiblingIndex() < currentQuestionIndex);
            }

            UpdateQuestion();
        }

        private void UpdateQuestion()
        {
            if (currentQuestionIndex >= questions.Count)
            {
                StartCoroutine(OnCubicBossCompleted());
                return;
            }
            
            titleText.text = questions[currentQuestionIndex].Title;
            descriptionText.text = questions[currentQuestionIndex].Description;
            image.texture = questions[currentQuestionIndex].ImageTexture;
            submitButton.interactable = true;
            inputField.interactable = true;
            EventSystem.current.SetSelectedGameObject(inputField.gameObject);
        }


        /// <summary>
        /// Called when the boss is completed and the cubic equation is solved.
        /// Adds the boss to the current 
        /// </summary>
        IEnumerator OnCubicBossCompleted()
        {
            yield return new WaitForSeconds(1);
            foreach (Button button in interactableButtons)
            {
                button.interactable = false;
            }

            LevelData existingLevel =
                SCR_GeneralManager.LevelManager.GetLevelInformation.Find(lvl => lvl.LevelID == bossID);

            //remove the existing level if the time achieved is less than the current time
            float elapsedTime = Time.time - currentTime;
            if (existingLevel != null && elapsedTime < existingLevel.LevelCompletedTime) {
                SCR_GeneralManager.LevelManager.GetLevelInformation.Remove(existingLevel);
            }
            
            //add it to the level information and save progress
            LevelData levelData = new LevelData(bossID, elapsedTime);
            SCR_GeneralManager.LevelManager.GetLevelInformation.Add(levelData);
            
            SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(DialogueObject.CreateDialogue(
                "Congratulations on solving the final boss! Your journey is over!",
                $"You have successfully finished PROJECT POLYNOMIAL VERSION {Application.unityVersion}!",
                "Your save data will be analysed and recorded for future testing. Thank you very much for your time and feedback!"), 
                OnDialogueEnd: () =>
                {
                    StartCoroutine(OnCompletionDialogueFinish());
                });


            
            IEnumerator OnCompletionDialogueFinish()
            {
                //save the player's information with the SetDefaultPlayerData() function
                //done to save the player's progress to the original map and starting position

                SCR_GeneralManager.InventoryManager.AddItem(trophyItem);
                SavingOperations.SaveInformation(SCR_GeneralManager.Instance.PlayerData.SetDefaultPlayerData);
                string jsonData = JsonUtility.ToJson(SavingOperations.LoadInformation());
                
                //upload the data to an external database
                yield return uploadDataModule.PostData(jsonData);
            }
            
            
        }

        public void OnNormalCalculatorPressed()
        {
            calculator?.UseItem();
            EventSystem.current.SetSelectedGameObject(null);
        }
        public void OnComplexCalculatorButtonPressed()
        {
            complexExponentiationCalculator?.UseItem();
            EventSystem.current.SetSelectedGameObject(null);
        }


        private string[] GetHint()
        {
            return currentQuestionIndex switch
            {
                0 => new string[]
                {
                    "Press the calculator button and input the cube root of 1 (i.e. ³√1).",
                    "You will need to go into the scientific calculator mode to do this.",
                },
                1 => new string[]
                {
                    "You are given the equation for z² + z + 1. Use the quadratic formula to find the roots of this equation.",
                    "The quadratic formula is given by z = (-b ± √(b² - 4ac)) / 2a, where a, b and c = 1."
                },
                2 => new string[]
                {
                    "You are given the equation for z² + z + 1. Use the quadratic formula to find the roots of this equation.",
                    "The quadratic formula is given by z = (-b ± √(b² - 4ac)) / 2a, where a, b and c = 1.",
                    "Use the other root provided by the quadratic formula different to the one you previously entered."
                },
                3 => new string[] 
                {
                    "Do each part of the expression separately.",
                    "Firstly, calculate -(b³ / 27a³) => replace a with 1 and b with -6. Do not forget the negative sign at the start of the sum.",
                    "Secondly, calculate (bc / 6a²) => replace a with 1, b with -6 and c with -151",
                    "Thirdly, calculate -(d - 2a) => replace a with 1 and d with 780. Do not forget the negative sign at the start of the sum.",
                    "Lastly, add all of these values together to get the final sum.",
                },
                4 => new string[]
                {
                    "Do each part of the expression separately.",
                    "Firstly, calculate (c / 3a) => replace a with 1 and c with -151.",
                    "Secondly, calculate -(b² / 9a²) => replace a with 1 and b with -6. Do not forget the negative sign at the start of the sum.",
                    "Lastly, add all of these values together to get the final sum."
                },
                5 => new string[]
                {
                    "Do each part of the expression separately.",
                    "Square the results of part1 => (-231)².",
                    "Cube the results of part2 => (-54.333)³.",
                    "Add these two sums together and find the cube root of the calculated result. i.e ³√(-231)² + (-54.333)³.",
                    "Alternatively, you can put the sum within the complex exponentiation calculator and use the principal root."
                },
                6 => new string[]
                {
                    "Calculate -b / 3a. Replace a with 1 and b with -6."
                },
                7 => new string[]
                {
                    "Add the results of these two parts together. This equates to -231 + 327.161i.",
                    "Input this value into the complex exponentiation calculator and use the principal root.",
                     "This answer will exist in the form of a + bi, where a and b are real numbers.",
                },
                8 => new string[]
                {
                    "Add the results of these two parts together. This equates to -231 + 327.161i.",
                    "Input this value into the complex exponentiation calculator and use the principal root.",
                    "This answer will exist in the form of a + bi, where a and b are real numbers.",
                    "Use the complex conjugate value of the previous value to find the correct answer.",
                    "It is possible to receive the correct answer if you input  -231 - 327.161i, however the principal root will be different."

                },
                9 => new string[]
                {
                    "Because ω¹ is equal to 1, you can add all segments together to get your first root.",
                    "Adding all the segments results in 2 + (5.5+4.907i) + (5.5-4.907i).",
                    "Remember to refer to the complex addition and subtraction rules by pressing the associated button if you are stuck.",
                },
                10 => new string[]
                {
                    "Because ω² is the second root of unity, this number must be multiplied by the results of the second segment.",
                    "Additionally, ω³ must be multiplied by the third segment.",
                    "Remember to refer to the complex multiplication and division rules by pressing the associated button if you are stuck.",
                    "Also, remember to refer to the complex addition and subtraction rules by pressing the associated button if you are stuck.",
                },
                11 => new string[]
                {
                    "Because ω³ is the third root of unity, this number must be multiplied by the results of the second segment.",
                    "Additionally, ω² must be multiplied by the third segment.",
                    "Remember to refer to the complex multiplication and division rules by pressing the associated button if you are stuck.",
                    "Also, remember to refer to the complex addition and subtraction rules by pressing the associated button if you are stuck.",
                },
                _ => new string[] { }
            };
        }
        public void OnHintButtonPressed()
        {
            EventSystem.current.SetSelectedGameObject(null);

            submitButton.interactable = false;
            inputField.interactable = false;
            foreach (Button button in interactableButtons)
            {
                button.interactable = false;
            }

            SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(DialogueObject.CreateDialogue(
                GetHint()), OnDialogueEnd: () =>
            {
                foreach (Button button in interactableButtons)
                {
                    button.interactable = true;
                }
                submitButton.interactable = true;
                inputField.interactable = true;
            });
        }




        public void OnHelpUIButtonPressed(Texture texture)
        {
            titleUI.text = texture.name;
            imageUI.texture = texture;
            helpUIDisplay.gameObject.SetActive(true);
        }

        public void OnReturnButtonPressed()
        {
            helpUIDisplay.gameObject.SetActive(false);
        }
        
        
        /// <summary>
        /// Struct that contains the properties for a question for the cubic equation
        /// </summary>
        [Serializable] struct Questions
        {
            [SerializeField] private string title;
            [SerializeField] [TextArea(4,4)] private string description;
            [SerializeField] private Texture2D imageTexture;
            
            public string Title => title;
            public string Description => description;
            public Texture2D ImageTexture => imageTexture;
        }

        
        /// <summary>
        /// Struct that contains the list of possible answers acceptable for a given question
        /// </summary>
        [Serializable] struct Answers
        {
            [SerializeField] private List<string> possibleAnswers;

            public bool ValidAnswer(string answer)
            {
                return possibleAnswers.Contains(answer);
            }
        }
        
    }
}
