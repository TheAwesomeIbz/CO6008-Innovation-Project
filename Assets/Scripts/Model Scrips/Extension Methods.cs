using Cinemachine;
using Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct URLs
{
    public static string FeedbackURL => "https://forms.gle/iGFUPBaKowUrU3UNA";
}
public static class ExtensionMethods
{

    public static void InitialiseCharacterNames<T>(this T[] array, string characterName) where T : DialogueObject
    {
        foreach (DialogueObject dialogueObject in array)
        {
            dialogueObject.SetSpeakingCharacter(characterName);

            if (dialogueObject is not ChoiceDialogueObject) { continue; }
            ChoiceDialogueObject choiceDialogueObject = (ChoiceDialogueObject) dialogueObject;

            if (choiceDialogueObject.choiceOptions?.Length > 0)
            {
                foreach (ChoiceDialogueObject.ChoiceOption option in choiceDialogueObject.choiceOptions)
                {
                    if (option.ResultingDialogue?.Length > 0)
                    {
                        foreach (DialogueObject dObject in option.ResultingDialogue)
                        {
                            dObject.SetSpeakingCharacter(characterName);
                        }
                    }

                }
            }
        }
    }


    /// <summary>
    /// Returns the type of a component attached to a game object.
    /// </summary>
    /// <typeparam name="T">Generic Type</typeparam>
    /// <param name="behaviour"></param>
    /// <param name="type"></param>
    /// <returns>The type attached to the component if it exists.</returns>
    public static T GetType<T>(this Component behaviour, out T type)
    {
        type = behaviour.GetComponent<T>();
        return type;
    }

    public static bool IsOfType<T>(this Component behaviour)
    {
        return behaviour.GetComponent<T>() != null;
    }

    public static CinemachineVirtualCamera VirtualCamera(this Camera cam) => cam.GetComponent<CinemachineVirtualCamera>();

    public static float RelativeDirectionTo(this Transform transform, Transform target)
    {
        Vector2 relativePosition = transform.position - target.position;
        return Mathf.Atan2(relativePosition.y, relativePosition.x);
    }
}

public struct GlobalMasks
{
    public static LayerMask BoundaryLayerMask => LayerMask.GetMask("Boundary");
    public static LayerMask EntityLayerMask => LayerMask.GetMask("Level Entities");
}

