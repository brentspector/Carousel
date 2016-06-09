/***************************************************************************************** 
 * File:    Shop.cs
 * Summary: Contains items and pokemon the player can buy or unlock
 *****************************************************************************************/ 
#region Using
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#endregion

[Serializable]
public class Shop 
{
    #region Variables
	List<int> tier1Pokemon;				//Lowest tier of Pokemon available
	List<int> tier2Pokemon;				//Middle low tier of Pokemon available
	List<int> tier3Pokemon;				//Middle tier of Pokemon available
	List<int> tier4Pokemon;				//Middle high tier of Pokemon available
	List<int> tier5Pokemon;				//Highest tier of Pokemon available
	List<int> tier1Cost;				//Lowest tier of Pokemon available
	List<int> tier2Cost;				//Middle low tier of Pokemon available
	List<int> tier3Cost;				//Middle tier of Pokemon available
	List<int> tier4Cost;				//Middle high tier of Pokemon available
	List<int> tier5Cost;				//Highest tier of Pokemon available
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
		tier1Pokemon = new List<int>();
		tier2Pokemon = new List<int>();
		tier3Pokemon = new List<int>();
		tier4Pokemon = new List<int>();
		tier5Pokemon = new List<int>();
		tier1Cost = new List<int>();
		tier2Cost = new List<int>();
		tier3Cost = new List<int>();
		tier4Cost = new List<int>();
		tier5Cost = new List<int>();
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
		switch (region)
		{
			case 0:
				Debug.Log("Kanto");
				break;
			case 1:
				Debug.Log("Johto");
				break;
			case 2:
				Debug.Log("Hoenn");
				break;
			case 3:
				Debug.Log("Sinnoh");
				break;
			case 4:
				Debug.Log("Unova");
				break;
			case 5:
				//Add pokemon stock
				string contents = DataContents.ExecuteSQL<string>("SELECT tier1 FROM Shop WHERE region=5");
				string[] list = contents.Split(',');
				for (int i = 0; i < list.Length; i++)
				{					
					tier1Pokemon.Add(int.Parse(list[i]));
					int cost = DataContents.ExecuteSQL<int>("SELECT catchRate FROM Pokemon WHERE rowid=" + tier1Pokemon[i]);
					cost = 555 - cost;	
					tier1Cost.Add(cost);
				} //end for
				contents = DataContents.ExecuteSQL<string>("SELECT tier2 FROM Shop WHERE region=5");
				list = contents.Split(',');
				for (int i = 0; i < list.Length; i++)
				{
					tier2Pokemon.Add(int.Parse(list[i]));
					int cost = DataContents.ExecuteSQL<int>("SELECT catchRate FROM Pokemon WHERE rowid=" + tier2Pokemon[i]);
					cost = 2 * (555 - cost);	
					tier2Cost.Add(cost);
				} //end for
				contents = DataContents.ExecuteSQL<string>("SELECT tier3 FROM Shop WHERE region=5");
				list = contents.Split(',');
				for (int i = 0; i < list.Length; i++)
				{
					tier3Pokemon.Add(int.Parse(list[i]));
					int cost = DataContents.ExecuteSQL<int>("SELECT catchRate FROM Pokemon WHERE rowid=" + tier3Pokemon[i]);
					cost = 3 * (555 - cost);
					tier3Cost.Add(cost);
				} //end for
				contents = DataContents.ExecuteSQL<string>("SELECT tier4 FROM Shop WHERE region=5");
				list = contents.Split(',');
				for (int i = 0; i < list.Length; i++)
				{
					tier4Pokemon.Add(int.Parse(list[i]));
					int cost = DataContents.ExecuteSQL<int>("SELECT catchRate FROM Pokemon WHERE rowid=" + tier4Pokemon[i]);
					cost = 4 * (555 - cost);
					tier4Cost.Add(cost);
				} //end for
				contents = DataContents.ExecuteSQL<string>("SELECT tier5 FROM Shop WHERE region=5");
				list = contents.Split(',');
				for (int i = 0; i < list.Length; i++)
				{
					tier5Pokemon.Add(int.Parse(list[i]));
					int cost = DataContents.ExecuteSQL<int>("SELECT catchRate FROM Pokemon WHERE rowid=" + tier5Pokemon[i]);
					cost = 5 * (555 - cost);
					tier5Cost.Add(cost);
				} //end for

				//Add held item stock
				contents = DataContents.ExecuteSQL<string>("SELECT held FROM Shop WHERE region=5");
				list = contents.Split(',');
				for (int i = 0; i < list.Length; i++)
				{
					heldItemStock.Add(int.Parse(list[i]));
				} //end for

				//Add mega stone stock
				contents = DataContents.ExecuteSQL<string>("SELECT megastones FROM Shop WHERE region=5");
				list = contents.Split(',');
				for (int i = 0; i < list.Length; i++)
				{
					megaStoneStock.Add(int.Parse(list[i]));
				} //end for

				//Add evolution item stock
				contents = DataContents.ExecuteSQL<string>("SELECT evolution FROM Shop WHERE region=5");
				list = contents.Split(',');
				for (int i = 0; i < list.Length; i++)
				{
					evolutionItemStock.Add(int.Parse(list[i]));
				} //end for

				//Add medicine item stock
				contents = DataContents.ExecuteSQL<string>("SELECT medicine FROM Shop WHERE region=5");
				list = contents.Split(',');
				for (int i = 0; i < list.Length; i++)
				{
					medicineStock.Add(int.Parse(list[i]));
				} //end for

				//Add TM stock
				contents = DataContents.ExecuteSQL<string>("SELECT tms FROM Shop WHERE region=5");
				list = contents.Split(',');
				for (int i = 0; i < list.Length; i++)
				{
					TMStock.Add(int.Parse(list[i]));
				} //end for

				//Add berry stock
				contents = DataContents.ExecuteSQL<string>("SELECT berries FROM Shop WHERE region=5");
				list = contents.Split(',');
				for (int i = 0; i < list.Length; i++)
				{
					berryStock.Add(int.Parse(list[i]));
				} //end for

				//Add battle item stock
				contents = DataContents.ExecuteSQL<string>("SELECT battle FROM Shop WHERE region=5");
				list = contents.Split(',');
				for (int i = 0; i < list.Length; i++)
				{
					battleItemStock.Add(int.Parse(list[i]));
				} //end for

				//Add key item stock
				contents = DataContents.ExecuteSQL<string>("SELECT key FROM Shop WHERE region=5");
				list = contents.Split(',');
				for (int i = 0; i < list.Length; i++)
				{
					keyItemStock.Add(int.Parse(list[i]));
				} //end for
				break;
		} //end switch
	} //end PopulateStock(int region)

	/***************************************
     * Name: GetShopStockList
     * Get list of items from stock list
     ***************************************/
	public List<int> GetShopStockList(int stock)
	{
		switch (stock)
		{
			//All
			case 0:
				return tier1Pokemon.Concat(tier2Pokemon).Concat(tier3Pokemon).Concat(tier4Pokemon).Concat(tier5Pokemon).Concat(heldItemStock).Concat(megaStoneStock).
					Concat(evolutionItemStock).Concat(medicineStock).Concat(TMStock).Concat(battleItemStock).Concat(keyItemStock).ToList();
				break;
			case 1:
				return tier1Pokemon;
				break;
			case 2:
				return tier2Pokemon;
				break;
			case 3:
				return tier3Pokemon;
				break;
			case 4:
				return tier4Pokemon;
				break;
			case 5:
				return tier5Pokemon;
				break;
			//All Pokemon
			case 6:
				return tier1Pokemon.Concat(tier2Pokemon).Concat(tier3Pokemon).Concat(tier4Pokemon).Concat(tier5Pokemon).ToList();
				break;
			case 7:
				return heldItemStock;
				break;
			case 8:
				return megaStoneStock;
				break;
			case 9:
				return evolutionItemStock;
				break;
			case 10:
				return medicineStock;
				break;
			case 11:
				return TMStock;
				break;
			case 12:
				return battleItemStock;
				break;
			case 13:
				return keyItemStock;
				break;
			//All items
			case 14:
				return heldItemStock.Concat(megaStoneStock).Concat(evolutionItemStock).Concat(medicineStock).
					Concat(TMStock).Concat(battleItemStock).Concat(keyItemStock).ToList();
				break;
		} //end switch

		//If no shop stock found, return a blank list
		return new List<int>();
	} //end GetShopStockList(int stock)

	/***************************************
     * Name: GetShopStockCost
     * Returns cost list for displayed pokemon 
     ***************************************/
	public List<int> GetShopStockCost(int filter)
	{
		switch (filter)
		{
			//All
			case 0:
				return tier1Cost.Concat(tier2Cost).Concat(tier3Cost).Concat(tier4Cost).Concat(tier5Cost).ToList();
				break;
			case 1:
				return tier1Cost;
				break;
			case 2:
				return tier2Cost;
				break;
			case 3:
				return tier3Cost;
				break;
			case 4:
				return tier4Cost;
				break;
			case 5:
				return tier5Cost;
				break;
			//All Pokemon
			case 6:
				return tier1Cost.Concat(tier2Cost).Concat(tier3Cost).Concat(tier4Cost).Concat(tier5Cost).ToList();
				break;			
		} //end switch

		//If no shop stock found, return a blank list
		return new List<int>();
	} //end GetShopStockCost(int filter)

	/***************************************
     * Name: GetPokemonTotal
     * Returns total amount of pokemon 
     ***************************************/
	public int GetPokemonTotal()
	{
		return tier1Pokemon.Count + tier2Pokemon.Count + tier3Pokemon.Count + tier4Pokemon.Count + tier5Pokemon.Count;
	} //end GetPokemonTotal
    #endregion
} //end Shop class
