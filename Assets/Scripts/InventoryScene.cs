/***************************************************************************************** 
 * File:    InventoryScene.cs
 * Summary: Handles process for Inventory
 *****************************************************************************************/ 
#region Using
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
#endregion

public class InventoryScene : MonoBehaviour 
{
	#region Variables
	//Scene variables
	int checkpoint = 0;			//Manage function progress
	int inventorySpot = 0;		//What slot in pocket the player is on
	bool processing = false;	//Whether a function is already processing something
	Image background;			//Background bag image
	Image sprite;				//Item currently highlighted
	Text description;			//The item's description
	Text quantity;				//How many of the item there are
	Text itemName;				//The name of the item
	#endregion

	#region Methods
	/***************************************
	 * Name: RunInventory
	 * Play the inventory scene
	 ***************************************/
	public void RunInventory()
	{
		//Initialize scene variables
		if (checkpoint == 0)
		{
			//Set checkpoint delegate
			GameManager.instance.checkDel = ChangeCheckpoint;

			//Initialize references
			background = GameObject.Find("Background").GetComponent<Image>();
			sprite = GameObject.Find("Sprite").GetComponent<Image>();
			description = GameObject.Find("Description").GetComponent<Text>();
			quantity = GameObject.Find("Quantity").GetComponent<Text>();
			itemName = GameObject.Find("ItemName").GetComponent<Text>();

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

			//Intialize scene to starting state
			int currentPocket = GameManager.instance.GetTrainer().GetCurrentBagPocket();
			background.sprite = Resources.Load<Sprite>("Sprites/Menus/bag" + currentPocket);
			List<int> item = GameManager.instance.GetTrainer().GetItem(inventorySpot);
			if (item.Any())
			{
				string itemIcon = "Sprites/Icons/item" + item[0].ToString("000");
				Debug.Log(itemIcon);
				sprite.sprite = Resources.Load<Sprite>(itemIcon);
				description.text = DataContents.ExecuteSQL<string>("SELECT description FROM Items WHERE rowid=" + item[0]);

				//Fill inventory
				quantity.text = "";
				itemName.text = "";
				for (int i = 0; i < GameManager.instance.GetTrainer().SlotCount(); i++)
				{
					item = GameManager.instance.GetTrainer().GetItem(i);
					quantity.text += item[1] + "\n";
					itemName.text += DataContents.GetItemGameName(item[0]) + "\n";
				} //end for
			} //end if

			//Move to next checkpoint
			GameManager.instance.FadeInAnimation(2);
		} //end else if
		else if (checkpoint == 2)
		{			
			//Get player input
			GetInput();


		} //end else if
	} //end RunInventory

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

		} //end else if Mouse Moves Left

		/*********************************************
		* Mouse Moves Right
		**********************************************/
		else if (Input.GetAxis("Mouse X") > 0)
		{

		} //end else if Mouse Moves Right

		/*********************************************
		* Mouse Moves Up
		**********************************************/
		else if (Input.GetAxis("Mouse Y") > 0)
		{

		} //end else if Mouse Moves Up

		/*********************************************
		* Mouse Moves Down
		**********************************************/
		else if (Input.GetAxis("Mouse Y") < 0)
		{

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
	 * Name: ChangeCheckpoint
	 * Changes the checkpoint
	 ***************************************/
	public void ChangeCheckpoint(int newCheckpoint)
	{
		checkpoint = newCheckpoint;
		processing = false;
	} //end ChangeCheckpoint(int newCheckpoint)
	#endregion
} //end InventoryScene class
