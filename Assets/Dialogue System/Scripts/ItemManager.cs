// DIALOGUE SYSTEM made by James Shipp
// Last updated 9/28/23

using UnityEngine;
using System;

/*
 * This script should go on an empty gameobject as manager
 * that only needs one instance in the whole game (set up 
 * as singleton). It is referenced to check and update items
 * held by the player as loaded by the item list CSV file
 */
public class ItemManager : MonoBehaviour
{
    [SerializeField]
    private TextAsset itemListCSV;
    [SerializeField]
    private Item[] itemList;

    // singleton
    private static ItemManager instance;

    public static ItemManager Instance()
    {
        return instance;
    }

    private void Awake()
    {
        // setting up singleton
        if (instance != null && instance != this)
            Destroy(this);
        instance = this;
    }

    private void Start()
    {
        if (itemListCSV != null)
            itemList = CSVReader.ReadItemListCSV(itemListCSV);
        else
            throw new Exception("CSV file not configured for ItemManager item list");
    }

    // returns whether or not player is currently holding
    // specified item
    public bool GetItemHolding(string itemName)
    {
        Item item = Array.Find(itemList, x => string.Compare(x.GetName(), itemName) == 0);
        if (item == null)
            throw new Exception("Searched item list for non-existant item! Did you make a typo somewhere?\n");
        return item.GetHolding();
    }

    // update status if player is holding this item or not.
    // this function INVERTS whatever the current status is-
    // so if the player is holding an item, it will be set
    // to not holding, and vice versa
    public void SetItemHolding(string itemName)
    {
        Item item = Array.Find(itemList, x => x.GetName() == itemName);
        if (item == null)
            throw new Exception("Searched item list non-existant item " + itemName + "! Did you make a typo somewhere?\n");
        
        bool taking = !item.GetHolding();
        item.SetHolding(taking); // if holding, take. if not holding, give
    }
}

// class to keep track of item data loaded from item list CSV file
[System.Serializable]
public class Item
{
    [SerializeField]
    private string name;
    [SerializeField]
    private bool holding;

    public string GetName() => name;
    public bool GetHolding() => holding;

    public Item(string itemId)
    {
        name = itemId;
        holding = false;
    }

    public void SetHolding(bool holdingStatus)
    {
        holding = holdingStatus;
    }
}
