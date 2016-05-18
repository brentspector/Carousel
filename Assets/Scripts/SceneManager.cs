///***************************************************************************************** 
// * File:    SceneManager.cs
// * Summary: Controls behavior for each scene in the game
// *****************************************************************************************/ 
//#region Using
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;
//using UnityEngine.SceneManagement;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//#endregion
//
//public class SceneManager : MonoBehaviour 
//{
//    #region Variables
//    //Scene Variables
//    public enum OverallGame
//    {
//        INTRO,
//        MENU,
//        NEWGAME,
//        CONTINUE,
//        PC,
//        SHOP,
//        POKEDEX
//    } //end OverallGame
//

//
//    //PC game states
//    public enum PCGame
//    {
//        HOME,
//        PARTY,
//        POKEMONSUBMENU,
//        POKEMONSUMMARY,
//        POKEMONRIBBONS,
//        POKEMONMARKINGS,
//        MOVESWITCH,
//        POKEMONHELD,
//		INPUTSCREEN
//    } //end PCGame
//
//    //Continue Game variables
//   
//
//    //PC variables
//    PCGame pcState;                 //Current state of the PC
//    Pokemon selectedPokemon;        //The currently selected pokemon
//    Pokemon heldPokemon;            //Pokemon held if move was chosen
//    GameObject boxBack;             //The wallpaper and pokemon panels for PC
//    GameObject detailsRegion;       //Area that displays the details of a highlighted pokemon
//    GameObject choiceHand;          //The highlighter for the PC
//    GameObject heldImage;           //Image of the held pokemon
//    GameObject partyTab;            //The panel displaying the current team in PC
//    GameObject currentPCSlot;       //The object that is currently highlighted
//    List<bool> markingChoices;      //A list of the marking choices made for the pokemon
//    int boxChoice;                  //The pokemon that is highlighted
//
//    //Pokedex variables
//    GameObject index;               //Region displaying overall pokedex location
//    GameObject stats;               //The stat bars of the selected pokemon
//    GameObject characteristics;     //The characteristics of the selected pokemon
//    GameObject movesRegion;         //The moves the selected pokemon can learn
//    GameObject evolutionsRegion;    //The pokemon the selected pokemon can evolve into
//    GameObject shownButton;         //Whether weaknesses or resistances are shown
//    GameObject weakTypes;           //Object containing all types selected pokemon is weak to
//    GameObject resistTypes;         //Object containing all types selected pokemon is resistant to
//    Image pokemonImage;             //The image of the selected pokemon
//    Text abilitiesText;             //The abilities the selected pokemon can have
//    Text pokedexText;               //The flavor text for the selected pokemon
//    int pokedexIndex;               //The location in the pokedex currently highlighted
//    #endregion
//
//    #region Methods
//	#region Scenes
//
//    /***************************************
//     * Name: ContinueGame
//     * Loads and plays the main game
//     ***************************************/
//    public void ContinueGame()
//    {
//        //If something is already happening, return
//        if(processing)
//        {
//            return;
//        } //end if
//        
//       
//
//            //End processing
//            processing = false;
//        } //end else if
//    } //end ContinueGame
//
//    /***************************************
//     * Name: PC
//     * Loads and plays the PC scene 
//     ***************************************/
//    public void PC()
//    {
//        //Return if processing
//        if (processing)
//        {
//            return;
//        } //end if
//
//        //Initialize each scene only once
//        if (checkpoint == 0)
//        {         
//            //Begin processing 
//            processing = true;
//
//            //Get references
//            boxBack = GameObject.Find ("BoxBack");
//            detailsRegion = GameObject.Find ("Details");
//            partyTab = GameObject.Find ("PartyTab");
//            heldImage = GameObject.Find ("HeldPokemon");
//            choiceHand = heldImage.transform.GetChild(0).gameObject;
//            summaryScreen = GameObject.Find ("Summary");
//            ribbonScreen = GameObject.Find ("Ribbons");
//
//            //Initialize pcState
//            pcState = PCGame.HOME;
//
//            //Details size has not been set yet
//            detailsSize = -1;
//
//            //Move to next checkpoint
//            checkpoint = 1;
//
//            //End processing
//            processing = false;
//        } //end if
//        else if (checkpoint == 1)
//        {
//            //Begin processing 
//            processing = true;
//
//            //Disable screens
//            partyTab.SetActive (false);
//            summaryScreen.SetActive (false);
//            ribbonScreen.SetActive (false);
//
//            //Fill in box
//            for (int i = 0; i < 30; i++)
//            {
//                //Get the pokemon in the slot
//                selectedPokemon = GameManager.instance.GetTrainer ().GetPC (
//                    GameManager.instance.GetTrainer ().GetPCBox (), i);
//
//                //If the slot is null, set the sprite to clear
//                if (selectedPokemon == null)
//                {
//                    boxBack.transform.FindChild ("PokemonRegion").GetChild (i).GetComponent<Image> ().
//                        color = Color.clear;
//                } //end if
//                //Otherwise fill in the icon for the pokemon
//                else
//                {
//                    boxBack.transform.FindChild ("PokemonRegion").GetChild (i).GetComponent<Image> ().
//                        color = Color.white;
//					boxBack.transform.FindChild("PokemonRegion").GetChild(i).GetComponent<Image>().
//						sprite = GetCorrectIcon(selectedPokemon);
//                } //end else
//            } //end for
//            
//            //Fill in details
//            boxBack.transform.FindChild ("BoxName").GetComponent<Text> ().text = GameManager.instance.
//                GetTrainer ().GetPCBoxName ();
//            boxBack.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Sprites/Menus/box" + 
//                GameManager.instance.GetTrainer ().GetPCBoxWallpaper ());
//            FillDetails ();
//
//            //Fill in party tab
//            for(int i = 1; i < GameManager.instance.GetTrainer().Team.Count + 1; i++)
//            {
//                partyTab.transform.FindChild("Pokemon"+i).GetComponent<Image>().sprite =
//                    GetCorrectIcon(GameManager.instance.GetTrainer().Team[i-1]);
//            } //end for
//
//            //Deactivate any empty party spots
//            for(int i = 6; i > GameManager.instance.GetTrainer().Team.Count; i--)
//            {
//                partyTab.transform.FindChild("Pokemon" + i).gameObject.SetActive(false);
//            } //end for
//
//            //Initialize box choice to boxName
//            heldImage.transform.position = new Vector3 (boxBack.transform.FindChild ("BoxName").position.x,
//                boxBack.transform.FindChild ("BoxName").position.y + 8, 100);
//            boxChoice = -2;
//
//            //Current slot is title
//            currentPCSlot = boxBack.transform.FindChild ("BoxName").gameObject;
//
//            //Move to next checkpoint
//            StartCoroutine (FadeInAnimation (2));
//        } //end else if
//        else if (checkpoint == 2)
//        {
//            //Begin processing 
//            processing = true;
//
//            //Process according to PC state
//            if (pcState == PCGame.HOME)
//            {                
//                //Get player input
//                //GatherInput ();
//
//                //Disable Party and Return buttons
//                detailsRegion.transform.FindChild ("Buttons").GetChild (0).GetComponent<Button> ().
//                    interactable = false;
//                detailsRegion.transform.FindChild ("Buttons").GetChild (1).GetComponent<Button> ().
//                    interactable = false;
//                detailsRegion.transform.FindChild ("Buttons").GetChild (0).GetComponent<Image> ().color =
//                    Color.grey;
//                detailsRegion.transform.FindChild ("Buttons").GetChild (1).GetComponent<Image> ().color =
//                    Color.grey;
//                
//                //Position choice hand
//                switch (boxChoice)
//                {
//                    //Left box arrow
//                    case -3:
//                    {
//                        //Load previous PC box
//                        GameManager.instance.GetTrainer ().PreviousBox ();
//                                                                                
//                        //Reset scene
//                        checkpoint = 1;
//                        break;
//                    } //end case -3 (Left box arrow)
//                    //Box name
//                    case -2:
//                    {
//                        heldImage.transform.position = new Vector3 (boxBack.transform.FindChild ("BoxName").position.x,
//                            boxBack.transform.FindChild ("BoxName").position.y + 8, 100);
//                        currentPCSlot = boxBack.transform.FindChild ("BoxName").gameObject;
//                        selectedPokemon = null;
//
//                        //Update details region
//                        FillDetails();
//                        break;
//                    } //end case -2 (Box name)
//                    //Right box arrow
//                    case -1:
//                    {
//                        //Load next PC box
//                        GameManager.instance.GetTrainer ().NextBox ();
//                    
//                        //Reset scene
//                        checkpoint = 1;
//                        break;
//                    } //end case -1 (Right box arrow)
//                    //Party button
//                    case 30:
//                    {
//                        detailsRegion.transform.FindChild ("Buttons").GetChild (0).GetComponent<Button> ().
//                            interactable = true;
//                        detailsRegion.transform.FindChild ("Buttons").GetChild (0).GetComponent<Image> ().color =
//                            Color.white;
//                        heldImage.transform.position = new Vector3 (
//                            detailsRegion.transform.FindChild ("Buttons").GetChild (0).position.x,
//                            detailsRegion.transform.FindChild ("Buttons").GetChild (0).position.y + 8, 100);
//                        currentPCSlot = detailsRegion.transform.FindChild ("Buttons").GetChild (0).gameObject;
//                        selectedPokemon = null;
//
//                        //Update details region
//                        FillDetails();
//                        break;
//                    } //end case 30 (Party button)
//                    //Return button
//                    case 31:
//                    {
//                        detailsRegion.transform.FindChild ("Buttons").GetChild (1).GetComponent<Button> ().
//                            interactable = true;
//                        detailsRegion.transform.FindChild ("Buttons").GetChild (1).GetComponent<Image> ().color =
//                            Color.white;
//                        heldImage.transform.position = new Vector3 (
//                            detailsRegion.transform.FindChild ("Buttons").GetChild (1).position.x,
//                            detailsRegion.transform.FindChild ("Buttons").GetChild (1).position.y + 8, 100);
//                        currentPCSlot = detailsRegion.transform.FindChild ("Buttons").GetChild (1).gameObject;
//                        selectedPokemon = null;
//
//                        //Update details region
//                        FillDetails();
//                        break;
//                    } //end case 31 (Return button)
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
//    } //end PC
//
//    /***************************************
//     * Name: Pokedex
//     * Loads and plays the Pokedex scene
//     ***************************************/
//    public void Pokedex()
//    {
//        //End function if processing
//        if (processing)
//        {
//            return;
//        } //end if
//
//        //Handle each stage of the scene
//        if (checkpoint == 0)
//        {
//            //Begin processing
//            processing = true;
//
//            //Initialize references
//            index = GameObject.Find("Index");
//            stats = GameObject.Find("Stats");
//            characteristics = GameObject.Find("Characteristics");         
//            movesRegion  = GameObject.Find("MovesRegion").transform.FindChild("MovesContainer").gameObject;             
//            evolutionsRegion  = GameObject.Find("Evolutions").transform.FindChild("EvolutionContainer").gameObject;        
//            shownButton = GameObject.Find("Shown");             
//            weakTypes = GameObject.Find("WeakTypes");               
//            resistTypes = GameObject.Find("ResistanceTypes");             
//            pokemonImage = GameObject.Find("Sprite").GetComponent<Image>();                 
//            abilitiesText = GameObject.Find("AbilitiesText").GetComponent<Text>();
//            pokedexText = GameObject.Find("PokedexText").GetComponent<Text>();
//            pokedexIndex = 1;
//                    
//            //Move to next checkpoint
//            checkpoint = 1;
//
//            //End processing
//            processing = false;
//        } //end if
//        else if (checkpoint == 1)
//        {
//            //Begin processing
//            processing = true;
//
//            //Fill in index region
//            FillInIndex();
//
//            //Fill in highlighted sprite
//            pokemonImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Pokemon/" + 
//                pokedexIndex.ToString("000"));
//
//            //Adjust stat bars
//            float dbStat = DataContents.ExecuteSQL<float>("SELECT health FROM Pokemon WHERE rowid=" + pokedexIndex);
//            float scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
//            Transform barChild = stats.transform.GetChild(1).FindChild("HPBar");
//            barChild.GetComponent<RectTransform>().localScale = new Vector3(scale, 1, 1);
//            barChild.GetComponent<Image>().color = dbStat < 85 ?
//                Color.Lerp(Color.red, Color.yellow, (float)dbStat/85f) : 
//                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-85f)/65f);
//            dbStat = DataContents.ExecuteSQL<float>("SELECT attack FROM Pokemon WHERE rowid=" + pokedexIndex);
//            scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
//            barChild = stats.transform.GetChild(1).FindChild("AttackBar");
//            barChild.GetComponent<RectTransform>().localScale = 
//                new Vector3(scale, 1, 1);
//            barChild.GetComponent<Image>().color = dbStat < 100 ?
//                Color.Lerp(Color.red, Color.yellow, (float)dbStat/100f) : 
//                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-100f)/50f);
//            dbStat = DataContents.ExecuteSQL<float>("SELECT defence FROM Pokemon WHERE rowid=" + pokedexIndex);
//            scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
//            barChild = stats.transform.GetChild(1).FindChild("DefenceBar");
//            barChild.GetComponent<RectTransform>().localScale = 
//                new Vector3(scale, 1, 1);
//            barChild.GetComponent<Image>().color = dbStat < 100 ?
//                Color.Lerp(Color.red, Color.yellow, (float)dbStat/100f) : 
//                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-100f)/50f);
//            dbStat = DataContents.ExecuteSQL<float>("SELECT specialAttack FROM Pokemon WHERE rowid=" + pokedexIndex);
//            scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
//            barChild = stats.transform.GetChild(1).FindChild("SpecialAttackBar");
//            barChild.GetComponent<RectTransform>().localScale = 
//                new Vector3(scale, 1, 1);
//            barChild.GetComponent<Image>().color = dbStat < 100 ?
//                Color.Lerp(Color.red, Color.yellow, (float)dbStat/100f) : 
//                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-100f)/50f);
//            dbStat = DataContents.ExecuteSQL<float>("SELECT specialDefence FROM Pokemon WHERE rowid=" + pokedexIndex);
//            scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
//            barChild = stats.transform.GetChild(1).FindChild("SpecialDefenceBar");
//            barChild.GetComponent<RectTransform>().localScale = 
//                new Vector3(scale, 1, 1);
//            barChild.GetComponent<Image>().color = dbStat < 95 ?
//                Color.Lerp(Color.red, Color.yellow, (float)dbStat/95f) : 
//                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-95f)/55f);
//            dbStat = DataContents.ExecuteSQL<float>("SELECT speed FROM Pokemon WHERE rowid=" + pokedexIndex);
//            scale = ExtensionMethods.CapAtFloat(dbStat/130f, 1);
//            barChild = stats.transform.GetChild(1).FindChild("SpeedBar");
//            barChild.GetComponent<RectTransform>().localScale = 
//                new Vector3(scale, 1, 1);
//            barChild.GetComponent<Image>().color = dbStat < 95 ?
//                Color.Lerp(Color.red, Color.yellow, (float)dbStat/95f) : 
//                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-95)/35f);
//
//            //Set types
//            Transform typeRegion = characteristics.transform.GetChild(1).FindChild("Types");
//            SetTypeSprites(typeRegion.GetChild(0).GetComponent<Image>(), typeRegion.GetChild(1).GetComponent<Image>(),
//                pokedexIndex);
//
//            //Set egg groups
//            characteristics.transform.GetChild(1).FindChild("EggText").GetComponent<Text>().text = 
//                DataContents.ExecuteSQL<string>("SELECT compatibility1 FROM Pokemon WHERE rowid=" + pokedexIndex) +
//                "   " + DataContents.ExecuteSQL<string>("SELECT compatibility2 FROM Pokemon WHERE rowid=" + pokedexIndex);
//
//            //Set height
//            characteristics.transform.GetChild(1).FindChild("HeightText").GetComponent<Text>().text = 
//                Math.Round(DataContents.ExecuteSQL<float>("SELECT height FROM Pokemon WHERE rowid=" + pokedexIndex), 1).
//                ToString();
//
//            //Set weight
//            characteristics.transform.GetChild(1).FindChild("WeightText").GetComponent<Text>().text =
//                Math.Round(DataContents.ExecuteSQL<float>("SELECT weight FROM Pokemon WHERE rowid=" +  pokedexIndex), 1).
//                ToString();
//
//            //Set species
//            characteristics.transform.GetChild(1).FindChild("SpeciesText").GetComponent<Text>().text =
//                DataContents.ExecuteSQL<string>("SELECT kind FROM Pokemon WHERE rowid=" + pokedexIndex) + " Pokemon";
//
//            //Set level up moves
//            string moveList = DataContents.ExecuteSQL<string>("SELECT moves FROM Pokemon WHERE rowid=" + pokedexIndex);
//            string[] arrayList = moveList.Split(',');
//            Text levelText = movesRegion.transform.FindChild("MoveLevel").GetComponent<Text>();
//            Text moveText = movesRegion.transform.FindChild("MoveName").GetComponent<Text>();
//            levelText.text = "";
//            moveText.text = "";
//            for(int i = 0; i < arrayList.Length-2; i+=2)
//            {
//                levelText.text += arrayList[i] + "\n";
//                moveText.text += arrayList[i+1] + "\n";
//            } //end for
//            levelText.text += arrayList[arrayList.Length-2];
//            moveText.text += arrayList[arrayList.Length-1];
//
//            //Add egg moves to move list
//            moveList = DataContents.ExecuteSQL<string>("SELECT eggMoves FROM Pokemon WHERE rowid=" + pokedexIndex);
//            arrayList = moveList.Split(',');
//            for(int i = 0; i < arrayList.Length; i++)
//            {
//                levelText.text += "\nEgg";
//                moveText.text += "\n" + arrayList[i];
//            } //end for
//
//            //Add evolutions
//            string evolveList = DataContents.ExecuteSQL<string>("SELECT evolutions FROM Pokemon WHERE rowid=" + pokedexIndex);
//            arrayList = evolveList.Split(',');
//            Text nameText = evolutionsRegion.transform.FindChild("EvolutionName").GetComponent<Text>();
//            Text methodText = evolutionsRegion.transform.FindChild("EvolutionMethod").GetComponent<Text>();
//            nameText.text = "";
//            methodText.text = "";
//            if(arrayList.Length == 1)
//            {
//                nameText.text = "None";
//            } //end if
//            else
//            {
//                for(int i = 0; i < arrayList.Length; i+=3)
//                {
//                    nameText.text += arrayList[i] + "\n";
//                    methodText.text += string.IsNullOrEmpty(arrayList[i+2]) ? arrayList[i+1] + "\n" :
//                        arrayList[i+2] + "\n";
//                } //end for
//            } //end else
//
//            //Set pokedex text
//            pokedexText.text = DataContents.ExecuteSQL<string>("SELECT pokedex FROM Pokemon WHERE rowid=" + pokedexIndex);
//
//            //Set abilities text
//            abilitiesText.text = DataContents.ExecuteSQL<string>("SELECT ability1 FROM Pokemon WHERE rowid=" + pokedexIndex);
//            string ability2 = DataContents.ExecuteSQL<string>("SELECT ability2 FROM Pokemon WHERE rowid=" + pokedexIndex);
//            string hiddenAbility = DataContents.ExecuteSQL<string>("SELECT hiddenAbility FROM Pokemon WHERE rowid=" + pokedexIndex);
//            abilitiesText.text += String.IsNullOrEmpty(ability2) ? String.IsNullOrEmpty(hiddenAbility) ? "" : 
//                "," + hiddenAbility : String.IsNullOrEmpty(hiddenAbility) ? "," + ability2 : 
//                "," + ability2 + "," + hiddenAbility;
//
//            //Set weakness and resistances
//            SetWeakResistSprites();
//
//            //Turn off resistances
//            resistTypes.SetActive(false);
//
//            //Move to next checkpoint
//            StartCoroutine (FadeInAnimation (2));
//                        
//            //End processing
//            processing = false;
//        } //end else if
//        else if (checkpoint == 2)
//        {
//            //Begin processing
//            processing = true;
//
//            //Get player input
//            //GatherInput();
//
//            //Fill in index region
//            FillInIndex();
//            
//            //Fill in highlighted sprite
//            pokemonImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Pokemon/" + 
//                    pokedexIndex.ToString("000"));
//            
//            //Adjust stat bars
//            float dbStat = DataContents.ExecuteSQL<float>("SELECT health FROM Pokemon WHERE rowid=" + pokedexIndex);
//            float scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
//            Transform barChild = stats.transform.GetChild(1).FindChild("HPBar");
//            barChild.GetComponent<RectTransform>().localScale = new Vector3(scale, 1, 1);
//            barChild.GetComponent<Image>().color = dbStat < 85 ?
//                Color.Lerp(Color.red, Color.yellow, (float)dbStat/85f) : 
//                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-85f)/65f);
//            dbStat = DataContents.ExecuteSQL<float>("SELECT attack FROM Pokemon WHERE rowid=" + pokedexIndex);
//            scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
//            barChild = stats.transform.GetChild(1).FindChild("AttackBar");
//            barChild.GetComponent<RectTransform>().localScale = 
//                new Vector3(scale, 1, 1);
//            barChild.GetComponent<Image>().color = dbStat < 100 ?
//                Color.Lerp(Color.red, Color.yellow, (float)dbStat/100f) : 
//                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-100f)/50f);
//            dbStat = DataContents.ExecuteSQL<float>("SELECT defence FROM Pokemon WHERE rowid=" + pokedexIndex);
//            scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
//            barChild = stats.transform.GetChild(1).FindChild("DefenceBar");
//            barChild.GetComponent<RectTransform>().localScale = 
//                new Vector3(scale, 1, 1);
//            barChild.GetComponent<Image>().color = dbStat < 100 ?
//                Color.Lerp(Color.red, Color.yellow, (float)dbStat/100f) : 
//                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-100f)/50f);
//            dbStat = DataContents.ExecuteSQL<float>("SELECT specialAttack FROM Pokemon WHERE rowid=" + pokedexIndex);
//            scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
//            barChild = stats.transform.GetChild(1).FindChild("SpecialAttackBar");
//            barChild.GetComponent<RectTransform>().localScale = 
//                new Vector3(scale, 1, 1);
//            barChild.GetComponent<Image>().color = dbStat < 100 ?
//                Color.Lerp(Color.red, Color.yellow, (float)dbStat/100f) : 
//                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-100f)/50f);
//            dbStat = DataContents.ExecuteSQL<float>("SELECT specialDefence FROM Pokemon WHERE rowid=" + pokedexIndex);
//            scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
//            barChild = stats.transform.GetChild(1).FindChild("SpecialDefenceBar");
//            barChild.GetComponent<RectTransform>().localScale = 
//                new Vector3(scale, 1, 1);
//            barChild.GetComponent<Image>().color = dbStat < 95 ?
//                Color.Lerp(Color.red, Color.yellow, (float)dbStat/95f) : 
//                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-95f)/55f);
//            dbStat = DataContents.ExecuteSQL<float>("SELECT speed FROM Pokemon WHERE rowid=" + pokedexIndex);
//            scale = ExtensionMethods.CapAtFloat(dbStat/130f, 1);
//            barChild = stats.transform.GetChild(1).FindChild("SpeedBar");
//            barChild.GetComponent<RectTransform>().localScale = 
//                new Vector3(scale, 1, 1);
//            barChild.GetComponent<Image>().color = dbStat < 95 ?
//                Color.Lerp(Color.red, Color.yellow, (float)dbStat/95f) : 
//                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-95)/35f);
//            
//            //Set types
//            Transform typeRegion = characteristics.transform.GetChild(1).FindChild("Types");
//            SetTypeSprites(typeRegion.GetChild(0).GetComponent<Image>(), typeRegion.GetChild(1).GetComponent<Image>(),
//                pokedexIndex);
//            
//            //Set egg groups
//            characteristics.transform.GetChild(1).FindChild("EggText").GetComponent<Text>().text = 
//                DataContents.ExecuteSQL<string>("SELECT compatibility1 FROM Pokemon WHERE rowid=" + pokedexIndex) +
//                "   " + DataContents.ExecuteSQL<string>("SELECT compatibility2 FROM Pokemon WHERE rowid=" + pokedexIndex);
//            
//            //Set height
//            characteristics.transform.GetChild(1).FindChild("HeightText").GetComponent<Text>().text = 
//                Math.Round(DataContents.ExecuteSQL<float>("SELECT height FROM Pokemon WHERE rowid=" + pokedexIndex), 1).
//                ToString();
//            
//            //Set weight
//            characteristics.transform.GetChild(1).FindChild("WeightText").GetComponent<Text>().text =
//                Math.Round(DataContents.ExecuteSQL<float>("SELECT weight FROM Pokemon WHERE rowid=" +  pokedexIndex), 1).
//                ToString();
//            
//            //Set species
//            characteristics.transform.GetChild(1).FindChild("SpeciesText").GetComponent<Text>().text =
//                DataContents.ExecuteSQL<string>("SELECT kind FROM Pokemon WHERE rowid=" + pokedexIndex) + " Pokemon";
//            
//            //Set level up moves
//            string moveList = DataContents.ExecuteSQL<string>("SELECT moves FROM Pokemon WHERE rowid=" + pokedexIndex);
//            string[] arrayList = moveList.Split(',');
//            Text levelText = movesRegion.transform.FindChild("MoveLevel").GetComponent<Text>();
//            Text moveText = movesRegion.transform.FindChild("MoveName").GetComponent<Text>();
//            levelText.text = "";
//            moveText.text = "";
//            for(int i = 0; i < arrayList.Length-2; i+=2)
//            {
//                levelText.text += arrayList[i] + "\n";
//                moveText.text += arrayList[i+1] + "\n";
//            } //end for
//            levelText.text += arrayList[arrayList.Length-2];
//            moveText.text += arrayList[arrayList.Length-1];
//            
//            //Add egg moves to move list
//            moveList = DataContents.ExecuteSQL<string>("SELECT eggMoves FROM Pokemon WHERE rowid=" + pokedexIndex);
//            arrayList = moveList.Split(',');
//            for(int i = 0; i < arrayList.Length; i++)
//            {
//                levelText.text += "\nEgg";
//                moveText.text += "\n" + arrayList[i];
//            } //end for
//            
//            //Add evolutions
//            string evolveList = DataContents.ExecuteSQL<string>("SELECT evolutions FROM Pokemon WHERE rowid=" + pokedexIndex);
//            arrayList = evolveList.Split(',');
//            Text nameText = evolutionsRegion.transform.FindChild("EvolutionName").GetComponent<Text>();
//            Text methodText = evolutionsRegion.transform.FindChild("EvolutionMethod").GetComponent<Text>();
//            nameText.text = "";
//            methodText.text = "";
//            if(arrayList.Length == 1)
//            {
//                nameText.text = "None";
//            } //end if
//            else
//            {
//                for(int i = 0; i < arrayList.Length; i+=3)
//                {
//                    nameText.text += arrayList[i] + "\n";
//                    methodText.text += string.IsNullOrEmpty(arrayList[i+2]) ? arrayList[i+1] + "\n" :
//                        arrayList[i+2] + "\n";
//                } //end for
//            } //end else
//            
//            //Set pokedex text
//            pokedexText.text = DataContents.ExecuteSQL<string>("SELECT pokedex FROM Pokemon WHERE rowid=" + pokedexIndex);
//            
//            //Set abilities text
//            abilitiesText.text = DataContents.ExecuteSQL<string>("SELECT ability1 FROM Pokemon WHERE rowid=" + pokedexIndex);
//            string ability2 = DataContents.ExecuteSQL<string>("SELECT ability2 FROM Pokemon WHERE rowid=" + pokedexIndex);
//            string hiddenAbility = DataContents.ExecuteSQL<string>("SELECT hiddenAbility FROM Pokemon WHERE rowid=" + pokedexIndex);
//            abilitiesText.text += String.IsNullOrEmpty(ability2) ? String.IsNullOrEmpty(hiddenAbility) ? "" : 
//                "," + hiddenAbility : String.IsNullOrEmpty(hiddenAbility) ? "," + ability2 : 
//                "," + ability2 + "," + hiddenAbility;
//            
//            //Set weakness and resistances
//            SetWeakResistSprites();
//
//            //End processing
//            processing = false;
//        } //end else if
//    } //end Pokedex
//	#endregion
//
//	#region Animations
//
//
//    
//	#endregion
//
//    #region Processing

//
//    /***************************************
//     * Name: SetStatColor
//     * Sets the color for stat ups and downs
//     ***************************************/
//    void SetStatColor(Pokemon teamMember)
//    {
//        /*Attack, Defence, Speed, SP Attack, SP Defence*/
//        //Get the pokemon's nature
//        int currentNature = teamMember.Nature;
//
//        //Find stat up
//        int nd5 = (int)Mathf.Floor (currentNature / 5);
//
//        //Find stat down
//        int nm5 = (int)Mathf.Floor (currentNature % 5);
//
//        //Get child number of attack
//        int childNumber = summaryScreen.transform.GetChild (2).FindChild ("Attack").GetSiblingIndex ();
//
//        //Set stat colors
//        for (int i = 0; i < 5; i++)
//        {
//            //If stat up
//            if(i == nd5 && nd5 != nm5)
//            {
//                summaryScreen.transform.GetChild(2).GetChild(i+childNumber).GetComponent<Text>().color = new Color(0.75f, 0, 0, 1);
//            } //end if
//            //If stat down
//            else if(i == nm5 && nd5 != nm5)
//            {
//                summaryScreen.transform.GetChild(2).GetChild(i+childNumber).GetComponent<Text>().color = new Color(0, 0, 0.75f, 1);
//            } //end else if
//            //Otherwise black
//            else
//            {
//                summaryScreen.transform.GetChild(2).GetChild(i+childNumber).GetComponent<Text>().color = Color.black;
//            } //end else
//        } //end for
//    } //end SetStatColor(Pokemon teamMember)
//
//    /***************************************
//     * Name: SetMoveSprites
//     * Sets the correct sprite, or disables
//     * if a move isn't found.
//     ***************************************/
//    void SetMoveSprites(Pokemon teamMember, Transform moveScreen)
//    {
//        //Loop through the list of pokemon moves and set each one
//        for (int i = 0; i < 4; i++)
//        {
//            //Make sure move isn't null
//            if(teamMember.GetMove(i) != -1)
//            {
//                //Set the move type
//                moveScreen.FindChild("Move" + (i+1).ToString()).gameObject.SetActive(true);
//                moveScreen.FindChild("Move" + (i+1).ToString()).GetChild(0).GetComponent<Image>().sprite = 
//                    DataContents.typeSprites [Convert.ToInt32 (Enum.Parse (typeof(Types),
//                    DataContents.ExecuteSQL<string> ("SELECT type FROM Moves WHERE rowid=" +
//                    teamMember.GetMove(i))))];
//                //Set the move name
//                moveScreen.FindChild("Move" + (i+1).ToString()).GetChild(1).GetComponent<Text>().text = 
//                    DataContents.ExecuteSQL<string> ("SELECT gameName FROM Moves WHERE rowid=" +
//                    teamMember.GetMove(i));
//
//                //Set the move PP
//                moveScreen.FindChild("Move" + (i+1).ToString()).GetChild(2).GetComponent<Text>().text = "PP " +
//                    teamMember.GetMovePP(i).ToString() + "/" +
//                    DataContents.ExecuteSQL<string> ("SELECT totalPP FROM Moves WHERE rowid=" +
//                    teamMember.GetMove(i));
//            } //end if
//            else
//            {
//                //Blank out the type
//                moveScreen.FindChild("Move" + (i+1).ToString()).gameObject.SetActive(false);
//            } //end else
//        } //end for
//    } //end SetMoveSprites(Pokemon teamMember, Transform moveScreen)
//
//    /***************************************
//     * Name: SetMoveDetails
//     * Sets the details of the move
//     ***************************************/
//    void SetMoveDetails(Pokemon teamMember, Transform moveScreen)
//    {
//        //Reposition selection rect
//        selection.SetActive(true);
//        
//        //Resize to same as top choice
//        Vector3 scale = new Vector3(moveScreen.FindChild("Move"+(moveChoice+1)).GetComponent<RectTransform>().rect.width,
//                                    moveScreen.FindChild("Move"+(moveChoice+1)).GetComponent<RectTransform>().rect.height,
//                                    0);
//        selection.GetComponent<RectTransform>().sizeDelta = scale;
//
//        //Reposition to location of top choice, with 2 unit offset to center it
//        selection.transform.position = Camera.main.WorldToScreenPoint (currentMoveSlot.transform.
//                                                                       position);
//
//        //Set the move category
//        moveScreen.FindChild("Category").GetComponent<Image>().sprite = 
//            DataContents.categorySprites [Convert.ToInt32 (Enum.Parse (typeof(Categories),
//            DataContents.ExecuteSQL<string> ("SELECT category FROM Moves WHERE rowid=" +
//            teamMember.GetMove(moveChoice))))];
//
//        //Set the move power
//        int temp = DataContents.ExecuteSQL<int> ("SELECT baseDamage FROM Moves WHERE rowid=" +
//                   teamMember.GetMove(moveChoice));
//        moveScreen.FindChild ("Power").GetComponent<Text> ().text = temp > 1 ? temp.ToString () : "---";
//            
//        //Set the move accuracy
//        temp = DataContents.ExecuteSQL<int> ("SELECT accuracy FROM Moves WHERE rowid=" +
//               teamMember.GetMove(moveChoice));
//        moveScreen.FindChild("Accuracy").GetComponent<Text>().text = temp >= 1 ? temp.ToString () : "---";
//
//        //Set font size of move description
//        if (detailsSize != -1)
//        {
//            //Set the move description text
//            moveScreen.FindChild ("MoveDescription").GetComponent<Text> ().text = 
//                DataContents.ExecuteSQL<string> ("SELECT description FROM Moves WHERE rowid=" +
//                teamMember.GetMove (moveChoice));
//        } //end if
//        else
//        {
//            //Get font size
//            moveScreen.FindChild ("MoveDescription").GetComponent<Text> ().text = 
//                DataContents.ExecuteSQL<string> ("SELECT description FROM Moves WHERE gameName='Stealth Rock'");
//            StartCoroutine (WaitForFontResize (moveScreen, teamMember));
//        } //end else
//    } //end SetMoveDetails(Pokemon teamMember, Transform moveScreen)
//
//
//    /***************************************
//     * Name: ReadRibbon
//     * Sets ribbon details based on ribbon selection
//     ***************************************/
//    public void ReadRibbon()
//    {
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
//    } //end ReadRibbon
//
//    /***************************************
//     * Name: FillDetails
//     * Fills or clears the details region
//     * based on selected pokemon
//     ***************************************/
//    void FillDetails()
//    {
//        //If it's not null, populate
//        if (selectedPokemon != null)
//        {
//            detailsRegion.transform.FindChild ("Name").GetComponent<Text> ().text = selectedPokemon.Nickname;
//            detailsRegion.transform.FindChild ("Gender").GetComponent<Image> ().color = Color.white;
//            detailsRegion.transform.FindChild ("Gender").GetComponent<Image> ().sprite = 
//                Resources.Load<Sprite> ("Sprites/Icons/gender" + selectedPokemon.Gender);
//            detailsRegion.transform.FindChild ("Sprite").GetComponent<Image> ().color = Color.white;
//			detailsRegion.transform.FindChild("Sprite").GetComponent<Image>().sprite = 
//				GetCorrectSprite(selectedPokemon);
//            detailsRegion.transform.FindChild ("Markings").GetComponent<Text> ().text = selectedPokemon.
//                GetMarkingsString();
//			detailsRegion.transform.FindChild  ("Shiny").gameObject.SetActive(selectedPokemon.IsShiny);
//            detailsRegion.transform.FindChild ("Ability").GetComponent<Text> ().text = selectedPokemon.
//                GetAbilityName ();
//            detailsRegion.transform.FindChild ("Item").GetComponent<Text> ().text = 
//                DataContents.GetItemGameName (selectedPokemon.Item);
//            detailsRegion.transform.FindChild ("Level").GetComponent<Text> ().text = "Lv. " + 
//                selectedPokemon.CurrentLevel.ToString ();
//            SetTypeSprites (detailsRegion.transform.FindChild ("Types").GetChild (0).GetComponent<Image> (),
//                detailsRegion.transform.FindChild ("Types").GetChild (1).GetComponent<Image> (),
//                selectedPokemon.NatSpecies);
//        } //end if
//        else
//        {
//            detailsRegion.transform.FindChild ("Name").GetComponent<Text> ().text = "";
//            detailsRegion.transform.FindChild ("Gender").GetComponent<Image> ().color = Color.clear;
//            detailsRegion.transform.FindChild ("Sprite").GetComponent<Image> ().color = Color.clear;
//            detailsRegion.transform.FindChild ("Markings").GetComponent<Text> ().text = "";
//			detailsRegion.transform.FindChild ("Shiny").gameObject.SetActive(false);
//            detailsRegion.transform.FindChild ("Level").GetComponent<Text> ().text = "";
//            detailsRegion.transform.FindChild ("Types").GetChild (0).gameObject.SetActive (false);
//            detailsRegion.transform.FindChild ("Types").GetChild (1).gameObject.SetActive (false);
//            detailsRegion.transform.FindChild ("Ability").GetComponent<Text> ().text = "";
//            detailsRegion.transform.FindChild ("Item").GetComponent<Text> ().text = "";
//        } //end else
//    } //end FillDetails
//
//    /***************************************
//     * Name: FillInIndex
//     * Sets the content for the left portion
//     * of the pokedex
//     ***************************************/
//    void FillInIndex()
//    {
//        //Make sure bottom of pokedex is blocked
//        if (pokedexIndex < 713)
//        {
//            //Fill in each slot
//            for (int i = 0; i < 10; i++)
//            {
//                Transform child = index.transform.GetChild (i);
//                int chosenPoke = pokedexIndex + i;
//                child.FindChild ("Name").GetComponent<Text> ().text = chosenPoke.ToString ("000") + ":" +
//                    DataContents.ExecuteSQL<string> ("SELECT name FROM Pokemon WHERE rowid=" + chosenPoke);
//                child.FindChild ("Ball").GetComponent<Image> ().sprite = 
//                    GameManager.instance.GetTrainer ().Owned.Contains (chosenPoke) ? 
//                Resources.Load<Sprite> ("Sprites/Icons/ballnormal") :
//                Resources.Load<Sprite> ("Sprites/Icons/ballfainted");
//				child.FindChild("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Icons/icon" + 
//					chosenPoke.ToString("000"));
//            } //end for
//        } //end if
//    } //end FillInIndex
//
//    /***************************************
//     * Name: SetWeakResistSprites
//     * Activates appropriate sprites for 
//     * weakness/resistances
//     ***************************************/
//    void SetWeakResistSprites()
//    {
//        //Set all to inactive
//        for(int i = 0; i < weakTypes.transform.childCount; i++)
//        {
//            weakTypes.transform.GetChild(i).gameObject.SetActive(false);
//            resistTypes.transform.GetChild(i).gameObject.SetActive(false);
//        } //end for
//
//        //Get types
//        int type1 = Convert.ToInt32 (Enum.Parse (typeof(Types), DataContents.ExecuteSQL<string> 
//            ("SELECT type1 FROM Pokemon WHERE rowid=" + pokedexIndex)));
//        int type2 = -1;
//        try
//        {
//            type2 = Convert.ToInt32(Enum.Parse (typeof(Types), DataContents.ExecuteSQL<string> 
//                ("SELECT type2 FROM Pokemon WHERE rowid=" + pokedexIndex)));
//        } //end try
//        catch(SystemException e){}//end catch
//
//        //Get weaknesses
//        List<int> weakList = DataContents.typeChart.DetermineWeaknesses (type1, type2);
//
//        //Fill in weaknesses
//        for (int i = 0; i < weakList.Count; i++)
//        {
//            weakTypes.transform.GetChild (weakList [i]).gameObject.SetActive (true);
//        } //end for
//
//        //Get resistances
//        List<int> resistList = DataContents.typeChart.DetermineResistances (type1, type2);
//
//        //Fill in resistances
//        for (int i = 0; i < resistList.Count; i++)
//        {
//            resistTypes.transform.GetChild (resistList [i]).gameObject.SetActive (true);
//        } //end for
//    } //end SetWeakResistSprites
//
//
//	/***************************************
//     * Name: GetCorrectSprite
//     * Returns the sprite for the pokemon,
//     * based on species, gender, shiny, and
//     * form
//     ***************************************/
//	Sprite GetCorrectSprite(Pokemon myPokemon)
//	{
//		//Get requested sprite
//		string chosenString = myPokemon.NatSpecies.ToString("000");
//		chosenString += myPokemon.Gender == 1 ? "f" : "";
//		chosenString += myPokemon.IsShiny ? "s" : "";
//		chosenString += myPokemon.FormNumber > 0 ? "_" + myPokemon.FormNumber.ToString() : "";
//
//		//Change sprite, and fix if sprite is null
//		Sprite result = Resources.Load<Sprite>("Sprites/Pokemon/" + chosenString);
//		if(result == null)
//		{
//			chosenString = chosenString.Replace("f", "");
//			result = Resources.Load<Sprite>("Sprites/Pokemon/" + chosenString);
//			if(result == null)
//			{
//				result = Resources.Load<Sprite>("Sprites/Pokemon/0");
//			} //end if
//		} //end if
//
//		return result;
//	} //end GetCorrectSprite(Pokemon myPokemon)
//

//
//    /***************************************
//     * Name: WaitForResize
//     * Waits for choice menu to resize before 
//     * setting selection dimensions
//     ***************************************/
//    IEnumerator WaitForResize()
//    {
//        yield return new WaitForEndOfFrame ();
//        if (gameState == MainGame.POKEMONSUBMENU || pcState == PCGame.POKEMONSUBMENU)
//        {
//            Vector3 scale = new Vector3 (choices.GetComponent<RectTransform> ().rect.width,
//                choices.GetComponent<RectTransform> ().rect.height /
//                choices.transform.childCount, 0);
//            selection.GetComponent<RectTransform> ().sizeDelta = scale;
//            selection.transform.position = choices.transform.GetChild (0).
//                GetComponent<RectTransform> ().position;
//            selection.SetActive (true);
//        } //end if
//        else if (pcState == PCGame.POKEMONMARKINGS)
//        {
//            //Reposition choices to bottom right
//            choices.GetComponent<RectTransform>().position = new Vector3(
//                choices.GetComponent<RectTransform>().position.x,
//                choices.GetComponent<RectTransform>().rect.height/2);
//
//            Vector3 scale = new Vector3 (choices.GetComponent<RectTransform> ().rect.width,
//                choices.GetComponent<RectTransform> ().rect.height /
//                choices.transform.childCount, 0);
//            selection.GetComponent<RectTransform> ().sizeDelta = scale;
//            selection.transform.position = choices.transform.GetChild (0).
//                GetComponent<RectTransform> ().position;
//            selection.SetActive (true);
//        } //end else if
//        else if (gameState == MainGame.POKEMONRIBBONS || pcState == PCGame.POKEMONRIBBONS)
//        {
//            Vector3 scale = new Vector3(ribbonScreen.transform.FindChild("RibbonRegion").GetChild(0).
//                                        GetComponent<RectTransform>().rect.width,
//                                        ribbonScreen.transform.FindChild("RibbonRegion").GetChild(0).
//                                        GetComponent<RectTransform>().rect.height
//                                        , 0);
//            selection.GetComponent<RectTransform>().sizeDelta = scale;
//            selection.transform.position = Camera.main.WorldToScreenPoint(ribbonScreen.transform.
//                FindChild("RibbonRegion").GetChild(0).GetComponent<RectTransform>().position);
//        } //end else if
//    } //end WaitForResize
//
//    /***************************************
//     * Name: WaitForFontResize
//     * Waits for move description font to
//     * resize to best fit
//     ***************************************/
//    IEnumerator WaitForFontResize(Transform moveScreen, Pokemon teamMember)
//    {
//        yield return new WaitForEndOfFrame ();
//        detailsSize = moveScreen.FindChild("MoveDescription").GetComponent<Text>().cachedTextGenerator.
//            fontSizeUsedForBestFit;
//        moveScreen.FindChild("MoveDescription").GetComponent<Text>().resizeTextForBestFit = false;
//        moveScreen.FindChild("MoveDescription").GetComponent<Text>().fontSize = detailsSize;
//        moveScreen.FindChild ("MoveDescription").GetComponent<Text> ().text = 
//            DataContents.ExecuteSQL<string> ("SELECT description FROM Moves WHERE rowid=" +
//            teamMember.GetMove (moveChoice));
//    } //end WaitForFontResize(Transform moveScreen, Pokemon teamMember)    
//    #endregion
//
//	//Miscellaneous functions
//	#region Misc
//    /***************************************
//     * Name: Reset
//     * Soft resets the game to intro screen
//     ***************************************/
//	public void Reset()
//    {
//		StopAllCoroutines ();
//        StartCoroutine (LoadScene ("Intro", OverallGame.INTRO));
//	} //end Reset
//
//    /***************************************
//     * Name: SetCheckpoint
//     * Sets checkpoint to parameter
//     ***************************************/
//    public void SetCheckpoint(int newCheckpoint)
//    {
//        checkpoint = newCheckpoint; 
//    } //end SetCheckpoint(int newCheckpoint)
//
//    /***************************************
//     * Name: SetGameState
//     * Sets gameState to parameter
//     ***************************************/
//    public IEnumerator SetGameState(MainGame newGameState)
//    {
//        //Process at end of frame
//        yield return new WaitForEndOfFrame ();
//
//        //Set the new state
//        EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
//        gameState = newGameState; 
//    } //end SetGameState(MainGame newGameState)
//
//    /***************************************
//     * Name: SetSummaryPage
//     * Sets summaryChoice to parameter
//     ***************************************/
//    public IEnumerator SetSummaryPage(int summaryPage)
//    {
//        //Process at end of frame
//        yield return new WaitForEndOfFrame ();
//
//        //Change screen only if summary screen is active
//        if (summaryScreen.activeSelf)
//        {
//            //If move switch is active
//            if(gameState == MainGame.MOVESWITCH)
//            {
//                //Return to summary
//                currentMoveSlot.GetComponent<Image>().color = Color.clear;
//                summaryScreen.transform.GetChild(5).gameObject.SetActive(false);
//                summaryScreen.transform.GetChild(4).gameObject.SetActive(false);
//                selection.SetActive(false);
//                
//                //Change to new page
//                summaryChoice = summaryPage;
//                gameState = MainGame.POKEMONSUMMARY;
//            } //end if
//            else if(summaryChoice == 5)
//            {
//                summaryScreen.transform.GetChild(5).gameObject.SetActive(false);
//                summaryScreen.transform.GetChild(4).gameObject.SetActive(false);
//                selection.SetActive(false);
//                summaryChoice = summaryPage;
//            } //end else if
//            else
//            {
//                //Deactivate current page
//                summaryScreen.transform.GetChild(summaryChoice).gameObject.SetActive(false);
//
//                //Change to new page
//                summaryChoice = summaryPage;
//            } //end else
//        } //end if
//    } //end SetSummaryPage(int summaryPage)
//

//
//    /***************************************
//     * Name: PartyState
//     * Opens/Closes the Party in PC box
//     ***************************************/ 
//    public IEnumerator PartyState(bool state)
//    {
//        //Process at end of frame
//        yield return new WaitForEndOfFrame ();
//
//        //Party to be opened
//        if (state)
//        {
//            partyTab.SetActive(true);
//            choiceNumber = 1;
//            currentTeamSlot = partyTab.transform.FindChild("Pokemon1").gameObject;
//            pcState = PCGame.PARTY;
//        } //end if
//        //Party to be closed
//        else
//        {
//            partyTab.SetActive(false);
//            pcState = PCGame.HOME;
//        } //end else
//    } //end PartyState(bool state)
//
//    /***************************************
//     * Name: ToggleShown
//     * Toggles Weakness/Resistance in Pokedex
//     ***************************************/ 
//    public void ToggleShown()
//    {
//        //If weakeness is shown, show resistances
//        if (weakTypes.activeSelf)
//        {
//            weakTypes.SetActive (false);
//            resistTypes.SetActive (true);
//            shownButton.GetComponent<Text>().text = "Resists";
//        } //end if
//        else
//        {
//            weakTypes.SetActive(true);
//            resistTypes.SetActive(false);
//            shownButton.GetComponent<Text>().text = "Weakness";
//        } //end else
//    } //end ToggleShown
//	#endregion
//
//	#region Debug
//    /***************************************
//     * Name: EditPokemonMode
//     * Activates the pokemon edit panel
//     ***************************************/ 
//    public void EditPokemonMode()
//    {
//        debugButtons.SetActive (false);
//        debugOptions.transform.GetChild (1).gameObject.SetActive (true);
//        debugOptions.transform.GetChild (2).gameObject.SetActive (false);
//    } //end EditPokemonMode
//
//    /***************************************
//     * Name: EditTrainerMode
//     * Activates the trainer edit panel
//     ***************************************/ 
//    public void EditTrainerMode()
//    {
//        debugButtons.SetActive (false);
//        debugOptions.transform.GetChild (1).gameObject.SetActive (false);
//        debugOptions.transform.GetChild (2).gameObject.SetActive (true);
//    } //end EditTrainerMode
//
//	/***************************************
//     * Name: RandomPokemon
//     * Adds a single random pokemon to the team
//     ***************************************/ 
//	public void RandomPokemon()
//	{		
//		//Generate random pokemon
//		Pokemon randomPoke = new Pokemon();
//
//		//Update relevant portions
//		UpdateDebug(randomPoke);
//	} //end RandomPokemon
//
//	/***************************************
//     * Name: EditPokemon
//     * Grabs a pokemon out of team or pc
//     * and populates debug with it
//     ***************************************/ 
//	public void EditPokemon()
//	{
//		//Get requested pokemon
//		if (GameObject.Find ("LeftRegion").transform.FindChild ("TeamToggle").GetComponent<Toggle> ().isOn)
//		{
//			int outputHolder = 0;
//			string slot= GameObject.Find ("LeftRegion").transform.FindChild ("Slot").GetComponent<InputField>().text;
//			if(int.TryParse (slot, out outputHolder))
//			{
//				int bound = ExtensionMethods.WithinIntRange (outputHolder, 0, GameManager.instance.GetTrainer ().Team.Count-1);
//				UpdateDebug (GameManager.instance.GetTrainer ().Team [bound]);
//			} //end if
//			else
//			{
//				UpdateDebug (GameManager.instance.GetTrainer ().Team [0]);
//			} //end else
//		} //end if
//		else
//		{
//			int slotSpace = 0;
//			int boxSpace = 0;
//			string slot = GameObject.Find ("LeftRegion").transform.FindChild ("Slot").GetComponent<InputField>().text;
//			string box = GameObject.Find ("LeftRegion").transform.FindChild ("Box").GetComponent<InputField>().text;
//			if (!int.TryParse (slot, out slotSpace)) 
//			{
//				slotSpace = 0;
//			} //end if
//			if (!int.TryParse (box, out boxSpace)) 
//			{
//				boxSpace = GameManager.instance.GetTrainer ().GetPCBox ();
//			} //end if
//            Pokemon toUse = GameManager.instance.GetTrainer().GetPC(boxSpace, slotSpace);
//            if (toUse == null)
//            {
//                toUse = GameManager.instance.GetTrainer ().GetFirstPokemon ();
//            } //end if
//            if (toUse != null)
//            {
//                UpdateDebug (toUse);
//            } //end if
//            else
//            {
//                GameManager.instance.DisplayText ("No pokemon found in current box. Edit Pokemon canceled.", true);
//            } //end else
//		} //end else
//	} //end EditPokemon
//
//	/***************************************
//     * Name: RemovePokemon
//     * Remove a pokemon from PC or team
//     ***************************************/ 
//	public void RemovePokemon()
//	{		
//		//Get requested pokemon
//		if (GameObject.Find ("LeftRegion").transform.FindChild ("TeamToggle").GetComponent<Toggle> ().isOn)
//		{
//			int outputHolder = 0;
//			string slot= GameObject.Find ("LeftRegion").transform.FindChild ("Slot").GetComponent<InputField>().text;
//            if (int.TryParse (slot, out outputHolder))
//            {
//                GameManager.instance.GetTrainer ().RemovePokemon (outputHolder);
//            } //end if
//            else
//            {
//                GameManager.instance.DisplayText ("No pokemon found on team at reuqested slot. Remove Pokemon canceled.", true);
//            } //end else
//		} //end if
//		else
//		{
//			int slotSpace = 0;
//			int boxSpace = 0;
//			string slot = GameObject.Find ("LeftRegion").transform.FindChild ("Slot").GetComponent<InputField>().text;
//			string box = GameObject.Find ("LeftRegion").transform.FindChild ("Box").GetComponent<InputField>().text;
//            if (int.TryParse (slot, out slotSpace))
//            {
//                if (int.TryParse (box, out boxSpace))
//                {
//                    GameManager.instance.GetTrainer ().RemoveFromPC (boxSpace, slotSpace);
//                } //end if	
//                else
//                {
//                    GameManager.instance.DisplayText ("Invalid Box given. Remove Pokemon canceled.", true);
//                } //end else
//            } //end if
//            else
//            {
//                GameManager.instance.DisplayText ("Invalid Slot given. Remove Pokemon canceled.", true);
//            } //end else
//		} //end else
//	} //end RemovePokemon
//
//    /***************************************
//     * Name: FinishEditing
//     * Apply pokemon to requested spot
//     ***************************************/ 
//    public void FinishEditing()
//    {
//        if (debugOptions.transform.GetChild (2).gameObject.activeSelf)
//        {
//			GameManager.instance.GetTrainer().PlayerImage = trainerRightRegion.transform.FindChild("TrainerSprite").
//				GetComponent<Dropdown>().value;
//            for(int i = 0; i < 8; i++)
//            {
//                GameManager.instance.GetTrainer ().SetPlayerBadges (39 + i, trainerRightRegion.transform.FindChild ("Badges").
//                    GetChild (i).GetComponent<Toggle> ().isOn);
//            } //end for
//            GameManager.instance.DisplayText("Updated trainer.", true);
//        } //end if
//        else
//        {
//            Pokemon newPokemon = new Pokemon (
//                pokemonRightRegion.transform.FindChild ("PokemonName").GetComponent<Dropdown> ().value + 1,
//                int.Parse (pokemonRightRegion.transform.FindChild ("TrainerID").GetComponent<InputField> ().text),
//                ExtensionMethods.WithinIntRange (
//                    int.Parse(pokemonRightRegion.transform.FindChild ("Level").GetComponent<InputField> ().text), 1, 100),
//                pokemonRightRegion.transform.FindChild ("Item").GetComponent<Dropdown> ().value,
//                pokemonRightRegion.transform.FindChild ("Ball").GetComponent<Dropdown> ().value,
//                5, 3,
//                pokemonRightRegion.transform.FindChild ("Ability").GetComponent<Dropdown> ().value + 1,
//                pokemonRightRegion.transform.FindChild ("Gender").GetComponent<Dropdown> ().value, 
//				int.Parse (pokemonRightRegion.transform.FindChild ("Form").GetComponent<InputField>().text),
//                pokemonRightRegion.transform.FindChild ("Nature").GetComponent<Dropdown> ().value,
//                int.Parse (pokemonRightRegion.transform.FindChild ("Happiness").GetComponent<InputField> ().text),
//                pokemonRightRegion.transform.FindChild ("Pokerus").GetComponent<Toggle> ().isOn,
//                pokemonRightRegion.transform.FindChild ("Shiny").GetComponent<Toggle> ().isOn);
//
//            //Update nickname
//            newPokemon.Nickname = pokemonRightRegion.transform.FindChild("Nickname").GetComponent<InputField>().text;
//
//            //Update IV
//            int[] ivFields = new int[6];
//            for (int i = 0; i < 6; i++)
//            {
//                ivFields [i] = int.Parse (pokemonRightRegion.transform.FindChild ("IV").GetChild (i).GetComponent<InputField> ().text);
//            } //end for
//            newPokemon.ChangeIVs(ivFields);
//
//            //Update EV
//            int[] evFields = new int[6];
//            for (int i = 0; i < 6; i++)
//            {
//                evFields [i] = int.Parse (pokemonRightRegion.transform.FindChild ("EV").GetChild (i).GetComponent<InputField> ().text);
//            } //end for
//            newPokemon.ChangeEVs(evFields);
//            newPokemon.CalculateStats ();
//
//            //Update Ribbons
//            for (int i = 0; i < pokemonRightRegion.transform.FindChild ("Ribbons").GetChild(0).childCount; i++)
//            {
//                if (pokemonRightRegion.transform.FindChild ("Ribbons").GetChild (0).GetChild (i).
//                    GetComponent<Toggle> ().isOn)
//                {
//                    newPokemon.ChangeRibbons (i);
//                } //end if
//            } //end for
//
//            //Update Moves
//            int[] updateMoves = new int[4];
//            for (int i = 0; i < 4; i++)
//            {
//                int value = pokemonRightRegion.transform.FindChild ("Moves").GetChild (i).GetComponent<Dropdown> ().value;
//                updateMoves [i] = value > 0 ? value : -1;                
//            } //end for
//            newPokemon.GiveInitialMoves(updateMoves);
//
//            //Add to requested spot, or PC if not chosen
//            if (GameObject.Find ("LeftRegion").transform.FindChild ("TeamToggle").GetComponent<Toggle> ().isOn)
//            {
//                GameManager.instance.GetTrainer ().AddPokemon (newPokemon);
//                GameManager.instance.DisplayText ("Added " + newPokemon.Nickname + " to your team", true);
//            } //end if
//            else
//            {
//                GameManager.instance.GetTrainer ().AddToPC (GameManager.instance.GetTrainer ().GetPCBox (), 0, newPokemon);
//                GameManager.instance.DisplayText ("Added " + newPokemon.Nickname + " to your PC", true);
//            } //end else
//        } //end else
//    } //end FinishEditing
//
//	/***************************************
//     * Name: UpdateSprite
//     * Changes sprite to reflect name choice
//     ***************************************/ 
//	public void UpdateSprite()
//	{
//		//Develop string path to image
//		string chosenString = (pokemonRightRegion.transform.FindChild("PokemonName").GetComponent<Dropdown>().value + 1).
//			ToString ("000");
//		chosenString +=  pokemonRightRegion.transform.FindChild("Gender").GetComponent<Dropdown>().value == 1 ? "f" : "";
//		chosenString += pokemonRightRegion.transform.FindChild("Shiny").GetComponent<Toggle>().isOn ? "s" : "";
//
//		//Change sprite, and fix if sprite is null
//		pokemonRightRegion.transform.FindChild("Sprite").GetComponent<Image>().sprite = 
//			Resources.Load<Sprite>("Sprites/Pokemon/" + chosenString);   
//		if( pokemonRightRegion.transform.FindChild("Sprite").GetComponent<Image>().sprite == null)
//		{
//			chosenString = chosenString.Replace("f", "");
//			pokemonRightRegion.transform.FindChild("Sprite").GetComponent<Image>().sprite = 
//				Resources.Load<Sprite>("Sprites/Pokemon/" + chosenString);
//			if( pokemonRightRegion.transform.FindChild("Sprite").GetComponent<Image>().sprite == null)
//			{
//				pokemonRightRegion.transform.FindChild("Sprite").GetComponent<Image>().sprite = 
//					Resources.Load<Sprite>("Sprites/Pokemon/0");
//			} //end if
//		} //end if
//	} //end UpdateSprite
//
//	/***************************************
//     * Name: UpdateDebug
//     * Changes fields to represent pokemon
//     ***************************************/ 
//    void UpdateDebug(Pokemon myPokemon)
//	{		
//        if (debugOptions.transform.GetChild (2).gameObject.activeSelf)
//        {
//			trainerRightRegion.transform.FindChild("TrainerSprite").GetComponent<Dropdown>().value =
//				GameManager.instance.GetTrainer().PlayerImage;
//            for(int i = 0; i < 8; i++)
//            {
//                trainerRightRegion.transform.FindChild ("Badges").GetChild (i).GetComponent<Toggle> ().isOn = 
//                    GameManager.instance.GetTrainer ().GetPlayerBadges (39 + i);
//            } //end for
//        } //end if
//        else
//        {
//    		pokemonRightRegion.transform.FindChild ("Gender").GetComponent<Dropdown>().value = myPokemon.Gender;
//    		pokemonRightRegion.transform.FindChild ("Gender").GetComponent<Dropdown> ().RefreshShownValue ();
//    		pokemonRightRegion.transform.FindChild ("PokemonName").GetComponent<Dropdown> ().value = myPokemon.NatSpecies - 1;
//    		pokemonRightRegion.transform.FindChild ("PokemonName").GetComponent<Dropdown> ().RefreshShownValue ();
//    		pokemonRightRegion.transform.FindChild ("Level").GetComponent<InputField>().text = myPokemon.CurrentLevel.ToString();
//    		pokemonRightRegion.transform.FindChild ("TrainerID").GetComponent<InputField>().text = myPokemon.TrainerID.ToString();
//            pokemonRightRegion.transform.FindChild ("Nickname").GetComponent<InputField> ().text = myPokemon.Nickname;
//    		pokemonRightRegion.transform.FindChild ("Nature").GetComponent<Dropdown> ().value = myPokemon.Nature;
//    		pokemonRightRegion.transform.FindChild ("Nature").GetComponent<Dropdown> ().RefreshShownValue ();
//    		pokemonRightRegion.transform.FindChild ("Item").GetComponent<Dropdown> ().value = myPokemon.Item-1;
//    		pokemonRightRegion.transform.FindChild ("Item").GetComponent<Dropdown> ().RefreshShownValue ();
//    		pokemonRightRegion.transform.FindChild ("Ball").GetComponent<Dropdown> ().value = myPokemon.BallUsed;
//    		pokemonRightRegion.transform.FindChild ("Ball").GetComponent<Dropdown> ().RefreshShownValue ();
//    		pokemonRightRegion.transform.FindChild ("Ability").GetComponent<Dropdown> ().value = myPokemon.Ability-1;
//    		pokemonRightRegion.transform.FindChild ("Ability").GetComponent<Dropdown> ().RefreshShownValue ();
//    		pokemonRightRegion.transform.FindChild ("Happiness").GetComponent<InputField>().text = myPokemon.Happiness.ToString();
//			pokemonRightRegion.transform.FindChild ("Form").GetComponent<InputField>().text = myPokemon.FormNumber.ToString();
//    		pokemonRightRegion.transform.FindChild ("Shiny").GetComponent<Toggle> ().isOn = myPokemon.IsShiny;
//    		pokemonRightRegion.transform.FindChild ("Pokerus").GetComponent<Toggle> ().isOn = myPokemon.HasPokerus;
//    		pokemonRightRegion.transform.FindChild ("IV").GetChild (0).GetComponent<InputField> ().text = myPokemon.GetIV (0).ToString();
//    		pokemonRightRegion.transform.FindChild ("IV").GetChild (1).GetComponent<InputField> ().text = myPokemon.GetIV (1).ToString();
//    		pokemonRightRegion.transform.FindChild ("IV").GetChild (2).GetComponent<InputField> ().text = myPokemon.GetIV (2).ToString();
//    		pokemonRightRegion.transform.FindChild ("IV").GetChild (3).GetComponent<InputField> ().text = myPokemon.GetIV (3).ToString();
//    		pokemonRightRegion.transform.FindChild ("IV").GetChild (4).GetComponent<InputField> ().text = myPokemon.GetIV (4).ToString();
//    		pokemonRightRegion.transform.FindChild ("IV").GetChild (5).GetComponent<InputField> ().text = myPokemon.GetIV (5).ToString();
//    		pokemonRightRegion.transform.FindChild ("EV").GetChild (0).GetComponent<InputField> ().text = myPokemon.GetEV (0).ToString();
//    		pokemonRightRegion.transform.FindChild ("EV").GetChild (1).GetComponent<InputField> ().text = myPokemon.GetEV (1).ToString();
//    		pokemonRightRegion.transform.FindChild ("EV").GetChild (2).GetComponent<InputField> ().text = myPokemon.GetEV (2).ToString();
//    		pokemonRightRegion.transform.FindChild ("EV").GetChild (3).GetComponent<InputField> ().text = myPokemon.GetEV (3).ToString();
//    		pokemonRightRegion.transform.FindChild ("EV").GetChild (4).GetComponent<InputField> ().text = myPokemon.GetEV (4).ToString();
//    		pokemonRightRegion.transform.FindChild ("EV").GetChild (5).GetComponent<InputField> ().text = myPokemon.GetEV (5).ToString();
//    		pokemonRightRegion.transform.FindChild ("Moves").GetChild (0).GetComponent<Dropdown> ().value = 
//    			ExtensionMethods.BindToInt(myPokemon.GetMove (0), 0);
//    		pokemonRightRegion.transform.FindChild ("Moves").GetChild (0).GetComponent<Dropdown> ().RefreshShownValue ();
//    		pokemonRightRegion.transform.FindChild ("Moves").GetChild (1).GetComponent<Dropdown> ().value = 
//    			ExtensionMethods.BindToInt(myPokemon.GetMove (1), 0);
//    		pokemonRightRegion.transform.FindChild ("Moves").GetChild (1).GetComponent<Dropdown> ().RefreshShownValue ();
//    		pokemonRightRegion.transform.FindChild ("Moves").GetChild (2).GetComponent<Dropdown> ().value = 
//    			ExtensionMethods.BindToInt(myPokemon.GetMove (2), 0);
//    		pokemonRightRegion.transform.FindChild ("Moves").GetChild (2).GetComponent<Dropdown> ().RefreshShownValue ();
//    		pokemonRightRegion.transform.FindChild ("Moves").GetChild (3).GetComponent<Dropdown> ().value = 
//    			ExtensionMethods.BindToInt(myPokemon.GetMove (3), 0);
//            pokemonRightRegion.transform.FindChild ("Moves").GetChild (3).GetComponent<Dropdown> ().RefreshShownValue ();
//    		for (int i = 0; i < myPokemon.GetRibbonCount(); i++)
//    		{            
//                pokemonRightRegion.transform.FindChild ("Ribbons").GetChild (0).GetChild (
//    				myPokemon.GetRibbon(i)).GetComponent<Toggle> ().isOn = true;
//    		} //end for
//        } //end else
//	} //end UpdateDebug(Pokemon myPokemon)
//	#endregion
//    #endregion
//} //end SceneManager class