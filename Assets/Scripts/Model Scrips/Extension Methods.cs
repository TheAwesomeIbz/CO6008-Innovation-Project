using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{

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


}

public struct GlobalMasks
{
    public static LayerMask GroundLayerMask => LayerMask.GetMask("Ground");
}

