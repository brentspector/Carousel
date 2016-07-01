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
	int teamSlot;				//The slot currently highlighted
	int previousTeamSlot;       //The slot last highlighted
	int switchSpot;				//The spot selected for switching to
	bool processing = false;	//Whether a function is already processing something
	bool pickMove = false;		//Whether the player must pick a move to use an item on
	GameObject playerTeam;		//Team screen for using an item on
	GameObject choices;			//Choices box from scene tools
	GameObject selection;		//Selection rectangle from scene tools
	GameObject viewport;		//The items shown to the player
	GameObject currentTeamSlot; //The object that is currently highlighted on the team
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

			//Set confirm delegate
			GameManager.instance.confirmDel = ApplyConfirm;

			//Initialize references
			selection = GameManager.tools.transform.FindChild("Selection").gameObject;
			choices = GameManager.tools.transform.FindChild("ChoiceUnit").gameObject;
			playerTeam = GameObject.Find("Team").gameObject;
			viewport = GameObject.Find("InventoryRegion").gameObject;
			background = GameObject.Find("Background").GetComponent<Image>();
			sprite = GameObject.Find("Canvas").transform.FindChild("Sprite").GetComponent<Image>();
			description = GameObject.Find("Description").GetComponent<Text>();

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

			//Move to checkpoint 7 if picking a move
			if (pickMove)
			{
				choices.SetActive(true);
				checkpoint = 7;
				return;
			} //end if

			//Begin processing
			processing = true;

			//Populate choices menu
			FillInChoices();

			//Intialize scene to starting state
			playerTeam.transform.FindChild("PartyInstructions").GetChild(0).GetComponent<Text>().text = 
				"These are your pokemon.";
			int currentPocket = GameManager.instance.GetTrainer().GetCurrentBagPocket();
			background.sprite = Resources.Load<Sprite>("Sprites/Menus/bag" + currentPocket);
			inventorySpot = 0;
			topShown = 0;
			bottomShown = 9;

			//Fill in first slot
			List<int> item = new List<int>();
			if (GameManager.instance.GetTrainer().SlotCount() - 1 < 0)
			{
				viewport.transform.GetChild(0).GetComponent<Text>().text = "";
			} //end if
			else
			{
				item = GameManager.instance.GetTrainer().GetItem(0);
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

			//Fill in all team data
			for (int i = 0; i < GameManager.instance.GetTrainer().Team.Count; i++)
			{
				//Activate slots
				playerTeam.transform.FindChild("Background").GetChild(i).gameObject.SetActive(true);
				playerTeam.transform.FindChild("Pokemon" + (i + 1)).gameObject.SetActive(true);

				//If on first slot
				if (i == 0)
				{
					//If pokemon in first slot is fainted
					if (GameManager.instance.GetTrainer().Team[i].Status == 1)
					{
						playerTeam.transform.FindChild("Background").GetChild(i).
						GetComponent<Image>().sprite = Resources.Load<Sprite>
							("Sprites/Menus/partyPanelRoundSelFnt");
					} //end if
					//Otherwise give regular slot image
					else
					{
						playerTeam.transform.FindChild("Background").GetChild(i).
						GetComponent<Image>().sprite = Resources.Load<Sprite>
							("Sprites/Menus/partyPanelRoundSel");
					} //end else

					//Set party ball to selected
					playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("PartyBall").
					GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/partyBallSel");
				} //end if
				//Fill in any other slot
				else
				{
					//If pokemon in slot is fainted
					if (GameManager.instance.GetTrainer().Team[i].Status == 1)
					{
						playerTeam.transform.FindChild("Background").GetChild(i).GetComponent<Image>().sprite =
							Resources.Load<Sprite>("Sprites/Menus/partyPanelRectFnt");
					} //end if
					//Otherwise give regular slot image
					else
					{
						playerTeam.transform.FindChild("Background").GetChild(i).GetComponent<Image>().sprite =
							Resources.Load<Sprite>("Sprites/Menus/partyPanelRect");
					} //end else

					//Set party ball to unselected
					playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("PartyBall").
					GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/partyBall");
				} //end else

				//Set status
				SetStatusIcon(playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("Status").
					GetComponent<Image>(), GameManager.instance.GetTrainer().Team[i]);

				//Set sprite
				playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("Sprite").GetComponent<Image>()
					.sprite = GetCorrectIcon(GameManager.instance.GetTrainer().Team[i]);

				//Set nickname
				playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("Nickname").GetComponent<Text>().text =
					GameManager.instance.GetTrainer().Team[i].Nickname;

				//Set item
				if (GameManager.instance.GetTrainer().Team[i].Item != 0)
				{
					playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("Item").GetComponent<Image>().color =
						Color.white;						
				} //end if
				else
				{
					playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("Item").GetComponent<Image>().color =
						Color.clear;	
				} //end else 

				//Set remaining HP
				playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("RemainingHP").GetComponent<RectTransform>().
				localScale = new Vector3((float)GameManager.instance.GetTrainer().Team[i].CurrentHP / (float)GameManager.
					instance.GetTrainer().Team[i].TotalHP, 1f, 1f);

				//Set level
				playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("Level").GetComponent<Text>().text = "Lv." +
				GameManager.instance.GetTrainer().Team[i].CurrentLevel.ToString();

				//Set HP text
				playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("CurrentHP").GetComponent<Text>().text =
					GameManager.instance.GetTrainer().Team[i].CurrentHP.ToString() + "/" + GameManager.instance.GetTrainer().
					Team[i].TotalHP.ToString();
			} //end for

			//Deactivate any empty party spots
			for (int i = 5; i > GameManager.instance.GetTrainer().Team.Count - 1; i--)
			{
				playerTeam.transform.FindChild("Background").GetChild(i).gameObject.SetActive(false);
				playerTeam.transform.FindChild("Pokemon" + (i + 1)).gameObject.SetActive(false);
			} //end for

			//Turn off team
			playerTeam.SetActive(false);

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
		else if (checkpoint == 4)
		{
			//Get player input for use selection
			GetInput();

			//Change background sprites based on player input
			if (previousTeamSlot != teamSlot)
			{
				//Deactivate panel
				if (previousTeamSlot == 1)
				{
					//Adjust if pokemon is fainted
					if (GameManager.instance.GetTrainer().Team[previousTeamSlot - 1].Status != 1)
					{
						playerTeam.transform.FindChild("Background").GetChild(previousTeamSlot - 1).GetComponent<Image>().
						sprite = Resources.Load<Sprite>("Sprites/Menus/partypanelRound");
					} //end if
					else
					{
						playerTeam.transform.FindChild("Background").GetChild(previousTeamSlot - 1).GetComponent<Image>().
						sprite = Resources.Load<Sprite>("Sprites/Menus/partypanelRoundFnt");
					} //end else

					//Deactivate party ball
					playerTeam.transform.FindChild("Pokemon" + previousTeamSlot).FindChild("PartyBall").
					GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/partyBall");
				} //end if

				//As long as previous choice was greater than 0 (no buttons)
				else if (previousTeamSlot > 0)
				{
					//Adjust if pokemon is fainted
					if (GameManager.instance.GetTrainer().Team[previousTeamSlot - 1].Status != 1)
					{
						playerTeam.transform.FindChild("Background").GetChild(previousTeamSlot - 1).GetComponent<Image>().
						sprite = Resources.Load<Sprite>("Sprites/Menus/partypanelRect");
					} //end if
					else
					{
						playerTeam.transform.FindChild("Background").GetChild(previousTeamSlot - 1).GetComponent<Image>().
						sprite = Resources.Load<Sprite>("Sprites/Menus/partypanelRectFnt");
					} //end else

					//Deactivate party ball
					playerTeam.transform.FindChild("Pokemon" + previousTeamSlot).FindChild("PartyBall").
					GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/partyBall");
				} //end else if

				//Deactivate buttons
				playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Button>().interactable = false;
				playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Button>().interactable = false;
				playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Image>().color = Color.gray;
				playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Image>().color = Color.gray;

				//Activate panel
				//First slot selected
				if (teamSlot == 1)
				{
					//Adjust if pokemon is fainted
					if (GameManager.instance.GetTrainer().Team[teamSlot - 1].Status != 1)
					{
						playerTeam.transform.FindChild("Background").GetChild(teamSlot - 1).GetComponent<Image>().
						sprite = Resources.Load<Sprite>("Sprites/Menus/partyPanelRoundSel");
					} //end if
					else
					{
						playerTeam.transform.FindChild("Background").GetChild(teamSlot - 1).GetComponent<Image>().
						sprite = Resources.Load<Sprite>("Sprites/Menus/partyPanelRoundSelFnt");
					} //end else

					//Activate party ball
					playerTeam.transform.FindChild("Pokemon" + teamSlot).FindChild("PartyBall").
					GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/partyBallSel");
				} //end if
				//PC Button selected
				else if (teamSlot == -1)
				{
					playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Button>().interactable = true;
					playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Image>().color = Color.gray * 2;
				} //end else if
				//Cancel Button selected
				else if (teamSlot == 0)
				{
					playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Button>().interactable = true;
					playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Image>().color = Color.gray * 2;
				} //end else if
				//Any other slot
				else
				{
					//Adjust if pokemon is fainted
					if (GameManager.instance.GetTrainer().Team[teamSlot - 1].Status != 1)
					{
						playerTeam.transform.FindChild("Background").GetChild(teamSlot - 1).GetComponent<Image>().
						sprite = Resources.Load<Sprite>("Sprites/Menus/partyPanelRectSel");
					} //end if
					else
					{
						playerTeam.transform.FindChild("Background").GetChild(teamSlot - 1).GetComponent<Image>().
						sprite = Resources.Load<Sprite>("Sprites/Menus/partyPanelRectSelFnt");
					} //end else

					//Activate party ball
					playerTeam.transform.FindChild("Pokemon" + teamSlot).FindChild("PartyBall").
					GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/partyBallSel");
				} //end else

				//Update previous slot
				previousTeamSlot = teamSlot;
			} //end if
		} //end else if
		else if (checkpoint == 5)
		{
			//Get player input for give selection
			GetInput();

			//Change background sprites based on player input
			if (previousTeamSlot != teamSlot)
			{
				//Deactivate panel
				if (previousTeamSlot == 1)
				{
					//Adjust if pokemon is fainted
					if (GameManager.instance.GetTrainer().Team[previousTeamSlot - 1].Status != 1)
					{
						playerTeam.transform.FindChild("Background").GetChild(previousTeamSlot - 1).GetComponent<Image>().
						sprite = Resources.Load<Sprite>("Sprites/Menus/partypanelRound");
					} //end if
					else
					{
						playerTeam.transform.FindChild("Background").GetChild(previousTeamSlot - 1).GetComponent<Image>().
						sprite = Resources.Load<Sprite>("Sprites/Menus/partypanelRoundFnt");
					} //end else

					//Deactivate party ball
					playerTeam.transform.FindChild("Pokemon" + previousTeamSlot).FindChild("PartyBall").
					GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/partyBall");
				} //end if

				//As long as previous choice was greater than 0 (no buttons)
				else if (previousTeamSlot > 0)
				{
					//Adjust if pokemon is fainted
					if (GameManager.instance.GetTrainer().Team[previousTeamSlot - 1].Status != 1)
					{
						playerTeam.transform.FindChild("Background").GetChild(previousTeamSlot - 1).GetComponent<Image>().
						sprite = Resources.Load<Sprite>("Sprites/Menus/partypanelRect");
					} //end if
					else
					{
						playerTeam.transform.FindChild("Background").GetChild(previousTeamSlot - 1).GetComponent<Image>().
						sprite = Resources.Load<Sprite>("Sprites/Menus/partypanelRectFnt");
					} //end else

					//Deactivate party ball
					playerTeam.transform.FindChild("Pokemon" + previousTeamSlot).FindChild("PartyBall").
					GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/partyBall");
				} //end else if

				//Deactivate buttons
				playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Button>().interactable = false;
				playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Button>().interactable = false;
				playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Image>().color = Color.gray;
				playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Image>().color = Color.gray;

				//Activate panel
				//First slot selected
				if (teamSlot == 1)
				{
					//Adjust if pokemon is fainted
					if (GameManager.instance.GetTrainer().Team[teamSlot - 1].Status != 1)
					{
						playerTeam.transform.FindChild("Background").GetChild(teamSlot - 1).GetComponent<Image>().
						sprite = Resources.Load<Sprite>("Sprites/Menus/partyPanelRoundSel");
					} //end if
					else
					{
						playerTeam.transform.FindChild("Background").GetChild(teamSlot - 1).GetComponent<Image>().
						sprite = Resources.Load<Sprite>("Sprites/Menus/partyPanelRoundSelFnt");
					} //end else

					//Activate party ball
					playerTeam.transform.FindChild("Pokemon" + teamSlot).FindChild("PartyBall").
					GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/partyBallSel");
				} //end if
				//PC Button selected
				else if (teamSlot == -1)
				{
					playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Button>().interactable = true;
					playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Image>().color = Color.gray * 2;
				} //end else if
				//Cancel Button selected
				else if (teamSlot == 0)
				{
					playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Button>().interactable = true;
					playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Image>().color = Color.gray * 2;
				} //end else if
				//Any other slot
				else
				{
					//Adjust if pokemon is fainted
					if (GameManager.instance.GetTrainer().Team[teamSlot - 1].Status != 1)
					{
						playerTeam.transform.FindChild("Background").GetChild(teamSlot - 1).GetComponent<Image>().
						sprite = Resources.Load<Sprite>("Sprites/Menus/partyPanelRectSel");
					} //end if
					else
					{
						playerTeam.transform.FindChild("Background").GetChild(teamSlot - 1).GetComponent<Image>().
						sprite = Resources.Load<Sprite>("Sprites/Menus/partyPanelRectSelFnt");
					} //end else

					//Activate party ball
					playerTeam.transform.FindChild("Pokemon" + teamSlot).FindChild("PartyBall").
					GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/partyBallSel");
				} //end else

				//Update previous slot
				previousTeamSlot = teamSlot;
			} //end if
		} //end else if
		else if (checkpoint == 6)
		{
			//Get player input for switch selection
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
					else if (topShown + i == switchSpot)
					{
						viewport.transform.GetChild(i).GetComponent<Text>().text = "<color=blue>" +
						GameManager.instance.GetTrainer().GetItem(topShown + i)[1] + " - " +
						DataContents.GetItemGameName(GameManager.instance.GetTrainer().GetItem(topShown + i)[0]) + "</color>";
					} //end else if
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
				List<int> item = GameManager.instance.GetTrainer().GetItem(switchSpot);
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
		else if (checkpoint == 7)
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
			//Normal Processing
			if (checkpoint == 2)
			{
				GameManager.instance.GetTrainer().PreviousPocket();
				checkpoint = 1;
			} //end if

			//Use Processing
			else if (checkpoint == 4)
			{
				//Decrease (higher slots are on lower children)
				teamSlot--;

				//Loop to end of team if on Cancel button
				if (teamSlot < 0)
				{
					teamSlot = GameManager.instance.GetTrainer().Team.Count;
				} //end if

				//Set current slot choice if on pokemon slot
				if (teamSlot > 0)
				{
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + teamSlot).gameObject;
				} //end if

				//If on Cancel Button
				else if (teamSlot == 0)
				{
					currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(1).gameObject;
				} //end else if
			} //end else if

			//Give Processing
			else if (checkpoint == 5)
			{
				//Decrease (higher slots are on lower children)
				teamSlot--;

				//Loop to end of team if on Cancel button
				if (teamSlot < 0)
				{
					teamSlot = GameManager.instance.GetTrainer().Team.Count;
				} //end if

				//Set current slot choice if on pokemon slot
				if (teamSlot > 0)
				{
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + teamSlot).gameObject;
				} //end if

				//If on Cancel Button
				else if (teamSlot == 0)
				{
					currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(1).gameObject;
				} //end else if
			} //end else if
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

			//Use Processing
			else if (checkpoint == 4)
			{
				//Increase (lower slots are on higher children)
				teamSlot++;

				//Loop to PC button if at end of team
				if (teamSlot > GameManager.instance.GetTrainer().Team.Count)
				{
					teamSlot = 0;
				} //end if

				//Set current slot choice if on pokemon slot
				if (teamSlot > 0)
				{
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + teamSlot).gameObject;
				} //end if
				//If on Cancel Button
				else if (teamSlot == 0)
				{
					currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(1).gameObject;
				} //end else if
			} //end else if

			//Give Processing
			else if (checkpoint == 5)
			{
				//Increase (lower slots are on higher children)
				teamSlot++;

				//Loop to PC button if at end of team
				if (teamSlot > GameManager.instance.GetTrainer().Team.Count)
				{
					teamSlot = 0;
				} //end if

				//Set current slot choice if on pokemon slot
				if (teamSlot > 0)
				{
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + teamSlot).gameObject;
				} //end if
				//If on Cancel Button
				else if (teamSlot == 0)
				{
					currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(1).gameObject;
				} //end else if
			} //end else if
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

			//Use Processing
			else if (checkpoint == 4)
			{
				//Move from top slot to Canel button
				if (teamSlot == 1 || teamSlot == 2)
				{
					teamSlot = 0;
					currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(1).gameObject;
				} //end if
				//Move from Cancel Button to last team slot
				else if (teamSlot == 0)
				{
					teamSlot = GameManager.instance.GetTrainer().Team.Count;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + teamSlot).gameObject;
				} //end else if
				//Go up vertically
				else
				{
					teamSlot -= 2;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + teamSlot).gameObject;
				} //end else
			} //end else if

			//Give Processing
			else if (checkpoint == 5)
			{
				//Move from top slot to Canel button
				if (teamSlot == 1 || teamSlot == 2)
				{
					teamSlot = 0;
					currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(1).gameObject;
				} //end if
				//Move from Cancel Button to last team slot
				else if (teamSlot == 0)
				{
					teamSlot = GameManager.instance.GetTrainer().Team.Count;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + teamSlot).gameObject;
				} //end else if
				//Go up vertically
				else
				{
					teamSlot -= 2;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + teamSlot).gameObject;
				} //end else
			} //end else if

			//Item Move Processing
			else if(checkpoint == 6)
			{
				switchSpot = ExtensionMethods.BindToInt(switchSpot - 1, 0);
				if (switchSpot < topShown)
				{
					topShown = switchSpot;
					bottomShown--;
				} //end if
			} //end else if

			//Pick Move Processing
			else if (checkpoint == 7)
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

			//Use Processing
			else if (checkpoint == 4)
			{
				//Move from last or second to last slot to Cancel Button
				if ((teamSlot == GameManager.instance.GetTrainer().Team.Count-1 && teamSlot > 0) || 
					teamSlot == GameManager.instance.GetTrainer().Team.Count)
				{
					teamSlot = 0;
					currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(1).gameObject;
				} //end if
				//Move from Cancel button to first team slot
				else if (teamSlot == 0)
				{
					teamSlot = 1;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon1").gameObject;
				} //end else if
				//Go down vertically
				else
				{
					teamSlot += 2;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + teamSlot).gameObject;
				} //end else
			} //end else if

			//Give Processing
			else if (checkpoint == 5)
			{
				//Move from last or second to last slot to Cancel Button
				if ((teamSlot == GameManager.instance.GetTrainer().Team.Count-1 && teamSlot > 0) || 
					teamSlot == GameManager.instance.GetTrainer().Team.Count)
				{
					teamSlot = 0;
					currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(1).gameObject;
				} //end if
				//Move from Cancel button to first team slot
				else if (teamSlot == 0)
				{
					teamSlot = 1;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon1").gameObject;
				} //end else if
				//Go down vertically
				else
				{
					teamSlot += 2;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + teamSlot).gameObject;
				} //end else
			} //end else if

			//Item Move Processing
			else if(checkpoint == 6)
			{
				switchSpot = ExtensionMethods.CapAtInt(switchSpot + 1, GameManager.instance.GetTrainer().SlotCount() - 1);
				if (switchSpot > bottomShown)
				{
					bottomShown = switchSpot;
					topShown++;
				} //end if
			} //end else if

			//Pick Move Processing
			else if (checkpoint == 7)
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
		if (Input.GetAxis("Mouse X") < 0)
		{
			//Use Processing
			if (checkpoint == 4 && Input.mousePosition.x < Camera.main.WorldToScreenPoint(
				currentTeamSlot.transform.position).x - currentTeamSlot.GetComponent<RectTransform>().rect.width / 2)
			{
				//If team slot is not odd, and is greater than 0, move left
				if ((teamSlot & 1) != 1 && teamSlot > 0)
				{
					teamSlot--;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + teamSlot).gameObject;
				} //end if
			} //end if

			//Give Processing
			else if (checkpoint == 5 && Input.mousePosition.x < Camera.main.WorldToScreenPoint(
				currentTeamSlot.transform.position).x - currentTeamSlot.GetComponent<RectTransform>().rect.width / 2)
			{
				//If team slot is not odd, and is greater than 0, move left
				if ((teamSlot & 1) != 1 && teamSlot > 0)
				{
					teamSlot--;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + teamSlot).gameObject;
				} //end if
			} //end else if
		} //end else if Mouse Moves Left

		/*********************************************
		* Mouse Moves Right
		**********************************************/
		else if (Input.GetAxis("Mouse X") > 0)
		{
			//Use Processing
			if (checkpoint == 4 && Input.mousePosition.x > Camera.main.WorldToScreenPoint(
				currentTeamSlot.transform.position).x + currentTeamSlot.GetComponent<RectTransform>().rect.width / 2)
			{
				//If choice number is odd, and team is not odd numbered, and choice is greater than 0, move right
				if ((teamSlot & 1) == 1 && teamSlot != GameManager.instance.GetTrainer().Team.Count && teamSlot > 0)
				{
					teamSlot++;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + teamSlot).gameObject;
				} //end if
			} //end if 

			//Give Processing
			else if (checkpoint == 5 && Input.mousePosition.x > Camera.main.WorldToScreenPoint(
				currentTeamSlot.transform.position).x + currentTeamSlot.GetComponent<RectTransform>().rect.width / 2)
			{
				//If choice number is odd, and team is not odd numbered, and choice is greater than 0, move right
				if ((teamSlot & 1) == 1 && teamSlot != GameManager.instance.GetTrainer().Team.Count && teamSlot > 0)
				{
					teamSlot++;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + teamSlot).gameObject;
				} //end else if
			} //end if 
		} //end else if Mouse Moves Right

		/*********************************************
		* Mouse Moves Up
		**********************************************/
		if (Input.GetAxis("Mouse Y") > 0)
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

			//Use Processing
			else if (checkpoint == 4 && Input.mousePosition.y > Camera.main.WorldToScreenPoint(
				currentTeamSlot.transform.position).y + currentTeamSlot.GetComponent<RectTransform>().rect.height / 2)
			{
				//Stay at top slot
				if (teamSlot == 1 || teamSlot == 2)
				{
					teamSlot = teamSlot;
				} //end if
				//Move from Cancel Button to last team slot
				else if (teamSlot == 0)
				{
					teamSlot = GameManager.instance.GetTrainer().Team.Count;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + teamSlot).gameObject;
				} //end else if
				//Go up vertically
				else
				{
					teamSlot -= 2;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + teamSlot).gameObject;
				} //end else
			} //end else if

			//Give Processing
			else if (checkpoint == 5 && Input.mousePosition.y > Camera.main.WorldToScreenPoint(
				currentTeamSlot.transform.position).y + currentTeamSlot.GetComponent<RectTransform>().rect.height / 2)
			{
				//Stay at top slot
				if (teamSlot == 1 || teamSlot == 2)
				{
					teamSlot = teamSlot;
				} //end if
				//Move from Cancel Button to last team slot
				else if (teamSlot == 0)
				{
					teamSlot = GameManager.instance.GetTrainer().Team.Count;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + teamSlot).gameObject;
				} //end else if
				//Go up vertically
				else
				{
					teamSlot -= 2;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + teamSlot).gameObject;
				} //end else
			} //end else if

			//Pick Move Processing
			else if(checkpoint == 7 && Input.mousePosition.y > selection.transform.position.y +
				selection.GetComponent<RectTransform>().rect.height / 2)
			{
				//If not on last option, decrease (higher slots are on lower children)
				if (subMenuChoice > 0)
				{
					subMenuChoice--;
				} //end if

				//Reposition selection
				selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
			} //end else if
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

			//Use Processing
			else if (checkpoint == 4 && Input.mousePosition.y < Camera.main.WorldToScreenPoint(
				currentTeamSlot.transform.position).y - currentTeamSlot.GetComponent<RectTransform>().rect.height / 2)
			{
				//Move from last or second to last slot to Cancel Button
				if ((teamSlot == GameManager.instance.GetTrainer().Team.Count-1 && teamSlot > 0) || 
					teamSlot == GameManager.instance.GetTrainer().Team.Count)
				{
					teamSlot = 0;
					currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(1).gameObject;
				} //end if
				//Stay on Cancel button
				else if (teamSlot == 0)
				{
					teamSlot = 0;
				} //end else if
				//Go down vertically
				else
				{
					teamSlot += 2;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + teamSlot).gameObject;
				} //end else
			} //end else if

			//Give Processing
			else if (checkpoint == 5 && Input.mousePosition.y < Camera.main.WorldToScreenPoint(
				currentTeamSlot.transform.position).y - currentTeamSlot.GetComponent<RectTransform>().rect.height / 2)
			{
				//Move from last or second to last slot to Cancel Button
				if ((teamSlot == GameManager.instance.GetTrainer().Team.Count-1 && teamSlot > 0) || 
					teamSlot == GameManager.instance.GetTrainer().Team.Count)
				{
					teamSlot = 0;
					currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(1).gameObject;
				} //end if
				//Stay on Cancel button
				else if (teamSlot == 0)
				{
					teamSlot = 0;
				} //end else if
				//Go down vertically
				else
				{
					teamSlot += 2;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + teamSlot).gameObject;
				} //end else
			} //end else if

			//Pick Move Processing
			else if(checkpoint == 7 && Input.mousePosition.y < selection.transform.position.y -
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

			//Item Move Processing
			else if(checkpoint == 6)
			{
				switchSpot = ExtensionMethods.BindToInt(switchSpot - 1, 0);
				if (switchSpot < topShown)
				{
					topShown = switchSpot;
					bottomShown--;
				} //end if
			} //end else if
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

			//Item Move Processing
			else if(checkpoint == 6)
			{
				switchSpot = ExtensionMethods.CapAtInt(switchSpot + 1, GameManager.instance.GetTrainer().SlotCount() - 1);
				if (switchSpot> bottomShown)
				{
					bottomShown = switchSpot;
					topShown++;
				} //end if
			} //end else if
		} //end else if Mouse Wheel Down

		/*********************************************
		* Left Mouse Button
		**********************************************/
		else if (Input.GetMouseButtonUp(0))
		{
			//Normal Processing
			if (checkpoint == 2)
			{
				//Turn on submenu if there is an item
				if (GameManager.instance.GetTrainer().SlotCount() != 0)
				{
					selection.SetActive(true);
					choices.SetActive(true);

					//Set up selection box at end of frame if it doesn't fit
					if (selection.GetComponent<RectTransform>().sizeDelta !=
					    choices.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta)
					{
						selection.SetActive(false);
						StartCoroutine(WaitForResize());
					} //end if

					checkpoint = 3;
				} //end if
			} //end if

			//Submenu Processing
			else if (checkpoint == 3)
			{
				//Get item
				int itemNumber = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];

				//Apply appropriate action
				switch (subMenuChoice)
				{
				//Use
					case 0:
						int itemType = DataContents.ExecuteSQL<int>("SELECT outsideUse FROM Items WHERE id=" + itemNumber);
						//One time use or infinite use
						if (itemType == 1 || itemType == 2 || itemType == 3)
						{
							playerTeam.SetActive(true);
							selection.SetActive(false);
							choices.SetActive(false);
							currentTeamSlot = playerTeam.transform.FindChild("Pokemon1").gameObject;
							playerTeam.transform.FindChild("PartyInstructions").GetChild(0).GetComponent<Text>().text = 
								"Use on who?";
							teamSlot = 1;
							checkpoint = 4;
						} //end if
						else
						{
							GameManager.instance.DisplayText("Can't be used outside of battle.", true);
						} //end else
						break;
				//Give
					case 1:
						if (GameManager.instance.GetTrainer().GetCurrentBagPocket() != 7)
						{
							playerTeam.SetActive(true);
							selection.SetActive(false);
							choices.SetActive(false);
							currentTeamSlot = playerTeam.transform.FindChild("Pokemon1").gameObject;
							playerTeam.transform.FindChild("PartyInstructions").GetChild(0).GetComponent<Text>().text = 
								"Give to who?";
							teamSlot = 1;
							checkpoint = 5;
						} //end if
						break;
				//Toss
					case 2:
						choices.SetActive(false);
						selection.SetActive(false);
						GameManager.instance.DisplayConfirm("Do you really want to throw out " +
						DataContents.GetItemGameName(itemNumber) + "?", 1, true);
						break;
				//Move
					case 3:
						selection.SetActive(false);
						choices.SetActive(false);
						switchSpot = inventorySpot;
						checkpoint = 6;
						break;
				//To Free Space/Pocket
					case 4:
						if (GameManager.instance.GetTrainer().GetCurrentBagPocket() == 0)
						{
							GameManager.instance.GetTrainer().MoveItemPocket(itemNumber);
						} //end if
						else
						{
							GameManager.instance.GetTrainer().MoveItemPocket(itemNumber, 0);
						} //end else
						selection.SetActive(false);
						choices.SetActive(false);
						inventorySpot = 0;
						topShown = 0;
						bottomShown = 9;
						checkpoint = 2;
						break;
				//Cancel
					case 5:
						selection.SetActive(false);
						choices.SetActive(false);
						inventorySpot = 0;
						topShown = 0;
						bottomShown = 9;
						checkpoint = 2;
						break;
				} //end switch
			} //end else if

			//Use Processing
			else if (checkpoint == 4)
			{
				int itemNumber = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];
				if (ItemEffects.UseOnPokemon(GameManager.instance.GetTrainer().Team[teamSlot - 1], itemNumber))
				{
					SetStatusIcon(playerTeam.transform.FindChild("Pokemon" + (teamSlot)).FindChild("Status").
						GetComponent<Image>(), GameManager.instance.GetTrainer().Team[teamSlot - 1]);

					if (DataContents.ExecuteSQL<int>("SELECT outsideUse FROM Items WHERE id=" + itemNumber) == 1)
					{
						GameManager.instance.GetTrainer().RemoveItem(itemNumber, 1);
					} //end if
				} //end if
				checkpoint = 1;
			} //end else if

			//Give Processing
			else if (checkpoint == 5)
			{
				//Make sure pokemon isn't holding another item
				if (GameManager.instance.GetTrainer().Team[teamSlot - 1].Item == 0)
				{
					int itemNumber = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];
					GameManager.instance.GetTrainer().RemoveItem(itemNumber, 1);
					GameManager.instance.GetTrainer().Team[teamSlot - 1].Item = itemNumber;
					GameManager.instance.DisplayText("Gave " + DataContents.GetItemGameName(itemNumber) + " to " +
					GameManager.instance.GetTrainer().Team[teamSlot - 1].Nickname, true);
					checkpoint = 1;
				} //end if
				else
				{
					int itemNumber = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];
					GameManager.instance.DisplayConfirm("Do you want to switch the held " + DataContents.GetItemGameName(
						GameManager.instance.GetTrainer().Team[teamSlot - 1].Item) + " for " + DataContents.GetItemGameName(
						itemNumber) + "?", 0, false);
				} //end else
			} //end else if

			//Item Move Processing
			else if (checkpoint == 6)
			{
				GameManager.instance.GetTrainer().MoveItemLocation(inventorySpot, switchSpot);
				inventorySpot = 0;
				topShown = 0;
				bottomShown = 9;
				checkpoint = 2;
			} //end else if

			//Pick Move Processing
			else if (checkpoint == 7)
			{
				//Get item
				int item = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];

				//Ether, Leppa Berry
				if (item == 83 || item == 148)
				{
					//Get current move pp and max
					int currentPP = GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMovePP(subMenuChoice);
					int maxPP = GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMovePPMax(subMenuChoice);
					//Check if Move PP is not full
					if (currentPP < maxPP)
					{
						int result = ExtensionMethods.CapAtInt(currentPP + 10, maxPP);
						GameManager.instance.GetTrainer().Team[teamSlot - 1].SetMovePP(subMenuChoice, 
							result);
						GameManager.instance.DisplayText(string.Format("{0} was restored by {1}!", 
							DataContents.GetMoveGameName(GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMove(subMenuChoice)),
							result), true);
						GameManager.instance.GetTrainer().RemoveItem(GameManager.instance.GetTrainer().GetItem(inventorySpot)[0], 1);
					} //end if
					else
					{
						GameManager.instance.DisplayText(string.Format("{0} is already at full PP. It won't have any effect.", 
							DataContents.GetMoveGameName(GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMove(subMenuChoice))), 
							true);
					} //end else

					//Return to inventory
					checkpoint = 1;
				} //end if

				//Max Ether
				else if (item == 169)
				{
					//Get current move pp and max
					int currentPP = GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMovePP(subMenuChoice);
					int maxPP = GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMovePPMax(subMenuChoice);
					//Check if Move PP is not full
					if (currentPP < maxPP)
					{
						int result = maxPP - currentPP;
						GameManager.instance.GetTrainer().Team[teamSlot - 1].SetMovePP(subMenuChoice, 
							maxPP);
						GameManager.instance.DisplayText(string.Format("{0} was restored by {1}!", 
							DataContents.GetMoveGameName(GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMove(subMenuChoice)),
							result), true);
						GameManager.instance.GetTrainer().RemoveItem(GameManager.instance.GetTrainer().GetItem(inventorySpot)[0], 1);
					} //end if
					else
					{
						GameManager.instance.DisplayText(string.Format("{0} is already at full PP. It won't have any effect.", 
							DataContents.GetMoveGameName(GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMove(subMenuChoice))), 
							true);
					} //end else

					//Return to inventory
					checkpoint = 1;
				} //end else if

				//PP Max
				else if (item == 215)
				{
					int PPUsed = GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMovePPUps(subMenuChoice);
					if (PPUsed < 3)
					{
						//Get base pp
						int basePP = DataContents.ExecuteSQL<int>("SELECT totalPP FROM Moves WHERE rowid=" +
						             GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMove(subMenuChoice));
						int changedPP = ((basePP * 3) / 5) + basePP;
						int currentPPUpdate = GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMovePP(subMenuChoice) +
							((basePP * (3-PPUsed))/ 5);
						GameManager.instance.GetTrainer().Team[teamSlot - 1].SetMovePP(subMenuChoice, currentPPUpdate);
						GameManager.instance.GetTrainer().Team[teamSlot - 1].SetMovePPMax(subMenuChoice, changedPP);
						GameManager.instance.GetTrainer().Team[teamSlot - 1].SetMovePPUps(subMenuChoice, 3);
						GameManager.instance.DisplayText(string.Format("{0} now has a maximum PP of {1}!", 
							DataContents.GetMoveGameName(GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMove(subMenuChoice)),
							changedPP), true);
						GameManager.instance.GetTrainer().RemoveItem(GameManager.instance.GetTrainer().GetItem(inventorySpot)[0], 1);
					} //end if
					else
					{
						GameManager.instance.DisplayText(string.Format("{0} is already at maximum total PP. It won't have any effect.", 
							DataContents.GetMoveGameName(GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMove(subMenuChoice))), 
							true);
					} //end else

					//Return to inventory
					checkpoint = 1;
				} //end else if

				//PP Up
				else if (item == 216)
				{
					int PPUsed = GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMovePPUps(subMenuChoice);
					if (PPUsed < 3)
					{
						//Get base pp
						int basePP = DataContents.ExecuteSQL<int>("SELECT totalPP FROM Moves WHERE rowid=" +
							GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMove(subMenuChoice));
						int changedPP = ((basePP * (PPUsed + 1)) / 5) + basePP;
						int currentPPUpdate = GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMovePP(subMenuChoice) +
						                      (basePP / 5);
						GameManager.instance.GetTrainer().Team[teamSlot - 1].SetMovePP(subMenuChoice, currentPPUpdate);
						GameManager.instance.GetTrainer().Team[teamSlot - 1].SetMovePPMax(subMenuChoice, changedPP);
						GameManager.instance.GetTrainer().Team[teamSlot - 1].SetMovePPUps(subMenuChoice, (PPUsed + 1));
						GameManager.instance.DisplayText(string.Format("{0} now has a maximum PP of {1}!", 
							DataContents.GetMoveGameName(GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMove(subMenuChoice)),
							changedPP), true);
						GameManager.instance.GetTrainer().RemoveItem(GameManager.instance.GetTrainer().GetItem(inventorySpot)[0], 1);
					} //end if
					else
					{
						GameManager.instance.DisplayText(string.Format("{0} is already at maximum total PP. It won't have any effect.", 
							DataContents.GetMoveGameName(GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMove(subMenuChoice))), 
							true);
					} //end else

					//Return to inventory
					checkpoint = 1;
				} //end else if

				//TMs
				else if (item > 288 && item < 395)
				{
					string[] contents = DataContents.ExecuteSQL<string>("SELECT description FROM Items WHERE rowid=" + item).Split(',');
					GameManager.instance.DisplayConfirm(string.Format("Are you sure you want to forget {0} and learn {1}?",
						DataContents.GetMoveGameName(GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMove(subMenuChoice)),
						contents[0]), 0, false);
				} //end else if

				//Anything else. This is what occurs when an item has no effect but is listed as usable
				else
				{
					GameManager.instance.DisplayText(DataContents.GetItemGameName(item) + " has no listed effect yet.", true);

					//Return to inventory
					checkpoint = 1;
				} //end else

				//Done picking a move
				selection.SetActive(false);
				choices.SetActive(false);
				pickMove = false;
			} //end else if
		} //end else if Left Mouse Button

		/*********************************************
		* Right Mouse Button
		**********************************************/
		else if (Input.GetMouseButtonUp(1))
		{
			//Normal Processing
			if (checkpoint == 2)
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

			//Team Use Processing
			else if (checkpoint == 4)
			{
				playerTeam.SetActive(false);
				checkpoint = 2;
			} //end else if

			//Team Give Processing
			else if(checkpoint == 5)
			{
				playerTeam.SetActive(false);
				checkpoint = 2;
			} //end else if

			//Item Move Processing
			else if(checkpoint == 6)
			{
				inventorySpot = 0;
				topShown = 0;
				bottomShown = 9;
				checkpoint = 2;
			} //end else if

			//Pick Move Processing
			else if (checkpoint == 7)
			{
				playerTeam.SetActive(false);
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
				//Turn on submenu if there is an item
				if (GameManager.instance.GetTrainer().SlotCount() != 0)
				{
					selection.SetActive(true);
					choices.SetActive(true);

					//Set up selection box at end of frame if it doesn't fit
					if (selection.GetComponent<RectTransform>().sizeDelta !=
					  choices.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta)
					{
						selection.SetActive(false);
						StartCoroutine(WaitForResize());
					} //end if

					checkpoint = 3;
				} //end if
			} //end if

			//Submenu Processing
			else if (checkpoint == 3)
			{
				//Get item
				int itemNumber = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];

				//Apply appropriate action
				switch (subMenuChoice)
				{
					//Use
					case 0:
						int itemType = DataContents.ExecuteSQL<int>("SELECT outsideUse FROM Items WHERE id=" + itemNumber);
						//One time use or infinite use
						if (itemType == 1 || itemType == 2 || itemType == 3)
						{
							playerTeam.SetActive(true);
							selection.SetActive(false);
							choices.SetActive(false);
							currentTeamSlot = playerTeam.transform.FindChild("Pokemon1").gameObject;
							playerTeam.transform.FindChild("PartyInstructions").GetChild(0).GetComponent<Text>().text = 
								"Use on who?";
							teamSlot = 1;
							checkpoint = 4;
						} //end if
						else
						{
							GameManager.instance.DisplayText("Can't be used outside of battle.", true);
						} //end else
						break;
					//Give
					case 1:
						if (GameManager.instance.GetTrainer().GetCurrentBagPocket() != 7)
						{
							playerTeam.SetActive(true);
							selection.SetActive(false);
							choices.SetActive(false);
							currentTeamSlot = playerTeam.transform.FindChild("Pokemon1").gameObject;
							playerTeam.transform.FindChild("PartyInstructions").GetChild(0).GetComponent<Text>().text = 
								"Give to who?";
							teamSlot = 1;
							checkpoint = 5;
						} //end if
						break;
					//Toss
					case 2:
						choices.SetActive(false);
						selection.SetActive(false);
						GameManager.instance.DisplayConfirm("Do you really want to throw out " +
						DataContents.GetItemGameName(itemNumber) + "?", 1, true);
						break;
					//Move
					case 3:
						selection.SetActive(false);
						choices.SetActive(false);
						switchSpot = inventorySpot;
						checkpoint = 6;
						break;
					//To Free Space/Pocket
					case 4:
						if (GameManager.instance.GetTrainer().GetCurrentBagPocket() == 0)
						{
							GameManager.instance.GetTrainer().MoveItemPocket(itemNumber);
						} //end if
						else
						{
							GameManager.instance.GetTrainer().MoveItemPocket(itemNumber, 0);
						} //end else
						selection.SetActive(false);
						choices.SetActive(false);
						checkpoint = 2;
						break;
					//Cancel
					case 5:
						selection.SetActive(false);
						choices.SetActive(false);
						checkpoint = 2;
						break;
				} //end switch
			} //end else if

			//Use Processing
			else if (checkpoint == 4)
			{
				int itemNumber = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];
				if (ItemEffects.UseOnPokemon(GameManager.instance.GetTrainer().Team[teamSlot - 1], itemNumber))
				{
					SetStatusIcon(playerTeam.transform.FindChild("Pokemon" + (teamSlot)).FindChild("Status").
						GetComponent<Image>(), GameManager.instance.GetTrainer().Team[teamSlot - 1]);

					if (DataContents.ExecuteSQL<int>("SELECT outsideUse FROM Items WHERE id=" + itemNumber) == 1)
					{
						GameManager.instance.GetTrainer().RemoveItem(itemNumber, 1);
					} //end if
				} //end if
				checkpoint = 1;
			} //end else if

			//Give Processing
			else if (checkpoint == 5)
			{
				//Make sure pokemon isn't holding another item
				if (GameManager.instance.GetTrainer().Team[teamSlot - 1].Item == 0)
				{
					int itemNumber = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];
					GameManager.instance.GetTrainer().RemoveItem(itemNumber, 1);
					GameManager.instance.GetTrainer().Team[teamSlot - 1].Item = itemNumber;
					GameManager.instance.DisplayText("Gave " + DataContents.GetItemGameName(itemNumber)  + " to " + 
						GameManager.instance.GetTrainer().Team[teamSlot - 1].Nickname,true);
					checkpoint = 1;
				} //end if
				else
				{
					int itemNumber = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];
					GameManager.instance.DisplayConfirm("Do you want to switch the held " + DataContents.GetItemGameName(
						GameManager.instance.GetTrainer().Team[teamSlot - 1].Item) + " for " + DataContents.GetItemGameName(
						itemNumber) + "?", 0, false);
				} //end else
			} //end else if

			//Item Move Processing
			else if(checkpoint == 6)
			{
				GameManager.instance.GetTrainer().MoveItemLocation(inventorySpot, switchSpot);
				inventorySpot = 0;
				topShown = 0;
				bottomShown = 9;
				checkpoint = 2;
			} //end else if

			//Pick Move Processing
			else if (checkpoint == 7)
			{
				//Get item
				int item = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];

				//Ether, Leppa Berry
				if (item == 83 || item == 148)
				{
					//Get current move pp and max
					int currentPP = GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMovePP(subMenuChoice);
					int maxPP = GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMovePPMax(subMenuChoice);
					//Check if Move PP is not full
					if (currentPP < maxPP)
					{
						int result = ExtensionMethods.CapAtInt(currentPP + 10, maxPP);
						GameManager.instance.GetTrainer().Team[teamSlot - 1].SetMovePP(subMenuChoice, 
							result);
						GameManager.instance.DisplayText(string.Format("{0} was restored by {1}!", 
							DataContents.GetMoveGameName(GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMove(subMenuChoice)),
							result), true);
						GameManager.instance.GetTrainer().RemoveItem(GameManager.instance.GetTrainer().GetItem(inventorySpot)[0], 1);
					} //end if
					else
					{
						GameManager.instance.DisplayText(string.Format("{0} is already at full PP. It won't have any effect.", 
							DataContents.GetMoveGameName(GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMove(subMenuChoice))), 
							true);
					} //end else

					//Return to inventory
					checkpoint = 1;
				} //end if

				//Max Ether
				else if (item == 169)
				{
					//Get current move pp and max
					int currentPP = GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMovePP(subMenuChoice);
					int maxPP = GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMovePPMax(subMenuChoice);
					//Check if Move PP is not full
					if (currentPP < maxPP)
					{
						int result = maxPP - currentPP;
						GameManager.instance.GetTrainer().Team[teamSlot - 1].SetMovePP(subMenuChoice, 
							maxPP);
						GameManager.instance.DisplayText(string.Format("{0} was restored by {1}!", 
							DataContents.GetMoveGameName(GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMove(subMenuChoice)),
							result), true);
						GameManager.instance.GetTrainer().RemoveItem(GameManager.instance.GetTrainer().GetItem(inventorySpot)[0], 1);
					} //end if
					else
					{
						GameManager.instance.DisplayText(string.Format("{0} is already at full PP. It won't have any effect.", 
							DataContents.GetMoveGameName(GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMove(subMenuChoice))), 
							true);
					} //end else

					//Return to inventory
					checkpoint = 1;
				} //end else if

				//PP Max
				else if (item == 215)
				{
					int PPUsed = GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMovePPUps(subMenuChoice);
					if (PPUsed < 3)
					{
						//Get base pp
						int basePP = DataContents.ExecuteSQL<int>("SELECT totalPP FROM Moves WHERE rowid=" +
							GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMove(subMenuChoice));
						int changedPP = ((basePP * 3) / 5) + basePP;
						int currentPPUpdate = GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMovePP(subMenuChoice) +
							((basePP * (3-PPUsed))/ 5);
						GameManager.instance.GetTrainer().Team[teamSlot - 1].SetMovePP(subMenuChoice, currentPPUpdate);
						GameManager.instance.GetTrainer().Team[teamSlot - 1].SetMovePPMax(subMenuChoice, changedPP);
						GameManager.instance.GetTrainer().Team[teamSlot - 1].SetMovePPUps(subMenuChoice, 3);
						GameManager.instance.DisplayText(string.Format("{0} now has a maximum PP of {1}!", 
							DataContents.GetMoveGameName(GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMove(subMenuChoice)),
							changedPP), true);
						GameManager.instance.GetTrainer().RemoveItem(GameManager.instance.GetTrainer().GetItem(inventorySpot)[0], 1);
					} //end if
					else
					{
						GameManager.instance.DisplayText(string.Format("{0} is already at maximum total PP. It won't have any effect.", 
							DataContents.GetMoveGameName(GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMove(subMenuChoice))), 
							true);
					} //end else

					//Return to inventory
					checkpoint = 1;
				} //end else if

				//PP Up
				else if (item == 216)
				{
					int PPUsed = GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMovePPUps(subMenuChoice);
					if (PPUsed < 3)
					{
						//Get base pp
						int basePP = DataContents.ExecuteSQL<int>("SELECT totalPP FROM Moves WHERE rowid=" +
							GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMove(subMenuChoice));
						int changedPP = ((basePP * (PPUsed + 1)) / 5) + basePP;
						int currentPPUpdate = GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMovePP(subMenuChoice) +
							(basePP / 5);
						GameManager.instance.GetTrainer().Team[teamSlot - 1].SetMovePP(subMenuChoice, currentPPUpdate);
						GameManager.instance.GetTrainer().Team[teamSlot - 1].SetMovePPMax(subMenuChoice, changedPP);
						GameManager.instance.GetTrainer().Team[teamSlot - 1].SetMovePPUps(subMenuChoice, (PPUsed + 1));
						GameManager.instance.DisplayText(string.Format("{0} now has a maximum PP of {1}!", 
							DataContents.GetMoveGameName(GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMove(subMenuChoice)),
							changedPP), true);
						GameManager.instance.GetTrainer().RemoveItem(GameManager.instance.GetTrainer().GetItem(inventorySpot)[0], 1);
					} //end if
					else
					{
						GameManager.instance.DisplayText(string.Format("{0} is already at maximum total PP. It won't have any effect.", 
							DataContents.GetMoveGameName(GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMove(subMenuChoice))), 
							true);
					} //end else

					//Return to inventory
					checkpoint = 1;
				} //end else if

				//TMs
				else if (item > 288 && item < 395)
				{
					string[] contents = DataContents.ExecuteSQL<string>("SELECT description FROM Items WHERE rowid=" + item).Split(',');
					GameManager.instance.DisplayConfirm(string.Format("Are you sure you want to forget {0} and learn {1}?",
						DataContents.GetMoveGameName(GameManager.instance.GetTrainer().Team[teamSlot - 1].GetMove(subMenuChoice)),
						contents[0]), 0, false);
				} //end else if

				//Anything else. This is what occurs when an item has no effect but is listed as usable
				else
				{
					GameManager.instance.DisplayText(DataContents.GetItemGameName(item) + " has no listed effect yet.", true);

					//Return to inventory
					checkpoint = 1;
				} //end else

				//Done picking a move
				selection.SetActive(false);
				choices.SetActive(false);
				pickMove = false;
			} //end else if
		} //end else if Enter/Return Key

		/*********************************************
		* X Key
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.X))
		{
			//Normal Processing
			if (checkpoint == 2)
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

			//Team Use Processing
			else if (checkpoint == 4)
			{
				playerTeam.SetActive(false);
				checkpoint = 2;
			} //end else if

			//Team Give Processing
			else if (checkpoint == 5)
			{
				playerTeam.SetActive(false);
				checkpoint = 2;
			} //end else if

			//Item Move Processing
			else if (checkpoint == 6)
			{
				inventorySpot = 0;
				topShown = 0;
				bottomShown = 9;
				checkpoint = 2;
			} //end else if

			//Pick Move Processing
			else if (checkpoint == 7)
			{
				playerTeam.SetActive(false);
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
		if (pickMove)
		{
			//Add to choice menu if necessary
			for (int i = choices.transform.childCount; i < 4; i++)
			{
				GameObject clone = Instantiate(choices.transform.GetChild(0).gameObject,
					choices.transform.GetChild(0).position,
					Quaternion.identity) as GameObject;
				clone.transform.SetParent(choices.transform);
			} //end for

			//Destroy extra chocies
			for (int i = choices.transform.childCount; i > 4; i--)
			{
				Destroy(choices.transform.GetChild(i - 1).gameObject);
			} //end for

			//Set text for each choice
			for (int i = 0; i < 4; i++)
			{
				choices.transform.GetChild(i).GetComponent<Text>().text = string.Format("{0} PP {1}/{2}",
					DataContents.GetMoveGameName(GameManager.instance.GetTrainer().Team[teamSlot-1].GetMove(i)),
					GameManager.instance.GetTrainer().Team[teamSlot-1].GetMovePP(i),
					GameManager.instance.GetTrainer().Team[teamSlot-1].GetMovePPMax(i));
			} //end for
		} //end if
		else if (GameManager.instance.GetTrainer().GetCurrentBagPocket() != 0)
		{
			//Add to choice menu if necessary
			for (int i = choices.transform.childCount; i < 6; i++)
			{
				GameObject clone = Instantiate(choices.transform.GetChild(0).gameObject,
					                   choices.transform.GetChild(0).position,
					                   Quaternion.identity) as GameObject;
				clone.transform.SetParent(choices.transform);
			} //end for

			//Destroy extra chocies
			for (int i = choices.transform.childCount; i > 6; i--)
			{
				Destroy(choices.transform.GetChild(i - 1).gameObject);
			} //end for

			//Set text for each choice
			choices.transform.GetChild(0).GetComponent<Text>().text = "Use";
			choices.transform.GetChild(1).GetComponent<Text>().text = "Give";
			choices.transform.GetChild(2).GetComponent<Text>().text = "Toss";
			choices.transform.GetChild(3).GetComponent<Text>().text = "Move";
			choices.transform.GetChild(4).GetComponent<Text>().text = "To Free Space";
			choices.transform.GetChild(5).GetComponent<Text>().text = "Cancel";
		} //end else if
		else
		{
			//Add to choice menu if necessary
			for (int i = choices.transform.childCount; i < 6; i++)
			{
				GameObject clone = Instantiate(choices.transform.GetChild(0).gameObject,
					choices.transform.GetChild(0).position,
					Quaternion.identity) as GameObject;
				clone.transform.SetParent(choices.transform);
			} //end for

			//Destroy extra chocies
			for (int i = choices.transform.childCount; i > 6; i--)
			{
				Destroy(choices.transform.GetChild(i - 1).gameObject);
			} //end for

			//Set text for each choice
			choices.transform.GetChild(0).GetComponent<Text>().text = "Use";
			choices.transform.GetChild(1).GetComponent<Text>().text = "Give";
			choices.transform.GetChild(2).GetComponent<Text>().text = "Toss";
			choices.transform.GetChild(3).GetComponent<Text>().text = "Move";
			choices.transform.GetChild(4).GetComponent<Text>().text = "To Regular";
			choices.transform.GetChild(5).GetComponent<Text>().text = "Cancel";
		} //end else
	} //end FillInChoices

	/***************************************
	 * Name: SetStatusIcon
	 * Sets status icon based on pokemon 
	 * status
	 ***************************************/
	void SetStatusIcon(Image statusImage, Pokemon myPokemon)
	{
		//Set status
		switch (myPokemon.Status)
		{
			//Healthy
			case 0:
				statusImage.color = Color.clear;
				break;
				//Faint
			case 1:
				statusImage.color = Color.white;
				statusImage.sprite = DataContents.statusSprites[5];
				break;
				//Sleep
			case 2:
				statusImage.color = Color.white;
				statusImage.sprite = DataContents.statusSprites[0];
				break;
				//Poison
			case 3:
				statusImage.color = Color.white;
				statusImage.sprite = DataContents.statusSprites[1];
				break;
				//Burn
			case 4:
				statusImage.color = Color.white;
				statusImage.sprite = DataContents.statusSprites[2];
				break;
				//Paralyze
			case 5:
				statusImage.color = Color.white;
				statusImage.sprite = DataContents.statusSprites[3];
				break;
				//Freeze
			case 6:
				statusImage.color = Color.white;
				statusImage.sprite = DataContents.statusSprites[4];
				break;
		} //end switch
	} //end SetStatusIcon(Image statusImage, Pokemon myPokemon)

	/***************************************
	 * Name: GetCorrectIcon
	 * Returns the icon sprite for the pokemon
	 * based on species, gender, and form
	 ***************************************/
	Sprite GetCorrectIcon(Pokemon myPokemon)
	{
		//Get requested sprite
		string chosenString = myPokemon.NatSpecies.ToString("000");
		chosenString += myPokemon.Gender == 1 ? "f" : "";
		chosenString += myPokemon.FormNumber > 0 ? "_" + myPokemon.FormNumber.ToString() : "";

		//Change sprite, and fix if sprite is null
		Sprite result = Resources.Load<Sprite>("Sprites/Icons/icon" + chosenString);
		if (result	 == null)
		{
			chosenString = chosenString.Replace("f", "");
			result = Resources.Load<Sprite>("Sprites/Icons/icon" + chosenString);

			//If still null, load generic
			if (result == null)
			{
				result = Resources.Load<Sprite>("Sprites/Icons/icon000");
			} //end if
		} //end if

		return result;
	} //end GetCorrectIcon(Pokemon myPokemon)

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

		//Reposition selection to top menu choice
		subMenuChoice = 0;
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
	 * Name: SetupPickMove
	 * Sets up screen for player to pick a
	 * move to use
	 ***************************************/
	public void SetupPickMove()
	{
		pickMove = true;
		FillInChoices();
		StartCoroutine(WaitForResize());
	} //end SetupPickMove

	/***************************************
	 * Name: ApplyConfirm
	 * Appliees the confirm choice
	 ***************************************/
	public void ApplyConfirm(ConfirmChoice e)
	{
		//Get item
		int itemNumber = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];

		//Yes selected
		if (e.Choice == 0)
		{
			//Toss from Submenu
			if (checkpoint == 3)
			{
				GameManager.instance.GetTrainer().RemoveItem(itemNumber, GameManager.instance.GetTrainer().
					ItemCount(itemNumber));
				GameManager.instance.DisplayText("Threw out " + DataContents.GetItemGameName(itemNumber), true);
				selection.SetActive(false);
				choices.SetActive(false);
				checkpoint = 2;
			} //end if

			//Give
			else if (checkpoint == 5)
			{
				GameManager.instance.GetTrainer().RemoveItem(itemNumber, 1);
				GameManager.instance.GetTrainer().AddItem(GameManager.instance.GetTrainer().Team[teamSlot - 1].
					Item, 1);
				GameManager.instance.GetTrainer().Team[teamSlot - 1].Item = itemNumber;
				GameManager.instance.DisplayText("Gave " + DataContents.GetItemGameName(itemNumber) + " to " +
				GameManager.instance.GetTrainer().Team[teamSlot - 1].Nickname + " and " +
				"put other item in bag.", true);
				checkpoint = 1;
			} //end else if

			//TM
			else if (checkpoint == 7)
			{
				//Get item
				int item = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];
				string[] contents = DataContents.ExecuteSQL<string>("SELECT description FROM Items WHERE rowid=" + item).Split(',');
				int TMMoveNumber = DataContents.ExecuteSQL<int>("SELECT rowid FROM Moves WHERE gameName='" + contents[0] + "'");
				GameManager.instance.GetTrainer().Team[teamSlot - 1].ChangeMoves(new int[]{ TMMoveNumber }, subMenuChoice);
				GameManager.instance.DisplayText(string.Format("{0} learned {1}!", GameManager.instance.GetTrainer().Team[teamSlot - 1].
					Nickname, contents[0]), true);
				checkpoint = 1;					
			} //end else if
		} //end if

		//No selected
		else if(e.Choice== 1)
		{
			//Give
			if (checkpoint == 5)
			{
				GameManager.instance.DisplayText("Did not switch items.", true);
				checkpoint = 1;
			} //end if
		} //end else			
	} //end ApplyConfirm(ConfirmChoice e)

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
