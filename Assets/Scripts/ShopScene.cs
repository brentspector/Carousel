/***************************************************************************************** 
 * File:    ShopScene.cs
 * Summary: Handles process for Shop
 *****************************************************************************************/ 
#region Using
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

public class ShopScene : MonoBehaviour 
{
	#region Variables
	//Scene variables
	int checkpoint = 0;			//Manage function progress
	int filter;					//The contents of the shop shown to the player
	int page;					//What section of items are being displayed
	int endPokemon;				//Stores the end of pokemon
	bool displayPokemon;		//Toggles whether to read Pokemon or Items table
	bool processing = false;	//Whether a function is already processing something
	List<int> toDisplay;		//The list of things to display
	List<int> pokemonCost;		//Lists of costs for displayed pokemon
	GameObject selection;		//Selection rectangle from scene tools
	GameObject itemRegion;		//Area where items are displayed
	Text description;			//The description of the item
	#endregion

	#region Methods
	/***************************************
	 * Name: RunShop
	 * Play the shop scene
	 ***************************************/
	public void RunShop()
	{
		//Initialize scene variables
		if (checkpoint == 0)
		{
			//Set checkpoint delegate
			GameManager.instance.checkDel = ChangeCheckpoint;

			//Initialize references
			itemRegion = GameObject.Find("ItemRegion");
			description = GameObject.Find("Description").transform.FindChild("Text").GetComponent<Text>();
			selection = GameManager.tools.transform.FindChild("Selection").gameObject;

			//Currently unfiltered
			filter = 0;
			displayPokemon = true;

			//Set starting event navigation
			EventSystem.current.SetSelectedGameObject(GameObject.Find("Filters").gameObject);

			//Get end of pokemon
			endPokemon = GameManager.instance.GetTrainer().GetPokemonTotal();

			//Move to next checkpoint
			checkpoint = 1;
		} //end if

		//Set up start of scene
		else if (checkpoint == 1)
		{
			//Return if processing
			if (processing)
			{
				return;
			} //end if

			//Begin processing
			processing = true;

			//Start on first page
			page = 0;

			//Start on first item
			StartCoroutine(WaitForResize());

			//Get list of items according to filter
			toDisplay = GameManager.instance.GetTrainer().GetShopStockList(filter);
			pokemonCost = GameManager.instance.GetTrainer().GetShopStockCost(filter);

			//Read from Pokemon or Items table 
			if (displayPokemon)
			{
				//Loop through and set sprite, name, and cost for each
				for (int i = 0; i < 15; i++)
				{
					itemRegion.transform.GetChild(i).FindChild("ItemIcon").GetComponent<Image>().sprite =
						Resources.Load<Sprite>("Sprites/Icons/icon" + toDisplay[i].ToString("000"));
					itemRegion.transform.GetChild(i).FindChild("ItemName").GetComponent<Text>().text =
						DataContents.ExecuteSQL<string>("SELECT name FROM Pokemon WHERE rowid=" + toDisplay[i]);
					itemRegion.transform.GetChild(i).FindChild("Cost").GetComponent<Text>().text = pokemonCost[i].ToString();
				} //end for
			} //end if
			else
			{
				//Loop through and set sprite, name, and cost for each
				for (int i = 0; i < 15; i++)
				{
					itemRegion.transform.GetChild(i).FindChild("ItemIcon").GetComponent<Image>().sprite =
						Resources.Load<Sprite>("Sprites/Icons/item" + toDisplay[i].ToString("000"));
					itemRegion.transform.GetChild(i).FindChild("ItemName").GetComponent<Text>().text =
						DataContents.ExecuteSQL<string>("SELECT gameName FROM Items WHERE rowid=" + toDisplay[i]);
					itemRegion.transform.GetChild(i).FindChild("Cost").GetComponent<Text>().text =
						DataContents.ExecuteSQL<string>("SELECT cost FROM Items WHERE rowid=" + toDisplay[i]);
				} //end for
			} //end else

			//Move to next checkpoint
			GameManager.instance.FadeInAnimation(2);
		} //end else if
		else if (checkpoint == 2)
		{	
 			//Get player input
			GetInput();

			//Get list location
			int location = page * 15;		

			//Position selection rect to requested spot
			StartCoroutine(WaitForResize());

			//Display pokemon starts as true
			displayPokemon = true;

			//Loop through and set sprite, name, and cost for each
			for (int i = 0; i < 15; i++)
			{
				//Read from Pokemon or Items table
				if (displayPokemon)
				{
					if (i + location < endPokemon)
					{
						itemRegion.transform.GetChild(i).FindChild("ItemIcon").GetComponent<Image>().color = Color.white;
						itemRegion.transform.GetChild(i).FindChild("ItemIcon").GetComponent<Image>().sprite =
							Resources.Load<Sprite>("Sprites/Icons/icon" + toDisplay[i + location].ToString("000"));
						itemRegion.transform.GetChild(i).FindChild("ItemName").GetComponent<Text>().text =
							DataContents.ExecuteSQL<string>("SELECT name FROM Pokemon WHERE rowid=" + toDisplay[i + location]);
						itemRegion.transform.GetChild(i).FindChild("Cost").GetComponent<Text>().text = pokemonCost[i + location].ToString();
					} //end if
					else
					{
						displayPokemon = false;
						i--;
					} //end else
				} //end if
				else
				{
					if (i + location < toDisplay.Count)
					{
						itemRegion.transform.GetChild(i).FindChild("ItemIcon").GetComponent<Image>().color = Color.white;
						itemRegion.transform.GetChild(i).FindChild("ItemIcon").GetComponent<Image>().sprite =
							Resources.Load<Sprite>("Sprites/Icons/item" + toDisplay[i + location].ToString("000"));
						itemRegion.transform.GetChild(i).FindChild("ItemName").GetComponent<Text>().text =
							DataContents.ExecuteSQL<string>("SELECT gameName FROM Items WHERE rowid=" + toDisplay[i + location]);
						itemRegion.transform.GetChild(i).FindChild("Cost").GetComponent<Text>().text =
							DataContents.ExecuteSQL<string>("SELECT cost FROM Items WHERE rowid=" + toDisplay[i + location]);
					} //end if
					else
					{
						itemRegion.transform.GetChild(i).FindChild("ItemIcon").GetComponent<Image>().color = Color.clear;
						itemRegion.transform.GetChild(i).FindChild("ItemName").GetComponent<Text>().text = "";
						itemRegion.transform.GetChild(i).FindChild("Cost").GetComponent<Text>().text = "";
					} //end else
				} //end else
			} //end for
		} //end else if
	} //end RunShop

	/***************************************
	 * Name: GetInput
	 * Gather user input and set variables
	 * as necessary
	 ***************************************/
	void GetInput()
	{
		/*********************************************
		 * Left Arrow
		 *********************************************/
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{

		} //end if Left Arrow

		/*********************************************
		* Right Arrow
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{

		} //end else if Right Arrow

		/*********************************************
		* Up Arrow
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.UpArrow))
		{

		} //end else if Up Arrow

		/*********************************************
		* Down Arrow
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{

		} //end else if Down Arrow

		/*********************************************
		* Mouse Moves Left
		**********************************************/
		else if (Input.GetAxis("Mouse X") < 0)
		{
			//Normal Processing
			if (checkpoint == 2)
			{

			} //end if
		} //end else if Mouse Moves Left

		/*********************************************
		* Mouse Moves Right
		**********************************************/
		else if (Input.GetAxis("Mouse X") > 0)
		{
			//Normal Processing
			if (checkpoint == 2)
			{

			} //end if
		} //end else if Mouse Moves Right

		/*********************************************
		* Mouse Moves Up
		**********************************************/
		else if (Input.GetAxis("Mouse Y") > 0)
		{
			//Normal Processing
			if (checkpoint == 2)
			{

			} //end if
		} //end else if Mouse Moves Up

		/*********************************************
		* Mouse Moves Down
		**********************************************/
		else if (Input.GetAxis("Mouse Y") < 0)
		{
			//Normal Processing
			if (checkpoint == 2)
			{

			} //end if
		} //end else if Mouse Moves Down

		/*********************************************
		* Mouse Wheel Up
		**********************************************/
		else if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{

		} //end else if Mouse Wheel Up

		/*********************************************
		* Mouse Wheel Down
		**********************************************/
		else if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{

		} //end else if Mouse Wheel Down

		/*********************************************
		* Left Mouse Button
		**********************************************/
		else if (Input.GetMouseButtonUp(0))
		{

		} //end else if Left Mouse Button

		/*********************************************
		* Right Mouse Button
		**********************************************/
		else if (Input.GetMouseButtonUp(1))
		{

		} //end else if Right Mouse Button

		/*********************************************
		* Enter/Return Key
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.Return))
		{

		} //end else if Enter/Return Key

		/*********************************************
		* X Key
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.X))
		{

		} //end else if X Key
	} //end GetInput

	/***************************************
	 * Name: NextPage
	 * Go to the next page of shop
	 ***************************************/
	public void NextPage()
	{
		page++;
		if (page > toDisplay.Count / 15)
		{
			page = 0;
		} //end if
	} //end NextPage

	/***************************************
	 * Name: PreviousPage
	 * Go to the next page of shop
	 ***************************************/
	public void PreviousPage()
	{
		page--;
		if (page < 0)
		{
			page = toDisplay.Count / 15;
		} //end if
	} //end PreviousPage

	/***************************************
	 * Name: WaitForResize
	 * Resizes selection rect to fit current
	 * selected object
	 ***************************************/
	IEnumerator WaitForResize()
	{
		//Process at end of frame
		yield return new WaitForEndOfFrame();

		//Resize to highlighted navigation object
		Vector3 scale = new Vector3(EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().rect.width,
			EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().rect.height, 0);
			selection.GetComponent<RectTransform>().sizeDelta = scale;
		selection.transform.position = Camera.main.WorldToScreenPoint(EventSystem.current.currentSelectedGameObject.transform.position);
		selection.SetActive(true);
	} //end WaitForResize

	/***************************************
	 * Name: ChangeCheckpoint
	 * Changes the checkpoint
	 ***************************************/
	public void ChangeCheckpoint(int newCheckpoint)
	{
		checkpoint = newCheckpoint;
		processing = false;
	} //end ChangeCheckpoint(int newCheckpoint)
	#endregion
} //end ShopScene class
