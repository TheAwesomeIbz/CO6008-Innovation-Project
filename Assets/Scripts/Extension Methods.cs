using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{

    public static T GetType<T>(this Behaviour behaviour, out T type)
    {
        type = behaviour.GetComponent<T>();
        return type;
    }

    public static bool IsOfType<T>(this Behaviour behaviour)
    {
        return behaviour.GetComponent<T>() != null;
    }

}
