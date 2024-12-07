using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SCR_InventoryManager : MonoBehaviour
{
    [field: Header("INVENTORY MANAGER PROPERTIES")]
    [field: SerializeField] public List<SO_Item> Inventory { get; private set; }

    /// <summary>
    /// Adds an item to the inventory
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Whether the item already exci</returns>
    public bool AddItem(SO_Item item)
    {
        bool itemExist = Inventory.Contains(item);
        if (!itemExist) { Inventory.Add(item); }
        return itemExist;
    }

    public bool RemoveItem(SO_Item item)
    {
        bool itemExist = Inventory.Contains(item);
        if (itemExist) { Inventory.Remove(item); }
        return itemExist;
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
