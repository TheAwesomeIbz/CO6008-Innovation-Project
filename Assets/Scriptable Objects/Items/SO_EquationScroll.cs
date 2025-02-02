using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Scriptable Objects/Inventory Items/Equation Scroll")]
public class SO_EquationScroll : SO_Item
{
    public void UseItem()
    {
        Debug.Log($"{name} Item Used");
    }
}
public interface iUsableItem
{
    public void UseItem();
}
