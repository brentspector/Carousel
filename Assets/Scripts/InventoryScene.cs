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
	int subMenuChoice;          //What choice is highlighted in the pokemon submenu
	float jumpAmount;			//How much the scrollbar should move for movement
	bool processing = false;	//Whether a function is already processing something
	GameObject choices;			//Choices box from scene tools
	GameObject selection;		//Selection rectangle from scene tools
	GameObject viewport;		//The items shown to the player
	Image background;			//Background bag image
	Image sprite;				//Item currently highlighted
	Text description;			//The item's description
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
			viewport = GameObject.Find("InventoryRegion").gameObject;
			background = GameObject.Find("Background").GetComponent<Image>();
			sprite = GameObject.Find("Sprite").GetComponent<Image>();
			description = GameObject.Find("Description").GetComponent<Text>();

			//Populate choices menu
			FillInChoices();

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
			bottomShown = 9;
			List<int> item = GameManager.instance.GetTrainer().GetItem(0);
			//Fill in first slot
			if (GameManager.instance.GetTrainer().SlotCount() - 1 < 0)
			{
				viewport.transform.GetChild(0).GetComponent<Text>().text = "";
			} //end if
			else
			{
				viewport.transform.GetChild(0).GetComponent<Text>().text = "<color=red>" +
				item[1]	+ " - " + DataContents.GetItemGameName(item[0]) + "</color>";
			} //end else

			//Fill in sprite and description
			if (GameManager.instance.GetTrainer().SlotCount() != 0)
			{				
				sprite.color = Color.white;
				sprite.sprite = Resources.Load<Sprite>("Sprites/Icons/item" + item[0].ToString("000"));
				description.text = DataContents.ExecuteSQL<string>("SELECT description FROM Items WHERE rowid=" + item[0]);
			} //end if
			else
			{
				sprite.color = Color.clear;
				description.text = "";
			} //end else

			//Fill in remaining slots
			for (int i = 1; i < 10; i++)
			{
				if (GameManager.instance.GetTrainer().SlotCount() - 1 < i)
				{
					viewport.transform.GetChild(i).GetComponent<Text>().text = "";
				} //end if
				else
				{
					viewport.transform.GetChild(i).GetComponent<Text>().text = GameManager.instance.GetTrainer().GetItem(inventorySpot + i)[1]
					+ " - " + DataContents.GetItemGameName(GameManager.instance.GetTrainer().GetItem(inventorySpot + i)[0]);
				} //end else
			} //end for

			//Move to next checkpoint
			GameManager.instance.FadeInAnimation(2);
		} //end else if
		else if (checkpoint == 2)
		{			
			//Get player input
			GetInput();

			//Fill in slots
			for (int i = 0; i < 10; i++)
			{
				if (GameManager.instance.GetTrainer().SlotCount() - 1 < topShown + i)
				{
					viewport.transform.GetChild(i).GetComponent<Text>().text = "";
				} //end if
				else
				{
					if (topShown + i == inventorySpot)
					{
						viewport.transform.GetChild(i).GetComponent<Text>().text = "<color=red>" +
						GameManager.instance.GetTrainer().GetItem(topShown + i)[1] + " - " +
						DataContents.GetItemGameName(GameManager.instance.GetTrainer().GetItem(topShown + i)[0]) + "</color>";
					} //end if
					else
					{
						viewport.transform.GetChild(i).GetComponent<Text>().text = GameManager.instance.GetTrainer().GetItem(topShown + i)[1]
						+ " - " + DataContents.GetItemGameName(GameManager.instance.GetTrainer().GetItem(topShown + i)[0]);
					} //end else
				} //end else
			} //end for

			//Fill in sprite and description
			if (GameManager.instance.GetTrainer().SlotCount() != 0)
			{
				List<int> item = GameManager.instance.GetTrainer().GetItem(inventorySpot);
				sprite.color = Color.white;
				sprite.sprite = Resources.Load<Sprite>("Sprites/Icons/item" + item[0].ToString("000"));
				description.text = DataContents.ExecuteSQL<string>("SELECT description FROM Items WHERE rowid=" + item[0]);
			} //end if
			else
			{
				sprite.color = Color.clear;
				description.text = "";
			} //end else
		} //end else if
		else if (checkpoint == 3)
		{
			//Get player input for submenu
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
			//Normal Processing
			if (checkpoint == 2)
			{
				inventorySpot = ExtensionMethods.BindToInt(inventorySpot - 1, 0);
				if (inventorySpot < topShown)
				{
					topShown = inventorySpot;
					bottomShown--;
				} //end if
			} //end if

			//Submenu Processing
			else if (checkpoint == 3)
			{
				//Decrease choice (higher slots are on lower children)
				subMenuChoice--;

				//If on first option, loop to end
				if (subMenuChoice < 0)
				{
					subMenuChoice = choices.transform.childCount - 1;
				} //end if

				//Reposition selection
				selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
			} //end else if
		} //end else if Up Arrow

		/*********************************************
		* Down Arrow
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			//Normal Processing
			if (checkpoint == 2)
			{
				inventorySpot = ExtensionMethods.CapAtInt(inventorySpot + 1, GameManager.instance.GetTrainer().SlotCount() - 1);
				if (inventorySpot > bottomShown)
				{
					bottomShown = inventorySpot;
					topShown++;
				} //end if
			} //end if

			//Submenu Processing
			else if (checkpoint == 3)
			{
				//Increase choice (lower slots are on higher children)
				subMenuChoice++;

				//If on last option, loop to first
				if (subMenuChoice > choices.transform.childCount - 1)
				{
					subMenuChoice = 0;
				} //end if

				//Reposition selection
				selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
			} //end else if
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
			//Submenu Processing
			if(checkpoint == 3 && Input.mousePosition.y > selection.transform.position.y +
				selection.GetComponent<RectTransform>().rect.height / 2)
			{
				//If not on last option, decrease (higher slots are on lower children)
				if (subMenuChoice > 0)
				{
					subMenuChoice--;
				} //end if

				//Reposition selection
				selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
			} //end if
		} //end else if Mouse Moves Up

		/*********************************************
		* Mouse Moves Down
		**********************************************/
		else if (Input.GetAxis("Mouse Y") < 0)
		{
			//Submenu Processing
			if(checkpoint == 3 && Input.mousePosition.y < selection.transform.position.y -
				selection.GetComponent<RectTransform>().rect.height / 2)
			{
				//If not on last option, increase (lower slots are on higher children)
				if (subMenuChoice < choices.transform.childCount - 1)
				{
					subMenuChoice++;
				} //end if

				//Reposition selection
				selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
			} //end if
		} //end else if Mouse Moves Down

		/*********************************************
		* Mouse Wheel Up
		**********************************************/
		else if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			//Normal Processing
			if (checkpoint == 2)
			{
				inventorySpot = ExtensionMethods.BindToInt(inventorySpot - 1, 0);
				if (inventorySpot < topShown)
				{
					topShown = inventorySpot;
					bottomShown--;
				} //end if
			} //end if
		} //end else if Mouse Wheel Up

		/*********************************************
		* Mouse Wheel Down
		**********************************************/
		else if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			//Normal Processing
			if (checkpoint == 2)
			{
				inventorySpot = ExtensionMethods.CapAtInt(inventorySpot + 1, GameManager.instance.GetTrainer().SlotCount() - 1);
				if (inventorySpot > bottomShown)
				{
					bottomShown = inventorySpot;
					topShown++;
				} //end if
			} //end if
		} //end else if Mouse Wheel Down

		/*********************************************
		* Left Mouse Button
		**********************************************/
		else if (Input.GetMouseButtonUp(0))
		{
			//Normal Processing
			if (checkpoint == 2)
			{
				//Turn on submenu
				selection.SetActive(true);
				choices.SetActive(true);

				//Set up selection box at end of frame if it doesn't fit
				if(selection.GetComponent<RectTransform>().sizeDelta != 
					choices.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta)
				{
					selection.SetActive(false);
					StartCoroutine(WaitForResize());
				} //end if

				checkpoint = 3;
			} //end if

			//Submenu Processing
			else if (checkpoint == 3)
			{
				//Apply appropriate action
				switch (subMenuChoice)
				{
					//Use
					case 0:
						break;
					//Give
					case 1:
						break;
					//Toss
					case 2:
						break;
					//Switch
					case 3:
						break;
					//Cancel
					case 4:
						selection.SetActive(false);
						choices.SetActive(false);
						checkpoint = 2;
						break;
				} //end switch
			} //end else if
		} //end else if Left Mouse Button

		/*********************************************
		* Right Mouse Button
		**********************************************/
		else if (Input.GetMouseButtonUp(1))
		{
			//Normal Processing
			if(checkpoint == 2)
			{
				GameManager.instance.LoadScene("MainGame", true);
			} //end if

			//Submenu Processing
			else if (checkpoint == 3)
			{
				selection.SetActive(false);
				choices.SetActive(false);
				checkpoint = 2;
			} //end else if
		} //end else if Right Mouse Button

		/*********************************************
		* Enter/Return Key
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.Return))
		{
			//Normal Processing
			if (checkpoint == 2)
			{
				//Turn on submenu
				selection.SetActive(true);
				choices.SetActive(true);

				//Set up selection box at end of frame if it doesn't fit
				if(selection.GetComponent<RectTransform>().sizeDelta != 
					choices.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta)
				{
					selection.SetActive(false);
					StartCoroutine(WaitForResize());
				} //end if

				checkpoint = 3;
			} //end if

			//Submenu Processing
			else if (checkpoint == 3)
			{
				//Apply appropriate action
				switch (subMenuChoice)
				{
					//Use
					case 0:
						break;
					//Give
					case 1:
						break;
					//Toss
					case 2:
						break;
					//Switch
					case 3:
						break;
					//Cancel
					case 4:
						selection.SetActive(false);
						choices.SetActive(false);
						checkpoint = 2;
						break;
				} //end switch
			} //end else if
		} //end else if Enter/Return Key

		/*********************************************
		* X Key
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.X))
		{
			//Normal Processing
			if(checkpoint == 2)
			{
				GameManager.instance.LoadScene("MainGame", true);
			} //end if

			//Submenu Processing
			else if (checkpoint == 3)
			{
				selection.SetActive(false);
				choices.SetActive(false);
				checkpoint = 2;
			} //end else if
		} //end else if X Key
	} //end GetInput

	/***************************************
	 * Name: FillInChoices
	 * Sets the choices for the choice menu
	 ***************************************/
	void FillInChoices()
	{
		//Add to choice menu if necessary
		for (int i = choices.transform.childCount; i < 5; i++)
		{
			GameObject clone = Instantiate(choices.transform.GetChild(0).gameObject,
				choices.transform.GetChild(0).position,
				Quaternion.identity) as GameObject;
			clone.transform.SetParent(choices.transform);
		} //end for

		//Destroy extra chocies
		for (int i = choices.transform.childCount; i > 5; i--)
		{
			Destroy(choices.transform.GetChild(i - 1).gameObject);
		} //end for

		//Set text for each choice
		choices.transform.GetChild(0).GetComponent<Text>().text = "Use";
		choices.transform.GetChild(1).GetComponent<Text>().text = "Give";
		choices.transform.GetChild(2).GetComponent<Text>().text = "Toss";
		choices.transform.GetChild(3).GetComponent<Text>().text = "Switch";
		choices.transform.GetChild(4).GetComponent<Text>().text = "Cancel";
	} //end FillInChoices

	/***************************************
	 * Name: PositionChoices
	 * Places choices in bottom right of screen
	 ***************************************/
	IEnumerator PositionChoices()
	{
		//Process at end of frame
		yield return new WaitForEndOfFrame();

		//Reposition choices to bottom right
		choices.GetComponent<RectTransform>().position = new Vector3(
			choices.GetComponent<RectTransform>().position.x,
			choices.GetComponent<RectTransform>().rect.height / 2);

		//Reposition selection to top menu choicec
		selection.transform.position = choices.transform.GetChild(0).position;
	} //end PositionChoices

	/***************************************
	 * Name: WaitForResize
	 * Waits for choice menu to resize before
	 * setting selection dimensions
	 ***************************************/
	IEnumerator WaitForResize()
	{
		//Process at end of frame
		StartCoroutine(PositionChoices());
		yield return new WaitForEndOfFrame();

		Vector3 scale = new Vector3(choices.GetComponent<RectTransform>().rect.width,
			choices.GetComponent<RectTransform>().rect.height /
			choices.transform.childCount, 0);
		selection.GetComponent<RectTransform>().sizeDelta = scale;
		selection.transform.position = choices.transform.GetChild(0).
			GetComponent<RectTransform>().position;
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
} //end InventoryScene class
