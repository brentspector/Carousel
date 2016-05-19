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
	bool initialize;                //Initialize each state only once per access
	bool processing = false;		//Whether a function is already processing something
	GameObject choices;				//Choices box from scene tools
	GameObject selection;			//Selection rectangle from scene tools
	GameObject buttonMenu;          //Menu of buttons in main game
	GameObject gymBattle;           //Screen of region leader battles
	GameObject playerTeam;          //Screen of the player's team
	GameObject summaryScreen;       //Screen showing summary of data for pokemon
	GameObject ribbonScreen;        //Screen showing ribbons for pokemon
	GameObject trainerCard;         //Screen of the trainer card
	GameObject debugOptions;        //Screen of the debug options
	GameObject debugButtons;        //Screen containing debug areas
	GameObject pokemonRightRegion;  //The right region of Pokemon Edit options
	GameObject trainerRightRegion;  //The right region of the Trainer edit options
	GameObject currentTeamSlot;     //The object that is currently highlighted on the team
	GameObject currentSwitchSlot;   //The object that is currently highlighted for switching to
	GameObject currentMoveSlot;     //The object that is currently highlighted for reading/moving
	GameObject currentRibbonSlot;   //The object that is currently highlighted for reading
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
							if (GameManager.instance.GetTrainer().Team[i - 1].Status == 1)
							{
								playerTeam.transform.FindChild("Background").GetChild(i - 1).GetComponent<Image>().sprite =
									Resources.Load<Sprite>("Sprites/Menus/partyPanelRectFnt");
							} //end if
							//Otherwise give regular slot image
							else
							{
								playerTeam.transform.FindChild("Background").GetChild(i - 1).GetComponent<Image>().sprite =
									Resources.Load<Sprite>("Sprites/Menus/partyPanelRect");
							} //end else

							//Set party ball to unselected
							playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("PartyBall").
							GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/partyBall");
						} //end else

						//Set status
						switch (GameManager.instance.GetTrainer().Team[i - 1].Status)
						{
						//Healthy
							case 0:
								playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("Status").
								GetComponent<Image>().color = Color.clear;
								break;
						//Faint
							case 1:
								playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("Status").
								GetComponent<Image>().color = Color.white;
								playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("Status").
								GetComponent<Image>().sprite = DataContents.statusSprites[5];
								break;
						//Sleep
							case 2:
								playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("Status").
								GetComponent<Image>().color = Color.white;
								playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("Status").
								GetComponent<Image>().sprite = DataContents.statusSprites[0];
								break;
						//Poison
							case 3:
								playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("Status").
								GetComponent<Image>().color = Color.white;
								playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("Status").
								GetComponent<Image>().sprite = DataContents.statusSprites[1];
								break;
						//Burn
							case 4:
								playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("Status").
								GetComponent<Image>().color = Color.white;
								playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("Status").
								GetComponent<Image>().sprite = DataContents.statusSprites[2];
								break;
						//Paralyze
							case 5:
								playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("Status").
								GetComponent<Image>().color = Color.white;
								playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("Status").
								GetComponent<Image>().sprite = DataContents.statusSprites[3];
								break;
						//Freeze
							case 6:
								playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("Status").
								GetComponent<Image>().color = Color.white;
								playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("Status").
								GetComponent<Image>().sprite = DataContents.statusSprites[4];
								break;
						} //end switch

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
					for (int i = 6; i > GameManager.instance.GetTrainer().Team.Count; i--)
					{
						playerTeam.transform.FindChild("Background").GetChild(i - 1).gameObject.SetActive(false);
						playerTeam.transform.FindChild("Pokemon" + (i)).gameObject.SetActive(false);
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
							sprite = Resources.Load<Sprite>("Sprites/Menus/partypanelRectSel");
						} //end if
						else
						{
							playerTeam.transform.FindChild("Background").GetChild(previousTeamSlot - 1).GetComponent<Image>().
							sprite = Resources.Load<Sprite>("Sprites/Menus/partypanelRectSelFnt");
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
					trainerCard.SetActive(false);

					//Fill in card data
					trainerCard.transform.FindChild("Name").GetComponent<Text>().text = "Name: " +
					GameManager.instance.GetTrainer().PlayerName;
					trainerCard.transform.FindChild("Points").GetComponent<Text>().text = "Points: " +
					GameManager.instance.GetTrainer().PlayerPoints;
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

				//Update player time
				trainerCard.transform.FindChild("Playtime").GetComponent<Text>().text =
					"Playtime: " + GameManager.instance.GetTrainer().HoursPlayed.ToString("00") + ":" +
					GameManager.instance.GetTrainer().MinutesPlayed.ToString("00"); 

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
			//Pokemon Summary on Continue Game -> Summary
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
			} //end if Pokemon Summary on Continue Game -> Summary

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
		else if (Input.GetAxis("Mouse X") > 0)
		{

		} //end else if Mouse Moves Left

		/*********************************************
		* Mouse Moves Right
		**********************************************/
		else if (Input.GetAxis("Mouse X") < 0)
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
		choices.transform.GetChild(0).GetComponent<Text>().text = "Summary";
		choices.transform.GetChild(1).GetComponent<Text>().text = "Switch";
		choices.transform.GetChild(2).GetComponent<Text>().text = "Item";
		choices.transform.GetChild(3).GetComponent<Text>().text = "Ribbons";
		choices.transform.GetChild(4).GetComponent<Text>().text = "Cancel";
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
					DataContents.typeSprites[Convert.ToInt32(Enum.Parse(typeof(Types),
					DataContents.ExecuteSQL<string>("SELECT type FROM Moves WHERE rowid=" +
					myPokemon.GetMove(i))))];

				//Set the move name
				moveScreen.FindChild("Move" + (i + 1)).GetChild(1).GetComponent<Text>().text =
					DataContents.ExecuteSQL<string>("SELECT gameName FROM Moves WHERE rowid=" +
					myPokemon.GetMove(i));

				//Set the move PP
				moveScreen.FindChild("Move" + (i + 1)).GetChild(2).GetComponent<Text>().text = "PP " +
					myPokemon.GetMovePP(i).ToString() + "/" + DataContents.ExecuteSQL<string>(
					"SELECT totalPP FROM Moves WHERE rowid=" + myPokemon.GetMove(i));
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

		//Reposition to location of top choice, with 2 unit offset to center it
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
					((ObtainType)pokemonChoice.ObtainType).ToString() + " from " + ((ObtainFrom)pokemonChoice.ObtainFrom).
					ToString();
				summaryScreen.transform.GetChild(1).FindChild("CaughtLevel").GetComponent<Text>().text =
					"Found at level " + pokemonChoice.ObtainLevel;
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
		//        //If text isn't displayed
		//        if ((gameState == MainGame.POKEMONRIBBONS|| pcState == PCGame.POKEMONRIBBONS) && 
		//            ribbonChoice != previousRibbonChoice && selection.activeSelf)
		//        {
		//            //Activate the fields
		//            ribbonScreen.transform.FindChild("RibbonName").gameObject.SetActive(true);
		//            ribbonScreen.transform.FindChild("RibbonDescription").gameObject.SetActive(true);
		//
		//            //Position selection rectangle
		//            selection.transform.position = Camera.main.WorldToScreenPoint(ribbonScreen.transform.
		//                FindChild("RibbonRegion").GetChild(ribbonChoice).GetComponent<RectTransform>().position);
		//
		//            //Get the ribbon value at the index
		//            int ribbonValue;
		//            if(gameState == MainGame.POKEMONRIBBONS)
		//            {
		//                ribbonValue = GameManager.instance.GetTrainer().Team[choiceNumber-1].GetRibbon(ribbonChoice);
		//            } //end if
		//            else
		//            {
		//                ribbonValue = selectedPokemon.GetRibbon(ribbonChoice);
		//            } //end else
		//            //Set the name and description
		//            ribbonScreen.transform.FindChild("RibbonName").GetComponent<Text>().text = 
		//                DataContents.ribbonData.GetRibbonName(ribbonValue);
		//            ribbonScreen.transform.FindChild("RibbonDescription").GetComponent<Text>().text = 
		//                DataContents.ribbonData.GetRibbonDescription(ribbonValue);
		//
		//            //Set the ribbon choice
		//            previousRibbonChoice = ribbonChoice;
		//        } //end if
		//        //Otherwise hide the text
		//        else
		//        {
		//            ribbonScreen.transform.FindChild("RibbonName").gameObject.SetActive(false);
		//            ribbonScreen.transform.FindChild("RibbonDescription").gameObject.SetActive(false);
		//            previousRibbonChoice = -1;
		//        } //end else
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
	 * Name: ChangeCheckpoint
	 * Changes the checkpoint
	 ***************************************/
	public void ChangeCheckpoint(int newCheckpoint)
	{
		checkpoint = newCheckpoint;
	} //end ChangeCheckpoint(int newCheckpoint)
	#endregion
} //end class MainGameScene