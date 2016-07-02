/***************************************************************************************** 
 * File:    MainGameScene.cs
 * Summary: Handles process for Main Game scene
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

public class MainGameScene : MonoBehaviour
{
	#region Variables
	//Main game states
	public enum MainGame
	{
		HOME, 
		GYMBATTLE,
		TEAM,
	    POKEMONSUBMENU,
	    POKEMONSUMMARY,
	    MOVESWITCH,
	    POKEMONSWITCH,
	    POKEMONRIBBONS,
		ITEMCHOICE,
		ITEMGIVE,
	    TRAINERCARD,
	    DEBUG
	} //end MainGame

	//Scene variables
	MainGame gameState;             //Current state of the main game
	int checkpoint = 0;				//Manage function progress
	int choiceNumber;				//What item the player is currently on
	int previousTeamSlot;           //The slot last highlighted
	int subMenuChoice;              //What choice is highlighted in the pokemon submenu
	int summaryChoice;              //What page is open on the summary screen
	int moveChoice;                 //What move is being highlighted for details
	int detailsSize;                //Font size for move description
	int switchChoice;               //The pokemon or move chosen to switch with the selected
	int previousSwitchSlot;         //The slot last highlighted for switching to
	int ribbonChoice;               //The ribbon currently shown
	int previousRibbonChoice;       //The ribbon last highlighted for reading
	int inventorySpot;				//What slot in pocket the player is on
	int topShown;					//The top slot displayed in the inventory
	int bottomShown;				//The bottom slot displayed in the inventory
	bool initialize;                //Initialize each state only once per access
	bool pocketChange = false;		//Whether the pocket of the player's bag is currently changing
	bool processing = false;		//Whether a function is already processing something
	GameObject choices;				//Choices box from scene tools
	GameObject selection;			//Selection rectangle from scene tools
	GameObject buttonMenu;          //Menu of buttons in main game
	GameObject gymBattle;           //Screen of region leader battles
	GameObject playerTeam;          //Screen of the player's team
	GameObject summaryScreen;       //Screen showing summary of data for pokemon
	GameObject ribbonScreen;        //Screen showing ribbons for pokemon
	GameObject trainerCard;         //Screen of the trainer card
	GameObject playerBag;			//Screen of the player's bag
	GameObject debugOptions;        //Screen of the debug options
	GameObject debugButtons;        //Screen containing debug areas
	GameObject pokemonRightRegion;  //The right region of Pokemon Edit options
	GameObject trainerRightRegion;  //The right region of the Trainer edit options
	GameObject currentTeamSlot;     //The object that is currently highlighted on the team
	GameObject currentSwitchSlot;   //The object that is currently highlighted for switching to
	GameObject currentMoveSlot;     //The object that is currently highlighted for reading/moving
	GameObject currentRibbonSlot;   //The object that is currently highlighted for reading
	GameObject viewport;			//The items shown to the player
	Image bagBack;					//Background bag image
	Image bagSprite;				//Item currently highlighted
	Text bagDescription;			//The item's description
	#endregion

	#region Methods
	/***************************************
	 * Name: RunMainGame
	 * Play the main game scene
	 ***************************************/
	public void RunMainGame()
	{
		//Set up scene
		if (checkpoint == 0)
		{
			//Return if processing
			if (processing)
			{
				return;
			} //end if

			//Begin processing
			processing = true;

			//Set checkpoint delegate
			GameManager.instance.checkDel = ChangeCheckpoint;

			//Set confirm delegate
			GameManager.instance.confirmDel = ApplyConfirm;

			//Initialize references and states
			gameState = MainGame.HOME;
			initialize = false;
			choices = GameManager.tools.transform.FindChild("ChoiceUnit").gameObject;
			selection = GameManager.tools.transform.FindChild("Selection").gameObject;
			buttonMenu = GameObject.Find("ButtonMenu");
			gymBattle = GameObject.Find("GymBattles");
			playerTeam = GameObject.Find("MyTeam");
			summaryScreen = playerTeam.transform.FindChild("Summary").gameObject;
			ribbonScreen = playerTeam.transform.FindChild("Ribbons").gameObject;
			trainerCard = GameObject.Find("PlayerCard");
			debugOptions = GameObject.Find("DebugOptions");
			debugButtons = debugOptions.transform.FindChild("Buttons").gameObject;
			pokemonRightRegion = debugOptions.transform.FindChild("Pokemon").FindChild("RightRegion").gameObject;
			trainerRightRegion = debugOptions.transform.FindChild("Trainer").FindChild("RightRegion").gameObject;
			playerBag = GameObject.Find("PlayerBag");
			viewport = playerBag.transform.FindChild("InventoryRegion").gameObject;
			bagBack = playerBag.transform.FindChild("BagBack").GetComponent<Image>();
			bagSprite = playerBag.transform.FindChild("BagSprite").GetComponent<Image>();
			bagDescription = playerBag.transform.FindChild("BagDescription").GetComponent<Text>();
			EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);

			//Enable debug button if allowed
			if (Application.isEditor || GameManager.instance.GetTrainer().DebugUnlocked)
			{
				buttonMenu.transform.FindChild("Debug").gameObject.SetActive(true);
				buttonMenu.transform.FindChild("Quit").GetComponent<Button>().navigation = Navigation.defaultNavigation;
			} //end if

			//Disable screens
			gymBattle.SetActive(false);
			playerTeam.SetActive(false);
			summaryScreen.SetActive(false);
			ribbonScreen.SetActive(false);
			trainerCard.SetActive(false);
			playerBag.SetActive(false);
			debugOptions.SetActive(false);
			debugOptions.transform.GetChild(1).gameObject.SetActive(false);
			debugOptions.transform.GetChild(2).gameObject.SetActive(false);

			//Details size has not been set yet
			detailsSize = -1;

			//Fade in
			GameManager.instance.FadeInAnimation(1);
		} //end if

		//Reset processing
		else if (checkpoint == 1)
		{
			processing = false;
			initialize = false;
			checkpoint = 2;
		} //end else if

		//Process according to game state
		else if (checkpoint == 2)
		{
			//Return if processing
			if (processing)
			{
				return;
			} //end if

			//Begin processing
			processing = true;

			//If on home screen
			if (gameState == MainGame.HOME)
			{
				buttonMenu.SetActive(true);
				gymBattle.SetActive(false);
				playerTeam.SetActive(false);
				trainerCard.SetActive(false);
				debugOptions.SetActive(false);
				choices.SetActive(false);
				selection.SetActive(false);
				initialize = false;

				//Get player input
				GetInput();
			} //end if

			//Gym battles
			else if (gameState == MainGame.GYMBATTLE)
			{
				//Initialize only once
				if (!initialize)
				{
					initialize = true;
					buttonMenu.SetActive(false);
					gymBattle.SetActive(true);
					EventSystem.current.SetSelectedGameObject(gymBattle.transform.GetChild(0).GetChild(0).
						GetChild(0).gameObject);
				} //end if

				//Get player input
				GetInput();
			} //end else if

			//Player team
			else if (gameState == MainGame.TEAM)
			{
				//Initialize only once
				if (!initialize)
				{
					initialize = true;
					buttonMenu.SetActive(false);
					playerTeam.SetActive(true);

					//Fill in choices box
					FillInChoices();

					//Disable buttons
					playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Button>().interactable = false;
					playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Button>().interactable = false;
					playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Image>().color = Color.gray;
					playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Image>().color = Color.gray;

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

					//Set choice number to 1 for first slot
					choiceNumber = 1;
					previousTeamSlot = 1;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon1").gameObject;
				} //end if !initialize

				//Change instruction text
				playerTeam.transform.FindChild("PartyInstructions").GetChild(0).GetComponent<Text>().text = 
					"These are your Pokemon.";				

				//Get player input
				GetInput();

				//Change background sprites based on player input
				if (previousTeamSlot != choiceNumber)
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
					if (choiceNumber == 1)
					{
						//Adjust if pokemon is fainted
						if (GameManager.instance.GetTrainer().Team[choiceNumber - 1].Status != 1)
						{
							playerTeam.transform.FindChild("Background").GetChild(choiceNumber - 1).GetComponent<Image>().
								sprite = Resources.Load<Sprite>("Sprites/Menus/partyPanelRoundSel");
						} //end if
						else
						{
							playerTeam.transform.FindChild("Background").GetChild(choiceNumber - 1).GetComponent<Image>().
								sprite = Resources.Load<Sprite>("Sprites/Menus/partyPanelRoundSelFnt");
						} //end else

						//Activate party ball
						playerTeam.transform.FindChild("Pokemon" + choiceNumber).FindChild("PartyBall").
						GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/partyBallSel");
					} //end if
					//PC Button selected
					else if (choiceNumber == -1)
					{
						playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Button>().interactable = true;
						playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Image>().color = Color.gray * 2;
					} //end else if
					//Cancel Button selected
					else if (choiceNumber == 0)
					{
						playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Button>().interactable = true;
						playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Image>().color = Color.gray * 2;
					} //end else if
					//Any other slot
					else
					{
						//Adjust if pokemon is fainted
						if (GameManager.instance.GetTrainer().Team[choiceNumber - 1].Status != 1)
						{
							playerTeam.transform.FindChild("Background").GetChild(choiceNumber - 1).GetComponent<Image>().
							sprite = Resources.Load<Sprite>("Sprites/Menus/partyPanelRectSel");
						} //end if
						else
						{
							playerTeam.transform.FindChild("Background").GetChild(choiceNumber - 1).GetComponent<Image>().
							sprite = Resources.Load<Sprite>("Sprites/Menus/partyPanelRectSelFnt");
						} //end else

						//Activate party ball
						playerTeam.transform.FindChild("Pokemon" + choiceNumber).FindChild("PartyBall").
						GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/partyBallSel");
					} //end else

					//Update previous slot
					previousTeamSlot = choiceNumber;
				} //end if
			} //end else if

			//Team SubMenu
			else if (gameState == MainGame.POKEMONSUBMENU)
			{
				//Initialize
				if (!initialize)
				{
					//Initalized
					initialize = true;

					//Set up choices
					StartCoroutine(PositionChoices());
				} //end if !initialze

				//Get player input
				GetInput();
			} //end else if

			//Pokemon Ribbons
			else if (gameState == MainGame.POKEMONRIBBONS)
			{
				//Initialize
				if (!initialize)
				{
					//Initialized
					initialize = true;

					//Fill in ribbon screen wtih correct data
					ribbonScreen.SetActive(true);
					ribbonScreen.transform.FindChild("Name").GetComponent<Text>().text =
						GameManager.instance.GetTrainer().Team[choiceNumber - 1].Nickname;
					ribbonScreen.transform.FindChild("Level").GetComponent<Text>().text =
						GameManager.instance.GetTrainer().Team[choiceNumber - 1].CurrentLevel.ToString();
					ribbonScreen.transform.FindChild("Ball").GetComponent<Image>().sprite =
						Resources.Load<Sprite>("Sprites/Icons/summaryBall" + GameManager.instance.GetTrainer().
							Team[choiceNumber - 1].BallUsed.ToString("00"));
					ribbonScreen.transform.FindChild("Gender").GetComponent<Image>().sprite =
						Resources.Load<Sprite>("Sprites/Icons/gender" + GameManager.instance.GetTrainer().
							Team[choiceNumber - 1].Gender.ToString());
					ribbonScreen.transform.FindChild("Sprite").GetComponent<Image>().sprite =
						Resources.Load<Sprite>("Sprites/Pokemon/" + GameManager.instance.GetTrainer().
							Team[choiceNumber - 1].NatSpecies.ToString("000"));
					ribbonScreen.transform.FindChild("Markings").GetComponent<Text>().text =
						GameManager.instance.GetTrainer().Team[choiceNumber - 1].GetMarkingsString();
					ribbonScreen.transform.FindChild("Item").GetComponent<Text>().text =
						DataContents.GetItemGameName(GameManager.instance.GetTrainer().Team[choiceNumber - 1].
							Item);
					ribbonScreen.transform.FindChild("RibbonName").gameObject.SetActive(false);
					ribbonScreen.transform.FindChild("RibbonDescription").gameObject.SetActive(false);

					//No ribbon selected yet
					previousRibbonChoice = -1;
					ribbonChoice = 0;
					selection.SetActive(false);

					//Set existing ribbons to inactive
					foreach (Transform child in ribbonScreen.transform.FindChild("RibbonRegion").transform)
					{
						child.gameObject.SetActive(false);
					} //end for each

					//Add ribbons
					for (int i = 0; i < GameManager.instance.GetTrainer().Team[choiceNumber - 1].GetRibbonCount(); i++)
					{
						//If at least one ribbon exists, resize the selection box
						if (i == 0)
						{
							StartCoroutine(WaitForResize());
						} //end if

						//The ribbon alrady exists, just fill it in
						if (i < ribbonScreen.transform.FindChild("RibbonRegion").childCount)
						{
							GameObject newRibbon = ribbonScreen.transform.FindChild("RibbonRegion").GetChild(i).gameObject;
							newRibbon.gameObject.SetActive(true);
							newRibbon.GetComponent<Image>().sprite = DataContents.ribbonSprites[
								GameManager.instance.GetTrainer().Team[choiceNumber - 1].GetRibbon(i)];
						} //end if
						//Create new ribbon
						else
						{
							GameObject newRibbon = Instantiate(ribbonScreen.transform.FindChild("RibbonRegion").
								GetChild(0).gameObject);
							newRibbon.transform.SetParent(ribbonScreen.transform.FindChild("RibbonRegion"));
							newRibbon.GetComponent<Image>().sprite = DataContents.ribbonSprites[
								GameManager.instance.GetTrainer().Team[choiceNumber - 1].GetRibbon(i)];
							newRibbon.GetComponent<RectTransform>().localScale = Vector3.one;
							newRibbon.GetComponent<RectTransform>().localPosition = Vector3.zero;
							newRibbon.SetActive(true);
						} //end else
					} //end for
				} //end if !initialized

				//Get player input
				GetInput();			
			} //end else if

			//Pokemon summary
			else if (gameState == MainGame.POKEMONSUMMARY)
			{
				//Get player input
				GetInput();

				//Fill in the summary screen with the correct data
				PokemonSummary(GameManager.instance.GetTrainer().Team[choiceNumber - 1]);
			} //end else if

			//Pokemon Switch
			else if (gameState == MainGame.POKEMONSWITCH)
			{
				//Change instruction text
				playerTeam.transform.FindChild("PartyInstructions").GetChild(0).GetComponent<Text>().text =
					"Switch to where?";

				//Get player input
				GetInput();

				//Cange background sprites based on player input
				if (previousSwitchSlot != switchChoice)
				{
					//Deactivate panel
					if (previousSwitchSlot != choiceNumber)
					{
						//If on first slot
						if (previousSwitchSlot == 1)
						{
							//Adjust if pokemon is fainted
							if (GameManager.instance.GetTrainer().Team[previousSwitchSlot - 1].Status != 1)
							{
								playerTeam.transform.FindChild("Background").GetChild(previousSwitchSlot - 1).GetComponent<Image>().
								sprite = Resources.Load<Sprite>("Sprites/Menus/partypanelRound");
							} //end if
							else
							{
								playerTeam.transform.FindChild("Background").GetChild(previousSwitchSlot - 1).GetComponent<Image>().
								sprite = Resources.Load<Sprite>("Sprites/Menus/partypanelRoundFnt");
							} //end else
						} //end if
						//Else if previous slot is not a button
						else if (previousSwitchSlot > 0)
						{
							//Adjust if pokemon is fainted
							if (GameManager.instance.GetTrainer().Team[previousSwitchSlot - 1].Status != 1)
							{
								playerTeam.transform.FindChild("Background").GetChild(previousSwitchSlot - 1).GetComponent<Image>().
								sprite = Resources.Load<Sprite>("Sprites/Menus/partypanelRect");
							} //end if
							else
							{
								playerTeam.transform.FindChild("Background").GetChild(previousSwitchSlot - 1).GetComponent<Image>().
								sprite = Resources.Load<Sprite>("Sprites/Menus/partypanelRectFnt");
							} //end else
						} //end else if

						//Deactivate party ball
						playerTeam.transform.FindChild("Pokemon" + previousSwitchSlot).FindChild("PartyBall").
						GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/partyBall");	
					} //end if

					//Activate panel
					if (switchChoice != choiceNumber)
					{
						//If on first slot
						if (switchChoice == 1)
						{
							//Adjust if pokemon is fainted
							if (GameManager.instance.GetTrainer().Team[switchChoice - 1].Status != 1)
							{
								playerTeam.transform.FindChild("Background").GetChild(switchChoice - 1).GetComponent<Image>().
								sprite = Resources.Load<Sprite>("Sprites/Menus/partypanelRoundSel");
							} //end if
							else
							{
								playerTeam.transform.FindChild("Background").GetChild(switchChoice - 1).GetComponent<Image>().
								sprite = Resources.Load<Sprite>("Sprites/Menus/partypanelRoundSelFnt");
							} //end else
						} //end if
						//Else if current slot is not a button
						else if (switchChoice > 0)
						{
							//Adjust if pokemon is fainted
							if (GameManager.instance.GetTrainer().Team[switchChoice - 1].Status != 1)
							{
								playerTeam.transform.FindChild("Background").GetChild(switchChoice - 1).GetComponent<Image>().
								sprite = Resources.Load<Sprite>("Sprites/Menus/partypanelRectSel");
							} //end if
							else
							{
								playerTeam.transform.FindChild("Background").GetChild(switchChoice - 1).GetComponent<Image>().
								sprite = Resources.Load<Sprite>("Sprites/Menus/partypanelRectSelFnt");
							} //end else
						} //end else if

						//Deactivate party ball
						playerTeam.transform.FindChild("Pokemon" + switchChoice).FindChild("PartyBall").
						GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/partyBallSel");	
					} //end if

					//Previous slot deactived, set to equal
					previousSwitchSlot = switchChoice;
				} //end if
			} //end else if

			//Pokemon move switching
			else if (gameState == MainGame.MOVESWITCH)
			{
				//Get player input
				GetInput();

				//Highlight selected switch to
				selection.SetActive(true);

				//Resize to same as top choice
				Transform moveScreen = summaryScreen.transform.GetChild(5);
				Vector3 scale = new Vector3(moveScreen.FindChild("Move" + (moveChoice + 1)).GetComponent<RectTransform>().rect.width, 
					                moveScreen.FindChild("Move" + (moveChoice + 1)).GetComponent<RectTransform>().rect.height, 0);
				selection.GetComponent<RectTransform>().sizeDelta = scale;

				//Reposition to location of top choice, with 2 unit offset to center it
				selection.transform.position = Camera.main.WorldToScreenPoint(currentSwitchSlot.transform.position);
			} //end else if

			//Pokemon Item Give/Take
			else if (gameState == MainGame.ITEMCHOICE)
			{
				//Initialize
				if (!initialize)
				{
					//Initialized
					initialize = true;

					//Set up choices
					StartCoroutine(PositionChoices());
				} //end if !initialize

				//Get player input
				GetInput();
			} //end else if

			//Pokemon Item Give From Bag
			else if (gameState == MainGame.ITEMGIVE)
			{
				//Initialize
				if (!initialize)
				{
					//Initialized
					initialize = true;

					playerTeam.transform.FindChild("PartyInstructions").GetChild(0).GetComponent<Text>().text = 
						"These are your pokemon.";
					int currentPocket = GameManager.instance.GetTrainer().GetCurrentBagPocket();
					bagBack.sprite = Resources.Load<Sprite>("Sprites/Menus/bag" + currentPocket);
					inventorySpot = 0;
					topShown = 0;
					bottomShown = 9;
				} //end if !initialize

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
					bagSprite.color = Color.white;
					bagSprite.sprite = Resources.Load<Sprite>("Sprites/Icons/item" + item[0].ToString("000"));
					bagDescription.text = DataContents.ExecuteSQL<string>("SELECT description FROM Items WHERE rowid=" + item[0]);
				} //end if
				else
				{
					bagSprite.color = Color.clear;
					bagDescription.text = "";
				} //end else
			} //end else if

			//Player's trainer card
			else if (gameState == MainGame.TRAINERCARD)
			{
				//Initialize only once
				if (!initialize)
				{
					//Initialized
					initialize = true;

					//Activate trainer card screen
					buttonMenu.SetActive(false);
					trainerCard.SetActive(true);

					//Fill in card data
					trainerCard.transform.FindChild("Name").GetComponent<Text>().text = "Name: " +
					GameManager.instance.GetTrainer().PlayerName;
					trainerCard.transform.FindChild("Points").GetComponent<Text>().text = "Points: " +
					GameManager.instance.GetTrainer().PlayerPoints;
					trainerCard.transform.FindChild("Playtime").GetComponent<Text>().text =
						"Playtime: " + GameManager.instance.GetTrainer().HoursPlayed.ToString("00") + ":" +
						GameManager.instance.GetTrainer().MinutesPlayed.ToString("00"); 
					trainerCard.transform.FindChild("Pokedex").GetComponent<Text>().text = "Pokedex: " +
					GameManager.instance.GetTrainer().Owned.Count + "/" + GameManager.instance.GetTrainer().Seen.Count;
					trainerCard.transform.FindChild("IDNumber").GetComponent<Text>().text = "ID: " +
					GameManager.instance.GetTrainer().PlayerID;
					trainerCard.transform.FindChild("Trainer").GetComponent<Image>().sprite = DataContents.trainerCardSprites[
						GameManager.instance.GetTrainer().PlayerImage];
					for (int i = 0; i < 8; i++)
					{
						trainerCard.transform.FindChild("Badges").FindChild("Kalos").GetChild(i).GetComponent<Image>().color =
							GameManager.instance.GetTrainer().GetPlayerBadges(39 + i) ? Color.white : Color.clear;
					} //end for
				} //end if !initialize

				//Get player input
				GetInput();
			} //end else if

			//Debug options
			else if (gameState == MainGame.DEBUG)
			{
				//Initialize only once
				if (!initialize)
				{
					//Initialized
					initialize = true;

					//Activate debug options
					buttonMenu.SetActive(false);
					debugOptions.SetActive(true);
					debugButtons.SetActive(true);
					debugOptions.transform.GetChild(1).gameObject.SetActive(false);
					debugOptions.transform.GetChild(2).gameObject.SetActive(false);

					//Fill in pokemon name dropdown list
					pokemonRightRegion.transform.FindChild("PokemonName").GetComponent<Dropdown>().ClearOptions();
					pokemonRightRegion.transform.FindChild("PokemonName").GetComponent<Dropdown>().
						AddOptions(DataContents.GeneratePokemonList());
					pokemonRightRegion.transform.FindChild("PokemonName").GetComponent<Dropdown>().RefreshShownValue();

					//Fill in ability dropdown list
					pokemonRightRegion.transform.FindChild("Ability").GetComponent<Dropdown>().ClearOptions();
					pokemonRightRegion.transform.FindChild("Ability").GetComponent<Dropdown>().
						AddOptions(DataContents.GenerateAbilityList());
					pokemonRightRegion.transform.FindChild("Ability").GetComponent<Dropdown>().RefreshShownValue();

					//Fill in item dropdown list
					pokemonRightRegion.transform.FindChild("Item").GetComponent<Dropdown>().ClearOptions();
					pokemonRightRegion.transform.FindChild("Item").GetComponent<Dropdown>().
					AddOptions(DataContents.GenerateItemList());
					pokemonRightRegion.transform.FindChild("Item").GetComponent<Dropdown>().RefreshShownValue();

					//Fill in nature dropdown list
					pokemonRightRegion.transform.FindChild("Nature").GetComponent<Dropdown>().ClearOptions();
					List<string> natList = new List<string>();
					for (Natures nat = Natures.HARDY; nat < Natures.COUNT; nat++)
					{
						natList.Add(nat.ToString());
					} //end for
					pokemonRightRegion.transform.FindChild("Nature").GetComponent<Dropdown>().AddOptions(natList);
					pokemonRightRegion.transform.FindChild("Nature").GetComponent<Dropdown>().RefreshShownValue();

					//Fill in move dropdown lists
					for (int i = 0; i < 4; i++)
					{
						pokemonRightRegion.transform.FindChild("Moves").GetChild(i).GetComponent<Dropdown>().
						ClearOptions();
						pokemonRightRegion.transform.FindChild("Moves").GetChild(i).GetComponent<Dropdown>().
							AddOptions(DataContents.GenerateMoveList());
						pokemonRightRegion.transform.FindChild("Moves").GetChild(i).GetComponent<Dropdown>().
							RefreshShownValue();
					} //end for

					//Fill in player sprite
					trainerRightRegion.transform.FindChild("TrainerSprite").GetComponent<Dropdown>().value =
						GameManager.instance.GetTrainer().PlayerImage;
					trainerRightRegion.transform.FindChild("TrainerSprite").GetComponent<Dropdown>().RefreshShownValue();

					//Fill in player badges
					for (int i = 0; i < 8; i++)
					{
						trainerRightRegion.transform.FindChild("Badges").GetChild(i).GetComponent<Toggle>().isOn =
							GameManager.instance.GetTrainer().GetPlayerBadges(39 + i);
					} //end for
				} //end if !initialize

				//Get player input
				GetInput();
			} //end else if

			//End processing
			processing = false;
		} //end else if
	} //end RunMainGame

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
			//Pokemon Summary on Continue Game -> Team -> Summary
			if (gameState == MainGame.POKEMONSUMMARY)
			{
				//If on any page besides move details
				if (summaryChoice != 5)
				{
					//Deactivate current page
					summaryScreen.transform.GetChild(summaryChoice).gameObject.SetActive(false);

					//Decrease choice
					summaryChoice--;

					//Loop to last page if on first page
					if (summaryChoice < 0)
					{
						summaryChoice = 4;
					} //end if
				} //end if
			} //end if Pokemon Summary on Continue Game -> Team -> Summary

			//Player Team
			else if (gameState == MainGame.TEAM)
			{
				//Decrease (higher slots are on lower children)
				choiceNumber--;

				//Loop to end of team if on PC button
				if (choiceNumber < -1)
				{
					choiceNumber = GameManager.instance.GetTrainer().Team.Count;
				} //end if

				//Set current slot choice if on pokemon slot
				if (choiceNumber > 0)
				{
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
				} //end if
				//If on Cancel Button
				else if (choiceNumber == 0)
				{
					currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(1).gameObject;
				} //end else if
				//If on PC button
				else if (choiceNumber == -1)
				{
					currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(0).gameObject;
				} //end else if
			} //end else if Player Team

			//Pokemon Switch on Continue Game -> Team -> Switch
			else if (gameState == MainGame.POKEMONSWITCH)
			{
				//Decrease (higher slots are on lower children)
				switchChoice--;

				//Loop to end of team if on first member
				if (switchChoice < 1)
				{
					switchChoice = GameManager.instance.GetTrainer().Team.Count;
				} //end if

				//Set current switch slot
				currentSwitchSlot = playerTeam.transform.FindChild("Pokemon" + switchChoice).gameObject;
			} //end else if Pokemon Switch on Continue Game -> Team -> Switch

			//Pokemon Ribbons on Continue Game -> Team -> Ribbons
			else if (gameState == MainGame.POKEMONRIBBONS)
			{
				//Decrease (higher slots are on lower children)
				ribbonChoice--;

				//Clamp at 0
				if (ribbonChoice < 0)
				{
					ribbonChoice = 0;
					previousRibbonChoice = -1;
				} //end if

				//Set current ribbon slot
				currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").GetChild(ribbonChoice).gameObject;

				//Read ribbon
				ReadRibbon();
			} //end else if Pokemon Ribbons on Continue Game -> Team -> Ribbons

			//Pokemon Item Give From Bag on Continue Game -> Team -> Inventory
			else if (gameState == MainGame.ITEMGIVE)
			{
				GameManager.instance.GetTrainer().PreviousPocket();
				initialize = false;
			} //end else if Pokemon Item Give From Bag on Continue Game -> Team -> Inventory
		} //end if Left Arrow

		/*********************************************
		* Right Arrow
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			//Pokemon Summary on Continue Game -> Team -> Summary
			if (gameState == MainGame.POKEMONSUMMARY)
			{
				//If on any page besides move details
				if (summaryChoice != 5)
				{
					//Deactivate current page
					summaryScreen.transform.GetChild(summaryChoice).gameObject.SetActive(false);

					//Increase choice
					summaryChoice++;

					//Loop to first page if on last page
					if (summaryChoice > 4)
					{
						summaryChoice = 0;
					} //end if
				} //end if
			} //end if Pokemon Summary on Continue Game -> Team -> Summary

			//Player Team
			else if (gameState == MainGame.TEAM)
			{
				//Increase (lower slots are on higher children)
				choiceNumber++;

				//Loop to PC button if at end of team
				if (choiceNumber > GameManager.instance.GetTrainer().Team.Count)
				{
					choiceNumber = -1;
				} //end if

				//Set current slot choice if on pokemon slot
				if (choiceNumber > 0)
				{
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
				} //end if
				//If on Cancel Button
				else if (choiceNumber == 0)
				{
					currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(1).gameObject;
				} //end else if
				//If on PC button
				else if (choiceNumber == -1)
				{
					currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(0).gameObject;
				} //end else if
			} //end else if Player Team

			//Pokemon Switch on Continue Game -> Team -> Switch
			else if (gameState == MainGame.POKEMONSWITCH)
			{
				//Increase (lower slots are on higher children)
				switchChoice++;

				//Loop to first member of team if at end of team
				if (switchChoice > GameManager.instance.GetTrainer().Team.Count)
				{
					switchChoice = 1;
				} //end if

				//Set current switch slot
				currentSwitchSlot = playerTeam.transform.FindChild("Pokemon" + switchChoice).gameObject;
			} //end else if Pokemon Switch on Continue Game -> Team -> Switch

			//Pokemon Ribbons on Continue Game -> Team -> Ribbons
			else if (gameState == MainGame.POKEMONRIBBONS)
			{
				//Increase (lower slots are on higher children)
				ribbonChoice++;

				//Clamp at ribbonLength
				if (ribbonChoice < GameManager.instance.GetTrainer().Team[choiceNumber-1].GetRibbonCount())
				{
					ribbonChoice = ExtensionMethods.BindToInt(GameManager.instance.GetTrainer().Team[choiceNumber-1].
						GetRibbonCount()-1, 0);
					previousRibbonChoice = -1;
				} //end if

				//Set current ribbon slot
				currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").GetChild(ribbonChoice).gameObject;

				//Read ribbon
				ReadRibbon();
			} //end else if Pokemon Ribbons on Continue Game -> Team -> Ribbons

			//Pokemon Item Give From Bag on Continue Game -> Team -> Inventory
			else if (gameState == MainGame.ITEMGIVE)
			{
				GameManager.instance.GetTrainer().NextPocket();
				initialize = false;
			} //end else if Pokemon Item Give From Bag on Continue Game -> Team -> Inventory
		} //end else if Right Arrow

		/*********************************************
		* Up Arrow
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			//Pokemon Submenu on Continue Game -> Team -> Submenu
			if (gameState == MainGame.POKEMONSUBMENU)
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
			} //end if Pokemon Submenu on Continue Game -> Team -> Submenu

			//Player team
			else if (gameState == MainGame.TEAM)
			{
				//Move from top slot to Canel button
				if (choiceNumber == 1 || choiceNumber == 2)
				{
					choiceNumber = 0;
					currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(1).gameObject;
				} //end if
				//Move from PC Button to last team slot
				else if (choiceNumber == -1)
				{
					choiceNumber = GameManager.instance.GetTrainer().Team.Count;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
				} //end else if
				//Move from Cancel button to PC button
				else if (choiceNumber == 0)
				{
					choiceNumber = -1;
					currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(0).gameObject;
				} //end else if
				//Go up vertically
				else
				{
					choiceNumber -= 2;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
				} //end else
			} //end else if Player team

			//Pokemon Summary on Continue Game -> Team -> Summary
			else if (gameState == MainGame.POKEMONSUMMARY)
			{
				//If on any page besides details
				if (summaryChoice != 5)
				{
					//Decrease (Higher slots are on lower children)
					choiceNumber--;

					//Loop to end if on first member
					if (choiceNumber < 1)
					{
						choiceNumber = GameManager.instance.GetTrainer().Team.Count;
					} //end if

					//Update current team slot
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
				} //end else if
				else
				{
					//Decrease (higher slots are on lower children)
					moveChoice--;

					//Loop to last move if on first move
					if (moveChoice < 0)
					{
						moveChoice = GameManager.instance.GetTrainer().Team[choiceNumber - 1].GetMoveCount() - 1;
					} //end if

					//Set move slot
					currentMoveSlot = summaryScreen.transform.GetChild(5).FindChild("Move" + (moveChoice + 1)).gameObject;
				} //end else
			} //end else if Pokemon Summary on Continue Game -> Team -> Summary

			//Pokemon Switch on Continue Game -> Team -> Switch
			else if (gameState == MainGame.POKEMONSWITCH)
			{
				//Move from top slot to last slot
				if (switchChoice < 3)
				{
					switchChoice = GameManager.instance.GetTrainer().Team.Count;
				} //end if
				//Otherwise go up vertically
				else
				{
					switchChoice -= 2;
				} //end else

				//Set current switch choice
				currentSwitchSlot = playerTeam.transform.FindChild("Pokemon" + switchChoice).gameObject;
			} //end else if Pokemon Switch on Continue Game -> Team -> Switch

			//Move switch on Continue Game -> Team -> Summary -> Move Details
			else if (gameState == MainGame.MOVESWITCH)
			{
				//Decrease (higher slots are on lower children)
				switchChoice--;

				//Loop to end if on first move
				if (switchChoice < 0)
				{
					switchChoice = GameManager.instance.GetTrainer().Team[choiceNumber - 1].GetMoveCount() - 1;
				} //end if

				//Set current switch slot
				currentSwitchSlot = summaryScreen.transform.GetChild(5).FindChild("Move" + (switchChoice + 1)).gameObject;
			} //end else if Move switch on Continue Game -> Team -> Summary -> Move Details

			//Pokemon Ribbons on Continue Game -> Team -> Ribbons
			else if (gameState == MainGame.POKEMONRIBBONS)
			{
				//Decrease (higher slots are on lower children)
				choiceNumber--;

				//Loop to end if on first member
				if (choiceNumber < 1)
				{
					choiceNumber = GameManager.instance.GetTrainer().Team.Count;
				} //end if

				//Update current team slot
				currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;

				//Reload ribbons
				initialize = false;
			} //end else if Pokemon Ribbons on Continue Game -> Team -> Ribbons

			//Pokemon Take/Give on Continue Game -> Team -> Item
			else if (gameState == MainGame.ITEMCHOICE)
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
			} //end else if Pokemon Take/Give on Continue Game -> Team -> Item

			//Pokemon Item Give From Bag on Continue Game -> Team -> Inventory
			else if (gameState == MainGame.ITEMGIVE)
			{
				inventorySpot = ExtensionMethods.BindToInt(inventorySpot - 1, 0);
				if (inventorySpot < topShown)
				{
					topShown = inventorySpot;
					bottomShown--;
				} //end if
			} //end else if Pokemon Item Give From Bag on Continue Game -> Team -> Inventory
		} //end else if Up Arrow

		/*********************************************
		* Down Arrow
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			//Pokemon Submenu on Continue Game -> Team -> Submenu
			if (gameState == MainGame.POKEMONSUBMENU)
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
			} //end if Pokemon Submenu on Continue Game -> Team -> Submenu

			//Player team
			else if (gameState == MainGame.TEAM)
			{
				//Move from last or second to last slot to PC Button
				if ((choiceNumber == GameManager.instance.GetTrainer().Team.Count-1 && choiceNumber > 0) || 
					choiceNumber == GameManager.instance.GetTrainer().Team.Count)
				{
					choiceNumber = -1;
					currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(0).gameObject;
				} //end if
				//Move from PC Button to Cancel Button
				else if (choiceNumber == -1)
				{
					choiceNumber = 0;
					currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(1).gameObject;
				} //end else if
				//Move from Cancel button to first team slot
				else if (choiceNumber == 0)
				{
					choiceNumber = 1;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon1").gameObject;
				} //end else if
				//Go down vertically
				else
				{
					choiceNumber += 2;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
				} //end else
			} //end else if Player team

			//Pokemon Summary on Continue Game -> Team -> Summary
			else if (gameState == MainGame.POKEMONSUMMARY)
			{
				//If on any page besides details
				if (summaryChoice != 5)
				{
					//Increase (lower slots are on higher children)
					choiceNumber++;

					//Loop to front if on last member
					if (choiceNumber > GameManager.instance.GetTrainer().Team.Count)
					{
						choiceNumber = 1;
					} //end if

					//Update current team slot
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
				} //end else if
				else
				{
					//Increase (lower slots are on higher children)
					moveChoice++;

					//If on last move, loop to front
					if (moveChoice >= GameManager.instance.GetTrainer().Team[choiceNumber - 1].GetMoveCount())
					{
						moveChoice = 0;
					} //end if

					//Set move slot
					currentMoveSlot = summaryScreen.transform.GetChild(5).FindChild("Move" + (moveChoice + 1)).gameObject;
				} //end else
			} //end else if Pokemon Summary on Continue Game -> Team -> Summary

			//Pokemon Switch on Continue Game -> Team -> Switch
			else if (gameState == MainGame.POKEMONSWITCH)
			{
				//Move from last slot to top slot
				if (switchChoice >= GameManager.instance.GetTrainer().Team.Count - 1)
				{
					switchChoice = 1;
				} //end if
				//Otherwise go down vertically
				else
				{
					switchChoice += 2;
				} //end else

				//Set current switch choice
				currentSwitchSlot = playerTeam.transform.FindChild("Pokemon" + switchChoice).gameObject;
			} //end else if Pokemon Switch on Continue Game -> Team -> Switch

			//Move switch on Continue Game -> Team -> Summary -> Move Details
			else if (gameState == MainGame.MOVESWITCH)
			{
				//Increase (lower slots are on higher children)
				switchChoice++;

				//Loop to end if on first move
				if (switchChoice >= GameManager.instance.GetTrainer().Team[choiceNumber - 1].GetMoveCount())
				{
					switchChoice = 0;
				} //end if

				//Set current switch slot
				currentSwitchSlot = summaryScreen.transform.GetChild(5).FindChild("Move" + (switchChoice + 1)).gameObject;
			} //end else if Move switch on Continue Game -> Team -> Summary -> Move Details

			//Pokemon Ribbons on Continue Game -> Team -> Ribbons
			else if (gameState == MainGame.POKEMONRIBBONS)
			{
				//Increase (lower slots are on higher children)
				choiceNumber++;

				//Loop to front if on last member
				if (choiceNumber > GameManager.instance.GetTrainer().Team.Count)
				{
					choiceNumber = 1;
				} //end if

				//Update current team slot
				currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;

				//Reload ribbons
				initialize = false;
			} //end else if Pokemon Ribbons on Continue Game -> Team -> Ribbons

			//Pokemon Take/Give on Continue Game -> Team -> Item
			else if (gameState == MainGame.ITEMCHOICE)
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
			} //end else if Pokemon Take/Give on Continue Game -> Team -> Item

			//Pokemon Item Give From Bag on Continue Game -> Team -> Inventory
			else if (gameState == MainGame.ITEMGIVE)
			{
				inventorySpot = ExtensionMethods.CapAtInt(inventorySpot + 1, GameManager.instance.GetTrainer().SlotCount() - 1);
				if (inventorySpot > bottomShown)
				{
					bottomShown = inventorySpot;
					topShown++;
				} //end if
			} //end else if Pokemon Item Give From Bag on Continue Game -> Team -> Inventory
		} //end else if Down Arrow

		/*********************************************
		* Mouse Moves Left
		**********************************************/
		if (Input.GetAxis("Mouse X") < 0)
		{
			//Player team
			if (gameState == MainGame.TEAM && Input.mousePosition.x < Camera.main.WorldToScreenPoint(
				    currentTeamSlot.transform.position).x - currentTeamSlot.GetComponent<RectTransform>().rect.width / 2)
			{
				//If choice number is not odd, and is greater than 0, move left
				if ((choiceNumber & 1) != 1 && choiceNumber > 0)
				{
					choiceNumber--;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
				} //end if
			} //end if Player team

			//Pokemon Switch on Continue Game -> Team -> Switch
			else if (gameState == MainGame.POKEMONSWITCH && Input.mousePosition.x < Camera.main.WorldToScreenPoint(
				         currentSwitchSlot.transform.position).x - currentSwitchSlot.GetComponent<RectTransform>().rect.width / 2)
			{
				//If switch number is not odd, move left
				if ((switchChoice & 1) != 1)
				{
					//Decrease (higher slots are on lower children)
					switchChoice--;

					//Set cuurentSwitchSlot
					currentSwitchSlot = playerTeam.transform.FindChild("Pokemon" + switchChoice).gameObject;
				} //end if
			} //end else if Pokemon Switch on Continue Game -> Team -> Switch

			//Pokemon Ribbon on Continue Game -> Team -> Ribbons
			else if (gameState == MainGame.POKEMONRIBBONS && Input.mousePosition.x < Camera.main.WorldToScreenPoint(
				         currentRibbonSlot.transform.position).x - currentRibbonSlot.GetComponent<RectTransform>().rect.width / 2)
			{
				//If next slot is null, don't move
				if (ribbonChoice - 1 > -1 && ribbonChoice % 4 != 0)
				{
					//Decrease (higher slots are on lower children)
					ribbonChoice--;

					//Read ribbon
					ReadRibbon();
				} //end if

				//Set currentRibbonSlot
				currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").GetChild(ribbonChoice).gameObject;
			} //end else if Pokemon Ribbon on Continue Game -> Team -> Ribbons
		} //end else if Mouse Moves Left

		/*********************************************
		* Mouse Moves Right
		**********************************************/
		if (Input.GetAxis("Mouse X") > 0)
		{
			//Player team
			if (gameState == MainGame.TEAM && Input.mousePosition.x > Camera.main.WorldToScreenPoint(
				currentTeamSlot.transform.position).x + currentTeamSlot.GetComponent<RectTransform>().rect.width / 2)
			{
				//If choice number is odd, and team is not odd numbered, and choice is greater than 0, move right
				if ((choiceNumber & 1) == 1 && choiceNumber != GameManager.instance.GetTrainer().Team.Count && choiceNumber > 0)
				{
					choiceNumber++;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
				} //end if
			} //end if Player team

			//Pokemon Switch on Continue Game -> Team -> Switch
			else if (gameState == MainGame.POKEMONSWITCH && Input.mousePosition.x > Camera.main.WorldToScreenPoint(
				currentSwitchSlot.transform.position).x + currentSwitchSlot.GetComponent<RectTransform>().rect.width / 2)
			{
				//If switch number is odd, and team is not odd numbered, move right
				if ((switchChoice&1) == 1 && switchChoice != GameManager.instance.GetTrainer().Team.Count)
				{
					//Increase (lower slots are on higher children)
					switchChoice++;

					//Set cuurentSwitchSlot
					currentSwitchSlot = playerTeam.transform.FindChild("Pokemon" + switchChoice).gameObject;
				} //end if
			} //end else if Pokemon Switch on Continue Game -> Team -> Switch

			//Pokemon Ribbon on Continue Game -> Team -> Ribbons
			else if (gameState == MainGame.POKEMONRIBBONS && Input.mousePosition.x > Camera.main.WorldToScreenPoint(
				currentRibbonSlot.transform.position).x + currentRibbonSlot.GetComponent<RectTransform>().rect.width / 2)
			{
				//If next slot is null, don't move
				if (ribbonChoice + 1 < GameManager.instance.GetTrainer().Team[choiceNumber-1].GetRibbonCount() && 
					ribbonChoice % 4 != 3)
				{
					//Increase (lower slots are on higher children)
					ribbonChoice++;

					//Read ribbon
					ReadRibbon();
				} //end if

				//Set currentRibbonSlot
				currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").GetChild(ribbonChoice).gameObject;
			} //end else if Pokemon Ribbon on Continue Game -> Team -> Ribbons
		} //end else if Mouse Moves Right

		/*********************************************
		* Mouse Moves Up
		**********************************************/
		if (Input.GetAxis("Mouse Y") > 0)
		{
			//Pokemon submenu on Continue Game -> Team -> Submenu
			if (gameState == MainGame.POKEMONSUBMENU && Input.mousePosition.y > selection.transform.position.y +
				selection.GetComponent<RectTransform>().rect.height / 2)
			{
				//If not on last option, decrease (higher slots are on lower children)
				if (subMenuChoice > 0)
				{
					subMenuChoice--;
				} //end if

				//Reposition selection
				selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
			} //end if Pokemon submenu on Continue Game -> Team -> Submenu

			//Player team
			else if (gameState == MainGame.TEAM && Input.mousePosition.y > Camera.main.WorldToScreenPoint(
				currentTeamSlot.transform.position).y + currentTeamSlot.GetComponent<RectTransform>().rect.height / 2)
			{
				//Stay put if on top slot
				if (choiceNumber == 1 || choiceNumber == 2)
				{
					//Do nothing
				} //end if
				//Move from PC Button to last team slot
				else if (choiceNumber == -1)
				{
					choiceNumber = GameManager.instance.GetTrainer().Team.Count;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
				} //end else if
				//Move from Cancel button to PC button
				else if (choiceNumber == 0)
				{
					choiceNumber = -1;
					currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(0).gameObject;
				} //end else if
				//Go up vertically
				else
				{
					choiceNumber -= 2;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
				} //end else
			} //end else if Player team

			//Pokemon switch on Continue Game -> Team -> Switch
			else if (gameState == MainGame.POKEMONSWITCH && Input.mousePosition.y > Camera.main.WorldToScreenPoint(
				currentSwitchSlot.transform.position).y + currentSwitchSlot.GetComponent<RectTransform>().rect.height / 2)
			{
				//Move up as long as not on first or second slot
				if (switchChoice > 2)
				{
					switchChoice -= 2;
					currentSwitchSlot = playerTeam.transform.FindChild("Pokemon" + switchChoice).gameObject;
				} //end if
			} //end else if Pokemon switch on Continue Game -> Team -> Switch

			//Pokemon Summary on Continue Game -> Team -> Summary
			else if (gameState == MainGame.POKEMONSUMMARY && summaryChoice == 5 && Input.mousePosition.y >
				Camera.main.WorldToScreenPoint(currentMoveSlot.transform.position).y + currentMoveSlot.
				GetComponent<RectTransform>().rect.height / 2)
			{
				//If not at top, move
				if (moveChoice > 0)
				{
					//Decrease (higher slots are on lower children)
					moveChoice--;

					//Set currentMoveSlot
					currentMoveSlot = summaryScreen.transform.GetChild(5).FindChild("Move" + (moveChoice + 1)).gameObject;
				} //end if
			} //end else if Pokemon Summary on Continue Game -> Team -> Summary

			//Move switch on Continue Game -> Team -> Summary -> Move Details
			else if (gameState == MainGame.MOVESWITCH && Input.mousePosition.y > Camera.main.WorldToScreenPoint(
				currentSwitchSlot.transform.position).y + currentSwitchSlot.GetComponent<RectTransform>().rect.height / 2)
			{
				//If not on top choice, move
				if (switchChoice > 0)
				{
					//Decrease (higher slots are on lower children)
					switchChoice--;

					//Set currentSwitchSlot
					currentSwitchSlot = summaryScreen.transform.GetChild(5).FindChild("Move" + (switchChoice + 1)).gameObject;
				} //end if
			} //end else if Move switch on Continue Game -> Team -> Summary -> Move Details

			//Pokemon Ribbons on Continue Game -> Team -> Ribbons
			else if (gameState == MainGame.POKEMONRIBBONS && Input.mousePosition.y > Camera.main.WorldToScreenPoint(
				currentRibbonSlot.transform.position).y + currentRibbonSlot.GetComponent<RectTransform>().rect.height / 2)
			{
				//If not on first row, move up
				if (ribbonChoice - 4 > -1)
				{
					//Decrease (higher slots are on lower children)
					ribbonChoice -= 4;

					//Read ribbon
					ReadRibbon();

					//Set currentRibbonSlot
					currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").GetChild(ribbonChoice).gameObject;
				} //end if
			} //end else if Pokemon Ribbons on Continue Game -> Team -> Ribbons

			//Pokemon Take/Give on Continue Game -> Team -> Item
			else if (gameState == MainGame.ITEMCHOICE && Input.mousePosition.y > selection.transform.position.y +
				selection.GetComponent<RectTransform>().rect.height / 2)
			{
				//If not on last option, decrease (higher slots are on lower children)
				if (subMenuChoice > 0)
				{
					subMenuChoice--;
				} //end if

				//Reposition selection
				selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
			} //end else if Pokemon Take/Give on Continue Game -> Team -> Item

			//Main Game Home
			else if (gameState == MainGame.HOME)
			{
				PointerEventData eventData = new PointerEventData(EventSystem.current);
				eventData.position = Input.mousePosition;
				List<RaycastResult> results = new List<RaycastResult>();
				EventSystem.current.RaycastAll(eventData, results);
				BaseEventData bEvent = new BaseEventData(EventSystem.current);
				bEvent.selectedObject = EventSystem.current.currentSelectedGameObject;
				if (results.Any() && results[0].gameObject.transform.parent.GetComponent<Button>() != null)
				{
					EventSystem.current.SetSelectedGameObject(results[0].gameObject.transform.parent.gameObject, bEvent);
				} //end if
			} //end else if Main Game Home

			//Gym Battle 
			else if (gameState == MainGame.GYMBATTLE)
			{
				PointerEventData eventData = new PointerEventData(EventSystem.current);
				eventData.position = Input.mousePosition;
				List<RaycastResult> results = new List<RaycastResult>();
				EventSystem.current.RaycastAll(eventData, results);
				BaseEventData bEvent = new BaseEventData(EventSystem.current);
				bEvent.selectedObject = EventSystem.current.currentSelectedGameObject;
				if (results.Any() && results[0].gameObject.transform.parent.GetComponent<Button>() != null)
				{
					EventSystem.current.SetSelectedGameObject(results[0].gameObject.transform.parent.gameObject, bEvent);
				} //end if
			} //end else if GymBattle
		} //end else if Mouse Moves Up

		/*********************************************
		* Mouse Moves Down
		**********************************************/
		if (Input.GetAxis("Mouse Y") < 0)
		{
			//Pokemon submenu on Continue Game -> Team -> Submenu
			if (gameState == MainGame.POKEMONSUBMENU && Input.mousePosition.y < selection.transform.position.y -
			    selection.GetComponent<RectTransform>().rect.height / 2)
			{
				//If not on last option, increase (lower slots are on higher children)
				if (subMenuChoice < choices.transform.childCount - 1)
				{
					subMenuChoice++;
				} //end if

				//Reposition selection
				selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
			} //end if Pokemon submenu on Continue Game -> Team -> Submenu

			//Player team
			else if (gameState == MainGame.TEAM && Input.mousePosition.y < Camera.main.WorldToScreenPoint(
				         currentTeamSlot.transform.position).y - currentTeamSlot.GetComponent<RectTransform>().rect.height / 2)
			{
				//Move from last or second to last slot to PC Button
				if ((choiceNumber == GameManager.instance.GetTrainer().Team.Count - 1 && choiceNumber > 0) ||
				    choiceNumber == GameManager.instance.GetTrainer().Team.Count)
				{
					choiceNumber = -1;
					currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(0).gameObject;
				} //end if
				//Move from PC Button to Cancel Button
				else if (choiceNumber == -1)
				{
					choiceNumber = 0;
					currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(1).gameObject;
				} //end else if
				//Stay on cancel button
				else if (choiceNumber == 0)
				{
					choiceNumber = 0;
				} //end else if
				//Go down vertically
				else
				{
					choiceNumber += 2;
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
				} //end else
			} //end else if Player team

			//Pokemon switch on Continue Game -> Team -> Switch
			else if (gameState == MainGame.POKEMONSWITCH && Input.mousePosition.y < Camera.main.WorldToScreenPoint(
				         currentSwitchSlot.transform.position).y - currentSwitchSlot.GetComponent<RectTransform>().rect.height / 2)
			{
				//Move down as long as not on last or second to last slot
				if (switchChoice < GameManager.instance.GetTrainer().Team.Count - 1)
				{
					switchChoice += 2;
					currentSwitchSlot = playerTeam.transform.FindChild("Pokemon" + switchChoice).gameObject;
				} //end if
			} //end else if Pokemon switch on Continue Game -> Team -> Switch

			//Pokemon Summary on Continue Game -> Team -> Summary
			else if (gameState == MainGame.POKEMONSUMMARY && summaryChoice == 5 && Input.mousePosition.y <
			         Camera.main.WorldToScreenPoint(currentMoveSlot.transform.position).y - currentMoveSlot.
				GetComponent<RectTransform>().rect.height / 2)
			{
				//If next slot is null, don't move
				if (moveChoice < GameManager.instance.GetTrainer().Team[choiceNumber - 1].GetMoveCount() - 1)
				{
					//Increase (lower slots are on higher children)
					moveChoice++;

					//Set currentMoveSlot
					currentMoveSlot = summaryScreen.transform.GetChild(5).FindChild("Move" + (moveChoice + 1)).gameObject;
				} //end if
			} //end else if Pokemon Summary on Continue Game -> Team -> Summary

			//Move switch on Continue Game -> Team -> Summary -> Move Details
			else if (gameState == MainGame.MOVESWITCH && Input.mousePosition.y < Camera.main.WorldToScreenPoint(
				         currentSwitchSlot.transform.position).y - currentSwitchSlot.GetComponent<RectTransform>().rect.height / 2)
			{
				//If next slot is null, don't move
				if (switchChoice < GameManager.instance.GetTrainer().Team[choiceNumber - 1].GetMoveCount() - 1)
				{
					//Increase (Lower slots are on higher children)
					switchChoice++;

					//Set currentSwitchSlot
					currentSwitchSlot = summaryScreen.transform.GetChild(5).FindChild("Move" + (switchChoice + 1)).gameObject;
				} //end if
			} //end else if Move switch on Continue Game -> Team -> Summary -> Move Details

			//Pokemon Ribbons on Continue Game -> Team -> Ribbons
			else if (gameState == MainGame.POKEMONRIBBONS && Input.mousePosition.y < Camera.main.WorldToScreenPoint(
				        currentRibbonSlot.transform.position).y - currentRibbonSlot.GetComponent<RectTransform>().rect.height / 2)
			{
				//If next slot is null, don't move
				if (ribbonChoice + 4 < GameManager.instance.GetTrainer().Team[choiceNumber - 1].GetRibbonCount())
				{
					//Increase (lower slots are on higher children)
					ribbonChoice += 4;

					//Read ribbon
					ReadRibbon();

					//Set currentRibbonSlot
					currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").GetChild(ribbonChoice).gameObject;
				} //end if
			} //end else if Pokemon Ribbons on Continue Game -> Team -> Ribbons

			//Pokemon Take/Give on Continue Game -> Team -> Item
			else if (gameState == MainGame.ITEMCHOICE && Input.mousePosition.y < selection.transform.position.y -
				selection.GetComponent<RectTransform>().rect.height / 2)
			{
				//If not on last option, increase (lower slots are on higher children)
				if (subMenuChoice < choices.transform.childCount - 1)
				{
					subMenuChoice++;
				} //end if

				//Reposition selection
				selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
			} //end else if Pokemon Take/Give on Continue Game -> Team -> Item

			//Main Game Home
			else if (gameState == MainGame.HOME)
			{
				PointerEventData eventData = new PointerEventData(EventSystem.current);
				eventData.position = Input.mousePosition;
				List<RaycastResult> results = new List<RaycastResult>();
				EventSystem.current.RaycastAll(eventData, results);
				BaseEventData bEvent = new BaseEventData(EventSystem.current);
				bEvent.selectedObject = EventSystem.current.currentSelectedGameObject;
				if (results.Any() && results[0].gameObject.transform.parent.GetComponent<Button>() != null)
				{
					EventSystem.current.SetSelectedGameObject(results[0].gameObject.transform.parent.gameObject, bEvent);
				} //end if
			} //end else if Main Game Home

			//Gym Battle 
			else if (gameState == MainGame.GYMBATTLE)
			{
				PointerEventData eventData = new PointerEventData(EventSystem.current);
				eventData.position = Input.mousePosition;
				List<RaycastResult> results = new List<RaycastResult>();
				EventSystem.current.RaycastAll(eventData, results);
				BaseEventData bEvent = new BaseEventData(EventSystem.current);
				bEvent.selectedObject = EventSystem.current.currentSelectedGameObject;
				if (results.Any() && results[0].gameObject.transform.parent.GetComponent<Button>() != null)
				{
					EventSystem.current.SetSelectedGameObject(results[0].gameObject.transform.parent.gameObject, bEvent);
				} //end if
			} //end else if GymBattle
		} //end else if Mouse Moves Down

		/*********************************************
		* Mouse Wheel Up
		**********************************************/
		else if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			//Pokemon Summary on Continue Game -> Team -> Summary
			if (gameState == MainGame.POKEMONSUMMARY)
			{
				//If on any page besides move details
				if (summaryChoice != 5)
				{
					//Decrease (higher slots are on lower children)
					choiceNumber--;

					//Loop to end of team if on first member
					if (choiceNumber < 1)
					{
						choiceNumber = GameManager.instance.GetTrainer().Team.Count;
					} //end if

					//Update current team slot
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
				} //end if
			} //end if Pokemon Summary on Continue Game -> Team -> Summary

			//Pokemon Ribbons on Continue Game -> Team -> Ribbons
			else if (gameState == MainGame.POKEMONRIBBONS)
			{
				//Decrease (higher slots are on lower children)
				choiceNumber--;

				//Loop to end of team if on first member
				if (choiceNumber < 1)
				{
					choiceNumber = GameManager.instance.GetTrainer().Team.Count;
				} //end if

				//Update current team slot
				currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;

				//Reload ribbons
				initialize = false;
			} //end else if Pokemon Ribbons on Continue Game -> Team -> Ribbons

			//Pokemon Item Give From Bag on Continue Game -> Team -> Inventory
			else if (gameState == MainGame.ITEMGIVE)
			{
				inventorySpot = ExtensionMethods.BindToInt(inventorySpot - 1, 0);
				if (inventorySpot < topShown)
				{
					topShown = inventorySpot;
					bottomShown--;
				} //end if
			} //end else if Pokemon Item Give From Bag on Continue Game -> Team -> Inventory
		} //end else if Mouse Wheel Up

		/*********************************************
		* Mouse Wheel Down
		**********************************************/
		else if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			//Pokemon Summary on Continue Game -> Team -> Summary
			if (gameState == MainGame.POKEMONSUMMARY)
			{
				//If on any page besides move details
				if (summaryChoice != 5)
				{
					//Increase (lower slots are on higher children)
					choiceNumber++;

					//Loop to front of team if on last member
					if (choiceNumber > GameManager.instance.GetTrainer().Team.Count)
					{
						choiceNumber = 1;
					} //end if

					//Update current team slot
					currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
				} //end if
			} //end if Pokemon Summary on Continue Game -> Team -> Summary

			//Pokemon Ribbons on Continue Game -> Team -> Ribbons
			else if (gameState == MainGame.POKEMONRIBBONS)
			{
				//Increase (lower slots are on higher children)
				choiceNumber++;

				//Loop to front of team if on last member
				if (choiceNumber > GameManager.instance.GetTrainer().Team.Count)
				{
					choiceNumber = 1;
				} //end if

				//Update current team slot
				currentTeamSlot = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;

				//Reload ribbons
				initialize = false;
			} //end else if Pokemon Ribbons on Continue Game -> Team -> Ribbons

			//Pokemon Item Give From Bag on Continue Game -> Team -> Inventory
			else if (gameState == MainGame.ITEMGIVE)
			{
				inventorySpot = ExtensionMethods.CapAtInt(inventorySpot + 1, GameManager.instance.GetTrainer().SlotCount() - 1);
				if (inventorySpot > bottomShown)
				{
					bottomShown = inventorySpot;
					topShown++;
				} //end if
			} //end else if Pokemon Item Give From Bag on Continue Game -> Team -> Inventory
		} //end else if Mouse Wheel Down

		/*********************************************
		* Left Mouse Button
		**********************************************/
		else if (Input.GetMouseButtonUp(0))
		{
			//Pokemon submenu on Continue Game -> Team -> Submenu
			if (gameState == MainGame.POKEMONSUBMENU)
			{
				//Apply appropriate action based on the submenu selection
				switch (subMenuChoice)
				{
				//Summary
					case 0:
						selection.SetActive(false);
						choices.SetActive(false);
						summaryScreen.SetActive(true);
						summaryChoice = 0;
						summaryScreen.transform.GetChild(0).gameObject.SetActive(false);
						summaryScreen.transform.GetChild(1).gameObject.SetActive(false);
						summaryScreen.transform.GetChild(2).gameObject.SetActive(false);
						summaryScreen.transform.GetChild(3).gameObject.SetActive(false);
						summaryScreen.transform.GetChild(4).gameObject.SetActive(false);
						summaryScreen.transform.GetChild(5).gameObject.SetActive(false);
						gameState = MainGame.POKEMONSUMMARY;
						break;
				//Switch
					case 1:
						selection.SetActive(false);
						choices.SetActive(false);
						switchChoice = choiceNumber;
						previousSwitchSlot = choiceNumber;
						currentSwitchSlot = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
						gameState = MainGame.POKEMONSWITCH;
						break;
				//Item
					case 2:
						initialize = false;
						subMenuChoice = 0;
						FillInChoices();
						gameState = MainGame.ITEMCHOICE;
						break;
				//Ribbons
					case 3:
						selection.SetActive(false);
						choices.SetActive(false);
						initialize = false;
						currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").GetChild(0).gameObject;
						gameState = MainGame.POKEMONRIBBONS;
						break;
				//Cancel
					case 4:
						selection.SetActive(false);
						choices.SetActive(false);
						gameState = MainGame.TEAM;
						break;
				} //end switch
			} //end if Pokemon submenu on Continue Game -> Team -> Submenu

			//Player team, Open menu as long as player isn't selecting a button
			else if (gameState == MainGame.TEAM && choiceNumber > 0)
			{
				//Set submenu active
				selection.SetActive(true);
				choices.SetActive(true);

				//Set up selection box at end of frame if it doesn't fit
				if (selection.GetComponent<RectTransform>().sizeDelta != choices.transform.
					GetChild(0).GetComponent<RectTransform>().sizeDelta)
				{
					selection.SetActive(false);
					StartCoroutine(WaitForResize());
				} //end if

				//Reset position to top of menu
				initialize = false;
				subMenuChoice = 0;
				gameState = MainGame.POKEMONSUBMENU;
			} //end else if player team

			//Pokemon Switch on Continue Game -> Team -> Switch
			else if (gameState == MainGame.POKEMONSWITCH)
			{
				//If the choice doesn't equal the switch, run swap
				if (choiceNumber != switchChoice)
				{
					//Switch function
					GameManager.instance.GetTrainer().Swap(choiceNumber - 1, switchChoice - 1);
				} //end if

				//Go back to team
				initialize = false;
				gameState = MainGame.TEAM;
			} //end else if Pokemon Switch on Continue Game -> Team -> Switch

			//Pokemon summary on Continue Game -> Team -> Summary
			else if (gameState == MainGame.POKEMONSUMMARY)
			{
				//If on moves screen, switch to move details
				if (summaryChoice == 4)
				{
					moveChoice = 0;
					summaryChoice = 5;
					currentMoveSlot = summaryScreen.transform.GetChild(5).FindChild("Move1").gameObject;
				} //end if

				//If on move details screen, go to move switch
				else if (summaryChoice == 5)
				{
					currentMoveSlot.GetComponent<Image>().color = Color.white;
					switchChoice = moveChoice;
					currentSwitchSlot = currentMoveSlot;
					gameState = MainGame.MOVESWITCH;
				} //end else if
			} //end else if Pokemon summary on Continue Game -> Team -> Summary

			//Move Switch on Continue Game -> Team -> Summary -> Move Details
			else if (gameState == MainGame.MOVESWITCH)
			{
				//If switching sparts aren't the same, switch the moves
				if (moveChoice != switchChoice)
				{					
					GameManager.instance.GetTrainer().Team[choiceNumber - 1].SwitchMoves(moveChoice, switchChoice);
				} //end if

				//Set color of background to clear
				currentMoveSlot.GetComponent<Image>().color = Color.clear;
				gameState = MainGame.POKEMONSUMMARY;
			} //end else if Move Switch on Continue Game -> Team -> Summary -> Move Details

			//Pokemon Ribbons on Continue Game -> Team -> Ribbons
			else if (gameState == MainGame.POKEMONRIBBONS)
			{
				//Make sure there are ribbons to be read
				if (GameManager.instance.GetTrainer().Team[choiceNumber - 1].GetRibbonCount() > 0)
				{
					selection.SetActive(!selection.activeSelf);
					ReadRibbon();
				} //end if
			} //end else if Pokemon Ribbons on Continue Game -> Team -> Ribbons

			//Pokemon Take/Give on Continue Game -> Team -> Item
			else if (gameState == MainGame.ITEMCHOICE)
			{
				//Apply appropriate action based on selection
				switch (subMenuChoice)
				{
					//Give
					case 0:
						playerBag.SetActive(true);
						choices.SetActive(false);
						selection.SetActive(false);
						gameState = MainGame.ITEMGIVE;
						initialize = false;
						break;
					//Take
					case 1:
						int itemNumber = GameManager.instance.GetTrainer().Team[choiceNumber - 1].Item;
						GameManager.instance.GetTrainer().AddItem(itemNumber, 1);
						GameManager.instance.GetTrainer().Team[choiceNumber - 1].Item = 0;
						GameManager.instance.DisplayText("Took " + DataContents.GetItemGameName(itemNumber) + " and " +
						"added it to bag.", true);
						gameState = MainGame.TEAM;
						choices.SetActive(false);
						selection.SetActive(false);
						initialize = false;
						break;
					//Cancel
					case 2:
						choices.SetActive(false);
						selection.SetActive(false);
						gameState = MainGame.TEAM;
						break;
				} //end switch
			} //end else if Pokemon Take/Give on Continue Game -> Team -> Item

			//Pokemon Item Give From Bag on Continue Game -> Team -> Inventory
			else if (gameState == MainGame.ITEMGIVE)
			{
				StartCoroutine(ProcessGiveItem());
			} //end else if Pokemon Item Give From Bag on Continue Game -> Team -> Inventory
		} //end else if Left Mouse Button

		/*********************************************
		* Right Mouse Button
		**********************************************/
		else if (Input.GetMouseButtonUp(1))
		{
			//Pokemon Summary on Continue Game -> Team -> Summary
			if (gameState == MainGame.POKEMONSUMMARY)
			{
				//If on any page besides details
				if (summaryChoice != 5)
				{
					//Deactivate summary
					summaryScreen.SetActive(false);

					//Return to team
					gameState = MainGame.TEAM;
				} //end if
				else
				{
					summaryScreen.transform.GetChild(5).gameObject.SetActive(false);
					selection.SetActive(false);
					summaryChoice = 4;
				} //end else
			} //end if Pokemon Summary on Continue Game -> Team -> Summary

			//Pokemon Submenu on Continue Game -> Team -> Submenu
			else if (gameState == MainGame.POKEMONSUBMENU)
			{
				//Deactivate submenu
				choices.SetActive(false);
				selection.SetActive(false);
				gameState = MainGame.TEAM;
			} //end else if Pokemon Submenu on Continue Game -> Team -> Submenu

			//Pokemon Ribbons on Continue Game -> Team -> Ribbons
			else if (gameState == MainGame.POKEMONRIBBONS)
			{
				//Deactivate ribbons
				ribbonScreen.SetActive(false);
				selection.SetActive(false);
				ribbonChoice = 0;
				previousRibbonChoice = -1;

				//Return to team
				gameState = MainGame.TEAM;
			} //end else if Pokemon Ribbons on Continue Game -> Team -> Ribbons

			//Player team
			else if (gameState == MainGame.TEAM)
			{
				EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
				gameState = MainGame.HOME;
			} //end else if Player team

			//Pokemon switch on Continue Game -> Team -> Switch
			else if (gameState == MainGame.POKEMONSWITCH)
			{
				//Go back to team
				gameState = MainGame.TEAM;
			} //end else if Pokemon switch on Continue Game -> Team -> Switch

			//Move Switch on Continue Game -> Team -> Summary -> Move Details
			else if (gameState == MainGame.MOVESWITCH)
			{
				//Return to summary
				currentMoveSlot.GetComponent<Image>().color = Color.clear;
				gameState = MainGame.POKEMONSUMMARY;
			} //end else if Move Switch on Continue Game -> Team -> Summary -> Move Details

			//Pokemon Take/Give on Continue Game -> Team -> Item
			else if (gameState == MainGame.ITEMCHOICE)
			{
				choices.SetActive(false);
				selection.SetActive(false);
				gameState = MainGame.TEAM;
			} //end else if Pokemon Take/Give on Continue Game -> Team -> Item

			//Pokemon Item Give From Bag on Continue Game -> Team -> Inventory
			else if (gameState == MainGame.ITEMGIVE)
			{
				playerBag.SetActive(false);
				gameState = MainGame.TEAM;
			} //end else if Pokemon Item Give From Bag on Continue Game -> Team -> Inventory

			//Gym Battles
			else if (gameState == MainGame.GYMBATTLE)
			{
				EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
				gameState = MainGame.HOME;
			} //end else if Gym Battles

			//Trainer Card
			else if (gameState == MainGame.TRAINERCARD)
			{
				EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
				gameState = MainGame.HOME;
			} //end else if Trainer Card

			//Debug
			else if (gameState == MainGame.DEBUG)
			{
				EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
				gameState = MainGame.HOME;
			} //end else if Debug
		} //end else if Right Mouse Button

		/*********************************************
		* Enter/Return Key
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.Return))
		{
			//Pokemon submenu on Continue Game -> Team -> Submenu
			if (gameState == MainGame.POKEMONSUBMENU)
			{
				//Apply appropriate action based on the submenu selection
				switch (subMenuChoice)
				{
					//Summary
					case 0:
						selection.SetActive(false);
						choices.SetActive(false);
						summaryScreen.SetActive(true);
						summaryChoice = 0;
						summaryScreen.transform.GetChild(0).gameObject.SetActive(false);
						summaryScreen.transform.GetChild(1).gameObject.SetActive(false);
						summaryScreen.transform.GetChild(2).gameObject.SetActive(false);
						summaryScreen.transform.GetChild(3).gameObject.SetActive(false);
						summaryScreen.transform.GetChild(4).gameObject.SetActive(false);
						summaryScreen.transform.GetChild(5).gameObject.SetActive(false);
						gameState = MainGame.POKEMONSUMMARY;
						break;
					//Switch
					case 1:
						selection.SetActive(false);
						choices.SetActive(false);
						switchChoice = choiceNumber;
						previousSwitchSlot = choiceNumber;
						currentSwitchSlot = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
						gameState = MainGame.POKEMONSWITCH;
						break;
					//Item
					case 2:
						initialize = false;
						subMenuChoice = 0;
						FillInChoices();
						gameState = MainGame.ITEMCHOICE;
						break;
					//Ribbons
					case 3:
						selection.SetActive(false);
						choices.SetActive(false);
						initialize = false;
						currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").GetChild(0).gameObject;
						gameState = MainGame.POKEMONRIBBONS;
						break;
					//Cancel
					case 4:
						selection.SetActive(false);
						choices.SetActive(false);
						gameState = MainGame.TEAM;
						break;
				} //end switch
			} //end if Pokemon submenu on Continue Game -> Team -> Submenu

			//Player team
			else if (gameState == MainGame.TEAM)
			{
				//Open menu as long as player isn't selecting a button
				if (choiceNumber > 0)
				{
					//Set submenu active
					selection.SetActive(true);
					choices.SetActive(true);

					//Set up selection box at end of frame if it doesn't fit
					if (selection.GetComponent<RectTransform>().sizeDelta != choices.transform.
					GetChild(0).GetComponent<RectTransform>().sizeDelta)
					{
						selection.SetActive(false);
						StartCoroutine(WaitForResize());
					} //end if

					//Reset position to top of menu
					initialize = false;
					subMenuChoice = 0;
					gameState = MainGame.POKEMONSUBMENU;
				} //end if
				//Return to hom if on Cancel Button
				else if (choiceNumber == 0)
				{
					EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
					gameState = MainGame.HOME;
				} //end else if
				//Go to PC if on PC Button
				else if (choiceNumber == -1)
				{
					GameManager.instance.LoadScene("PC", true);
				} //end else if
			} //end else if player team

			//Pokemon Switch on Continue Game -> Team -> Switch
			else if (gameState == MainGame.POKEMONSWITCH)
			{
				//If the choice doesn't equal the switch, run swap
				if (choiceNumber != switchChoice)
				{
					//Switch function
					GameManager.instance.GetTrainer().Swap(choiceNumber - 1, switchChoice - 1);
				} //end if

				//Go back to team
				initialize = false;
				gameState = MainGame.TEAM;
			} //end else if Pokemon Switch on Continue Game -> Team -> Switch

			//Pokemon summary on Continue Game -> Team -> Summary
			else if (gameState == MainGame.POKEMONSUMMARY)
			{
				//If on moves screen, switch to move details
				if (summaryChoice == 4)
				{
					moveChoice = 0;
					summaryChoice = 5;
					currentMoveSlot = summaryScreen.transform.GetChild(5).FindChild("Move1").gameObject;
				} //end if

				//If on move details screen, go to move switch
				else if (summaryChoice == 5)
				{
					currentMoveSlot.GetComponent<Image>().color = Color.white;
					switchChoice = moveChoice;
					currentSwitchSlot = currentMoveSlot;
					gameState = MainGame.MOVESWITCH;
				} //end else if
			} //end else if Pokemon summary on Continue Game -> Team -> Summary

			//Move Switch on Continue Game -> Team -> Summary -> Move Details
			else if (gameState == MainGame.MOVESWITCH)
			{
				//If switching sparts aren't the same, switch the moves
				if (moveChoice != switchChoice)
				{					
					GameManager.instance.GetTrainer().Team[choiceNumber - 1].SwitchMoves(moveChoice, switchChoice);
				} //end if

				//Set color of background to clear
				currentMoveSlot.GetComponent<Image>().color = Color.clear;
				gameState = MainGame.POKEMONSUMMARY;
			} //end else if Move Switch on Continue Game -> Team -> Summary -> Move Details

			//Pokemon Ribbons on Continue Game -> Team -> Ribbons
			else if (gameState == MainGame.POKEMONRIBBONS)
			{
				//Make sure there are ribbons to be read
				if (GameManager.instance.GetTrainer().Team[choiceNumber - 1].GetRibbonCount() > 0)
				{
					selection.SetActive(!selection.activeSelf);
					ReadRibbon();
				} //end if
			} //end else if Pokemon Ribbons on Continue Game -> Team -> Ribbons

			//Pokemon Take/Give on Continue Game -> Team -> Item
			else if (gameState == MainGame.ITEMCHOICE)
			{
				//Apply appropriate action based on selection
				switch (subMenuChoice)
				{
					//Give
					case 0:
						playerBag.SetActive(true);
						choices.SetActive(false);
						selection.SetActive(false);
						gameState = MainGame.ITEMGIVE;
						initialize = false;
						break;
					//Take
					case 1:
						int itemNumber = GameManager.instance.GetTrainer().Team[choiceNumber - 1].Item;
						GameManager.instance.GetTrainer().AddItem(itemNumber, 1);
						GameManager.instance.GetTrainer().Team[choiceNumber - 1].Item = 0;
						GameManager.instance.DisplayText("Took " + DataContents.GetItemGameName(itemNumber) + " and " +
							"added it to bag.", true);
						choices.SetActive(false);
						selection.SetActive(false);
						initialize = false;
						gameState = MainGame.TEAM;
						break;
						//Cancel
					case 2:
						choices.SetActive(false);
						selection.SetActive(false);
						gameState = MainGame.TEAM;
						break;
				} //end switch
			} //end else if Pokemon Take/Give on Continue Game -> Team -> Item

			//Pokemon Item Give From Bag on Continue Game -> Team -> Inventory
			else if (gameState == MainGame.ITEMGIVE)
			{
				//Verify a item is highlighted
				if(inventorySpot < GameManager.instance.GetTrainer().SlotCount())
				{
					//Make sure pokemon isn't holding another item
					if (GameManager.instance.GetTrainer().Team[choiceNumber - 1].Item == 0)
					{
						int itemNumber = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];
						GameManager.instance.GetTrainer().RemoveItem(itemNumber, 1);
						GameManager.instance.GetTrainer().Team[choiceNumber - 1].Item = itemNumber;
						GameManager.instance.DisplayText("Gave " + DataContents.GetItemGameName(itemNumber)  + " to " + 
							GameManager.instance.GetTrainer().Team[choiceNumber - 1].Nickname,true);
						playerBag.SetActive(false);
						initialize = false;
						gameState = MainGame.TEAM;
					} //end if
					else
					{
						int itemNumber = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];
						GameManager.instance.DisplayConfirm("Do you want to switch the held " + DataContents.GetItemGameName(
							GameManager.instance.GetTrainer().Team[choiceNumber - 1].Item) + " for " + DataContents.GetItemGameName(
								itemNumber) + "?", 0, false);
					} //end else
				} //end if
			} //end else if Pokemon Item Give From Bag on Continue Game -> Team -> Inventory
		} //end else if Enter/Return Key

		/*********************************************
		* X Key
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.X))
		{
			//Pokemon Summary on Continue Game -> Team -> Summary
			if (gameState == MainGame.POKEMONSUMMARY)
			{
				//If on any page besides details
				if (summaryChoice != 5)
				{
					//Deactivate summary
					summaryScreen.SetActive(false);

					//Return to team
					gameState = MainGame.TEAM;
				} //end if
				else
				{
					summaryScreen.transform.GetChild(5).gameObject.SetActive(false);
					selection.SetActive(false);
					summaryChoice = 4;
				} //end else
			} //end if Pokemon Summary on Continue Game -> Team -> Summary

			//Pokemon Submenu on Continue Game -> Team -> Submenu
			else if (gameState == MainGame.POKEMONSUBMENU)
			{
				//Deactivate submenu
				choices.SetActive(false);
				selection.SetActive(false);
				gameState = MainGame.TEAM;
			} //end else if Pokemon Submenu on Continue Game -> Team -> Submenu

			//Pokemon Ribbons on Continue Game -> Team -> Ribbons
			else if (gameState == MainGame.POKEMONRIBBONS)
			{
				//Deactivate ribbons
				ribbonScreen.SetActive(false);
				selection.SetActive(false);
				ribbonChoice = 0;
				previousRibbonChoice = -1;

				//Return to team
				gameState = MainGame.TEAM;
			} //end else if Pokemon Ribbons on Continue Game -> Team -> Ribbons

			//Player team
			else if (gameState == MainGame.TEAM)
			{
				EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
				gameState = MainGame.HOME;
			} //end else if Player team

			//Pokemon switch on Continue Game -> Team -> Switch
			else if (gameState == MainGame.POKEMONSWITCH)
			{
				//Go back to team
				gameState = MainGame.TEAM;
			} //end else if Pokemon switch on Continue Game -> Team -> Switch

			//Move Switch on Continue Game -> Team -> Summary -> Move Details
			else if (gameState == MainGame.MOVESWITCH)
			{
				//Return to summary
				currentMoveSlot.GetComponent<Image>().color = Color.clear;
				gameState = MainGame.POKEMONSUMMARY;
			} //end else if Move Switch on Continue Game -> Team -> Summary -> Move Details

			//Pokemon Take/Give on Continue Game -> Team -> Item
			else if (gameState == MainGame.ITEMCHOICE)
			{
				choices.SetActive(false);
				selection.SetActive(false);
				gameState = MainGame.TEAM;
			} //end else if Pokemon Take/Give on Continue Game -> Team -> Item

			//Pokemon Item Give From Bag on Continue Game -> Team -> Inventory
			else if (gameState == MainGame.ITEMGIVE)
			{
				playerBag.SetActive(false);
				gameState = MainGame.TEAM;
			} //end else if Pokemon Item Give From Bag on Continue Game -> Team -> Inventory

			//Gym Battles
			else if (gameState == MainGame.GYMBATTLE)
			{
				EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
				gameState = MainGame.HOME;
			} //end else if Gym Battles

			//Trainer Card
			else if (gameState == MainGame.TRAINERCARD)
			{
				EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
				gameState = MainGame.HOME;
			} //end else if Trainer Card

			//Debug
			else if (gameState == MainGame.DEBUG)
			{
				EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
				gameState = MainGame.HOME;
			} //end else if Debug
		} //end else if X Key
	} //end GetInput

	/***************************************
	 * Name: FillInChoices
	 * Sets the choices for the choice menu
	 ***************************************/
	void FillInChoices()
	{
		if (gameState == MainGame.TEAM)
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
			choices.transform.GetChild(0).GetComponent<Text>().text = "Summary";
			choices.transform.GetChild(1).GetComponent<Text>().text = "Switch";
			choices.transform.GetChild(2).GetComponent<Text>().text = "Item";
			choices.transform.GetChild(3).GetComponent<Text>().text = "Ribbons";
			choices.transform.GetChild(4).GetComponent<Text>().text = "Cancel";
		} //end if
		else if (gameState == MainGame.POKEMONSUBMENU)
		{
			//Add to choice menu if necessary
			for (int i = choices.transform.childCount; i < 3; i++)
			{
				GameObject clone = Instantiate(choices.transform.GetChild(0).gameObject,
					choices.transform.GetChild(0).position,
					Quaternion.identity) as GameObject;
				clone.transform.SetParent(choices.transform);
			} //end for

			//Destroy extra chocies
			for (int i = choices.transform.childCount; i > 3; i--)
			{
				Destroy(choices.transform.GetChild(i - 1).gameObject);
			} //end for

			//Set text for each choice
			choices.transform.GetChild(0).GetComponent<Text>().text = "Give";
			choices.transform.GetChild(1).GetComponent<Text>().text = "Take";
			choices.transform.GetChild(2).GetComponent<Text>().text = "Cancel";
		} //end else if
	} //end FillInChoices

	/***************************************
	 * Name: GetCorrectSprite
	 * Returns the sprite for the pokemon
	 * based on species, gender, shiny, and
	 * form
	 ***************************************/
	Sprite GetCorrectSprite(Pokemon myPokemon)
	{
		//Get requested sprite
		string chosenString = myPokemon.NatSpecies.ToString("000");
		chosenString += myPokemon.Gender == 1 ? "f" : "";
		chosenString += myPokemon.IsShiny ? "s" : "";
		chosenString += myPokemon.FormNumber > 0 ? "_" + myPokemon.FormNumber.ToString() : "";

		//Change sprite, and fix if sprite is nul
		Sprite result = Resources.Load<Sprite>("Sprites/Pokemon/" + chosenString);
		if (result == null)
		{
			chosenString = chosenString.Replace("f", "");
			result = Resources.Load<Sprite>("Sprites/Pokemon/" + chosenString);

			//If still null, load generic
			if (result == null)
			{
				result = Resources.Load<Sprite>("Sprites/Pokemon/0");
			} //end if
		} //end if

		return result;
	} //end GetCorrectSprite(Pokemon myPokemon)

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
	 * Name: SetTypeSprites
	 * Sets the correct sprite, or disables
	 * if a type isn't found
	 ***************************************/
	void SetTypeSprites(Image type1, Image type2, int natSpecies)
	{
		//Set the primary (first) type
		type1.gameObject.SetActive(true);
		type1.sprite = DataContents.typeSprites[Convert.ToInt32(Enum.Parse(typeof(Types),
			DataContents.ExecuteSQL<string>("SELECT type1 FROM Pokemon WHERE rowid=" + natSpecies)))];

		//Get the string for the secondary type
		string type2SQL = DataContents.ExecuteSQL<string>("SELECT type2 FROM Pokemon WHERE rowid=" + natSpecies);

		//If a second type exists, load the appropriate sprite
		if (!String.IsNullOrEmpty(type2SQL))
		{
			type2.gameObject.SetActive(true);
			type2.sprite = DataContents.typeSprites[Convert.ToInt32(Enum.Parse(typeof(Types), type2SQL))];
		} //end if
		//Otherwise disable the image
		else
		{
			type2.gameObject.SetActive(false);
		} //end else
	} //end SetTypeSprites(Image type1, Image type2, int natSpecies)

	/***************************************
	 * Name: SetStatColor
	 * Sets the color for stat ups and downs
	 ***************************************/
	void SetStatColor(Pokemon myPokemon)
	{
		/*Attack, Defense, Speed, SP Attack, SP Defense*/
		//Get the pokemon's nature
		int currentNature = myPokemon.Nature;

		//Find stat up
		int nd5 = (int)Mathf.Floor(currentNature/5);

		//Find stat down
		int nm5 = (int)Mathf.Floor(currentNature % 5);

		//Get child number of attack
		int childNumber = summaryScreen.transform.GetChild(2).FindChild("Attack").GetSiblingIndex();

		//Set stat colors
		for (int i = 0; i < 5; i++)
		{
			//If stat up
			if (i == nd5 && nd5 != nm5)
			{
				summaryScreen.transform.GetChild(2).GetChild(i + childNumber).GetComponent<Text>().color =
					new Color(0.75f, 0, 0, 1);
			} //end if
			//If stat down
			else if (i == nm5 && nd5 != nm5)
			{
				summaryScreen.transform.GetChild(2).GetChild(i + childNumber).GetComponent<Text>().color =
					new Color(0, 0, 0.75f, 1);
			} //end else if
			//Otherwise black
			else
			{
				summaryScreen.transform.GetChild(2).GetChild(i + childNumber).GetComponent<Text>().color = Color.black;
			} //end else
		} //end for
	} //end SetStatColor(Pokemon myPokemon) 

	/***************************************
	 * Name: SetMoveSprites
	 * Sets the correct sprite, or disables 
	 * if a move isn't found
	 ***************************************/
	void SetMoveSprites(Pokemon myPokemon, Transform moveScreen)
	{
		//Loop through the list of pokemon moves and set each one
		for (int i = 0; i < 4; i++)
		{
			//Make sure move isn't null
			if (myPokemon.GetMove(i) != -1)
			{
				//Set the move type
				moveScreen.FindChild("Move" + (i + 1)).gameObject.SetActive(true);
				moveScreen.FindChild("Move" + (i + 1)).GetChild(0).GetComponent<Image>().sprite =
					DataContents.typeSprites[DataContents.GetMoveIcon(myPokemon.GetMove(i))];

				//Set the move name
				moveScreen.FindChild("Move" + (i + 1)).GetChild(1).GetComponent<Text>().text =
					DataContents.ExecuteSQL<string>("SELECT gameName FROM Moves WHERE rowid=" +
					myPokemon.GetMove(i));

				//Set the move PP
				moveScreen.FindChild("Move" + (i + 1)).GetChild(2).GetComponent<Text>().text = "PP " +
					myPokemon.GetMovePP(i).ToString() + "/" + myPokemon.GetMovePPMax(i);
			} //end if
			else
			{
				//Blank out type
				moveScreen.FindChild("Move" + (i+1)).gameObject.SetActive(false);
			} //end else
		} //end for
	} //end SetMoveSprites(Pokemon myPokemon, Transform moveScreen)

	/***************************************
	 * Name: SetMoveDetails
	 * Sets summary screen move summary page
	 * with details of the moves
	 ***************************************/
	void SetMoveDetails(Pokemon myPokemon, Transform moveScreen)
	{
		//Resize selection rect
		selection.SetActive(true);
		Vector3 scale = new Vector3(moveScreen.FindChild("Move" + (moveChoice + 1)).GetComponent<RectTransform>().rect.width,
			                moveScreen.FindChild("Move" + (moveChoice + 1)).GetComponent<RectTransform>().rect.height,
			                0);
		selection.GetComponent<RectTransform>().sizeDelta = scale;

		//Reposition to location of top choice
		selection.transform.position = Camera.main.WorldToScreenPoint(currentMoveSlot.transform.position);

		//Set the move category
		moveScreen.FindChild("Category").GetComponent<Image>().sprite =
			DataContents.categorySprites[Convert.ToInt32(Enum.Parse(typeof(Categories),
			DataContents.ExecuteSQL<string>("SELECT category FROM Moves WHERE rowid=" +
			myPokemon.GetMove(moveChoice))))];

		//Set the move power
		int temp = DataContents.ExecuteSQL<int>("SELECT baseDamage FROM Moves WHERE rowid=" +
		           myPokemon.GetMove(moveChoice));
		moveScreen.FindChild("Power").GetComponent<Text>().text = temp > 1 ? temp.ToString() : "---";

		//Set the move accuracy
		temp = DataContents.ExecuteSQL<int>("SELECT accuracy FROM Moves WHERE rowid=" + myPokemon.GetMove(moveChoice));
		moveScreen.FindChild("Accuracy").GetComponent<Text>().text = temp >= 1 ? temp.ToString() : "---";

		//Set font size of move description
		if (detailsSize != -1)
		{
			//Set the move description text
			moveScreen.FindChild("MoveDescription").GetComponent<Text>().text =
				DataContents.ExecuteSQL<string>("SELECT description FROM Moves WHERE rowid=" + myPokemon.GetMove(moveChoice));
		} //end if
		else
		{
			//Get font size
			moveScreen.FindChild("MoveDescription").GetComponent<Text>().text =
				DataContents.ExecuteSQL<string>("SELECT description FROM Moves WHERE gameName='Stealth Rock'");
			StartCoroutine(WaitForFontResize(moveScreen, myPokemon));
		} //end else
	} //end SetMoveDetails

	/***************************************
	 * Name: PokemonSummary
	 * Sets summary screen details for each
	 * page
	 ***************************************/
	void PokemonSummary(Pokemon pokemonChoice)
	{
		//Switch based on active page
		switch (summaryChoice)
		{
			//Info Screen
			case 0:
				summaryScreen.transform.GetChild(0).gameObject.SetActive(true);
				summaryScreen.transform.GetChild(0).FindChild("Name").GetComponent<Text>().text =
					pokemonChoice.Nickname;
				summaryScreen.transform.GetChild(0).FindChild("Level").GetComponent<Text>().text =
					pokemonChoice.CurrentLevel.ToString();
				summaryScreen.transform.GetChild(0).FindChild("Ball").GetComponent<Image>().sprite =
					Resources.Load<Sprite>("Sprites/Icons/summaryBall" + pokemonChoice.BallUsed.ToString("00"));
				summaryScreen.transform.GetChild(0).FindChild("Gender").GetComponent<Image>().sprite =
					Resources.Load<Sprite>("Sprites/Icons/gender" + pokemonChoice.Gender.ToString());
				summaryScreen.transform.GetChild(0).FindChild("Sprite").GetComponent<Image>().sprite =
					GetCorrectSprite(pokemonChoice);
				summaryScreen.transform.GetChild(0).FindChild("Markings").GetComponent<Text>().text =
					pokemonChoice.GetMarkingsString();
				summaryScreen.transform.GetChild(0).FindChild("Shiny").gameObject.SetActive(pokemonChoice.IsShiny);
				summaryScreen.transform.GetChild(0).FindChild("Item").GetComponent<Text>().text =
					DataContents.GetItemGameName(pokemonChoice.Item);
				summaryScreen.transform.GetChild(0).FindChild("DexNumber").GetComponent<Text>().text =
					pokemonChoice.NatSpecies.ToString();
				summaryScreen.transform.GetChild(0).FindChild("Species").GetComponent<Text>().text =
					DataContents.ExecuteSQL<string>("SELECT name FROM Pokemon WHERE rowid=" + pokemonChoice.NatSpecies);
				summaryScreen.transform.GetChild(0).FindChild("OT").GetComponent<Text>().text =
					pokemonChoice.OTName;
				summaryScreen.transform.GetChild(0).FindChild("IDNumber").GetComponent<Text>().text =
					pokemonChoice.TrainerID.ToString();
				summaryScreen.transform.GetChild(0).FindChild("CurrentXP").GetComponent<Text>().text =
					pokemonChoice.CurrentEXP.ToString();
				summaryScreen.transform.GetChild(0).FindChild("RemainingXP").GetComponent<Text>().text =
					pokemonChoice.RemainingEXP.ToString();
				SetTypeSprites(summaryScreen.transform.GetChild(0).FindChild("Types").GetChild(0).GetComponent<Image>(),
					summaryScreen.transform.GetChild(0).FindChild("Types").GetChild(1).GetComponent<Image>(), 
					pokemonChoice.NatSpecies);
				SetStatusIcon(summaryScreen.transform.GetChild(0).FindChild("Status").GetComponent<Image>(), pokemonChoice);
				break;
			//Memo screen
			case 1:
				summaryScreen.transform.GetChild(1).gameObject.SetActive(true);
				summaryScreen.transform.GetChild(1).FindChild("Name").GetComponent<Text>().text =
					pokemonChoice.Nickname;
				summaryScreen.transform.GetChild(1).FindChild("Level").GetComponent<Text>().text =
					pokemonChoice.CurrentLevel.ToString();
				summaryScreen.transform.GetChild(1).FindChild("Ball").GetComponent<Image>().sprite =
					Resources.Load<Sprite>("Sprites/Icons/summaryBall" + pokemonChoice.BallUsed.ToString("00"));
				summaryScreen.transform.GetChild(1).FindChild("Gender").GetComponent<Image>().sprite =
					Resources.Load<Sprite>("Sprites/Icons/gender" + pokemonChoice.Gender.ToString());
				summaryScreen.transform.GetChild(1).FindChild("Sprite").GetComponent<Image>().sprite =
					GetCorrectSprite(pokemonChoice);
				summaryScreen.transform.GetChild(1).FindChild("Markings").GetComponent<Text>().text =
					pokemonChoice.GetMarkingsString();
				summaryScreen.transform.GetChild(1).FindChild("Shiny").gameObject.SetActive(pokemonChoice.IsShiny);
				summaryScreen.transform.GetChild(1).FindChild("Item").GetComponent<Text>().text =
					DataContents.GetItemGameName(pokemonChoice.Item);
				summaryScreen.transform.GetChild(1).FindChild("Nature").GetComponent<Text>().text =
					"<color=#cc0000ff>" + ((Natures)pokemonChoice.Nature).ToString() + "</color> nature";
				summaryScreen.transform.GetChild(1).FindChild("CaughtDate").GetComponent<Text>().text =
					pokemonChoice.ObtainTime.ToLongDateString() + " at " + pokemonChoice.ObtainTime.ToShortTimeString();
				summaryScreen.transform.GetChild(1).FindChild("CaughtType").GetComponent<Text>().text =
					((ObtainTypeEnum)pokemonChoice.ObtainType).ToString() + " from " + ((ObtainFromEnum)pokemonChoice.ObtainFrom).
					ToString();
				summaryScreen.transform.GetChild(1).FindChild("CaughtLevel").GetComponent<Text>().text =
					"Found at level " + pokemonChoice.ObtainLevel;
				SetStatusIcon(summaryScreen.transform.GetChild(1).FindChild("Status").GetComponent<Image>(), pokemonChoice);
				break;
			//Stats
			case 2:
				summaryScreen.transform.GetChild(2).gameObject.SetActive(true);
				summaryScreen.transform.GetChild(2).FindChild("Name").GetComponent<Text>().text =
					pokemonChoice.Nickname;
				summaryScreen.transform.GetChild(2).FindChild("Level").GetComponent<Text>().text =
					pokemonChoice.CurrentLevel.ToString();
				summaryScreen.transform.GetChild(2).FindChild("Ball").GetComponent<Image>().sprite =
					Resources.Load<Sprite>("Sprites/Icons/summaryBall" + pokemonChoice.BallUsed.ToString("00"));
				summaryScreen.transform.GetChild(2).FindChild("Gender").GetComponent<Image>().sprite =
					Resources.Load<Sprite>("Sprites/Icons/gender" + pokemonChoice.Gender.ToString());
				summaryScreen.transform.GetChild(2).FindChild("Sprite").GetComponent<Image>().sprite =
					GetCorrectSprite(pokemonChoice);
				summaryScreen.transform.GetChild(2).FindChild("Markings").GetComponent<Text>().text =
					pokemonChoice.GetMarkingsString();
				summaryScreen.transform.GetChild(2).FindChild("Shiny").gameObject.SetActive(pokemonChoice.IsShiny);
				summaryScreen.transform.GetChild(2).FindChild("Item").GetComponent<Text>().text =
					DataContents.GetItemGameName(pokemonChoice.Item);
				summaryScreen.transform.GetChild(2).FindChild("HP").GetComponent<Text>().text = 
					pokemonChoice.CurrentHP.ToString() + "/" + pokemonChoice.TotalHP.ToString();
				summaryScreen.transform.GetChild(2).FindChild("RemainingHP").GetComponent<RectTransform>().localScale =
					new Vector3((float)pokemonChoice.CurrentHP / (float)pokemonChoice.TotalHP, 1f, 1f);
				summaryScreen.transform.GetChild(2).FindChild("Attack").GetComponent<Text>().text = 
					pokemonChoice.Attack.ToString();
				summaryScreen.transform.GetChild(2).FindChild("Defense").GetComponent<Text>().text = 
					pokemonChoice.Defense.ToString();
				summaryScreen.transform.GetChild(2).FindChild("SpAttack").GetComponent<Text>().text = 
					pokemonChoice.SpecialA.ToString();
				summaryScreen.transform.GetChild(2).FindChild("SpDefense").GetComponent<Text>().text = 
					pokemonChoice.SpecialD.ToString();
				summaryScreen.transform.GetChild(2).FindChild("Speed").GetComponent<Text>().text = 
					pokemonChoice.Speed.ToString();
				summaryScreen.transform.GetChild(2).FindChild("AbilityName").GetComponent<Text>().text = 
					pokemonChoice.GetAbilityName();
				summaryScreen.transform.GetChild(2).FindChild("AbilityDescription").GetComponent<Text>().text = 
					pokemonChoice.GetAbilityDescription();
				SetStatColor(pokemonChoice);
				SetStatusIcon(summaryScreen.transform.GetChild(2).FindChild("Status").GetComponent<Image>(), pokemonChoice);
				break;
			//EV-IV
			case 3:
				summaryScreen.transform.GetChild(3).gameObject.SetActive(true);
				summaryScreen.transform.GetChild(3).FindChild("Name").GetComponent<Text>().text =
					pokemonChoice.Nickname;
				summaryScreen.transform.GetChild(3).FindChild("Level").GetComponent<Text>().text =
					pokemonChoice.CurrentLevel.ToString();
				summaryScreen.transform.GetChild(3).FindChild("Ball").GetComponent<Image>().sprite =
					Resources.Load<Sprite>("Sprites/Icons/summaryBall" + pokemonChoice.BallUsed.ToString("00"));
				summaryScreen.transform.GetChild(3).FindChild("Gender").GetComponent<Image>().sprite =
					Resources.Load<Sprite>("Sprites/Icons/gender" + pokemonChoice.Gender.ToString());
				summaryScreen.transform.GetChild(3).FindChild("Sprite").GetComponent<Image>().sprite =
					GetCorrectSprite(pokemonChoice);
				summaryScreen.transform.GetChild(3).FindChild("Markings").GetComponent<Text>().text =
					pokemonChoice.GetMarkingsString();
				summaryScreen.transform.GetChild(3).FindChild("Shiny").gameObject.SetActive(pokemonChoice.IsShiny);
				summaryScreen.transform.GetChild(3).FindChild("Item").GetComponent<Text>().text =
					DataContents.GetItemGameName(pokemonChoice.Item);
				summaryScreen.transform.GetChild(3).FindChild("HP").GetComponent<Text>().text = 
					pokemonChoice.GetEV(0).ToString() + "/" + pokemonChoice.GetIV(0).ToString();
				summaryScreen.transform.GetChild(3).FindChild("RemainingHP").GetComponent<RectTransform>().localScale =
					new Vector3((float)pokemonChoice.CurrentHP / (float)pokemonChoice.TotalHP, 1f, 1f);
				summaryScreen.transform.GetChild(3).FindChild("Attack").GetComponent<Text>().text = 
					pokemonChoice.GetEV(1).ToString() + "/" + pokemonChoice.GetIV(1).ToString();
				summaryScreen.transform.GetChild(3).FindChild("Defense").GetComponent<Text>().text = 
					pokemonChoice.GetEV(2).ToString() + "/" + pokemonChoice.GetIV(2).ToString();
				summaryScreen.transform.GetChild(3).FindChild("SpAttack").GetComponent<Text>().text = 
					pokemonChoice.GetEV(4).ToString() + "/" + pokemonChoice.GetIV(4).ToString();
				summaryScreen.transform.GetChild(3).FindChild("SpDefense").GetComponent<Text>().text = 
					pokemonChoice.GetEV(5).ToString() + "/" + pokemonChoice.GetIV(5).ToString();
				summaryScreen.transform.GetChild(3).FindChild("Speed").GetComponent<Text>().text = 
					pokemonChoice.GetEV(3).ToString() + "/" + pokemonChoice.GetIV(3).ToString();
				summaryScreen.transform.GetChild(3).FindChild("AbilityName").GetComponent<Text>().text = 
					pokemonChoice.GetAbilityName();
				summaryScreen.transform.GetChild(3).FindChild("AbilityDescription").GetComponent<Text>().text = 
					pokemonChoice.GetAbilityDescription();
				SetStatusIcon(summaryScreen.transform.GetChild(3).FindChild("Status").GetComponent<Image>(), pokemonChoice);
				break;
			//Moves
			case 4:
				summaryScreen.transform.GetChild(4).gameObject.SetActive(true);
				summaryScreen.transform.GetChild(4).FindChild("Name").GetComponent<Text>().text =
					pokemonChoice.Nickname;
				summaryScreen.transform.GetChild(4).FindChild("Level").GetComponent<Text>().text =
					pokemonChoice.CurrentLevel.ToString();
				summaryScreen.transform.GetChild(4).FindChild("Ball").GetComponent<Image>().sprite =
					Resources.Load<Sprite>("Sprites/Icons/summaryBall" + pokemonChoice.BallUsed.ToString("00"));
				summaryScreen.transform.GetChild(4).FindChild("Gender").GetComponent<Image>().sprite =
					Resources.Load<Sprite>("Sprites/Icons/gender" + pokemonChoice.Gender.ToString());
				summaryScreen.transform.GetChild(4).FindChild("Sprite").GetComponent<Image>().sprite =
					GetCorrectSprite(pokemonChoice);
				summaryScreen.transform.GetChild(4).FindChild("Markings").GetComponent<Text>().text =
					pokemonChoice.GetMarkingsString();
				summaryScreen.transform.GetChild(4).FindChild("Shiny").gameObject.SetActive(pokemonChoice.IsShiny);
				summaryScreen.transform.GetChild(4).FindChild("Item").GetComponent<Text>().text =
					DataContents.GetItemGameName(pokemonChoice.Item);
				SetMoveSprites(pokemonChoice, summaryScreen.transform.GetChild(4));
				SetStatusIcon(summaryScreen.transform.GetChild(4).FindChild("Status").GetComponent<Image>(), pokemonChoice);
				break;
			//Move Details
			case 5:
				summaryScreen.transform.GetChild(5).gameObject.SetActive(true);
				summaryScreen.transform.GetChild(5).FindChild("Sprite").GetComponent<Image>().sprite =
					GetCorrectIcon(pokemonChoice);
				SetMoveDetails(pokemonChoice, summaryScreen.transform.GetChild(5));
				SetTypeSprites(summaryScreen.transform.GetChild(5).FindChild("SpeciesTypes").GetChild(0).GetComponent<Image>(),
					summaryScreen.transform.GetChild(5).FindChild("SpeciesTypes").GetChild(1).GetComponent<Image>(),
					pokemonChoice.NatSpecies);
				SetMoveSprites(pokemonChoice, summaryScreen.transform.GetChild(5));
				break;
		} //end switch
	} //end PokemonSummary(Pokemon pokemonChoice)

	/***************************************
	 * Name: ReadRibbon
	 * Reads or disables ribbon data
	 ***************************************/
	void ReadRibbon()
	{
		//If text isn't displayed
		if (gameState == MainGame.POKEMONRIBBONS && ribbonChoice != previousRibbonChoice && selection.activeSelf)
		{
			//Activate the fields
			ribbonScreen.transform.FindChild("RibbonName").gameObject.SetActive(true);
			ribbonScreen.transform.FindChild("RibbonDescription").gameObject.SetActive(true);

			//Positon selection rectangle
			selection.transform.position = Camera.main.WorldToScreenPoint(ribbonScreen.transform.
				FindChild("RibbonRegion").GetChild(ribbonChoice).GetComponent<RectTransform>().position);

			//Get the ribbon value at the index
			int ribbonValue = GameManager.instance.GetTrainer().Team[choiceNumber - 1].GetRibbon(ribbonChoice);

			//Set the name and description
			ribbonScreen.transform.FindChild("RibbonName").GetComponent<Text>().text = 
				DataContents.ribbonData.GetRibbonName(ribbonValue);
			ribbonScreen.transform.FindChild("RibbonDescription").GetComponent<Text>().text = 
				DataContents.ribbonData.GetRibbonDescription(ribbonValue);

			//Finished reading, set previous to current
			previousRibbonChoice = ribbonChoice;
		} //end if

		//Otherwise hide the text
		else
		{
			ribbonScreen.transform.FindChild("RibbonName").gameObject.SetActive(false);
			ribbonScreen.transform.FindChild("RibbonDescription").gameObject.SetActive(false);
			previousRibbonChoice = -1;
		} //end else
	} //end ReadRibbon

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
		yield return new WaitForEndOfFrame();

		//Team Submenu
		if (gameState == MainGame.POKEMONSUBMENU)
		{
			Vector3 scale = new Vector3(choices.GetComponent<RectTransform>().rect.width,
				choices.GetComponent<RectTransform>().rect.height /
				choices.transform.childCount, 0);
			selection.GetComponent<RectTransform>().sizeDelta = scale;
			selection.transform.position = choices.transform.GetChild(0).
				GetComponent<RectTransform>().position;
			selection.SetActive(true);
		} //end if

		//Pokemon Ribbons
		else if (gameState == MainGame.POKEMONRIBBONS)
		{
			Vector3 scale = new Vector3(ribbonScreen.transform.FindChild("RibbonRegion").GetChild(0).
				GetComponent<RectTransform>().rect.width, ribbonScreen.transform.FindChild("RibbonRegion").
				GetChild(0).GetComponent<RectTransform>().rect.height, 0);
			selection.GetComponent<RectTransform>().sizeDelta = scale;
			selection.transform.position = Camera.main.WorldToScreenPoint(ribbonScreen.transform.
				FindChild("RibbonRegion").GetChild(0).GetComponent<RectTransform>().position);
		} //end else if
	} //end WaitForResize

	/***************************************
	 * Name: WaitForFontResize
	 * Waits for move description font to
	 * resize to best fit
	 ***************************************/
	IEnumerator WaitForFontResize(Transform moveScreen, Pokemon myPokemon)
	{
		//Process at end of frame
		yield return new WaitForEndOfFrame();

		//Get details size
		detailsSize = moveScreen.FindChild("MoveDescription").GetComponent<Text>().cachedTextGenerator.fontSizeUsedForBestFit;

		//Assign size
		moveScreen.FindChild("MoveDescription").GetComponent<Text>().resizeTextForBestFit = false;
		moveScreen.FindChild("MoveDescription").GetComponent<Text>().fontSize = detailsSize;
		moveScreen.FindChild("MoveDescription").GetComponent<Text>().text =
			DataContents.ExecuteSQL<string>("SELECT description FROM Moves WHERE rowid=" +
			myPokemon.GetMove(moveChoice));
	} //end WaitForFontResize(Transform moveScreen, Pokemon myPokemon)

	/***************************************
	* Name: SetGameState
	* Sets gameState to parameter
	***************************************/
	public IEnumerator SetGameState(MainGame newGameState)
	{
		//Process at end of frame
	    yield return new WaitForEndOfFrame ();
	
	    //Set the new state
	    gameState = newGameState; 
	} //end SetGameState(MainGame newGameState)

    /***************************************
     * Name: SetSummaryPage
     * Sets summaryChoice to parameter
     ***************************************/
    public IEnumerator SetSummaryPage(int summaryPage)
    {
        //Process at end of frame
        yield return new WaitForEndOfFrame ();

        //Change screen only if summary screen is active
        if (summaryScreen.activeSelf)
        {
            //If move switch is active
            if(gameState == MainGame.MOVESWITCH)
            {
                //Return to summary
                currentMoveSlot.GetComponent<Image>().color = Color.clear;
                summaryScreen.transform.GetChild(5).gameObject.SetActive(false);
                summaryScreen.transform.GetChild(4).gameObject.SetActive(false);
                selection.SetActive(false);
                
                //Change to new page
                summaryChoice = summaryPage;
                gameState = MainGame.POKEMONSUMMARY;
            } //end if
            else if(summaryChoice == 5)
            {
                summaryScreen.transform.GetChild(5).gameObject.SetActive(false);
                summaryScreen.transform.GetChild(4).gameObject.SetActive(false);
                selection.SetActive(false);
                summaryChoice = summaryPage;
            } //end else if
            else
            {
                //Deactivate current page
                summaryScreen.transform.GetChild(summaryChoice).gameObject.SetActive(false);

                //Change to new page
                summaryChoice = summaryPage;
            } //end else
        } //end if
    } //end SetSummaryPage(int summaryPage)

	/***************************************
	 * Name: FadeInBagPocket
	 * Plays a fade in animation for the bag
	 * after a delay because Invoke can't take
	 * parameters and a delay.
	 ***************************************/
	public IEnumerator FadeInBagPocket()
	{
		pocketChange = true;
		inventorySpot = 0;
		bottomShown = 9;
		topShown = 0;
		initialize = false;
		yield return new WaitForSeconds(Time.deltaTime);
		GameManager.instance.FadeInAnimation(2);
		pocketChange = false;
	} //end FadeInBagPocket

	/***************************************
	* Name: ProcessGiveItem
	* Processes what to do when item is given
	***************************************/
	IEnumerator ProcessGiveItem()
	{
		//Process at end of frame
		yield return new WaitForEndOfFrame ();

		//Verify a item is highlighted
		if(inventorySpot < GameManager.instance.GetTrainer().SlotCount() && !pocketChange)
		{
			//Make sure pokemon isn't holding another item
			if (GameManager.instance.GetTrainer().Team[choiceNumber - 1].Item == 0)
			{
				int itemNumber = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];
				GameManager.instance.GetTrainer().RemoveItem(itemNumber, 1);
				GameManager.instance.GetTrainer().Team[choiceNumber - 1].Item = itemNumber;
				GameManager.instance.DisplayText("Gave " + DataContents.GetItemGameName(itemNumber)  + " to " + 
					GameManager.instance.GetTrainer().Team[choiceNumber - 1].Nickname,true);
				playerBag.SetActive(false);
				initialize = false;
				gameState = MainGame.TEAM;
			} //end if
			else
			{
				int itemNumber = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];
				GameManager.instance.DisplayConfirm("Do you want to switch the held " + DataContents.GetItemGameName(
					GameManager.instance.GetTrainer().Team[choiceNumber - 1].Item) + " for " + DataContents.GetItemGameName(
						itemNumber) + "?", 0, false);
			} //end else
		} //end if
	} //end ProcessGiveItem

	/***************************************
	 * Name: ApplyConfirm
	 * Applies the confirm choice
	 ***************************************/
	public void ApplyConfirm(ConfirmChoice e)
	{
		//Yes selected
		if (e.Choice == 0)
		{
			//Get item
			int itemNumber = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];

			//Swap items
			GameManager.instance.GetTrainer().RemoveItem(itemNumber, 1);
			GameManager.instance.GetTrainer().AddItem(GameManager.instance.GetTrainer().Team[choiceNumber - 1].
				Item , 1);
			GameManager.instance.GetTrainer().Team[choiceNumber - 1].Item = itemNumber;
			GameManager.instance.DisplayText("Gave " + DataContents.GetItemGameName(itemNumber)  + " to " + 
				GameManager.instance.GetTrainer().Team[choiceNumber - 1].Nickname + " and " +
				"put other item in bag.",true);
			playerBag.SetActive(false);
			initialize = false;
			gameState = MainGame.TEAM;
		} //end if

		//No selected
		else if(e.Choice== 1)
		{
			GameManager.instance.DisplayText("Did not switch items.", true);
			playerBag.SetActive(false);
			initialize = false;
			gameState = MainGame.TEAM;
		} //end else			
	} //end ApplyConfirm(ConfirmChoice e)

	/***************************************
	 * Name: ChangeCheckpoint
	 * Changes the checkpoint
	 ***************************************/
	public void ChangeCheckpoint(int newCheckpoint)
	{
		checkpoint = newCheckpoint;
	} //end ChangeCheckpoint(int newCheckpoint)

	#region Debug
	/***************************************
	 * Name: EditPokemonMode
	 * Activates the pokemon edit panel
	 ***************************************/
	public void EditPokemonMode()
	{
		debugButtons.SetActive(false);
		debugOptions.transform.GetChild(1).gameObject.SetActive(true);
		debugOptions.transform.GetChild(2).gameObject.SetActive(false);
	} //end EditPokemonMode

	/***************************************
	 * Name: EditTrainerMode
	 * Activates the trainer edit panel
	 ***************************************/
	public void EditTrainerMode()
	{
		debugButtons.SetActive(false);
		debugOptions.transform.GetChild(1).gameObject.SetActive(false);
		debugOptions.transform.GetChild(2).gameObject.SetActive(true);
	} //end EditTrainerMode

	/***************************************
	 * Name: RandomPokemon
	 * Adds a single random pokemon to 
	 * requested area
	 ***************************************/
	public void RandomPokemon()
	{
		//Generate random pokemon
		Pokemon newPokemon = new Pokemon();

		//Update relevant portions
		UpdateDebug(newPokemon);
	} //end RandomPokemon

	/***************************************
	 * Name: EditPokemon
	 * Grab a pokemon from team or pc and
	 * populate debug fields with it
	 ***************************************/
	public void EditPokemon()
	{
		//Get requested pokemon
		if (GameObject.Find("LeftRegion").transform.FindChild("TeamToggle").GetComponent<Toggle>().isOn)
		{
			int outputHolder = 0;
			string slot = GameObject.Find("LeftRegion").transform.FindChild("Slot").GetComponent<InputField>().text;
			if (int.TryParse(slot, out outputHolder))
			{
				int bound = ExtensionMethods.WithinIntRange(outputHolder, 0, GameManager.instance.GetTrainer().Team.Count - 1);
				UpdateDebug(GameManager.instance.GetTrainer().Team[bound]);
			} //end if
			else
			{
				UpdateDebug(GameManager.instance.GetTrainer().Team[0]);
			} //end else
		} //end if
		else
		{
			int slotSpace = 0;
			int boxSpace = 0;
			string slot = GameObject.Find("LeftRegion").transform.FindChild("Slot").GetComponent<InputField>().text;
			string box = GameObject.Find("LeftRegion").transform.FindChild("Box").GetComponent<InputField>().text;
			if(!int.TryParse(slot, out slotSpace))
			{
				slotSpace = 0;
			} //end if
			if(!int.TryParse(box, out boxSpace))
			{
				boxSpace = GameManager.instance.GetTrainer().GetPCBox();
			} //end if
			Pokemon toUse = GameManager.instance.GetTrainer().GetPC(boxSpace, slotSpace);
			if (toUse == null)
			{
				toUse = GameManager.instance.GetTrainer().GetFirstPokemon();	
			} //end if
			if (toUse != null)
			{
				UpdateDebug(toUse);
			} //end if
			else
			{
				GameManager.instance.DisplayText("No pokemon found in the current box. Edit Pokemon canceled.", true);
			} //end else
		} //end else 
	} //end EditPokemon

	/***************************************
	 * Name: RemovePokemon
	 * Remove a pokemon from PC or Team
	 ***************************************/
	public void RemovePokemon()
	{
		//Get requested pokemon
		if (GameObject.Find("LeftRegion").transform.FindChild("TeamToggle").GetComponent<Toggle>().isOn)
		{
			int outputHolder = 0;
			string slot = GameObject.Find("LeftRegion").transform.FindChild("Slot").GetComponent<InputField>().text;
			if (int.TryParse(slot, out outputHolder))
			{
				if (outputHolder < GameManager.instance.GetTrainer().Team.Count)
				{
					GameManager.instance.GetTrainer().RemovePokemon(outputHolder);
				} //end if
			} //ed if
			else
			{
				GameManager.instance.DisplayText("No pokemon found on team at requested slot. Remove Pokemon canceled.", true);
			} //end else
		} //end if
		else
		{
			int slotSpace = 0;
			int boxSpace = 0;
			string slot = GameObject.Find("LeftRegion").transform.FindChild("Slot").GetComponent<InputField>().text;
			string box = GameObject.Find("LeftRegion").transform.FindChild("Box").GetComponent<InputField>().text;
			if (!int.TryParse(slot, out slotSpace))
			{
				if (!int.TryParse(box, out boxSpace))
				{
					GameManager.instance.GetTrainer().RemoveFromPC(boxSpace, slotSpace);
				} //end if
				else
				{
					GameManager.instance.DisplayText("Invalid Box given. Remove Pokemon canceled.", true);
				} //end else
			} //end if
			else
			{
				GameManager.instance.DisplayText("Invalid Slot given. Remove Pokemon canceled.", true);
			} //end else
		} //end else
	} //end RemovePokemon

    /***************************************
     * Name: FinishEditing
     * Apply pokemon to requested spot
     ***************************************/ 
    public void FinishEditing()
    {
        if (debugOptions.transform.GetChild (2).gameObject.activeSelf)
        {
			GameManager.instance.GetTrainer().PlayerImage = trainerRightRegion.transform.FindChild("TrainerSprite").
				GetComponent<Dropdown>().value;
            for(int i = 0; i < 8; i++)
            {
                GameManager.instance.GetTrainer ().SetPlayerBadges (39 + i, trainerRightRegion.transform.FindChild ("Badges").
                    GetChild (i).GetComponent<Toggle> ().isOn);
            } //end for
            GameManager.instance.DisplayText("Updated trainer.", true);
        } //end if
        else
        {
            Pokemon newPokemon = new Pokemon (
                pokemonRightRegion.transform.FindChild ("PokemonName").GetComponent<Dropdown> ().value + 1,
                int.Parse (pokemonRightRegion.transform.FindChild ("TrainerID").GetComponent<InputField> ().text),
                ExtensionMethods.WithinIntRange (
                    int.Parse(pokemonRightRegion.transform.FindChild ("Level").GetComponent<InputField> ().text), 1, 100),
                pokemonRightRegion.transform.FindChild ("Item").GetComponent<Dropdown> ().value,
                pokemonRightRegion.transform.FindChild ("Ball").GetComponent<Dropdown> ().value,
                5, 3,
                pokemonRightRegion.transform.FindChild ("Ability").GetComponent<Dropdown> ().value + 1,
                pokemonRightRegion.transform.FindChild ("Gender").GetComponent<Dropdown> ().value, 
				int.Parse (pokemonRightRegion.transform.FindChild ("Form").GetComponent<InputField>().text),
                pokemonRightRegion.transform.FindChild ("Nature").GetComponent<Dropdown> ().value,
                int.Parse (pokemonRightRegion.transform.FindChild ("Happiness").GetComponent<InputField> ().text),
                pokemonRightRegion.transform.FindChild ("Pokerus").GetComponent<Toggle> ().isOn,
                pokemonRightRegion.transform.FindChild ("Shiny").GetComponent<Toggle> ().isOn);

            //Update nickname
            newPokemon.Nickname = pokemonRightRegion.transform.FindChild("Nickname").GetComponent<InputField>().text;

			//Update status
			newPokemon.Status = pokemonRightRegion.transform.FindChild("Status").GetComponent<Dropdown>().value;

            //Update IV
            int[] ivFields = new int[6];
            for (int i = 0; i < 6; i++)
            {
                ivFields [i] = int.Parse (pokemonRightRegion.transform.FindChild ("IV").GetChild (i).GetComponent<InputField> ().text);
            } //end for
            newPokemon.ChangeIVs(ivFields);

            //Update EV
            int[] evFields = new int[6];
            for (int i = 0; i < 6; i++)
            {
                evFields [i] = int.Parse (pokemonRightRegion.transform.FindChild ("EV").GetChild (i).GetComponent<InputField> ().text);
            } //end for
            newPokemon.ChangeEVs(evFields);
            newPokemon.CalculateStats ();

            //Update Ribbons
            for (int i = 0; i < pokemonRightRegion.transform.FindChild ("Ribbons").GetChild(0).childCount; i++)
            {
                if (pokemonRightRegion.transform.FindChild ("Ribbons").GetChild (0).GetChild (i).
                    GetComponent<Toggle> ().isOn)
                {
                    newPokemon.ChangeRibbons (i);
                } //end if
            } //end for

            //Update Moves
            int[] updateMoves = new int[4];
            for (int i = 0; i < 4; i++)
            {
                int value = pokemonRightRegion.transform.FindChild ("Moves").GetChild (i).GetComponent<Dropdown> ().value;
                updateMoves [i] = value > 0 ? value : -1;                
            } //end for
			newPokemon.ChangeMoves(updateMoves);

            //Add to requested spot, or PC if not chosen
            if (GameObject.Find ("LeftRegion").transform.FindChild ("TeamToggle").GetComponent<Toggle> ().isOn)
            {
                GameManager.instance.GetTrainer ().AddPokemon (newPokemon);
                GameManager.instance.DisplayText ("Added " + newPokemon.Nickname + " to your team", true);
            } //end if
            else
            {
                GameManager.instance.GetTrainer ().AddToPC (GameManager.instance.GetTrainer ().GetPCBox (), 0, newPokemon);
                GameManager.instance.DisplayText ("Added " + newPokemon.Nickname + " to your PC", true);
            } //end else
        } //end else
    } //end FinishEditing

	/***************************************
	 * Name: UpdateDebug
	 * Changes fields to represent the 
	 * given Pokemon
	 ***************************************/
	void UpdateDebug(Pokemon myPokemon)
	{
		//Update values to match the pokemon
		pokemonRightRegion.transform.FindChild("Gender").GetComponent<Dropdown>().value = myPokemon.Gender;
		pokemonRightRegion.transform.FindChild("Gender").GetComponent<Dropdown>().RefreshShownValue();
		pokemonRightRegion.transform.FindChild("PokemonName").GetComponent<Dropdown>().value = myPokemon.NatSpecies - 1;
		pokemonRightRegion.transform.FindChild("PokemonName").GetComponent<Dropdown>().RefreshShownValue();
		pokemonRightRegion.transform.FindChild("Level").GetComponent<InputField>().text = myPokemon.CurrentLevel.ToString();
		pokemonRightRegion.transform.FindChild("TrainerID").GetComponent<InputField>().text = myPokemon.TrainerID.ToString();
		pokemonRightRegion.transform.FindChild("Nickname").GetComponent<InputField>().text = myPokemon.Nickname;
		pokemonRightRegion.transform.FindChild("Nature").GetComponent<Dropdown>().value = myPokemon.Nature;
		pokemonRightRegion.transform.FindChild("Nature").GetComponent<Dropdown>().RefreshShownValue();
		pokemonRightRegion.transform.FindChild("Item").GetComponent<Dropdown>().value = myPokemon.Item;
		pokemonRightRegion.transform.FindChild("Item").GetComponent<Dropdown>().RefreshShownValue();
		pokemonRightRegion.transform.FindChild("Ball").GetComponent<Dropdown>().value = myPokemon.BallUsed;
		pokemonRightRegion.transform.FindChild("Ball").GetComponent<Dropdown>().RefreshShownValue();
		pokemonRightRegion.transform.FindChild("Ability").GetComponent<Dropdown>().value = myPokemon.Ability-1;
		pokemonRightRegion.transform.FindChild("Ability").GetComponent<Dropdown>().RefreshShownValue();
		pokemonRightRegion.transform.FindChild("Status").GetComponent<Dropdown>().value = myPokemon.Status;
		pokemonRightRegion.transform.FindChild("Status").GetComponent<Dropdown>().RefreshShownValue();
		pokemonRightRegion.transform.FindChild("Happiness").GetComponent<InputField>().text = myPokemon.Happiness.ToString();
		pokemonRightRegion.transform.FindChild("Form").GetComponent<InputField>().text = myPokemon.FormNumber.ToString();
		pokemonRightRegion.transform.FindChild("Shiny").GetComponent<Toggle>().isOn = myPokemon.IsShiny;
		pokemonRightRegion.transform.FindChild("Pokerus").GetComponent<Toggle>().isOn = myPokemon.HasPokerus;
		pokemonRightRegion.transform.FindChild("IV").GetChild(0).GetComponent<InputField>().text = myPokemon.GetIV(0).ToString();
		pokemonRightRegion.transform.FindChild("IV").GetChild(1).GetComponent<InputField>().text = myPokemon.GetIV(1).ToString();
		pokemonRightRegion.transform.FindChild("IV").GetChild(2).GetComponent<InputField>().text = myPokemon.GetIV(2).ToString();
		pokemonRightRegion.transform.FindChild("IV").GetChild(3).GetComponent<InputField>().text = myPokemon.GetIV(3).ToString();
		pokemonRightRegion.transform.FindChild("IV").GetChild(4).GetComponent<InputField>().text = myPokemon.GetIV(4).ToString();
		pokemonRightRegion.transform.FindChild("IV").GetChild(5).GetComponent<InputField>().text = myPokemon.GetIV(5).ToString();
		pokemonRightRegion.transform.FindChild("EV").GetChild(0).GetComponent<InputField>().text = myPokemon.GetEV(0).ToString();
		pokemonRightRegion.transform.FindChild("EV").GetChild(1).GetComponent<InputField>().text = myPokemon.GetEV(1).ToString();
		pokemonRightRegion.transform.FindChild("EV").GetChild(2).GetComponent<InputField>().text = myPokemon.GetEV(2).ToString();
		pokemonRightRegion.transform.FindChild("EV").GetChild(3).GetComponent<InputField>().text = myPokemon.GetEV(3).ToString();
		pokemonRightRegion.transform.FindChild("EV").GetChild(4).GetComponent<InputField>().text = myPokemon.GetEV(4).ToString();
		pokemonRightRegion.transform.FindChild("EV").GetChild(5).GetComponent<InputField>().text = myPokemon.GetEV(5).ToString();
		pokemonRightRegion.transform.FindChild("Moves").GetChild(0).GetComponent<Dropdown>().value =
			ExtensionMethods.BindToInt(myPokemon.GetMove(0), 0);
		pokemonRightRegion.transform.FindChild("Moves").GetChild(1).GetComponent<Dropdown>().value =
			ExtensionMethods.BindToInt(myPokemon.GetMove(1), 0);
		pokemonRightRegion.transform.FindChild("Moves").GetChild(2).GetComponent<Dropdown>().value =
			ExtensionMethods.BindToInt(myPokemon.GetMove(2), 0);
		pokemonRightRegion.transform.FindChild("Moves").GetChild(3).GetComponent<Dropdown>().value =
			ExtensionMethods.BindToInt(myPokemon.GetMove(3), 0);
		for (int i = 0; i < myPokemon.GetRibbonCount(); i++)
		{
			pokemonRightRegion.transform.FindChild("Ribbons").GetChild(0).GetChild(
				myPokemon.GetRibbon(i)).GetComponent<Toggle>().isOn = true;
		} //end for
		UpdateSprite();
	} //end UpdateDebug(Pokemon myPokemon)

	/***************************************
     * Name: UpdateSprite
     * Changes sprite to reflect name choice
     ***************************************/ 
	public void UpdateSprite()
	{
		//Develop string path to image
		string chosenString = (pokemonRightRegion.transform.FindChild("PokemonName").GetComponent<Dropdown>().value + 1).
			ToString ("000");
		chosenString +=  pokemonRightRegion.transform.FindChild("Gender").GetComponent<Dropdown>().value == 1 ? "f" : "";
		chosenString += pokemonRightRegion.transform.FindChild("Shiny").GetComponent<Toggle>().isOn ? "s" : "";

		//Change sprite, and fix if sprite is null
		pokemonRightRegion.transform.FindChild("Sprite").GetComponent<Image>().sprite = 
			Resources.Load<Sprite>("Sprites/Pokemon/" + chosenString);   
		if( pokemonRightRegion.transform.FindChild("Sprite").GetComponent<Image>().sprite == null)
		{
			chosenString = chosenString.Replace("f", "");
			pokemonRightRegion.transform.FindChild("Sprite").GetComponent<Image>().sprite = 
				Resources.Load<Sprite>("Sprites/Pokemon/" + chosenString);
			if( pokemonRightRegion.transform.FindChild("Sprite").GetComponent<Image>().sprite == null)
			{
				pokemonRightRegion.transform.FindChild("Sprite").GetComponent<Image>().sprite = 
					Resources.Load<Sprite>("Sprites/Pokemon/0");
			} //end if
		} //end if
	} //end UpdateSprite

	/***************************************
     * Name: FillInventory
     * Fills inventory with one of each item
     ***************************************/ 
	public void FillInventory()
	{	
		List<string> list = DataContents.GenerateItemList();	
		for (int i = 1; i < list.Count; i++)
		{
			GameManager.instance.GetTrainer().AddItem(DataContents.GetItemID(list[i]), 1);
		} //end for
		GameManager.instance.DisplayText("Filled inventory", true);
	} //end FillInventory
	#endregion
	#endregion
} //end class MainGameScene