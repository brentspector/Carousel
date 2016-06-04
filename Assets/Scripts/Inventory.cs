/***************************************************************************************** 
 * File:    Inventory.cs
 * Summary: Contains player's item bag
 *****************************************************************************************/ 
#region Using
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

[Serializable]
public class Inventory
{
	#region Variables
	/*** Bag is divded as ****
	 * 0: Free Space
	 * 1: Items
	 * 2: Medicine
	 * 3: Pokeballs
	 * 4: TMs&HMs
	 * 5: Berries
	 * 6: Battle Items
	 * 7: Key Items
	 ********************/
	List<List<List<int>>> inventory;

	int currentPocket;					//The pocket the player is on
	#endregion

	#region Methods
	/***************************************
     * Name: Inventory
     * Initialize inventory variables
     ***************************************/
	public Inventory()
	{
		//Initialize inventory
		inventory = new List<List<List<int>>>();

		//Initialize pockets
		for (int i = 0; i < 8; i++)
		{
			inventory.Add(new List<List<int>>());
		} //end for

		//Start at pocket 0
	} //end Inventory

	/***************************************
     * Name: AddItem
     * Adds an item to the inventory
     ***************************************/
	public void AddItem(int item, int quantity, int bagSpot = -1)
	{
		//Get item slot
		int slot = bagSpot > -1 ? bagSpot : ExtensionMethods.CapAtInt(DataContents.GetItemBagSpot(item), 7);

		//Check is item already exists in bag
		int index = -1;
		index = inventory[slot].FindIndex(searchItem => searchItem[0] == item);

		if (index < 0)
		{
			//Add item
			List<int> newItem = new List<int>();
			newItem.Add(item);
			newItem.Add(quantity);
			inventory[slot].Add(newItem);
		} //end if
		else
		{
			inventory[slot][index][1] += quantity;
		} //end else
	} //end AddItem(int item, int quantity, int bagSpot = -1)

	/***************************************
     * Name: RemoveItem
     * Remove an item from the inventory
     ***************************************/
	public void RemoveItem(int item, int quantity, int bagSpot = -1)
	{
		//Get item slot
		int slot = bagSpot > -1 ? bagSpot : currentPocket;

		//Find the item
		int index = inventory[slot].FindIndex(theItem => theItem[0] == item);

		//Remove it entirely if quantity matches or exceeds stored quanitity
		if (quantity >= inventory[slot][index][1])
		{
			inventory[slot].RemoveAt(index);
		} //end if

		//Reduce quantity by requested amount
		else
		{
			inventory[slot][index][1] -= quantity;
		} //end else

	} //end RemoveItem(int item, int quantity, int bagSpot = -1)

	/***************************************
     * Name: GetItem
     * Get an item from a spot in inventory
     ***************************************/
	public List<int> GetItem(int spot, int bagSpot = -1)
	{
		//Get item slot
		int slot = bagSpot > -1 ? bagSpot : currentPocket;

		//Return item in spot
		return inventory[slot][spot];
	} //end GetItem(int spot, int bagSpot = -1)

	/***************************************
     * Name: SlotCount
     * How many items are in the inventory
     * slot
     ***************************************/
	public int SlotCount(int bagSpot = -1)
	{
		//Get item slot
		int slot = bagSpot > -1 ? bagSpot : currentPocket;

		return inventory[slot].Count;
	} //end SlotCount(int bagSpot = -1)

	/***************************************
     * Name: ItemCount
     * How many of the item are in the inventory
     ***************************************/
	public int ItemCount(int item, int bagSpot = -1)
	{
		//Get item slot
		int slot = bagSpot > -1 ? bagSpot : DataContents.GetItemBagSpot(item);

		//Find the item
		List<int> itemRequested = inventory[slot].Find(theItem => theItem[0] == item);

		return itemRequested != null ? itemRequested[1] : 0;
	} //end ItemCount(int item, int bagSpot = -1)

	/***************************************
     * Name: MoveItemPocket
     * Move an item to a different bag slot
     ***************************************/
	public void MoveItemPocket(int item, int bagPocketTo = -1)
	{
		//Get the item
		List<int> theItem = inventory[currentPocket].Find(stored => stored[0] == item);

		//Remove the item then add it
		RemoveItem(theItem[0], theItem[1], currentPocket);
		AddItem(theItem[0], theItem[1], bagPocketTo);

		//Display success
		if (bagPocketTo == -1)
		{
			GameManager.instance.DisplayText("Moved " + DataContents.GetItemGameName(item) +
			" to regular pocket.", true);
		} //end if
		else
		{
			GameManager.instance.DisplayText("Moved " + DataContents.GetItemGameName(item) +
				" to free space.", true);
		} //end else
	} //end MoveItemPocket(int item, int bagPocketFrom = -1, int bagPocketTo = -1)

	/***************************************
     * Name: MoveItemLocation
     * Move an item to a different position 
     * in bag pocket
     ***************************************/
	public void MoveItemLocation(int bagSpotFrom, int bagSpotTo)
	{
		//Get the reuqested item
		List<int> theItem = inventory[currentPocket][bagSpotFrom];

		inventory[currentPocket].RemoveAt(bagSpotFrom);
		inventory[currentPocket].Insert(bagSpotTo, theItem);
	} //end MoveItemLocation(int bagSpotFrom, int bagSpotTo)

	/***************************************
     * Name: NextPocket
     * Moves to next pocket
     ***************************************/
	public void NextPocket()
	{
		currentPocket++;

		//Loop to free space if beyond key items
		if(currentPocket > 7)
		{
			currentPocket = 0;
		} //end if
	} //end NextPocket

	/***************************************
     * Name: PreviousPocket
     * Moves to previous pocket
     ***************************************/
	public void PreviousPocket()
	{
		currentPocket--;

		//Loop to key items if lower than free space
		if(currentPocket < 0)
		{
			currentPocket = 7;
		} //end if
	} //end PreviousPocket

	/***************************************
     * Name: ChangePocket
     * Moves to requested pocket
     ***************************************/
	public void ChangePocket(int requested)
	{
		currentPocket = ExtensionMethods.WithinIntRange(requested, 0, 7);
	} //end ChangePocket(int requested)

	/***************************************
     * Name: GetCurrentBagPocket
     * Returns the active pocket
     ***************************************/
	public int GetCurrentBagPocket()
	{
		return currentPocket;
	} //end GetCurrentBagPocket
	#endregion
} //end Inventory class