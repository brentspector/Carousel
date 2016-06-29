/***************************************************************************************** 
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
		MOVESWITCH,
		PROCESSQUEUE,
		ENDROUND,
		ENDFIGHT
	} //end Battle

	Battle battleState;				//Current state of the battle scene
	int checkpoint = 0;				//Manage function progress
	int commandInt;					//What choice is being selected on commandChoice
	int choiceNumber;				//What choice is being selected on the relevant screen
	int battleType;					//Singles, Doubles, Triples, or other type
	int currentAttack;				//What move is currently being used
	int previousTeamSlot;       	//The slot last highlighted
	int topShown;					//The top slot displayed in the inventory
	int bottomShown;				//The bottom slot displayed in the inventory
	Field battleField;				//The active battle field
	Pokemon currentAttacker;		//Who is currently attacking
	Pokemon lastAttacker;			//Who was the last pokemon to attack
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
	bool processing = false;		//Whether a function is already processing something
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
				DataContents.trainerBacks[GameManager.instance.GetTrainer().PlayerImage * 5];
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
			SetStatusIcon(battlerBoxes[0].transform.FindChild("Status").GetComponent<Image>(), battlers[0].BattlerPokemon);
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
				SetStatusIcon(battlerBoxes[i].transform.FindChild("Status").GetComponent<Image>(), battlers[i].BattlerPokemon);
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

			//Fill in all team data
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
					GetComponent<Image>(), combatants[0].Team[i]);

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
					combatants[0].Team[i].CurrentHP.ToString() + "/" + GameManager.instance.GetTrainer().
					Team[i].TotalHP.ToString();
			} //end for

			//Deactivate any empty party spots
			for (int i = 5; i > combatants[0].Team.Count - 1; i--)
			{
				playerTeam.transform.FindChild("Background").GetChild(i).gameObject.SetActive(false);
				playerTeam.transform.FindChild("Pokemon" + (i + 1)).gameObject.SetActive(false);
			} //end for

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
				int currentPocket = GameManager.instance.GetTrainer().GetCurrentBagPocket();
				bagBack.sprite = Resources.Load<Sprite>("Sprites/Menus/bag" + currentPocket);

				//Fill in slots
				for (int i = 0; i < 10; i++)
				{
					if (GameManager.instance.GetTrainer().SlotCount() - 1 < topShown + i)
					{
						viewport.transform.GetChild(i).GetComponent<Text>().text = "";
					} //end if
					else
					{
						if (topShown + i == choiceNumber)
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
					List<int> item = GameManager.instance.GetTrainer().GetItem(choiceNumber);
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
					//Deactivate panel
					if (previousTeamSlot == 1)
					{
						//Adjust if pokemon is fainted
						if (combatants[0].Team[previousTeamSlot - 1].Status != 1)
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
						if (combatants[0].Team[previousTeamSlot - 1].Status != 1)
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
						if (combatants[0].Team[choiceNumber - 1].Status != 1)
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
					//Any other slot
					else
					{
						//Adjust if pokemon is fainted
						if (combatants[0].Team[choiceNumber - 1].Status != 1)
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

				//Get player input
				GetInput();
			} //end else if
			else if (battleState == Battle.PROCESSQUEUE)
			{
				Debug.Log("Your action was " + queue[0].action + " with " + DataContents.GetMoveGameName(battlers[0].BattlerPokemon.
					GetMove(queue[0].selection)));
				queue.Clear();
				choiceNumber = 0;
				commandChoice.SetActive(true);
				battleState = Battle.ROUNDSTART;
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
			 * -Player picks Fight/Bag/Pokemon/Run
			 * --Fight
			 * ----Display pokemon attacks
			 * ----Player picks an attack or cancels back to main
			 * --Bag
			 * ----Display inventory
			 * ----Player picks an item with a battle use
			 * ----Player picks pokemon to use it on
			 * --Pokemon
			 * ----Team roster is shown
			 * ----Player picks a pokemon
			 * ----If switch is chosen, queue it
			 * ----Player can view summary or cancel back to main
			 * --Run
			 * ----End the fight and return any variables to start
			 * ----Return to battle menu
			 * -AI picks Fight/Item/Switch
			 * 
			 * End of Choice
			 * -Queue events
			 * --Run
			 * --Switch (fastest goes first)
			 * ---Resolve OnEntry
			 * --Item (fastest goes first)
			 * --Attack
			 * ---Sort by Priority, then Speed
			 * ---Queue with flag indentifying the pokemon who used it
			 * Middle of round
			 * -Resolve Queue
			 * --Attack
			 * ----Resolve protect
			 * ----Resolve blocking abilities (Bulletproof & Telepathy)
			 * ----Resolve typing
			 * ----Resolve accuracy
			 * ----Resolve critical
			 * ----Resolve damage, item, ability, field
			 * ----Resolve recoil
			 * ----Check if either pokemon is fainted
			 * -----Resolve ability (aftermath)
			 * -----Resolve player targeted secondary effects
			 * -----If opponent is still alive, process opponent secondary effects
			 * --Loop until queue is empty
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
					GameManager.instance.GetTrainer().PreviousPocket();
					choiceNumber = 0;
					bottomShown = 9;
					topShown = 0;
					GameManager.instance.FadeInAnimation(4);
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
					if (choiceNumber > battlers[0].BattlerPokemon.GetMoveCount()-1)
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
					GameManager.instance.GetTrainer().NextPocket();
					choiceNumber = 0;
					bottomShown = 9;
					topShown = 0;
					GameManager.instance.FadeInAnimation(4);
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

				//Pokemon selection
				else if(battleState == Battle.SELECTPOKEMON)
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
					choiceNumber = ExtensionMethods.CapAtInt(choiceNumber + 1, GameManager.instance.GetTrainer().SlotCount() - 1);
					if (choiceNumber > bottomShown)
					{
						bottomShown = choiceNumber;
						topShown++;
					} //end if
				} //end else if

				//Pokemon selection
				else if(battleState == Battle.SELECTPOKEMON)
				{
					//Move from bottom slot to first pokemon
					if ((choiceNumber == combatants[0].Team.Count-1 && choiceNumber > 0) || 
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
					choiceNumber = ExtensionMethods.CapAtInt(choiceNumber + 1, GameManager.instance.GetTrainer().SlotCount() - 1);
					if (choiceNumber > bottomShown)
					{
						bottomShown = choiceNumber;
						topShown++;
					} //end if
				} //end if
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
							int currentPocket = GameManager.instance.GetTrainer().GetCurrentBagPocket();
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
					QueueEvent newEvent = new QueueEvent();
					newEvent.battler = 0;
					newEvent.action = 0;
					newEvent.selection = choiceNumber;
					newEvent.target = 1;
					newEvent.success = false;
					queue.Add(newEvent);
					attackSelection.SetActive(false);
					battleState = Battle.PROCESSQUEUE;
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
					switch(commandInt)
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
							int currentPocket = GameManager.instance.GetTrainer().GetCurrentBagPocket();
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
					QueueEvent newEvent = new QueueEvent();
					newEvent.battler = 0;
					newEvent.action = 0;
					newEvent.selection = choiceNumber;
					newEvent.target = 1;
					newEvent.success = false;
					queue.Add(newEvent);
					battleState = Battle.PROCESSQUEUE;
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
	public Pokemon CheckMoveUser()
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
	 * Name: SortPriority
	 * Sorts movement order based on priority
	 ***************************************/
	public List<int> SortPriority(bool nonMove)
	{
		//Create list of battlers
		List<int> movementOrder = new List<int>();
		movementOrder.AddRange(Enumerable.Range(0,battlers.Count-1));

		//If based only on speed
		if (nonMove)
		{
			//Sort based on speed
			movementOrder = movementOrder.OrderBy(x => battlers[x].Speed).ToList();
			Debug.Log(movementOrder.ToString());
			return movementOrder;
		} //end if

		//Evaluate moves, items, and abilities
		else
		{
			return movementOrder;
		} //end else
	} //end SortPriority(bool nonMove)

	/***************************************
	 * Name: ResolveFieldEntrance
	 * Activates entrance effects of fields,
	 * abilities, and items
	 ***************************************/
	IEnumerator ResolveFieldEntrance()
	{
		WriteBattleMessage("Field entrance effects are processing");
		battlers = battleField.ResolveFieldEntrance(battlers);
		yield return new WaitForSeconds(2.5f);
		StartCoroutine(EntranceAbilities());
	} //end ResolveFieldEntrance

	/***************************************
	 * Name: EntranceAbilities
	 * Activates entrance abilities based on 
	 * speed
	 ***************************************/
	IEnumerator EntranceAbilities()
	{
		WriteBattleMessage("Abilities are processing");
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
		WriteBattleMessage("Items are processing");
		yield return new WaitForSeconds(1.5f);
		checkpoint = 4;
		processing = false;
	} //end EntranceItems

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
	 * Name: FadePlayer
	 * Plays the correct player fade animation
	 ***************************************/
	IEnumerator FadePlayer()
	{
		yield return new WaitForSeconds(1f);
		AnimatorOverrideController newController = new AnimatorOverrideController();
		newController.runtimeAnimatorController = trainerStands[0].GetComponent<Animator>().runtimeAnimatorController;
		newController["StopPlayerFade"] = Resources.Load<AnimationClip>("Animations/TrainerBack" + 
			GameManager.instance.GetTrainer().PlayerImage.ToString());
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
		choiceNumber = 0;
		bottomShown = 9;
		topShown = 0;
		yield return new WaitForSeconds(Time.deltaTime);
		GameManager.instance.FadeInAnimation(4);
	} //end FadeInBagPocket

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

struct QueueEvent
{
	public int battler;		//Who is performing the action
	public int action;		//What action is being performed
	public int selection;	//What attack/item was used, or pokemon chosen
	public int target;		//Who receives the effects of the action
	public bool success;	//Did this action succeed

	/*Actions
	 * Attack = 0
	 * UseItem = 1
	 * SwitchPokemon = 2
	 */
} //end QueueEvent struct