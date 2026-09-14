using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class InventoryItem
{
    public ItemData data;
    public int stackSize; // declare a variable stack for numbers of item

    // constructor
    public InventoryItem(ItemData _newItemData)
    {
        data = _newItemData;  // Initialize the data with the provided item data
        AddStack(); // Initialize the stack size to 1
    }

    // add number of item
    public void AddStack() => stackSize++;

    // remove number of item
    public void RemoveStack() => stackSize--;


}
