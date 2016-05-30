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
	int inventorySpot;			//What slot in pocket the player is on
	int topShown;				//The top slot displayed in the inventory
	int bottomShown;			//The bottom slot displayed in the inventory
	float jumpAmount;			//How much the scrollbar should move for movement
	bool processing = false;	//Whether a function is already processing something
	GameObject choices;			//Choices box from scene tools
	GameObject selection;		//Selection rectangle from scene tools
	GameObject container;		//Contains entire list of items in bag
	ScrollRect viewport;		//Shows a few items in the bag
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
			selection = GameManager.tools.transform.FindChild("Selection").gameObject;
		 	choices = GameManager.tools.transform.FindChild("ChoiceUnit").gameObject;
			container = GameObject.Find("InventoryContainer").gameObject;
			viewport = GameObject.Find("InventoryRegion").GetComponent<ScrollRect>();
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
			inventorySpot = 0;
			topShown = 0;
			bottomShown = 8;
			if (GameManager.instance.GetTrainer().SlotCount() > 0)
			{			
				List<int> item = GameManager.instance.GetTrainer().GetItem(inventorySpot);
				string itemIcon = "Sprites/Icons/item" + item[0].ToString("000");
				sprite.sprite = Resources.Load<Sprite>(itemIcon);
				description.text = DataContents.ExecuteSQL<string>("SELECT description FROM Items WHERE rowid=" + item[0]);

				//Fill inventory
				quantity.text = "<color=red>" + item[1] + "</color>\n";
				itemName.text = "<color=red>" + DataContents.GetItemGameName(item[0]) + "</color>\n";
				for (int i = 1; i < GameManager.instance.GetTrainer().SlotCount() - 1; i++)
				{
					item = GameManager.instance.GetTrainer().GetItem(i);
					quantity.text += item[1] + "\n";
					itemName.text += DataContents.GetItemGameName(item[0]) + "\n";
				} //end for
				item = GameManager.instance.GetTrainer().GetItem(GameManager.instance.GetTrainer().SlotCount() - 1);
				quantity.text += item[1];
				itemName.text += DataContents.GetItemGameName(item[0]);
			} //end if

			//Set viewport size
			StartCoroutine(ResizeContainer());

			//Move to next checkpoint
			GameManager.instance.FadeInAnimation(2);
		} //end else if
		else if (checkpoint == 2)
		{			
			//Get player input
			GetInput();

			//Update scene
			if (GameManager.instance.GetTrainer().SlotCount() > 0)
			{
				List<int> item = GameManager.instance.GetTrainer().GetItem(inventorySpot);
				string itemIcon = "Sprites/Icons/item" + item[0].ToString("000");
				sprite.sprite = Resources.Load<Sprite>(itemIcon);
				description.text = DataContents.ExecuteSQL<string>("SELECT description FROM Items WHERE rowid=" + item[0]);

				//Fill inventory
				quantity.text = "";
				itemName.text = "";
				for (int i = 0; i < GameManager.instance.GetTrainer().SlotCount() - 1; i++)
				{
					item = GameManager.instance.GetTrainer().GetItem(i);
					quantity.text += inventorySpot == i ? "<color=red>" + item[1] + "</color>\n" :
						item[1] + "\n";
					itemName.text += inventorySpot == i ? "<color=red>" + DataContents.GetItemGameName(item[0]) + "</color>\n": 
						DataContents.GetItemGameName(item[0]) + "\n";
				} //end for
				item = GameManager.instance.GetTrainer().GetItem(GameManager.instance.GetTrainer().SlotCount() - 1);
				quantity.text += inventorySpot == GameManager.instance.GetTrainer().SlotCount() - 1 ? 
					"<color=red>" + item[1].ToString() + "</color>" : item[1].ToString();
				itemName.text += inventorySpot == GameManager.instance.GetTrainer().SlotCount() - 1 ? 
					"<color=red>" + DataContents.GetItemGameName(item[0]) + "</color>": DataContents.GetItemGameName(item[0]);
			} //end if
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
			//Normal Processing
			if (checkpoint == 2)
			{
				GameManager.instance.GetTrainer().PreviousPocket();
				checkpoint = 1;
			} //end if
		} //end if Left Arrow

		/*********************************************
		* Right Arrow
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			//Normal Processing
			if (checkpoint == 2)
			{
				GameManager.instance.GetTrainer().NextPocket();
				checkpoint = 1;
			} //end if
		} //end else if Right Arrow

		/*********************************************
		* Up Arrow
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			inventorySpot = ExtensionMethods.BindToInt(inventorySpot - 1, 0);
			if (inventorySpot < topShown)
			{
				topShown = inventorySpot;
				bottomShown = inventorySpot + 8;
				if (inventorySpot == 0)
				{
					viewport.verticalScrollbar.value = 1;
				} //end if
				else if (inventorySpot == GameManager.instance.GetTrainer().SlotCount() - 10)
				{					
					float num = 1;
					float den = GameManager.instance.GetTrainer().SlotCount() - 8;
					float constant = quantity.lineSpacing * 0.015f;
					viewport.verticalScrollbar.value = num / den - constant;
				} //end else if
				else
				{	
					viewport.verticalScrollbar.value -= jumpAmount;
					Debug.Log(viewport.verticalScrollbar.value);
				} //end else
			} //end if
		} //end else if Up Arrow

		/*********************************************
		* Down Arrow
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			inventorySpot = ExtensionMethods.CapAtInt(inventorySpot + 1, GameManager.instance.GetTrainer().SlotCount() - 1);
			if (inventorySpot > bottomShown)
			{
				bottomShown = inventorySpot;
				topShown = inventorySpot - 8;
				if (inventorySpot == GameManager.instance.GetTrainer().SlotCount() - 1)
				{
					viewport.verticalScrollbar.value = 0;
				} //end if
				else if (inventorySpot == GameManager.instance.GetTrainer().SlotCount() - 10)
				{
					float num = GameManager.instance.GetTrainer().SlotCount() - inventorySpot;
					float den = GameManager.instance.GetTrainer().SlotCount() - 8;
					float constant = quantity.lineSpacing * 0.015f;
					viewport.verticalScrollbar.value = num / den + constant;
				} //end else if
				else
				{
					viewport.verticalScrollbar.value += jumpAmount;
				} //end else
			} //end if
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
			//Normal Processing
			if (checkpoint == 2 && Input.mousePosition.y > selection.transform.position.y +
				selection.GetComponent<RectTransform>().rect.height/2)
			{

			} //end if
		} //end else if Mouse Moves Up

		/*********************************************
		* Mouse Moves Down
		**********************************************/
		else if (Input.GetAxis("Mouse Y") < 0)
		{
			//Normal Processing
			if (checkpoint == 2 && Input.mousePosition.y < selection.transform.position.y -
				selection.GetComponent<RectTransform>().rect.height/2)
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
	 * Name: ResizeContainer
	 * Sets container size to size of contents
	 ***************************************/
	IEnumerator ResizeContainer()
	{
		//Process at end of frame
		yield return new WaitForEndOfFrame();

		container.GetComponent<RectTransform>().sizeDelta = new Vector2(0, quantity.rectTransform.rect.height-300);
		viewport.verticalNormalizedPosition = 1f;

		float num = ExtensionMethods.BindToInt(GameManager.instance.GetTrainer().SlotCount() - 9 - 10, 0);
		float den = GameManager.instance.GetTrainer().SlotCount() - 8;
		float constant = quantity.lineSpacing * 0.001f;
		float valueOne = num / den;
		num = ExtensionMethods.BindToInt(GameManager.instance.GetTrainer().SlotCount() - 9 - 11, 0);
		float valueTwo = num / den;
		jumpAmount = valueTwo - valueOne - constant;
	} //end ResizeContainer

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
