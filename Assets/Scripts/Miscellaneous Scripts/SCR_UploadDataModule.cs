using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Dialogue;
using Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.TerrainUtils;
using UnityEngine.UI;

public class SCR_UploadDataModule : MonoBehaviour
{
    [SerializeField] private GameObject saveDataObject;
    [SerializeField] private string formURl = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSeERGsKZcJua20tXoz7GYX-DF57v0op2Lt07b2qp7UY9Ri-pA/formResponse";
    [SerializeField] private string entryURl = "entry.1448748991";

    private void OutputAnswerLog(Exam exam)
    {
        if (exam == null) { return; }

        string outputLog = "";

        int count = 0;
        foreach (ExamQuestion examQuestion in exam.examQuestions)
        {
            string question = $"Q{++count} | {examQuestion.question}";
            string selectedChoiceString;
            string correctChoiceString;
            string resultString = "The answer inputted is " + (examQuestion.IsCorrectAnswer() ? "correct!" : "incorrect!");

            if (examQuestion.IsMultipleChoiceInput())
            {
                string inputtedChoice = examQuestion.multipleChoiceQuestion.selectedAnswerIndex == -1 ? "None Selected" :
                    examQuestion.multipleChoiceQuestion.answers[examQuestion.multipleChoiceQuestion.selectedAnswerIndex];
                selectedChoiceString = "Selected Choice: " + inputtedChoice;
                correctChoiceString = $"Correct Choice: {examQuestion.multipleChoiceQuestion.answers[examQuestion.multipleChoiceQuestion.correctAnswerIndex]}"; 
            }
            else
            {
                string inputtedChoice = examQuestion.textInput.inputAnswer;
                selectedChoiceString = "Inputted Answer: " + inputtedChoice;
                correctChoiceString = $"Correct Answer: ";
                if (string.IsNullOrEmpty(examQuestion.textInput.correctAnswer))
                {
                    foreach (string answer in examQuestion.textInput.rootAnswers)
                    {
                        correctChoiceString += answer + " ";
                    }
                }
                else
                {
                    correctChoiceString += examQuestion.textInput.correctAnswer;
                }
            }

            outputLog += $"{question}\n{selectedChoiceString}\n{correctChoiceString}\n{resultString}\n\n"
                .Replace("<color=green>", "")
                .Replace("</color>", "")
                .ToUpper();
                ;
        }



        int correctAnswers = Array.FindAll(exam.examQuestions, (question) => question.IsCorrectAnswer()).Length;
        float finalGrade = correctAnswers * (100 / 16f);
        outputLog += $"OVERALL SCORE: {finalGrade}% ({(finalGrade < 40 ? "FAIL" : "PASS")})";


        File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\examResults.txt", outputLog);
        System.Diagnostics.Process.Start("notepad.exe", Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\examResults.txt");
    }

    public IEnumerator PostData(string jsonData, Exam exam = null)
    {
        if (Application.version.StartsWith("1"))
        {
            DialogueObject[] dialogue = DialogueObject.CreateDialogue(
                "As this is a final build of the project, your information will not be exported to an external Google document.",
                "Thank you for your time.");
            Action dialogueAction = () =>
            {
                OutputAnswerLog(exam);
                SCR_GeneralManager.UIManager.FindUIObject<UI_LoadScene>().LoadScene(
                    new UI_LoadScene.TransitionProperties
                    {
                        SceneName = "Splash Scene",
                        EnablePlayerControls = true
                    });
            };
            yield return new WaitForEndOfFrame();
            SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>()
                .DisplayDialogue(dialogue, dialogueAction);



            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField(entryURl, jsonData);
        UnityWebRequest webRequest = UnityWebRequest.Post(formURl, form);

        saveDataObject.SetActive(true);
        yield return webRequest.SendWebRequest();
        saveDataObject.SetActive(false);

        DialogueObject[] resultingDialogue;
        Action resultingAction = null;

        switch (webRequest.result)
        {
            case UnityWebRequest.Result.Success:
                resultingDialogue = DialogueObject.CreateDialogue("Successfully saved results to external storage!",
                    "Thank you for your patience!");
                resultingAction = () =>
                {
                    SCR_GeneralManager.UIManager.FindUIObject<UI_LoadScene>().LoadScene(
                        new UI_LoadScene.TransitionProperties
                        {
                            SceneName = "Splash Scene",
                            EnablePlayerControls = true
                        });
                };
                break;
            default:

                ChoiceDialogueObject.ChoiceOption yesOption = new ChoiceDialogueObject.ChoiceOption("Yes", null,
                    onChoiceMade: () => { StartCoroutine(PostData(jsonData)); });
                ChoiceDialogueObject.ChoiceOption noOption = new ChoiceDialogueObject.ChoiceOption("No", null,
                    onChoiceMade: () =>
                    {
                        SCR_GeneralManager.UIManager.FindUIObject<UI_LoadScene>().LoadScene(
                            new UI_LoadScene.TransitionProperties
                            {
                                SceneName = "Splash Scene",
                                EnablePlayerControls = true
                            });
                    });

                ChoiceDialogueObject choiceDialogue = new ChoiceDialogueObject(
                    choiceOptions: new ChoiceDialogueObject.ChoiceOption[] { yesOption, noOption },
                    nonImpactingChoice: true,
                    _speakingCharacter: "",
                    _dialogueText: "Would you like to try submitting your answers again?");

                resultingDialogue = new DialogueObject[]
                {
                    new DialogueObject("An error occured with submitting your results."),
                    new DialogueObject(webRequest.error + "."),
                    choiceDialogue,
                };

                break;
        }

        SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>()
            .DisplayDialogue(resultingDialogue, resultingAction);
    }
}
