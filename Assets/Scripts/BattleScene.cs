﻿/***************************************************************************************** 
 * File:    BattleScene.cs
 * Summary: Handles Process for Battle
 *****************************************************************************************/ 
#region Using
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#endregion

public class BattleScene : MonoBehaviour 
{
	#region Variables
	public enum Battle
	{
		ROUNDSTART,
		SELECTATTACK,
		SELECTITEM,
		SELECTPOKEMON,
		POKEMONSUBMENU,
		POKEMONSUMMARY,
		POKEMONRIBBONS,
		ITEMUSE,
		PICKMOVE,
		MOVESWITCH,
		GETAICHOICE,
		PROCESSQUEUE,
		ENDROUND,
		ENDFIGHT
	} //end Battle

	Battle battleState;				//Current state of the battle scene
	int checkpoint = 0;				//Manage function progress
	int commandInt;					//What choice is being selected on commandChoice
	int choiceNumber;				//What choice is being selected on the relevant screen
	int choiceTarget;				//What is the target of the action
	int battleType;					//Singles, Doubles, Triples, or other type
	int currentAttack;				//What move is currently being used
	int previousTeamSlot;       	//The slot last highlighted
	int topShown;					//The top slot displayed in the inventory
	int bottomShown;				//The bottom slot displayed in the inventory
	int subMenuChoice;          	//What choice is highlighted in the pokemon submenu
	int summaryChoice;              //What page is open on the summary screen
	int moveChoice;                 //What move is being highlighted for details
	int switchChoice;				//Currently selected move to switch to
	int detailsSize;                //Font size for move description
	int ribbonChoice;               //The ribbon currently shown
	int previousRibbonChoice;       //The ribbon last highlighted for reading
	bool processing = false;		//Whether a function is already processing something
	bool pickMove = false;			//Whether the player must pick a move to use an item on
	bool pocketChange;				//Is the pocket currently changing
	Field battleField;				//The active battle field
	PokemonBattler currentAttacker;	//Who is currently attacking
	PokemonBattler lastAttacker;	//Who was the last pokemon to attack
	List<Pokemon> originalOrder;	//Restores the team to the order it was before battle
	List<Pokemon> participants;		//The pokemon that participated in the fight
	List<List<int>> fieldEffects;	//Effects that are present on the field
	List<PokemonBattler> battlers;	//A list of the battling spots on the field
	List<GameObject> battlerBoxes;	//List of each box that represents an active pokemon
	List<GameObject> trainerStands; //List of places the trainer, pokemon, and base images are
	List<GameObject> partyLineups;	//List of parties for trainers
	List<Trainer> combatants;		//Lists the trainers participating in the battle
	List<QueueEvent> queue;			//Lists the actions to be performed
	Image bagBack;					//Background bag image
	Image bagSprite;				//Item currently highlighted
	Text bagDescription;			//The item's description
	Text messageBox;				//Commentary section for the battle
	GameObject attackSelection;		//Contains all attacks player can choose for the active pokemon
	GameObject commandChoice;		//Contains all the options the player can conduct in battle
	GameObject selectedChoice;		//The command or selection currently chosen
	GameObject playerTeam;			//Screen showing party of pokemon
	GameObject summaryScreen;       //Screen showing summary of data for pokemon
	GameObject ribbonScreen;        //Screen showing ribbons for pokemon
	GameObject playerBag;			//Screen of the player's bag
	GameObject viewport;			//The items shown to the player
	GameObject choices;				//Choices box from scene tools
	GameObject selection;			//Selection rectangle from scene tools
	GameObject currentRibbonSlot;   //The object that is currently highlighted for reading
	GameObject currentSwitchSlot;	//The move currently being selected to switch to
	#endregion

	#region Methods
	/***************************************
	 * Name: RunBattle
	 * Play the battle scene
	 ***************************************/
	public void RunBattle()
	{
		//Initialize scene variables
		if (checkpoint == 0)
		{
			//Set checkpoint delegate
			GameManager.instance.checkDel = ChangeCheckpoint;

			//Set confirm delegate
			GameManager.instance.confirmDel = ApplyConfirm;

			//Initialize references
			battleState = Battle.ROUNDSTART;
			commandInt = 0;
			choiceNumber = 0;
			currentAttack = -1;
			detailsSize = -1;
			battleField = new Field(0);
			currentAttacker = null;
			lastAttacker = null;
			participants = new List<Pokemon>();
			fieldEffects = new List<List<int>>();
			battlers = new List<PokemonBattler>();
			battlerBoxes = new List<GameObject>();
			trainerStands = new List<GameObject>();
			partyLineups = new List<GameObject>();
			queue = new List<QueueEvent>();
			messageBox = GameObject.Find("MessageBox").GetComponentInChildren<Text>();
			attackSelection = GameObject.Find("AttackSelection");
			commandChoice = GameObject.Find("CommandChoice");
			selectedChoice = commandChoice.transform.GetChild(0).gameObject;
			playerTeam = GameObject.Find("Team").gameObject;
			summaryScreen = playerTeam.transform.FindChild("Summary").gameObject;
			ribbonScreen = playerTeam.transform.FindChild("Ribbons").gameObject;
			playerBag = GameObject.Find("PlayerBag");
			viewport = playerBag.transform.FindChild("InventoryRegion").gameObject;
			bagBack = playerBag.transform.FindChild("BagBack").GetComponent<Image>();
			bagSprite = playerBag.transform.FindChild("BagSprite").GetComponent<Image>();
			bagDescription = playerBag.transform.FindChild("BagDescription").GetComponent<Text>();
			selection = GameManager.tools.transform.FindChild("Selection").gameObject;
			choices = GameManager.tools.transform.FindChild("ChoiceUnit").gameObject;

			//Initialize effects lists
			for (int i = 0; i < combatants.Count; i++)
			{
				fieldEffects.Add(new List<int>());
				for (int j = 0; j < (int)FieldEffects.COUNT; j++)
				{
					fieldEffects[i].Add(0);
				} //end for
			} //end for

			//Initialize battlers
			for (int i = 0; i < combatants.Count; i++)
			{
				battlers.Add(new PokemonBattler(combatants[i].Team[0]));
				participants.Add(combatants[i].Team[0]);
			} //end for

			//Initialize battler boxes
			battlerBoxes.Add(GameObject.Find("PlayerBox"));
			battlerBoxes.Add(GameObject.Find("FoeBox"));

			//Initialize player stands
			trainerStands.Add(GameObject.Find("PlayerSide"));
			trainerStands.Add(GameObject.Find("FoeSide"));

			//Initialize party lineups
			partyLineups.Add(GameObject.Find("PlayerPartyLineup"));
			partyLineups.Add(GameObject.Find("FoePartyLineup"));

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

			//Set selection font to black
			commandChoice.transform.GetChild(commandInt).GetChild(0).GetComponent<Text>().color = Color.black;

			//Turn off command choice and attack selection
			commandChoice.SetActive(false);
			attackSelection.SetActive(false);

			//Set starting message
			WriteBattleMessage("The battle between " + combatants[0].PlayerName + " and " + combatants[1].PlayerName + " is starting!");

			//Set trainer images
			trainerStands[0].transform.FindChild("Trainer").GetComponent<Image>().sprite =
				DataContents.trainerBacks[combatants[0].PlayerImage * 5];
			trainerStands[1].transform.FindChild("Trainer").GetComponent<Image>().sprite =
				DataContents.leaderSprites[combatants[1].PlayerImage];

			//Set player party lineup
			for (int i = 0; i < combatants[0].Team.Count; i++)
			{
				if (combatants[0].Team[i].Status == (int)Status.FAINT)
				{
					partyLineups[0].transform.GetChild(i + 1).GetComponent<Image>().sprite = Resources.
						Load<Sprite>("Sprites/Battle/ballfainted");
				} //end if
				else if (combatants[0].Team[i].Status == (int)Status.HEALTHY)
				{
					partyLineups[0].transform.GetChild(i + 1).GetComponent<Image>().sprite = Resources.
						Load<Sprite>("Sprites/Battle/ballnormal");
				} //end else if
				else
				{
					partyLineups[0].transform.GetChild(i + 1).GetComponent<Image>().sprite = Resources.
						Load<Sprite>("Sprites/Battle/ballstatus");
				} //end else
			} //end for

			//Set foe party lineup
			for (int i = 0; i < combatants[1].Team.Count; i++)
			{
				partyLineups[1].transform.GetChild(i + 1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Battle/ballnormal");
			} //end for

			//Fill in sprites and battler box data
			FillInBattlerData();

			//Fill in all team data
			UpdateDisplayedTeam();

			//Turn off team
			summaryScreen.SetActive(false);
			ribbonScreen.SetActive(false);
			playerTeam.SetActive(false);

			//Turn off bag
			playerBag.SetActive(false);

			//Move to next checkpoint
			GameManager.instance.OpenScene(2);
		} //end else if
		else if (checkpoint == 2)
		{	
			//Get player input
			GetInput();
		} //end else if
		else if (checkpoint == 3)
		{
			//Return if processing
			if (processing)
			{
				return;
			} //end if

			//Begin processing
			processing = true;

			//Check field effect, abilities, and items
			StartCoroutine(ResolveFieldEntrance());
		} //end else if
		else if (checkpoint == 4)
		{
			if (battleState == Battle.ROUNDSTART)
			{
				//Turn on command choice screen
				commandChoice.SetActive(true);

				//Reset choiceNumber to zero
				choiceNumber = 0;

				//Reset choiceTarget to zero
				choiceTarget = 0;

				//Reset processing to false
				processing = false;

				//Get player input
				GetInput();
			} //end if
			else if (battleState == Battle.SELECTATTACK)
			{
				//Get player input
				GetInput();
			} //end else if
			else if (battleState == Battle.SELECTITEM)
			{
				//Get player input
				GetInput();

				//Change background
				int currentPocket = combatants[0].GetCurrentBagPocket();
				bagBack.sprite = Resources.Load<Sprite>("Sprites/Menus/bag" + currentPocket);

				//Fill in slots
				for (int i = 0; i < 10; i++)
				{
					if (combatants[0].SlotCount() - 1 < topShown + i)
					{
						viewport.transform.GetChild(i).GetComponent<Text>().text = "";
					} //end if
					else
					{
						if (topShown + i == choiceNumber)
						{
							viewport.transform.GetChild(i).GetComponent<Text>().text = "<color=red>" +
							combatants[0].GetItem(topShown + i)[1] + " - " +
							DataContents.GetItemGameName(combatants[0].GetItem(topShown + i)[0]) + "</color>";
						} //end if
						else
						{
							viewport.transform.GetChild(i).GetComponent<Text>().text = combatants[0].GetItem(topShown + i)[1]
							+ " - " + DataContents.GetItemGameName(combatants[0].GetItem(topShown + i)[0]);
						} //end else
					} //end else
				} //end for

				//Fill in sprite and description
				if (combatants[0].SlotCount() != 0)
				{
					List<int> item = combatants[0].GetItem(choiceNumber);
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
			else if (battleState == Battle.SELECTPOKEMON)
			{
				//Change background sprites based on player input
				if (previousTeamSlot != choiceNumber)
				{
					//Update backgrounds
					FillInTeam(choiceNumber);

					//Update previous slot
					previousTeamSlot = choiceNumber;
				} //end if

				//Get player input
				GetInput();
			} //end else if
			else if (battleState == Battle.POKEMONSUBMENU)
			{
				//Get player input
				GetInput();
			} //end else if
			else if (battleState == Battle.POKEMONSUMMARY)
			{
				//Get player input
				GetInput();

				//Fill in the summary screen with the correct data
				PokemonSummary(combatants[0].Team[choiceNumber - 1]);
			} //end else if
			else if (battleState == Battle.ITEMUSE)
			{
				//Change background sprites based on player input
				if (previousTeamSlot != choiceTarget)
				{
					//Update backgrounds
					FillInTeam(choiceTarget);

					//Update previous slot
					previousTeamSlot = choiceTarget;
				} //end if

				//Get player input
				GetInput();
			} //end else if
			else if (battleState == Battle.PICKMOVE)
			{
				//Get player input
				GetInput();
			} //end else if
			else if (battleState == Battle.MOVESWITCH)
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
			else if (battleState == Battle.POKEMONRIBBONS)
			{
				//Get player input
				GetInput();			
			} //end else if
			else if (battleState == Battle.GETAICHOICE)
			{
				//Select a random attack
				int randomSelection = GameManager.instance.RandomInt(0, battlers[1].BattlerPokemon.GetMoveCount()-1);

				//Queue it
				AddToQueue(1, 0, randomSelection, 0, DeterminePriority(1, 0, randomSelection));

				//Organize list
				SortQueue();

				//Move to process
				battleState = Battle.PROCESSQUEUE;
			} //end else if
			else if (battleState == Battle.PROCESSQUEUE)
			{
				//Return if processing
				if (processing)
				{
					return;
				} //end if

				//Begin processing
				processing = true;

				//Process queue
				StartCoroutine(ProcessQueue());
			} //end if
			/* Done - Start of Battle
			 * ++++++-Display trainers
			 * ++++++-AI sends out first pokemon in roster
			 * ++++++-Player sends out first pokemon in roster
			 * 
			 * Done - On Entry
			 * ++++++-Resolve field
			 * ++++++-Resolve ability (intimidate,levitate,frisk)
			 * ++++++-Resolve item(air balloon)
			 * 
			 * Beginning of round
			 * ++++++-Player picks Fight/Bag/Pokemon/Run
			 * ++++++--Fight
			 * ++++++----Display pokemon attacks
			 * ++++++----Player picks an attack or cancels back to main
			 * ++++++--Bag
			 * ++++++----Display inventory
			 * ++++++----Player picks an item with a battle use
			 * ++++++----Player picks pokemon to use it on
			 * ++++++--Pokemon
			 * ++++++----Team roster is shown
			 * ++++++----Player picks a pokemon
			 * ++++++----If switch is chosen, queue it
			 * ++++++----Player can view summary or cancel back to main
			 * ++++++--Run
			 * ++++++----End the fight and return any variables to start
			 * ++++++----Return to battle menu
			 * -AI picks Fight/Item/Switch
			 * 
			 * Done - End of Choice
			 * ++++++-Queue events
			 * ++++++--Switch (fastest goes first)
			 * ++++++---Resolve OnEntry
			 * ++++++--Item (fastest goes first)
			 * ++++++--Attack
			 * ++++++---Sort by Priority, then Speed
			 * ++++++---Queue with flag indentifying the pokemon who used it
			 * 
			 * Middle of round
			 * -Resolve Queue
			 * --Attack
			 * ++++++----Resolve protect
			 * ++++++----Resolve blocking abilities (Bulletproof & Telepathy)
			 * ++++++----Resolve typing
			 * ++++++----Resolve accuracy
			 * ----Resolve critical
			 * ----Resolve damage, item, ability, field
			 * ----Resolve recoil
			 * ----Check if either pokemon is fainted
			 * -----Resolve ability (aftermath)
			 * -----Resolve player targeted secondary effects
			 * -----If opponent is still alive, process opponent secondary effects
			 * --Loop until queue is empty
			 * 
			 * End of round
			 * -Resolve field & item
			 * -Replace missing pokemon
			 * -Loop to beginning of round
			 * 
			 * End of Battle
			 * -Give badge if needed
			 * -Give points
			 * -Give item
			 */

		} //end else if
	} //end RunBattle

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
			//Regular processing
			if (checkpoint == 4)
			{
				//Command choice
				if (battleState == Battle.ROUNDSTART)
				{
					//Set font color for current option to white
					commandChoice.transform.GetChild(commandInt).GetChild(0).GetComponent<Text>().color = Color.white;

					//Change selection
					commandInt--;
					if (commandInt < 0)
					{
						commandInt = 3;
					} //end if

					//Set new selection font to black
					commandChoice.transform.GetChild(commandInt).GetChild(0).GetComponent<Text>().color = Color.black;

					//Update selection reference
					selectedChoice = commandChoice.transform.GetChild(commandInt).gameObject;
				} //end if

				//Attack selection
				else if (battleState == Battle.SELECTATTACK)
				{
					//Change current sprite to non selected
					attackSelection.transform.GetChild(choiceNumber).GetComponent<Image>().sprite = DataContents.attackNonSelSprites[
						Array.IndexOf(DataContents.attackSelSprites, attackSelection.transform.GetChild(choiceNumber).
							GetComponent<Image>().sprite)];

					//Change selection
					choiceNumber--;
					if (choiceNumber < 0)
					{
						choiceNumber = battlers[0].BattlerPokemon.GetMoveCount() - 1;
					} //end if

					//Change new sprite to selected
					attackSelection.transform.GetChild(choiceNumber).GetComponent<Image>().sprite = DataContents.attackSelSprites[
						Array.IndexOf(DataContents.attackNonSelSprites, attackSelection.transform.GetChild(choiceNumber).
							GetComponent<Image>().sprite)];

					//Update PP for new selection
					attackSelection.transform.GetChild(4).GetComponent<Text>().text = "PP\n" + battlers[0].BattlerPokemon.
						GetMovePP(choiceNumber) + "/" + battlers[0].BattlerPokemon.GetMovePPMax(choiceNumber);

					//Update selection reference
					selectedChoice = attackSelection.transform.GetChild(choiceNumber).gameObject;
				} //end else if

				//Inventory selection
				else if (battleState == Battle.SELECTITEM)
				{
					combatants[0].PreviousPocket();
					choiceNumber = 0;
					bottomShown = 9;
					topShown = 0;
					GameManager.instance.FadeInAnimation(4);
				} //end else if

				//Item use
				else if (battleState == Battle.ITEMUSE)
				{
					//Decrease (higher slots are on lower children)
					choiceTarget--;

					//Loop to end of team if on top slot
					if (choiceTarget < 1)
					{
						choiceTarget = combatants[0].Team.Count;
					} //end if

					//Set current slot choice if on pokemon slot
					selectedChoice = playerTeam.transform.FindChild("Pokemon" + choiceTarget).gameObject;
				} //end else if

				//Pokemon selection
				else if (battleState == Battle.SELECTPOKEMON)
				{
					//Decrease (higher slots are on lower children)
					choiceNumber--;

					//Loop to end of team if out of pokemon choice range
					if (choiceNumber < 1)
					{
						choiceNumber = combatants[0].Team.Count;
					} //end if

					//Set current slot choice
					selectedChoice = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
				} //end else if

				//Pokemon summary
				else if (battleState == Battle.POKEMONSUMMARY)
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
				} //end else if

				//Pokemon ribbons
				else if (battleState == Battle.POKEMONRIBBONS)
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
				} //end else if
			} //end if
		} //end if Left Arrow

		/*********************************************
		* Right Arrow
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			//Regular processing
			if(checkpoint == 4)
			{
				//Command choice
				if (battleState == Battle.ROUNDSTART)
				{
					//Set font color for current option to white
					commandChoice.transform.GetChild(commandInt).GetChild(0).GetComponent<Text>().color = Color.white;
				
					//Change selection
					commandInt++;
					if (commandInt > 3)
					{
						commandInt = 0;
					} //end if
				
					//Set new selection font to black
					commandChoice.transform.GetChild(commandInt).GetChild(0).GetComponent<Text>().color = Color.black;
				
					//Update selection reference
					selectedChoice = commandChoice.transform.GetChild(commandInt).gameObject;
				} //end if
				
				//Attack selection
				else if (battleState == Battle.SELECTATTACK)
				{
					//Change current sprite to non selected
					attackSelection.transform.GetChild(choiceNumber).GetComponent<Image>().sprite = DataContents.attackNonSelSprites[
						Array.IndexOf(DataContents.attackSelSprites, attackSelection.transform.GetChild(choiceNumber).
							GetComponent<Image>().sprite)];
				
					//Change selection
					choiceNumber++;
					if (choiceNumber > battlers[0].BattlerPokemon.GetMoveCount() - 1)
					{
						choiceNumber = 0;
					} //end if
				
					//Change new sprite to selected
					attackSelection.transform.GetChild(choiceNumber).GetComponent<Image>().sprite = DataContents.attackSelSprites[
						Array.IndexOf(DataContents.attackNonSelSprites, attackSelection.transform.GetChild(choiceNumber).
							GetComponent<Image>().sprite)];
				
					//Update PP for new selection
					attackSelection.transform.GetChild(4).GetComponent<Text>().text = "PP\n" + battlers[0].BattlerPokemon.
						GetMovePP(choiceNumber) + "/" + battlers[0].BattlerPokemon.GetMovePPMax(choiceNumber);
					
					//Update selection reference
					selectedChoice = attackSelection.transform.GetChild(choiceNumber).gameObject;
				} //end else if
				
				//Inventory selection
				else if (battleState == Battle.SELECTITEM)
				{
					combatants[0].NextPocket();
					choiceNumber = 0;
					bottomShown = 9;
					topShown = 0;
					GameManager.instance.FadeInAnimation(4);
				} //end else if

				//Item use
				else if (battleState == Battle.ITEMUSE)
				{
					//Increase (lower slots are on higher children)
					choiceTarget++;

					//Loop to end of team if on bottom slot
					if (choiceTarget > combatants[0].Team.Count)
					{
						choiceTarget = 1;
					} //end if

					//Set current slot choice if on pokemon slot
					selectedChoice = playerTeam.transform.FindChild("Pokemon" + choiceTarget).gameObject;
				} //end else if

				//Pokemon selection
				else if (battleState == Battle.SELECTPOKEMON)
				{
					//Increase (lower slots are on higher children)
					choiceNumber++;

					//Loop to end of team if outside of pokemon range
					if (choiceNumber > combatants[0].Team.Count)
					{
						choiceNumber = 1;
					} //end if

					//Set current slot choice
					selectedChoice = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
				} //end else if

				//Pokemon summary
				else if (battleState == Battle.POKEMONSUMMARY)
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
				} //end else if

				//Pokemon ribbons
				else if (battleState == Battle.POKEMONRIBBONS)
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
				} //end else if
			} //end if
		} //end else if Right Arrow

		/*********************************************
		* Up Arrow
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			//Regular processing
			if(checkpoint == 4)
			{
				//Command choice
				if (battleState == Battle.ROUNDSTART)
				{
					//Set font color for current option to white
					commandChoice.transform.GetChild(commandInt).GetChild(0).GetComponent<Text>().color = Color.white;
				
					//Change selection
					commandInt -= 2;
					if (commandInt < 0)
					{
						commandInt += 4;
					} //end if
				
					//Set new selection font to black
					commandChoice.transform.GetChild(commandInt).GetChild(0).GetComponent<Text>().color = Color.black;
				
					//Update selection reference
					selectedChoice = commandChoice.transform.GetChild(commandInt).gameObject;
				} //end if
				
				//Attack selection
				else if (battleState == Battle.SELECTATTACK)
				{
					//Change current sprite to non selected
					attackSelection.transform.GetChild(choiceNumber).GetComponent<Image>().sprite = DataContents.attackNonSelSprites[
						Array.IndexOf(DataContents.attackSelSprites, attackSelection.transform.GetChild(choiceNumber).
							GetComponent<Image>().sprite)];
				
					//Change selection
					choiceNumber -= 2;
					if (choiceNumber < 0)
					{
						choiceNumber += 2;
					} //end if
				
					//Change new sprite to selected
					attackSelection.transform.GetChild(choiceNumber).GetComponent<Image>().sprite = DataContents.attackSelSprites[
						Array.IndexOf(DataContents.attackNonSelSprites, attackSelection.transform.GetChild(choiceNumber).
							GetComponent<Image>().sprite)];
				
					//Update PP for new selection
					attackSelection.transform.GetChild(4).GetComponent<Text>().text = "PP\n" + battlers[0].BattlerPokemon.
						GetMovePP(choiceNumber) + "/" + battlers[0].BattlerPokemon.GetMovePPMax(choiceNumber);
					
					//Update selection reference
					selectedChoice = attackSelection.transform.GetChild(choiceNumber).gameObject;
				} //end else if
				
				//Inventory selection
				else if (battleState == Battle.SELECTITEM)
				{
					choiceNumber = ExtensionMethods.BindToInt(choiceNumber - 1, 0);
					if (choiceNumber < topShown)
					{
						topShown = choiceNumber;
						bottomShown--;
					} //end if
				} //end else if

				//Item use
				else if (battleState == Battle.ITEMUSE)
				{
					//Move from top slot to last team slot
					if (choiceTarget == 1 || choiceTarget == 2)
					{
						choiceTarget = combatants[0].Team.Count;
						selectedChoice = playerTeam.transform.FindChild("Pokemon" + choiceTarget).gameObject;
					} //end if
					//Go up vertically
					else
					{
						choiceTarget -= 2;
						selectedChoice = playerTeam.transform.FindChild("Pokemon" + choiceTarget).gameObject;
					} //end else
				} //end else if

				//Pick move
				else if (battleState == Battle.PICKMOVE)
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

				//Pokemon selection
				else if (battleState == Battle.SELECTPOKEMON)
				{
					//Move from top slot to last pokemon
					if (choiceNumber == 1 || choiceNumber == 2)
					{
						choiceNumber = combatants[0].Team.Count;
					} //end else if
					//Go up vertically
					else
					{
						choiceNumber -= 2;
					} //end else

					//Set current slot choice
					selectedChoice = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
				} //end else if

				//Pokemon submenu
				else if (battleState == Battle.POKEMONSUBMENU)
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

				//Pokemon summary
				else if (battleState == Battle.POKEMONSUMMARY)
				{
					//If on any page besides details
					if (summaryChoice != 5)
					{
						//Decrease (Higher slots are on lower children)
						choiceNumber--;

						//Loop to end if on first member
						if (choiceNumber < 1)
						{
							choiceNumber = combatants[0].Team.Count;
						} //end if

						//Update selected pokemon 
						selectedChoice = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
					} //end else if
					else
					{
						//Decrease (higher slots are on lower children)
						moveChoice--;

						//Loop to last move if on first move
						if (moveChoice < 0)
						{
							moveChoice = combatants[0].Team[choiceNumber - 1].GetMoveCount() - 1;
						} //end if

						//Set move slot
						selectedChoice = summaryScreen.transform.GetChild(5).FindChild("Move" + (moveChoice + 1)).gameObject;
					} //end else
				} //end else if

				//Pokemon move switch
				else if (battleState == Battle.MOVESWITCH)
				{
					//Decrease (higher slots are on lower children)
					switchChoice--;

					//Loop to end if on first move
					if (switchChoice < 0)
					{
						switchChoice = combatants[0].Team[choiceNumber - 1].GetMoveCount() - 1;
					} //end if

					//Set current switch slot
					currentSwitchSlot = summaryScreen.transform.GetChild(5).FindChild("Move" + (switchChoice + 1)).gameObject;
				} //end else if

				//Pokemon ribbons
				else if (battleState == Battle.POKEMONRIBBONS)
				{
					//Decrease (higher slots are on lower children)
					choiceNumber--;

					//Loop to end if on first member
					if (choiceNumber < 1)
					{
						choiceNumber = GameManager.instance.GetTrainer().Team.Count;
					} //end if

					//Update selected pokemon 
					selectedChoice = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;

					//Reload ribbons
					InitializeRibbons();
				} //end else if
			} //end if
		} //end else if Up Arrow

		/*********************************************
		* Down Arrow
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			//Regular processing
			if(checkpoint == 4)
			{
				//Command choice
				if (battleState == Battle.ROUNDSTART)
				{
					//Set font color for current option to white
					commandChoice.transform.GetChild(commandInt).GetChild(0).GetComponent<Text>().color = Color.white;
				
					//Change selection
					commandInt += 2;
					if (commandInt > 3)
					{
						commandInt -= 4;
					} //end if
				
					//Set new selection font to black
					commandChoice.transform.GetChild(commandInt).GetChild(0).GetComponent<Text>().color = Color.black;
				
					//Update selection reference
					selectedChoice = commandChoice.transform.GetChild(commandInt).gameObject;
				} //end if
				
				//Attack selection
				else if (battleState == Battle.SELECTATTACK)
				{
					//Change current sprite to non selected
					attackSelection.transform.GetChild(choiceNumber).GetComponent<Image>().sprite = DataContents.attackNonSelSprites[
						Array.IndexOf(DataContents.attackSelSprites, attackSelection.transform.GetChild(choiceNumber).
							GetComponent<Image>().sprite)];
				
					//Change selection
					choiceNumber += 2;
					if (choiceNumber > battlers[0].BattlerPokemon.GetMoveCount() - 1)
					{
						choiceNumber -= 2;
					} //end if
				
					//Change new sprite to selected
					attackSelection.transform.GetChild(choiceNumber).GetComponent<Image>().sprite = DataContents.attackSelSprites[
						Array.IndexOf(DataContents.attackNonSelSprites, attackSelection.transform.GetChild(choiceNumber).
							GetComponent<Image>().sprite)];
				
					//Update PP for new selection
					attackSelection.transform.GetChild(4).GetComponent<Text>().text = "PP\n" + battlers[0].BattlerPokemon.
						GetMovePP(choiceNumber) + "/" + battlers[0].BattlerPokemon.GetMovePPMax(choiceNumber);
					
					//Update selection reference
					selectedChoice = attackSelection.transform.GetChild(choiceNumber).gameObject;
				} //end else if
				
				//Inventory selection
				else if (battleState == Battle.SELECTITEM)
				{
					choiceNumber = ExtensionMethods.CapAtInt(choiceNumber + 1, combatants[0].SlotCount() - 1);
					if (choiceNumber > bottomShown)
					{
						bottomShown = choiceNumber;
						topShown++;
					} //end if
				} //end else if

				//Item use
				else if (battleState == Battle.ITEMUSE)
				{
					//Move from bottom slot to first team slot
					if ((choiceTarget == combatants[0].Team.Count - 1 && choiceTarget > 0) ||
					    choiceTarget == combatants[0].Team.Count)
					{
						choiceTarget = 1;
						selectedChoice = playerTeam.transform.FindChild("Pokemon" + choiceTarget).gameObject;
					} //end if
					//Go down vertically
					else
					{
						choiceTarget += 2;
						selectedChoice = playerTeam.transform.FindChild("Pokemon" + choiceTarget).gameObject;
					} //end else
				} //end else if

				//Pick move
				else if (battleState == Battle.PICKMOVE)
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

				//Pokemon selection
				else if (battleState == Battle.SELECTPOKEMON)
				{
					//Move from bottom slot to first pokemon
					if ((choiceNumber == combatants[0].Team.Count - 1 && choiceNumber > 0) ||
					    choiceNumber == combatants[0].Team.Count)
					{
						choiceNumber = 1;
					} //end else if
					//Go down vertically
					else
					{
						choiceNumber += 2;
					} //end else

					//Set current slot choice
					selectedChoice = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
				} //end else if

				//Pokemon submenu
				else if (battleState == Battle.POKEMONSUBMENU)
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

				//Pokemon summary
				else if (battleState == Battle.POKEMONSUMMARY)
				{
					//If on any page besides details
					if (summaryChoice != 5)
					{
						//Increase (lower slots are on higher children)
						choiceNumber++;

						//Loop to front if on last member
						if (choiceNumber > combatants[0].Team.Count)
						{
							choiceNumber = 1;
						} //end if

						//Update selected pokemon 
						selectedChoice = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
					} //end else if
					else
					{
						//Increase (lower slots are on higher children)
						moveChoice++;

						//If on last move, loop to front
						if (moveChoice >= combatants[0].Team[choiceNumber - 1].GetMoveCount())
						{
							moveChoice = 0;
						} //end if

						//Set move slot
						selectedChoice = summaryScreen.transform.GetChild(5).FindChild("Move" + (moveChoice + 1)).gameObject;
					} //end else
				} //end else if

				//Pokemon move switch
				else if (battleState == Battle.MOVESWITCH)
				{
					//Increase (lower slots are on higher children)
					switchChoice++;

					//Loop to end if on first move
					if (switchChoice >= combatants[0].Team[choiceNumber - 1].GetMoveCount())
					{
						switchChoice = 0;
					} //end if

					//Set current switch slot
					currentSwitchSlot = summaryScreen.transform.GetChild(5).FindChild("Move" + (switchChoice + 1)).gameObject;
				} //end else if

				//Pokemon ribbons
				else if (battleState == Battle.POKEMONRIBBONS)
				{
					//Increase (lower slots are on higher children)
					choiceNumber++;

					//Loop to front if on last member
					if (choiceNumber > GameManager.instance.GetTrainer().Team.Count)
					{
						choiceNumber = 1;
					} //end if

					//Update selected pokemon 
					selectedChoice = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;

					//Reload ribbons
					InitializeRibbons();
				} //end else if
			} //end if
		} //end else if Down Arrow

		/*********************************************
		* Mouse Moves Left
		**********************************************/
		else if (Input.GetAxis("Mouse X") < 0)
		{
			//Regular processing
			if(checkpoint == 4)
			{
				//Command choice
				if (battleState == Battle.ROUNDSTART && Input.mousePosition.x < Camera.main.WorldToScreenPoint(selectedChoice.transform.position).x -
				    selectedChoice.GetComponent<RectTransform>().rect.width / 2)
				{
					//Move left only if on an odd choice
					if (commandInt % 2 == 1)
					{
						//Set font color for current option to white
						commandChoice.transform.GetChild(commandInt).GetChild(0).GetComponent<Text>().color = Color.white;
				
						//Change selection
						commandInt--;
				
						//Set new selection font to black
						commandChoice.transform.GetChild(commandInt).GetChild(0).GetComponent<Text>().color = Color.black;
				
						//Update selection reference
						selectedChoice = commandChoice.transform.GetChild(commandInt).gameObject;
					} //end if
				} //end if
				
				//Attack selection
				else if (battleState == Battle.SELECTATTACK && Input.mousePosition.x < Camera.main.WorldToScreenPoint(
					         selectedChoice.transform.position).x - selectedChoice.GetComponent<RectTransform>().rect.width / 2)
				{
					//Move left only if on an odd choice
					if (choiceNumber % 2 == 1)
					{
						//Change current sprite to non selected
						attackSelection.transform.GetChild(choiceNumber).GetComponent<Image>().sprite = DataContents.attackNonSelSprites[
							Array.IndexOf(DataContents.attackSelSprites, attackSelection.transform.GetChild(choiceNumber).
								GetComponent<Image>().sprite)];
				
						//Change selection
						choiceNumber--;
				
						//Change new sprite to selected
						attackSelection.transform.GetChild(choiceNumber).GetComponent<Image>().sprite = DataContents.attackSelSprites[
							Array.IndexOf(DataContents.attackNonSelSprites, attackSelection.transform.GetChild(choiceNumber).
								GetComponent<Image>().sprite)];
				
						//Update PP for new selection
						attackSelection.transform.GetChild(4).GetComponent<Text>().text = "PP\n" + battlers[0].BattlerPokemon.
							GetMovePP(choiceNumber) + "/" + battlers[0].BattlerPokemon.GetMovePPMax(choiceNumber);
						
						//Update selection reference
						selectedChoice = attackSelection.transform.GetChild(choiceNumber).gameObject;
					} //end if
				} //end else if

				//Pokemon selection
				else if (battleState == Battle.SELECTPOKEMON && Input.mousePosition.x < Camera.main.WorldToScreenPoint(
					         selectedChoice.transform.position).x - selectedChoice.GetComponent<RectTransform>().rect.width / 2)
				{
					//If choice number is not odd, and is greater than 0, move left
					if ((choiceNumber & 1) != 1 && choiceNumber > 0)
					{
						choiceNumber--;
						selectedChoice = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
					} //end if
				} //end else if

				//Item use
				else if (battleState == Battle.ITEMUSE && Input.mousePosition.x < Camera.main.WorldToScreenPoint(
					         selectedChoice.transform.position).x - selectedChoice.GetComponent<RectTransform>().rect.width / 2)
				{
					//If choice target is not odd, and is greater than 0, move left
					if ((choiceTarget & 1) != 1 && choiceTarget > 0)
					{
						choiceTarget--;
						selectedChoice = playerTeam.transform.FindChild("Pokemon" + choiceTarget).gameObject;
					} //end if
				} //end else if

				//Pokemon ribbons
				else if (battleState == Battle.POKEMONRIBBONS && Input.mousePosition.x < Camera.main.WorldToScreenPoint(
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
				} //end else if
			} //end if
		} //end else if Mouse Moves Left

		/*********************************************
		* Mouse Moves Right
		**********************************************/
		if (Input.GetAxis("Mouse X") > 0)
		{
			//Regular processing
			if(checkpoint == 4)
			{
				//Command choice
				if (battleState == Battle.ROUNDSTART && Input.mousePosition.x > Camera.main.WorldToScreenPoint(selectedChoice.transform.position).x +
					selectedChoice.GetComponent<RectTransform>().rect.width/2)
				{
					//Move right only if on an even choice
					if (commandInt%2 == 0)
					{
						//Set font color for current option to white
						commandChoice.transform.GetChild(commandInt).GetChild(0).GetComponent<Text>().color = Color.white;
				
						//Change selection
						commandInt++;
				
						//Set new selection font to black
						commandChoice.transform.GetChild(commandInt).GetChild(0).GetComponent<Text>().color = Color.black;
				
						//Update selection reference
						selectedChoice = commandChoice.transform.GetChild(commandInt).gameObject;
					} //end if
				} //end if
				
				//Attack selection
				else if (battleState == Battle.SELECTATTACK && Input.mousePosition.x > Camera.main.WorldToScreenPoint(selectedChoice.transform.position).x +
					selectedChoice.GetComponent<RectTransform>().rect.width/2)
				{
					//Move right only if on an even choice
					if (choiceNumber%2 == 0 && choiceNumber < battlers[0].BattlerPokemon.GetMoveCount() - 1)
					{
						//Change current sprite to non selected
						attackSelection.transform.GetChild(choiceNumber).GetComponent<Image>().sprite = DataContents.attackNonSelSprites[
							Array.IndexOf(DataContents.attackSelSprites, attackSelection.transform.GetChild(choiceNumber).
								GetComponent<Image>().sprite)];
				
						//Change selection
						choiceNumber++;
				
						//Change new sprite to selected
						attackSelection.transform.GetChild(choiceNumber).GetComponent<Image>().sprite = DataContents.attackSelSprites[
							Array.IndexOf(DataContents.attackNonSelSprites, attackSelection.transform.GetChild(choiceNumber).
								GetComponent<Image>().sprite)];
				
						//Update PP for new selection
						attackSelection.transform.GetChild(4).GetComponent<Text>().text = "PP\n" + battlers[0].BattlerPokemon.
							GetMovePP(choiceNumber) + "/" + battlers[0].BattlerPokemon.GetMovePPMax(choiceNumber);
						
						//Update selection reference
						selectedChoice = attackSelection.transform.GetChild(choiceNumber).gameObject;
					} //end if
				} //end else if

				//Pokemon selection
				else if (battleState == Battle.SELECTPOKEMON && Input.mousePosition.x > Camera.main.WorldToScreenPoint(
					selectedChoice.transform.position).x + selectedChoice.GetComponent<RectTransform>().rect.width / 2)
				{
					//If choice number is odd, and team is not odd numbered, and choice is greater than 0, move right
					if ((choiceNumber & 1) == 1 && choiceNumber != combatants[0].Team.Count && choiceNumber > 0)
					{
						choiceNumber++;
						selectedChoice = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
					} //end if
				} //end else if

				//Item use
				else if (battleState == Battle.ITEMUSE && Input.mousePosition.x > Camera.main.WorldToScreenPoint(
					selectedChoice.transform.position).x + selectedChoice.GetComponent<RectTransform>().rect.width / 2)
				{
					//If choice target is odd, and team is not odd numbered, and choice is greater than 0, move right
					if ((choiceTarget & 1) == 1 && choiceTarget != combatants[0].Team.Count && choiceTarget > 0)
					{
						choiceTarget++;
						selectedChoice = playerTeam.transform.FindChild("Pokemon" + choiceTarget).gameObject;
					} //end if
				} //end else if

				//Pokemon ribbons
				else if(battleState == Battle.POKEMONRIBBONS && Input.mousePosition.x > Camera.main.WorldToScreenPoint(
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
				} //end else if
			} //end if
		} //end else if Mouse Moves Right

		/*********************************************
		* Mouse Moves Up
		**********************************************/
		if (Input.GetAxis("Mouse Y") > 0)
		{
			//Regular processing
			if(checkpoint == 4)
			{
				//Command choice
				if (battleState == Battle.ROUNDSTART && Input.mousePosition.y > Camera.main.WorldToScreenPoint(selectedChoice.transform.position).y +
				    selectedChoice.GetComponent<RectTransform>().rect.height / 2)
				{
					//Move up only if on a higher child (lower in game)
					if (commandInt > 1)
					{
						//Set font color for current option to white
						commandChoice.transform.GetChild(commandInt).GetChild(0).GetComponent<Text>().color = Color.white;
				
						//Change selection
						commandInt -= 2;
				
						//Set new selection font to black
						commandChoice.transform.GetChild(commandInt).GetChild(0).GetComponent<Text>().color = Color.black;
				
						//Update selection reference
						selectedChoice = commandChoice.transform.GetChild(commandInt).gameObject;
					} //end if
				} //end if
				
				//Attack selection
				else if (battleState == Battle.SELECTATTACK && Input.mousePosition.y > Camera.main.WorldToScreenPoint(selectedChoice.transform.position).y +
				         selectedChoice.GetComponent<RectTransform>().rect.height / 2)
				{
					//Move up only on a higher child (lower in game)
					if (choiceNumber > 1)
					{
						//Change current sprite to non selected
						attackSelection.transform.GetChild(choiceNumber).GetComponent<Image>().sprite = DataContents.attackNonSelSprites[
							Array.IndexOf(DataContents.attackSelSprites, attackSelection.transform.GetChild(choiceNumber).
								GetComponent<Image>().sprite)];
				
						//Change selection
						choiceNumber -= 2;
				
						//Change new sprite to selected
						attackSelection.transform.GetChild(choiceNumber).GetComponent<Image>().sprite = DataContents.attackSelSprites[
							Array.IndexOf(DataContents.attackNonSelSprites, attackSelection.transform.GetChild(choiceNumber).
								GetComponent<Image>().sprite)];
				
						//Update PP for new selection
						attackSelection.transform.GetChild(4).GetComponent<Text>().text = "PP\n" + battlers[0].BattlerPokemon.
							GetMovePP(choiceNumber) + "/" + battlers[0].BattlerPokemon.GetMovePPMax(choiceNumber);
						
						//Update selection reference
						selectedChoice = attackSelection.transform.GetChild(choiceNumber).gameObject;
					} //end if
				} //end else if

				//Item use
				else if (battleState == Battle.ITEMUSE && Input.mousePosition.y > Camera.main.WorldToScreenPoint(
					selectedChoice.transform.position).y + selectedChoice.GetComponent<RectTransform>().rect.height / 2)
				{
					//Stay put if on top slot
					if (choiceTarget == 1 || choiceTarget == 2)
					{
						//Do nothing
					} //end if
					//Go up vertically
					else
					{
						choiceTarget -= 2;
						selectedChoice = playerTeam.transform.FindChild("Pokemon" + choiceTarget).gameObject;
					} //end else
				} //end else if

				//Pick move
				else if(battleState == Battle.PICKMOVE && Input.mousePosition.y > selection.transform.position.y +
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

				//Pokemon selection
				else if (battleState == Battle.SELECTPOKEMON && Input.mousePosition.y > Camera.main.WorldToScreenPoint(
					selectedChoice.transform.position).y + selectedChoice.GetComponent<RectTransform>().rect.height / 2)
				{
					//Stay put if on top slot
					if (choiceNumber == 1 || choiceNumber == 2)
					{
						//Do nothing
					} //end if
					//Go up vertically
					else
					{
						choiceNumber -= 2;
						selectedChoice = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
					} //end else
				} //end else if

				//Pokemon submenu
				else if(battleState == Battle.POKEMONSUBMENU && Input.mousePosition.y > selection.transform.position.y +
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

				//Pokemon summary
				else if(battleState == Battle.POKEMONSUMMARY && summaryChoice == 5 && Input.mousePosition.y >
					Camera.main.WorldToScreenPoint(selectedChoice.transform.position).y + selectedChoice.
					GetComponent<RectTransform>().rect.height / 2)
				{
					//If not at top, move
					if (moveChoice > 0)
					{
						//Decrease (higher slots are on lower children)
						moveChoice--;

						//Set currentMoveSlot
						selectedChoice = summaryScreen.transform.GetChild(5).FindChild("Move" + (moveChoice + 1)).gameObject;
					} //end if
				} //end else if

				//Pokemon move switch
				else if(battleState == Battle.MOVESWITCH && Input.mousePosition.y > Camera.main.WorldToScreenPoint(
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
				} //end else if

				//Pokemon ribbons
				else if(battleState == Battle.POKEMONRIBBONS && Input.mousePosition.y > Camera.main.WorldToScreenPoint(
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
				} //end else if
			} //end if
		} //end else if Mouse Moves Up

		/*********************************************
		* Mouse Moves Down
		**********************************************/
		if (Input.GetAxis("Mouse Y") < 0)
		{
			//Regular processing
			if(checkpoint == 4)
			{
				//Command choice
				if (battleState == Battle.ROUNDSTART && Input.mousePosition.y < Camera.main.WorldToScreenPoint(
					selectedChoice.transform.position).y - selectedChoice.GetComponent<RectTransform>().rect.height/2)
				{
					//Move down only if on a lower child (higher in game)
					if (commandInt < 2)
					{
						//Set font color for current option to white
						commandChoice.transform.GetChild(commandInt).GetChild(0).GetComponent<Text>().color = Color.white;
				
						//Change selection
						commandInt+=2;
				
						//Set new selection font to black
						commandChoice.transform.GetChild(commandInt).GetChild(0).GetComponent<Text>().color = Color.black;
				
						//Update selection reference
						selectedChoice = commandChoice.transform.GetChild(commandInt).gameObject;
					} //end if
				} //end if
				
				//Attack selection
				else if (battleState == Battle.SELECTATTACK && Input.mousePosition.y < Camera.main.WorldToScreenPoint(
					selectedChoice.transform.position).y - selectedChoice.GetComponent<RectTransform>().rect.height/2)
				{
					//Move down only on a lower child (higher in game)
					if (choiceNumber < 2 && choiceNumber + 2 < battlers[0].BattlerPokemon.GetMoveCount())
					{
						//Change current sprite to non selected
						attackSelection.transform.GetChild(choiceNumber).GetComponent<Image>().sprite = DataContents.attackNonSelSprites[
							Array.IndexOf(DataContents.attackSelSprites, attackSelection.transform.GetChild(choiceNumber).
								GetComponent<Image>().sprite)];
				
						//Change selection
						choiceNumber += 2;
				
						//Change new sprite to selected
						attackSelection.transform.GetChild(choiceNumber).GetComponent<Image>().sprite = DataContents.attackSelSprites[
							Array.IndexOf(DataContents.attackNonSelSprites, attackSelection.transform.GetChild(choiceNumber).
								GetComponent<Image>().sprite)];
				
						//Update PP for new selection
						attackSelection.transform.GetChild(4).GetComponent<Text>().text = "PP\n" + battlers[0].BattlerPokemon.
							GetMovePP(choiceNumber) + "/" + battlers[0].BattlerPokemon.GetMovePPMax(choiceNumber);
						
						//Update selection reference
						selectedChoice = attackSelection.transform.GetChild(choiceNumber).gameObject;
					} //end if
				} //end else if

				//Item use
				else if (battleState == Battle.ITEMUSE && Input.mousePosition.y < Camera.main.WorldToScreenPoint(
					selectedChoice.transform.position).y - selectedChoice.GetComponent<RectTransform>().rect.height / 2)
				{
					//Stay put if on bottom slot
					if ((choiceTarget == combatants[0].Team.Count - 1 && choiceTarget > 0) ||
						choiceTarget == combatants[0].Team.Count)
					{
						//Do nothing
					} //end if
					//Go down vertically
					else
					{
						choiceTarget += 2;
						selectedChoice = playerTeam.transform.FindChild("Pokemon" + choiceTarget).gameObject;
					} //end else
				} //end else if

				//Pick move
				else if(battleState == Battle.PICKMOVE && Input.mousePosition.y < selection.transform.position.y -
					selection.GetComponent<RectTransform>().rect.height / 2)
				{
					//If not on last option, increase (lower slots are on higher children)
					if (subMenuChoice < choices.transform.childCount - 1)
					{
						subMenuChoice++;
					} //end if

					//Reposition selection
					selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
				} //end else if

				//Pokemon selection
				else if (battleState == Battle.SELECTPOKEMON && Input.mousePosition.y < Camera.main.WorldToScreenPoint(
					selectedChoice.transform.position).y - selectedChoice.GetComponent<RectTransform>().rect.height / 2)
				{
					//Stay put if on bottom slot
					if ((choiceNumber == combatants[0].Team.Count - 1 && choiceNumber > 0) ||
						choiceNumber == combatants[0].Team.Count)
					{
						//Do nothing
					} //end if
					//Go down vertically
					else
					{
						choiceNumber += 2;
						selectedChoice = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
					} //end else
				} //end else if

				//Pokemon submenu
				else if(battleState == Battle.POKEMONSUBMENU && Input.mousePosition.y < selection.transform.position.y -
					selection.GetComponent<RectTransform>().rect.height / 2)
				{
					//If not on last option, increase (lower slots are on higher children)
					if (subMenuChoice < choices.transform.childCount - 1)
					{
						subMenuChoice++;
					} //end if

					//Reposition selection
					selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
				} //end else if

				//Pokemon summary
				else if(battleState == Battle.POKEMONSUMMARY && summaryChoice == 5 && Input.mousePosition.y <
					Camera.main.WorldToScreenPoint(selectedChoice.transform.position).y - selectedChoice.
					GetComponent<RectTransform>().rect.height / 2)
				{
					//If next slot is null, don't move
					if (moveChoice < combatants[0].Team[choiceNumber - 1].GetMoveCount() - 1)
					{
						//Increase (lower slots are on higher children)
						moveChoice++;

						//Set currentMoveSlot
						selectedChoice = summaryScreen.transform.GetChild(5).FindChild("Move" + (moveChoice + 1)).gameObject;
					} //end if
				} //end else if

				//Pokemon move switch
				else if(battleState == Battle.MOVESWITCH && Input.mousePosition.y < Camera.main.WorldToScreenPoint(
					currentSwitchSlot.transform.position).y - currentSwitchSlot.GetComponent<RectTransform>().rect.height / 2)
				{
					//If next slot is null, don't move
					if (switchChoice < combatants[0].Team[choiceNumber - 1].GetMoveCount() - 1)
					{
						//Increase (Lower slots are on higher children)
						switchChoice++;

						//Set currentSwitchSlot
						currentSwitchSlot = summaryScreen.transform.GetChild(5).FindChild("Move" + (switchChoice + 1)).gameObject;
					} //end if
				} //end else if

				//Pokemon ribbons
				else if(battleState == Battle.POKEMONRIBBONS && Input.mousePosition.y < Camera.main.WorldToScreenPoint(
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
				} //end else if
			} //end if
		} //end else if Mouse Moves Down

		/*********************************************
		* Mouse Wheel Up
		**********************************************/
		else if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			//Regular processing
			if(checkpoint == 4)
			{
				//Inventory selection
				if (battleState == Battle.SELECTITEM)
				{
					choiceNumber = ExtensionMethods.BindToInt(choiceNumber - 1, 0);
					if (choiceNumber < topShown)
					{
						topShown = choiceNumber;
						bottomShown--;
					} //end if
				} //end if

				//Pokemon summary
				else if (battleState == Battle.POKEMONSUMMARY)
				{
					//If on any page besides move details
					if (summaryChoice != 5)
					{
						//Decrease (higher slots are on lower children)
						choiceNumber--;

						//Loop to end of team if on first member
						if (choiceNumber < 1)
						{
							choiceNumber = combatants[0].Team.Count;
						} //end if

						//Update selected pokemon 
						selectedChoice = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
					} //end if
				} //end else if

				//Pokemon ribbons
				else if (battleState == Battle.POKEMONRIBBONS)
				{
					//Decrease (higher slots are on lower children)
					choiceNumber--;

					//Loop to end of team if on first member
					if (choiceNumber < 1)
					{
						choiceNumber = combatants[0].Team.Count;
					} //end if

					//Update selected pokemon 
					selectedChoice = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;

					//Reload ribbons
					InitializeRibbons();
				} //end else if
			} //end if
		} //end else if Mouse Wheel Up

		/*********************************************
		* Mouse Wheel Down
		**********************************************/
		else if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			//Regular processing
			if(checkpoint == 4)
			{
				//Inventory selection
				if (battleState == Battle.SELECTITEM)
				{
					choiceNumber = ExtensionMethods.CapAtInt(choiceNumber + 1, combatants[0].SlotCount() - 1);
					if (choiceNumber > bottomShown)
					{
						bottomShown = choiceNumber;
						topShown++;
					} //end if
				} //end if

				//Pokemon summary
				else if (battleState == Battle.POKEMONSUMMARY)
				{
					//If on any page besides move details
					if (summaryChoice != 5)
					{
						//Increase (lower slots are on higher children)
						choiceNumber++;

						//Loop to front of team if on last member
						if (choiceNumber > combatants[0].Team.Count)
						{
							choiceNumber = 1;
						} //end if

						//Update selected pokemon 
						selectedChoice = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
					} //end if
				} //end else if

				//Pokemon ribbons
				else if (battleState == Battle.POKEMONRIBBONS)
				{
					//Increase (lower slots are on higher children)
					choiceNumber++;

					//Loop to front of team if on last member
					if (choiceNumber > GameManager.instance.GetTrainer().Team.Count)
					{
						choiceNumber = 1;
					} //end if

					//Update selected pokemon
					selectedChoice = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;

					//Reload ribbons
					InitializeRibbons();
				} //end else if
			} //end if
		} //end else if Mouse Wheel Down

		/*********************************************
		* Left Mouse Button
		**********************************************/
		else if (Input.GetMouseButtonUp(0))
		{	
			//Wait for player to begin the fade animation
			if (checkpoint == 2)
			{
				if (processing)
				{
					return;
				} //end if
				processing = true;
				trainerStands[1].GetComponent<Animator>().SetTrigger("FadeEnemy");
				WriteBattleMessage(combatants[1].PlayerName + " sent out " + battlers[1].Nickname + "!");
				StartCoroutine(FadePlayer());
			} //end if

			//Regular processing
			else if(checkpoint == 4)
			{				
				//Player confirms a choice
				if (battleState == Battle.ROUNDSTART)
				{
					switch (commandInt)
					{
					//Fight
						case 0:
							//Show attack selection screen
							commandChoice.SetActive(false);
							attackSelection.SetActive(true);
				
							//Load screen with pokemon attacks
							for (int i = 0; i < 4; i++)
							{
								int attackType = DataContents.GetMoveIcon(battlers[0].BattlerPokemon.GetMove(i));
								if (attackType != -1)
								{
									attackSelection.transform.GetChild(i).gameObject.SetActive(true);
									attackSelection.transform.GetChild(i).GetComponent<Image>().sprite = DataContents.attackNonSelSprites[
										attackType];
									attackSelection.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = 
										DataContents.GetMoveGameName(battlers[0].BattlerPokemon.GetMove(i));
								} //end if
								else
								{
									attackSelection.transform.GetChild(i).gameObject.SetActive(false);
								} //end else
							} //end for
							attackSelection.transform.GetChild(choiceNumber).GetComponent<Image>().sprite = DataContents.attackSelSprites[
								Array.IndexOf(DataContents.attackNonSelSprites, attackSelection.transform.GetChild(choiceNumber).
									GetComponent<Image>().sprite)];
							attackSelection.transform.GetChild(4).GetComponent<Text>().text = "PP\n" + battlers[0].BattlerPokemon.
								GetMovePP(choiceNumber) + "/" + battlers[0].BattlerPokemon.GetMovePPMax(choiceNumber);
							selectedChoice = attackSelection.transform.GetChild(choiceNumber).gameObject;
							battleState = Battle.SELECTATTACK;
							break;
					//Bag
						case 1:
							playerBag.SetActive(true);
							int currentPocket = combatants[0].GetCurrentBagPocket();
							bagBack.sprite = Resources.Load<Sprite>("Sprites/Menus/bag" + currentPocket);
							choiceNumber = 0;
							topShown = 0;
							bottomShown = 9;
							battleState = Battle.SELECTITEM;
							break;
					//Pokemon
						case 2:
							playerTeam.SetActive(true);
							selectedChoice = playerTeam.transform.FindChild("Pokemon1").gameObject;
							playerTeam.transform.FindChild("PartyInstructions").GetChild(0).GetComponent<Text>().text = 
								"Choose a Pokemon to switch in.";
							choiceNumber = 1;
							battleState = Battle.SELECTPOKEMON;
							break;
					//Run
						case 3:
							GameManager.instance.DisplayConfirm("Are you sure you want to cancel the current battle?", 1, true);
							break;
					} //end switch
				} //end else if

				//Attack selection
				else if (battleState == Battle.SELECTATTACK)
				{						
					if (battlers[0].GetMovePP(choiceNumber) > 0)
					{
						attackSelection.SetActive(false);
						AddToQueue(0, commandInt, choiceNumber, DetermineTarget(0, 0, choiceNumber), DeterminePriority(0, 0, choiceNumber));
						battleState = Battle.GETAICHOICE;
					} //end if
					else
					{
						GameManager.instance.DisplayText("There's no PP left! You can't choose this move.", true);
					} //end else
				} //end else if

				//Item selection
				else if (battleState == Battle.SELECTITEM)
				{
					StartCoroutine(ProcessUseItem());
				} //end else if

				//Item use
				else if (battleState == Battle.ITEMUSE)
				{
					int itemNumber = combatants[0].GetItem(choiceNumber)[0];
					if (bool.Parse(ItemEffects.BattleUseOnPokemon(combatants[0].Team[choiceTarget - 1], itemNumber, true)))
					{
						playerTeam.SetActive(false);
						commandChoice.SetActive(false);
						AddToQueue(0, commandInt, choiceNumber, choiceTarget - 1, DeterminePriority(0, 1, itemNumber));
						battleState = Battle.GETAICHOICE;
					}  //end if
				} //end else if

				//Pick Move Processing
				else if (battleState == Battle.PICKMOVE)
				{
					int itemNumber = combatants[0].GetItem(choiceNumber)[0];
					QueueEvent newEvent = new QueueEvent();
					newEvent.battler = 0;
					newEvent.action = 1;
					newEvent.selection = choiceNumber;
					newEvent.target = DetermineTarget(0, 1, choiceNumber);
					newEvent.priority = 7;
					if (bool.Parse(ApplySpecialItem(newEvent, true)))
					{
						selection.SetActive(false);
						choices.SetActive(false);
						playerTeam.SetActive(false);
						commandChoice.SetActive(false);
						AddToQueue(0, commandInt, choiceNumber, DetermineTarget(0, 1, itemNumber), DeterminePriority(0, 1, itemNumber));
						battleState = Battle.GETAICHOICE;
					} //end if
				} //end else if

				//Pokemon selection
				else if (battleState == Battle.SELECTPOKEMON)
				{
					FillInChoices();
					StartCoroutine(WaitForResize());
					choices.SetActive(true);
					subMenuChoice = 0;
					battleState = Battle.POKEMONSUBMENU;
				} //end else if

				//Pokemon submenu
				else if (battleState == Battle.POKEMONSUBMENU)
				{
					//Apply appropriate action based on subMenuChoice
					switch (subMenuChoice)
					{
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
							battleState = Battle.POKEMONSUMMARY;
							break;
						case 1:
							if (combatants[0].Team[choiceNumber - 1].Status != (int)Status.FAINT)
							{
								selection.SetActive(false);
								choices.SetActive(false);
								playerTeam.SetActive(false);
								commandChoice.SetActive(false);
								AddToQueue(0, commandInt, 0, choiceNumber - 1, DeterminePriority(0, 2, choiceNumber));
								battleState = Battle.GETAICHOICE;
							} //end if
							else
							{
								GameManager.instance.DisplayText(combatants[0].Team[choiceNumber - 1].Nickname +
								" has fainted. It can't be used in battle currently.", true);
							} //end else
							break;
						case 2:
							selection.SetActive(false);
							choices.SetActive(false);
							currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").GetChild(0).gameObject;
							ribbonScreen.SetActive(true);
							InitializeRibbons();
							battleState = Battle.POKEMONRIBBONS;
							break;
						case 3:
							selection.SetActive(false);
							choices.SetActive(false);
							battleState = Battle.SELECTPOKEMON;
							break;
					} //end switch
				} //end else if

				//Pokemon summary
				else if (battleState == Battle.POKEMONSUMMARY)
				{
					//If on moves screen, switch to move details
					if (summaryChoice == 4)
					{
						moveChoice = 0;
						summaryChoice = 5;
						selectedChoice = summaryScreen.transform.GetChild(5).FindChild("Move1").gameObject;
					} //end if

					//If on move details screen, go to move switch
					else if (summaryChoice == 5)
					{
						selectedChoice.GetComponent<Image>().color = Color.white;
						switchChoice = moveChoice;
						currentSwitchSlot = selectedChoice;
						battleState = Battle.MOVESWITCH;
					} //end else if
				} //end else if

				//Pokemon move switch
				else if (battleState == Battle.MOVESWITCH)
				{
					//If switching sparts aren't the same, switch the moves
					if (moveChoice != switchChoice)
					{					
						combatants[0].Team[choiceNumber - 1].SwitchMoves(moveChoice, switchChoice);
					} //end if

					//Set color of background to clear
					selectedChoice.GetComponent<Image>().color = Color.clear;
					battleState = Battle.POKEMONSUMMARY;
				} //end else if

				//Pokemon ribbons
				else if (battleState == Battle.POKEMONRIBBONS)
				{
					//Make sure there are ribbons to be read
					if (combatants[0].Team[choiceNumber - 1].GetRibbonCount() > 0)
					{
						selection.SetActive(!selection.activeSelf);
						ReadRibbon();
					} //end if
				} //end else if
			} //end else if
		} //end else if Left Mouse Button

		/*********************************************
		* Right Mouse Button
		**********************************************/
		else if (Input.GetMouseButtonUp(1))
		{
			//Regular processing
			if (checkpoint == 4)
			{
				//Attack selection
				if (battleState == Battle.SELECTATTACK)
				{
					commandChoice.SetActive(true);
					attackSelection.SetActive(false);
					battleState = Battle.ROUNDSTART;
				} //end if

				//Item selection
				else if (battleState == Battle.SELECTITEM)
				{
					commandChoice.SetActive(true);
					playerBag.SetActive(false);
					battleState = Battle.ROUNDSTART;
				} //end else if

				//Item use
				else if (battleState == Battle.ITEMUSE)
				{
					playerBag.SetActive(true);
					playerTeam.SetActive(false);
					battleState = Battle.SELECTITEM;
				} //end else if

				//Pick move
				else if (battleState == Battle.PICKMOVE)
				{
					selection.SetActive(false);
					choices.SetActive(false);
					battleState = Battle.ITEMUSE;
				} //end else if

				//Pokemon selection
				else if (battleState == Battle.SELECTPOKEMON)
				{
					commandChoice.SetActive(true);
					playerTeam.SetActive(false);
					battleState = Battle.ROUNDSTART;
				} //end else if

				//Pokemon submenu
				else if (battleState == Battle.POKEMONSUBMENU)
				{
					selection.SetActive(false);
					choices.SetActive(false);
					battleState = Battle.SELECTPOKEMON;
				} //end else if

				//Pokemon summary
				else if (battleState == Battle.POKEMONSUMMARY)
				{
					//If on any page besides details
					if (summaryChoice != 5)
					{
						//Deactivate summary
						summaryScreen.SetActive(false);

						//Return to select pokemon
						battleState = Battle.SELECTPOKEMON;
					} //end if
					else
					{
						summaryScreen.transform.GetChild(5).gameObject.SetActive(false);
						selection.SetActive(false);
						summaryChoice = 4;
					} //end else
				} //end else if

				//Pokemon move switch
				else if (battleState == Battle.MOVESWITCH)
				{
					selectedChoice.GetComponent<Image>().color = Color.clear;
					battleState = Battle.POKEMONSUMMARY;
				} //end else if

				//Pokemon ribbons
				else if (battleState == Battle.POKEMONRIBBONS)
				{
					//Deactivate ribbons
					ribbonScreen.SetActive(false);
					selection.SetActive(false);
					ribbonChoice = 0;
					previousRibbonChoice = -1;

					//Return to select pokemon
					battleState = Battle.SELECTPOKEMON;
				} //end else if
			} //end if
		} //end else if Right Mouse Button

		/*********************************************
		* Enter/Return Key
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.Return))
		{
			//Wait for player to begin the fade animation
			if (checkpoint == 2)
			{
				if (processing)
				{
					return;
				} //end if
				processing = true;
				trainerStands[1].GetComponent<Animator>().SetTrigger("FadeEnemy");
				WriteBattleMessage(combatants[1].PlayerName + " sent out " + battlers[1].Nickname + "!");
				StartCoroutine(FadePlayer());
			} //end if

			//Regular processing
			else if(checkpoint == 4)
			{				
				//Player confirms a choice
				if (battleState == Battle.ROUNDSTART)
				{
					switch (commandInt)
					{
						//Fight
						case 0:
							//Show attack selection screen
							commandChoice.SetActive(false);
							attackSelection.SetActive(true);

							//Load screen with pokemon attacks
							for (int i = 0; i < 4; i++)
							{
								int attackType = DataContents.GetMoveIcon(battlers[0].BattlerPokemon.GetMove(i));
								if (attackType != -1)
								{
									attackSelection.transform.GetChild(i).gameObject.SetActive(true);
									attackSelection.transform.GetChild(i).GetComponent<Image>().sprite = DataContents.attackNonSelSprites[
										attackType];
									attackSelection.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = 
										DataContents.GetMoveGameName(battlers[0].BattlerPokemon.GetMove(i));
								} //end if
								else
								{
									attackSelection.transform.GetChild(i).gameObject.SetActive(false);
								} //end else
							} //end for
							attackSelection.transform.GetChild(choiceNumber).GetComponent<Image>().sprite = DataContents.attackSelSprites[
								Array.IndexOf(DataContents.attackNonSelSprites, attackSelection.transform.GetChild(choiceNumber).
									GetComponent<Image>().sprite)];
							attackSelection.transform.GetChild(4).GetComponent<Text>().text = "PP\n" + battlers[0].BattlerPokemon.
								GetMovePP(choiceNumber) + "/" + battlers[0].BattlerPokemon.GetMovePPMax(choiceNumber);
							selectedChoice = attackSelection.transform.GetChild(choiceNumber).gameObject;
							battleState = Battle.SELECTATTACK;
							break;
							//Bag
						case 1:
							playerBag.SetActive(true);
							int currentPocket = combatants[0].GetCurrentBagPocket();
							bagBack.sprite = Resources.Load<Sprite>("Sprites/Menus/bag" + currentPocket);
							choiceNumber = 0;
							topShown = 0;
							bottomShown = 9;
							battleState = Battle.SELECTITEM;
							break;
							//Pokemon
						case 2:
							playerTeam.SetActive(true);
							selectedChoice = playerTeam.transform.FindChild("Pokemon1").gameObject;
							playerTeam.transform.FindChild("PartyInstructions").GetChild(0).GetComponent<Text>().text = 
								"Choose a Pokemon to switch in.";
							choiceNumber = 1;
							battleState = Battle.SELECTPOKEMON;
							break;
							//Run
						case 3:
							GameManager.instance.DisplayConfirm("Are you sure you want to cancel the current battle?", 1, true);
							break;
					} //end switch
				} //end else if

				//Attack selection
				else if (battleState == Battle.SELECTATTACK)
				{						
					if (battlers[0].GetMovePP(choiceNumber) > 0)
					{
						attackSelection.SetActive(false);
						AddToQueue(0, commandInt, choiceNumber, DetermineTarget(0, 0, choiceNumber), DeterminePriority(0, 0, choiceNumber));
						battleState = Battle.GETAICHOICE;
					} //end if
					else
					{
						GameManager.instance.DisplayText("There's no PP left! You can't choose this move.", true);
					} //end else
				} //end else if

				//Item selection
				else if (battleState == Battle.SELECTITEM)
				{
					StartCoroutine(ProcessUseItem());
				} //end else if

				//Item use
				else if (battleState == Battle.ITEMUSE)
				{
					int itemNumber = combatants[0].GetItem(choiceNumber)[0];
					if (bool.Parse(ItemEffects.BattleUseOnPokemon(combatants[0].Team[choiceTarget - 1], itemNumber, true)))
					{
						playerTeam.SetActive(false);
						commandChoice.SetActive(false);
						AddToQueue(0, commandInt, choiceNumber, choiceTarget - 1, DeterminePriority(0, 1, itemNumber));
						battleState = Battle.GETAICHOICE;
					}  //end if
				} //end else if

				//Pick Move Processing
				else if (battleState == Battle.PICKMOVE)
				{
					int itemNumber = combatants[0].GetItem(choiceNumber)[0];
					QueueEvent newEvent = new QueueEvent();
					newEvent.battler = 0;
					newEvent.action = 1;
					newEvent.selection = choiceNumber;
					newEvent.target = DetermineTarget(0, 1, choiceNumber);
					newEvent.priority = 7;
					if (bool.Parse(ApplySpecialItem(newEvent, true)))
					{
						selection.SetActive(false);
						choices.SetActive(false);
						playerTeam.SetActive(false);
						commandChoice.SetActive(false);
						AddToQueue(0, commandInt, choiceNumber, DetermineTarget(0, 1, itemNumber), DeterminePriority(0, 1, itemNumber));
						battleState = Battle.GETAICHOICE;
					} //end if
				} //end else if

				//Pokemon selection
				else if (battleState == Battle.SELECTPOKEMON)
				{
					FillInChoices();
					StartCoroutine(WaitForResize());
					choices.SetActive(true);
					subMenuChoice = 0;
					battleState = Battle.POKEMONSUBMENU;
				} //end else if

				//Pokemon submenu
				else if (battleState == Battle.POKEMONSUBMENU)
				{
					//Apply appropriate action based on subMenuChoice
					switch (subMenuChoice)
					{
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
							battleState = Battle.POKEMONSUMMARY;
							break;
						case 1:
							if (combatants[0].Team[choiceNumber - 1].Status != (int)Status.FAINT)
							{
								selection.SetActive(false);
								choices.SetActive(false);
								playerTeam.SetActive(false);
								commandChoice.SetActive(false);
								AddToQueue(0, commandInt, 0, choiceNumber - 1, DeterminePriority(0, 2, choiceNumber));
								battleState = Battle.GETAICHOICE;
							} //end if
							else
							{
								GameManager.instance.DisplayText(combatants[0].Team[choiceNumber - 1].Nickname +
									" has fainted. It can't be used in battle currently.", true);
							} //end else
							break;
						case 2:
							selection.SetActive(false);
							choices.SetActive(false);
							currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").GetChild(0).gameObject;
							ribbonScreen.SetActive(true);
							InitializeRibbons();
							battleState = Battle.POKEMONRIBBONS;
							break;
						case 3:
							selection.SetActive(false);
							choices.SetActive(false);
							battleState = Battle.SELECTPOKEMON;
							break;
					} //end switch
				} //end else if

				//Pokemon summary
				else if (battleState == Battle.POKEMONSUMMARY)
				{
					//If on moves screen, switch to move details
					if (summaryChoice == 4)
					{
						moveChoice = 0;
						summaryChoice = 5;
						selectedChoice = summaryScreen.transform.GetChild(5).FindChild("Move1").gameObject;
					} //end if

					//If on move details screen, go to move switch
					else if (summaryChoice == 5)
					{
						selectedChoice.GetComponent<Image>().color = Color.white;
						switchChoice = moveChoice;
						currentSwitchSlot = selectedChoice;
						battleState = Battle.MOVESWITCH;
					} //end else if
				} //end else if

				//Pokemon move switch
				else if (battleState == Battle.MOVESWITCH)
				{
					//If switching sparts aren't the same, switch the moves
					if (moveChoice != switchChoice)
					{					
						combatants[0].Team[choiceNumber - 1].SwitchMoves(moveChoice, switchChoice);
					} //end if

					//Set color of background to clear
					selectedChoice.GetComponent<Image>().color = Color.clear;
					battleState = Battle.POKEMONSUMMARY;
				} //end else if

				//Pokemon ribbons
				else if (battleState == Battle.POKEMONRIBBONS)
				{
					//Make sure there are ribbons to be read
					if (combatants[0].Team[choiceNumber - 1].GetRibbonCount() > 0)
					{
						selection.SetActive(!selection.activeSelf);
						ReadRibbon();
					} //end if
				} //end else if
			} //end else if
		} //end else if Enter/Return Key

		/*********************************************
		* X Key
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.X))
		{
			//Regular processing
			if (checkpoint == 4)
			{
				//Attack selection
				if (battleState == Battle.SELECTATTACK)
				{
					commandChoice.SetActive(true);
					attackSelection.SetActive(false);
					battleState = Battle.ROUNDSTART;
				} //end if

				//Item selection
				else if (battleState == Battle.SELECTITEM)
				{
					commandChoice.SetActive(true);
					playerBag.SetActive(false);
					battleState = Battle.ROUNDSTART;
				} //end else if

				//Item use
				else if (battleState == Battle.ITEMUSE)
				{
					playerBag.SetActive(true);
					playerTeam.SetActive(false);
					battleState = Battle.SELECTITEM;
				} //end else if

				//Pick move
				else if (battleState == Battle.PICKMOVE)
				{
					selection.SetActive(false);
					choices.SetActive(false);
					battleState = Battle.ITEMUSE;
				} //end else if

				//Pokemon selection
				else if (battleState == Battle.SELECTPOKEMON)
				{
					commandChoice.SetActive(true);
					playerTeam.SetActive(false);
					battleState = Battle.ROUNDSTART;
				} //end else if

				//Pokemon submenu
				else if (battleState == Battle.POKEMONSUBMENU)
				{
					selection.SetActive(false);
					choices.SetActive(false);
					battleState = Battle.SELECTPOKEMON;
				} //end else if

				//Pokemon summary
				else if (battleState == Battle.POKEMONSUMMARY)
				{
					//If on any page besides details
					if (summaryChoice != 5)
					{
						//Deactivate summary
						summaryScreen.SetActive(false);

						//Return to select pokemon
						battleState = Battle.SELECTPOKEMON;
					} //end if
					else
					{
						summaryScreen.transform.GetChild(5).gameObject.SetActive(false);
						selection.SetActive(false);
						summaryChoice = 4;
					} //end else
				} //end else if

				//Pokemon move switch
				else if (battleState == Battle.MOVESWITCH)
				{
					selectedChoice.GetComponent<Image>().color = Color.clear;
					battleState = Battle.POKEMONSUMMARY;
				} //end else if

				//Pokemon ribbons
				else if (battleState == Battle.POKEMONRIBBONS)
				{
					//Deactivate ribbons
					ribbonScreen.SetActive(false);
					selection.SetActive(false);
					ribbonChoice = 0;
					previousRibbonChoice = -1;

					//Return to select pokemon
					battleState = Battle.SELECTPOKEMON;
				} //end else if
			} //end if
		} //end else if X Key
	} //end GetInput

	/***************************************
	 * Name: InitializeBattle
	 * Sets battle as a Single, Double, Triple
	 * or other type and the trainers 
	 ***************************************/
	public void InitializeBattle(int bType, List<Trainer> trainers)
	{
		battleType = bType;
		combatants = trainers;
		originalOrder = trainers[0].Team;
	} //end InitializeBattle(int bType)

	/***************************************
	 * Name: WriteBattleMessage
	 * Writes a message to the battle window
	 ***************************************/
	public void WriteBattleMessage(string message)
	{
		messageBox.text = message;
	} //end WriteBattleMessage(string message)

	/***************************************
	 * Name: CheckMoveUsed
	 * Retrieves name of the attack used
	 ***************************************/
	public string CheckMoveUsed()
	{
		return DataContents.GetMoveGameName(currentAttack);
	} //end CheckMoveUsed

	/***************************************
	 * Name: CheckMoveUser
	 * Returns the pokemon using the move
	 ***************************************/
	public PokemonBattler CheckMoveUser()
	{
		return currentAttacker;
	} //end CheckMoveUser

	/***************************************
	 * Name: CheckEffect
	 * Checks if an effect is in place on
	 * the field
	 ***************************************/
	public bool CheckEffect(int effect, int target)
	{
		return fieldEffects[target][effect] > 0;
	} //end CheckEffect(int effect, int target)

	/***************************************
	 * Name: DeterminePriority
	 * Returns the priority of the action
	 ***************************************/
	int DeterminePriority(int battler, int action, int selection)
	{
		switch (action)
		{
			case 0:
				return DataContents.GetMovePriority(battlers[battler].GetMove(selection));
			case 1:
				return 6;
			case 2:
				return 7;
			default:
				return 0;
		} //end switch
	} //end DeterminePriority(int battler, int action, int selection)

	/***************************************
	 * Name: DetermineTarget
	 * Returns the target of the action
	 ***************************************/
	int DetermineTarget(int battler, int action, int selection)
	{
		switch (action)
		{
			case 0:
				switch (DataContents.ExecuteSQL<int>("SELECT target FROM Moves WHERE rowid=" + selection))
				{
					//Single Pokemon other than user
					case 0:
						return battler == 0 ? 1 : 0;
					//No target - Refer to battler's effects
					case 1:
						return -1;
					//Random opponent
					case 2:
						return battler == 0 ? 1 : 0;
					//All opponents
					case 4:
						return battler == 0 ? 1 : 0;
					//All pokemon besides user
					case 8:
						return battler == 0 ? 1 : 0;
					//User
					case 10:
						return battler;
					//User's side - Refer to own side's field effects
					case 20:
						return -2;
					//Affects all on field
					case 40:
						return battlers.Count;
					//Opponent's side - Refer to other side's field effects
					case 80:
						return -3;
					//User's partner
					case 100:
						return battler;
					//Single pokemon on user's side
					case 200:
						return battler;
					//Single opposing pokemon
					case 400:
						return battler == 0 ? 1 : 0;
					//Single pokemon directly opposite of user
					case 800:
						return battler == 0 ? 1 : 0;
					//No target found, log it and return opponent
					default:
						GameManager.instance.LogErrorMessage(selection + " did not have a target.");
						return  battler == 0 ? 1 : 0;
				} //end switch
			case 1:
				//Get item used
				int item = combatants[0].GetItem(choiceNumber)[0];

				//Check for Leppa Berry/Ether/MaxEther
				if (item == 83 || item == 148 || item == 169)
				{
					return int.Parse("9" + (choiceTarget-1).ToString() + subMenuChoice.ToString());
				} //end if

				//Otherwise return battler
				else
				{
					return battler;
				} //end else
			case 2:
				return battler;
			default:
				return battler == 0 ? 1 : 0;
		} //end switch
	} //end DetermineTarget(int battler, int action, int selection)

	/***************************************
	 * Name: AddToQueue
	 * Adds an event to queue
	 ***************************************/
	public void AddToQueue(int battler, int action, int selection, int target, int priority)
	{
		QueueEvent newEvent = new QueueEvent();
		newEvent.battler = battler;
		newEvent.action = action;
		newEvent.selection = selection;
		newEvent.target = target;
		newEvent.priority = priority;
		newEvent.success = false;
		queue.Add(newEvent);
	} //end AddToQueue(int battler, int action, int selection, int target, int priority)

	/***************************************
	 * Name: SortQueue
	 * Sorts movement order based on priority
	 ***************************************/
	public void SortQueue()
	{
		//Go through the queue and sort by priority then speed
		for (int i = 0; i < queue.Count; i++)
		{
			//Store the highest priority move and speed
			int eventIndex = i;

			//Loop through the remaining list
			for (int j = i+1; j < queue.Count; j++)
			{
				//If the priority is the same
				if (queue[eventIndex].priority == queue[j].priority)
				{
					//If current event has a lower speed, swap to new event
					if (battlers[queue[eventIndex].battler].GetSpeed() < battlers[queue[j].battler].GetSpeed())
					{
						eventIndex = j;
					} //end if
				} //end if

				//If the priority is lower, swap to new event
				else if (queue[eventIndex].priority < queue[j].priority)
				{
					eventIndex = j;
				} //end else if
			} //end for

			//Put the event in the next space of queue
			ExtensionMethods.Swap(queue,i,eventIndex);
		} //end for
	} //end SortQueue

	/***************************************
	 * Name: ApplySpecialItem
	 * Applies items that require multiple 
	 * parameters beyond normal item
	 ***************************************/
	string ApplySpecialItem(QueueEvent toProcess, bool check = false)
	{
		//Get item and targets
		string givenTarget = toProcess.target.ToString();
		int pokemonTarget = int.Parse(givenTarget[1].ToString());
		int moveTarget = int.Parse(givenTarget[2].ToString());
		int item = combatants[0].GetItem(toProcess.selection)[0];

		//Ether, Leppa Berry
		if (item == 83 || item == 148)
		{
			//Get current move pp and max
			int currentPP = combatants[toProcess.battler].Team[pokemonTarget].GetMovePP(moveTarget);
			int maxPP = combatants[toProcess.battler].Team[pokemonTarget].GetMovePPMax(moveTarget);

			//Check if Move PP is not full
			if (currentPP < maxPP)
			{
				if (!check)
				{
					int result = ExtensionMethods.CapAtInt(currentPP + 10, maxPP);
					combatants[toProcess.battler].Team[pokemonTarget].SetMovePP(moveTarget, result);
					return string.Format("{0}! {1} was restored by {2}!",combatants[toProcess.battler].Team[pokemonTarget].Nickname, 
						DataContents.GetMoveGameName(combatants[toProcess.battler].Team[pokemonTarget].GetMove(moveTarget)),
						result);
					combatants[toProcess.battler].RemoveItem(combatants[toProcess.battler].GetItem(toProcess.selection)[0], 1);
				} //end if
				else
				{
					return bool.TrueString;
				} //end else
			} //end if
			else
			{
				GameManager.instance.DisplayText(string.Format("{0} is already at full PP. It won't have any effect.", 
					DataContents.GetMoveGameName(combatants[toProcess.battler].Team[pokemonTarget].GetMove(moveTarget))), 
					true);
				return bool.FalseString;
			} //end else
		} //end if

		//Max Ether
		else if (item == 169)
		{
			//Get current move pp and max
			int currentPP = combatants[toProcess.battler].Team[pokemonTarget].GetMovePP(moveTarget);
			int maxPP = combatants[toProcess.battler].Team[pokemonTarget].GetMovePPMax(moveTarget);

			//Check if Move PP is not full
			if (currentPP < maxPP)
			{
				if (!check)
				{
					int result = maxPP - currentPP;
					combatants[toProcess.battler].Team[pokemonTarget].SetMovePP(moveTarget, maxPP);
					return string.Format("{0}! {1} was restored by {2}!", combatants[toProcess.battler].Team[pokemonTarget].Nickname,
						DataContents.GetMoveGameName(combatants[toProcess.battler].Team[pokemonTarget].GetMove(moveTarget)),
						result);
					combatants[toProcess.battler].RemoveItem(combatants[toProcess.battler].GetItem(toProcess.selection)[0], 1);
				} //end if
				else
				{
					return bool.TrueString;
				} //end else
			} //end if
			else
			{
				GameManager.instance.DisplayText(string.Format("{0} is already at full PP. It won't have any effect.", 
					DataContents.GetMoveGameName(combatants[toProcess.battler].Team[pokemonTarget].GetMove(moveTarget))), 
					true);
				return bool.FalseString;
			} //end else
		} //end else if

		//Anything else. This is what occurs when an item has no effect but is listed as usable
		else
		{
			GameManager.instance.DisplayText(DataContents.GetItemGameName(item) + " has no listed effect yet.", true);
			return bool.FalseString;
		} //end else
	} //end ApplySpecialItem(QueueEvent toProcess, bool check = false)

	/***************************************
	 * Name: ResolveFieldEntrance
	 * Activates entrance effects of fields,
	 * abilities, and items
	 ***************************************/
	IEnumerator ResolveFieldEntrance()
	{
		WriteBattleMessage("There were no field entrance effects to process.");
		battlers = battleField.ResolveFieldEntrance(battlers);
		yield return new WaitForSeconds(1.5f);
		StartCoroutine(EntranceAbilities());
	} //end ResolveFieldEntrance

	/***************************************
	 * Name: EntranceAbilities
	 * Activates entrance abilities based on 
	 * speed
	 ***************************************/
	IEnumerator EntranceAbilities()
	{
		WriteBattleMessage("There were no abilities to process.");
		yield return new WaitForSeconds(1.5f);
		StartCoroutine(EntranceItems());
	} //end EntranceAbilities

	/***************************************
	 * Name: EntranceItems
	 * Activates entrance items based on 
	 * speed
	 ***************************************/
	IEnumerator EntranceItems()
	{
		WriteBattleMessage("There were no items to process");
		yield return new WaitForSeconds(1.5f);
		checkpoint = 4;
	} //end EntranceItems

	/***************************************
	 * Name: SetStatusIcon
	 * Sets status icon based on pokemon 
	 * status
	 ***************************************/
	void SetStatusIcon(Image statusImage, PokemonBattler myPokemon)
	{
		//Set status
		switch (myPokemon.BattlerStatus)
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
	} //end SetStatusIcon(Image statusImage, PokemonBattler myPokemon)

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
	 * Name: FillInBattlerData
	 * Updates the sprites and boxes to match
	 * the active pokemon
	 ***************************************/
	void FillInBattlerData()
	{
		//Update the battler
		battlers[0].UpdateActiveBattler();

		//Set back of player pokemon
		string toLoad = "Sprites/Pokemon/" + battlers[0].BattlerPokemon.NatSpecies.ToString("000");
		toLoad += battlers[0].BattlerPokemon.Gender == 1 ? "f" : "";
		toLoad += battlers[0].BattlerPokemon.IsShiny ? "sb" : "b";
		if (Resources.Load<Sprite>(toLoad) == null)
		{
			toLoad = toLoad.Replace("f", "");
			trainerStands[0].transform.FindChild("Pokemon").GetComponent<Image>().sprite = 
				Resources.Load<Sprite>(toLoad);
		} //end if
		else
		{
			trainerStands[0].transform.FindChild("Pokemon").GetComponent<Image>().sprite = 
				Resources.Load<Sprite>(toLoad);
		} //end else 

		//Set front for rest
		for (int i = 1; i < trainerStands.Count; i++)
		{
			//Update the battler
			battlers[i].UpdateActiveBattler();

			//Set sprites
			toLoad = "Sprites/Pokemon/" + battlers[i].BattlerPokemon.NatSpecies.ToString("000");
			toLoad += battlers[i].BattlerPokemon.Gender == 1 ? "f" : "";
			toLoad += battlers[i].BattlerPokemon.IsShiny ? "s" : "";
			if (Resources.Load<Sprite>(toLoad) == null)
			{
				toLoad = toLoad.Replace("f", "");
				trainerStands[i].transform.FindChild("Pokemon").GetComponent<Image>().sprite = 
					Resources.Load<Sprite>(toLoad);
			} //end if
			else
			{
				trainerStands[i].transform.FindChild("Pokemon").GetComponent<Image>().sprite = 
					Resources.Load<Sprite>(toLoad);
			} //end else 
		} //end for

		//Set player battler box
		battlerBoxes[0].transform.FindChild("HP").GetComponent<Text>().text = battlers[0].CurrentHP.ToString() + "/" +
			battlers[0].TotalHP.ToString();
		battlerBoxes[0].transform.FindChild("Name").GetComponent<Text>().text = battlers[0].Nickname;
		battlerBoxes[0].transform.FindChild("Level").GetComponent<Text>().text = "Lv." + battlers[0].CurrentLevel.ToString();
		battlerBoxes[0].transform.FindChild("Gender").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Icons/gender"
			+ battlers[0].Gender);
		SetStatusIcon(battlerBoxes[0].transform.FindChild("Status").GetComponent<Image>(), battlers[0]);
		if (battlers[0].CurrentLevel != 100)
		{
			battlerBoxes[0].transform.FindChild("Experience").GetComponent<RectTransform>().localScale = new Vector3(
				(float)(battlers[0].BattlerPokemon.EXPForLevel - battlers[0].BattlerPokemon.RemainingEXP) /
				(float)battlers[0].BattlerPokemon.EXPForLevel, 1, 1);
		} //end if
		else
		{
			battlerBoxes[0].transform.FindChild("Experience").GetComponent<RectTransform>().localScale = new Vector3(0, 1, 1);
		} //end else
		float scale = (float)battlers[0].CurrentHP / (float)battlers[0].TotalHP;
		battlerBoxes[0].transform.FindChild("HPBar").GetComponent<RectTransform>().localScale = new Vector3(
			scale, 1, 1);
		if (scale < 0.25f)
		{
			battlerBoxes[0].transform.FindChild("HPBar").GetComponent<Image>().color = Color.red;
		} //end if
		else if (scale < 0.5f)
		{
			battlerBoxes[0].transform.FindChild("HPBar").GetComponent<Image>().color = Color.yellow;
		} //end else if
		else
		{
			battlerBoxes[0].transform.FindChild("HPBar").GetComponent<Image>().color = Color.green;
		} //end else 

		//Set remaining battler boxes
		for (int i = 1; i < combatants.Count; i++)
		{
			battlerBoxes[i].transform.FindChild("Name").GetComponent<Text>().text = battlers[i].Nickname;
			battlerBoxes[i].transform.FindChild("Level").GetComponent<Text>().text = "Lv." + battlers[i].CurrentLevel.ToString();
			battlerBoxes[i].transform.FindChild("Gender").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Icons/gender"
				+ battlers[i].Gender);
			SetStatusIcon(battlerBoxes[i].transform.FindChild("Status").GetComponent<Image>(), battlers[i]);
			scale = (float)battlers[i].CurrentHP / (float)battlers[i].TotalHP;
			battlerBoxes[i].transform.FindChild("HPBar").GetComponent<RectTransform>().localScale = new Vector3(
				scale, 1, 1);
			if (scale < 0.25f)
			{
				battlerBoxes[i].transform.FindChild("HPBar").GetComponent<Image>().color = Color.red;
			} //end if
			else if (scale < 0.5f)
			{
				battlerBoxes[i].transform.FindChild("HPBar").GetComponent<Image>().color = Color.yellow;
			} //end else if
			else
			{
				battlerBoxes[i].transform.FindChild("HPBar").GetComponent<Image>().color = Color.green;
			} //end else 
		} //end for
	} //end FillInBattlerData

	/***************************************
	 * Name: FillInTeam
	 * Fills in the player's team data
	 ***************************************/
	void FillInTeam(int selectedPokemon)
	{		
		//Deactivate panel
		if (previousTeamSlot == 1)
		{
			//Adjust if pokemon is fainted
			if (combatants[0].Team[previousTeamSlot - 1].Status != (int)Status.FAINT)
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
			if (combatants[0].Team[previousTeamSlot - 1].Status != (int)Status.FAINT)
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
		if (selectedPokemon == 1)
		{
			//Adjust if pokemon is fainted
			if (combatants[0].Team[selectedPokemon - 1].Status != (int)Status.FAINT)
			{
				playerTeam.transform.FindChild("Background").GetChild(selectedPokemon - 1).GetComponent<Image>().
				sprite = Resources.Load<Sprite>("Sprites/Menus/partyPanelRoundSel");
			} //end if
			else
			{
				playerTeam.transform.FindChild("Background").GetChild(selectedPokemon - 1).GetComponent<Image>().
				sprite = Resources.Load<Sprite>("Sprites/Menus/partyPanelRoundSelFnt");
			} //end else

			//Activate party ball
			playerTeam.transform.FindChild("Pokemon" + selectedPokemon).FindChild("PartyBall").
			GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/partyBallSel");
		} //end if
		//Any other slot
		else
		{
			//Adjust if pokemon is fainted
			if (combatants[0].Team[selectedPokemon - 1].Status != (int)Status.FAINT)
			{
				playerTeam.transform.FindChild("Background").GetChild(selectedPokemon - 1).GetComponent<Image>().
				sprite = Resources.Load<Sprite>("Sprites/Menus/partyPanelRectSel");
			} //end if
			else
			{
				playerTeam.transform.FindChild("Background").GetChild(selectedPokemon - 1).GetComponent<Image>().
				sprite = Resources.Load<Sprite>("Sprites/Menus/partyPanelRectSelFnt");
			} //end else

			//Activate party ball
			playerTeam.transform.FindChild("Pokemon" + selectedPokemon).FindChild("PartyBall").
			GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/partyBallSel");
		} //end else
	} //end FillInTeam(int selectedPokemon)

	/***************************************
	 * Name: UpdateDisplayedTeam
	 * Changes sprites to match switches
	 ***************************************/
	void UpdateDisplayedTeam()
	{
		for (int i = 0; i < combatants[0].Team.Count; i++)
		{
			//Activate slots
			playerTeam.transform.FindChild("Background").GetChild(i).gameObject.SetActive(true);
			playerTeam.transform.FindChild("Pokemon" + (i + 1)).gameObject.SetActive(true);

			//If on first slot
			if (i == 0)
			{
				//If pokemon in first slot is fainted
				if (combatants[0].Team[i].Status == 1)
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
				if (combatants[0].Team[i].Status == 1)
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
				GetComponent<Image>(), new PokemonBattler(combatants[0].Team[i]));

			//Set sprite
			playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("Sprite").GetComponent<Image>()
				.sprite = GetCorrectIcon(combatants[0].Team[i]);

			//Set nickname
			playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("Nickname").GetComponent<Text>().text =
				combatants[0].Team[i].Nickname;

			//Set item
			if (combatants[0].Team[i].Item != 0)
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
			localScale = new Vector3((float)combatants[0].Team[i].CurrentHP / (float)GameManager.
				instance.GetTrainer().Team[i].TotalHP, 1f, 1f);

			//Set level
			playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("Level").GetComponent<Text>().text = "Lv." +
				combatants[0].Team[i].CurrentLevel.ToString();

			//Set HP text
			playerTeam.transform.FindChild("Pokemon" + (i + 1)).FindChild("CurrentHP").GetComponent<Text>().text =
				combatants[0].Team[i].CurrentHP.ToString() + "/" + combatants[0].
				Team[i].TotalHP.ToString();
		} //end for

		//Deactivate any empty party spots
		for (int i = 5; i > combatants[0].Team.Count - 1; i--)
		{
			playerTeam.transform.FindChild("Background").GetChild(i).gameObject.SetActive(false);
			playerTeam.transform.FindChild("Pokemon" + (i + 1)).gameObject.SetActive(false);
		} //end for
	} //end UpdateDisplayedTeam

	/***************************************
	 * Name: InitializeRibbons
	 * Sets up ribbon screen for newly selected
	 * pokemon
	 ***************************************/
	void InitializeRibbons()
	{
		//Fill in ribbon screen wtih correct data
		ribbonScreen.SetActive(true);
		ribbonScreen.transform.FindChild("Name").GetComponent<Text>().text =
			combatants[0].Team[choiceNumber - 1].Nickname;
		ribbonScreen.transform.FindChild("Level").GetComponent<Text>().text =
			combatants[0].Team[choiceNumber - 1].CurrentLevel.ToString();
		ribbonScreen.transform.FindChild("Ball").GetComponent<Image>().sprite =
			Resources.Load<Sprite>("Sprites/Icons/summaryBall" + combatants[0].
				Team[choiceNumber - 1].BallUsed.ToString("00"));
		ribbonScreen.transform.FindChild("Gender").GetComponent<Image>().sprite =
			Resources.Load<Sprite>("Sprites/Icons/gender" + combatants[0].
				Team[choiceNumber - 1].Gender.ToString());
		ribbonScreen.transform.FindChild("Sprite").GetComponent<Image>().sprite =
			Resources.Load<Sprite>("Sprites/Pokemon/" + combatants[0].
				Team[choiceNumber - 1].NatSpecies.ToString("000"));
		ribbonScreen.transform.FindChild("Markings").GetComponent<Text>().text =
			combatants[0].Team[choiceNumber - 1].GetMarkingsString();
		ribbonScreen.transform.FindChild("Item").GetComponent<Text>().text =
			DataContents.GetItemGameName(combatants[0].Team[choiceNumber - 1].
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
		for (int i = 0; i < combatants[0].Team[choiceNumber - 1].GetRibbonCount(); i++)
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
					combatants[0].Team[choiceNumber - 1].GetRibbon(i)];
			} //end if
			//Create new ribbon
			else
			{
				GameObject newRibbon = Instantiate(ribbonScreen.transform.FindChild("RibbonRegion").
					GetChild(0).gameObject);
				newRibbon.transform.SetParent(ribbonScreen.transform.FindChild("RibbonRegion"));
				newRibbon.GetComponent<Image>().sprite = DataContents.ribbonSprites[
					combatants[0].Team[choiceNumber - 1].GetRibbon(i)];
				newRibbon.GetComponent<RectTransform>().localScale = Vector3.one;
				newRibbon.GetComponent<RectTransform>().localPosition = Vector3.zero;
				newRibbon.SetActive(true);
			} //end else
		} //end for
	} //end InitializeRibbons

	/***************************************
	 * Name: ReadRibbon
	 * Reads or disables ribbon data
	 ***************************************/
	void ReadRibbon()
	{
		//If text isn't displayed
		if (battleState == Battle.POKEMONRIBBONS && ribbonChoice != previousRibbonChoice && selection.activeSelf)
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
		selection.transform.position = Camera.main.WorldToScreenPoint(selectedChoice.transform.position);

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
				SetStatusIcon(summaryScreen.transform.GetChild(0).FindChild("Status").GetComponent<Image>(), 
					new PokemonBattler(pokemonChoice));
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
				SetStatusIcon(summaryScreen.transform.GetChild(1).FindChild("Status").GetComponent<Image>(), 
					new PokemonBattler(pokemonChoice));
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
				SetStatusIcon(summaryScreen.transform.GetChild(2).FindChild("Status").GetComponent<Image>(), 
					new PokemonBattler(pokemonChoice));
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
				SetStatusIcon(summaryScreen.transform.GetChild(3).FindChild("Status").GetComponent<Image>(), 
					new PokemonBattler(pokemonChoice));
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
				SetStatusIcon(summaryScreen.transform.GetChild(4).FindChild("Status").GetComponent<Image>(), 
					new PokemonBattler(pokemonChoice));
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
					DataContents.GetMoveGameName(combatants[0].Team[choiceTarget - 1].GetMove(i)),
					combatants[0].Team[choiceTarget - 1].GetMovePP(i),
					combatants[0].Team[choiceTarget - 1].GetMovePPMax(i));
			} //end for
		} //end if
		else
		{
			if (battleState == Battle.SELECTPOKEMON)
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
				choices.transform.GetChild(0).GetComponent<Text>().text = "Summary";
				choices.transform.GetChild(1).GetComponent<Text>().text = "Switch";
				choices.transform.GetChild(2).GetComponent<Text>().text = "Ribbons";
				choices.transform.GetChild(3).GetComponent<Text>().text = "Cancel";
			} //end if
		} //end else
	} //end FillInChoices

	/***************************************
	 * Name: ProcessAttack
	 * Processes effect of an attack
	 ***************************************/
	void ProcessAttack(QueueEvent toProcess)
	{
		//Decrease PP of attack used
		battlers[toProcess.battler].SetMovePP(toProcess.selection, battlers[toProcess.battler].GetMovePP(toProcess.selection) - 1);

		//Check for protect
		if (battlers[toProcess.target].CheckEffect((int)LastingEffects.Protect))
		{
			WriteBattleMessage(battlers[toProcess.target].Nickname + " protected itself!");
			return;
		} //end if

		//Check for negating abilities
		if (AbilityEffects.ResolveBlockingAbilities(15, battlers[toProcess.battler].GetMove(toProcess.selection),
			battlers[toProcess.target]))
		{
			return;
		} //end if

		//Check for typing
		float typeMod = battlers[toProcess.target].CheckDefenderTyping(battlers[toProcess.battler].GetMove(toProcess.selection),
			                battlers[toProcess.battler]);
		if (typeMod == 0)
		{
			WriteBattleMessage("But it had no effect...");
			return;
		} //end if

		//Check for accuracy
		if (!battlers[toProcess.target].CheckAccuracy(DataContents.GetMoveAccuracy(battlers[toProcess.battler].
			GetMove(toProcess.selection)), battlers[toProcess.battler].GetStage(6), DataContents.GetMoveIcon(
			   battlers[toProcess.battler].GetMove(toProcess.selection)), false))
		{
			WriteBattleMessage(battlers[toProcess.target].Nickname + "'s attack missed!");
			return;
		} //end if

		//Check for critical
		bool critical = ProcessCritical(toProcess);		

		//Resolve damage, item, ability, field
		//Resolve recoil
		//Check if either pokemon is fainted
			//Resolve ability (aftermath)
			//Resolve player targeted secondary effects
			//If opponent is still alive, process opponent secondary effects
		battlers[toProcess.battler].ProcessAttackEffect(battlers[toProcess.battler].GetMove(toProcess.selection));

		WriteBattleMessage("This would go through");
		return;

	} //end ProcessAttack(QueueEvent toProcess)

	/***************************************
	 * Name: ProcessQueue
	 * Applies each queue action
	 ***************************************/
	bool ProcessCritical(QueueEvent toProcess)
	{
		//Check for Shell Armor and Battle Armor
		if ((battlers[toProcess.target].CheckAbility(12) || battlers[toProcess.target].CheckAbility(139)) &&
		    !battlers[toProcess.battler].CheckAbility(92) && !battlers[toProcess.battler].CheckAbility(170) &&
		    !battlers[toProcess.battler].CheckAbility(178))
		{
			return false;
		} //end if

		//Check for Lucky Chant
		else if (GameManager.instance.CheckEffect((int)FieldEffects.LuckyChant, toProcess.target))
		{
			return false;
		} //end else if

		//Check for Frost Breath
		else if (battlers[toProcess.battler].GetMove(toProcess.selection) == 199)
		{
			return true;
		} //end else if

		//Run battler critical check
		else
		{
			int baseStage = DataContents.GetMoveFlag(battlers[toProcess.battler].GetMove(toProcess.selection), "h") ?
				1 : 0;
			return battlers[toProcess.battler].CheckCritical(baseStage);
		} //end else
	} //end ProcessCritical(QueueEvent toProcess)

	/***************************************
	 * Name: ProcessQueue
	 * Applies each queue action
	 ***************************************/
	IEnumerator ProcessQueue()
	{
		for (int i = 0; i < queue.Count; i++)
		{
			switch (queue[i].action)
			{
				case 0:
					WriteBattleMessage(string.Format("{0} used {1} on {2}!", battlers[queue[i].battler].Nickname, 
						DataContents.GetMoveGameName(battlers[queue[i].battler].GetMove(queue[i].selection)), 
						battlers[queue[i].target].Nickname));
					yield return new WaitForSeconds(1.5f);
					ProcessAttack(queue[i]);
					break;
				case 1:
					if (queue[i].target < 900)
					{
						int item = combatants[queue[i].battler].GetItem(queue[i].selection)[0];
						string effect = ItemEffects.BattleUseOnPokemon(combatants[queue[i].battler].Team[queue[i].target], 
							                combatants[queue[i].battler].GetItem(queue[i].selection)[0]);
						combatants[queue[i].battler].RemoveItem(item, 1);
						WriteBattleMessage(string.Format("{0} used {1} on {2}! {3}", combatants[queue[i].battler].PlayerName,
							DataContents.GetItemGameName(item),	combatants[queue[i].battler].Team[queue[i].target].Nickname, effect));
						FillInBattlerData();
						UpdateDisplayedTeam();
					} //end if
					else
					{
						string effect = ApplySpecialItem(queue[i]);
						WriteBattleMessage(string.Format("{0} used {1} on {2}", combatants[queue[i].battler].PlayerName,
							DataContents.GetItemGameName(combatants[queue[i].battler].GetItem(queue[i].selection)[0]), 
							effect));
					} //end else
					break;
				case 2:
					WriteBattleMessage(string.Format("That's enough, {0}! Go, {1}!", battlers[queue[i].battler].Nickname,
						combatants[queue[i].battler].Team[queue[i].target].Nickname));
					battlers[queue[i].battler].SwitchInPokemon(combatants[queue[i].battler].Team[queue[i].target]);
					combatants[queue[i].battler].Swap(0, queue[i].target);
					if (queue[i].battler == 1)
					{
						battleField.ResetDefaultBoosts();
					} //end if
					FillInBattlerData();
					UpdateDisplayedTeam();
					yield return new WaitForSeconds(1.5f);
					StartCoroutine(ResolveFieldEntrance());
					break;
			} //end switch

			yield return new WaitForSeconds(1.5f);
		} //end for
		ProcessEndOfRound();
	} //end ProcessQueue

	/***************************************
	 * Name: ProcessEndOfRound
	 * Apply any end of round effects or
	 * resets
	 ***************************************/
	void ProcessEndOfRound()
	{
		//Reset battler effects
		for (int i = 0; i < battlers.Count; i++)
		{
			battlers[i].EndOfRoundReset();
		} //end for

		//Reset to beginning of round
		queue.Clear();
		commandChoice.SetActive(true);
		selectedChoice = commandChoice.transform.GetChild(commandInt).gameObject;
		battleState = Battle.ROUNDSTART;
		processing = false;
	} //end ProcessEndOfRound

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
			if(battleState == Battle.MOVESWITCH)
			{
				//Return to summary
				selectedChoice.GetComponent<Image>().color = Color.clear;
				summaryScreen.transform.GetChild(5).gameObject.SetActive(false);
				summaryScreen.transform.GetChild(4).gameObject.SetActive(false);
				selection.SetActive(false);

				//Change to new page
				summaryChoice = summaryPage;
				battleState = Battle.POKEMONSUMMARY;
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
	 * Name: FadePlayer
	 * Plays the correct player fade animation
	 ***************************************/
	IEnumerator FadePlayer()
	{
		yield return new WaitForSeconds(1f);
		AnimatorOverrideController newController = new AnimatorOverrideController();
		newController.runtimeAnimatorController = trainerStands[0].GetComponent<Animator>().runtimeAnimatorController;
		newController["StopPlayerFade"] = Resources.Load<AnimationClip>("Animations/TrainerBack" + 
			combatants[0].PlayerImage.ToString());
		trainerStands[0].GetComponent<Animator>().runtimeAnimatorController = newController;
		trainerStands[0].GetComponent<Animator>().SetTrigger("FadeEnemy");
		WriteBattleMessage("Go, " + battlers[0].Nickname + "!");
		yield return new WaitForSeconds(newController["StopPlayerFade"].length+0.5f);
		checkpoint = 3;
		processing = false;
	} //end FadePlayer

	/***************************************
	 * Name: FadeInBagPocket
	 * Plays a fade in animation for the bag
	 * after a delay because Invoke can't take
	 * parameters and a delay.
	 ***************************************/
	public IEnumerator FadeInBagPocket()
	{
		pocketChange = true;
		choiceNumber = 0;
		bottomShown = 9;
		topShown = 0;
		yield return new WaitForSeconds(Time.deltaTime);
		GameManager.instance.FadeInAnimation(4);
		pocketChange = false;
	} //end FadeInBagPocket

	/***************************************
	* Name: ProcessUseItem
	* Processes what to do when item is used
	* in battle
	***************************************/
	IEnumerator ProcessUseItem()
	{
		//Process at end of frame
		yield return new WaitForEndOfFrame();

		if (choiceNumber < combatants[0].SlotCount() && !pocketChange)
		{
			//Get item type
			int itemType = DataContents.ExecuteSQL<int>("SELECT battleUse FROM Items WHERE id=" + combatants[0].GetItem(choiceNumber)[0]);

			//If player can use it in battle
			if ( (itemType == 1 || itemType == 2))
			{
				playerBag.SetActive(false);
				playerTeam.SetActive(true);
				choiceTarget = 1;
				selectedChoice = playerTeam.transform.FindChild("Pokemon1").gameObject;
				battleState = Battle.ITEMUSE;
			} //end if
			else
			{
				GameManager.instance.DisplayText("Can't be used inside of battle.", true);
			} //end else
		} //end if
	} //end ProcessUseItem

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
	 * Name: SetupPickMove
	 * Sets up screen for player to pick a
	 * move to use
	 ***************************************/
	public void SetupPickMove()
	{
		pickMove = true;
		FillInChoices();
		StartCoroutine(WaitForResize());
		choices.SetActive(true);
		subMenuChoice = 0;
		battleState = Battle.PICKMOVE;
	} //end SetupPickMove

	/***************************************
	 * Name: ApplyConfirm
	 * Applies the confirm choice
	 ***************************************/
	public void ApplyConfirm(ConfirmChoice e)
	{
		//Yes selected
		if (e.Choice == 0)
		{
			//Run choice
			if (checkpoint == 4)
			{
				GameManager.instance.LoadScene("MainGame", true);
			} //end if
		} //end if

		//No selected
		else if(e.Choice== 1)
		{
			//Run choice
			if (checkpoint == 4)
			{
				//Set font color for current option to white
				commandChoice.transform.GetChild(commandInt).GetChild(0).GetComponent<Text>().color = Color.white;

				//Change selection
				commandInt = 0;

				//Set new selection font to black
				commandChoice.transform.GetChild(commandInt).GetChild(0).GetComponent<Text>().color = Color.black;

				//Update selection reference
				selectedChoice = commandChoice.transform.GetChild(commandInt).gameObject;
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
} //end class BattleScene

//Struct for storing battle round decisions
struct QueueEvent
{
	public int battler;		//Who is performing the action
	public int action;		//What action is being performed
	public int selection;	//What attack/item was used, or pokemon chosen
	public int target;		//Who receives the effects of the action
	public int priority;	//What order does this move 
	public bool success;	//Did this action succeed

	/*Actions
	 * Attack = 0
	 * UseItem = 1
	 * SwitchPokemon = 2
	 */
} //end QueueEvent struct