/***************************************************************************************** 
 * File:    PCScene.cs
 * Summary: Handles process for PC scene
 *****************************************************************************************/
#region Using
using UnityEngine;
using UnityEngine.UI;
using System;
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
        POKEMONHELD,
		INPUTSCREEN
    } //end PCGame

	int checkpoint = 0;				//Manage function progress
	int boxChoice;                  //The pokemon that is highlighted
	int detailsSize;				//Font size for move description
	bool processing = false;		//Whether a function is already processing something
    PCGame pcState;                 //Current state of the PC
    Pokemon selectedPokemon;        //The currently selected pokemon
    Pokemon heldPokemon;            //Pokemon held if move was chosen
    GameObject boxBack;             //The wallpaper and pokemon panels for PC
    GameObject detailsRegion;       //Area that displays the details of a highlighted pokemon
    GameObject choiceHand;          //The highlighter for the PC
    GameObject heldImage;           //Image of the held pokemon
    GameObject partyTab;            //The panel displaying the current team in PC
	GameObject summaryScreen;		//Screen showing summary of data for pokemon
	GameObject ribbonScreen;		//Screen showing ribbons for pokemon
    GameObject currentPCSlot;       //The object that is currently highlighted
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

			//Get references
			boxBack = GameObject.Find("BoxBack");
			detailsRegion = GameObject.Find("Details");
			partyTab = GameObject.Find("PartyTab");
			heldImage = GameObject.Find("HeldPokemon");
			choiceHand = heldImage.transform.GetChild(0).gameObject;
			summaryScreen = GameObject.Find("Summary");
			ribbonScreen = GameObject.Find("Ribbons");

			//Initialize pcState
			pcState = PCGame.HOME;

			//Details size has not been set yet
			detailsSize = -1;

			//Move to next checkpoint
			checkpoint = 1;
		} //end if

		//Initialize starting state
		else if (checkpoint == 1)
		{
			//Return if processing
			if (processing)
			{
				return;
			} //end if

			//Begin processing
			processing = true;

			//Disable screens
			partyTab.SetActive(false);
			summaryScreen.SetActive(false);
			ribbonScreen.SetActive(false);

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

		//Run PC as normal
		else if (checkpoint == 2)
		{
			//Process according to PC state
			if (pcState == PCGame.HOME)
			{
				//Get player input
				GetInput();

				//Disable Party and Return buttons
                detailsRegion.transform.FindChild ("Buttons").GetChild (0).GetComponent<Button> ().
                    interactable = false;
                detailsRegion.transform.FindChild ("Buttons").GetChild (1).GetComponent<Button> ().
                    interactable = false;
                detailsRegion.transform.FindChild ("Buttons").GetChild (0).GetComponent<Image> ().color =
                    Color.grey;
                detailsRegion.transform.FindChild ("Buttons").GetChild (1).GetComponent<Image> ().color =
                    Color.grey;

				//Position choice hand
				switch (boxChoice)
				{
					//Left box arrow
					case -3:
						//Load previous PC box
						GameManager.instance.GetTrainer().PreviousBox();

						//Reset scene
						checkpoint = -1;
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
						checkpoint = -1;
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
						break;
				} //end switch
			} //end if
		} //end else if
		//                    //Pokemon region
		//                    default:
		//                    {
		//                        //Position hand
		//                        heldImage.transform.position = new Vector3 (
		//                            boxBack.transform.FindChild ("PokemonRegion").GetChild (boxChoice).position.x,
		//                            boxBack.transform.FindChild ("PokemonRegion").GetChild (boxChoice).position.y + 8, 100);
		//                        currentPCSlot = boxBack.transform.FindChild ("PokemonRegion").GetChild (boxChoice).gameObject;
		//
		//                        //Get the pokemon in the slot
		//                        selectedPokemon = GameManager.instance.GetTrainer ().GetPC (
		//                        GameManager.instance.GetTrainer ().GetPCBox (), boxChoice);
		//
		//                        //Update details region
		//                        FillDetails();
		//                        break;
		//                    } //end default (Pokemon region)
		//                } //end switch
		//            } //end if
		//            else if (pcState == PCGame.POKEMONSUBMENU)
		//            {
		//                //Initialize
		//                if(!initialize)
		//                {                  
		//                    //Reposition choices to bottom right
		//                    choices.GetComponent<RectTransform>().position = new Vector3(
		//                        choices.GetComponent<RectTransform>().position.x,
		//                        choices.GetComponent<RectTransform>().rect.height/2);
		//
		//                    //Reposition selection to top menu choice
		//                    selection.transform.position = choices.transform.GetChild(0).position;
		//
		//                    //Finished initializing
		//                    initialize = true;
		//                } //end if
		//
		//                //Get player input
		//                //GatherInput ();
		//            } //end else if
		//            else if (pcState == PCGame.POKEMONSUMMARY)
		//            {
		//                //Get player input
		//                //GatherInput ();
		//
		//                //Update selected pokemon
		//                if(heldPokemon != null)
		//                {
		//                    selectedPokemon = heldPokemon;
		//                } //end else if
		//                else if(partyTab.activeSelf)
		//                {
		//                    selectedPokemon = GameManager.instance.GetTrainer().Team[choiceNumber-1];
		//                } //end else if
		//                else
		//                {
		//                    selectedPokemon = GameManager.instance.GetTrainer ().GetPC (
		//                        GameManager.instance.GetTrainer ().GetPCBox (), boxChoice);
		//                } //end else
		//
		//                //Display pokemon summary
		//                PokemonSummary (selectedPokemon);
		//            } //end else if
		//            else if (pcState == PCGame.MOVESWITCH)
		//            {
		//                //Get player input
		//                //GatherInput ();
		//                
		//                //Highlight selected switch to
		//                selection.SetActive (true);
		//                
		//                //Resize to same as top choice
		//                Transform moveScreen = summaryScreen.transform.GetChild (5);
		//                Vector3 scale = new Vector3 (
		//                    moveScreen.FindChild ("Move" + (moveChoice + 1)).GetComponent<RectTransform> ().rect.width,
		//                    moveScreen.FindChild ("Move" + (moveChoice + 1)).GetComponent<RectTransform> ().rect.height,
		//                    0);
		//                selection.GetComponent<RectTransform> ().sizeDelta = scale;
		//                
		//                //Reposition to location of top choice, with 2 unit offset to center it
		//                selection.transform.position = Camera.main.WorldToScreenPoint (currentSwitchSlot.transform.
		//                                                                               position);
		//            } //end else if
		//            else if (pcState == PCGame.PARTY)
		//            {
		//                //Get player input
		//                //GatherInput();
		//                
		//                //Put choice hand at party slot position
		//                heldImage.transform.position = new Vector3 (currentTeamSlot.transform.position.x, 
		//                    currentTeamSlot.transform.position.y + 8, 100);
		//
		//                //Selected pokemon is same as choice
		//                if(choiceNumber > 0 && choiceNumber <= GameManager.instance.GetTrainer().Team.Count)
		//                {
		//                    selectedPokemon = GameManager.instance.GetTrainer().Team[choiceNumber-1];
		//                } //end if
		//                else
		//                {
		//                    selectedPokemon = null;
		//                } //end else
		//
		//                //Update details region
		//                FillDetails();
		//            } //end else if
		//            else if(pcState == PCGame.POKEMONHELD)
		//            {
		//                //If no pokemon is currently held
		//                if(heldPokemon == null)
		//                {
		//                    //Pokemon Party
		//                    if(partyTab.activeSelf)
		//                    {
		//                        //If on last pokemon
		//                        if(GameManager.instance.GetTrainer().Team.Count == 1)
		//                        {
		//                            GameManager.instance.DisplayText("You can't remove your last pokemon.",  true);
		//                        } //end if
		//
		//                        heldPokemon = selectedPokemon;
		//                        selectedPokemon = null;
		//                        GameManager.instance.GetTrainer().RemovePokemon(choiceNumber-1);
		//                        choiceHand.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/boxfist");
		//                        heldImage.GetComponent<Image>().color = Color.white;
		//						heldImage.GetComponent<Image>().sprite = GetCorrectIcon(heldPokemon);
		//
		//                        //Fill in party tab
		//                        for(int i = 1; i < GameManager.instance.GetTrainer().Team.Count + 1; i++)
		//                        {
		//                            partyTab.transform.FindChild("Pokemon"+i).GetComponent<Image>().sprite =
		//                                GetCorrectIcon(GameManager.instance.GetTrainer().Team[i-1]);
		//                        } //end for
		//
		//                        //Deactivate any empty party spots
		//                        for(int i = 6; i > GameManager.instance.GetTrainer().Team.Count; i--)
		//                        {
		//                            partyTab.transform.FindChild("Pokemon" + (i)).gameObject.SetActive(false);
		//                        } //end for
		//                    } //end if
		//                    //Pokemon Region of box
		//                    else
		//                    {
		//                        heldPokemon = selectedPokemon;
		//                        selectedPokemon = null;
		//                        GameManager.instance.GetTrainer().RemoveFromPC(
		//                            GameManager.instance.GetTrainer ().GetPCBox (), boxChoice);
		//                        choiceHand.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/boxfist");
		//                        heldImage.GetComponent<Image>().color = Color.white;
		//						heldImage.GetComponent<Image>().sprite = GetCorrectIcon(heldPokemon);
		//                        boxBack.transform.FindChild ("PokemonRegion").GetChild (boxChoice).GetComponent<Image> ().
		//                            color = Color.clear;
		//                    } //end else
		//
		//                    //Move choice now becomes swap
		//                    choices.transform.GetChild(0).GetComponent<Text>().text = "Swap";
		//                } //end if
		//                //Otherwise switch pokemon
		//                else
		//                {
		//                    //Pokemon Party
		//                    if(partyTab.activeSelf)
		//                    {
		//                        //If there is a pokemon
		//                        if(selectedPokemon != null)
		//                        {
		//                            GameManager.instance.GetTrainer().ReplacePokemon(heldPokemon, choiceNumber-1);
		//                            heldPokemon = selectedPokemon;
		//                            selectedPokemon = GameManager.instance.GetTrainer ().Team[choiceNumber-1];
		//							heldImage.GetComponent<Image>().sprite = GetCorrectIcon(heldPokemon);
		//							partyTab.transform.FindChild("Pokemon" + choiceNumber).GetComponent<Image>().
		//								sprite = GetCorrectIcon(selectedPokemon);
		//                        } //end if
		//                        else
		//                        {
		//                            GameManager.instance.GetTrainer().AddPokemon(heldPokemon);
		//                            selectedPokemon = heldPokemon;
		//                            heldPokemon = null;
		//                            choiceHand.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/boxpoint1");
		//                            heldImage.GetComponent<Image>().color = Color.clear;
		//							partyTab.transform.FindChild("Pokemon" + choiceNumber).GetComponent<Image>().
		//								sprite = GetCorrectIcon(selectedPokemon);
		//                            partyTab.transform.FindChild("Pokemon" + choiceNumber).gameObject.SetActive(true);
		//                        } //end else
		//                    } //end if
		//                    //Pokemon Region of box
		//                    else
		//                    {
		//                        //If there is a pokemon
		//                        if(selectedPokemon != null)
		//                        {
		//                            GameManager.instance.GetTrainer().RemoveFromPC(
		//                                GameManager.instance.GetTrainer ().GetPCBox (), boxChoice);
		//                            GameManager.instance.GetTrainer().AddToPC(
		//                                GameManager.instance.GetTrainer().GetPCBox(), boxChoice, heldPokemon);
		//                            heldPokemon = selectedPokemon;
		//                            selectedPokemon = GameManager.instance.GetTrainer ().GetPC(
		//                                GameManager.instance.GetTrainer().GetPCBox(), boxChoice);
		//							heldImage.GetComponent<Image>().sprite = GetCorrectIcon(heldPokemon);
		//							boxBack.transform.FindChild("PokemonRegion").GetChild(boxChoice).GetComponent<Image>().
		//							sprite = GetCorrectIcon(selectedPokemon);
		//                        } //end if
		//                        else
		//                        {
		//                            GameManager.instance.GetTrainer().AddToPC(
		//                                GameManager.instance.GetTrainer().GetPCBox(), boxChoice, heldPokemon);
		//                            selectedPokemon = heldPokemon;
		//                            heldPokemon = null;
		//                            choiceHand.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/boxpoint1");
		//                            heldImage.GetComponent<Image>().color = Color.clear;
		//							boxBack.transform.FindChild("PokemonRegion").GetChild(boxChoice).GetComponent<Image>().sprite = 
		//								GetCorrectIcon(selectedPokemon);
		//                            boxBack.transform.FindChild ("PokemonRegion").GetChild (boxChoice).GetComponent<Image> ().
		//                                color = Color.white;
		//
		//                            //Swap choice now becomes move
		//                            choices.transform.GetChild(0).GetComponent<Text>().text = "Move";
		//                        } //end else
		//                    } //end else
		//                } //end else
		//
		//                //Go back to previous state
		//                pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
		//            } //end else if
		//            else if(pcState == PCGame.POKEMONRIBBONS)
		//            {
		//                if(!initialize)
		//                {
		//                    //Fill in ribbon screen with correct data
		//                    ribbonScreen.SetActive(true);
		//                    ribbonScreen.transform.FindChild("Name").GetComponent<Text>().text=
		//                        selectedPokemon.Nickname;
		//                    ribbonScreen.transform.FindChild("Level").GetComponent<Text>().text=
		//                        selectedPokemon.CurrentLevel.ToString();
		//                    ribbonScreen.transform.FindChild("Ball").GetComponent<Image>().sprite=
		//                        Resources.Load<Sprite>("Sprites/Icons/summaryBall"+selectedPokemon.BallUsed.ToString("00"));
		//                    ribbonScreen.transform.FindChild("Gender").GetComponent<Image>().sprite=
		//                        Resources.Load<Sprite>("Sprites/Icons/gender"+selectedPokemon.Gender.ToString());
		//                    ribbonScreen.transform.FindChild("Sprite").GetComponent<Image>().sprite=
		//                        Resources.Load<Sprite>("Sprites/Pokemon/"+selectedPokemon.NatSpecies.ToString("000"));
		//                    ribbonScreen.transform.FindChild("Markings").GetComponent<Text>().text=
		//                        selectedPokemon.GetMarkingsString();
		//                    ribbonScreen.transform.FindChild("Item").GetComponent<Text>().text=
		//                        DataContents.GetItemGameName(selectedPokemon.Item);
		//                    ribbonScreen.transform.FindChild("RibbonName").gameObject.SetActive(false);
		//                    ribbonScreen.transform.FindChild("RibbonDescription").gameObject.SetActive(false);
		//                    
		//                    //No ribbon selected yet
		//                    previousRibbonChoice = -1;
		//                    ribbonChoice = 0;
		//                    selection.SetActive(false);
		//                    
		//                    //Set existing ribbons to inactive
		//                    foreach(Transform child in ribbonScreen.transform.FindChild("RibbonRegion").transform)
		//                    {
		//                        child.gameObject.SetActive(false);
		//                    } //end for
		//                    
		//                    //Add ribbons
		//                    for(int i = 0; i < selectedPokemon.GetRibbonCount();i++)
		//                    {
		//                        //If at least one ribbon exists, resize the selection box
		//                        if(i == 0)
		//                        {                            
		//                            StartCoroutine("WaitForResize");
		//                        } //end if
		//                        
		//                        //The ribbon already exists, just fill it in
		//                        if(i < ribbonScreen.transform.FindChild("RibbonRegion").childCount)
		//                        {
		//                            GameObject newRibbon = ribbonScreen.transform.FindChild("RibbonRegion").GetChild(i).
		//                                gameObject;
		//                            newRibbon.gameObject.SetActive(true);
		//                            newRibbon.GetComponent<Image>().sprite = DataContents.ribbonSprites[
		//                                selectedPokemon.GetRibbon(i)];
		//                        } //end if
		//                        //Create new ribbon
		//                        else
		//                        {
		//                            GameObject newRibbon = Instantiate(ribbonScreen.transform.FindChild("RibbonRegion").
		//                                GetChild(0).gameObject);
		//                            newRibbon.transform.SetParent(ribbonScreen.transform.FindChild("RibbonRegion"));
		//                            newRibbon.GetComponent<Image>().sprite = DataContents.ribbonSprites[
		//                                selectedPokemon.GetRibbon(i)];
		//                            newRibbon.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
		//                            newRibbon.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
		//                            newRibbon.SetActive(true);
		//                        } //end else
		//                    } //end for
		//                    
		//                    //Initialization is done
		//                    initialize = true;
		//                } //end if
		//                
		//                //Get player input
		//                //GatherInput();
		//            } //end else if
		//            else if (pcState == PCGame.POKEMONMARKINGS)
		//            {
		//                //Initialize
		//                if(!initialize)
		//                {                              
		//                    //Fill in choices box
		//                    FillInChoices();
		//
		//                    //SubmenuChoice at top
		//                    subMenuChoice = 0;
		//
		//                    //Reposition choices menu and selection rectangle
		//                    StartCoroutine("WaitForResize");
		//
		//                    //Initialize finished
		//                    initialize = true;
		//                } //end if
		//
		//                //Reposition selection rectangle
		//                selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
		//
		//                //Get player input
		//                //GatherInput ();
		//            } //end else if
		//			else if(pcState == PCGame.INPUTSCREEN)
		//			{
		//				//Make sure text field is always active
		//				if(input.activeInHierarchy)
		//				{
		//					inputText.ActivateInputField();
		//				} //end if
		//
		//				//Get player input
		//				//GatherInput();
		//			} //end else if
		//
		//            //End processing
		//            processing = false;
		//        } //end else if
	} //end RunPC

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

	//    /***************************************
	//     * Name: FillInChoices
	//     * Sets the choices for the choice menu
	//     * depending on the scene
	//     ***************************************/
	//    void FillInChoices()
	//    {
	//        //If in PC in Pokemon Region
	//        else if (sceneState == OverallGame.PC && boxChoice > -1)
	//        {
	//            //Fill in choices box
	//            for (int i = choices.transform.childCount - 1; i < 6; i++)
	//            {
	//                GameObject clone = Instantiate (choices.transform.GetChild (0).gameObject,
	//                                       choices.transform.GetChild (0).position,
	//                                       Quaternion.identity) as GameObject;
	//                clone.transform.SetParent (choices.transform);
	//            } //end for
	//            choices.transform.GetChild (0).GetComponent<Text> ().text = "Move";
	//            choices.transform.GetChild (1).GetComponent<Text> ().text = "Summary";
	//            choices.transform.GetChild (2).GetComponent<Text> ().text = "Item";
	//            choices.transform.GetChild (3).GetComponent<Text> ().text = "Ribbons";
	//            choices.transform.GetChild (4).GetComponent<Text> ().text = "Markings";
	//            choices.transform.GetChild (5).GetComponent<Text> ().text = "Release";
	//            choices.transform.GetChild (6).GetComponent<Text> ().text = "Cancel";
	//            choices.transform.GetChild (0).GetComponent<Text> ().color = Color.black;
	//            choices.transform.GetChild (1).GetComponent<Text> ().color = Color.black;
	//            choices.transform.GetChild (2).GetComponent<Text> ().color = Color.black;
	//            choices.transform.GetChild (3).GetComponent<Text> ().color = Color.black;
	//            choices.transform.GetChild (4).GetComponent<Text> ().color = Color.black;
	//            choices.transform.GetChild (5).GetComponent<Text> ().color = Color.black;
	//            choices.transform.GetChild (6).GetComponent<Text> ().color = Color.black;
	//            if (choices.transform.childCount > 7)
	//            {
	//                for (int i = 7; i < choices.transform.childCount; i++)
	//                {
	//                    Destroy (choices.transform.GetChild (i).gameObject);
	//                } //end for
	//            } //end if
	//        } //end else if
	//        //If in PC on Box Title
	//        else if (sceneState == OverallGame.PC && boxChoice == -2)
	//        {
	//            //Fill in choices box
	//            for (int i = choices.transform.childCount-1; i < 3; i++)
	//            {
	//                GameObject clone = Instantiate (choices.transform.GetChild (0).gameObject,
	//                    choices.transform.GetChild (0).position,
	//                    Quaternion.identity) as GameObject;
	//                clone.transform.SetParent (choices.transform);
	//            } //end for
	//            choices.transform.GetChild (0).GetComponent<Text> ().text = "Jump To";
	//            choices.transform.GetChild (1).GetComponent<Text> ().text = "Rename";
	//            choices.transform.GetChild (2).GetComponent<Text> ().text = "Wallpaper";
	//            choices.transform.GetChild (3).GetComponent<Text> ().text = "Cancel";
	//            choices.transform.GetChild (0).GetComponent<Text> ().color = Color.black;
	//            choices.transform.GetChild (1).GetComponent<Text> ().color = Color.black;
	//            choices.transform.GetChild (2).GetComponent<Text> ().color = Color.black;
	//            choices.transform.GetChild (3).GetComponent<Text> ().color = Color.black;
	//            if (choices.transform.childCount > 4)
	//            {
	//                for (int i = 4; i < choices.transform.childCount; i++)
	//                {
	//                    Destroy (choices.transform.GetChild (i).gameObject);
	//                } //end for
	//            } //end if
	//        }  //end else if
	//        //If in PC Markings
	//        else if (sceneState == OverallGame.PC && pcState == PCGame.POKEMONMARKINGS)
	//        {
	//            for (int i = choices.transform.childCount-1; i < DataContents.markingCharacters.Length+2; i++)
	//            {
	//                GameObject clone = Instantiate (choices.transform.GetChild (0).gameObject,
	//                    choices.transform.GetChild (0).position, Quaternion.identity) as GameObject;
	//                clone.transform.SetParent (choices.transform);
	//            } //end for
	//            for(int i = 0; i < DataContents.markingCharacters.Length+2; i++)
	//            {
	//                if(i == DataContents.markingCharacters.Length)
	//                {
	//                    choices.transform.GetChild(i).GetComponent<Text>().text = "OK";
	//                } //end if
	//                else if(i == DataContents.markingCharacters.Length+1)
	//                {
	//                    choices.transform.GetChild(i).GetComponent<Text>().text = "Cancel";
	//                } //end else if
	//                else
	//                {                           
	//                    choices.transform.GetChild(i).GetComponent<Text>().text =
	//                        DataContents.markingCharacters[i].ToString(); 
	//                } //end else
	//            } //end for
	//
	//            //Destroy extra
	//            if (choices.transform.childCount > DataContents.markingCharacters.Length+1)
	//            {
	//                for (int i = DataContents.markingCharacters.Length+2; i < choices.transform.childCount; i++)
	//                {
	//                    Destroy (choices.transform.GetChild (i).gameObject);
	//                } //end for
	//            } //end if
	//
	//            //Color in choices
	//            for(int i = 0; i < markingChoices.Count; i++)
	//            {
	//                choices.transform.GetChild(i).GetComponent<Text>().color =
	//                    markingChoices[i] ? Color.black : Color.gray;
	//            } //end for           
	//        } //end else if
	//    } //end FillInChoices

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
	 * Name: ChangeCheckpoint
	 * Changes the checkpoint
	 ***************************************/
	public void ChangeCheckpoint(int newCheckpoint)
	{
		checkpoint = newCheckpoint;
	} //end ChangeCheckpoint(int newCheckpoint)
	#endregion
} //end class PCScene

