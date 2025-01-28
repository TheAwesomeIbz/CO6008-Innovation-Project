using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dialogue
{
    /// <summary>
    /// Base dialogue object that entities use to communicate with the player.
    /// </summary>
    [Serializable] public class DialogueObject
    {
        public static DialogueObject[] CreateDialogue(params string[] dialogueText)
        {
            List<DialogueObject> dialogueObjects = new List<DialogueObject>();
            foreach (string dialogue in dialogueText)
            {
                dialogueObjects.Add(new DialogueObject(dialogue));
            }
            return dialogueObjects.ToArray();
        }

        [SerializeField] protected string _speakingCharacter;
        [SerializeField][TextArea(2, 2)] protected string _dialogueText;
        [SerializeField] protected AudioClip _speakingSFX;

        /// <summary>
        /// The name of the character that is speaking
        /// </summary>
        public string SpeakingCharacter => _speakingCharacter;

        /// <summary>
        /// The text of the dialogue that should be displayed within the dialogue box.
        /// </summary>
        public string DialogueText => _dialogueText;

        /// <summary>
        /// The sound effect that would be played anytime the character speaks.
        /// </summary>
        public AudioClip SpeakingSFX => _speakingSFX;

        public DialogueObject(string _dialogueText)
        {
            this._dialogueText = _dialogueText;
            _speakingCharacter = null;
            _speakingSFX = null;
        }

        public DialogueObject(string _speakingCharacter, string _dialogueText, AudioClip _speakingSFX = null)
        {
            this._speakingCharacter = _speakingCharacter;
            this._dialogueText = _dialogueText;
            this._speakingSFX = _speakingSFX;
        }

        public void SetSpeakingCharacter(string text) => _speakingCharacter = text;
        public static DialogueObject[] NullDialogueObject { get; } = new DialogueObject[] { new DialogueObject("NullDialogueException") };
    }

    /// <summary>
    /// Choice dialogue object that can be used to ask the player multiple questions or a quiz
    /// </summary>
    [Serializable] public class ChoiceDialogueObject : DialogueObject
    {
        public ChoiceOption[] choiceOptions = new ChoiceOption[3];
        public bool NonImpactingChoice = false;

        public int CorrectChoice => Array.FindIndex(choiceOptions, choice => choice.CorrectAnswer);
        public ChoiceDialogueObject(ChoiceOption[] choiceOptions, bool nonImpactingChoice, string _speakingCharacter, string _dialogueText, AudioClip _speakingSFX = null) : base(_speakingCharacter, _dialogueText, _speakingSFX = null)
        {
            this.choiceOptions = choiceOptions;
            NonImpactingChoice = nonImpactingChoice;
        }

        /// <summary>
        /// Model class containing information about what choices to be made stored within ChoiceDialogueObject
        /// </summary>
        [Serializable] public class ChoiceOption
        {
            public ChoiceOption(string choiceText, DialogueObject[] resultingDialogue, Action onChoiceMade)
            {
                ChoiceText = choiceText;
                CorrectAnswer = false;
                ResultingDialogue = resultingDialogue;
                OnChoiceMade = onChoiceMade;
            }

            [field: SerializeField] public string ChoiceText { get; private set; }
            [field: SerializeField] public bool CorrectAnswer { get; private set; }
            [field: SerializeField] public DialogueObject[] ResultingDialogue { get; private set; }
            public Action OnChoiceMade { get; private set; }

        }

    }

    /// <summary>
    /// Model class containing information about choices made, in the format they will be saved to in the player's save data
    /// </summary>
    [Serializable] public class SavableChoice
    {
        public SavableChoice(string choiceID)
        {
            ChoiceID = choiceID;
            SelectedChoice = -1;
            TimeTakenToSelect = 0;
        }

        /// <summary>
        /// Set the selected choice and time taken to select a choice
        /// </summary>
        /// <param name="selectedChoice"></param>
        /// <param name="timeTaken"></param>
        public void SetChoice(int selectedChoice, float timeTaken) {
            SelectedChoice = selectedChoice;
            TimeTakenToSelect = timeTaken;
        } 
        [field : SerializeField] public string ChoiceID { get; private set; }
        [field: SerializeField] public int SelectedChoice { get; private set; }
        [field: SerializeField] public float TimeTakenToSelect {  get; private set; }
        
    }

    /// <summary>
    /// An object that inherits this interface can be one with dialogue instructions
    /// </summary>
    public interface IDialogueInterface
    {
        /// <summary>
        /// The dialogue instructions attached to this object
        /// </summary>
        public DialogueObject[] DialogueObjects { get; }
    }

    /// <summary>
    /// An object that inherits this interface can have questions saved to the player's save file
    /// </summary>
    public interface ISavableChoice
    {

        /// <summary>
        /// The savable choice attribites attached to this object
        /// </summary>
        public SavableChoice SavableChoice { get; }
    }

    /// <summary>
    /// An object that inherits this interface essentially acts like a quiz, with the use of a multiple choice question dialogue for the dialogue manager to recognise.
    /// </summary>
    public interface IQuizInterface
    {
        public int CorrectChoice { get; }
        /// <summary>
        /// Whether the quiz is only answerable once, whether the right answer is chosen or not.
        /// </summary>
        public bool OnlyOneChance {  get; }

        /// <summary>
        /// Method called when the right choice is made
        /// </summary>
        public void OnCorrectChoiceMade();

        /// <summary>
        /// Method called when the wrong choice is made
        /// </summary>
        public void OnIncorrectChoiceMade();
    }
}