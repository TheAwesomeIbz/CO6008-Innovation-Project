using System;
using System.Collections;
using System.Collections.Generic;
using Dialogue;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SCR_UploadDataModule : MonoBehaviour
{
    [SerializeField] private GameObject saveDataObject;
    [SerializeField] private string formURl = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSeERGsKZcJua20tXoz7GYX-DF57v0op2Lt07b2qp7UY9Ri-pA/formResponse";
    [SerializeField] private string entryURl = "entry.1448748991";


    public IEnumerator PostData(string jsonData)
    {
        
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
