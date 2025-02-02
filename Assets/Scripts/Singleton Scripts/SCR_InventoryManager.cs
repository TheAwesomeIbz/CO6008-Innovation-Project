using Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SCR_InventoryManager : MonoBehaviour
{
    [field: Header("INVENTORY MANAGER PROPERTIES")]
    [field: SerializeField] public List<SO_Item> Inventory { get; private set; }


    void Start()
    {
        SavingOperations.OnSaveDataLoaded += OnSaveDataLoaded;
    }

    private void OnSaveDataLoaded(SaveData obj)
    {
        Inventory = obj.InventoryInformation;
    }

    public bool AddItemWithDialogue(SO_Item item)
    {
        bool startsWithVowel = "aeiou".Contains(item.name.ToLower()[0]);
        DialogueObject[] dialogue = DialogueObject.CreateDialogue($"{SCR_GeneralManager.Instance.PlayerData.PlayerName} obtained {(startsWithVowel ? "an" : "a")} {item.name}!",
                $"The {item.name} was stored into your inventory.");
        SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(dialogue);

        return AddItem(item);
    }

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
    
    void Update()
    {
        
    }

    private void OnDisable()
    {
        SavingOperations.OnSaveDataLoaded -= OnSaveDataLoaded;
    }
}
