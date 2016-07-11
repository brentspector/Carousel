/***************************************************************************************** 
 * File:    PCScene.cs
 * Summary: Handles process for PC scene
 *****************************************************************************************/
#region Using
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
#endregion

public class PCScene : MonoBehaviour
{
	#region Variables
    //PC game states
    public enum PCGame
    {
        HOME,
        PARTY,
        POKEMONSUBMENU,
        POKEMONSUMMARY,
        POKEMONRIBBONS,
        POKEMONMARKINGS,
        MOVESWITCH,
		ITEMCHOICE,
		ITEMGIVE,
        POKEMONHELD,
		INPUTSCREEN
    } //end PCGame

	int checkpoint = 0;				//Manage function progress
	int choiceNumber;				//What item the player is currently on
	int summaryChoice;              //What page is open on the summary screen
	int boxChoice;                  //The pokemon that is highlighted
	int subMenuChoice;              //What choice is highlighted in the pokemon submenu
	int moveChoice;                 //What move is being highlighted for details 
	int detailsSize;				//Font size for move description
	int switchChoice;               //The move chosen to switch with the selected
	int ribbonChoice;               //The ribbon currently shown
	int previousRibbonChoice;       //The ribbon last highlighted for reading
	int inventorySpot;				//What slot in pocket the player is on
	int topShown;					//The top slot displayed in the inventory
	int bottomShown;				//The bottom slot displayed in the inventory
	bool initialize;				//Initialize each state once per access
    PCGame pcState;                 //Current state of the PC
    Pokemon selectedPokemon;        //The currently selected pokemon
    Pokemon heldPokemon;            //Pokemon held if move was chosen
	GameObject choices;				//Choices box from scene tools
	GameObject selection;			//Selection rectangle from scene tools
	GameObject input;				//The input portion of scene tools
    GameObject boxBack;             //The wallpaper and pokemon panels for PC
    GameObject detailsRegion;       //Area that displays the details of a highlighted pokemon
    GameObject choiceHand;          //The highlighter for the PC
    GameObject heldImage;           //Image of the held pokemon
    GameObject partyTab;            //The panel displaying the current team in PC
	GameObject summaryScreen;		//Screen showing summary of data for pokemon
	GameObject ribbonScreen;		//Screen showing ribbons for pokemon
	GameObject playerBag;			//Screen of the player's bag
    GameObject currentPCSlot;       //The object that is currently highlighted in the PC
	GameObject currentTeamSlot;     //The object that is currently highlighted on the team
	GameObject currentMoveSlot;     //The object that is currently highlighted for reading/moving
	GameObject currentSwitchSlot;   //The object that is currently highlighted for switching to
	GameObject currentRibbonSlot;   //The object that is currently highlighted for reading
	GameObject viewport;			//The items shown to the player
	Image bagBack;					//Background bag image
	Image bagSprite;				//Item currently highlighted
	Text bagDescription;			//The item's description
	InputField inputText;			//The actual text of input
    List<bool> markingChoices;      //A list of the marking choices made for the pokemon
	#endregion

	#region Methods
	/***************************************
	 * Name: RunPC
	 * Play the PC scene
	 ***************************************/
	public void RunPC()
	{
		//Setup PC Scene
		if (checkpoint == 0)
		{
			//Set checkpoint delegate
			GameManager.instance.checkDel = ChangeCheckpoint;

			//Set confirm delegate
			GameManager.instance.confirmDel = ApplyConfirm;

			//Get references
			choices = GameManager.tools.transform.FindChild("ChoiceUnit").gameObject;
			selection = GameManager.tools.transform.FindChild("Selection").gameObject;
			input = GameManager.tools.transform.FindChild("Input").gameObject;
			inputText = input.transform.GetChild(1).GetComponent<InputField>();
			boxBack = GameObject.Find("BoxBack");
			detailsRegion = GameObject.Find("Details");
			partyTab = GameObject.Find("PartyTab");
			heldImage = GameObject.Find("HeldPokemon");
			choiceHand = heldImage.transform.GetChild(0).gameObject;
			summaryScreen = GameObject.Find("Summary");
			ribbonScreen = GameObject.Find("Ribbons");
			playerBag = GameObject.Find("PlayerBag");
			viewport = playerBag.transform.FindChild("InventoryRegion").gameObject;
			bagBack = playerBag.transform.FindChild("BagBack").GetComponent<Image>();
			bagSprite = playerBag.transform.FindChild("BagSprite").GetComponent<Image>();
			bagDescription = playerBag.transform.FindChild("BagDescription").GetComponent<Text>();

			//Initialize pcState
			pcState = PCGame.HOME;

			//Details size has not been set yet
			detailsSize = -1;

			//No choice made yet
			choiceNumber = 0;

			//Move to next checkpoint
			checkpoint = 1;
		} //end if

		//Initialize starting state
		else if (checkpoint == 1)
		{
			//Return if processing animation
			if (GameManager.instance.IsProcessing())
			{
				return;
			} //end if

			//Nothing has been initialized
			initialize = false;

			//Disable screens
			partyTab.SetActive(false);
			summaryScreen.SetActive(false);
			ribbonScreen.SetActive(false);
			playerBag.SetActive(false);

			//Fill in box
			for (int i = 0; i < 30; i++)
			{
				//Get the pokemon in the slot
				selectedPokemon = GameManager.instance.GetTrainer().GetPC(
					GameManager.instance.GetTrainer().GetPCBox(), i);

				//If the slot is null, set the sprite to clear
				if (selectedPokemon == null)
				{
					boxBack.transform.FindChild("PokemonRegion").GetChild(i).GetComponent<Image>().color =
						Color.clear;
				} //end if

				//Otherwise fill in the icon for the pokemon
				else
				{
					boxBack.transform.FindChild("PokemonRegion").GetChild(i).GetComponent<Image>().color =
						Color.white;
					boxBack.transform.FindChild("PokemonRegion").GetChild(i).GetComponent<Image>().sprite =
						GetCorrectIcon(selectedPokemon);
				} //end else
			} //end for

			//Fill in box definitions
			boxBack.transform.FindChild("BoxName").GetComponent<Text>().text = GameManager.instance.GetTrainer().
				GetPCBoxName();
			boxBack.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/box" +
			GameManager.instance.GetTrainer().GetPCBoxWallpaper());
			FillDetails();

			//Fill in party tab
			for (int i = 1; i < GameManager.instance.GetTrainer().Team.Count + 1; i++)
			{
				partyTab.transform.FindChild("Pokemon" + i).GetComponent<Image>().sprite =
					GetCorrectIcon(GameManager.instance.GetTrainer().Team[i - 1]);
			} //end for

			//Deactivate any empty party spots
			for (int i = 6; i > GameManager.instance.GetTrainer().Team.Count; i--)
			{
				partyTab.transform.FindChild("Pokemon" + i).gameObject.SetActive(false);
			} //end for

			//Initialize box choice to boxName
			heldImage.transform.position = new Vector3(boxBack.transform.FindChild("BoxName").position.x,
				boxBack.transform.FindChild("BoxName").position.y + 8, 100);
			boxChoice = -2;

			//Current slot is title
			currentPCSlot = boxBack.transform.FindChild("BoxName").gameObject;

			//Move to next checkpoint
			GameManager.instance.FadeInAnimation(2);
		} //end else if

		//Process according to PC state
		else if (checkpoint == 2)
		{
			//If on PC home
			if (pcState == PCGame.HOME)
			{
				//Get player input
				GetInput();

				//Disable Party and Return buttons
				detailsRegion.transform.FindChild("Buttons").GetChild(0).GetComponent<Button>().
                    interactable = false;
				detailsRegion.transform.FindChild("Buttons").GetChild(1).GetComponent<Button>().
                    interactable = false;
				detailsRegion.transform.FindChild("Buttons").GetChild(0).GetComponent<Image>().color =
                    Color.grey;
				detailsRegion.transform.FindChild("Buttons").GetChild(1).GetComponent<Image>().color =
                    Color.grey;

				//Position choice hand
				switch (boxChoice)
				{
					//Left box arrow
					case -3:
						//Load previous PC box
						GameManager.instance.GetTrainer().PreviousBox();

						//Reset scene
						checkpoint = 1;
						break;
					//Box title
					case -2:
						heldImage.transform.position = new Vector3(boxBack.transform.FindChild("BoxName").position.x,
							boxBack.transform.FindChild("BoxName").position.y + 8, 100);
						currentPCSlot = boxBack.transform.FindChild("BoxName").gameObject;
						selectedPokemon = null;

						//Update details
						FillDetails();
						break;
					//Right box arrow
					case -1:
						//Load next PC box
						GameManager.instance.GetTrainer().NextBox();

						//Reset scene
						checkpoint = 1;
						break;
					//Party button
					case 30:
						//Activate party button
						detailsRegion.transform.FindChild("Buttons").GetChild(0).GetComponent<Button>().interactable = true;
						detailsRegion.transform.FindChild("Buttons").GetChild(0).GetComponent<Image>().color =
							Color.white;

						//Move choice hand to party button
						heldImage.transform.position = new Vector3(detailsRegion.transform.FindChild("Buttons").GetChild(0).position.x,
							detailsRegion.transform.FindChild("Buttons").GetChild(0).position.y + 8, 100);
						currentPCSlot = detailsRegion.transform.FindChild("Buttons").GetChild(0).gameObject;
						selectedPokemon = null;

						//Update details region
						FillDetails();
						break;
					//Back button
					case 31:
						//Activate back button
						detailsRegion.transform.FindChild("Buttons").GetChild(1).GetComponent<Button>().interactable = true;
						detailsRegion.transform.FindChild("Buttons").GetChild(1).GetComponent<Image>().color =
							Color.white;

						//Move choice hand to back button
						heldImage.transform.position = new Vector3(detailsRegion.transform.FindChild("Buttons").GetChild(1).position.x,
							detailsRegion.transform.FindChild("Buttons").GetChild(1).position.y + 8, 100);
						currentPCSlot = detailsRegion.transform.FindChild("Buttons").GetChild(1).gameObject;
						selectedPokemon = null;

						//Update details region
						FillDetails();
						break;
					//Pokemon region
					default:
						//Position hand
						heldImage.transform.position = new Vector3(
							boxBack.transform.FindChild("PokemonRegion").GetChild(boxChoice).position.x,
							boxBack.transform.FindChild("PokemonRegion").GetChild(boxChoice).position.y + 8, 100);

						//Get the pokemon in the slot
						selectedPokemon = GameManager.instance.GetTrainer().GetPC(
							GameManager.instance.GetTrainer().GetPCBox(), boxChoice);

						//Update currentPCSlot
						currentPCSlot = boxBack.transform.FindChild("PokemonRegion").GetChild(boxChoice).gameObject;

						//Update details region
						FillDetails();
						break;
				} //end switch
			} //end if

			//Pokemon submenu
			else if (pcState == PCGame.POKEMONSUBMENU)
			{
				//Initialize only once
				if (!initialize)
				{
					//Initialized
					initialize = true;

					//Disable party close button
					partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = false;

					//Reposition choices to bottom right
					StartCoroutine(PositionChoices());

					//Reposition selection to top menu choice
					selection.transform.position = choices.transform.GetChild(0).position;
				} //end if !initialize

				//Get player input
				GetInput();
			} //end else if

			//Pokemon summary
			else if (pcState == PCGame.POKEMONSUMMARY)
			{
				//Get player input
				GetInput();

				//Update selected pokemon
				if (heldPokemon != null)
				{
					selectedPokemon = heldPokemon;
				} //end if
				else if (partyTab.activeSelf)
				{
					selectedPokemon = GameManager.instance.GetTrainer().Team[choiceNumber - 1];
				} //end else if
				else
				{
					selectedPokemon = GameManager.instance.GetTrainer().GetPC(
						GameManager.instance.GetTrainer().GetPCBox(), boxChoice);
				} //end else

				//Display pokemon summary
				PokemonSummary(selectedPokemon);
			} //end else if

			//Switch pokemon moves 
			else if (pcState == PCGame.MOVESWITCH)
			{
				//Get player input
				GetInput();

				//Highlight selected switch to
				selection.SetActive(true);
				Transform moveScreen = summaryScreen.transform.GetChild(5);
				Vector3 scale = new Vector3(
					                moveScreen.FindChild("Move" + (moveChoice + 1)).GetComponent<RectTransform>().rect.width,
					                moveScreen.FindChild("Move" + (moveChoice + 1)).GetComponent<RectTransform>().rect.height, 0); 
				selection.GetComponent<RectTransform>().sizeDelta = scale;
				selection.transform.position = Camera.main.WorldToScreenPoint(currentSwitchSlot.transform.position);

			} //end else if

			//Party tab open
			else if (pcState == PCGame.PARTY)
			{
				//Get player input
				GetInput();
			
				//Put choice hand at party slot position
				heldImage.transform.position = new Vector3(currentTeamSlot.transform.position.x,
					currentTeamSlot.transform.position.y + 8, 100);
			
				//Selected pokemon is same as choice
				if (choiceNumber > 0 && choiceNumber <= GameManager.instance.GetTrainer().Team.Count)
				{
					selectedPokemon = GameManager.instance.GetTrainer().Team[choiceNumber - 1];
				} //end if
				else
				{
					selectedPokemon = null;
				} //end else
			
				//Update details region
				FillDetails();
			} //end else if
			
			//If a pokemon is selected to hold or place
			else if (pcState == PCGame.POKEMONHELD)
			{
				//If no pokemon is currently held
				if (heldPokemon == null)
				{
					//If in pokemon party
					if (partyTab.activeSelf)
					{
						//If on last pokemon
						if (GameManager.instance.GetTrainer().Team.Count == 1)
						{
							GameManager.instance.DisplayText("You can't remove your last pokemon.", true);
						} //end if
			
						heldPokemon = selectedPokemon;
						selectedPokemon = null;
						GameManager.instance.GetTrainer().RemovePokemon(choiceNumber - 1);
						choiceHand.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/boxfist");
						heldImage.GetComponent<Image>().color = Color.white;
						heldImage.GetComponent<Image>().sprite = GetCorrectIcon(heldPokemon);
			
						//Fill in party tab
						for (int i = 1; i < GameManager.instance.GetTrainer().Team.Count + 1; i++)
						{
							partyTab.transform.FindChild("Pokemon" + i).GetComponent<Image>().sprite = 
								GetCorrectIcon(GameManager.instance.GetTrainer().Team[i - 1]);
						} //end for
			
						//Deactivate any empty party spots
						for (int i = 6; i > GameManager.instance.GetTrainer().Team.Count; i--)
						{
							partyTab.transform.FindChild("Pokemon" + i).gameObject.SetActive(false);
						} //end for
					} //end if
			
					//Pokemon Region of box
					else
					{
						heldPokemon = selectedPokemon;
						selectedPokemon = null;
						GameManager.instance.GetTrainer().RemoveFromPC(
							GameManager.instance.GetTrainer().GetPCBox(), boxChoice);
						choiceHand.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/boxfist");
						heldImage.GetComponent<Image>().color = Color.white;
						heldImage.GetComponent<Image>().sprite = GetCorrectIcon(heldPokemon);
						boxBack.transform.FindChild("PokemonRegion").GetChild(boxChoice).GetComponent<Image>().color = Color.clear;
					} //end else
			
					//Move choice now becomes swap
					choices.transform.GetChild(0).GetComponent<Text>().text = "Swap";
				} //end if
			
				//Otherwise switch pokemon
				else
				{
					//If in pokemon party
					if (partyTab.activeSelf)
					{
						//If there is a pokemon
						if (selectedPokemon != null)
						{
							GameManager.instance.GetTrainer().ReplacePokemon(heldPokemon, choiceNumber - 1);
							heldPokemon = selectedPokemon;
							selectedPokemon = GameManager.instance.GetTrainer().Team[choiceNumber - 1];
							heldImage.GetComponent<Image>().sprite = GetCorrectIcon(heldPokemon);
							partyTab.transform.FindChild("Pokemon" + choiceNumber).GetComponent<Image>().sprite =
								GetCorrectIcon(selectedPokemon);
						} //end if
						else
						{
							GameManager.instance.GetTrainer().AddPokemon(heldPokemon);
							selectedPokemon = heldPokemon;
							heldPokemon = null;
							choiceHand.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/boxpoint1");
							heldImage.GetComponent<Image>().color = Color.clear;
							partyTab.transform.FindChild("Pokemon" + choiceNumber).GetComponent<Image>().sprite =
								GetCorrectIcon(selectedPokemon);
							partyTab.transform.FindChild("Pokemon" + choiceNumber).gameObject.SetActive(true);
						} //end else
					} //end if
			
					//Pokemon Region of box
					else
					{
						//If there is a pokemon
						if (selectedPokemon != null)
						{
							GameManager.instance.GetTrainer().RemoveFromPC(
								GameManager.instance.GetTrainer().GetPCBox(), boxChoice);
							GameManager.instance.GetTrainer().AddToPC(
								GameManager.instance.GetTrainer().GetPCBox(), boxChoice, heldPokemon);
							heldPokemon = selectedPokemon;
							selectedPokemon = GameManager.instance.GetTrainer().GetPC(
								GameManager.instance.GetTrainer().GetPCBox(), boxChoice);
							heldImage.GetComponent<Image>().sprite = GetCorrectIcon(heldPokemon);
							boxBack.transform.FindChild("PokemonRegion").GetChild(boxChoice).GetComponent<Image>().sprite =
								GetCorrectIcon(selectedPokemon);
						} //end if
						else
						{
							GameManager.instance.GetTrainer().AddToPC(
								GameManager.instance.GetTrainer().GetPCBox(), boxChoice, heldPokemon);
							selectedPokemon = heldPokemon;
							heldPokemon = null;
							choiceHand.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/boxpoint1");
							heldImage.GetComponent<Image>().color = Color.clear;
							boxBack.transform.FindChild("PokemonRegion").GetChild(boxChoice).GetComponent<Image>().sprite =
								GetCorrectIcon(selectedPokemon);
							boxBack.transform.FindChild("PokemonRegion").GetChild(boxChoice).GetComponent<Image>().color = Color.white;
			
							//Swap choice now becomes move
							choices.transform.GetChild(0).GetComponent<Text>().text = "Move";
						} //end else
					} //end else
				} //end else
			
				//Enable party close button
				partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;

				//Go back to previous state
				pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
			} //end else if
			
			//Pokemon ribbons
			else if (pcState == PCGame.POKEMONRIBBONS)
			{
				if (!initialize)
				{
					//Fill in ribbon screen with correct data
					ribbonScreen.SetActive(true);
					ribbonScreen.transform.FindChild("Name").GetComponent<Text>().text =
        	            selectedPokemon.Nickname;
					ribbonScreen.transform.FindChild("Level").GetComponent<Text>().text =
        	            selectedPokemon.CurrentLevel.ToString();
					ribbonScreen.transform.FindChild("Ball").GetComponent<Image>().sprite =
        	            Resources.Load<Sprite>("Sprites/Icons/summaryBall" + selectedPokemon.BallUsed.ToString("00"));
					ribbonScreen.transform.FindChild("Gender").GetComponent<Image>().sprite =
        	            Resources.Load<Sprite>("Sprites/Icons/gender" + selectedPokemon.Gender.ToString());
					ribbonScreen.transform.FindChild("Sprite").GetComponent<Image>().sprite =
        	            Resources.Load<Sprite>("Sprites/Pokemon/" + selectedPokemon.NatSpecies.ToString("000"));
					ribbonScreen.transform.FindChild("Markings").GetComponent<Text>().text =
        	            selectedPokemon.GetMarkingsString();
					ribbonScreen.transform.FindChild("Item").GetComponent<Text>().text =
        	            DataContents.GetItemGameName(selectedPokemon.Item);
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
					} //end for
        	        
					//Add ribbons
					for (int i = 0; i < selectedPokemon.GetRibbonCount(); i++)
					{
						//If at least one ribbon exists, resize the selection box
						if (i == 0)
						{                            
							StartCoroutine("WaitForResize");
						} //end if
        	            
						//The ribbon already exists, just fill it in
						if (i < ribbonScreen.transform.FindChild("RibbonRegion").childCount)
						{
							GameObject newRibbon = ribbonScreen.transform.FindChild("RibbonRegion").GetChild(i).
        	                    gameObject;
							newRibbon.gameObject.SetActive(true);
							newRibbon.GetComponent<Image>().sprite = DataContents.ribbonSprites[
								selectedPokemon.GetRibbon(i)];
						} //end if
        	            //Create new ribbon
        	            else
						{
							GameObject newRibbon = Instantiate(ribbonScreen.transform.FindChild("RibbonRegion").
        	                    GetChild(0).gameObject);
							newRibbon.transform.SetParent(ribbonScreen.transform.FindChild("RibbonRegion"));
							newRibbon.GetComponent<Image>().sprite = DataContents.ribbonSprites[
								selectedPokemon.GetRibbon(i)];
							newRibbon.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
							newRibbon.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
							newRibbon.SetActive(true);
						} //end else
					} //end for
        	        
					//Initialization is done
					initialize = true;
				} //end if
        	    
				//Get player input
				GetInput();
			} //end else if
			
			//Pokemon markings
			else if (pcState == PCGame.POKEMONMARKINGS)
			{
				//Initialize
        	    if(!initialize)
        	    {
        	        //Fill in choices box
        	        FillInChoices();
			
        	        //SubmenuChoice at top
        	        subMenuChoice = 0;
			
        	        //Reposition choices menu and selection rectangle
					StartCoroutine(WaitForResize());
			
        	        //Initialize finished
        	        initialize = true;
        	    } //end if
			
        	    //Reposition selection rectangle
        	    selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
			
        	    //Get player input
				GetInput();
			} //end else if
			
			//Box name, wallpaper, and jump to
			else if(pcState == PCGame.INPUTSCREEN)
			{
				//Make sure text field is always active
				if(input.activeInHierarchy)
				{
					inputText.ActivateInputField();
				} //end if
			
				//Get player input
				GetInput();
			} //end else if

			//Pokemon Item Give/Take
			else if (pcState == PCGame.ITEMCHOICE)
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
			else if (pcState == PCGame.ITEMGIVE)
			{
				//Initialize
				if (!initialize)
				{
					//Initialized
					initialize = true;

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
		} //end else if checkpoint 2
	}//end RunPC

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
			//PC Home
			if (pcState == PCGame.HOME)
			{
				//If on box title
				if (boxChoice == -2)
				{
					boxChoice = -3;
				} //end if
				//If on top left slot
				else if (boxChoice == 0)
				{
					boxChoice = -2;
				} //end else if
				//Otherwise move left
				else
				{
					//Decrease (higher slots are on lower children)
					boxChoice--;
				} //end else
			} //end if Home

			//Pokemon Party on PC -> Party Tab
			else if (pcState == PCGame.PARTY)
			{
				//Decrease (higher slots are on lower children)
				choiceNumber--;

				//Loop to end of team
				if (choiceNumber < 0)
				{
					//If there is a held pokemon
					if (heldPokemon != null && GameManager.instance.GetTrainer().Team.Count < 6)
					{
						choiceNumber = GameManager.instance.GetTrainer().Team.Count + 1;
					} //end if
					else
					{
						choiceNumber = GameManager.instance.GetTrainer().Team.Count;
					} //end else
				} //end if

				//Set currentSlotChoice
				if (choiceNumber > 0)
				{
					currentTeamSlot = partyTab.transform.FindChild("Pokemon" + choiceNumber).gameObject;
				} //end if
				else if (choiceNumber == 0)
				{
					currentTeamSlot = partyTab.transform.FindChild("Close").gameObject;
				} //end else if
			} //end else if Pokemon Party on PC -> Party Tab

			//Pokemon Summary on PC -> Summary
			else if (pcState == PCGame.POKEMONSUMMARY)
			{
				//If on any page besides move details
				if(summaryChoice != 5)
				{
					//Deactivate current page
					summaryScreen.transform.GetChild(summaryChoice).gameObject.SetActive(false);

					//Decrease choice
					summaryChoice--;

					//Loop to last child if on first child
					if(summaryChoice < 0)
					{
						summaryChoice = 4;
					} //end if
				} //end if
			} // end else if Pokemon Summary on PC -> Summary

			//Pokemon Ribbons on PC -> Ribbons
			else if(pcState == PCGame.POKEMONRIBBONS)
			{
				//Decrease (higher slots are lower childs)
				ribbonChoice--;

				//Clamp at 0
				if(ribbonChoice < 0)
				{
					ribbonChoice = 0;
					previousRibbonChoice = -1;
				} //end if

				//Set currentRibbonSlot
				currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
					GetChild(ribbonChoice).gameObject;

				//Read ribbon
				ReadRibbon();
			} //end else if Pokemon Ribbons on PC -> Ribbons

			//Pokemon Item Give From Bag on Continue Game -> Team -> Inventory
			else if (pcState == PCGame.ITEMGIVE)
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
			//PC Home
			if(pcState == PCGame.HOME)
			{
				//Increase (lower slots on higher children)
				boxChoice++;

				//Clamp at 31
				if(boxChoice > 31)
				{
					boxChoice = 31;
				} //end if
			} //end if Home

			//Pokemon Party on PC -> Party Tab
			else if(pcState == PCGame.PARTY)
			{
				//Increase (lower slots are higher children)
				choiceNumber++;

				//If there is a held pokemon
				if(heldPokemon != null && GameManager.instance.GetTrainer().Team.Count < 6)
				{
					//Clamp at one over team size
					if(choiceNumber > GameManager.instance.GetTrainer().Team.Count + 1)
					{
						choiceNumber = 0;
					} //end if
				} //end if
				else
				{
					//Clamp at team size
					if(choiceNumber > GameManager.instance.GetTrainer().Team.Count)
					{
						choiceNumber = 0;
					} //end if
				} //end else

				//Set currentSlotChoice
				if(choiceNumber > 0)
				{
					currentTeamSlot = partyTab.transform.FindChild("Pokemon"+choiceNumber).gameObject;
				} //end if
				else if(choiceNumber == 0)
				{
					currentTeamSlot = partyTab.transform.FindChild("Close").gameObject;
				} //end else if
			} //end else if Pokemon Party on PC -> Party Tab

			//Pokemon Summary on PC -> Summary
			else if(pcState == PCGame.POKEMONSUMMARY)
			{
				//If on any page besides move details
				if(summaryChoice != 5)
				{
					//Deactivate current page
					summaryScreen.transform.GetChild(summaryChoice).gameObject.SetActive(false);

					//Increase choice
					summaryChoice++;

					//Loop to last child if on first child
					if(summaryChoice > 4)
					{
						summaryChoice = 0;
					} //end if
				} //end if
			} //end else if Pokemon Summary on PC -> Summary

			//Pokemon Ribbons on PC -> Ribbons
			else if(pcState == PCGame.POKEMONRIBBONS)
			{
				//Increase (lower slots are higher childs)
				ribbonChoice++;

				//Clamp at ribbon length
				if(ribbonChoice >= selectedPokemon.GetRibbonCount())
				{
					ribbonChoice = ExtensionMethods.BindToInt(selectedPokemon.GetRibbonCount()-1, 0);
					previousRibbonChoice = -1;
				} //end if

				//Set currentRibbonSlot
				currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
					GetChild(ribbonChoice).gameObject;

				//Read ribbon
				ReadRibbon();
			} //end else if Pokemon Ribbons on PC -> Ribbons

			//Pokemon Item Give From Bag on Continue Game -> Team -> Inventory
			else if (pcState == PCGame.ITEMGIVE)
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
			//PC Home
			if(pcState == PCGame.HOME)
			{
				//If on title, move to party button
				if(boxChoice == -2)
				{
					boxChoice = 30;
				} //end if
				else
				{
					//Decrease (higher slots on lower children)
					boxChoice -= 6;

					//Clamp at -2 if not on a pokemon
					if(boxChoice < 0)
					{
						boxChoice = -2;
					} //end if
				} //end else
			} //end if

			//Pokemon Party on PC -> Party Tab
			else if(pcState == PCGame.PARTY)
			{
				//If on first or second slot, go to Close button
				if(choiceNumber == 1 || choiceNumber == 2)
				{
					choiceNumber = 0;
					currentTeamSlot = partyTab.transform.FindChild("Close").gameObject;
				} //end if
				//If on Close button, go to last slot
				else if(choiceNumber == 0)
				{
					//If there is a held pokemon
					if(heldPokemon != null && GameManager.instance.GetTrainer().Team.Count < 6)
					{
						choiceNumber =  GameManager.instance.GetTrainer().Team.Count + 1;
					} //end if
					else
					{
						choiceNumber = GameManager.instance.GetTrainer().Team.Count;
					} //end else

					currentTeamSlot = partyTab.transform.FindChild("Pokemon" + choiceNumber).gameObject;
				} //end else if
				//Go up vertically
				else
				{
					choiceNumber -= 2;
					currentTeamSlot = partyTab.transform.FindChild("Pokemon"+choiceNumber).gameObject;
				} //end else
			} //end else if Pokemon Party on PC -> Party Tab

			//Pokemon Submenu on PC
			else if(pcState == PCGame.POKEMONSUBMENU)
			{
				//Decrease choice (higher slots on lower children)
				subMenuChoice--;

				//If on the first option, loop to end
				if(subMenuChoice < 0)
				{
					subMenuChoice = choices.transform.childCount-1;
				} //end if

				//Reposition selection
				selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
			} //end else if Pokemon Submenu on PC

			//Pokemon Summary on PC -> Summary
			else if(pcState == PCGame.POKEMONSUMMARY)
			{
				//If on any page besides move details
				if(summaryChoice != 5)
				{
					//If party is open
					if(partyTab.activeSelf && heldPokemon == null)
					{
						//Decrease (higher slots are lower childs)
						choiceNumber--;

						//Clamp between 1 and team size
						if(choiceNumber < 1)
						{
							choiceNumber = GameManager.instance.GetTrainer().Team.Count;
						} //end if
					} //end if
					else if(!partyTab.activeSelf && heldPokemon == null)
					{
						//Decrease to previous pokemon index
						boxChoice = GameManager.instance.GetTrainer().GetPreviousPokemon(boxChoice);
					} //end else
				} //end if
				else
				{
					//Decrease (higher slots are lower childs)
					moveChoice--;

					//Clamp between 0 and highest non-null move
					if(moveChoice < 0)
					{
						moveChoice = selectedPokemon.GetMoveCount()-1;
					} //end if

					//Set move slot
					currentMoveSlot = summaryScreen.transform.GetChild(5).
						FindChild("Move"+(moveChoice+1)).gameObject;
				} //end else
			} //end else if Pokemon Summary on PC -> Summary

			//Move Switch on PC -> Summary -> Move Details
			else if(pcState == PCGame.MOVESWITCH)
			{
				//Decrease (higher slots on lower children)
				switchChoice--;

				//Clamp between 0 and highest non-null move
				if(switchChoice < 0)
				{
					switchChoice = selectedPokemon.GetMoveCount()-1;
				} //end if

				//Set currentSwitchSlot
				currentSwitchSlot = summaryScreen.transform.GetChild(5).
					FindChild("Move"+(switchChoice+1)).gameObject;
			} //end else if Move Switch on PC -> Summary -> Move Details

			//Pokemon Ribbons on PC -> Ribbons
			else if(pcState == PCGame.POKEMONRIBBONS)
			{
				//If party is open
				if(partyTab.activeSelf && heldPokemon == null)
				{
					//Decrease (higher slots are lower childs)
					choiceNumber--;

					//Clamp between 1 and team size
					if(choiceNumber < 1)
					{
						choiceNumber = GameManager.instance.GetTrainer().Team.Count;
					} //end if

					//Update selected pokemon
					selectedPokemon = GameManager.instance.GetTrainer().Team[choiceNumber-1];
				} //end if
				//If in pokemon region
				else if(!partyTab.activeSelf && heldPokemon == null)
				{
					//Decrease to previous pokemon index
					boxChoice = GameManager.instance.GetTrainer().GetPreviousPokemon(boxChoice);

					//Update selected pokemon
					selectedPokemon = GameManager.instance.GetTrainer().GetPC(
						GameManager.instance.GetTrainer().GetPCBox(), boxChoice);
				} //end else if

				//Reload ribbons
				initialize = false;
			} //end else if Pokemon Ribbons on PC -> Ribbons

			//Pokemon Markings on PC -> Submenu
			else if(pcState == PCGame.POKEMONMARKINGS)
			{
				//Decrease choice (higher slots on lower children)
				subMenuChoice--;

				//If on the first option, loop to end
				if(subMenuChoice < 0)
				{
					subMenuChoice = choices.transform.childCount-1;
				} //end if
			} //end else if Pokemon Markings on PC -> Submenu

			//Pokemon Take/Give on Continue Game -> Team -> Item
			else if (pcState == PCGame.ITEMCHOICE)
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
			else if (pcState == PCGame.ITEMGIVE)
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
			//PC Home
			if(pcState == PCGame.HOME)
			{
				//If on party button, move to box title
				if(boxChoice == 30)
				{
					boxChoice = -2;
				} //end if

				//Otherwise increase (lower slots on higher children)
				else
				{
					boxChoice += 6;

					//Clamp to 30 (party button)
					if(boxChoice > 29)
					{
						boxChoice = 30;
					} //end if
				} //end else
			} //end if PC Home

			//Pokemon Party on PC -> Party Tab
			else if(pcState == PCGame.PARTY)
			{
				//If there is a held pokemon
				if(heldPokemon != null && GameManager.instance.GetTrainer().Team.Count < 6)
				{
					//If on last, or one after last slot, go to Close button
					if(choiceNumber == GameManager.instance.GetTrainer().Team.Count ||
						choiceNumber == GameManager.instance.GetTrainer().Team.Count+1)
					{
						choiceNumber = 0;
						currentTeamSlot = partyTab.transform.FindChild("Close").gameObject;
					} //end if
					//If on Close button, go to first slot
					else if(choiceNumber == 0)
					{
						choiceNumber = 1;
						currentTeamSlot = partyTab.transform.FindChild("Pokemon1").gameObject;
					} //end else if
					//Go down vertically
					else
					{
						choiceNumber += 2;
						currentTeamSlot = partyTab.transform.FindChild("Pokemon"+choiceNumber).gameObject;
					} //end else
				} //end if
				//If on last, or second to last team slot, go to Close button
				else if((choiceNumber == GameManager.instance.GetTrainer().Team.Count - 1
					&& choiceNumber > 0)
					|| choiceNumber == GameManager.instance.GetTrainer().Team.Count)
				{
					choiceNumber = 0;
					currentTeamSlot = partyTab.transform.FindChild("Close").gameObject;
				} //end else if
				//If on Close button, go to first slot
				else if(choiceNumber == 0)
				{
					choiceNumber = 1;
					currentTeamSlot = partyTab.transform.FindChild("Pokemon1").gameObject;
				} //end else if
				//Go down vertically
				else
				{
					choiceNumber += 2;
					currentTeamSlot = partyTab.transform.FindChild("Pokemon"+choiceNumber).gameObject;
				} //end else
			} //end else if Pokemon Party on PC -> Party Tab

			//Pokemon Submenu on PC
			else if(pcState == PCGame.POKEMONSUBMENU)
			{
				//Increase choice (lower slots on higher children)
				subMenuChoice++;

				//If on the last option, loop to first
				if(subMenuChoice > choices.transform.childCount-1)
				{
					subMenuChoice = 0;
				} //end if

				//Reposition selection
				selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
			} //end else if Pokemon Submenu on PC

			//Pokemon Summary on PC -> Summary
			else if(pcState == PCGame.POKEMONSUMMARY)
			{
				//If on any page besides move details
				if(summaryChoice != 5)
				{
					//If party is open
					if(partyTab.activeSelf && heldPokemon == null)
					{
						//Increase (lower slots are on higher childs)
						choiceNumber++;

						//Clamp between 1 and team size
						if(choiceNumber > GameManager.instance.GetTrainer().Team.Count)
						{
							choiceNumber = 1;
						} //end if
					} //end if
					//If in pokemon region
					else if(!partyTab.activeSelf && heldPokemon == null)
					{
						//Increase to next pokemon slot
						boxChoice = GameManager.instance.GetTrainer().GetNextPokemon(boxChoice);
					} //end else if
				} //end else if
				else
				{
					//Increase (lower slots are on higher childs)
					moveChoice++;

					//If chosen move is null, loop to top
					if(moveChoice >= 4 || selectedPokemon.GetMove(moveChoice) == -1)
					{
						moveChoice = 0;
					} //end if

					//Set move slot
					currentMoveSlot = summaryScreen.transform.GetChild(5).
						FindChild("Move"+(moveChoice+1)).gameObject;
				} //end else
			} //end else if Pokemon Summary on PC -> Summary

			//Move Switch on PC -> Summary -> Move Details
			else if(pcState == PCGame.MOVESWITCH)
			{
				//Increase (lower slots on higher children)
				switchChoice++;

				//Clamp between 0 and highest non-null move
				if(switchChoice > selectedPokemon.GetMoveCount()-1)
				{
					switchChoice = 0;
				} //end if

				//Set currentSwitchSlot
				currentSwitchSlot = summaryScreen.transform.GetChild(5).
					FindChild("Move"+(switchChoice+1)).gameObject;
			} //end else if Move Switch on PC -> Summary -> Move Details

			//Pokemon Ribbons in PC -> Ribbons
			else if(pcState == PCGame.POKEMONRIBBONS)
			{
				//If party is open
				if(partyTab.activeSelf && heldPokemon == null)
				{
					//Increase (lower slots are on higher childs)
					choiceNumber++;

					//Clamp between 1 and team size
					if(choiceNumber > GameManager.instance.GetTrainer().Team.Count)
					{
						choiceNumber = 1;
					} //end if

					//Update selected pokemon
					selectedPokemon = GameManager.instance.GetTrainer().Team[choiceNumber-1];
				} //end if
				//If in pokemon region
				else if(!partyTab.activeSelf && heldPokemon == null)
				{
					//Increase to next pokemon slot
					boxChoice = GameManager.instance.GetTrainer().GetNextPokemon(boxChoice);

					//Update selected pokemon
					selectedPokemon = GameManager.instance.GetTrainer().GetPC(
						GameManager.instance.GetTrainer().GetPCBox(), boxChoice);
				} //end else if

				//Reload ribbons
				initialize = false;
			} //end else if Pokemon Ribbons in PC -> Ribbons

			//Pokemon Markings on PC -> Submenu
			else if(pcState == PCGame.POKEMONMARKINGS)
			{
				//Increase choice (lower slots on higher children)
				subMenuChoice++;

				//If on the last option, loop to first
				if(subMenuChoice > choices.transform.childCount-1)
				{
					subMenuChoice = 0;
				} //end if
			} //end else if Pokemon Markings on PC -> Submenu

			//Pokemon Take/Give on Continue Game -> Team -> Item
			else if (pcState == PCGame.ITEMCHOICE)
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
			else if (pcState == PCGame.ITEMGIVE)
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
			//PC Home
			if(pcState == PCGame.HOME && Input.mousePosition.x < 
				Camera.main.WorldToScreenPoint(currentPCSlot.transform.position).x - 
				currentPCSlot.GetComponent<RectTransform>().rect.width/2)
			{
				//If not at bottom of PC or on left side
				if(boxChoice < 30 && boxChoice % 6 != 0)
				{
					boxChoice--;
				} //end if
				else if(boxChoice == 31)
				{
					//Set to party button
					boxChoice = 30;
				} //end else if
				else if(boxChoice == -2)
				{
					//Set to left box
					boxChoice = -3;
				} //end else
			} //end if

			//Pokemon Party on PC -> Party Tab
			else if(pcState == PCGame.PARTY && Input.mousePosition.x < 
				Camera.main.WorldToScreenPoint(currentTeamSlot.transform.position).x - 
				currentTeamSlot.GetComponent<RectTransform>().rect.width/2)
			{
				//If choice number is not odd, and is greater than 0, move left
				if((choiceNumber&1) != 1 && choiceNumber > 0)
				{
					choiceNumber--;
					currentTeamSlot = partyTab.transform.FindChild("Pokemon"+choiceNumber).gameObject;
				} //end if   
			} //end else if Pokemon Party on PC -> Party Tab

			//Pokemon Ribbons on PC -> Ribbons
			else if(pcState == PCGame.POKEMONRIBBONS &&
				Input.mousePosition.x < Camera.main.WorldToScreenPoint(currentRibbonSlot.transform.
					position).x - currentRibbonSlot.GetComponent<RectTransform>().rect.width/2)
			{
				//If next slot is null, don't move
				if(ribbonChoice-1 > -1 && ribbonChoice % 4 != 0)
				{
					ribbonChoice--;

					//Read ribbon
					ReadRibbon();
				} //end if

				//Set currentRibbonSlot
				currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
					GetChild(ribbonChoice).gameObject;
			} //end else if Pokemon Ribbons on PC -> Ribbons
		} //end else if Mouse Moves Left

		/*********************************************
		* Mouse Moves Right
		**********************************************/
		if (Input.GetAxis("Mouse X") > 0)
		{
			//PC Home
			if(pcState == PCGame.HOME && Input.mousePosition.x > 
				Camera.main.WorldToScreenPoint(currentPCSlot.transform.position).x + 
				currentPCSlot.GetComponent<RectTransform>().rect.width/2)
			{
				//If not at bottom of PC or on right side
				if(boxChoice < 30 && boxChoice % 6 != 5)
				{
					boxChoice++;
				} //end if
				else if(boxChoice == 30)
				{
					//Set to return button
					boxChoice = 31;
				} //end else if
				else if(boxChoice == -2)
				{
					//Set to right box
					boxChoice = -1;
				} //end else
			} //end if

			//Pokemon Party on PC -> Party Tab
			else if(pcState == PCGame.PARTY && Input.mousePosition.x > Camera.main.
				WorldToScreenPoint(currentTeamSlot.transform.position).x + currentTeamSlot.
				GetComponent<RectTransform>().rect.width/2)
			{
				//If choice is odd, and currently less than or equal to team size, and a pokemon is held, move right
				if((choiceNumber&1) == 1 && choiceNumber <= GameManager.instance.GetTrainer().Team.Count && 
					heldPokemon != null)
				{
					choiceNumber++;
					currentTeamSlot = partyTab.transform.FindChild("Pokemon"+choiceNumber).gameObject;
				} //end if
				//If choice is odd and team is not odd numbered and choice is greater than 0, move right
				else  if((choiceNumber&1) == 1 && choiceNumber != GameManager.instance.GetTrainer().Team.Count && 
					choiceNumber > 0 && heldPokemon == null)
				{
					choiceNumber++;
					currentTeamSlot = partyTab.transform.FindChild("Pokemon"+choiceNumber).gameObject;
				} //end else if
			} //end else if Pokemon Party on PC -> Party Tab

			//Pokemon Ribbons on PC -> Ribbons
			else if(pcState == PCGame.POKEMONRIBBONS &&
				Input.mousePosition.x > Camera.main.WorldToScreenPoint(currentRibbonSlot.transform.
					position).x + currentRibbonSlot.GetComponent<RectTransform>().rect.width/2)
			{
				//If next slot is null, don't move
				if(ribbonChoice + 1 < selectedPokemon.GetRibbonCount() && ribbonChoice % 4 != 3)
				{
					ribbonChoice++;

					//Read Ribbon
					ReadRibbon();
				} //end if

				//Set currentRibbonSlot
				currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
					GetChild(ribbonChoice).gameObject;
			} //end else if Pokemon Ribbons on PC -> Ribbons
		} //end else if Mouse Moves Right

		/*********************************************
		* Mouse Moves Up
		**********************************************/
		if (Input.GetAxis("Mouse Y") > 0)
		{
			//PC Home
			if(pcState == PCGame.HOME && Input.mousePosition.y > 
				Camera.main.WorldToScreenPoint(currentPCSlot.transform.position).y + 
				currentPCSlot.GetComponent<RectTransform>().rect.height/2)
			{
				//If not at top of PC
				if(boxChoice > 5)
				{
					boxChoice -= 6;
				} //end if
				else
				{
					//Go to title                           
					boxChoice = -2;
				} //end else
			} //end if Home

			//Pokemon Party on PC -> Party Tab
			else if(pcState == PCGame.PARTY && Input.mousePosition.y > 
				Camera.main.WorldToScreenPoint(currentTeamSlot.transform.position).y + 
				currentTeamSlot.GetComponent<RectTransform>().rect.height/2)
			{
				//If on a top slot, stay there (Causes glitches when mouse is above slot)
				if(choiceNumber == 1 || choiceNumber == 2)
				{
					choiceNumber = choiceNumber;
				} //end if
				//If on Close button, go to last team slot
				else if(choiceNumber == 0)
				{
					if(heldPokemon != null && GameManager.instance.GetTrainer().Team.Count < 6)
					{
						choiceNumber = GameManager.instance.GetTrainer().Team.Count + 1;
					} //end if
					else
					{
						choiceNumber = GameManager.instance.GetTrainer().Team.Count;
					} //end else
					currentTeamSlot = partyTab.transform.FindChild("Pokemon"+choiceNumber).gameObject;
				} //end else if
				//Go up vertically
				else
				{
					choiceNumber -= 2;
					currentTeamSlot = partyTab.transform.FindChild("Pokemon"+choiceNumber).gameObject;
				} //end else
			} //end else if Continue Game -> My Team

			//Pokemon Submenu on PC
			if(pcState == PCGame.POKEMONSUBMENU && Input.mousePosition.y > 
				selection.transform.position.y + selection.GetComponent<RectTransform>().
				rect.height/2)
			{
				//If not on the first option, decrease
				if(subMenuChoice > 0)
				{
					subMenuChoice--;
				} //end if

				//Reposition selection
				selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
			} //end if Pokemon Submenu on PC

			//Pokemon Summary on PC -> Summary
			else if(pcState == PCGame.POKEMONSUMMARY && summaryChoice == 5 &&
				Input.mousePosition.y > Camera.main.WorldToScreenPoint(currentMoveSlot.transform.
					position).y + currentMoveSlot.GetComponent<RectTransform>().rect.height/2)
			{
				//If not on first slot, go up vertically
				if(moveChoice > 0)
				{
					moveChoice--;
					currentMoveSlot = summaryScreen.transform.GetChild(5).
						FindChild("Move"+(moveChoice+1)).gameObject;
				} //end if
			} //end else if Pokemon Summary on PC -> Summary

			//Move Switch on PC -> Summary -> Move Details
			else if(pcState == PCGame.MOVESWITCH &&
				Input.mousePosition.y > Camera.main.WorldToScreenPoint(currentSwitchSlot.transform.
					position).y + currentSwitchSlot.GetComponent<RectTransform>().rect.height/2)
			{
				//If not on first slot, go up vertically
				if(switchChoice > 0)
				{
					switchChoice--;                        
					currentSwitchSlot = summaryScreen.transform.GetChild(5).
						FindChild("Move"+(switchChoice+1)).gameObject;
				} //end if
			} //end else if Move Switch on PC -> Summary -> Move Details

			//Pokemon Ribbons on PC -> Ribbons
			else if(pcState == PCGame.POKEMONRIBBONS &&
				Input.mousePosition.y > Camera.main.WorldToScreenPoint(currentRibbonSlot.transform.
					position).y + currentRibbonSlot.GetComponent<RectTransform>().rect.height/2)
			{
				//If next slot is null, don't move
				if(ribbonChoice-4 > -1)
				{
					ribbonChoice -= 4;

					//Read ribbon
					ReadRibbon();
				} //end if

				//Set currentRibbonSlot
				currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
					GetChild(ribbonChoice).gameObject;
			} //end else if Pokemon Ribbons on PC -> Ribbons

			//Pokemon Markings on PC -> Submenu
			else if(pcState == PCGame.POKEMONMARKINGS && Input.mousePosition.y > 
				selection.transform.position.y+1)
			{
				//If not on the first option, decrease
				if(subMenuChoice > 0)
				{
					subMenuChoice--;
				} //end if
			} //end else if Pokemon Markings on PC -> Submenu

			//Pokemon Take/Give on Continue Game -> Team -> Item
			else if (pcState == PCGame.ITEMCHOICE && Input.mousePosition.y > selection.transform.position.y +
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
		} //end else if Mouse Moves Up

		/*********************************************
		* Mouse Moves Down
		**********************************************/
		if (Input.GetAxis("Mouse Y") < 0)
		{
			//PC Home
			if(pcState == PCGame.HOME && Input.mousePosition.y < 
				Camera.main.WorldToScreenPoint(currentPCSlot.transform.position).y - 
				currentPCSlot.GetComponent<RectTransform>().rect.height/2)
			{
				//If not at bottom of PC
				if(boxChoice < 24)
				{
					boxChoice += 6;
				} //end if
				//Otherwise go to nearest button
				else if(boxChoice < 27)
				{
					//Set to party button
					boxChoice = 30;
				} //end else if
				else if(boxChoice < 30)
				{
					//Set to return button
					boxChoice = 31;
				} //end else
			} //end if PC Home

			//Pokemon Party on PC -> Party Tab
			else if(pcState == PCGame.PARTY && Input.mousePosition.y < 
				Camera.main.WorldToScreenPoint(currentTeamSlot.transform.position).y - 
				currentTeamSlot.GetComponent<RectTransform>().rect.height/2)
			{
				//If there is a held pokemon
				if(heldPokemon != null && GameManager.instance.GetTrainer().Team.Count < 6)
				{
					//If on last, or one after last slot, go to Close button
					if(choiceNumber == GameManager.instance.GetTrainer().Team.Count ||
						choiceNumber == GameManager.instance.GetTrainer().Team.Count+1)
					{
						choiceNumber = 0;
						currentTeamSlot = partyTab.transform.FindChild("Close").gameObject;
					} //end if
					//If on Cancel button, stay on Cancel button (Causes glitches when mouse goes below button)
					else if(choiceNumber == 0)
					{
						choiceNumber = 0;
						currentTeamSlot = partyTab.transform.FindChild("Pokemon1").gameObject;
					} //end else if
					//Go down vertically
					else
					{
						choiceNumber += 2;
						currentTeamSlot = partyTab.transform.FindChild("Pokemon"+choiceNumber).gameObject;
					} //end else
				} //end if
				//If on last or second to last slot, go to Close button
				else if((choiceNumber == GameManager.instance.GetTrainer().Team.Count - 1
					&& choiceNumber > 0)
					|| choiceNumber == GameManager.instance.GetTrainer().Team.Count)
				{
					choiceNumber = 0;
					currentTeamSlot = partyTab.transform.FindChild("Close").gameObject;
				} //end else if
				//If on Cancel button, stay on Cancel button (Causes glitches when mouse goes below button)
				else if(choiceNumber == 0)
				{
					choiceNumber = 0;
				} //end else if
				//Go down vertically
				else
				{
					choiceNumber += 2;
					currentTeamSlot = partyTab.transform.FindChild("Pokemon"+choiceNumber).gameObject;
				} //end else
			} //end else if Pokemon Party on PC -> Party Tab

			//Pokemon Submenu on PC
			else if(pcState == PCGame.POKEMONSUBMENU && Input.mousePosition.y < 
				selection.transform.position.y - selection.GetComponent<RectTransform>().
				rect.height/2)
			{
				//If not on the last option, increase (lower slots on higher children)
				if(subMenuChoice < choices.transform.childCount-1)
				{
					subMenuChoice++;
				} //end if

				//Reposition selection
				selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
			} //end else if Pokemon Submenu on PC

			//Pokemon Summary on PC -> Summary
			else if(pcState == PCGame.POKEMONSUMMARY && summaryChoice == 5 &&
				Input.mousePosition.y < Camera.main.WorldToScreenPoint(currentMoveSlot.transform.
					position).y - currentMoveSlot.GetComponent<RectTransform>().rect.height/2)
			{
				//If not on the last slot
				if(moveChoice < 3)
				{
					//If next slot is null, don't move
					if(moveChoice < selectedPokemon.GetMoveCount() - 1)
					{
						moveChoice++;
					} //end if

					//Set currentMoveSlot
					currentMoveSlot = summaryScreen.transform.GetChild(5).
						FindChild("Move"+(moveChoice+1)).gameObject;
				} //end if
			} //end else if Pokemon Summary on PC -> Summary

			//Move Switch on PC -> Summary -> Move Details
			else if(pcState == PCGame.MOVESWITCH &&
				Input.mousePosition.y < Camera.main.WorldToScreenPoint(currentSwitchSlot.transform.
					position).y - currentSwitchSlot.GetComponent<RectTransform>().rect.height/2)
			{
				//If next slot is null, don't move
				if(switchChoice < selectedPokemon.GetMoveCount() - 1)
				{
					switchChoice++;
				} //end if

				//Set currentSwitchSlot
				currentSwitchSlot = summaryScreen.transform.GetChild(5).
					FindChild("Move"+(switchChoice+1)).gameObject;
			} //end else if Move Switch on PC -> Summary -> Move Details

			//Pokemon Ribbons on PC -> Ribbons
			else if(pcState == PCGame.POKEMONRIBBONS &&
				Input.mousePosition.y < Camera.main.WorldToScreenPoint(currentRibbonSlot.transform.
					position).y - currentRibbonSlot.GetComponent<RectTransform>().rect.height/2)
			{
				//If next slot is null, don't move
				if(ribbonChoice+4 < selectedPokemon.GetRibbonCount())
				{
					ribbonChoice += 4;

					//Read ribbon
					ReadRibbon();
				} //end if

				//Set currentRibbonSlot
				currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
					GetChild(ribbonChoice).gameObject;
			} //end else if Pokemon Ribbons on PC -> Ribbons

			//Pokemon Markings on PC -> Submenu
			else if(pcState == PCGame.POKEMONMARKINGS && Input.mousePosition.y < 
				selection.transform.position.y-1)
			{
				//If not on the last option, increase (lower slots on higher children)
				if(subMenuChoice < choices.transform.childCount-1)
				{
					subMenuChoice++;
				} //end if
			} //end else if Pokemon Markings on PC -> Submenu

			//Pokemon Take/Give on Continue Game -> Team -> Item
			else if (pcState == PCGame.ITEMCHOICE && Input.mousePosition.y < selection.transform.position.y -
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
		} //end else if Mouse Moves Down

		/*********************************************
		* Mouse Wheel Up
		**********************************************/
		else if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			//Pokemon Summary on PC -> Summary
			if(pcState == PCGame.POKEMONSUMMARY)
			{
				//If on any page besides move details
				if(summaryChoice != 5)
				{
					//If party tab is open
					if(partyTab.activeSelf && heldPokemon == null)
					{
						//Decrease (higher slots are lower childs)
						choiceNumber--;

						//Clamp between 1 and team size
						if(choiceNumber < 1)
						{
							choiceNumber = GameManager.instance.GetTrainer().Team.Count;
						} //end if
					} //end if
					else if(!partyTab.activeSelf && heldPokemon == null)
					{
						//Decrease to previous pokemon slot
						boxChoice = GameManager.instance.GetTrainer().GetPreviousPokemon(boxChoice);
					} //end else if
				} //end if
			} //end if Pokemon Summary on PC -> Summary

			//Pokemon Ribbons on PC -> Ribbons
			else if(pcState == PCGame.POKEMONRIBBONS)
			{
				//If party tab is open
				if(partyTab.activeSelf && heldPokemon == null)
				{
					//Decrease (higher slots are lower childs)
					choiceNumber--;

					//Clamp between 1 and team size
					if(choiceNumber < 1)
					{
						choiceNumber = GameManager.instance.GetTrainer().Team.Count;
					} //end if

					//Reload ribbons
					initialize = false;
				} //end if
				//If in pokemon region
				else if(!partyTab.activeSelf && heldPokemon == null)
				{
					//Decrease to previous pokemon slot
					boxChoice = GameManager.instance.GetTrainer().GetPreviousPokemon(boxChoice);
				} //end else if
			} //end else if Pokemon Ribbons on PC -> Ribbons

			//Pokemon Item Give From Bag on Continue Game -> Team -> Inventory
			else if (pcState == PCGame.ITEMGIVE)
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
			//Pokemon Summary on PC -> Summary
			if(pcState == PCGame.POKEMONSUMMARY)
			{
				//If on any page besides move details
				if(summaryChoice != 5)
				{
					//Party tab is open
					if(partyTab.activeSelf && heldPokemon == null)
					{
						//Increase (lower slots are on higher childs)
						choiceNumber++;

						//Clamp between 1 and team size
						if(choiceNumber > GameManager.instance.GetTrainer().Team.Count)
						{
							choiceNumber = 1;
						} //end if
					} //end if
					else if(!partyTab.activeSelf && heldPokemon == null)
					{
						//Increase to next pokemon slot
						boxChoice = GameManager.instance.GetTrainer().GetNextPokemon(boxChoice);
					} //end else if
				} //end if
			} //end if Pokemon Summary on PC -> Summary

			//Pokemon Ribbons on PC -> Ribbons
			else if(pcState == PCGame.POKEMONRIBBONS)
			{
				//Party tab is open
				if(partyTab.activeSelf && heldPokemon == null)
				{
					//Increase (lower slots are on higher childs)
					choiceNumber++;

					//Clamp between 1 and team size
					if(choiceNumber > GameManager.instance.GetTrainer().Team.Count)
					{
						choiceNumber = 1;
					} //end if
				} //end if
				else if(!partyTab.activeSelf && heldPokemon == null)
				{
					//Increase to next pokemon slot
					boxChoice = GameManager.instance.GetTrainer().GetNextPokemon(boxChoice);
				} //end else if
			} //end else if Pokemon Ribbons on PC -> Ribbons

			//Pokemon Item Give From Bag on Continue Game -> Team -> Inventory
			else if (pcState == PCGame.ITEMGIVE)
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
			//PC Home
			if(pcState == PCGame.HOME && (selectedPokemon != null || heldPokemon != null || boxChoice == -2))
			{
				//Open submenu as long as player is in pokemon region or on title
				if((boxChoice > -1 && boxChoice < 30) || boxChoice == -2)
				{
					//Fill in choices
					FillInChoices();

					//Set up selection box at end of frame if it doesn't fit
					if(selection.GetComponent<RectTransform>().sizeDelta != 
						choices.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta)
					{
						selection.SetActive(false);
						StartCoroutine(WaitForResize());
					} //end if

					//Reset position to top of menu
					subMenuChoice = 0;
					initialize = false;
					pcState = PCGame.POKEMONSUBMENU;
				} //end if  
			} //end if Home

			//Pokemon Party on PC -> Party Tab
			else if(pcState == PCGame.PARTY && (selectedPokemon != null || heldPokemon != null))
			{
				//Open submenu as long as player is in party
				if(choiceNumber > 0)
				{
					//Fill in choices
					FillInChoices(); 

					//Set up selection box at end of frame if it doesn't fit
					if(selection.GetComponent<RectTransform>().sizeDelta != 
						choices.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta)
					{
						selection.SetActive(false);
						StartCoroutine(WaitForResize());
					} //end if

					//Reset position to top of menu
					subMenuChoice = 0;
					initialize = false;
					pcState = PCGame.POKEMONSUBMENU;
				} //end if  
				//Close submenu if open
				else
				{
					choices.SetActive(false);
					selection.SetActive(false);
				} //end else
			} //end else if Pokemon Party on PC -> Party Tab

			//Pokemon Submenu on PC
			else if(pcState == PCGame.POKEMONSUBMENU)
			{
				if(boxChoice != -2)
				{
					//Apply appropriate action based on submenu selection
					switch(subMenuChoice)
					{
						//Move
						case 0:
							{
								selection.SetActive(false);
								choices.SetActive(false);
								pcState = PCGame.POKEMONHELD;
								break;
							} //end case 0 (Move)

						//Summary
						case 1:
							{
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
								pcState = PCGame.POKEMONSUMMARY;
								break;
							} //end case 1 (Summary)

						//Item
						case 2:
							{
								initialize = false;
								subMenuChoice = 0;
								FillInChoices();
								pcState = PCGame.ITEMCHOICE;
								break;
							} //end case 2 (Item)

						//Ribbons
						case 3:
							{
								initialize = false;
								choices.SetActive(false);
								selection.SetActive(false);
								currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
									GetChild(0).gameObject;
								pcState = PCGame.POKEMONRIBBONS;
								break;
							} //end case 3 (Ribbons)

						//Markings
						case 4:
							{
								markingChoices = heldPokemon == null ? selectedPokemon.GetMarkings().ToList() :
									heldPokemon.GetMarkings().ToList();
								initialize = false;
								pcState = PCGame.POKEMONMARKINGS;
								break;
							} //end case 4 (Markings)

						//Release
						case 5:
							{								
								//If party tab is open
								if(partyTab.activeSelf && GameManager.instance.GetTrainer().Team.Count > 1)
								{
									//Request confirmation
									GameManager.instance.DisplayConfirm("Are you sure you want to release " +
									GameManager.instance.GetTrainer().Team[choiceNumber - 1].Nickname + "?", 1, true);
								} //end if
								else
								{
									//Request confirmation
									GameManager.instance.DisplayConfirm("Are you sure you want to release " +
									GameManager.instance.GetTrainer().GetPC(
										GameManager.instance.GetTrainer().GetPCBox(), boxChoice).Nickname + "?", 1, true);
								} //end else

								choices.SetActive(false);
								selection.SetActive(false);
								break;
							} //end case 5 (Release)

						//Cancel
						case 6:
							{
								choices.SetActive(false);
								selection.SetActive(false);
								//Enable party close button
								partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;
								pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
								break;
							} //end case 6 (Cancel)
					} //end switch
				} //end if
				else
				{
					//Apply appropriate action based on subMenuChoice
					switch(subMenuChoice)
					{
						//Jump To
						case 0:
							{
								input.transform.GetChild(2).GetComponent<Text>().text = "What box do you want to jump to? (0-49)";
								input.transform.GetChild(0).GetComponent<Text>().text = "Desired box:";
								inputText.text = GameManager.instance.GetTrainer().GetPCBox().ToString();
								choices.SetActive(false);
								selection.SetActive(false);
								input.SetActive(true);
								pcState = PCGame.INPUTSCREEN;
								break;
							} //end case 0
							//Change Name
						case 1:
							{
								input.transform.GetChild(2).GetComponent<Text>().text = "Please enter box name.";
								input.transform.GetChild(0).GetComponent<Text>().text = "Box name:";
								inputText.text = GameManager.instance.GetTrainer().GetPCBoxName();
								choices.SetActive(false);
								selection.SetActive(false);
								input.SetActive(true);
								pcState = PCGame.INPUTSCREEN;
								break;
							} //end case 1
							//Change Wallpaper
						case 2:
							{
								input.transform.GetChild(2).GetComponent<Text>().text = "Choose a wallpaper. (0-24)";
								input.transform.GetChild(0).GetComponent<Text>().text = "Wallpaper:";
								inputText.text = GameManager.instance.GetTrainer().GetPCBoxWallpaper().ToString();
								choices.SetActive(false);
								selection.SetActive(false);
								input.SetActive(true);
								pcState = PCGame.INPUTSCREEN;
								break;
							} //end case 2
							//Cancel
						case 3:
							{
								choices.SetActive(false);
								selection.SetActive(false);
								//Enable party close button
								partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;
								pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
								break;
							} //end case 3
					} //end switch
				} //end else
			} //end else if Pokemon Submenu on PC

			//Pokemon Summary on PC -> Summary
			else if(pcState == PCGame.POKEMONSUMMARY)
			{
				//If on moves screen, switch to move details
				if(summaryChoice == 4)
				{
					moveChoice = 0;
					summaryChoice = 5;
					currentMoveSlot = summaryScreen.transform.GetChild(5).
						FindChild("Move1").gameObject;
				} //end if

				//If on move details screen, go to move switch
				else if(summaryChoice == 5)
				{
					currentMoveSlot.GetComponent<Image>().color = Color.white;
					switchChoice = moveChoice;
					currentSwitchSlot = currentMoveSlot;
					pcState = PCGame.MOVESWITCH;
				} //end else if
			} //end else if Pokemon Summary on PC -> Summary

			//Move Switch on PC -> Summary -> Move Details
			else if(pcState == PCGame.MOVESWITCH)
			{
				//If switching spots aren't the same
				if(moveChoice != switchChoice)
				{
					selectedPokemon.SwitchMoves(moveChoice, switchChoice);
				} //end if

				currentMoveSlot.GetComponent<Image>().color = Color.clear;
				pcState = PCGame.POKEMONSUMMARY;
			} //end else if Move Switch on PC -> Summary -> Move Details

			//Pokemon Ribbons on PC -> Ribbons
			else if(pcState == PCGame.POKEMONRIBBONS)
			{
				//Make sure there are ribbons to be read
				if(selectedPokemon.GetRibbonCount() > 0)
				{
					selection.SetActive(!selection.activeSelf);
					ReadRibbon();
				} //end if
			} //end else if Pokemon Ribbons on PC -> Ribbons

			//Pokemon Markings on PC -> Submenu
			else if(pcState == PCGame.POKEMONMARKINGS)
			{
				if(subMenuChoice < DataContents.markingCharacters.Length)
				{
					//Turn the marking on or off
					markingChoices[subMenuChoice] = !markingChoices[subMenuChoice];

					//Color in choices
					for(int i = 0; i < markingChoices.Count; i++)
					{
						choices.transform.GetChild(i).GetComponent<Text>().color =
							markingChoices[i] ? Color.black : Color.gray;
					} //end for
				} //end if
				else if(subMenuChoice == DataContents.markingCharacters.Length)
				{
					//Turn menu off                            
					choices.SetActive(false);
					selection.SetActive(false);

					//If holding a pokemon
					if(heldPokemon != null)
					{
						//Update held pokemon markings
						heldPokemon.SetMarkings(markingChoices.ToArray());

						//Enable party close button
						partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;

						//Return to home or party
						pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
					} //end if
					//If in party
					else if(partyTab.activeSelf)
					{
						//Update team pokemon markings
						GameManager.instance.GetTrainer().Team[choiceNumber-1].SetMarkings(markingChoices.ToArray());

						//Enable party close button
						partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;

						//Return to party
						pcState = PCGame.PARTY;
					} //end else if
					//In pokemon region
					else
					{
						//Update pc box pokemon markings
						GameManager.instance.GetTrainer().GetPC(
							GameManager.instance.GetTrainer().GetPCBox(),
							boxChoice).SetMarkings(markingChoices.ToArray());

						//Return to home
						pcState = PCGame.HOME;
					} //end else
				} //end else if
				else if(subMenuChoice == DataContents.markingCharacters.Length+1)
				{
					//Turn menu off
					FillInChoices();
					choices.SetActive(false);
					selection.SetActive(false);

					//Enable party close button
					partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;

					//Return to home or party
					pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
				} //end else if
			} //end else if Pokemon Markings on PC -> Submenu

			//Pokemon Input on PC -> Input
			else if(pcState == PCGame.INPUTSCREEN)
			{
				//Handle according to subMenuChoice
				switch(subMenuChoice)
				{
					//Jump to
					case 0:
						{
							if (inputText.text.Length > 0)
							{
								int outputHolder = 0;
								if (int.TryParse(inputText.text, out outputHolder))
								{
									outputHolder = ExtensionMethods.WithinIntRange(outputHolder, 0, 49);
									GameManager.instance.GetTrainer().ChangeBox(outputHolder);
								} //end if
							} //end if
							break;
						} //end case 0
						//Change name
					case 1:
						{
							if (inputText.text.Length > 0)
							{
								GameManager.instance.GetTrainer().SetPCBoxName(inputText.text);
							} //end if
							break;
						} //end case 1
						//Change wallpaper
					case 2:
						{
							if (inputText.text.Length > 0)
							{
								int outputHolder = 0;
								if (int.TryParse(inputText.text, out outputHolder))
								{
									outputHolder = ExtensionMethods.WithinIntRange(outputHolder, 0, 24);
									GameManager.instance.GetTrainer().SetPCBoxWallpaper(outputHolder);
								} //end if
							} //end if
							break;
						} //end case 2
				} //end switch

				//Turn input off
				input.SetActive(false);

				//Return to home
				checkpoint = 1;
				pcState = PCGame.HOME;
			} //end else if Pokemon Input on PC -> Input

			//Pokemon Take/Give on Continue Game -> Team -> Item
			else if (pcState == PCGame.ITEMCHOICE)
			{
				//Apply appropriate action based on selection
				switch (subMenuChoice)
				{
					//Give
					case 0:
						playerBag.SetActive(true);
						choices.SetActive(false);
						selection.SetActive(false);

						//Set selected pokemon
						if (partyTab.activeSelf)
						{
							selectedPokemon = GameManager.instance.GetTrainer().Team[choiceNumber - 1];
						} //end if
						else
						{
							selectedPokemon = GameManager.instance.GetTrainer().GetPC(
								GameManager.instance.GetTrainer().GetPCBox(), boxChoice);
						} //end else

						//Move to giving item
						pcState = PCGame.ITEMGIVE;
						initialize = false;
						break;
					//Take
					case 1:
						//If in party
						if (partyTab.activeSelf)
						{
							int itemNumber = GameManager.instance.GetTrainer().Team[choiceNumber - 1].Item;
							if (itemNumber != 0)
							{
								GameManager.instance.GetTrainer().AddItem(itemNumber, 1);
								GameManager.instance.GetTrainer().Team[choiceNumber - 1].Item = 0;
								GameManager.instance.DisplayText("Took " + DataContents.GetItemGameName(itemNumber) + " and " +
									"added it to bag.", true);						
								choices.SetActive(false);
								selection.SetActive(false);
								initialize = false;
							} //end if
							else
							{
								GameManager.instance.DisplayText("This Pokemon isn't holding an item.", true);
							} //end else

							//Enable party close button
							partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;

							//Return to party
							pcState = PCGame.PARTY;
						} //end if

						//If in PC box
						else
						{
							int itemNumber = GameManager.instance.GetTrainer().GetPC(
								GameManager.instance.GetTrainer().GetPCBox(), boxChoice).Item;
							if (itemNumber != 0)
							{
								GameManager.instance.GetTrainer().AddItem(itemNumber, 1);
								GameManager.instance.GetTrainer().GetPC(
									GameManager.instance.GetTrainer().GetPCBox(), boxChoice).Item = 0;
								GameManager.instance.DisplayText("Took " + DataContents.GetItemGameName(itemNumber) + " and " +
								"added it to bag.", true);						
								choices.SetActive(false);
								selection.SetActive(false);
								initialize = false;
							} //end if
							else
							{
								GameManager.instance.DisplayText("This Pokemon isn't holding an item.", true);
							} //end else

							//Return to home
							pcState = PCGame.HOME;
						} //end else
						break;
						//Cancel
					case 2:
						choices.SetActive(false);
						selection.SetActive(false);

						//Enable party close button
						partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;

						//Return to home or party
						pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
						break;
				} //end switch
			} //end else if Pokemon Take/Give on Continue Game -> Team -> Item

			//Pokemon Item Give From Bag on Continue Game -> Team -> Inventory
			else if (pcState == PCGame.ITEMGIVE)
			{	
				//Verify an item is highlighted
				if(inventorySpot > GameManager.instance.GetTrainer().SlotCount())
				{
					//If in party
					if (partyTab.activeSelf)
					{
						//Make sure pokemon isn't holding another item
						if (GameManager.instance.GetTrainer().Team[choiceNumber - 1].Item == 0)
						{
							int itemNumber = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];
							GameManager.instance.GetTrainer().RemoveItem(itemNumber, 1);
							GameManager.instance.GetTrainer().Team[choiceNumber - 1].Item = itemNumber;
							GameManager.instance.DisplayText("Gave " + DataContents.GetItemGameName(itemNumber) + " to " +
							GameManager.instance.GetTrainer().Team[choiceNumber - 1].Nickname, true);
							playerBag.SetActive(false);
							initialize = false;

							//Enable party close button
							partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;

							//Return to party
							pcState = PCGame.PARTY;
						} //end if
						else
						{
							int itemNumber = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];
							GameManager.instance.DisplayConfirm("Do you want to switch the held " + DataContents.GetItemGameName(
								GameManager.instance.GetTrainer().Team[choiceNumber - 1].Item) + " for " + DataContents.GetItemGameName(
								itemNumber) + "?", 0, false);
						} //end else
					} //end if

					//If in pc box
					else
					{
						//Make sure pokemon isn't holding another item
						if (GameManager.instance.GetTrainer().GetPC(
							GameManager.instance.GetTrainer().GetPCBox(), boxChoice).Item == 0)
						{
							int itemNumber = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];
							GameManager.instance.GetTrainer().RemoveItem(itemNumber, 1);
							GameManager.instance.GetTrainer().GetPC(
								GameManager.instance.GetTrainer().GetPCBox(), boxChoice).Item = itemNumber;
							GameManager.instance.DisplayText("Gave " + DataContents.GetItemGameName(itemNumber) + " to " +
								GameManager.instance.GetTrainer().GetPC(
									GameManager.instance.GetTrainer().GetPCBox(), boxChoice).Nickname, true);
							playerBag.SetActive(false);
							initialize = false;
							//Return to home
							pcState = PCGame.HOME;
						} //end if
						else
						{
							int itemNumber = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];
							GameManager.instance.DisplayConfirm("Do you want to switch the held " + DataContents.GetItemGameName(
								GameManager.instance.GetTrainer().GetPC(
									GameManager.instance.GetTrainer().GetPCBox(), boxChoice).Item) + " for " + DataContents.GetItemGameName(
									itemNumber) + "?", 0, false);
						} //end else
					} //end else
				} //end if
			} //end else if Pokemon Item Give From Bag on Continue Game -> Team -> Inventory
		} //end else if Left Mouse Button

		/*********************************************
		* Right Mouse Button
		**********************************************/
		else if (Input.GetMouseButtonUp(1))
		{
			//PC Home
			if(pcState == PCGame.HOME)
			{
				GameManager.instance.LoadScene("MainGame", true);
			} //end if PC Home

			//Pokemon Submenu on PC
			else  if(pcState == PCGame.POKEMONSUBMENU)
			{
				choices.SetActive(false);
				selection.SetActive(false);

				//Enable party close button
				partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;
				pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
			} //end else if Pokemon Submenu on PC

			//Pokemon Party on PC -> Party Tab
			else if(pcState == PCGame.PARTY)
			{
				choices.SetActive(false);
				selection.SetActive(false);
				StartCoroutine(PartyState(false));
			} //end else if Pokemon Party on PC -> Party Tab

			//Pokemon Summary on PC -> Summary
			else if(pcState == PCGame.POKEMONSUMMARY)
			{
				//If on any page besides details
				if(summaryChoice != 5)
				{
					//Deactivate summary
					summaryScreen.SetActive(false);

					//Enable party close button
					partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;

					//Return home or party
					pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
				} //end if
				else
				{
					summaryScreen.transform.GetChild(5).gameObject.SetActive(false);
					selection.SetActive(false);
					summaryChoice = 4;
				} //end else
			} //end else if Pokemon Summary on PC -> Summary

			//Move Switch on PC -> Summary -> Move Details
			else if(pcState == PCGame.MOVESWITCH)
			{
				//Return to summary
				currentMoveSlot.GetComponent<Image>().color = Color.clear;
				pcState = PCGame.POKEMONSUMMARY;
			} //end else if Move Switch on PC -> Summary -> Move Details

			//Pokemon Ribbons on PC -> Ribbons
			else if(pcState == PCGame.POKEMONRIBBONS)
			{
				//Deactivate ribbons
				ribbonScreen.SetActive(false);
				selection.SetActive(false);
				ribbonChoice = 0;
				previousRibbonChoice = -1;

				//Enable party close button
				partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;

				//Return to home or party
				pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
			} //end else if Pokemon Ribbons on PC -> Ribbons

			//Pokemon Markings on PC -> Submenu
			else if(pcState == PCGame.POKEMONMARKINGS)
			{
				//Turn menu off
				FillInChoices();
				choices.SetActive(false);
				selection.SetActive(false);

				//Enable party close button
				partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;

				//Return to home or party
				pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
			} //end else if Pokemon Markings on PC -> Submenu

			//Pokemon Take/Give on PC -> Submenu -> Item
			else if (pcState == PCGame.ITEMCHOICE)
			{
				choices.SetActive(false);
				selection.SetActive(false);

				//Enable party close button
				partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;
				pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
			} //end else if Pokemon Take/Give on PC -> Submenu -> Item

			//Pokemon Item Give From Bag on PC -> Inventory
			else if (pcState == PCGame.ITEMGIVE)
			{
				playerBag.SetActive(false);

				//Enable party close button
				partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;
				pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
			} //end else if Pokemon Item Give From Bag on PC -> Inventory
		} //end else if Right Mouse Button

		/*********************************************
		* Enter/Return Key
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.Return))
		{
			//PC Home
			if(pcState == PCGame.HOME)
			{
				//Open submenu as long as player is in pokemon region or on title
				if((boxChoice > -1 && boxChoice < 30 && (selectedPokemon != null || heldPokemon != null)) || boxChoice == -2)
				{
					//Fill in choices
					FillInChoices();

					//Set up selection box at end of frame if it doesn't fit
					if(selection.GetComponent<RectTransform>().sizeDelta != 
						choices.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta)
					{
						selection.SetActive(false);
						StartCoroutine(WaitForResize());
					} //end if

					//Reset position to top of menu
					subMenuChoice = 0;
					initialize = false;
					pcState = PCGame.POKEMONSUBMENU;
				} //end if
				//If on Party button
				else if(boxChoice == 30)
				{
					StartCoroutine(PartyState(true));
				} //end else if
				//If on Return button
				else if(boxChoice == 31)
				{
					GameManager.instance.LoadScene("MainGame", true);
				} //end else if
			} //end if Home

			//Pokemon Party on PC -> Party Tab
			else if(pcState == PCGame.PARTY)
			{
				//Open submenu as long as player is in party
				if(choiceNumber > 0 && (selectedPokemon != null || heldPokemon != null))
				{
					//Fill in choices
					FillInChoices();

					//Set up selection box at end of frame if it doesn't fit
					if(selection.GetComponent<RectTransform>().sizeDelta != 
						choices.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta)
					{
						selection.SetActive(false);
						StartCoroutine(WaitForResize());
					} //end if

					//Reset position to top of menu
					subMenuChoice = 0;
					initialize = false;
					pcState = PCGame.POKEMONSUBMENU;
				} //end if  
				else
				{
					StartCoroutine(PartyState(false));
				} //end else 
			} //end else if Pokemon Party on PC -> Party Tab

			//Pokemon Submenu on PC
			else if(pcState == PCGame.POKEMONSUBMENU)
			{
				if(boxChoice != -2)
				{
					//Apply appropriate action based on submenu selection
					switch(subMenuChoice)
					{
						//Move
						case 0:
							{
								selection.SetActive(false);
								choices.SetActive(false);
								pcState = PCGame.POKEMONHELD;
								break;
							} //end case 0 (Move)

						//Summary
						case 1:
							{
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
								pcState = PCGame.POKEMONSUMMARY;
								break;
							} //end case 1 (Summary)

						//Item
						case 2:
							{
								initialize = false;
								subMenuChoice = 0;
								FillInChoices();
								pcState = PCGame.ITEMCHOICE;
								break;
							} //end case 2 (Item)

						//Ribbons
						case 3:
							{
								initialize = false;
								choices.SetActive(false);
								selection.SetActive(false);
								currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
									GetChild(0).gameObject;
								pcState = PCGame.POKEMONRIBBONS;
								break;
							} //end case 3 (Ribbons)

						//Markings
						case 4:
							{
								markingChoices = heldPokemon == null ? selectedPokemon.GetMarkings().ToList() :
									heldPokemon.GetMarkings().ToList();
								initialize = false;
								pcState = PCGame.POKEMONMARKINGS;
								break;
							} //end case 4 (Markings)

						//Release
						case 5:
							{
								//If party tab is open
								if(partyTab.activeSelf && GameManager.instance.GetTrainer().Team.Count > 1)
								{
									//Request confirmation
									GameManager.instance.DisplayConfirm("Are you sure you want to release " +
										GameManager.instance.GetTrainer().Team[choiceNumber - 1].Nickname + "?", 1, true);
								} //end if
								else
								{
									//Request confirmation
									GameManager.instance.DisplayConfirm("Are you sure you want to release " +
										GameManager.instance.GetTrainer().GetPC(
											GameManager.instance.GetTrainer().GetPCBox(), boxChoice).Nickname + "?", 1, true);
								} //end else

								choices.SetActive(false);
								selection.SetActive(false);
								break;
							} //end case 5 (Release)

						//Cancel
						case 6:
							{
								choices.SetActive(false);
								selection.SetActive(false);

								//Enable party close button
								partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;
								pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
								break;
							} //end case 6 (Cancel)
					} //end switch
				} //end if
				else
				{
					//Apply appropriate action based on subMenuChoice
					switch(subMenuChoice)
					{
						//Jump To
						case 0:
							{
								input.transform.GetChild(2).GetComponent<Text>().text = "What box do you want to jump to? (0-49)";
								input.transform.GetChild(0).GetComponent<Text>().text = "Desired box:";
								inputText.text = GameManager.instance.GetTrainer().GetPCBox().ToString();
								choices.SetActive(false);
								selection.SetActive(false);
								input.SetActive(true);
								pcState = PCGame.INPUTSCREEN;
								break;
							} //end case 0
							//Change Name
						case 1:
							{
								input.transform.GetChild(2).GetComponent<Text>().text = "Please enter box name.";
								input.transform.GetChild(0).GetComponent<Text>().text = "Box name:";
								inputText.text = GameManager.instance.GetTrainer().GetPCBoxName();
								choices.SetActive(false);
								selection.SetActive(false);
								input.SetActive(true);
								pcState = PCGame.INPUTSCREEN;
								break;
							} //end case 1
							//Change Wallpaper
						case 2:
							{
								input.transform.GetChild(2).GetComponent<Text>().text = "Choose a wallpaper. (0-24)";
								input.transform.GetChild(0).GetComponent<Text>().text = "Wallpaper:";
								inputText.text = GameManager.instance.GetTrainer().GetPCBoxWallpaper().ToString();
								choices.SetActive(false);
								selection.SetActive(false);
								input.SetActive(true);
								pcState = PCGame.INPUTSCREEN;
								break;
							} //end case 2
							//Cancel
						case 3:
							{
								choices.SetActive(false);
								selection.SetActive(false);

								//Enable party close button
								partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;
								pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
								break;
							} //end case 3
					} //end switch
				} //end else
			} //end else if Pokemon Submenu on PC

			//Pokemon Summary on PC -> Summary
			else if(pcState == PCGame.POKEMONSUMMARY)
			{
				//If on moves screen, switch to move details
				if(summaryChoice == 4)
				{
					moveChoice = 0;
					summaryChoice = 5;
					currentMoveSlot = summaryScreen.transform.GetChild(5).
						FindChild("Move1").gameObject;
				} //end if

				//If on move details screen, go to move switch
				else if(summaryChoice == 5)
				{
					currentMoveSlot.GetComponent<Image>().color = Color.white;
					switchChoice = moveChoice;
					currentSwitchSlot = currentMoveSlot;
					pcState = PCGame.MOVESWITCH;
				} //end else if
			} //end else if Pokemon Summary on PC -> Summary

			//Move Switch on PC -> Summary -> Move Details
			else if(pcState == PCGame.MOVESWITCH)
			{
				//If switching spots aren't the same
				if(moveChoice != switchChoice)
				{
					selectedPokemon.SwitchMoves(moveChoice, switchChoice);
				} //end if

				currentMoveSlot.GetComponent<Image>().color = Color.clear;
				pcState = PCGame.POKEMONSUMMARY;
			} //end else if Move Switch on PC -> Summary -> Move Details

			//Pokemon Ribbons on PC -> Ribbons
			else if(pcState == PCGame.POKEMONRIBBONS)
			{
				//Make sure there are ribbons to be read
				if(selectedPokemon.GetRibbonCount() > 0)
				{
					selection.SetActive(!selection.activeSelf);
					ReadRibbon();
				} //end if
			} //end else if Pokemon Ribbons on PC -> Ribbons

			//Pokemon Markings on PC -> Submenu
			else if(pcState == PCGame.POKEMONMARKINGS)
			{
				if(subMenuChoice < DataContents.markingCharacters.Length)
				{
					//Turn the marking on or off
					markingChoices[subMenuChoice] = !markingChoices[subMenuChoice];

					//Color in choices
					for(int i = 0; i < markingChoices.Count; i++)
					{
						choices.transform.GetChild(i).GetComponent<Text>().color =
							markingChoices[i] ? Color.black : Color.gray;
					} //end for
				} //end if
				else if(subMenuChoice == DataContents.markingCharacters.Length)
				{
					//Turn menu off
					FillInChoices();
					choices.SetActive(false);
					selection.SetActive(false);

					//If holding a pokemon
					if(heldPokemon != null)
					{
						//Update held pokemon markings
						heldPokemon.SetMarkings(markingChoices.ToArray());

						//Enable party close button
						partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;

						//Return to home or party
						pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
					} //end if
					//If in party
					else if(partyTab.activeSelf)
					{
						//Update team pokemon markings
						GameManager.instance.GetTrainer().Team[choiceNumber-1].SetMarkings(markingChoices.ToArray());

						//Enable party close button
						partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;

						//Return to party
						pcState = PCGame.PARTY;
					} //end else if
					//In pokemon region
					else
					{
						//Update pc box pokemon markings
						GameManager.instance.GetTrainer().GetPC(
							GameManager.instance.GetTrainer().GetPCBox(),
							boxChoice).SetMarkings(markingChoices.ToArray());

						//Return to home
						pcState = PCGame.HOME;
					} //end else
				} //end else if
				else if(subMenuChoice == DataContents.markingCharacters.Length+1)
				{
					//Turn menu off
					FillInChoices();
					choices.SetActive(false);
					selection.SetActive(false);

					//Enable party close button
					partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;

					//Return to home or party
					pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
				} //end else if
			} //end else if Pokemon Markings on PC -> Submenu

			//Pokemon Input on PC -> Input
			else if(pcState == PCGame.INPUTSCREEN)
			{
				//Handle according to subMenuChoice
				switch(subMenuChoice)
				{
					//Jump to
					case 0:
						{
							if (inputText.text.Length > 0)
							{
								int outputHolder = 0;
								if (int.TryParse(inputText.text, out outputHolder))
								{
									outputHolder = ExtensionMethods.WithinIntRange(outputHolder, 0, 49);
									GameManager.instance.GetTrainer().ChangeBox(outputHolder);
								} //end if
							} //end if
							break;
						} //end case 0
						//Change name
					case 1:
						{
							if (inputText.text.Length > 0)
							{
								GameManager.instance.GetTrainer().SetPCBoxName(inputText.text);
							} //end if
							break;
						} //end case 1
						//Change wallpaper
					case 2:
						{
							if (inputText.text.Length > 0)
							{
								int outputHolder = 0;
								if (int.TryParse(inputText.text, out outputHolder))
								{
									outputHolder = ExtensionMethods.WithinIntRange(outputHolder, 0, 24);
									GameManager.instance.GetTrainer().SetPCBoxWallpaper(outputHolder);
								} //end if
							} //end if
							break;
						} //end case 2
				} //end switch

				//Turn input off
				input.SetActive(false);

				//Return to home
				checkpoint = 1;
				pcState = PCGame.HOME;
			} //end else if Pokemon Input on PC -> Input

			//Pokemon Take/Give on Continue Game -> Team -> Item
			else if (pcState == PCGame.ITEMCHOICE)
			{
				//Apply appropriate action based on selection
				switch (subMenuChoice)
				{
					//Give
					case 0:
						playerBag.SetActive(true);
						choices.SetActive(false);
						selection.SetActive(false);

						//Set selected pokemon
						if (partyTab.activeSelf)
						{
							selectedPokemon = GameManager.instance.GetTrainer().Team[choiceNumber - 1];
						} //end if
						else
						{
							selectedPokemon = GameManager.instance.GetTrainer().GetPC(
								GameManager.instance.GetTrainer().GetPCBox(), boxChoice);
						} //end else

						//Move to giving item
						pcState = PCGame.ITEMGIVE;
						initialize = false;
						break;
					//Take
					case 1:
						//If in party
						if (partyTab.activeSelf)
						{
							int itemNumber = GameManager.instance.GetTrainer().Team[choiceNumber - 1].Item;
							if (itemNumber != 0)
							{
								GameManager.instance.GetTrainer().AddItem(itemNumber, 1);
								GameManager.instance.GetTrainer().Team[choiceNumber - 1].Item = 0;
								GameManager.instance.DisplayText("Took " + DataContents.GetItemGameName(itemNumber) + " and " +
								"added it to bag.", true);						
								choices.SetActive(false);
								selection.SetActive(false);
								initialize = false;
							} //end if
							else
							{
								GameManager.instance.DisplayText("This Pokemon isn't holding an item.", true);
							} //end else

							//Enable party close button
							partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;

							//Return to party
							pcState = PCGame.PARTY;
						} //end if

						//If in PC box
						else
						{
							int itemNumber = GameManager.instance.GetTrainer().GetPC(
								GameManager.instance.GetTrainer().GetPCBox(), boxChoice).Item;
							if (itemNumber != 0)
							{
								GameManager.instance.GetTrainer().AddItem(itemNumber, 1);
								GameManager.instance.GetTrainer().GetPC(
									GameManager.instance.GetTrainer().GetPCBox(), boxChoice).Item = 0;
								GameManager.instance.DisplayText("Took " + DataContents.GetItemGameName(itemNumber) + " and " +
									"added it to bag.", true);						
								choices.SetActive(false);
								selection.SetActive(false);
								initialize = false;
							} //end if
							else
							{
								GameManager.instance.DisplayText("This Pokemon isn't holding an item.", true);
							} //end else

							//Return to home
							pcState = PCGame.HOME;
						} //end else
						break;
						//Cancel
					case 2:
						choices.SetActive(false);
						selection.SetActive(false);

						//Enable party close button
						partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;

						//Return to home or party
						pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
						break;
				} //end switch
			} //end else if Pokemon Take/Give on Continue Game -> Team -> Item

			//Pokemon Item Give From Bag on Continue Game -> Team -> Inventory
			else if (pcState == PCGame.ITEMGIVE)
			{				
				//Verify an item is highlighted
				if(inventorySpot > GameManager.instance.GetTrainer().SlotCount())
				{
					//If in party
					if (partyTab.activeSelf)
					{
						//Make sure pokemon isn't holding another item
						if (GameManager.instance.GetTrainer().Team[choiceNumber - 1].Item == 0)
						{
							int itemNumber = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];
							GameManager.instance.GetTrainer().RemoveItem(itemNumber, 1);
							GameManager.instance.GetTrainer().Team[choiceNumber - 1].Item = itemNumber;
							GameManager.instance.DisplayText("Gave " + DataContents.GetItemGameName(itemNumber) + " to " +
								GameManager.instance.GetTrainer().Team[choiceNumber - 1].Nickname, true);
							playerBag.SetActive(false);
							initialize = false;

							//Enable party close button
							partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;

							//Return to party
							pcState = PCGame.PARTY;
						} //end if
						else
						{
							int itemNumber = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];
							GameManager.instance.DisplayConfirm("Do you want to switch the held " + DataContents.GetItemGameName(
								GameManager.instance.GetTrainer().Team[choiceNumber - 1].Item) + " for " + DataContents.GetItemGameName(
									itemNumber) + "?", 0, false);
						} //end else
					} //end if

					//If in pc box
					else
					{
						//Make sure pokemon isn't holding another item
						if (GameManager.instance.GetTrainer().GetPC(
							GameManager.instance.GetTrainer().GetPCBox(), boxChoice).Item == 0)
						{
							int itemNumber = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];
							GameManager.instance.GetTrainer().RemoveItem(itemNumber, 1);
							GameManager.instance.GetTrainer().GetPC(
								GameManager.instance.GetTrainer().GetPCBox(), boxChoice).Item = itemNumber;
							GameManager.instance.DisplayText("Gave " + DataContents.GetItemGameName(itemNumber) + " to " +
								GameManager.instance.GetTrainer().GetPC(
									GameManager.instance.GetTrainer().GetPCBox(), boxChoice).Nickname, true);
							playerBag.SetActive(false);
							initialize = false;
							//Return to home
							pcState = PCGame.HOME;
						} //end if
						else
						{
							int itemNumber = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];
							GameManager.instance.DisplayConfirm("Do you want to switch the held " + DataContents.GetItemGameName(
								GameManager.instance.GetTrainer().GetPC(
									GameManager.instance.GetTrainer().GetPCBox(), boxChoice).Item) + " for " + DataContents.GetItemGameName(
										itemNumber) + "?", 0, false);
						} //end else
					} //end else
				} //end if
			} //end else if Pokemon Item Give From Bag on Continue Game -> Team -> Inventory
		} //end else if Enter/Return Key

		/*********************************************
		* X Key
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.X))
		{
			//PC Home
			if(pcState == PCGame.HOME)
			{
				GameManager.instance.LoadScene("MainGame", true);
			} //end if PC Home

			//Pokemon Submenu on PC
			else if(pcState == PCGame.POKEMONSUBMENU)
			{
				choices.SetActive(false);
				selection.SetActive(false);

				//Enable party close button
				partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;
				pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
			} //end else if Pokemon Submenu on PC

			//Pokeon Party on PC -> Party Tab
			else if(pcState == PCGame.PARTY)
			{
				StartCoroutine(PartyState(false));
			} //end else if Pokemon Party on PC -> Party Tab

			//Pokemon Summary on PC -> Summary
			else if(pcState == PCGame.POKEMONSUMMARY)
			{
				//If on any page besides details
				if(summaryChoice != 5)
				{
					//Deactivate summary
					summaryScreen.SetActive(false);

					//Enable party close button
					partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;

					//Return to home or party
					pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
				} //end if
				else
				{
					summaryScreen.transform.GetChild(5).gameObject.SetActive(false);
					selection.SetActive(false);
					summaryChoice = 4;
				} //end else
			} //end else if Pokemon Summary on PC -> Summary

			//Move Switch on PC -> Summary -> Move Details
			else if(pcState == PCGame.MOVESWITCH)
			{
				//Return to summary
				currentMoveSlot.GetComponent<Image>().color = Color.clear;
				pcState = PCGame.POKEMONSUMMARY;
			} //end else if Move Switch on PC -> Summary -> Move Details

			//Pokemon Ribbons on PC -> Ribbons
			else if(pcState == PCGame.POKEMONRIBBONS)
			{
				//Deactivate ribbons
				ribbonScreen.SetActive(false);
				selection.SetActive(false);
				ribbonChoice = 0;
				previousRibbonChoice = -1;

				//Enable party close button
				partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;

				//Return to home or party
				pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
			} //end else if Pokemon Ribbons on PC -> Ribbons

			//Pokemon Markings on PC -> Submenu
			else if(pcState == PCGame.POKEMONMARKINGS)
			{
				//Turn menu off
				FillInChoices();
				choices.SetActive(false);
				selection.SetActive(false);

				//Enable party close button
				partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;

				//Return to home or party
				pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
			} //end else if Pokemon Markings on PC -> Submenu

			//Pokemon Take/Give on PC -> Submenu -> Item
			else if (pcState == PCGame.ITEMCHOICE)
			{
				choices.SetActive(false);
				selection.SetActive(false);

				//Enable party close button
				partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;
				pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
			} //end else if Pokemon Take/Give on PC -> Submenu -> Item

			//Pokemon Item Give From Bag on PC -> Inventory
			else if (pcState == PCGame.ITEMGIVE)
			{
				playerBag.SetActive(false);

				//Enable party close button
				partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;
				pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
			} //end else if Pokemon Item Give From Bag on PC -> Inventory
		} //end else if X Key
	} //end GetInput

    /***************************************
     * Name: FillInChoices
     * Sets the choices for the choice menu
     * depending on the scene
     ***************************************/
    void FillInChoices()
    {
        //If in PC in Pokemon Region
		if ((pcState == PCGame.HOME && boxChoice > -1) || pcState == PCGame.PARTY)
		{
			//Fill in choices box
			for (int i = choices.transform.childCount - 1; i < 6; i++)
			{
				GameObject clone = Instantiate(choices.transform.GetChild(0).gameObject,
					                               choices.transform.GetChild(0).position,
					                               Quaternion.identity) as GameObject;
				clone.transform.SetParent(choices.transform);
			} //end for
			choices.transform.GetChild(0).GetComponent<Text>().text = "Move";
			choices.transform.GetChild(1).GetComponent<Text>().text = "Summary";
			choices.transform.GetChild(2).GetComponent<Text>().text = "Item";
			choices.transform.GetChild(3).GetComponent<Text>().text = "Ribbons";
			choices.transform.GetChild(4).GetComponent<Text>().text = "Markings";
			choices.transform.GetChild(5).GetComponent<Text>().text = "Release";
			choices.transform.GetChild(6).GetComponent<Text>().text = "Cancel";
			choices.transform.GetChild(0).GetComponent<Text>().color = Color.black;
			choices.transform.GetChild(1).GetComponent<Text>().color = Color.black;
			choices.transform.GetChild(2).GetComponent<Text>().color = Color.black;
			choices.transform.GetChild(3).GetComponent<Text>().color = Color.black;
			choices.transform.GetChild(4).GetComponent<Text>().color = Color.black;
			choices.transform.GetChild(5).GetComponent<Text>().color = Color.black;
			choices.transform.GetChild(6).GetComponent<Text>().color = Color.black;
			if (choices.transform.childCount > 7)
			{
				for (int i = 7; i < choices.transform.childCount; i++)
				{
					Destroy(choices.transform.GetChild(i).gameObject);
				} //end for
			} //end if
		} //end else if
        //If in PC on Box Title
		else if (pcState == PCGame.HOME && boxChoice == -2)
		{
			//Fill in choices box
			for (int i = choices.transform.childCount - 1; i < 3; i++)
			{
				GameObject clone = Instantiate(choices.transform.GetChild(0).gameObject,
					                               choices.transform.GetChild(0).position,
					                               Quaternion.identity) as GameObject;
				clone.transform.SetParent(choices.transform);
			} //end for
			choices.transform.GetChild(0).GetComponent<Text>().text = "Jump To";
			choices.transform.GetChild(1).GetComponent<Text>().text = "Rename";
			choices.transform.GetChild(2).GetComponent<Text>().text = "Wallpaper";
			choices.transform.GetChild(3).GetComponent<Text>().text = "Cancel";
			choices.transform.GetChild(0).GetComponent<Text>().color = Color.black;
			choices.transform.GetChild(1).GetComponent<Text>().color = Color.black;
			choices.transform.GetChild(2).GetComponent<Text>().color = Color.black;
			choices.transform.GetChild(3).GetComponent<Text>().color = Color.black;
			if (choices.transform.childCount > 4)
			{
				for (int i = 4; i < choices.transform.childCount; i++)
				{
					Destroy(choices.transform.GetChild(i).gameObject);
				} //end for
			} //end if
		}  //end else if
        //If in PC Markings
        else if (pcState == PCGame.POKEMONMARKINGS)
		{
			for (int i = choices.transform.childCount - 1; i < DataContents.markingCharacters.Length + 2; i++)
			{
				GameObject clone = Instantiate(choices.transform.GetChild(0).gameObject,
					                               choices.transform.GetChild(0).position, Quaternion.identity) as GameObject;
				clone.transform.SetParent(choices.transform);
			} //end for
			for (int i = 0; i < DataContents.markingCharacters.Length + 2; i++)
			{
				if (i == DataContents.markingCharacters.Length)
				{
					choices.transform.GetChild(i).GetComponent<Text>().text = "OK";
				} //end if
                else if (i == DataContents.markingCharacters.Length + 1)
				{
					choices.transform.GetChild(i).GetComponent<Text>().text = "Cancel";
				} //end else if
                else
				{                           
					choices.transform.GetChild(i).GetComponent<Text>().text =
                        DataContents.markingCharacters[i].ToString(); 
				} //end else
			} //end for

			//Destroy extra
			if (choices.transform.childCount > DataContents.markingCharacters.Length + 1)
			{
				for (int i = DataContents.markingCharacters.Length + 2; i < choices.transform.childCount; i++)
				{
					Destroy(choices.transform.GetChild(i).gameObject);
				} //end for
			} //end if

			//Color in choices
			for (int i = 0; i < markingChoices.Count; i++)
			{
				choices.transform.GetChild(i).GetComponent<Text>().color =
                    markingChoices[i] ? Color.black : Color.gray;
			} //end for 
		} //end else if

		//If selecting item
		else if (pcState == PCGame.POKEMONSUBMENU)
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

		//Set submenu active
		choices.SetActive(true);
		selection.SetActive(true);
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
     * Name: FillDetails
     * Fills or clears the details region
     * based on selected pokemon
     ***************************************/
    void FillDetails()
    {
        //If it's not null, populate
        if (selectedPokemon != null)
        {
            detailsRegion.transform.FindChild ("Name").GetComponent<Text> ().text = selectedPokemon.Nickname;
            detailsRegion.transform.FindChild ("Gender").GetComponent<Image> ().color = Color.white;
            detailsRegion.transform.FindChild ("Gender").GetComponent<Image> ().sprite = 
                Resources.Load<Sprite> ("Sprites/Icons/gender" + selectedPokemon.Gender);
            detailsRegion.transform.FindChild ("Sprite").GetComponent<Image> ().color = Color.white;
			detailsRegion.transform.FindChild("Sprite").GetComponent<Image>().sprite = 
				GetCorrectSprite(selectedPokemon);
            detailsRegion.transform.FindChild ("Markings").GetComponent<Text> ().text = selectedPokemon.
                GetMarkingsString();
			detailsRegion.transform.FindChild  ("Shiny").gameObject.SetActive(selectedPokemon.IsShiny);
            detailsRegion.transform.FindChild ("Ability").GetComponent<Text> ().text = selectedPokemon.
                GetAbilityName ();
            detailsRegion.transform.FindChild ("Item").GetComponent<Text> ().text = 
                DataContents.GetItemGameName (selectedPokemon.Item);
            detailsRegion.transform.FindChild ("Level").GetComponent<Text> ().text = "Lv. " + 
                selectedPokemon.CurrentLevel.ToString ();
            SetTypeSprites (detailsRegion.transform.FindChild ("Types").GetChild (0).GetComponent<Image> (),
                detailsRegion.transform.FindChild ("Types").GetChild (1).GetComponent<Image> (),
                selectedPokemon.NatSpecies);
        } //end if
        else
        {
            detailsRegion.transform.FindChild ("Name").GetComponent<Text> ().text = "";
            detailsRegion.transform.FindChild ("Gender").GetComponent<Image> ().color = Color.clear;
            detailsRegion.transform.FindChild ("Sprite").GetComponent<Image> ().color = Color.clear;
            detailsRegion.transform.FindChild ("Markings").GetComponent<Text> ().text = "";
			detailsRegion.transform.FindChild ("Shiny").gameObject.SetActive(false);
            detailsRegion.transform.FindChild ("Level").GetComponent<Text> ().text = "";
            detailsRegion.transform.FindChild ("Types").GetChild (0).gameObject.SetActive (false);
            detailsRegion.transform.FindChild ("Types").GetChild (1).gameObject.SetActive (false);
            detailsRegion.transform.FindChild ("Ability").GetComponent<Text> ().text = "";
            detailsRegion.transform.FindChild ("Item").GetComponent<Text> ().text = "";
        } //end else
    } //end FillDetails

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

		//Reposition to location of top choice, with 2 unit offset to center it
		selection.transform.position = Camera.main.WorldToScreenPoint(currentMoveSlot.transform.position);

		//Set the move category
		moveScreen.FindChild("Category").GetComponent<Image>().sprite =
			DataContents.categorySprites[DataContents.GetMoveCategory(myPokemon.GetMove(moveChoice))];

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
	 * Name: ReadRibbon
	 * Reads or disables ribbon data
	 ***************************************/
	void ReadRibbon()
	{
		//If text isn't displayed
		if (pcState == PCGame.POKEMONRIBBONS && ribbonChoice != previousRibbonChoice && selection.activeSelf)
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
		StartCoroutine(PositionChoices());
		yield return new WaitForEndOfFrame();

		//Team Submenu
		if (pcState == PCGame.POKEMONSUBMENU || pcState == PCGame.POKEMONMARKINGS)
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
		else if (pcState == PCGame.POKEMONRIBBONS)
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
				if (pokemonChoice.CurrentLevel != 100)
				{
					summaryScreen.transform.GetChild(0).FindChild("XPBar").GetComponent<RectTransform>().localScale = new Vector3(
						(float)(pokemonChoice.EXPForLevel - pokemonChoice.RemainingEXP) /
						(float)pokemonChoice.EXPForLevel, 1, 1);
				} //end if
				else
				{
					summaryScreen.transform.GetChild(0).FindChild("XPBar").GetComponent<RectTransform>().localScale = new Vector3(0, 1, 1);
				} //end else
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
			if(pcState == PCGame.MOVESWITCH)
			{
				//Return to summary
				currentMoveSlot.GetComponent<Image>().color = Color.clear;
				summaryScreen.transform.GetChild(5).gameObject.SetActive(false);
				summaryScreen.transform.GetChild(4).gameObject.SetActive(false);
				selection.SetActive(false);

				//Change to new page
				summaryChoice = summaryPage;
				pcState = PCGame.POKEMONSUMMARY;
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
     * Name: PartyState
     * Opens/Closes the Party in PC box
     ***************************************/ 
    public IEnumerator PartyState(bool state)
    {
        //Process at end of frame
        yield return new WaitForEndOfFrame ();

        //Party to be opened
        if (state)
        {
            partyTab.SetActive(true);
            choiceNumber = 1;
            currentTeamSlot = partyTab.transform.FindChild("Pokemon1").gameObject;

			//Enable party close button
			partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;
            pcState = PCGame.PARTY;
        } //end if
        //Party to be closed
        else
        {
            partyTab.SetActive(false);
            pcState = PCGame.HOME;
        } //end else
    } //end PartyState(bool state)

	/***************************************
	 * Name: ApplyConfirm
	 * Appliees the confirm choice
	 ***************************************/
	public void ApplyConfirm(ConfirmChoice e)
	{
		//Yes selected
		if (e.Choice == 0)
		{
			//Giving item
			if (pcState == PCGame.ITEMGIVE)
			{
				//If in party
				if (partyTab.activeSelf)
				{
					int itemNumber = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];
					GameManager.instance.GetTrainer().RemoveItem(itemNumber, 1);
					GameManager.instance.GetTrainer().Team[choiceNumber - 1].Item = itemNumber;
					GameManager.instance.DisplayText("Gave " + DataContents.GetItemGameName(itemNumber) + " to " +
					GameManager.instance.GetTrainer().Team[choiceNumber - 1].Nickname + " and " +
					"put other item in bag.", true);
					playerBag.SetActive(false);
					initialize = false;

					//Enable party close button
					partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;

					//Return to party
					pcState = PCGame.PARTY;
				} //end if

				//If in pc box
				else
				{
					int itemNumber = GameManager.instance.GetTrainer().GetItem(inventorySpot)[0];
					GameManager.instance.GetTrainer().RemoveItem(itemNumber, 1);
					GameManager.instance.GetTrainer().GetPC(
						GameManager.instance.GetTrainer().GetPCBox(), boxChoice).Item = itemNumber;
					GameManager.instance.DisplayText("Gave " + DataContents.GetItemGameName(itemNumber) + " to " +
					GameManager.instance.GetTrainer().GetPC(
						GameManager.instance.GetTrainer().GetPCBox(), boxChoice).Nickname + " and " +
					"put other item in bag.", true);
					playerBag.SetActive(false);
					initialize = false;
					//Return to home
				
					pcState = PCGame.HOME;
				} //end else
			} //end if

			//Releasing pokemon
			else if (pcState == PCGame.POKEMONSUBMENU)
			{
				//If party tab is open
				if(partyTab.activeSelf && GameManager.instance.GetTrainer().Team.Count > 1)
				{
					//Get the pokemon
					selectedPokemon = GameManager.instance.GetTrainer().Team[choiceNumber-1];

					//Remove the pokemon from the party
					GameManager.instance.GetTrainer().RemovePokemon(choiceNumber-1);

					//Fill in party tab
					for(int i = 1; i < GameManager.instance.GetTrainer().Team.Count + 1; i++)
					{
						partyTab.transform.FindChild("Pokemon"+i).GetComponent<Image>().sprite =
							GetCorrectIcon(GameManager.instance.GetTrainer().Team[i-1]);
					} //end for

					//Deactivate any empty party spots
					for(int i = 6; i > GameManager.instance.GetTrainer().Team.Count; i--)
					{
						partyTab.transform.FindChild("Pokemon" + (i)).gameObject.SetActive(false);
					} //end for
				} //end if
				else
				{
					//Get the pokemon
					selectedPokemon = GameManager.instance.GetTrainer().GetPC(
						GameManager.instance.GetTrainer().GetPCBox(), boxChoice);

					//Remove the pokemon from the PC
					GameManager.instance.GetTrainer().RemoveFromPC(
						GameManager.instance.GetTrainer().GetPCBox(), boxChoice);

					//Set PC slot to clear
					boxBack.transform.FindChild("PokemonRegion").GetChild(boxChoice).
					GetComponent<Image>().color = Color.clear;
				} //end else
					
				GameManager.instance.DisplayText("You released " + selectedPokemon.Nickname, true);
				selectedPokemon = null;

				//Enable party close button
				partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;
				pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
			} //end else if
		} //end if

		//No selected
		else if(e.Choice== 1)
		{
			//Giving item
			if (pcState == PCGame.ITEMGIVE)
			{
				GameManager.instance.DisplayText("Did not switch items.", true);
				playerBag.SetActive(false);
				initialize = false;

				//Enable party close button
				partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;

				//Return to home or party
				pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
			} //end if

			//Releasing pokemon
			else if (pcState == PCGame.POKEMONSUBMENU)
			{
				GameManager.instance.DisplayText("Decided not to release " + selectedPokemon.Nickname, true);
				selectedPokemon = null;

				//Enable party close button
				partyTab.transform.FindChild("Close").GetComponent<Button>().interactable = true;
				pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
			} //end else if
		} //end else if			
	} //end ApplyConfirm(ConfirmChoice e)

	/***************************************
	 * Name: ChangeCheckpoint
	 * Changes the checkpoint
	 ***************************************/
	public void ChangeCheckpoint(int newCheckpoint)
	{
		checkpoint = newCheckpoint;
	} //end ChangeCheckpoint(int newCheckpoint)
	#endregion
} //end class PCScene

