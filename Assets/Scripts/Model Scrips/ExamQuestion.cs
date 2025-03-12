using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class Exam
    {
        public ExamQuestion[] examQuestions;

        public ExamQuestion this[int index]
        {
            get { 
                return examQuestions[index];
            }
            set
            {
                examQuestions[index] = value;
            }
        }
    }

    [Serializable]
    public class ExamQuestion
    {
        [TextArea(2, 2)] public string question;
        [SerializeField] public Texture2D questionTexture;
        public MultipleChoiceInput multipleChoiceQuestion;
        public TextInput textInput;
        public float timeTakenToAnswer;

        public bool QuestionAnswered() => !string.IsNullOrEmpty(textInput.inputAnswer) || multipleChoiceQuestion.selectedAnswerIndex != -1;
        public bool IsMultipleChoiceInput() => string.IsNullOrEmpty(textInput.correctAnswer) && multipleChoiceQuestion.answers.Length == 3;
    }

    [Serializable]
    public class MultipleChoiceInput
    {
        [TextArea(2,2)] public string[] answers;
        [Range(-1, 2)] public int selectedAnswerIndex = -1;
        [Range(0, 2)] public int correctAnswerIndex;

        public bool IsCorrectAnswer() => selectedAnswerIndex == correctAnswerIndex;

    }

    [Serializable]
    public class TextInput
    {
        [TextArea(2, 2)] public string inputAnswer;
        [TextArea(2, 2)] public string correctAnswer;

        public bool IsCorrectAnswer() => inputAnswer.Replace(" ", "").ToLower() == correctAnswer.Replace(" ", "").ToLower();
    }
}