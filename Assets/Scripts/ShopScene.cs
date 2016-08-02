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
using System.Linq;
#endregion

public class ShopScene : MonoBehaviour 
{
	#region Variables
	//Scene variables
	int checkpoint = 0;			//Manage function progress
	int filter;					//The contents of the shop shown to the player
	int page;					//What section of items are being displayed
	int endPokemon;				//Stores the end of pokemon
	int selectedItem;			//The selected item for reference
	bool displayPokemon;		//Toggles whether to read Pokemon or Items table
	bool processing = false;	//Whether a function is already processing something
	List<int> toDisplay;		//The list of things to display
	List<int> pokemonCost;		//Lists of costs for displayed pokemon
	GameObject selection;		//Selection rectangle from scene tools
	GameObject itemRegion;		//Area where items are displayed
	GameObject purchaseRegion;	//Area for a purchase or sale to be conducted
	GameObject codeRegion;		//Area for user to input codes
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
			purchaseRegion = GameObject.Find("PurchaseRegion");
			codeRegion = GameObject.Find("CodeInput");
			description = GameObject.Find("Description").transform.FindChild("Text").GetComponent<Text>();
			selection = GameManager.tools.transform.FindChild("Selection").gameObject;

			//Not buying anything
			purchaseRegion.SetActive(false);

			//Not inputing code
			codeRegion.SetActive(false);

			//Currently unfiltered
			filter = 0;
			displayPokemon = true;

			//Set starting event navigation
			EventSystem.current.SetSelectedGameObject(GameObject.Find("Filters").gameObject);
			selectedItem = -1;

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
					if (i < toDisplay.Count)
					{
						itemRegion.transform.GetChild(i).FindChild("ItemIcon").GetComponent<Image>().sprite =
						Resources.Load<Sprite>("Sprites/Icons/icon" + toDisplay[i].ToString("000"));
						itemRegion.transform.GetChild(i).FindChild("ItemName").GetComponent<Text>().text =
						DataContents.ExecuteSQL<string>("SELECT name FROM Pokemon WHERE rowid=" + toDisplay[i]);
						itemRegion.transform.GetChild(i).FindChild("Cost").GetComponent<Text>().text = pokemonCost[i].ToString();
					} //end if
					else
					{
						itemRegion.transform.GetChild(i).FindChild("ItemIcon").GetComponent<Image>().color = Color.clear;
						itemRegion.transform.GetChild(i).FindChild("ItemName").GetComponent<Text>().text = "";
						itemRegion.transform.GetChild(i).FindChild("Cost").GetComponent<Text>().text = "";
					} //end else
				} //end for
			} //end if
			else
			{
				//Loop through and set sprite, name, and cost for each
				for (int i = 0; i < 15; i++)
				{
					if (i < toDisplay.Count)
					{
						itemRegion.transform.GetChild(i).FindChild("ItemIcon").GetComponent<Image>().sprite =
						Resources.Load<Sprite>("Sprites/Icons/item" + toDisplay[i].ToString("000"));
						itemRegion.transform.GetChild(i).FindChild("ItemName").GetComponent<Text>().text =
						DataContents.ExecuteSQL<string>("SELECT gameName FROM Items WHERE rowid=" + toDisplay[i]);
						itemRegion.transform.GetChild(i).FindChild("Cost").GetComponent<Text>().text =
						DataContents.ExecuteSQL<string>("SELECT cost FROM Items WHERE rowid=" + toDisplay[i]);
					} //end if
					else
					{
						itemRegion.transform.GetChild(i).FindChild("ItemIcon").GetComponent<Image>().color = Color.clear;
						itemRegion.transform.GetChild(i).FindChild("ItemName").GetComponent<Text>().text = "";
						itemRegion.transform.GetChild(i).FindChild("Cost").GetComponent<Text>().text = "";
					} //end else
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
			selectedItem = -1;

			//Position selection rect to requested spot
			StartCoroutine(WaitForResize());

			//Display pokemon starts as true
			displayPokemon = true;

			//Loop through and set sprite, name, and cost for each
			for (int i = 0; i < 15; i++)
			{
				//If displaying all items
				if (filter == 0)
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

							//Update description text
							if (EventSystem.current.currentSelectedGameObject == itemRegion.transform.GetChild(i).gameObject)
							{
								description.text = DataContents.ExecuteSQL<string>("SELECT pokedex FROM Pokemon WHERE rowid=" +
								toDisplay[i + location]);
								selectedItem = i + location;
							} //end if
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
							
							//Update description text
							if (EventSystem.current.currentSelectedGameObject == itemRegion.transform.GetChild(i).gameObject)
							{
								description.text = DataContents.ExecuteSQL<string>("SELECT description FROM Items WHERE rowid=" +
								toDisplay[i + location]);
								selectedItem = i + location;
							} //end if
						} //end if
						else
						{
							itemRegion.transform.GetChild(i).FindChild("ItemIcon").GetComponent<Image>().color = Color.clear;
							itemRegion.transform.GetChild(i).FindChild("ItemName").GetComponent<Text>().text = "";
							itemRegion.transform.GetChild(i).FindChild("Cost").GetComponent<Text>().text = "";
						} //end else
					} //end else
				} //end if

				//Pokemon filter
				else if (filter < 7)
				{
					if (i + location < toDisplay.Count)
					{
						itemRegion.transform.GetChild(i).FindChild("ItemIcon").GetComponent<Image>().color = Color.white;
						itemRegion.transform.GetChild(i).FindChild("ItemIcon").GetComponent<Image>().sprite =
							Resources.Load<Sprite>("Sprites/Icons/icon" + toDisplay[i + location].ToString("000"));
						itemRegion.transform.GetChild(i).FindChild("ItemName").GetComponent<Text>().text =
							DataContents.ExecuteSQL<string>("SELECT name FROM Pokemon WHERE rowid=" + toDisplay[i + location]);
						itemRegion.transform.GetChild(i).FindChild("Cost").GetComponent<Text>().text = pokemonCost[i + location].ToString();

						//Update description text
						if (EventSystem.current.currentSelectedGameObject == itemRegion.transform.GetChild(i).gameObject)
						{
							description.text = DataContents.ExecuteSQL<string>("SELECT pokedex FROM Pokemon WHERE rowid=" +
							toDisplay[i + location]);
							selectedItem = i + location;
						} //end if
					} //end if
					else
					{
						itemRegion.transform.GetChild(i).FindChild("ItemIcon").GetComponent<Image>().color = Color.clear;
						itemRegion.transform.GetChild(i).FindChild("ItemName").GetComponent<Text>().text = "";
						itemRegion.transform.GetChild(i).FindChild("Cost").GetComponent<Text>().text = "";
					} //end else
				} //end else if

				//Item filter
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

						//Update description text
						if (EventSystem.current.currentSelectedGameObject == itemRegion.transform.GetChild(i).gameObject)
						{
							description.text = DataContents.ExecuteSQL<string>("SELECT description FROM Items WHERE rowid=" +
							toDisplay[i + location]);
							selectedItem = i + location;
						} //end if
					} //end if
					else
					{
						itemRegion.transform.GetChild(i).FindChild("ItemIcon").GetComponent<Image>().color = Color.clear;
						itemRegion.transform.GetChild(i).FindChild("ItemName").GetComponent<Text>().text = "";
						itemRegion.transform.GetChild(i).FindChild("Cost").GetComponent<Text>().text = "";
					} //end else
				} //end else
			} //end for

			//Blank out description if not on an item
			if (EventSystem.current.currentSelectedGameObject != null && !EventSystem.current.currentSelectedGameObject.transform.
				IsChildOf(itemRegion.transform) || selectedItem < 0)
			{
				description.text = "";
			} //end if
		} //end else if
		else if (checkpoint == 3)
		{
			//Get player input
			GetInput();

			//Reposition selection rectangle
			StartCoroutine(WaitForResize());
		} //end else if
		else if (checkpoint == 4)
		{
			//Return if processing
			if (processing)
			{
				return;
			} //end if

			//Begin processing
			processing = true;

			//Start on first item
			StartCoroutine(WaitForResize());

			//Get list of items according to filter
			toDisplay = GameManager.instance.GetTrainer().GetBagInventory();

			//Start on first page
			page = 0;	

			for (int i = 0; i < 15; i++)
			{
				if (i < toDisplay.Count)
				{
					itemRegion.transform.GetChild(i).FindChild("ItemIcon").GetComponent<Image>().color = Color.white;
					itemRegion.transform.GetChild(i).FindChild("ItemIcon").GetComponent<Image>().sprite =
						Resources.Load<Sprite>("Sprites/Icons/item" + toDisplay[i].ToString("000"));
					itemRegion.transform.GetChild(i).FindChild("ItemName").GetComponent<Text>().text =
						DataContents.ExecuteSQL<string>("SELECT gameName FROM Items WHERE rowid=" + toDisplay[i]);
					itemRegion.transform.GetChild(i).FindChild("Cost").GetComponent<Text>().text =
						DataContents.ExecuteSQL<string>("SELECT cost FROM Items WHERE rowid=" + toDisplay[i]);
				} //end if
				else
				{
					itemRegion.transform.GetChild(i).FindChild("ItemIcon").GetComponent<Image>().color = Color.clear;
					itemRegion.transform.GetChild(i).FindChild("ItemName").GetComponent<Text>().text = "";
					itemRegion.transform.GetChild(i).FindChild("Cost").GetComponent<Text>().text = "";
				} //end else
			} //end for

			//Move to next checkpoint
			GameManager.instance.FadeInAnimation(5);
		} //end else if
		else if (checkpoint == 5)
		{
			//Get player input
			GetInput();

			//Get list location
			int location = page * 15;		
			selectedItem = -1;

			//Position selection rect to requested spot
			StartCoroutine(WaitForResize());

			//Loop through and set sprite, name, and cost for each
			for (int i = 0; i < 15; i++)
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

					//Update description text
					if (EventSystem.current.currentSelectedGameObject == itemRegion.transform.GetChild(i).gameObject)
					{
						description.text = DataContents.ExecuteSQL<string>("SELECT description FROM Items WHERE rowid=" +
						toDisplay[i + location]);
						selectedItem = i + location;
					} //end if
				} //end if
				else
				{
					itemRegion.transform.GetChild(i).FindChild("ItemIcon").GetComponent<Image>().color = Color.clear;
					itemRegion.transform.GetChild(i).FindChild("ItemName").GetComponent<Text>().text = "";
					itemRegion.transform.GetChild(i).FindChild("Cost").GetComponent<Text>().text = "";
				} //end else
			} //end for

			//Blank out description if not on an item
			if (EventSystem.current.currentSelectedGameObject != null && !EventSystem.current.currentSelectedGameObject.transform.
				IsChildOf(itemRegion.transform) || selectedItem < 0)
			{
				description.text = "";
			} //end if
		} //end else if
		else if (checkpoint == 6)
		{
			//Get player input
			GetInput();

			//Reposition selection rectangle
			StartCoroutine(WaitForResize());
		} //end else if
		else if (checkpoint == 7)
		{
			//Get player input
			GetInput();
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
			if (checkpoint == 2 || checkpoint == 5)
			{
				PointerEventData eventData = new PointerEventData(EventSystem.current);
				eventData.position = Input.mousePosition;
				List<RaycastResult> results = new List<RaycastResult>();
				EventSystem.current.RaycastAll(eventData, results);
				if (results.Any() && results[0].gameObject.transform.parent.GetComponent<Selectable>() != null)
				{
					EventSystem.current.SetSelectedGameObject(results[0].gameObject.transform.parent.gameObject);
				} //end if

				//Play choice SFX
				AudioManager.instance.PlayChange();
			} //end if

			//Purchase Processing
			else if (checkpoint == 3 || checkpoint == 6)
			{
				PointerEventData eventData = new PointerEventData(EventSystem.current);
				eventData.position = Input.mousePosition;
				List<RaycastResult> results = new List<RaycastResult>();
				EventSystem.current.RaycastAll(eventData, results);
				if (results.Any() && results[0].gameObject.transform.IsChildOf(purchaseRegion.transform.FindChild("Buttons")))
				{
					EventSystem.current.SetSelectedGameObject(results[0].gameObject);
				} //end if

				//Play choice SFX
				AudioManager.instance.PlayChange();
			} //end else if
		} //end else if Mouse Moves Left

		/*********************************************
		* Mouse Moves Right
		**********************************************/
		else if (Input.GetAxis("Mouse X") > 0)
		{
			//Normal Processing
			if (checkpoint == 2 || checkpoint == 5)
			{
				PointerEventData eventData = new PointerEventData(EventSystem.current);
				eventData.position = Input.mousePosition;
				List<RaycastResult> results = new List<RaycastResult>();
				EventSystem.current.RaycastAll(eventData, results);
				if (results.Any() && results[0].gameObject.transform.parent.GetComponent<Selectable>() != null)
				{
					EventSystem.current.SetSelectedGameObject(results[0].gameObject.transform.parent.gameObject);
				} //end if

				//Play choice SFX
				AudioManager.instance.PlayChange();
			} //end if

			//Purchase Processing
			else if (checkpoint == 3 || checkpoint == 6)
			{
				PointerEventData eventData = new PointerEventData(EventSystem.current);
				eventData.position = Input.mousePosition;
				List<RaycastResult> results = new List<RaycastResult>();
				EventSystem.current.RaycastAll(eventData, results);
				if (results.Any() && results[0].gameObject.transform.IsChildOf(purchaseRegion.transform.FindChild("Buttons")))
				{
					EventSystem.current.SetSelectedGameObject(results[0].gameObject);
				} //end if

				//Play choice SFX
				AudioManager.instance.PlayChange();
			} //end else if
		} //end else if Mouse Moves Right

		/*********************************************
		* Mouse Moves Up
		**********************************************/
		else if (Input.GetAxis("Mouse Y") > 0)
		{
			//Normal Processing
			if (checkpoint == 2 || checkpoint == 5)
			{
				PointerEventData eventData = new PointerEventData(EventSystem.current);
				eventData.position = Input.mousePosition;
				List<RaycastResult> results = new List<RaycastResult>();
				EventSystem.current.RaycastAll(eventData, results);
				if (results.Any() && results[0].gameObject.transform.parent.GetComponent<Selectable>() != null)
				{
					EventSystem.current.SetSelectedGameObject(results[0].gameObject.transform.parent.gameObject);
				} //end if

				//Play choice SFX
				AudioManager.instance.PlayChange();
			} //end if

			//Purchase Processing
			else if (checkpoint == 3 || checkpoint == 6)
			{
				PointerEventData eventData = new PointerEventData(EventSystem.current);
				eventData.position = Input.mousePosition;
				List<RaycastResult> results = new List<RaycastResult>();
				EventSystem.current.RaycastAll(eventData, results);
				if (results.Any() && results[0].gameObject.transform.IsChildOf(purchaseRegion.transform.FindChild("Buttons")))
				{
					EventSystem.current.SetSelectedGameObject(results[0].gameObject);
				} //end if

				//Play choice SFX
				AudioManager.instance.PlayChange();
			} //end else if
		} //end else if Mouse Moves Up

		/*********************************************
		* Mouse Moves Down
		**********************************************/
		else if (Input.GetAxis("Mouse Y") < 0)
		{
			//Normal Processing
			if (checkpoint == 2 || checkpoint == 5)
			{
				PointerEventData eventData = new PointerEventData(EventSystem.current);
				eventData.position = Input.mousePosition;
				List<RaycastResult> results = new List<RaycastResult>();
				EventSystem.current.RaycastAll(eventData, results);
				if (results.Any() && results[0].gameObject.transform.parent.GetComponent<Selectable>() != null)
				{
					EventSystem.current.SetSelectedGameObject(results[0].gameObject.transform.parent.gameObject);
				} //end if

				//Play choice SFX
				AudioManager.instance.PlayChange();
			} //end if

			//Purchase Processing
			else if (checkpoint == 3 || checkpoint == 6)
			{
				PointerEventData eventData = new PointerEventData(EventSystem.current);
				eventData.position = Input.mousePosition;
				List<RaycastResult> results = new List<RaycastResult>();
				EventSystem.current.RaycastAll(eventData, results);
				if (results.Any() && results[0].gameObject.transform.IsChildOf(purchaseRegion.transform.FindChild("Buttons")))
				{
					EventSystem.current.SetSelectedGameObject(results[0].gameObject);
				} //end if

				//Play choice SFX
				AudioManager.instance.PlayChange();
			} //end else if
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
			//Normal Processing
			if (checkpoint == 2 || checkpoint == 5)
			{
				if (selectedItem != -1)
				{
					StartCoroutine(SetupPurchase());
				} //end if

				//Play selection SFX
				AudioManager.instance.PlaySelect();
			} //end if

			//Code processing
			else if (checkpoint == 7)
			{
				if(!string.IsNullOrEmpty(codeRegion.GetComponent<InputField>().text))
				{
					GameManager.instance.GetTrainer().ProcessCode(codeRegion.GetComponent<InputField>().text);
					codeRegion.GetComponent<InputField>().text = "";
					codeRegion.SetActive(false);
					selection.SetActive(true);
					if (GameObject.Find("Navigation").transform.GetChild(0).GetChild(0).GetComponent<Text>().text == "Buy")
					{
						checkpoint = 5;
					} //end if
					else
					{
						checkpoint = 2;
					} //end else
				} //end if

				//Play selection SFX
				AudioManager.instance.PlaySelect();
			} //end else if
		} //end else if Left Mouse Button

		/*********************************************
		* Right Mouse Button
		**********************************************/
		else if (Input.GetMouseButtonUp(1))
		{
			//Normal Processing
			if (checkpoint == 2 || checkpoint == 5)
			{
				selection.SetActive(false);
				GameManager.instance.LoadScene("MainGame", true);

				//Play selection SFX
				AudioManager.instance.PlaySelect();
			} //end if

			//Purchase Processing
			else if (checkpoint == 3)
			{
				purchaseRegion.SetActive(false);
				selection.SetActive(false);
				EventSystem.current.SetSelectedGameObject(itemRegion.transform.GetChild(selectedItem - 
					(page * 15)).gameObject);
				checkpoint = 2;

				//Play selection SFX
				AudioManager.instance.PlaySelect();
			} //end else if

			//Sale Processing
			else if (checkpoint == 6)
			{
				purchaseRegion.SetActive(false);
				selection.SetActive(false);
				EventSystem.current.SetSelectedGameObject(itemRegion.transform.GetChild(selectedItem - 
					(page * 15)).gameObject);
				checkpoint = 5;

				//Play selection SFX
				AudioManager.instance.PlaySelect();
			} //end else if

			//Code processing
			else if (checkpoint == 7)
			{
				codeRegion.SetActive(false);
				if (GameObject.Find("Navigation").transform.GetChild(0).GetChild(0).GetComponent<Text>().text == "Buy")
				{
					checkpoint = 5;
				} //end if
				else
				{
					checkpoint = 2;
				} //end else

				//Play selection SFX
				AudioManager.instance.PlaySelect();
			} //end else if
		} //end else if Right Mouse Button

		/*********************************************
		* Enter/Return Key
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.Return))
		{
			//Normal Processing
			if (checkpoint == 2 || checkpoint == 5)
			{
				if (selectedItem != -1)
				{
					StartCoroutine(SetupPurchase());
				} //end if

				//Play selection SFX
				AudioManager.instance.PlaySelect();
			} //end if

			//Code processing
			else if (checkpoint == 7)
			{
				if(!string.IsNullOrEmpty(codeRegion.GetComponent<InputField>().text))
				{
					GameManager.instance.GetTrainer().ProcessCode(codeRegion.GetComponent<InputField>().text);
					codeRegion.GetComponent<InputField>().text = "";
					codeRegion.SetActive(false);
					selection.SetActive(true);
					if (GameObject.Find("Navigation").transform.GetChild(0).GetChild(0).GetComponent<Text>().text == "Buy")
					{
						checkpoint = 5;
					} //end if
					else
					{
						checkpoint = 2;
					} //end else

					//Play selection SFX
					AudioManager.instance.PlaySelect();
				} //end if
			} //end else if
		} //end else if Enter/Return Key

		/*********************************************
		* X Key
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.X))
		{
			//Normal Processing
			if (checkpoint == 2 || checkpoint == 5)
			{
				selection.SetActive(false);
				GameManager.instance.LoadScene("MainGame", true);

				//Play selection SFX
				AudioManager.instance.PlaySelect();
			} //end if

			//Purchase Processing
			else if (checkpoint == 3)
			{
				purchaseRegion.SetActive(false);
				selection.SetActive(false);
				EventSystem.current.SetSelectedGameObject(itemRegion.transform.GetChild(selectedItem - 
					(page * 15)).gameObject);
				checkpoint = 2;

				//Play selection SFX
				AudioManager.instance.PlaySelect();
			} //end else if

			//Sale Processing
			else if (checkpoint == 6)
			{
				purchaseRegion.SetActive(false);
				selection.SetActive(false);
				EventSystem.current.SetSelectedGameObject(itemRegion.transform.GetChild(selectedItem - 
					(page * 15)).gameObject);
				checkpoint = 5;

				//Play selection SFX
				AudioManager.instance.PlaySelect();
			} //end else if

			//Code processing
			else if (checkpoint == 7)
			{
				codeRegion.SetActive(false);
				if (GameObject.Find("Navigation").transform.GetChild(0).GetChild(0).GetComponent<Text>().text == "Buy")
				{
					checkpoint = 5;
				} //end if
				else
				{
					checkpoint = 2;
				} //end else

				//Play selection SFX
				AudioManager.instance.PlaySelect();
			} //end else if
		} //end else if X Key
	} //end GetInput

	/***************************************
	 * Name: NextPage
	 * Go to the next page of shop
	 ***************************************/
	public void NextPage()
	{
		if(checkpoint == 2 || checkpoint == 5)
		{
			page++;
			int count = toDisplay.Count / 15;
			if (page > count || (toDisplay.Count % 15 == 0 && page == count))
			{
				page = 0;
			} //end if
		} //end if
	} //end NextPage

	/***************************************
	 * Name: PreviousPage
	 * Go to the next page of shop
	 ***************************************/
	public void PreviousPage()
	{
		if(checkpoint == 2 || checkpoint == 5)
		{
			page--;
			if (page < 0)
			{
				if (toDisplay.Count % 15 != 0)
				{
					page = toDisplay.Count / 15;
				}//end if
				else
				{
					page = toDisplay.Count / 15 - 1;
				} //end else
			} //end if
		} //end if
	} //end PreviousPage

	/***************************************
	 * Name: IncreaseQuantity
	 * Adds one to the purchase quantity
	 ***************************************/
	public void IncreaseQuantity()
	{
		//Purchase
		if (checkpoint == 3)
		{
			int quantity = int.Parse(purchaseRegion.transform.FindChild("Quantity").GetComponent<InputField>().text);
			quantity++;
			purchaseRegion.transform.FindChild("Quantity").GetComponent<InputField>().text = quantity.ToString();
			int cost = selectedItem <= endPokemon ? pokemonCost[selectedItem] : 
			DataContents.ExecuteSQL<int>("SELECT cost FROM Items WHERE rowid=" + toDisplay[selectedItem]);
			cost *= quantity;
			purchaseRegion.transform.FindChild("Total").GetComponent<Text>().text = cost.ToString();
		} //end if

		//Sale
		else if(checkpoint == 6)
		{
			int quantity = int.Parse(purchaseRegion.transform.FindChild("Quantity").GetComponent<InputField>().text);
			quantity++;
			int itemCount = GameManager.instance.GetTrainer().ItemCount(toDisplay[selectedItem]);
			if (quantity > itemCount)
			{
				quantity = itemCount;
			} //end if
			purchaseRegion.transform.FindChild("Quantity").GetComponent<InputField>().text = quantity.ToString();
			int cost = DataContents.ExecuteSQL<int>("SELECT cost FROM Items WHERE rowid=" + toDisplay[selectedItem]) / 2;
			cost *= quantity;
			purchaseRegion.transform.FindChild("Total").GetComponent<Text>().text = cost.ToString();
		} //end else if

	} //end IncreaseQuantity

	/***************************************
	 * Name: DecreaseQuantity
	 * Subtracts one from the purchase quantity
	 ***************************************/
	public void DecreaseQuantity()
	{
		int quantity = int.Parse(purchaseRegion.transform.FindChild("Quantity").GetComponent<InputField>().text);
		quantity -= quantity > 0 ? 1 : 0; 
		purchaseRegion.transform.FindChild("Quantity").GetComponent<InputField>().text = quantity.ToString();

		//Purchase
		if (checkpoint == 3)
		{
			int cost = selectedItem <= endPokemon ? pokemonCost[selectedItem] : 
			DataContents.ExecuteSQL<int>("SELECT cost FROM Items WHERE rowid=" + toDisplay[selectedItem]);
			cost *= quantity;
			purchaseRegion.transform.FindChild("Total").GetComponent<Text>().text = cost.ToString();
		} //end if

		//Sale
		else if (checkpoint == 6)
		{
			purchaseRegion.transform.FindChild("Quantity").GetComponent<InputField>().text = quantity.ToString();
			int cost = DataContents.ExecuteSQL<int>("SELECT cost FROM Items WHERE rowid=" + toDisplay[selectedItem]) / 2;
			cost *= quantity;
			purchaseRegion.transform.FindChild("Total").GetComponent<Text>().text = cost.ToString();
		} //end else if
	} //end DecreaseQuantity

	/***************************************
	 * Name: ConfirmPurchase
	 * Allows purchase and returns to 
	 * shop
	 ***************************************/
	public void ConfirmPurchase()
	{
		//Purchase
		if (checkpoint == 3)
		{
			int cost = int.Parse(purchaseRegion.transform.FindChild("Total").GetComponent<Text>().text);
			if (GameManager.instance.GetTrainer().PlayerPoints >= cost)
			{
				int quantity = int.Parse(purchaseRegion.transform.FindChild("Quantity").GetComponent<InputField>().text);
				GameManager.instance.GetTrainer().PlayerPoints -= cost;
				if (filter < 7 &&  selectedItem < endPokemon)
				{
					for (int i = 0; i < quantity; i++)
					{
						bool buyShiny = GameManager.instance.RandomInt(0, 100) == 5;
						Pokemon newPokemon = new Pokemon(toDisplay[selectedItem],oType: (int)ObtainTypeEnum.Bought, 
							oWhere: (int)ObtainFromEnum.Shop, shiny: buyShiny);
						GameManager.instance.GetTrainer().AddToPC(
							GameManager.instance.GetTrainer().GetPCBox(), 0, newPokemon);
					} //end for
					GameManager.instance.DisplayText("Bought " + quantity + " " + DataContents.ExecuteSQL<string> 
						("SELECT name FROM Pokemon WHERE rowid=" + toDisplay[selectedItem]) + " for " + cost + ".", true);
				} //end if
				else
				{
					GameManager.instance.GetTrainer().AddItem(toDisplay[selectedItem], quantity);
					GameManager.instance.DisplayText("Bought " + quantity + " " + DataContents.GetItemGameName(toDisplay[selectedItem]) +
					" for " + cost + ".", true);
				} //end else
			} //end if
			else
			{
				GameManager.instance.DisplayText("Not enough points to afford this transaction. Canceled.", true);
			} //end else
			purchaseRegion.SetActive(false);
			selection.SetActive(false);
			EventSystem.current.SetSelectedGameObject(itemRegion.transform.GetChild(selectedItem - (page * 15)).gameObject);
			checkpoint = 2;
		} //end if

		//Sale
		else if (checkpoint == 6)
		{
			int cost = int.Parse(purchaseRegion.transform.FindChild("Total").GetComponent<Text>().text);
			int quantity = 	int.Parse(purchaseRegion.transform.FindChild("Quantity").GetComponent<InputField>().text);
			GameManager.instance.GetTrainer().RemoveItem(toDisplay[selectedItem], quantity);
			GameManager.instance.GetTrainer().PlayerPoints += cost;
			GameManager.instance.DisplayText("Sold " + quantity + " " + DataContents.GetItemGameName(toDisplay[selectedItem]) +
				" for " + cost + ".", true);
			purchaseRegion.SetActive(false);
			selection.SetActive(false);
			EventSystem.current.SetSelectedGameObject(itemRegion.transform.GetChild(selectedItem - (page * 15)).gameObject);
			toDisplay = GameManager.instance.GetTrainer().GetBagInventory();
			checkpoint = 5;
		} //end else if
	} //end ConfirmPurchase

	/***************************************
     * Name: Codes
     * Allows player to input codes
     ***************************************/ 
	public IEnumerator Codes()
	{
		//Process at end of frame
		yield return new WaitForEndOfFrame();

		if (checkpoint == 2 || checkpoint == 5)
		{
			selection.SetActive(false);
			codeRegion.SetActive(true);
			codeRegion.GetComponent<InputField>().ActivateInputField();
			checkpoint = 7;
		} //end if
	} //end Codes

	/***************************************
     * Name: ChangeFilter
     * Changes shop to display only items
     * within the filter
     ***************************************/ 
	public IEnumerator ChangeFilter(int requested)
	{		
		//Process at end of frame
		yield return new WaitForEndOfFrame();

		//Update filter
		if (checkpoint == 2)
		{
			filter = requested;
			displayPokemon = filter < 7 ? true : false;
			checkpoint = 1;
		} //end if
		else
		{
			GameObject.Find("Filters").GetComponent<Dropdown>().value = filter;
		} //end if
	} //end ChangeFilter(int requested)

	/***************************************
     * Name: ItemMode
     * Changes shop between buy and sell mode
     ***************************************/ 
	public IEnumerator ItemMode()
	{
		//Process at end of frame
		yield return new WaitForEndOfFrame();

		if (checkpoint == 2)
		{			
			GameObject.Find("Navigation").transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Buy";
			processing = false;
			checkpoint = 4;
		} //end if

		else if (checkpoint == 5)
		{
			GameObject.Find("Navigation").transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Sell";
			processing = false;
			checkpoint = 1;
		} //end else if
	} //end ItemMode

	/***************************************
	 * Name: CancelPurchase
	 * Cancels purchase and returns to 
	 * shop
	 ***************************************/
	public IEnumerator CancelPurchase()
	{
		//Process at end of frame
		yield return new WaitForEndOfFrame();

		//Turn off purchase and move to main processing
		purchaseRegion.SetActive(false);
		selection.SetActive(false);
		EventSystem.current.SetSelectedGameObject(itemRegion.transform.GetChild(selectedItem - (page * 15)).gameObject);

		//Return to Purchase (main processing) if canceling a purchase, or return to Sale otherwise
		checkpoint = checkpoint == 3 ? 2 : 5;
	} //end CancelPurchase

	/***************************************
	 * Name: SetupPurchase
	 * Displays purchase box
	 ***************************************/
	IEnumerator SetupPurchase()
	{
		//Process at end of frame
		yield return new WaitForEndOfFrame();

		purchaseRegion.SetActive(true);

		if (checkpoint == 2)
		{
			purchaseRegion.transform.FindChild("Sprite").GetComponent<Image>().sprite = selectedItem <= endPokemon ?
			Resources.Load<Sprite>("Sprites/Icons/icon" + toDisplay[selectedItem].ToString("000")) : 
			Resources.Load<Sprite>("Sprites/Icons/item" + toDisplay[selectedItem].ToString("000"));
			purchaseRegion.transform.FindChild("Quantity").GetComponent<InputField>().text = "1";
			purchaseRegion.transform.FindChild("Total").GetComponent<Text>().text = selectedItem <= endPokemon ?
			pokemonCost[selectedItem].ToString() : DataContents.ExecuteSQL<string>("SELECT cost FROM Items WHERE rowid=" +
			toDisplay[selectedItem]);
			EventSystem.current.SetSelectedGameObject(purchaseRegion.transform.FindChild("Buttons").GetChild(0).gameObject);
			selection.SetActive(false);
			checkpoint = 3;
		} //end if

		else if (checkpoint == 5)
		{
			purchaseRegion.transform.FindChild("Sprite").GetComponent<Image>().sprite =  
				Resources.Load<Sprite>("Sprites/Icons/item" + toDisplay[selectedItem].ToString("000"));
			purchaseRegion.transform.FindChild("Quantity").GetComponent<InputField>().text = "1";
			int salePrice =	DataContents.ExecuteSQL<int>("SELECT cost FROM Items WHERE rowid=" + toDisplay[selectedItem])/2;
			purchaseRegion.transform.FindChild("Total").GetComponent<Text>().text = salePrice.ToString();
			EventSystem.current.SetSelectedGameObject(purchaseRegion.transform.FindChild("Buttons").GetChild(0).gameObject);
			selection.SetActive(false);
			checkpoint = 6;
		} //end else if
	} //end SetupPurchase

	/***************************************
	 * Name: WaitForResize
	 * Resizes selection rect to fit current
	 * selected object
	 ***************************************/
	IEnumerator WaitForResize()
	{
		//Process at end of frame
		yield return new WaitForEndOfFrame();

		//Make sure event system object isn't null
		if (EventSystem.current.currentSelectedGameObject != null &&
			EventSystem.current.currentSelectedGameObject.name != "Blocker" &&
			EventSystem.current.currentSelectedGameObject.name != "CodeInput")
		{
			//Resize to highlighted navigation object
			Vector3 scale = new Vector3(EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().rect.width,
				               EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().rect.height, 0);
			selection.GetComponent<RectTransform>().sizeDelta = scale;
			selection.transform.position = Camera.main.WorldToScreenPoint(EventSystem.current.currentSelectedGameObject.transform.position);
			selection.SetActive(true);
		} //end if
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
