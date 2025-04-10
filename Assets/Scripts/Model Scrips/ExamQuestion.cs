using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Models
{

    [Serializable]
    public class UserData
    {
        public bool playedProjectPolynomial;
        public List<QuestionData> questionData;

        public UserData()
        {
            playedProjectPolynomial = false;
            questionData = new List<QuestionData>();
        }
    }

    [Serializable]
    public class QuestionData
    {
        public string QuestionID;
        public bool CorrectAnswer;
        public float TimeTakenToAnswer;
    }
    
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
        [TextArea(3, 3)] public string question;
        [SerializeField] public Texture2D questionTexture;
        public MultipleChoiceInput multipleChoiceQuestion;
        public TextInput textInput;
        public float timeTakenToAnswer;

        public bool QuestionAnswered() => !string.IsNullOrEmpty(textInput.inputAnswer) || multipleChoiceQuestion.selectedAnswerIndex != -1;
        public bool IsMultipleChoiceInput() => string.IsNullOrEmpty(textInput.correctAnswer) && multipleChoiceQuestion.answers.Length == 3;

        public bool IsCorrectAnswer()
        {
            return IsMultipleChoiceInput() ? 
                multipleChoiceQuestion.IsCorrectAnswer() : textInput.IsCorrectAnswer();
        }
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
        
        public List<string> rootAnswers;

        public bool IsCorrectAnswer()
        {
            string formattedAnswer = inputAnswer.Replace(" ", "").ToLower();
            if (rootAnswers.Count > 0)
            {
                List<int> answers = new List<int>();

                foreach (string root in rootAnswers)
                {
                    answers.Add(formattedAnswer.Contains(root) ? 1 : 0);
                }
                
                return answers.Sum() == answers.Count;
                
            }
            return formattedAnswer == correctAnswer;
        }
    }
}