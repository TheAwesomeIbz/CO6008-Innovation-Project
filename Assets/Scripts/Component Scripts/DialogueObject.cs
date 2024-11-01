using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    [System.Serializable]
    public class DialogueObject
    {
        [SerializeField] private string _speakingCharacter;
        [SerializeField] [TextArea(2, 2)] private string _dialogueText;
        [SerializeField] private AudioClip _speakingSFX;

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

    }

}