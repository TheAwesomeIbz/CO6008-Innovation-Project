using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityEngine
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Inventory Items/Base Item")]
    public class SO_Item : ScriptableObject
    {
        [field : Header("BASE ITEM PROPERTIES")]
        [field: SerializeField] public Texture2D SpriteIcon { get; protected set; }
        [field: SerializeField] [field: TextArea(3,3)] public string SpriteDescription { get; protected set; }
    }

}
