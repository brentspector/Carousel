/***************************************************************************************** 
 * File:    Shop.cs
 * Summary: Contains items and pokemon the player can buy or unlock
 *****************************************************************************************/ 
#region Using
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

[Serializable]
public class Shop 
{
    #region Variables
	List<int> pokemonStock;				//The pokemon available
	List<int> heldItemStock;			//Held items (use of 0 for both)
	List<int> megaStoneStock;			//Held items for mega evolutions
	List<int> evolutionItemStock;		//Evolution stones and held items
	List<int> medicineStock;			//Restoration and modification (use of 1 for outside and 1/2 inside)
	List<int> TMStock;					//Attacks (use of 3 for outside)
	List<int> berryStock;				//Berries (use of 1 for outside and 2/3 for inside)
	List<int> battleItemStock;			//Stat modifiers mainly (use of 1/4 for inside)
	List<int> keyItemStock;				//Permanent items (use of 2 inside)
	List<int> boughtStock;				//One time purchases to prevent duplication
    #endregion

    #region Methods
	/***************************************
     * Name: Shop
     * Initializes shop
     ***************************************/
	public Shop()
	{
		//Initialize stock lists
		pokemonStock = new List<int>();
		heldItemStock = new List<int>();
		megaStoneStock = new List<int>();
		evolutionItemStock = new List<int>();
		medicineStock = new List<int>();
		TMStock = new List<int>();
		berryStock = new List<int>();
		battleItemStock = new List<int>();
		keyItemStock = new List<int>();
		boughtStock = new List<int>();
	} //end Shop

	/***************************************
     * Name: PopulateStock
     * Adds items into stock
     ***************************************/
	public void PopulateStock(int region)
	{

	} //end PopulateStock(int region)
    #endregion
} //end Shop class
