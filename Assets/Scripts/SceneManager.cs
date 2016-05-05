/***************************************************************************************** 
 * File:    SceneManager.cs
 * Summary: Controls behavior for each scene in the game
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

public class SceneManager : MonoBehaviour 
{
    #region Variables
    //Scene Variables
    public enum OverallGame
    {
        INTRO,
        MENU,
        NEWGAME,
        CONTINUE,
        PC,
        SHOP,
        POKEDEX
    } //end OverallGame

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
        POKEMONHELD
    } //end PCGame

	//Scene variables
    OverallGame sceneState;         //The scene the program is in
	Image fade;						//Fade screen
	GameObject selection;			//Red selection rectangle
	GameObject text;				//Text unit
	GameObject confirm;				//Yes/No confirmation unit
	GameObject choices;				//Choices unit
	GameObject input;				//Allows player to input text
	InputField inputText;			//Input field of input unit
	int checkpoint;					//Manages function progress
	bool processing;				//Currently performing task

	//Intro variables
	GameObject title;				//Title on Intro scene
	GameObject image;				//Image on Intro scene
	GameObject enter;				//Press enter to start

	//Animation variables
	bool  playing = false;			//Animation is playing
	float damping = 0.8f;			//How fast animation is
	float maxScale = 2f;			//Maximum scale of object in animation
	int   bounces = 3;				//Bounces of title

	//Menu variables
	List<RectTransform> transforms;	//List of RectTransforms of choices
	GameObject mChoices;			//All the menu choices
	GameObject mContinue;			//Menu's continue data
	int choiceNumber = 0;			//What choice is highlighted 
	Text pName;						//Player's name on continue panel
	Text badges;					//Amount of badges save file has
	Text totalTime;					//Total time on save file

	//New Game variables
	Image profBase;					//Base professor stands on
	Image professor;				//Professor image
	string playerName;				//The player's name

    //Continue Game variables
    MainGame gameState;             //Current state of the main game
    bool initialize;                //Initialize each state only once per access
    GameObject buttonMenu;          //Menu of buttons in main game
    GameObject gymBattle;           //Screen of region leader battles
    GameObject playerTeam;          //Screen of the player's team
    GameObject summaryScreen;       //Screen showing summary of data for pokemon
    GameObject ribbonScreen;        //Screen showing ribbons for pokemon
    GameObject trainerCard;         //Screen of the trainer card
    GameObject debugOptions;        //Screen of the debug options
    GameObject currentTeamSlot;     //The object that is currently highlighted on the team
    GameObject currentSwitchSlot;   //The object that is currently highlighted for switching to
    GameObject currentMoveSlot;     //The object that is currently highlighted for reading/moving
    GameObject currentRibbonSlot;   //The object that is currently highlighted for reading
    int previousTeamSlot;           //The slot last highlighted
    int subMenuChoice;              //What choice is highlighted in the pokemon submenu
    int summaryChoice;              //What page is open on the summary screen
    int moveChoice;                 //What move is being highlighted for details
    int detailsSize;                //Font size for move description
    int switchChoice;               //The pokemon or move chosen to switch with the selected
    int previousSwitchSlot;         //The slot last highlighted for switching to
    int ribbonChoice;               //The ribbon currently shown
    int previousRibbonChoice;       //The ribbon last highlighted for reading

    //PC variables
    PCGame pcState;                 //Current state of the PC
    Pokemon selectedPokemon;        //The currently selected pokemon
    Pokemon heldPokemon;            //Pokemon held if move was chosen
    GameObject boxBack;             //The wallpaper and pokemon panels for PC
    GameObject detailsRegion;       //Area that displays the details of a highlighted pokemon
    GameObject choiceHand;          //The highlighter for the PC
    GameObject heldImage;           //Image of the held pokemon
    GameObject partyTab;            //The panel displaying the current team in PC
    GameObject currentPCSlot;       //The object that is currently highlighted
    List<bool> markingChoices;      //A list of the marking choices made for the pokemon
    int boxChoice;                  //The pokemon that is highlighted

    //Pokedex variables
    GameObject index;               //Region displaying overall pokedex location
    GameObject stats;               //The stat bars of the selected pokemon
    GameObject characteristics;     //The characteristics of the selected pokemon
    GameObject movesRegion;         //The moves the selected pokemon can learn
    GameObject evolutionsRegion;    //The pokemon the selected pokemon can evolve into
    GameObject shownButton;         //Whether weaknesses or resistances are shown
    GameObject weakTypes;           //Object containing all types selected pokemon is weak to
    GameObject resistTypes;         //Object containing all types selected pokemon is resistant to
    Image pokemonImage;             //The image of the selected pokemon
    Text abilitiesText;             //The abilities the selected pokemon can have
    #endregion

    #region Methods
    /***************************************
     * Name: Awake
     * Initializes scene object references and control variables
     ***************************************/
	void Awake()
	{
		//Reset scene tools canvas' camera
		GameManager.tools.GetComponent<Canvas> ().worldCamera = Camera.main;

		//Get scene tools
		fade = GameManager.tools.transform.GetChild (0).gameObject.GetComponent<Image> ();
		text = GameManager.tools.transform.GetChild (1).gameObject;
		confirm = GameManager.tools.transform.GetChild (2).gameObject;
		choices = GameManager.tools.transform.GetChild (3).gameObject;
		input = GameManager.tools.transform.GetChild (4).gameObject;
		selection = GameManager.tools.transform.GetChild (5).gameObject;
		inputText = input.transform.GetChild (1).GetComponent<InputField>();

		//Disable scene tools
		fade.gameObject.SetActive (false);
		selection.SetActive (false);
		text.SetActive (false);
		confirm.SetActive (false);
		choices.SetActive (false);
		input.SetActive (false);

        //Begin game at intro
        sceneState = OverallGame.INTRO;

		//Begin checkpoint at zero
		checkpoint = 0;

		//Begin processing as false
		processing = false;
	} //end Awake

	#region Scenes
    /***************************************
     * Name: Intro
     * Runs intro scene animation
     ***************************************/
	public void Intro()
	{
		//If something is already happening, handle input then return
		if(processing)
		{
            GatherInput();
            return;
		} //end if

		//Get title screen objects
		if(checkpoint == 0)
		{
			processing = true;
            text.SetActive(false);
			title = GameObject.Find("Title");
			image = GameObject.Find("Image");
			enter = GameObject.Find("PressEnter");
			checkpoint = 1;
			processing = false;
		} //end if

		//Black out starting image, shrink title, hide enter
		else if(checkpoint == 1)
		{
			processing = true;
			image.GetComponent<Image>().color = Color.black;
			title.transform.localScale = new Vector3(0.2f, 0.2f);
			enter.SetActive(false);
			fade.gameObject.SetActive (false);
			checkpoint = 2;
			processing = false;
		} //end else if

		//Play animation
		else if(checkpoint == 2)
		{
			processing = true;
			playing = true;
			StartCoroutine(IntroAnimation());
		} //end else if

		//End animation and fade out when player hits enter/return
		else if(checkpoint == 3)
		{
			processing = true;
            GatherInput();
            processing = false;			
		} //end else if

		//Move to menu scene when finished fading out
		else if(checkpoint == 4)
		{
            StartCoroutine(LoadScene("StartMenu", OverallGame.MENU));
		} //end else if
	} //end Intro

    /***************************************
     * Name: Menu
     * Controls menu display and commands
     ***************************************/
	public void Menu()
	{
		//If something is already happening, return
		if(processing)
		{
			return;
		} //end if

		// Get menu objects
		if(checkpoint == 0) 
		{
			//Begin processing
			processing = true;

			//Set up fade screen
			fade.gameObject.SetActive(true);

			//Set reference for menu components
			mChoices = GameObject.Find ("ChoiceGrid");
			mContinue = GameObject.Find("ContinueGrid");
			pName = mContinue.transform.GetChild(0).GetComponent<Text>();
			badges = mContinue.transform.GetChild(1).GetComponent<Text>();
			totalTime = mContinue.transform.GetChild(2).GetComponent<Text>();

            //Initialize text box
            GameManager.instance.InitText(text.transform, text.transform.GetChild(1));

			// Initialize rect transform list
			transforms = new List<RectTransform>();
			for(int i = 0; i < mChoices.transform.childCount; i++)
			{
			    transforms.Add(mChoices.transform.GetChild(i).GetComponent<RectTransform>());
			} //end for

			// If a file isn't found, disable continue option, process alternate menu
			if(!GameManager.instance.GetPersist())
			{
				mContinue.SetActive(false);
				mChoices.transform.GetChild(0).gameObject.SetActive(false);
				choiceNumber = 1;
				checkpoint = 3;
			} //end if
			//Otherwise file is found, process normally
			else
			{
				checkpoint = 1;
			} //end else
			//End processing
			processing = false;
		} //end if
		// Initialize menu data
		else if(checkpoint == 1)
		{
			//Begin processing
			processing = true;

			//Disable fade screen
			fade.gameObject.SetActive(false);

			//Set up selection rectangle
			selection.SetActive(true);

			//Resize to width of top choice, and to height of 1/3 of top choice's height
			selection.GetComponent<RectTransform>().sizeDelta = 
				new Vector2(transforms[choiceNumber].sizeDelta.x, transforms[choiceNumber].sizeDelta.y/3);

			//Reposition to location of top choice, with 2 unit offset to center it
			selection.transform.position = new Vector3(mChoices.transform.GetChild(choiceNumber).transform.position.x, 
			                                           mChoices.transform.GetChild(choiceNumber).transform.position.y-1,
			                                           100);
		
			//Fill in continue area
			pName.text = GameManager.instance.GetTrainer().PlayerName;
			badges.text = "Badges: " + GameManager.instance.GetTrainer().PlayerBadges.ToString();
			totalTime.text = "Playtime: " + GameManager.instance.GetTrainer().HoursPlayed.ToString() + ":" + 
				GameManager.instance.GetTrainer().MinutesPlayed.ToString("00");

			//Run fade animation
			StartCoroutine(FadeInAnimation(2));
		} //end else if
		//Run menu
		else if(checkpoint == 2)
		{
			//Begin processing
			processing = true;

            //Get the player input
            GatherInput();

			//Menu finished this check
			processing = false;
		} //end else if
		// Initialize scene data without continue
		else if(checkpoint == 3)
		{
			//Begin processing
			processing = true;

			//Disable fade screen
			fade.gameObject.SetActive(false);

			//Set up selection rectangle
			selection.SetActive(true);

			//Resize to width of top option, and to height 1/3 of top option's height
			selection.GetComponent<RectTransform>().sizeDelta = 
				new Vector2(transforms[choiceNumber].sizeDelta.x, transforms[choiceNumber].sizeDelta.y/3);

			//Reposition to top choice's location, with 2 unit offset to center it
			selection.transform.position = 
				new Vector3(mChoices.transform.GetChild(choiceNumber).transform.position.x, 
				            mChoices.transform.GetChild(choiceNumber).transform.position.y-2,
				            100);

			//Disable it again
			selection.SetActive(false);

			//Run fade animation
			StartCoroutine(FadeInAnimation(4));
		} //end else if
		//Run menu without continue
		else if(checkpoint == 4)
		{
			//Begin processing
			processing = true;

			//Enable selection tool
			if(!selection.activeInHierarchy)
			{
				selection.SetActive(true);
			} //end if

            //Get player input
            GatherInput();

			//Menu finished this check
			processing = false;
		} //end else if
		//Move to relevant scene
		else if(checkpoint == 5)
		{
            //Disable selection
            selection.SetActive(false);

			// First choice selected, this is usually continue
			if(choiceNumber == 0)
			{
                StartCoroutine(LoadScene("MainGame", OverallGame.CONTINUE));
			} //end if
			// Second choice selected, this is usually new game
			else if(choiceNumber == 1)
			{
                StartCoroutine(LoadScene("NewGame", OverallGame.NEWGAME));
			} //end else if
			// Third choice selected, this is usually options
			else if(choiceNumber == 2)
			{
                StartCoroutine(LoadScene("Intro", OverallGame.INTRO));
			} //end else if
		} //end else if
	} //end Menu

    /***************************************
     * Name: NewGame
     * Loads and plays the New Game scene 
     ***************************************/
	public void NewGame()
	{
		//If something is already happening, return
		if(processing)
		{
			return;
		} //end if

		//Set up scene
		if(checkpoint == 0)
		{
			processing = true;
			profBase = GameObject.Find("Base").GetComponent<Image>();
			professor = GameObject.Find("Professor").GetComponent<Image>();
			profBase.color = new Color(1, 1, 1, 0);
			professor.color = new Color(1, 1, 1, 0);
			StartCoroutine(FadeInAnimation(1));
		} //end if
		//Init SystemManager variable
		else if(checkpoint == 1)
		{
			processing = true;
			StartCoroutine(FadeObjectIn(new Image[]{profBase, professor}, 2)); 
		} //end else if
		//Begin scene
		else if(checkpoint == 2)
		{
            //Begin processing
			processing = true;
			
            //Play text
            GameManager.instance.DisplayText("Welcome to Pokemon Carousel! I am the Ringmaster " +
            "Melotta.", false);

            //Move to next checkpoint
    		checkpoint = 3;

            //End processing
			processing = false;
		} //end else if
		else if(checkpoint == 3)
		{
            //Begin processing
			processing = true;

            //Display text
            GameManager.instance.DisplayText("This circus has attracted major gym leaders from " +
                "around the world! In fact, that's why you're here, isn't it?", false);

            //Move to next checkpoint
            checkpoint = 4;          

            //End processing
			processing = false;
		} //end else if
		else if(checkpoint == 4)
		{
            //Begin processing
			processing = true;

            //Display text
            GameManager.instance.DisplayText("Alright, let's get you set up. First, what is " +
                "your name?", true);
            
            //Move to next checkpoint
            checkpoint = 5;

            //End processing
			processing = false;
		} //end else if
		else if(checkpoint == 5)
		{
            //Begin processing
			processing = true;

            //Display name input
            input.SetActive(true);
            input.transform.GetChild(2).GetComponent<Text>().text = "Please enter your name.";
            input.transform.GetChild(0).GetComponent<Text>().text = "Player name:";
            inputText.text = "";
            inputText.ActivateInputField();
            checkpoint = 6;

            //End processing
			processing = false;
		} //end else if
		else if(checkpoint == 6)
		{
            //Begin processing
			processing = true;
			
            //Make sure text field is always active
			if(input.activeInHierarchy)
			{
				inputText.ActivateInputField();
			} //end if

            //Get player input
            GatherInput();

            //End processing
			processing = false;
		} //end else if
		else if(checkpoint == 7)
		{
            //Being processing
			processing = true;

			//Activate confirmation box
			confirm.SetActive(true);

			//Get confirm's dimensions
			Vector3[] boxDimensions = new Vector3[4];
			confirm.transform.GetChild(0).GetComponent<RectTransform>().GetWorldCorners(boxDimensions);
			float width = boxDimensions[2].x - boxDimensions[0].x;
			float height = boxDimensions[2].y - boxDimensions[0].y;

			//Reposition selection rect
			selection.SetActive(true);
			choiceNumber = 0;

			//Resize to same as top choice
			selection.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
			//Reposition to location of top choice, with 2 unit offset to center it
			selection.transform.position = new Vector3(confirm.transform.GetChild(0).position.x,
			                                           confirm.transform.GetChild(0).position.y,
			                                           100);

			//Continue to next section when selection rect is properly set
			if(selection.GetComponent<RectTransform>().sizeDelta.x != 0)
			{
				checkpoint = 8;
			} //end if

            //End processing
			processing = false;
		} //end else if
		else if(checkpoint == 8)
		{
            //Begin processing
			processing = true;

            //Get player input
            GatherInput();

            //End processing
            processing = false;
		} //end else if
		else if(checkpoint == 9)
		{
            //Begin processing
			processing = true;

            //Display text
            GameManager.instance.DisplayText("Great! Now here's your things. See you again.", true);

            //Move to next checkpoint
            checkpoint = 10;
			
            //End processing
            processing = false;
		} //end else if
		else if(checkpoint == 10)
		{
            //Begin processing
			processing = true;

            checkpoint = 0;
            GameManager.instance.RestartFile(GameManager.instance.GetPersist());            
            GameManager.instance.GetTrainer().PlayerName = playerName;
            GameManager.instance.GetTrainer().RandomTeam();
            GameManager.instance.Persist(false);
            StartCoroutine(LoadScene("Intro", OverallGame.INTRO));

            //End processing
			processing = false;
		} //end else if
	} //end NewGame

    /***************************************
     * Name: ContinueGame
     * Loads and plays the main game
     ***************************************/
    public void ContinueGame()
    {
        //If something is already happening, return
        if(processing)
        {
            return;
        } //end if
        
        //Set up scene
        if (checkpoint == 0)
        {
            //Begin processing
            processing = true;

            //Initialize references and states
            gameState = MainGame.HOME;
            initialize = false;
            buttonMenu = GameObject.Find("ButtonMenu");
            EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
            gymBattle = GameObject.Find("GymBattles");
            playerTeam = GameObject.Find("MyTeam");
            summaryScreen = playerTeam.transform.FindChild("Summary").gameObject;
            ribbonScreen = playerTeam.transform.FindChild("Ribbons").gameObject;
            trainerCard = GameObject.Find("PlayerCard");
            debugOptions = GameObject.Find("DebugOptions");

            //Enable debug button if allowed
            //#if UNITY_EDITOR
            buttonMenu.transform.FindChild("Debug").gameObject.SetActive(true);
            buttonMenu.transform.FindChild("Quit").GetComponent<Button>().navigation = Navigation.defaultNavigation;
            //#endif

            //Disable screens
            gymBattle.SetActive(false);
            playerTeam.SetActive(false);
            summaryScreen.SetActive(false);
            ribbonScreen.SetActive(false);
            trainerCard.SetActive(false);
            debugOptions.SetActive(false);

            //Details size has not been set yet
            detailsSize = -1;
        
            //Fade in
            StartCoroutine (FadeInAnimation (1));
        } //end if
        else if (checkpoint == 1)
        {
            //Begin processing
            processing = true;

            //Disable fade screen
            fade.gameObject.SetActive (false);

            //Move to next section 
            checkpoint = 2;
  
            //End processing
            processing = false;
        } //end else if
        else if (checkpoint == 2)
        {
            //Begin processing
            processing = true;

            //Process according to game state
            if(gameState == MainGame.HOME)
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
                GatherInput();
            } //end if
            else if(gameState == MainGame.GYMBATTLE)
            {
                //Initialize each scene only once
                if(!initialize)
                {
                    initialize = true;
                    buttonMenu.SetActive(false);
                    gymBattle.SetActive(true);
                } //end if

                //Get player input
                GatherInput();
            } //end else if
            else if(gameState == MainGame.TEAM)
            {
                //Initalize each scene only once
                if(!initialize)
                {
                    initialize = true;
                    buttonMenu.SetActive(false);
                    playerTeam.SetActive(true);

                    //Fill in choices box
                    for(int i = choices.transform.childCount-1; i < 4; i++)
                    {
                        GameObject clone = Instantiate(choices.transform.GetChild(0).gameObject,
                                                       choices.transform.GetChild(0).position,
                                                       Quaternion.identity) as GameObject;
                        clone.transform.SetParent(choices.transform);
                    } //end for
                    choices.transform.GetChild(0).GetComponent<Text>().text = "Summary";
                    choices.transform.GetChild(1).GetComponent<Text>().text = "Switch";
                    choices.transform.GetChild(2).GetComponent<Text>().text = "Item";
                    choices.transform.GetChild(3).GetComponent<Text>().text = "Ribbons";
                    choices.transform.GetChild(4).GetComponent<Text>().text = "Cancel";
                    if(choices.transform.childCount > 5)
                    {
                        for(int i = 5; i < choices.transform.childCount; i++)
                        {
                            Destroy(choices.transform.GetChild(i).gameObject);
                        } //end for
                    } //end if

                    //Fill in all team data
                    playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Button>().interactable = false;
                    playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Button>().interactable = false;
                    playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Image>().color = Color.gray;
                    playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Image>().color = Color.gray;
                    for(int i = 1; i < GameManager.instance.GetTrainer().Team.Count+1; i++)
                    {
                        if(i == 1)
                        {
                            if(GameManager.instance.GetTrainer().Team[i-1].Status==1)
                            {
                                playerTeam.transform.FindChild("Background").GetChild(i-1).
                                    GetComponentInChildren<Image>().sprite = 
                                        Resources.Load<Sprite>("Sprites/Menus/partyPanelRoundSelFnt");
                            } //end if
                            else
                            {
                                playerTeam.transform.FindChild("Background").GetChild(i-1).
                                    GetComponentInChildren<Image>().sprite = 
                                        Resources.Load<Sprite>("Sprites/Menus/partyPanelRoundSel");
                            } //end else
                            playerTeam.transform.FindChild("Pokemon" + i).FindChild("PartyBall").
                                GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/partyBallSel");
                        } //end if
                        else
                        {
                            if(GameManager.instance.GetTrainer().Team[i-1].Status==1)
                            {
                                playerTeam.transform.FindChild("Background").GetChild(i-1).
                                    GetComponentInChildren<Image>().sprite = 
                                        Resources.Load<Sprite>("Sprites/Menus/partyPanelRectFnt");
                            } //end if
                            else
                            {
                                playerTeam.transform.FindChild("Background").GetChild(i-1).
                                    GetComponentInChildren<Image>().sprite = 
                                        Resources.Load<Sprite>("Sprites/Menus/partyPanelRect");
                            } //end else
                            playerTeam.transform.FindChild("Pokemon" + i).FindChild("PartyBall").
                                GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/partyBall");
                        } //end else
                        switch(GameManager.instance.GetTrainer().Team[i-1].Status)
                        {
                            //Healthy
                            case 0:
                            {
                                playerTeam.transform.FindChild("Pokemon" + i).FindChild("Status").
                                GetComponentInChildren<Image>().color = Color.clear;
                                break;
                            } //end case 0
                            //Faint
                            case 1:
                            {
                                playerTeam.transform.FindChild("Pokemon" + i).FindChild("Status").
                                    GetComponentInChildren<Image>().sprite = DataContents.statusSprites[5];
                                break;
                            } //end case 1
                            //Sleep
                            case 2:
                            {
                                playerTeam.transform.FindChild("Pokemon" + i).FindChild("Status").
                                    GetComponentInChildren<Image>().sprite = DataContents.statusSprites[0];
                                break;
                            } //end case 2
                            //Poison
                            case 3:
                            {
                                playerTeam.transform.FindChild("Pokemon" + i).FindChild("Status").
                                    GetComponentInChildren<Image>().sprite = DataContents.statusSprites[1];
                                break;
                            } //end case 3
                            //Burn
                            case 4:
                            {
                                playerTeam.transform.FindChild("Pokemon" + i).FindChild("Status").
                                    GetComponentInChildren<Image>().sprite = DataContents.statusSprites[2];
                                break;
                            } //end case 4
                            //Paralyze
                            case 5:
                            {
                                playerTeam.transform.FindChild("Pokemon" + i).FindChild("Status").
                                    GetComponentInChildren<Image>().sprite = DataContents.statusSprites[3];
                                break;
                            } //end case 5
                            //Freeze
                            case 6:
                            {
                                playerTeam.transform.FindChild("Pokemon" + i).FindChild("Status").
                                    GetComponentInChildren<Image>().sprite = DataContents.statusSprites[4];
                                break;
                            } //end case 6                            
                        } //end switch

                        playerTeam.transform.FindChild("Pokemon" + i).FindChild("Sprite").
                            GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/Icons/icon" 
                            + GameManager.instance.GetTrainer().Team[i-1].NatSpecies.ToString("000"));
                        playerTeam.transform.FindChild("Pokemon" + i).FindChild("Nickname").
                            GetComponentInChildren<Text>().text = GameManager.instance.GetTrainer().Team[i-1].Nickname;
                        if(GameManager.instance.GetTrainer().Team[i-1].Item != 0)
                        {
                            playerTeam.transform.FindChild("Pokemon" + i).FindChild("Item").
                                GetComponentInChildren<Image>().color = Color.white;
                        } //end if
                        else
                        {
                            playerTeam.transform.FindChild("Pokemon" + i).FindChild("Item").
                                GetComponentInChildren<Image>().color = Color.clear;
                        } //end else
                        playerTeam.transform.FindChild("Pokemon" + i).FindChild("RemainingHP").
                            GetComponentInChildren<RectTransform>().localScale = new Vector3((float)GameManager.instance.
                            GetTrainer().Team[i-1].CurrentHP/(float)GameManager.instance.GetTrainer().Team[i-1].TotalHP, 1f, 1f);
                        playerTeam.transform.FindChild("Pokemon" + i).FindChild("Level").
                            GetComponentInChildren<Text>().text = "Lv." + GameManager.instance.GetTrainer().Team[i-1].CurrentLevel.ToString();
                        playerTeam.transform.FindChild("Pokemon" + i).FindChild("CurrentHP").
                            GetComponentInChildren<Text>().text = GameManager.instance.GetTrainer().Team[i-1].CurrentHP
                                + "/" + GameManager.instance.GetTrainer().Team[i-1].TotalHP;
                    } //end for

                    //Deactivate any empty party spots
                    for(int i = 6; i > GameManager.instance.GetTrainer().Team.Count; i--)
                    {
                        playerTeam.transform.FindChild("Background").GetChild(i-1).gameObject.SetActive(false);
                        playerTeam.transform.FindChild("Pokemon" + (i)).gameObject.SetActive(false);
                    } //end for

                    //Set choice number to 1 for first slot
                    choiceNumber  = 1;
                    previousTeamSlot = 1;
                    currentTeamSlot = playerTeam.transform.FindChild("Pokemon1").gameObject;
                } //end if !initialize

                //Change instruction text
                playerTeam.transform.FindChild("PartyInstructions").GetChild(0).GetComponent<Text>().text = 
                    "These are your Pokemon.";

                //Collect player input
                GatherInput();

                //Change background sprites based on player input
                if(previousTeamSlot != choiceNumber)
                {
                    //Deactivate panel
                    if(previousTeamSlot == 1)
                    {
                        if(GameManager.instance.GetTrainer().Team[previousTeamSlot-1].Status!=1)
                        {
                            playerTeam.transform.FindChild("Background").GetChild(previousTeamSlot-1).
                                GetComponentInChildren<Image>().sprite = 
                                    Resources.Load<Sprite>("Sprites/Menus/partyPanelRound");
                        } //end if
                        else
                        {
                            playerTeam.transform.FindChild("Background").GetChild(previousTeamSlot-1).
                                GetComponentInChildren<Image>().sprite = 
                                    Resources.Load<Sprite>("Sprites/Menus/partyPanelRoundFnt");
                        } //end else

                        //Deactivate party ball
                        playerTeam.transform.FindChild("Pokemon" + previousTeamSlot).FindChild("PartyBall").
                            GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/partyBall");
                    } //end if
                    else if(previousTeamSlot > 0)
                    {
                        if(GameManager.instance.GetTrainer().Team[previousTeamSlot-1].Status!=1)
                        {
                            playerTeam.transform.FindChild("Background").GetChild(previousTeamSlot-1).
                                GetComponentInChildren<Image>().sprite = 
                                    Resources.Load<Sprite>("Sprites/Menus/partyPanelRect");
                        } //end if
                        else
                        {
                            playerTeam.transform.FindChild("Background").GetChild(previousTeamSlot-1).
                                GetComponentInChildren<Image>().sprite = 
                                    Resources.Load<Sprite>("Sprites/Menus/partyPanelRectFnt");
                        } //end else

                        //Deactivate party ball
                        playerTeam.transform.FindChild("Pokemon" + previousTeamSlot).FindChild("PartyBall").
                            GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/partyBall");
                    } //end else if

                    //Activate panel
                    playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Button>().interactable = false;
                    playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Button>().interactable = false;
                    playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Image>().color = Color.gray;
                    playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Image>().color = Color.gray;
                    if(choiceNumber == 1)
                    {
                        if(GameManager.instance.GetTrainer().Team[choiceNumber-1].Status!=1)
                        {
                            playerTeam.transform.FindChild("Background").GetChild(choiceNumber-1).
                                GetComponentInChildren<Image>().sprite = 
                                    Resources.Load<Sprite>("Sprites/Menus/partyPanelRoundSel");
                        } //end if
                        else
                        {
                            playerTeam.transform.FindChild("Background").GetChild(choiceNumber-1).
                                GetComponentInChildren<Image>().sprite = 
                                    Resources.Load<Sprite>("Sprites/Menus/partyPanelRoundSelFnt");
                        } //end else

                        //Activate party ball
                        playerTeam.transform.FindChild("Pokemon" + choiceNumber).FindChild("PartyBall").
                            GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/partyBallSel");
                    } //end if
                    else if(choiceNumber == -1)
                    {
                        playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Image>().color = Color.gray*2;
                        playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Button>().interactable = true;
                    } //end else if
                    else if(choiceNumber == 0)
                    {
                        playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Image>().color = Color.gray*2;
                        playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Button>().interactable = true;
                    } //end else if
                    else
                    {
                        if(GameManager.instance.GetTrainer().Team[choiceNumber-1].Status!=1)
                        {
                            playerTeam.transform.FindChild("Background").GetChild(choiceNumber-1).
                                GetComponentInChildren<Image>().sprite = 
                                    Resources.Load<Sprite>("Sprites/Menus/partyPanelRectSel");
                        } //end if
                        else
                        {
                            playerTeam.transform.FindChild("Background").GetChild(choiceNumber-1).
                                GetComponentInChildren<Image>().sprite = 
                                    Resources.Load<Sprite>("Sprites/Menus/partyPanelRectSelFnt");
                        } //end else

                        //Activate party ball
                        playerTeam.transform.FindChild("Pokemon" + choiceNumber).FindChild("PartyBall").
                            GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/partyBallSel");
                    } //end else

                    //Previous slot is now deactivated, set equal to current
                    previousTeamSlot = choiceNumber;
                } //end if
            } //end else if
            else if(gameState == MainGame.POKEMONSUBMENU)
            {
                //Initialize
                if(!initialize)
                {                    
                    //Reposition choices to bottom right
                    choices.GetComponent<RectTransform>().position = new Vector3(
                        choices.GetComponent<RectTransform>().position.x,
                        choices.GetComponent<RectTransform>().rect.height/2);
                    
                    //Reposition selection to top menu choice
                    selection.transform.position = choices.transform.GetChild(0).position;
                    
                    //Finished initializing
                    initialize = true;
                } //end if

                //Get player input
                GatherInput();

                //Reposition choices to bottom right
                choices.GetComponent<RectTransform>().position = new Vector3(
                    choices.GetComponent<RectTransform>().position.x,
                    choices.GetComponent<RectTransform>().rect.height/2);
            } //end else if
            else if(gameState == MainGame.POKEMONRIBBONS)
            {
                if(!initialize)
                {
                    //Fill in ribbon screen with correct data
                    ribbonScreen.SetActive(true);
                    ribbonScreen.transform.FindChild("Name").GetComponent<Text>().text=
                        GameManager.instance.GetTrainer().Team[choiceNumber-1].Nickname;
                    ribbonScreen.transform.FindChild("Level").GetComponent<Text>().text=
                        GameManager.instance.GetTrainer().Team[choiceNumber-1].CurrentLevel.ToString();
                    ribbonScreen.transform.FindChild("Ball").GetComponent<Image>().sprite=
                        Resources.Load<Sprite>("Sprites/Icons/summaryBall"+GameManager.instance.GetTrainer().
                        Team[choiceNumber-1].BallUsed.ToString("00"));
                    ribbonScreen.transform.FindChild("Gender").GetComponent<Image>().sprite=
                        Resources.Load<Sprite>("Sprites/Icons/gender"+GameManager.instance.GetTrainer().
                        Team[choiceNumber-1].Gender.ToString());
                    ribbonScreen.transform.FindChild("Sprite").GetComponent<Image>().sprite=
                        Resources.Load<Sprite>("Sprites/Pokemon/"+GameManager.instance.GetTrainer().
                        Team[choiceNumber-1].NatSpecies.ToString("000"));
                    ribbonScreen.transform.FindChild("Markings").GetComponent<Text>().text=
                        GameManager.instance.GetTrainer().Team[choiceNumber-1].GetMarkingsString();
                    ribbonScreen.transform.FindChild("Item").GetComponent<Text>().text=
                        DataContents.GetItemGameName(GameManager.instance.GetTrainer().Team[choiceNumber-1].Item);
                    ribbonScreen.transform.FindChild("RibbonName").gameObject.SetActive(false);
                    ribbonScreen.transform.FindChild("RibbonDescription").gameObject.SetActive(false);

                    //No ribbon selected yet
                    previousRibbonChoice = -1;
                    ribbonChoice = 0;
                    selection.SetActive(false);

                    //Set existing ribbons to inactive
                    foreach(Transform child in ribbonScreen.transform.FindChild("RibbonRegion").transform)
                    {
                        child.gameObject.SetActive(false);
                    } //end for

                    //Add ribbons
                    for(int i = 0; i < GameManager.instance.GetTrainer().Team[choiceNumber-1].GetRibbonCount();i++)
                    {
                        //If at least one ribbon exists, resize the selection box
                        if(i == 0)
                        {                            
                            StartCoroutine("WaitForResize");
                        } //end if

                        //The ribbon already exists, just fill it in
                        if(i < ribbonScreen.transform.FindChild("RibbonRegion").childCount)
                        {
                            GameObject newRibbon = ribbonScreen.transform.FindChild("RibbonRegion").GetChild(i).
                                gameObject;
                            newRibbon.gameObject.SetActive(true);
                            newRibbon.GetComponent<Image>().sprite = DataContents.ribbonSprites[
                                GameManager.instance.GetTrainer().Team[choiceNumber-1].GetRibbon(i)];
                        } //end if
                        //Create new ribbon
                        else
                        {
                            GameObject newRibbon = Instantiate(ribbonScreen.transform.FindChild("RibbonRegion").
                                GetChild(0).gameObject);
                            newRibbon.transform.SetParent(ribbonScreen.transform.FindChild("RibbonRegion"));
                            newRibbon.GetComponent<Image>().sprite = DataContents.ribbonSprites[
                                GameManager.instance.GetTrainer().Team[choiceNumber-1].GetRibbon(i)];
                            newRibbon.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                            newRibbon.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                            newRibbon.SetActive(true);
                        } //end else
                    } //end for

                    //Initialization is done
                    initialize = true;
                } //end if

                //Get player input
                GatherInput();
            } //end else if
            else if(gameState == MainGame.POKEMONSUMMARY)
            {
                //Get player input
                GatherInput();

                //Fill in the summary screen with the correct data
                PokemonSummary(GameManager.instance.GetTrainer().Team[choiceNumber-1]);
            } //end else if
            else if(gameState == MainGame.POKEMONSWITCH)
            {
                //Change instruction text
                playerTeam.transform.FindChild("PartyInstructions").GetChild(0).GetComponent<Text>().text = 
                    "Switch to where?";

                //Get player input
                GatherInput();

                //Change background sprites based on player input
                if(previousSwitchSlot != switchChoice)
                {
                    //Deactivate panel
                    if(previousSwitchSlot != choiceNumber)
                    {
                        if(previousSwitchSlot == 1)
                        {
                            if(GameManager.instance.GetTrainer().Team[previousSwitchSlot-1].Status!=1)
                            {
                                playerTeam.transform.FindChild("Background").GetChild(previousSwitchSlot-1).
                                    GetComponentInChildren<Image>().sprite = 
                                        Resources.Load<Sprite>("Sprites/Menus/partyPanelRound");
                            } //end if
                            else
                            {
                                playerTeam.transform.FindChild("Background").GetChild(previousSwitchSlot-1).
                                    GetComponentInChildren<Image>().sprite = 
                                        Resources.Load<Sprite>("Sprites/Menus/partyPanelRoundFnt");
                            } //end else
                        } //end if
                        else if(previousSwitchSlot > 0)
                        {
                            if(GameManager.instance.GetTrainer().Team[previousSwitchSlot-1].Status!=1)
                            {
                                playerTeam.transform.FindChild("Background").GetChild(previousSwitchSlot-1).
                                    GetComponentInChildren<Image>().sprite = 
                                        Resources.Load<Sprite>("Sprites/Menus/partyPanelRect");
                            } //end if
                            else
                            {
                                playerTeam.transform.FindChild("Background").GetChild(previousSwitchSlot-1).
                                    GetComponentInChildren<Image>().sprite = 
                                        Resources.Load<Sprite>("Sprites/Menus/partyPanelRectFnt");
                            } //end else
                        } //end else if

                        //Deactivate the party ball
                        playerTeam.transform.FindChild("Pokemon" + previousSwitchSlot).FindChild("PartyBall").
                            GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/partyBall");
                    } //end if

                    //Activate panel
                    if(switchChoice != choiceNumber)
                    {
                        if(switchChoice == 1)
                        {
                            if(GameManager.instance.GetTrainer().Team[switchChoice-1].Status!=1)
                            {
                                playerTeam.transform.FindChild("Background").GetChild(switchChoice-1).
                                    GetComponentInChildren<Image>().sprite = 
                                        Resources.Load<Sprite>("Sprites/Menus/partyPanelRoundSel");
                            } //end if
                            else
                            {
                                playerTeam.transform.FindChild("Background").GetChild(switchChoice-1).
                                    GetComponentInChildren<Image>().sprite = 
                                        Resources.Load<Sprite>("Sprites/Menus/partyPanelRoundSelFnt");
                            } //end else                         
                        } //end if
                        else
                        {
                            if(GameManager.instance.GetTrainer().Team[switchChoice-1].Status!=1)
                            {
                                playerTeam.transform.FindChild("Background").GetChild(switchChoice-1).
                                    GetComponentInChildren<Image>().sprite = 
                                        Resources.Load<Sprite>("Sprites/Menus/partyPanelRectSel");
                            } //end if
                            else
                            {
                                playerTeam.transform.FindChild("Background").GetChild(switchChoice-1).
                                    GetComponentInChildren<Image>().sprite = 
                                        Resources.Load<Sprite>("Sprites/Menus/partyPanelRectSelFnt");
                            } //end else                           
                        } //end else

                        //Activate party ball
                        playerTeam.transform.FindChild("Pokemon" + switchChoice).FindChild("PartyBall").
                            GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/partyBallSel");
                    } //end if
                    //Previous slot is now deactivated, set equal to current
                    previousSwitchSlot = switchChoice;
                } //end if
            } //end else if
            else if(gameState == MainGame.MOVESWITCH)
            {
                //Get player input
                GatherInput();
                
                //Highlight selected switch to
                selection.SetActive(true);
                
                //Resize to same as top choice
                Transform moveScreen =  summaryScreen.transform.GetChild(5);
                Vector3 scale = new Vector3(
                    moveScreen.FindChild("Move"+(moveChoice+1)).GetComponent<RectTransform>().rect.width,
                    moveScreen.FindChild("Move"+(moveChoice+1)).GetComponent<RectTransform>().rect.height,
                    0);
                selection.GetComponent<RectTransform>().sizeDelta = scale;
                
                //Reposition to location of top choice, with 2 unit offset to center it
                selection.transform.position = Camera.main.WorldToScreenPoint (currentSwitchSlot.transform.
                                                                               position);
            } //end else if
            else if(gameState == MainGame.TRAINERCARD)
            {
                //Initalize each scene only once
                if(!initialize)
                {
                    initialize = true;
                    buttonMenu.SetActive(false);
                    trainerCard.SetActive(true);
                } //end if

                //Get player input
                GatherInput();
            } //end eise if

            else if(gameState == MainGame.DEBUG)
            {
                //Initalize each scene only once
                if(!initialize)
                {
                    initialize = true;
                    buttonMenu.SetActive(false);
                    debugOptions.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(debugOptions.transform.GetChild(0).gameObject);
                } //end if
                
                //Get player input
                GatherInput();
            } //end eise if

            //End processing
            processing = false;
        } //end else if
    } //end ContinueGame

    /***************************************
     * Name: PC
     * Loads and plays the PC scene 
     ***************************************/
    public void PC()
    {
        //Return if processing
        if (processing)
        {
            return;
        } //end if

        //Initialize each scene only once
        if (checkpoint == 0)
        {         
            //Begin processing 
            processing = true;

            //Get references
            boxBack = GameObject.Find ("BoxBack");
            detailsRegion = GameObject.Find ("Details");
            partyTab = GameObject.Find ("PartyTab");
            heldImage = GameObject.Find ("HeldPokemon");
            choiceHand = heldImage.transform.GetChild(0).gameObject;
            summaryScreen = GameObject.Find ("Summary");
            ribbonScreen = GameObject.Find ("Ribbons");

            //Initialize pcState
            pcState = PCGame.HOME;

            //Fill in choices box
            for (int i = choices.transform.childCount-1; i < 6; i++)
            {
                GameObject clone = Instantiate (choices.transform.GetChild (0).gameObject,
                                               choices.transform.GetChild (0).position,
                                               Quaternion.identity) as GameObject;
                clone.transform.SetParent (choices.transform);
            } //end for
            choices.transform.GetChild (0).GetComponent<Text> ().text = "Move";
            choices.transform.GetChild (1).GetComponent<Text> ().text = "Summary";
            choices.transform.GetChild (2).GetComponent<Text> ().text = "Item";
            choices.transform.GetChild (3).GetComponent<Text> ().text = "Ribbons";
            choices.transform.GetChild (4).GetComponent<Text> ().text = "Markings";
            choices.transform.GetChild (5).GetComponent<Text> ().text = "Release";
            choices.transform.GetChild (6).GetComponent<Text> ().text = "Cancel";
            if (choices.transform.childCount > 7)
            {
                for (int i = 7; i < choices.transform.childCount; i++)
                {
                    Destroy (choices.transform.GetChild (i).gameObject);
                } //end for
            } //end if

            //Details size has not been set yet
            detailsSize = -1;

            //Move to next checkpoint
            checkpoint = 1;

            //End processing
            processing = false;
        } //end if
        else if (checkpoint == 1)
        {
            //Begin processing 
            processing = true;
            System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
            myStopwatch.Start();
            //Disable screens
            partyTab.SetActive (false);
            summaryScreen.SetActive (false);
            ribbonScreen.SetActive (false);

            //Fill in box
            for (int i = 0; i < 30; i++)
            {
                //Get the pokemon in the slot
                selectedPokemon = GameManager.instance.GetTrainer ().GetPC (
                    GameManager.instance.GetTrainer ().GetPCBox (), i);

                //If the slot is null, set the sprite to clear
                if (selectedPokemon == null)
                {
                    boxBack.transform.FindChild ("PokemonRegion").GetChild (i).GetComponent<Image> ().
                        color = Color.clear;
                } //end if
                //Otherwise fill in the icon for the pokemon
                else
                {
                    boxBack.transform.FindChild ("PokemonRegion").GetChild (i).GetComponent<Image> ().
                        color = Color.white;
                    boxBack.transform.FindChild ("PokemonRegion").GetChild (i).GetComponent<Image> ().
                        sprite = Resources.Load<Sprite> ("Sprites/Icons/icon" + selectedPokemon.NatSpecies.ToString ("000"));
                } //end else
            } //end for
            
            //Fill in details
            boxBack.transform.FindChild ("BoxName").GetComponent<Text> ().text = GameManager.instance.
                GetTrainer ().GetPCBoxName ();
            boxBack.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Sprites/Menus/box" + 
                GameManager.instance.GetTrainer ().GetPCBoxWallpaper ());
            detailsRegion.transform.FindChild ("Name").GetComponent<Text> ().text = "";
            detailsRegion.transform.FindChild ("Gender").GetComponent<Image> ().color = Color.clear;
            detailsRegion.transform.FindChild ("Sprite").GetComponent<Image> ().color = Color.clear;
            detailsRegion.transform.FindChild ("Markings").GetComponent<Text> ().text = "";
            detailsRegion.transform.FindChild ("Shiny").GetComponent<Text> ().color = Color.clear;
            detailsRegion.transform.FindChild ("Level").GetComponent<Text> ().text = "";
            detailsRegion.transform.FindChild ("Types").GetChild (0).gameObject.SetActive (false);
            detailsRegion.transform.FindChild ("Types").GetChild (1).gameObject.SetActive (false);
            detailsRegion.transform.FindChild ("Ability").GetComponent<Text> ().text = "";
            detailsRegion.transform.FindChild ("Item").GetComponent<Text> ().text = "";

            //Fill in party tab
            for(int i = 1; i < GameManager.instance.GetTrainer().Team.Count + 1; i++)
            {
                partyTab.transform.FindChild("Pokemon"+i).GetComponent<Image>().sprite =
                    Resources.Load<Sprite>("Sprites/Icons/icon"+GameManager.instance.GetTrainer().
                    Team[i-1].NatSpecies.ToString("000"));
            } //end for

            //Deactivate any empty party spots
            for(int i = 6; i > GameManager.instance.GetTrainer().Team.Count; i--)
            {
                partyTab.transform.FindChild("Pokemon" + (i)).gameObject.SetActive(false);
            } //end for

            //Initialize box choice to boxName
            heldImage.transform.position = new Vector3 (boxBack.transform.FindChild ("BoxName").position.x,
                boxBack.transform.FindChild ("BoxName").position.y + 8, 100);
            boxChoice = -2;

            //Current slot is title
            currentPCSlot = boxBack.transform.FindChild ("BoxName").gameObject;

            //Move to next checkpoint
            StartCoroutine (FadeInAnimation (2));
        } //end else if
        else if (checkpoint == 2)
        {
            //Begin processing 
            processing = true;

            //Process according to PC state
            if (pcState == PCGame.HOME)
            {                
                //Get player input
                GatherInput ();

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
                    {
                        //Load previous PC box
                        GameManager.instance.GetTrainer ().PreviousBox ();

                        //Reset scene
                        checkpoint = 1;
                        break;
                    } //end case -3 (Left box arrow)
                    //Box name
                    case -2:
                    {
                        heldImage.transform.position = new Vector3 (boxBack.transform.FindChild ("BoxName").position.x,
                            boxBack.transform.FindChild ("BoxName").position.y + 8, 100);
                        currentPCSlot = boxBack.transform.FindChild ("BoxName").gameObject;

                        //Update details region
                        FillDetails();
                        break;
                    } //end case -2 (Box name)
                    //Right box arrow
                    case -1:
                    {
                        //Load next PC box
                        GameManager.instance.GetTrainer ().NextBox ();
                    
                        //Reset scene
                        checkpoint = 1;
                        break;
                    } //end case -1 (Right box arrow)
                    //Party button
                    case 30:
                    {
                        detailsRegion.transform.FindChild ("Buttons").GetChild (0).GetComponent<Button> ().
                            interactable = true;
                        detailsRegion.transform.FindChild ("Buttons").GetChild (0).GetComponent<Image> ().color =
                            Color.white;
                        heldImage.transform.position = new Vector3 (
                            detailsRegion.transform.FindChild ("Buttons").GetChild (0).position.x,
                            detailsRegion.transform.FindChild ("Buttons").GetChild (0).position.y + 8, 100);
                        currentPCSlot = detailsRegion.transform.FindChild ("Buttons").GetChild (0).gameObject;

                        //Update details region
                        FillDetails();
                        break;
                    } //end case 30 (Party button)
                    //Return button
                    case 31:
                    {
                        detailsRegion.transform.FindChild ("Buttons").GetChild (1).GetComponent<Button> ().
                            interactable = true;
                        detailsRegion.transform.FindChild ("Buttons").GetChild (1).GetComponent<Image> ().color =
                            Color.white;
                        heldImage.transform.position = new Vector3 (
                            detailsRegion.transform.FindChild ("Buttons").GetChild (1).position.x,
                            detailsRegion.transform.FindChild ("Buttons").GetChild (1).position.y + 8, 100);
                        currentPCSlot = detailsRegion.transform.FindChild ("Buttons").GetChild (1).gameObject;

                        //Update details region
                        FillDetails();
                        break;
                    } //end case 31 (Return button)
                    //Pokemon region
                    default:
                    {
                        //Position hand
                        heldImage.transform.position = new Vector3 (
                            boxBack.transform.FindChild ("PokemonRegion").GetChild (boxChoice).position.x,
                            boxBack.transform.FindChild ("PokemonRegion").GetChild (boxChoice).position.y + 8, 100);
                        currentPCSlot = boxBack.transform.FindChild ("PokemonRegion").GetChild (boxChoice).gameObject;

                        //Get the pokemon in the slot
                        selectedPokemon = GameManager.instance.GetTrainer ().GetPC (
                        GameManager.instance.GetTrainer ().GetPCBox (), boxChoice);

                        //Update details region
                        FillDetails();
                        break;
                    } //end default (Pokemon region)
                } //end switch
            } //end if
            else if (pcState == PCGame.POKEMONSUBMENU)
            {
                //Initialize
                if(!initialize)
                {                    
                    //Reposition choices to bottom right
                    choices.GetComponent<RectTransform>().position = new Vector3(
                        choices.GetComponent<RectTransform>().position.x,
                        choices.GetComponent<RectTransform>().rect.height/2);

                    //Reposition selection to top menu choice
                    selection.transform.position = choices.transform.GetChild(0).position;

                    //Finished initializing
                    initialize = true;
                } //end if

                //Get player input
                GatherInput ();
            } //end else if
            else if (pcState == PCGame.POKEMONSUMMARY)
            {
                //Get player input
                GatherInput ();

                //Update selected pokemon
                if(heldPokemon != null)
                {
                    selectedPokemon = heldPokemon;
                } //end else if
                else if(partyTab.activeSelf)
                {
                    selectedPokemon = GameManager.instance.GetTrainer().Team[choiceNumber-1];
                } //end else if
                else
                {
                    selectedPokemon = GameManager.instance.GetTrainer ().GetPC (
                        GameManager.instance.GetTrainer ().GetPCBox (), boxChoice);
                } //end else

                //Display pokemon summary
                PokemonSummary (selectedPokemon);
            } //end else if
            else if (pcState == PCGame.MOVESWITCH)
            {
                //Get player input
                GatherInput ();
                
                //Highlight selected switch to
                selection.SetActive (true);
                
                //Resize to same as top choice
                Transform moveScreen = summaryScreen.transform.GetChild (5);
                Vector3 scale = new Vector3 (
                    moveScreen.FindChild ("Move" + (moveChoice + 1)).GetComponent<RectTransform> ().rect.width,
                    moveScreen.FindChild ("Move" + (moveChoice + 1)).GetComponent<RectTransform> ().rect.height,
                    0);
                selection.GetComponent<RectTransform> ().sizeDelta = scale;
                
                //Reposition to location of top choice, with 2 unit offset to center it
                selection.transform.position = Camera.main.WorldToScreenPoint (currentSwitchSlot.transform.
                                                                               position);
            } //end else if
            else if (pcState == PCGame.PARTY)
            {
                //Get player input
                GatherInput();
                
                //Put choice hand at party slot position
                heldImage.transform.position = new Vector3 (currentTeamSlot.transform.position.x, 
                    currentTeamSlot.transform.position.y + 8, 100);

                //Selected pokemon is same as choice
                if(choiceNumber > 0 && choiceNumber <= GameManager.instance.GetTrainer().Team.Count)
                {
                    selectedPokemon = GameManager.instance.GetTrainer().Team[choiceNumber-1];
                } //end if
                else
                {
                    selectedPokemon = null;
                } //end else

                //Update details region
                FillDetails();
            } //end else if
            else if(pcState == PCGame.POKEMONHELD)
            {
                //If no pokemon is currently held
                if(heldPokemon == null)
                {
                    //Pokemon Party
                    if(partyTab.activeSelf)
                    {
                        //If on last pokemon
                        if(GameManager.instance.GetTrainer().Team.Count == 1)
                        {
                            GameManager.instance.DisplayText("You can't remove your last pokemon.",  true);
                        } //end if

                        heldPokemon = selectedPokemon;
                        selectedPokemon = null;
                        GameManager.instance.GetTrainer().RemovePokemon(choiceNumber-1);
                        choiceHand.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/boxfist");
                        heldImage.GetComponent<Image>().color = Color.white;
                        heldImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Icons/icon" + 
                            heldPokemon.NatSpecies.ToString("000"));

                        //Fill in party tab
                        for(int i = 1; i < GameManager.instance.GetTrainer().Team.Count + 1; i++)
                        {
                            partyTab.transform.FindChild("Pokemon"+i).GetComponent<Image>().sprite =
                                Resources.Load<Sprite>("Sprites/Icons/icon"+GameManager.instance.GetTrainer().
                                Team[i-1].NatSpecies.ToString("000"));
                        } //end for

                        //Deactivate any empty party spots
                        for(int i = 6; i > GameManager.instance.GetTrainer().Team.Count; i--)
                        {
                            partyTab.transform.FindChild("Pokemon" + (i)).gameObject.SetActive(false);
                        } //end for
                    } //end if
                    //Pokemon Region of box
                    else
                    {
                        heldPokemon = selectedPokemon;
                        selectedPokemon = null;
                        GameManager.instance.GetTrainer().RemoveFromPC(
                            GameManager.instance.GetTrainer ().GetPCBox (), boxChoice);
                        choiceHand.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/boxfist");
                        heldImage.GetComponent<Image>().color = Color.white;
                        heldImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Icons/icon" + 
                            heldPokemon.NatSpecies.ToString("000"));
                        boxBack.transform.FindChild ("PokemonRegion").GetChild (boxChoice).GetComponent<Image> ().
                            color = Color.clear;
                    } //end else

                    //Move choice now becomes swap
                    choices.transform.GetChild(0).GetComponent<Text>().text = "Swap";
                } //end if
                //Otherwise switch pokemon
                else
                {
                    //Pokemon Party
                    if(partyTab.activeSelf)
                    {
                        //If there is a pokemon
                        if(selectedPokemon != null)
                        {
                            GameManager.instance.GetTrainer().ReplacePokemon(heldPokemon, choiceNumber-1);
                            heldPokemon = selectedPokemon;
                            selectedPokemon = GameManager.instance.GetTrainer ().Team[choiceNumber-1];
                            heldImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Icons/icon" + 
                                heldPokemon.NatSpecies.ToString("000"));
                            partyTab.transform.FindChild("Pokemon" + choiceNumber).GetComponent<Image> ().
                                sprite = Resources.Load<Sprite>("Sprites/Icons/icon" + selectedPokemon.NatSpecies.
                                ToString("000"));
                        } //end if
                        else
                        {
                            GameManager.instance.GetTrainer().AddPokemon(heldPokemon);
                            selectedPokemon = heldPokemon;
                            heldPokemon = null;
                            choiceHand.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/boxpoint1");
                            heldImage.GetComponent<Image>().color = Color.clear;
                            partyTab.transform.FindChild("Pokemon" + choiceNumber).GetComponent<Image> ().
                                sprite = Resources.Load<Sprite>("Sprites/Icons/icon" + selectedPokemon.NatSpecies.
                                ToString("000"));
                            partyTab.transform.FindChild("Pokemon" + choiceNumber).gameObject.SetActive(true);
                        } //end else
                    } //end if
                    //Pokemon Region of box
                    else
                    {
                        //If there is a pokemon
                        if(selectedPokemon != null)
                        {
                            GameManager.instance.GetTrainer().RemoveFromPC(
                                GameManager.instance.GetTrainer ().GetPCBox (), boxChoice);
                            GameManager.instance.GetTrainer().AddToPC(
                                GameManager.instance.GetTrainer().GetPCBox(), boxChoice, heldPokemon);
                            heldPokemon = selectedPokemon;
                            selectedPokemon = GameManager.instance.GetTrainer ().GetPC(
                                GameManager.instance.GetTrainer().GetPCBox(), boxChoice);
                            heldImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Icons/icon" + 
                                heldPokemon.NatSpecies.ToString("000"));
                            boxBack.transform.FindChild ("PokemonRegion").GetChild (boxChoice).GetComponent<Image> ().
                                sprite = Resources.Load<Sprite>("Sprites/Icons/icon" + selectedPokemon.NatSpecies.
                                ToString("000"));
                        } //end if
                        else
                        {
                            GameManager.instance.GetTrainer().AddToPC(
                                GameManager.instance.GetTrainer().GetPCBox(), boxChoice, heldPokemon);
                            selectedPokemon = heldPokemon;
                            heldPokemon = null;
                            choiceHand.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Menus/boxpoint1");
                            heldImage.GetComponent<Image>().color = Color.clear;
                            boxBack.transform.FindChild ("PokemonRegion").GetChild (boxChoice).GetComponent<Image>().sprite = 
                                Resources.Load<Sprite>("Sprites/Icons/icon" + selectedPokemon.NatSpecies.ToString("000"));
                            boxBack.transform.FindChild ("PokemonRegion").GetChild (boxChoice).GetComponent<Image> ().
                                color = Color.white;

                            //Swap choice now becomes move
                            choices.transform.GetChild(0).GetComponent<Text>().text = "Move";
                        } //end else
                    } //end else
                } //end else

                //Go back to previous state
                pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
            } //end else if
            else if(pcState == PCGame.POKEMONRIBBONS)
            {
                if(!initialize)
                {
                    //Fill in ribbon screen with correct data
                    ribbonScreen.SetActive(true);
                    ribbonScreen.transform.FindChild("Name").GetComponent<Text>().text=
                        selectedPokemon.Nickname;
                    ribbonScreen.transform.FindChild("Level").GetComponent<Text>().text=
                        selectedPokemon.CurrentLevel.ToString();
                    ribbonScreen.transform.FindChild("Ball").GetComponent<Image>().sprite=
                        Resources.Load<Sprite>("Sprites/Icons/summaryBall"+selectedPokemon.BallUsed.ToString("00"));
                    ribbonScreen.transform.FindChild("Gender").GetComponent<Image>().sprite=
                        Resources.Load<Sprite>("Sprites/Icons/gender"+selectedPokemon.Gender.ToString());
                    ribbonScreen.transform.FindChild("Sprite").GetComponent<Image>().sprite=
                        Resources.Load<Sprite>("Sprites/Pokemon/"+selectedPokemon.NatSpecies.ToString("000"));
                    ribbonScreen.transform.FindChild("Markings").GetComponent<Text>().text=
                        selectedPokemon.GetMarkingsString();
                    ribbonScreen.transform.FindChild("Item").GetComponent<Text>().text=
                        DataContents.GetItemGameName(selectedPokemon.Item);
                    ribbonScreen.transform.FindChild("RibbonName").gameObject.SetActive(false);
                    ribbonScreen.transform.FindChild("RibbonDescription").gameObject.SetActive(false);
                    
                    //No ribbon selected yet
                    previousRibbonChoice = -1;
                    ribbonChoice = 0;
                    selection.SetActive(false);
                    
                    //Set existing ribbons to inactive
                    foreach(Transform child in ribbonScreen.transform.FindChild("RibbonRegion").transform)
                    {
                        child.gameObject.SetActive(false);
                    } //end for
                    
                    //Add ribbons
                    for(int i = 0; i < selectedPokemon.GetRibbonCount();i++)
                    {
                        //If at least one ribbon exists, resize the selection box
                        if(i == 0)
                        {                            
                            StartCoroutine("WaitForResize");
                        } //end if
                        
                        //The ribbon already exists, just fill it in
                        if(i < ribbonScreen.transform.FindChild("RibbonRegion").childCount)
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
                GatherInput();
            } //end else if
            else if (pcState == PCGame.POKEMONMARKINGS)
            {
                //Initialize
                if(!initialize)
                {                              
                    //Fill in choices box
                    for (int i = choices.transform.childCount-1; i < DataContents.markingCharacters.Length+2; i++)
                    {
                        GameObject clone = Instantiate (choices.transform.GetChild (0).gameObject,
                        choices.transform.GetChild (0).position, Quaternion.identity) as GameObject;
                        clone.transform.SetParent (choices.transform);
                    } //end for
                    for(int i = 0; i < DataContents.markingCharacters.Length+2; i++)
                    {
                        if(i == DataContents.markingCharacters.Length)
                        {
                            choices.transform.GetChild(i).GetComponent<Text>().text = "OK";
                        } //end if
                        else if(i == DataContents.markingCharacters.Length+1)
                        {
                            choices.transform.GetChild(i).GetComponent<Text>().text = "Cancel";
                        } //end else if
                        else
                        {                           
                            choices.transform.GetChild(i).GetComponent<Text>().text =
                                DataContents.markingCharacters[i].ToString(); 
                        } //end else
                    } //end for

                    if (choices.transform.childCount > DataContents.markingCharacters.Length+1)
                    {
                        for (int i = DataContents.markingCharacters.Length+2; i < choices.transform.childCount; i++)
                        {
                            Destroy (choices.transform.GetChild (i).gameObject);
                        } //end for
                    } //end if

                    //Color in choices
                    for(int i = 0; i < markingChoices.Count; i++)
                    {
                        choices.transform.GetChild(i).GetComponent<Text>().color =
                            markingChoices[i] ? Color.black : Color.gray;
                    } //end for

                    //SubmenuChoice at top
                    subMenuChoice = 0;

                    //Reposition choices menu and selection rectangle
                    StartCoroutine("WaitForResize");

                    //Initialize finished
                    initialize = true;
                } //end if

                //Reposition selection rectangle
                selection.transform.position = choices.transform.GetChild(subMenuChoice).position;

                //Get player input
                GatherInput ();
            } //end else if

            //End processing
            processing = false;
        } //end else if
    } //end PC

    /***************************************
     * Name: Pokedex
     * Loads and plays the Pokedex scene
     ***************************************/
    public void Pokedex()
    {
        //End function if processing
        if (processing)
        {
            return;
        } //end if

        //Begin processing
        processing = true;

        //Handle each stage of the scene
        if (checkpoint == 0)
        {

        } //end if
        else if (checkpoint == 1)
        {

        } //end else if
        else if (checkpoint == 2)
        {

        } //end else if

        //End processing
        processing = false;
    } //end Pokedex
	#endregion

	#region Animations
    /***************************************
     * Name: IntroAnimation
     * This is what the intro animation is
     ***************************************/
	IEnumerator IntroAnimation()
	{
		//Keep internal count of bounces
		int numBounce = 0;
		
		//Internal timer for lerp
		float elapsedTime = 0f;
		
		//Colors
		Color bounceColor = Color.white / bounces;					//Divdes color up into equal parts
		Color targetColor = bounceColor;							//Initializes first target color
		Color lastColor = image.GetComponent<Image> ().color;		//Initializes starting color
		
		//Title
		Vector3 targetScale = new Vector3(maxScale, maxScale, 0);	//Initializes title to bounce to highest
		Vector3 lastScale = title.transform.localScale;				//Initializes last scale size
		
		//Loop doesn't break until break command is reached
		while(true)
		{
			//If player ends the animation early
			if(!playing)
			{
				break;
			} //end if
			
			//Reset bounce target if target met
			if(title.transform.localScale == targetScale)
			{
				//Go to goal if bounces have been met
				if(numBounce == bounces - 1)
				{
					targetScale = new Vector3(1f, 1f);
				} //end if
				//If final bounce is done, break
				else if(numBounce == bounces)
				{
					break;
				} //end else if
				//If even number of bounces
				else if(numBounce%2 == 0)
				{
					//Get a value between 0.2 and 1
					float value = 0.2f + ((0.8f/(float)bounces) * (numBounce + 1));
					targetScale = new Vector3(value, value, 1);
				} //end else if
				//If odd number
				else
				{
					//Get a value between 1 and maxScale
					float value = maxScale - (((maxScale-1)/bounces) * numBounce);
					targetScale = new Vector3(value, value, 1);
				} //end else
				
				//Reset color target
				targetColor = targetColor+bounceColor;
				
				//Reset last positions
				lastColor = image.GetComponent<Image>().color;
				lastScale = title.transform.localScale;
				
				//Increase bounce count
				numBounce++;
				
				//Reset elapsed time
				elapsedTime = 0f;
			} //end if
			
			//Lighten image over bounces
			image.GetComponent<Image>().color = Color.Lerp(lastColor, targetColor, elapsedTime * damping);
			
			//Bounce title
			title.transform.localScale = Vector3.Lerp(lastScale, targetScale, elapsedTime * damping);
			
			//Increase elapsedTime by dt
			elapsedTime += Time.deltaTime;
			
			yield return null;
		} //end while
		
		//Make sure image color is white
		image.GetComponent<Image>().color = Color.white;
		
		//Make sure title scale is 1
		title.transform.localScale = new Vector3 (1f, 1f);
		
		//Process is finished
		enter.SetActive (true);
		checkpoint = 3;
		playing = false;
		processing = false;
	} //end IntroAnimation

    /***************************************
     * Name: FadeInAnimation
     * Fades the blackout screen to show a "fade in" effect of scene
     ***************************************/
	IEnumerator FadeInAnimation(int targetCheckpoint)
	{
		//Initialize color values
		Color startColor = new Color (0, 0, 0, 1);
		Color endColor = new Color (0, 0, 0, 0);

		//Internal elapsed time
		float elapsedTime = 0f;

		//Set fade active
		fade.gameObject.SetActive (true);
		fade.color = startColor;

		//Lerp color for specified time
		while(fade.color.a != 0)
		{
			fade.color = Color.Lerp(startColor, endColor, 2 * elapsedTime);
			elapsedTime+= Time.deltaTime;
			yield return null;
		} //end while

		//Move to next checkpoint
        fade.gameObject.SetActive (false);
		checkpoint = targetCheckpoint;

		//End fade animation
		playing = false;
		processing = false;
	} //end FadeInAnimation(int targetCheckpoint)

    /***************************************
     * Name: FadeOutAnimation
     * Fades blackout screen to create "fade out" effect of scene
     ***************************************/
	IEnumerator FadeOutAnimation(int targetCheckpoint)
	{
		//Initialize color values
		Color startColor = new Color (0, 0, 0, 0);
		Color endColor = new Color (0, 0, 0, 1);
		
		//Internal elapsed time
		float elapsedTime = 0f;

		//Set fade active
		fade.gameObject.SetActive (true);
		fade.color = startColor;

		//Deactivate selection
		selection.SetActive(false);

		//Lerp color for specified time
		while(fade.color.a != 1)
		{
			fade.color = Color.Lerp(startColor, endColor, 2* elapsedTime);
			elapsedTime+= Time.deltaTime;
			yield return null;
		} //end while

		//Move to next checkpoint
		checkpoint = targetCheckpoint;

		//End fade animation
		playing = false;
		processing = false;
	} //end FadeOutAnimation(int targetCheckpoint)

    /***************************************
     * Name: FadeObjectIn
     * Fades one or more objects in, then moves to the given checkpoint. Assumes objects already exist
     ***************************************/
	IEnumerator FadeObjectIn(Image[] targetObject, int targetCheckpoint)
	{
		//Initialize color value containers
		Color[] startColor = new Color[targetObject.Length];
		Color[] endColor = new Color[targetObject.Length];

		//Fill containers
		for(int i = 0; i < targetObject.Length; i++)
		{
			startColor[i] = targetObject[i].color;
			endColor[i] = new Color(startColor[i].r, startColor[i].g, startColor[i].b, 1);
		} //end foreach

		//Internal elapsed time
		float elapsedTime = 0f;
		
		//Lerp color for specified time
		while(targetObject[targetObject.Length-1].color.a != 1)
		{
			for(int i = 0; i < targetObject.Length; i++)
			{
				targetObject[i].color = Color.Lerp(startColor[i], endColor[i], 2* elapsedTime);
			} //end for
			elapsedTime+= Time.deltaTime;
			yield return null;
		} //end while
		
		//Move to next checkpoint
		checkpoint = targetCheckpoint;
		
		//End fade animation
		playing = false;
		processing = false;
	} //end FadeObjectIn(Image[] targetObject, int targetCheckpoint)

    /***************************************
     * Name: FadeObjectOut
     * Fades one or more objects out, then moves to the given checkpoint. Assumes objects already exist
     ***************************************/
	IEnumerator FadeObjectOut(Image[] targetObject, int targetCheckpoint)
	{
		//Initialize color value containers
		Color[] startColor = new Color[targetObject.Length];
		Color[] endColor = new Color[targetObject.Length];
		
		//Fill containers
		for(int i = 0; i < targetObject.Length; i++)
		{
			startColor[i] = targetObject[i].color;
			endColor[i] = new Color(startColor[i].r, startColor[i].g, startColor[i].b, 0);
		} //end foreach
		
		//Internal elapsed time
		float elapsedTime = 0f;
		
		//Lerp color for specified time
		while(targetObject[targetObject.Length-1].color.a != 0)
		{
			for(int i = 0; i < targetObject.Length; i++)
			{
				targetObject[i].color = Color.Lerp(startColor[i], endColor[i], 2* elapsedTime);
			} //end for
			elapsedTime+= Time.deltaTime;
			yield return null;
		} //end while
		
		//Move to next checkpoint
		checkpoint = targetCheckpoint;
		
		//End fade animation
		playing = false;
		processing = false;
	} //end FadeObjectOut(Image[] targetObject, int targetCheckpoint)
	#endregion

    #region Processing
    /***************************************
     * Name: SetTypeSprites
     * Sets the correct sprite, or disables
     * if a type isn't found.
     ***************************************/
    void SetTypeSprites(Image type1, Image type2, Pokemon teamMember)
    {
        //Set the primary (first) type
        type1.gameObject.SetActive(true);
        type1.sprite = DataContents.typeSprites [Convert.ToInt32 (Enum.Parse (typeof(Types),
            DataContents.ExecuteSQL<string> ("SELECT type1 FROM Pokemon WHERE rowid=" + teamMember.NatSpecies)))];
        
        //Get the string for the secondary type
        string type2SQL = DataContents.ExecuteSQL<string> ("SELECT type2 FROM Pokemon WHERE rowid=" + teamMember.NatSpecies);
        
        //If a second type exists, load the appropriate sprite
        if (!String.IsNullOrEmpty (type2SQL))
        {
            type2.gameObject.SetActive (true);
            type2.sprite = DataContents.typeSprites [Convert.ToInt32 (Enum.Parse (typeof(Types), type2SQL))];
        } //end if
        //Otherwise disable the image
        else
        {
            type2.gameObject.SetActive (false);
        } //end else
    } //end SetTypeSprites(Image type1, Image type2, Pokemon teamMember)

    /***************************************
     * Name: SetStatColor
     * Sets the color for stat ups and downs
     ***************************************/
    void SetStatColor(Pokemon teamMember)
    {
        /*Attack, Defence, Speed, SP Attack, SP Defence*/
        //Get the pokemon's nature
        int currentNature = teamMember.Nature;

        //Find stat up
        int nd5 = (int)Mathf.Floor (currentNature / 5);

        //Find stat down
        int nm5 = (int)Mathf.Floor (currentNature % 5);

        //Get child number of attack
        int childNumber = summaryScreen.transform.GetChild (2).FindChild ("Attack").GetSiblingIndex ();

        //Set stat colors
        for (int i = 0; i < 5; i++)
        {
            //If stat up
            if(i == nd5 && nd5 != nm5)
            {
                summaryScreen.transform.GetChild(2).GetChild(i+childNumber).GetComponent<Text>().color = new Color(0.75f, 0, 0, 1);
            } //end if
            //If stat down
            else if(i == nm5 && nd5 != nm5)
            {
                summaryScreen.transform.GetChild(2).GetChild(i+childNumber).GetComponent<Text>().color = new Color(0, 0, 0.75f, 1);
            } //end else if
            //Otherwise black
            else
            {
                summaryScreen.transform.GetChild(2).GetChild(i+childNumber).GetComponent<Text>().color = Color.black;
            } //end else
        } //end for
    } //end SetStatColor(Pokemon teamMember)

    /***************************************
     * Name: SetMoveSprites
     * Sets the correct sprite, or disables
     * if a move isn't found.
     ***************************************/
    void SetMoveSprites(Pokemon teamMember, Transform moveScreen)
    {
        //Loop through the list of pokemon moves and set each one
        for (int i = 0; i < 4; i++)
        {
            //Make sure move isn't null
            if(teamMember.GetMove(i) != -1)
            {
                //Set the move type
                moveScreen.FindChild("Move" + (i+1).ToString()).gameObject.SetActive(true);
                moveScreen.FindChild("Move" + (i+1).ToString()).GetChild(0).GetComponent<Image>().sprite = 
                    DataContents.typeSprites [Convert.ToInt32 (Enum.Parse (typeof(Types),
                    DataContents.ExecuteSQL<string> ("SELECT type FROM Moves WHERE rowid=" +
                    teamMember.GetMove(i))))];
                //Set the move name
                moveScreen.FindChild("Move" + (i+1).ToString()).GetChild(1).GetComponent<Text>().text = 
                    DataContents.ExecuteSQL<string> ("SELECT gameName FROM Moves WHERE rowid=" +
                    teamMember.GetMove(i));

                //Set the move PP
                moveScreen.FindChild("Move" + (i+1).ToString()).GetChild(2).GetComponent<Text>().text = "PP " +
                    teamMember.GetMovePP(i).ToString() + "/" +
                    DataContents.ExecuteSQL<string> ("SELECT totalPP FROM Moves WHERE rowid=" +
                    teamMember.GetMove(i));
            } //end if
            else
            {
                //Blank out the type
                moveScreen.FindChild("Move" + (i+1).ToString()).gameObject.SetActive(false);
            } //end else
        } //end for
    } //end SetMoveSprites(Pokemon teamMember, Transform moveScreen)

    /***************************************
     * Name: SetMoveDetails
     * Sets the details of the move
     ***************************************/
    void SetMoveDetails(Pokemon teamMember, Transform moveScreen)
    {
        //Reposition selection rect
        selection.SetActive(true);
        
        //Resize to same as top choice
        Vector3 scale = new Vector3(moveScreen.FindChild("Move"+(moveChoice+1)).GetComponent<RectTransform>().rect.width,
                                    moveScreen.FindChild("Move"+(moveChoice+1)).GetComponent<RectTransform>().rect.height,
                                    0);
        selection.GetComponent<RectTransform>().sizeDelta = scale;

        //Reposition to location of top choice, with 2 unit offset to center it
        selection.transform.position = Camera.main.WorldToScreenPoint (currentMoveSlot.transform.
                                                                       position);

        //Set the move category
        moveScreen.FindChild("Category").GetComponent<Image>().sprite = 
            DataContents.categorySprites [Convert.ToInt32 (Enum.Parse (typeof(Categories),
            DataContents.ExecuteSQL<string> ("SELECT category FROM Moves WHERE rowid=" +
            teamMember.GetMove(moveChoice))))];

        //Set the move power
        int temp = DataContents.ExecuteSQL<int> ("SELECT baseDamage FROM Moves WHERE rowid=" +
                   teamMember.GetMove(moveChoice));
        moveScreen.FindChild ("Power").GetComponent<Text> ().text = temp > 1 ? temp.ToString () : "---";
            
        //Set the move accuracy
        temp = DataContents.ExecuteSQL<int> ("SELECT accuracy FROM Moves WHERE rowid=" +
               teamMember.GetMove(moveChoice));
        moveScreen.FindChild("Accuracy").GetComponent<Text>().text = temp >= 1 ? temp.ToString () : "---";

        //Set font size of move description
        if (detailsSize != -1)
        {
            //Set the move description text
            moveScreen.FindChild ("MoveDescription").GetComponent<Text> ().text = 
                DataContents.ExecuteSQL<string> ("SELECT description FROM Moves WHERE rowid=" +
                teamMember.GetMove (moveChoice));
        } //end if
        else
        {
            //Get font size
            moveScreen.FindChild ("MoveDescription").GetComponent<Text> ().text = 
                DataContents.ExecuteSQL<string> ("SELECT description FROM Moves WHERE gameName='Rollout'");
            StartCoroutine (WaitForFontResize (moveScreen, teamMember));
        } //end else
    } //end SetMoveDetails(Pokemon teamMember, Transform moveScreen)

    /***************************************
     * Name: PokemonSummary
     * Sets summary screen details for each page
     ***************************************/
    void PokemonSummary(Pokemon pokemonChoice)
    {
        switch(summaryChoice)
        {
            //Info screen
            case 0:
            {
                summaryScreen.transform.GetChild(0).gameObject.SetActive(true);
                summaryScreen.transform.GetChild(0).FindChild("Name").GetComponent<Text>().text=
                    pokemonChoice.Nickname;
                summaryScreen.transform.GetChild(0).FindChild("Level").GetComponent<Text>().text=
                    pokemonChoice.CurrentLevel.ToString();
                summaryScreen.transform.GetChild(0).FindChild("Ball").GetComponent<Image>().sprite=
                    Resources.Load<Sprite>("Sprites/Icons/summaryBall"+pokemonChoice.BallUsed.ToString("00"));
                summaryScreen.transform.GetChild(0).FindChild("Gender").GetComponent<Image>().sprite=
                    Resources.Load<Sprite>("Sprites/Icons/gender"+pokemonChoice.Gender.ToString());
                summaryScreen.transform.GetChild(0).FindChild("Sprite").GetComponent<Image>().sprite=
                    Resources.Load<Sprite>("Sprites/Pokemon/"+pokemonChoice.NatSpecies.ToString("000"));
                summaryScreen.transform.GetChild(0).FindChild("Markings").GetComponent<Text>().text=
                    pokemonChoice.GetMarkingsString();
                summaryScreen.transform.GetChild(0).FindChild("Item").GetComponent<Text>().text=
                    DataContents.GetItemGameName(pokemonChoice.Item);
                summaryScreen.transform.GetChild(0).FindChild("DexNumber").GetComponent<Text>().text=
                    pokemonChoice.NatSpecies.ToString();
                summaryScreen.transform.GetChild(0).FindChild("Species").GetComponent<Text>().text=
                    DataContents.ExecuteSQL<String>("SELECT name FROM Pokemon WHERE rowid=" +
                    pokemonChoice.NatSpecies); 
                summaryScreen.transform.GetChild(0).FindChild("OT").GetComponent<Text>().text=
                    pokemonChoice.OTName;
                summaryScreen.transform.GetChild(0).FindChild("IDNumber").GetComponent<Text>().text=
                    pokemonChoice.TrainerID.ToString();
                summaryScreen.transform.GetChild(0).FindChild("CurrentXP").GetComponent<Text>().text=
                    pokemonChoice.CurrentEXP.ToString();
                summaryScreen.transform.GetChild(0).FindChild("RemainingXP").GetComponent<Text>().text=
                    pokemonChoice.RemainingEXP.ToString();
                SetTypeSprites(summaryScreen.transform.GetChild(0).FindChild("Types").GetChild(0).GetComponent<Image>(),
                    summaryScreen.transform.GetChild(0).FindChild("Types").GetChild(1).GetComponent<Image>(), pokemonChoice);
                break;
            } //end case 0 (Info)
            //Memo screen
            case 1:
            {
                summaryScreen.transform.GetChild(1).gameObject.SetActive(true);
                summaryScreen.transform.GetChild(1).FindChild("Name").GetComponent<Text>().text=
                    pokemonChoice.Nickname;
                summaryScreen.transform.GetChild(1).FindChild("Level").GetComponent<Text>().text=
                    pokemonChoice.CurrentLevel.ToString();
                summaryScreen.transform.GetChild(1).FindChild("Ball").GetComponent<Image>().sprite=
                    Resources.Load<Sprite>("Sprites/Icons/summaryBall"+pokemonChoice.BallUsed.ToString("00"));
                summaryScreen.transform.GetChild(1).FindChild("Gender").GetComponent<Image>().sprite=
                    Resources.Load<Sprite>("Sprites/Icons/gender"+pokemonChoice.Gender.ToString());
                summaryScreen.transform.GetChild(1).FindChild("Sprite").GetComponent<Image>().sprite=
                    Resources.Load<Sprite>("Sprites/Pokemon/"+pokemonChoice.NatSpecies.ToString("000"));
                summaryScreen.transform.GetChild(1).FindChild("Markings").GetComponent<Text>().text=
                    pokemonChoice.GetMarkingsString();
                summaryScreen.transform.GetChild(1).FindChild("Item").GetComponent<Text>().text=
                    DataContents.GetItemGameName(pokemonChoice.Item);
                summaryScreen.transform.GetChild(1).FindChild("Nature").GetComponent<Text>().text= 
                    "<color=#cc0000ff>" + ((Natures)pokemonChoice.
                    Nature).ToString() + "</color> nature";
                summaryScreen.transform.GetChild(1).FindChild("CaughtDate").GetComponent<Text>().text=
                    pokemonChoice.ObtainTime.ToLongDateString() + 
                    " at " + pokemonChoice.ObtainTime.
                    ToShortTimeString();
                summaryScreen.transform.GetChild(1).FindChild("CaughtType").GetComponent<Text>().text=
                    ((ObtainType)pokemonChoice.ObtainType).ToString()
                    + " from " +
                    ((ObtainFrom)pokemonChoice.ObtainFrom).ToString();
                summaryScreen.transform.GetChild(1).FindChild("CaughtLevel").GetComponent<Text>().text=
                    "Found at level " + pokemonChoice.ObtainLevel;
                break;
            } //end case 1 (Memo)
            //Stats
            case 2:
            {
                summaryScreen.transform.GetChild(2).gameObject.SetActive(true);
                summaryScreen.transform.GetChild(2).FindChild("Name").GetComponent<Text>().text=
                    pokemonChoice.Nickname;
                summaryScreen.transform.GetChild(2).FindChild("Level").GetComponent<Text>().text=
                    pokemonChoice.CurrentLevel.ToString();
                summaryScreen.transform.GetChild(2).FindChild("Ball").GetComponent<Image>().sprite=
                    Resources.Load<Sprite>("Sprites/Icons/summaryBall"+pokemonChoice.BallUsed.ToString("00"));
                summaryScreen.transform.GetChild(2).FindChild("Gender").GetComponent<Image>().sprite=
                    Resources.Load<Sprite>("Sprites/Icons/gender"+pokemonChoice.Gender.ToString());
                summaryScreen.transform.GetChild(2).FindChild("Sprite").GetComponent<Image>().sprite=
                    Resources.Load<Sprite>("Sprites/Pokemon/"+pokemonChoice.NatSpecies.ToString("000"));
                summaryScreen.transform.GetChild(2).FindChild("Markings").GetComponent<Text>().text=
                    pokemonChoice.GetMarkingsString();
                summaryScreen.transform.GetChild(2).FindChild("Item").GetComponent<Text>().text=
                    DataContents.GetItemGameName(pokemonChoice.Item);
                summaryScreen.transform.GetChild(2).FindChild("HP").GetComponent<Text>().text=
                    pokemonChoice.CurrentHP.ToString() +
                    "/" + pokemonChoice.TotalHP.ToString();
                summaryScreen.transform.GetChild(2).FindChild("RemainingHP").
                    GetComponent<RectTransform>().localScale = new Vector3((float)pokemonChoice.CurrentHP/
                    (float)pokemonChoice.TotalHP, 1f, 1f);
                summaryScreen.transform.GetChild(2).FindChild("Attack").GetComponent<Text>().text=
                    pokemonChoice.Attack.ToString();
                summaryScreen.transform.GetChild(2).FindChild("Defense").GetComponent<Text>().text=
                    pokemonChoice.Defense.ToString();
                summaryScreen.transform.GetChild(2).FindChild("SpAttack").GetComponent<Text>().text=
                    pokemonChoice.SpecialA.ToString();
                summaryScreen.transform.GetChild(2).FindChild("SpDefense").GetComponent<Text>().text=
                    pokemonChoice.SpecialD.ToString();
                summaryScreen.transform.GetChild(2).FindChild("Speed").GetComponent<Text>().text=
                    pokemonChoice.Speed.ToString();
                summaryScreen.transform.GetChild(2).FindChild("AbilityName").GetComponent<Text>().text=
                    pokemonChoice.GetAbilityName();
                summaryScreen.transform.GetChild(2).FindChild("AbilityDescription").GetComponent<Text>().text=
                    pokemonChoice.GetAbilityDescription();
                SetStatColor(pokemonChoice);
                break;
            } //end case 2 (Stats)
            //EV-IV
            case 3:
            {
                summaryScreen.transform.GetChild(3).gameObject.SetActive(true);
                summaryScreen.transform.GetChild(3).FindChild("Name").GetComponent<Text>().text=
                    pokemonChoice.Nickname;
                summaryScreen.transform.GetChild(3).FindChild("Level").GetComponent<Text>().text=
                    pokemonChoice.CurrentLevel.ToString();
                summaryScreen.transform.GetChild(3).FindChild("Ball").GetComponent<Image>().sprite=
                    Resources.Load<Sprite>("Sprites/Icons/summaryBall"+pokemonChoice.BallUsed.ToString("00"));
                summaryScreen.transform.GetChild(3).FindChild("Gender").GetComponent<Image>().sprite=
                    Resources.Load<Sprite>("Sprites/Icons/gender"+pokemonChoice.Gender.ToString());
                summaryScreen.transform.GetChild(3).FindChild("Sprite").GetComponent<Image>().sprite=
                    Resources.Load<Sprite>("Sprites/Pokemon/"+pokemonChoice.NatSpecies.ToString("000"));
                summaryScreen.transform.GetChild(3).FindChild("Markings").GetComponent<Text>().text=
                    pokemonChoice.GetMarkingsString();
                summaryScreen.transform.GetChild(3).FindChild("Item").GetComponent<Text>().text=
                    DataContents.GetItemGameName(pokemonChoice.Item);
                summaryScreen.transform.GetChild(3).FindChild("HP").GetComponent<Text>().text=
                    pokemonChoice.GetEV(0).ToString() +
                    "/" + pokemonChoice.GetIV(0).ToString();
                summaryScreen.transform.GetChild(3).FindChild("RemainingHP").
                    GetComponent<RectTransform>().localScale = new Vector3((float)pokemonChoice.CurrentHP/
                    (float)pokemonChoice.TotalHP, 1f, 1f);
                summaryScreen.transform.GetChild(3).FindChild("Attack").GetComponent<Text>().text=
                    pokemonChoice.GetEV(1).ToString() +
                    "/" + pokemonChoice.GetIV(1).ToString();
                summaryScreen.transform.GetChild(3).FindChild("Defense").GetComponent<Text>().text=
                    pokemonChoice.GetEV(2).ToString() +
                    "/" + pokemonChoice.GetIV(2).ToString();
                summaryScreen.transform.GetChild(3).FindChild("SpAttack").GetComponent<Text>().text=
                    pokemonChoice.GetEV(4).ToString() +
                    "/" + pokemonChoice.GetIV(4).ToString();
                summaryScreen.transform.GetChild(3).FindChild("SpDefense").GetComponent<Text>().text=
                    pokemonChoice.GetEV(5).ToString() +
                    "/" + pokemonChoice.GetIV(5).ToString();
                summaryScreen.transform.GetChild(3).FindChild("Speed").GetComponent<Text>().text=
                    pokemonChoice.GetEV(3).ToString() +
                    "/" + pokemonChoice.GetIV(3).ToString();
                summaryScreen.transform.GetChild(3).FindChild("AbilityName").GetComponent<Text>().text=
                    pokemonChoice.GetAbilityName();
                summaryScreen.transform.GetChild(3).FindChild("AbilityDescription").GetComponent<Text>().text=
                    pokemonChoice.GetAbilityDescription();
                break;
            } //end case 3 (EV-IV)
            //Moves
            case 4:
            {
                summaryScreen.transform.GetChild(4).gameObject.SetActive(true);
                summaryScreen.transform.GetChild(4).FindChild("Name").GetComponent<Text>().text=
                    pokemonChoice.Nickname;
                summaryScreen.transform.GetChild(4).FindChild("Level").GetComponent<Text>().text=
                    pokemonChoice.CurrentLevel.ToString();
                summaryScreen.transform.GetChild(4).FindChild("Ball").GetComponent<Image>().sprite=
                    Resources.Load<Sprite>("Sprites/Icons/summaryBall"+pokemonChoice.BallUsed.ToString("00"));
                summaryScreen.transform.GetChild(4).FindChild("Gender").GetComponent<Image>().sprite=
                    Resources.Load<Sprite>("Sprites/Icons/gender"+pokemonChoice.Gender.ToString());
                summaryScreen.transform.GetChild(4).FindChild("Sprite").GetComponent<Image>().sprite=
                    Resources.Load<Sprite>("Sprites/Pokemon/"+pokemonChoice.NatSpecies.ToString("000"));
                summaryScreen.transform.GetChild(4).FindChild("Markings").GetComponent<Text>().text=
                    pokemonChoice.GetMarkingsString();
                summaryScreen.transform.GetChild(4).FindChild("Item").GetComponent<Text>().text=
                    DataContents.GetItemGameName(pokemonChoice.Item);
                SetMoveSprites(pokemonChoice, summaryScreen.transform.GetChild(4));
                break;
            } //end case 4 (Moves)
            //Move Details
            case 5:
            {
                summaryScreen.transform.GetChild(5).gameObject.SetActive(true);
                summaryScreen.transform.GetChild(5).FindChild("Sprite").GetComponent<Image>().sprite=
                    Resources.Load<Sprite>("Sprites/Icons/icon"+pokemonChoice.NatSpecies.ToString("000"));
                SetMoveDetails(pokemonChoice, summaryScreen.transform.GetChild(5));
                SetTypeSprites(summaryScreen.transform.GetChild(5).FindChild("SpeciesTypes").GetChild(0).GetComponent<Image>(),
                    summaryScreen.transform.GetChild(5).FindChild("SpeciesTypes").GetChild(1).GetComponent<Image>(), 
                    pokemonChoice);
                SetMoveSprites(pokemonChoice, summaryScreen.transform.GetChild(5));
                break;
            } //end case 5 (Move Details)
        } //end switch
    } //end PokemonSummary

    /***************************************
     * Name: ReadRibbon
     * Sets ribbon details based on ribbon selection
     ***************************************/
    public void ReadRibbon()
    {
        //If text isn't displayed
        if ((gameState == MainGame.POKEMONRIBBONS|| pcState == PCGame.POKEMONRIBBONS) && 
            ribbonChoice != previousRibbonChoice && selection.activeSelf)
        {
            //Activate the fields
            ribbonScreen.transform.FindChild("RibbonName").gameObject.SetActive(true);
            ribbonScreen.transform.FindChild("RibbonDescription").gameObject.SetActive(true);

            //Position selection rectangle
            selection.transform.position = Camera.main.WorldToScreenPoint(ribbonScreen.transform.
                FindChild("RibbonRegion").GetChild(ribbonChoice).GetComponent<RectTransform>().position);

            //Get the ribbon value at the index
            int ribbonValue;
            if(gameState == MainGame.POKEMONRIBBONS)
            {
                ribbonValue = GameManager.instance.GetTrainer().Team[choiceNumber-1].GetRibbon(ribbonChoice);
            } //end if
            else
            {
                ribbonValue = selectedPokemon.GetRibbon(ribbonChoice);
            } //end else
            //Set the name and description
            ribbonScreen.transform.FindChild("RibbonName").GetComponent<Text>().text = 
                DataContents.ribbonData.GetRibbonName(ribbonValue);
            ribbonScreen.transform.FindChild("RibbonDescription").GetComponent<Text>().text = 
                DataContents.ribbonData.GetRibbonDescription(ribbonValue);

            //Set the ribbon choice
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
            detailsRegion.transform.FindChild ("Sprite").GetComponent<Image> ().sprite = 
                Resources.Load<Sprite> ("Sprites/Pokemon/" + selectedPokemon.NatSpecies.ToString ("000"));
            detailsRegion.transform.FindChild ("Markings").GetComponent<Text> ().text = selectedPokemon.
                GetMarkingsString();
            detailsRegion.transform.FindChild ("Shiny").GetComponent<Text> ().color = 
                selectedPokemon.IsShiny ? Color.white : Color.clear;
            detailsRegion.transform.FindChild ("Ability").GetComponent<Text> ().text = selectedPokemon.
                GetAbilityName ();
            detailsRegion.transform.FindChild ("Item").GetComponent<Text> ().text = 
                DataContents.GetItemGameName (selectedPokemon.Item);
            detailsRegion.transform.FindChild ("Level").GetComponent<Text> ().text = "Lv. " + 
                selectedPokemon.CurrentLevel.ToString ();
            SetTypeSprites (detailsRegion.transform.FindChild ("Types").GetChild (0).GetComponent<Image> (),
                detailsRegion.transform.FindChild ("Types").GetChild (1).GetComponent<Image> (),
                selectedPokemon);
        } //end if
        else
        {
            detailsRegion.transform.FindChild ("Name").GetComponent<Text> ().text = "";
            detailsRegion.transform.FindChild ("Gender").GetComponent<Image> ().color = Color.clear;
            detailsRegion.transform.FindChild ("Sprite").GetComponent<Image> ().color = Color.clear;
            detailsRegion.transform.FindChild ("Markings").GetComponent<Text> ().text = "";
            detailsRegion.transform.FindChild ("Shiny").GetComponent<Text> ().color = Color.clear;
            detailsRegion.transform.FindChild ("Level").GetComponent<Text> ().text = "";
            detailsRegion.transform.FindChild ("Types").GetChild (0).gameObject.SetActive (false);
            detailsRegion.transform.FindChild ("Types").GetChild (1).gameObject.SetActive (false);
            detailsRegion.transform.FindChild ("Ability").GetComponent<Text> ().text = "";
            detailsRegion.transform.FindChild ("Item").GetComponent<Text> ().text = "";
        } //end else
    } //end FillDetails

    /***************************************
     * Name: WaitForResize
     * Waits for choice menu to resize before 
     * setting selection dimensions
     ***************************************/
    IEnumerator WaitForResize()
    {
        yield return new WaitForEndOfFrame ();
        if (gameState == MainGame.POKEMONSUBMENU || pcState == PCGame.POKEMONSUBMENU)
        {
            Vector3 scale = new Vector3 (choices.GetComponent<RectTransform> ().rect.width,
                choices.GetComponent<RectTransform> ().rect.height /
                choices.transform.childCount, 0);
            selection.GetComponent<RectTransform> ().sizeDelta = scale;
            selection.transform.position = choices.transform.GetChild (0).
                GetComponent<RectTransform> ().position;
            selection.SetActive (true);
        } //end if
        else if (pcState == PCGame.POKEMONMARKINGS)
        {
            //Reposition choices to bottom right
            choices.GetComponent<RectTransform>().position = new Vector3(
                choices.GetComponent<RectTransform>().position.x,
                choices.GetComponent<RectTransform>().rect.height/2);

            Vector3 scale = new Vector3 (choices.GetComponent<RectTransform> ().rect.width,
                choices.GetComponent<RectTransform> ().rect.height /
                choices.transform.childCount, 0);
            selection.GetComponent<RectTransform> ().sizeDelta = scale;
            selection.transform.position = choices.transform.GetChild (0).
                GetComponent<RectTransform> ().position;
            selection.SetActive (true);
        } //end else if
        else if (gameState == MainGame.POKEMONRIBBONS || pcState == PCGame.POKEMONRIBBONS)
        {
            Vector3 scale = new Vector3(ribbonScreen.transform.FindChild("RibbonRegion").GetChild(0).
                                        GetComponent<RectTransform>().rect.width,
                                        ribbonScreen.transform.FindChild("RibbonRegion").GetChild(0).
                                        GetComponent<RectTransform>().rect.height
                                        , 0);
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
    IEnumerator WaitForFontResize(Transform moveScreen, Pokemon teamMember)
    {
        yield return new WaitForEndOfFrame ();
        detailsSize = moveScreen.FindChild("MoveDescription").GetComponent<Text>().cachedTextGenerator.
            fontSizeUsedForBestFit;
        moveScreen.FindChild("MoveDescription").GetComponent<Text>().resizeTextForBestFit = false;
        moveScreen.FindChild("MoveDescription").GetComponent<Text>().fontSize = detailsSize;
        moveScreen.FindChild ("MoveDescription").GetComponent<Text> ().text = 
            DataContents.ExecuteSQL<string> ("SELECT description FROM Moves WHERE rowid=" +
            teamMember.GetMove (moveChoice));
    } //end WaitForFontResize(Transform moveScreen, Pokemon teamMember)

    /***************************************
     * Name: GatherInput
     * Gather user input and set variables 
     * as necessary
     ***************************************/
    void GatherInput()
    {
        /***********************************************
         * Left Arrow
         ***********************************************/ 
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //Scene switch
            switch(sceneState)
            {
                //Intro
                case OverallGame.INTRO:
                {
                    break;
                } //end case OverallGame INTRO

                //Menu
                case OverallGame.MENU:
                {
                    break;
                } //end case OverallGame MENU

                //New Game
                case OverallGame.NEWGAME:
                {
                    break;
                } //end case OverallGame NEWGAME

                //Main Game scene
                case OverallGame.CONTINUE:
                {
                    //Pokemon Summary on Continue Game -> Summary
                    if(gameState == MainGame.POKEMONSUMMARY)
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
                    } //end if Pokemon Summary on Continue Game -> Summary

                    //Continue Game -> My Team
                    else if(gameState == MainGame.TEAM)
                    {
                        //Decrease (higher slots are lower childs)
                        choiceNumber--;

                        //Clamp between -1 and team size
                        if(choiceNumber < -1)
                        {
                            choiceNumber = GameManager.instance.GetTrainer().Team.Count;
                        } //end if

                        //Set currentSlotChoice
                        if(choiceNumber > 0)
                        {
                            currentTeamSlot = playerTeam.transform.FindChild("Pokemon"+choiceNumber).gameObject;
                        } //end if
                        else if(choiceNumber == 0)
                        {
                            currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(1).gameObject;
                        } //end else if
                        else if(choiceNumber == -1)
                        {
                            currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(0).gameObject;
                        } //end else if
                    } //end else if Continue Game -> My Team

                    //Pokemon Switch on Continue Game -> Switch
                    else if(gameState == MainGame.POKEMONSWITCH)
                    {
                        //Decrease (higher slots are lower childs)
                        switchChoice--;
                        
                        //Clamp between 1 and team size
                        if(switchChoice < 1)
                        {
                            switchChoice = GameManager.instance.GetTrainer().Team.Count;
                        } //end if

                        //Set currentSwitchSlot
                        currentSwitchSlot = playerTeam.transform.FindChild("Pokemon"+switchChoice).gameObject;
                    } //end else if Pokemon Switch on Continue Game -> Switch

                    //Pokemon Ribbons on Continue Game -> Ribbons
                    else if(gameState == MainGame.POKEMONRIBBONS)
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
                    } //end else if Pokemon Ribbons on Continue Game -> Ribbons
                    break;
                } //end case OverallState CONTINUEGAME
                //PC
                case OverallGame.PC:
                {
                    //PC Home
                    if(pcState == PCGame.HOME)
                    {
                        //If on box title
                        if(boxChoice == -2)
                        {
                            boxChoice = -3;
                        } //end if
                        //If on top left slot
                        else if(boxChoice == 0)
                        {
                            boxChoice = -2;
                        } //end else if
                        //Otherwise move left
                        else
                        {
                            //Decrease (higher slots on lower children)
                            boxChoice--;
                        } //end else
                    } //end if Home

                    //Pokemon Party on PC -> Party Tab
                    else if(pcState == PCGame.PARTY)
                    {
                        //Decrease (higher slots are lower childs)
                        choiceNumber--;
                        
                        //Clamp at 0
                        if(choiceNumber < 0)
                        {
                            //If there is a held pokemon
                            if(heldPokemon != null && GameManager.instance.GetTrainer().Team.Count < 6)
                            {
                                choiceNumber = GameManager.instance.GetTrainer().Team.Count + 1;
                            } //end if
                            else
                            {
                                choiceNumber = GameManager.instance.GetTrainer().Team.Count;
                            } //end else
                        } //end if
                        
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
                            
                            //Decrease choice
                            summaryChoice--;
                            
                            //Loop to last child if on first child
                            if(summaryChoice < 0)
                            {
                                summaryChoice = 4;
                            } //end if
                        } //end if
                    } //end else if Pokemon Summary on PC -> Summary

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
                    break;
                } //end case OverallState PC
            } //end scene switch
        } //end if Left Arrow

        /***********************************************
         * Right Arrow
         ***********************************************/ 
        else if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            //Scene switch
            switch(sceneState)
            {
                //Intro
                case OverallGame.INTRO:
                {
                    break;
                } //end case OverallGame INTRO
                    
                //Menu
                case OverallGame.MENU:
                {
                    break;
                } //end case OverallGame MENU
                    
                //New Game
                case OverallGame.NEWGAME:
                {
                    break;
                } //end case OverallGame NEWGAME
                    
                //Main Game scene
                case OverallGame.CONTINUE:
                {
                    //Pokemon Summary on Continue Game -> Summary
                    if(gameState == MainGame.POKEMONSUMMARY)
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
                    } //end if Pokemon Summary on Continue Game -> Summary

                    //Continue Game -> My Team
                    else if(gameState == MainGame.TEAM)
                    {
                        //Increase (lower slots are higher children)
                        choiceNumber++;

                        //Clamp between -1 and team size
                        if(choiceNumber > GameManager.instance.GetTrainer().Team.Count)
                        {
                            choiceNumber = -1;
                        } //end if

                        //Set currentSlotChoice
                        if(choiceNumber > 0)
                        {
                            currentTeamSlot = playerTeam.transform.FindChild("Pokemon"+choiceNumber).gameObject;
                        } //end if
                        else if(choiceNumber == 0)
                        {
                            currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(1).gameObject;
                        } //end else if
                        else if(choiceNumber == -1)
                        {
                            currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(0).gameObject;
                        } //end else if

                    } //end else if Pokemon submenu on Continue Game -> My Team

                    //Pokemon Switch on Continue Game -> Switch
                    else if(gameState == MainGame.POKEMONSWITCH)
                    {
                        //Increase (lower slots are higher childs)
                        switchChoice++;
                        
                        //Clamp between 1 and team size
                        if(switchChoice > GameManager.instance.GetTrainer().Team.Count)
                        {
                            switchChoice = 1;
                        } //end if

                        //Set currentSwitchSlot
                        currentSwitchSlot = playerTeam.transform.FindChild("Pokemon"+switchChoice).gameObject;
                    } //end else if Pokemon Switch on Continue Game -> Switch

                    //Pokemon Ribbons on Continue Game -> Ribbons
                    else if(gameState == MainGame.POKEMONRIBBONS)
                    {
                        //Increase (lower slots are higher childs)
                        ribbonChoice++;
                        
                        //Clamp at ribbon length
                        if(ribbonChoice >= GameManager.instance.GetTrainer().Team[choiceNumber-1].GetRibbonCount())
                        {
                            ribbonChoice = ExtensionMethods.ClampToZero(
                                GameManager.instance.GetTrainer().Team[choiceNumber-1].GetRibbonCount()-1);
                            previousRibbonChoice = -1;
                        } //end if
                        
                        //Set currentRibbonSlot
                        currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
                            GetChild(ribbonChoice).gameObject;

                        //Read ribbon
                        ReadRibbon();
                    } //end else if Pokemon Ribbons on Continue Game -> Ribbons
                    break;
                } // end case OverallGame CONTINUEGAME

                //PC
                case OverallGame.PC:
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
                            ribbonChoice = ExtensionMethods.ClampToZero(selectedPokemon.GetRibbonCount()-1);
                            previousRibbonChoice = -1;
                        } //end if
                        
                        //Set currentRibbonSlot
                        currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
                            GetChild(ribbonChoice).gameObject;
                        
                        //Read ribbon
                        ReadRibbon();
                    } //end else if Pokemon Ribbons on PC -> Ribbons
                    break;
                } //end case OverallState PC
            } //end scene switch
        } //end else if Right Arrow

        /***********************************************
         * Up Arrow
         ***********************************************/
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            //Scene switch
            switch(sceneState)
            {
                //Intro
                case OverallGame.INTRO:
                {
                    break;
                } //end case OverallGame INTRO
                    
                //Menu
                case OverallGame.MENU:
                {
                    if(checkpoint == 2)
                    {
                        //Decrease choice to show previous option is highlighted
                        choiceNumber--;
                        //Loop to last choice if it's lower than zero
                        if(choiceNumber < 0)
                        {
                            choiceNumber = mChoices.transform.childCount-1;
                        } //end if
                        
                        //Resize to choice width
                        selection.GetComponent<RectTransform>().sizeDelta = 
                            new Vector2(transforms[choiceNumber].sizeDelta.x, transforms[choiceNumber].sizeDelta.y/3);
                        
                        //Reposition to choice location
                        selection.transform.position = 
                            new Vector3(mChoices.transform.GetChild(choiceNumber).transform.position.x, 
                                        mChoices.transform.GetChild(choiceNumber).transform.position.y-1,
                                        100);
                    } //end if
                    else if(checkpoint == 4)
                    {
                        //Decrease choice to show previous option is highlighted
                        choiceNumber--;
                        //Loop to last choice if it's lower than zero
                        if(choiceNumber < 1)
                        {
                            choiceNumber = mChoices.transform.childCount-1;
                        } //end if
                        
                        //Resize to choice width
                        selection.GetComponent<RectTransform>().sizeDelta = 
                            new Vector2(transforms[choiceNumber].sizeDelta.x, transforms[choiceNumber].sizeDelta.y/3);
                        
                        //Reposition to choice location
                        selection.transform.position = 
                            new Vector3(mChoices.transform.GetChild(choiceNumber).transform.position.x, 
                                        mChoices.transform.GetChild(choiceNumber).transform.position.y-2,
                                        100);
                    } //end else if
                    break;
                } //end case OverallGame MENU
                    
                //New Game
                case OverallGame.NEWGAME:
                {
                    //If up arrow is pressed on confirm box
                    if(checkpoint == 8)
                    {
                        //Decrease choice to show previous option is highlighted
                        choiceNumber--;
                        //Loop to last choice if it's lower than zero
                        if(choiceNumber < 0)
                        {
                            choiceNumber = 1;
                        } //end if
                        
                        //Resize to choice width
                        selection.transform.position = new Vector3(
                            confirm.transform.GetChild(choiceNumber).position.x,
                            confirm.transform.GetChild(choiceNumber).position.y, 100);
                    } //end if
                    break;
                } //end case OverallGame NEWGAME
                    
                //Main Game scene
                case OverallGame.CONTINUE:
                {
                    //Pokemon submenu on Continue Game -> My Team
                    if(gameState == MainGame.POKEMONSUBMENU)
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
                    } //end if Pokemon submenu on Continue Game -> My Team
                    
                    //Continue Game -> My Team
                    else if(gameState == MainGame.TEAM)
                    {
                        //Move from top slot to Cancel button
                        if(choiceNumber == 1 || choiceNumber == 2)
                        {
                            choiceNumber = 0;
                            currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(1).gameObject;
                        } //end if
                        //Move from PC button to last team slot
                        else if(choiceNumber == -1)
                        {
                            choiceNumber = GameManager.instance.GetTrainer().Team.Count;
                            currentTeamSlot = playerTeam.transform.FindChild("Pokemon"+choiceNumber).gameObject;
                        } //end else if
                        //Move from Cancel button to PC button
                        else if(choiceNumber == 0)
                        {
                            choiceNumber = -1;
                            currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(0).gameObject;
                        } //end else if
                        //Go up vertically
                        else
                        {
                            choiceNumber -= 2;
                            currentTeamSlot = playerTeam.transform.FindChild("Pokemon"+choiceNumber).gameObject;
                        } //end else
                    } //end else if Continue Game -> My Team

                    //Pokemon Summary on Continue Game -> Summary
                    else if(gameState == MainGame.POKEMONSUMMARY)
                    {
                        //If on any page besides move details
                        if(summaryChoice != 5)
                        {
                            //Decrease (higher slots are lower childs)
                            choiceNumber--;
                            
                            //Clamp between 1 and team size
                            if(choiceNumber < 1)
                            {
                                choiceNumber = GameManager.instance.GetTrainer().Team.Count;
                            } //end if
                        } //end if
                        else
                        {
                            //Decrease (higher slots are lower childs)
                            moveChoice--;

                            //Clamp between 0 and highest non-null move
                            if(moveChoice < 0)
                            {
                                moveChoice = GameManager.instance.GetTrainer().Team[choiceNumber-1].GetMoveCount()-1;
                            } //end if

                            //Set move slot
                            currentMoveSlot = summaryScreen.transform.GetChild(5).
                                FindChild("Move"+(moveChoice+1)).gameObject;
                        } //end else
                    } //end else if Pokemon Summary on Continue Game -> Summary

                    //Pokemon Switch on Continue Game -> Switch
                    else if(gameState == MainGame.POKEMONSWITCH)
                    {
                        //Move from top slot to last slot
                        if(switchChoice <= 2)
                        {
                            switchChoice = GameManager.instance.GetTrainer().Team.Count;
                        } //end else if
                        //Go up vertically
                        else
                        {
                            switchChoice -= 2;
                        } //end else

                        //Set currentSwitchSlot
                        currentSwitchSlot = playerTeam.transform.FindChild("Pokemon"+switchChoice).gameObject;
                    } //end else if Pokemon Switch on Continue Game -> Switch

                    //Move Switch on Continue Game -> Summary -> Move Details
                    else if(gameState  == MainGame.MOVESWITCH)
                    {
                        //Decrease (higher slots on lower children)
                        switchChoice--;

                        //Clamp between 0 and highest non-null move
                        if(switchChoice < 0)
                        {
                            switchChoice = GameManager.instance.GetTrainer().Team[choiceNumber-1].GetMoveCount()-1;
                        } //end if

                        //Set currentSwitchSlot
                        currentSwitchSlot = summaryScreen.transform.GetChild(5).
                            FindChild("Move"+(switchChoice+1)).gameObject;
                    } //end else if Move Switch on Continue Game -> Summary -> Move Details

                    //Pokemon Ribbons on Continue Game -> Ribbons
                    else if(gameState == MainGame.POKEMONRIBBONS)
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
                    } //end else if Pokemon Ribbons on Continue Game -> Ribbons
                    break;
                } //end case OverallGame CONTINUEGAME

                //PC
                case OverallGame.PC:
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
                    break;
                } //end case OverallState PC
            } //end scene switch
        }//end else if Up Arrow

        /***********************************************
         * Down Arrow
         ***********************************************/ 
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            //Scene switch
            switch(sceneState)
            {
                //Intro
                case OverallGame.INTRO:
                {
                    break;
                } //end case OverallGame INTRO
                    
                //Menu
                case OverallGame.MENU:
                {
                    if(checkpoint == 2)
                    {
                        //Increase choice to show next option is highlighted
                        choiceNumber++;
                        //Loop back to start if it's higher than the amount of choices
                        if(choiceNumber >= mChoices.transform.childCount)
                        {
                            choiceNumber = 0;
                        } //end if
                        
                        //Resize to choice width
                        selection.GetComponent<RectTransform>().sizeDelta = 
                            new Vector2(transforms[choiceNumber].sizeDelta.x, transforms[choiceNumber].sizeDelta.y/3);
                        
                        //Reposition to choice location
                        selection.transform.position = 
                            new Vector3(mChoices.transform.GetChild(choiceNumber).transform.position.x, 
                                        mChoices.transform.GetChild(choiceNumber).transform.position.y-1,
                                        100);
                    } //end if
                    else if(checkpoint == 4)
                    {
                        //Increase choice to show next option is highlighted
                        choiceNumber++;
                        //Loop back to start if it's higher than the amount of choices
                        if(choiceNumber >= mChoices.transform.childCount)
                        {
                            choiceNumber = 1;
                        } //end if
                        
                        //Resize to choice width
                        selection.GetComponent<RectTransform>().sizeDelta = 
                            new Vector2(transforms[choiceNumber].sizeDelta.x, transforms[choiceNumber].sizeDelta.y/3);
                        
                        //Reposition to choice location
                        selection.transform.position = 
                            new Vector3(mChoices.transform.GetChild(choiceNumber).transform.position.x, 
                                        mChoices.transform.GetChild(choiceNumber).transform.position.y-2,
                                        100);
                    } //end else if
                    break;
                } //end case OverallGame MENU
                    
                //New Game
                case OverallGame.NEWGAME:
                {
                    //If down arrow is pressed on confirm box
                    if(checkpoint == 8)
                    {
                        //Increase choice to show next option is highlighted
                        choiceNumber++;
                        //Loop back to start if it's higher than the amount of choices
                        if(choiceNumber > 1)
                        {
                            choiceNumber = 0;
                        } //end if
                        
                        //Reposition to choice location
                        selection.transform.position = new Vector3(
                            confirm.transform.GetChild(choiceNumber).position.x,
                            confirm.transform.GetChild(choiceNumber).position.y, 100);
                    } //end if
                    break;
                } //end case OverallGame NEWGAME
                    
                //Main Game scene
                case OverallGame.CONTINUE:
                {
                    //Pokemon submenu on Continue Game -> My Team
                    if(gameState == MainGame.POKEMONSUBMENU)
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
                    } //end if Pokemon submenu on Continue Game -> My Team
                    
                    //Continue Game -> My Team
                    else if(gameState == MainGame.TEAM)
                    {
                        //If on last, or second to last team slot, go to PC button
                        if((choiceNumber == GameManager.instance.GetTrainer().Team.Count - 1
                            && choiceNumber > 0)
                           || choiceNumber == GameManager.instance.GetTrainer().Team.Count)
                        {
                            choiceNumber = -1;
                            currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(0).gameObject;
                        } //end if
                        //If on Cancel button, go to first slot
                        else if(choiceNumber == 0)
                        {
                            choiceNumber = 1;
                            currentTeamSlot = playerTeam.transform.FindChild("Pokemon1").gameObject;
                        } //end else if
                        //If on PC button, go to Cancel button
                        else if(choiceNumber == -1)
                        {
                            choiceNumber = 0;
                            currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(1).gameObject;
                        } //end else if
                        //Go down vertically
                        else
                        {
                            choiceNumber += 2;
                            currentTeamSlot = playerTeam.transform.FindChild("Pokemon"+choiceNumber).gameObject;
                        } //end else
                    } //end else if Continue Game -> My Team

                    //Pokemon Summary on Continue Game -> Summary
                    else if(gameState == MainGame.POKEMONSUMMARY)
                    {
                        //If on any page besides move details
                        if(summaryChoice != 5)
                        {
                            //Increase (lower slots are on higher childs)
                            choiceNumber++;
                            
                            //Clamp between 1 and team size
                            if(choiceNumber > GameManager.instance.GetTrainer().Team.Count)
                            {
                                choiceNumber = 1;
                            } //end if
                        } //end if
                        else
                        {
                            //Increase (lower slots are on higher childs)
                            moveChoice++;
                            
                            //If chosen move is null, loop to top
                            if(moveChoice >= 4 || GameManager.instance.GetTrainer().Team[choiceNumber-1].
                               GetMove(moveChoice) == -1)
                            {
                                moveChoice = 0;
                            } //end if

                            //Set move slot
                            currentMoveSlot = summaryScreen.transform.GetChild(5).
                                FindChild("Move"+(moveChoice+1)).gameObject;
                        } //end else
                    } //end else if Pokemon Summary on Continue Game -> Summary

                    //Pokemon Switch on Continue Game -> Switch
                    else if(gameState == MainGame.POKEMONSWITCH)
                    {
                        //Move from bottom slot to top slot
                        if(switchChoice >= GameManager.instance.GetTrainer().Team.Count - 1)
                        {
                            switchChoice = 1;
                        } //end else if
                        //Go down vertically
                        else
                        {
                            switchChoice += 2;
                        } //end else

                        //Set currentSwitchSlot
                        currentSwitchSlot = playerTeam.transform.FindChild("Pokemon"+switchChoice).gameObject;
                    } //end else if Pokemon Switch on Continue Game -> Switch

                    //Move Switch on Continue Game -> Summary -> Move Details
                    else if(gameState  == MainGame.MOVESWITCH)
                    {
                        //Increase (lower slots on higher children)
                        switchChoice++;
                        
                        //Clamp between 0 and highest non-null move
                        if(switchChoice > GameManager.instance.GetTrainer().Team[choiceNumber-1].GetMoveCount()-1)
                        {
                            switchChoice = 0;
                        } //end if

                        //Set currentSwitchSlot
                        currentSwitchSlot = summaryScreen.transform.GetChild(5).
                            FindChild("Move"+(switchChoice+1)).gameObject;
                    } //end else if Move Switch on Continue Game -> Summary -> Move Details

                    //Pokemon Ribbons in Continue Game -> Ribbons
                    else if(gameState == MainGame.POKEMONRIBBONS)
                    {
                        //Increase (lower slots are on higher childs)
                        choiceNumber++;
                        
                        //Clamp between 1 and team size
                        if(choiceNumber > GameManager.instance.GetTrainer().Team.Count)
                        {
                            choiceNumber = 1;
                        } //end if

                        //Reload ribbons
                        initialize = false;
                    } //end else if Pokemon Ribbons in Continue Game -> Ribbons
                    break;
                } //end case OverallGame CONTINUEGAME

                //PC
                case OverallGame.PC:
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
                    break;
                } //end case OverallState PC
            } //end scene switch
        }//end else if Down Arrow

        /***********************************************
         * Mouse Moves Down
         ***********************************************/ 
        if(Input.GetAxis("Mouse Y") < 0)
        {
            //Scene switch
            switch(sceneState)
            {
                //Intro
                case OverallGame.INTRO:
                {
                    break;
                } //end case OverallGame INTRO
                    
                //Menu
                case OverallGame.MENU:
                {
                    //If the mouse is lower than the selection box, and it moved, reposition to next choice
                    if(checkpoint == 2  && Input.mousePosition.y < 
                       Camera.main.WorldToScreenPoint(selection.transform.position).y-25)
                    {
                        //Make sure it's not on last choice
                        if(choiceNumber < mChoices.transform.childCount-1)
                        {
                            //Update choiceNumber
                            choiceNumber++;
                            //Resize to choice width
                            
                            selection.GetComponent<RectTransform>().sizeDelta = 
                                new Vector2(transforms[choiceNumber].sizeDelta.x, transforms[choiceNumber].sizeDelta.y/3);
                            
                            //Reposition to choice location
                            selection.transform.position = 
                                new Vector3(mChoices.transform.GetChild(choiceNumber).transform.position.x, 
                                            mChoices.transform.GetChild(choiceNumber).transform.position.y-1,
                                            100);
                        } //end if
                    } //end if
                    //If the mouse is lower than the selection box, and it moved, reposition to next choice
                    else if(checkpoint == 4 && Input.mousePosition.y <
                            Camera.main.WorldToScreenPoint(selection.transform.position).y-25)
                    {
                        //Make sure it's not on last choice
                        if(choiceNumber < mChoices.transform.childCount-1)
                        {
                            //Update choiceNumber
                            choiceNumber++;
                            
                            //Resize to choice width
                            selection.GetComponent<RectTransform>().sizeDelta = 
                                new Vector2(transforms[choiceNumber].sizeDelta.x, transforms[choiceNumber].sizeDelta.y/3);
                            
                            //Reposition to choice location
                            selection.transform.position = 
                                new Vector3(mChoices.transform.GetChild(choiceNumber).transform.position.x, 
                                            mChoices.transform.GetChild(choiceNumber).transform.position.y-2,
                                            100);
                        } //end if
                    } //end else if
                    break;
                } //end case OverallGame MENU
                    
                //New Game
                case OverallGame.NEWGAME:
                {
                    //If the mouse is lower than the selection box, and it moved, reposition to next choice
                    if(checkpoint == 8 && Input.mousePosition.y < selection.transform.position.y-1)
                    {
                        //Make sure it's not on last choice
                        if(choiceNumber == 0)
                        {
                            //Update choiceNumber
                            choiceNumber++;
                            
                            //Reposition to choice location
                            selection.transform.position = new Vector3(
                                confirm.transform.GetChild(choiceNumber).position.x,
                                confirm.transform.GetChild(choiceNumber).position.y, 100);
                        } //end if
                    } //end if
                    break;
                } //end case OverallGame NEWGAME
                    
                //Main Game scene
                case OverallGame.CONTINUE:
                {
                    //Pokemon submenu on Continue Game -> My Team
                    if(gameState == MainGame.POKEMONSUBMENU && Input.mousePosition.y < 
                       selection.transform.position.y-1)
                    {
                        //If not on the last option, increase (lower slots on higher children)
                        if(subMenuChoice < choices.transform.childCount-1)
                        {
                            subMenuChoice++;
                        } //end if

                        //Reposition selection
                        selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
                    } //end if Pokemon submenu on Continue Game -> My Team
                    
                    //Continue Game -> My Team
                    else if(gameState == MainGame.TEAM &&
                            Input.mousePosition.y < Camera.main.WorldToScreenPoint(currentTeamSlot.transform.
                            position).y - currentTeamSlot.GetComponent<RectTransform>().rect.height/2)
                    {
                        //If on last or second to last slot, go to PC button
                        if((choiceNumber == GameManager.instance.GetTrainer().Team.Count - 1
                            && choiceNumber > 0)
                           || choiceNumber == GameManager.instance.GetTrainer().Team.Count)
                        {
                            choiceNumber = -1;
                            currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(0).gameObject;
                        } //end if
                        //If on Cancel button, stay on Cancel button (Causes glitches when mouse goes below button)
                        else if(choiceNumber == 0)
                        {
                            choiceNumber = 0;
                        } //end else if
                        //If on PC button, go to Cancel button
                        else if(choiceNumber == -1)
                        {
                            choiceNumber = 0;
                            currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(1).gameObject;
                        } //end else if
                        //Go down vertically
                        else
                        {
                            choiceNumber += 2;
                            currentTeamSlot = playerTeam.transform.FindChild("Pokemon"+choiceNumber).gameObject;
                        } //end else
                    } //end else if Continue Game -> My Team

                    //Pokemon Switch on Continue Game -> Switch
                    else if(gameState == MainGame.POKEMONSWITCH &&
                            Input.mousePosition.y < Camera.main.WorldToScreenPoint(currentSwitchSlot.transform.
                            position).y - currentSwitchSlot.GetComponent<RectTransform>().rect.height/2)
                    {
                        //If not on second to last or last slot, go to down vertically
                        if(switchChoice < GameManager.instance.GetTrainer().Team.Count - 1)
                        {
                            switchChoice += 2;
                            currentSwitchSlot = playerTeam.transform.FindChild("Pokemon"+switchChoice).gameObject;
                        } //end else
                    } //end else if Pokemon Switch on Continue Game -> Switch

                    //Pokemon Summary on Continue Game -> Summary
                    else if(gameState == MainGame.POKEMONSUMMARY && summaryChoice == 5 &&
                            Input.mousePosition.y < Camera.main.WorldToScreenPoint(currentMoveSlot.transform.
                            position).y - currentMoveSlot.GetComponent<RectTransform>().rect.height/2)
                    {
                        //If not on the last slot
                        if(moveChoice < 3)
                        {
                            //If next slot is null, don't move
                            if(moveChoice < GameManager.instance.GetTrainer().Team[choiceNumber-1].GetMoveCount()-1)
                            {
                                moveChoice++;
                            } //end if

                            //Set currentMoveSlot
                            currentMoveSlot = summaryScreen.transform.GetChild(5).
                                FindChild("Move"+(moveChoice+1)).gameObject;
                        } //end if
                    } //end else if Pokemon Summary on Continue Game -> Summary

                    //Move Switch on Continue Game -> Summary -> Move Details
                    else if(gameState  == MainGame.MOVESWITCH &&
                            Input.mousePosition.y < Camera.main.WorldToScreenPoint(currentSwitchSlot.transform.
                            position).y - currentSwitchSlot.GetComponent<RectTransform>().rect.height/2)
                    {
                        //If next slot is null, don't move
                        if(switchChoice < GameManager.instance.GetTrainer().Team[choiceNumber-1].GetMoveCount() - 1)
                        {
                            switchChoice++;
                        } //end if

                        //Set currentSwitchSlot
                        currentSwitchSlot = summaryScreen.transform.GetChild(5).
                            FindChild("Move"+(switchChoice+1)).gameObject;
                    } //end else if Move Switch on Continue Game -> Summary -> Move Details

                    //Pokemon Ribbons on Continue Game -> Ribbons
                    else if(gameState  == MainGame.POKEMONRIBBONS &&
                            Input.mousePosition.y < Camera.main.WorldToScreenPoint(currentRibbonSlot.transform.
                            position).y - currentRibbonSlot.GetComponent<RectTransform>().rect.height/2)
                    {
                        //If next slot is null, don't move
                        if(ribbonChoice+4 < GameManager.instance.GetTrainer().Team[choiceNumber-1].GetRibbonCount())
                        {
                            ribbonChoice += 4;

                            //Read ribbon
                            ReadRibbon();
                        } //end if
                        
                        //Set currentRibbonSlot
                        currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
                            GetChild(ribbonChoice).gameObject;
                    } //end else if Pokemon Ribbons on Continue Game -> Ribbons
                    break;
                } //end case OverallGame CONTINUEGAME
                //PC
                case OverallGame.PC:
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
                            selection.transform.position.y-1)
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
                    break;
                } //end case OverallGame PC
            } //end scene switch
        } //end else if Mouse Moves Down

        /***********************************************
         * Mouse Moves Up
         ***********************************************/ 
        else if(Input.GetAxis("Mouse Y") > 0)
        {
            //Scene switch
            switch(sceneState)
            {
                //Intro
                case OverallGame.INTRO:
                {
                    break;
                } //end case OverallGame INTRO
                    
                //Menu
                case OverallGame.MENU:
                {
                    //If the mouse is higher than the selection box, and it moved, reposition to next choice
                    if(checkpoint == 2 && Input.mousePosition.y > 
                       Camera.main.WorldToScreenPoint(selection.transform.position).y+25)
                    {
                        //Make sure it's not on last choice
                        if(choiceNumber > 0)
                        {
                            //Update choiceNumber
                            choiceNumber--;
                            
                            //Resize to choice width
                            selection.GetComponent<RectTransform>().sizeDelta = 
                                new Vector2(transforms[choiceNumber].sizeDelta.x, transforms[choiceNumber].sizeDelta.y/3);
                            
                            //Reposition to choice location
                            selection.transform.position = 
                                new Vector3(mChoices.transform.GetChild(choiceNumber).transform.position.x, 
                                            mChoices.transform.GetChild(choiceNumber).transform.position.y-1,
                                            100);
                        } //end if
                    } //end if
                    //If the mouse is higher than the selection box, and it moved, reposition to next choice
                    else if(checkpoint == 4 && Input.mousePosition.y >
                            Camera.main.WorldToScreenPoint(selection.transform.position).y+25)
                    {
                        //Make sure it's not on last choice
                        if(choiceNumber > 1)
                        {
                            //Update choiceNumber
                            choiceNumber--;
                            
                            //Resize to choice width
                            selection.GetComponent<RectTransform>().sizeDelta = 
                                new Vector2(transforms[choiceNumber].sizeDelta.x, transforms[choiceNumber].sizeDelta.y/3);
                            
                            //Reposition to choice location
                            selection.transform.position = 
                                new Vector3(mChoices.transform.GetChild(choiceNumber).transform.position.x, 
                                mChoices.transform.GetChild(choiceNumber).transform.position.y-2,
                                100);
                        } //end if
                        
                        //Menu finished this check
                        processing = false;
                    } //end else if
                    break;
                } //end case OverallGame MENU
                    
                //New Game
                case OverallGame.NEWGAME:
                {
                    //If the mouse is higher than the selection box, and it moved, reposition to next choice
                    if(checkpoint == 8 && Input.mousePosition.y > selection.transform.position.y+1)
                    {
                        //Make sure it's not on last choice
                        if(choiceNumber == 1)
                        {
                            //Update choiceNumber
                            choiceNumber--;
                            
                            //Reposition to choice location
                            selection.transform.position = new Vector3(
                                confirm.transform.GetChild(choiceNumber).position.x,
                                confirm.transform.GetChild(choiceNumber).position.y, 100);
                        } //end if
                    } //end if
                    break;
                } //end case OverallGame NEWGAME
                    
                //Main Game scene
                case OverallGame.CONTINUE:
                {
                    //Pokemon submenu on Continue Game -> My Team
                    if(gameState == MainGame.POKEMONSUBMENU && Input.mousePosition.y > 
                       selection.transform.position.y+1)
                    {
                        //If not on the first option, decrease
                        if(subMenuChoice > 0)
                        {
                            subMenuChoice--;
                        } //end if

                        //Reposition selection
                        selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
                    } //end if Pokemon submenu on Continue Game -> My Team
                    
                    //Continue Game -> My Team
                    else if(gameState == MainGame.TEAM &&
                            Input.mousePosition.y > Camera.main.WorldToScreenPoint(currentTeamSlot.transform.
                            position).y + currentTeamSlot.GetComponent<RectTransform>().rect.height/2)
                    {
                        //If on a top slot, stay there (Causes glitches when mouse is above slot)
                        if(choiceNumber == 1 || choiceNumber == 2)
                        {
                            choiceNumber = choiceNumber;
                        } //end if
                        //If on PC button, go to last team slot
                        else if(choiceNumber == -1)
                        {
                            choiceNumber = GameManager.instance.GetTrainer().Team.Count;
                            currentTeamSlot = playerTeam.transform.FindChild("Pokemon"+choiceNumber).gameObject;
                        } //end else if
                        //If on Cancel button, go to PC button
                        else if(choiceNumber == 0)
                        {
                            choiceNumber = -1;
                            currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(0).gameObject;
                        } //end else if
                        //Go up vertically
                        else
                        {
                            choiceNumber -= 2;
                            currentTeamSlot = playerTeam.transform.FindChild("Pokemon"+choiceNumber).gameObject;
                        } //end else
                    } //end else if Continue Game -> My Team

                    //Pokemon Switch on Continue Game -> Switch
                    else if(gameState == MainGame.POKEMONSWITCH &&
                            Input.mousePosition.y > Camera.main.WorldToScreenPoint(currentSwitchSlot.transform.
                            position).y + currentSwitchSlot.GetComponent<RectTransform>().rect.height/2)
                    {
                        //If not on a top slot, go up vertically
                        if(switchChoice > 2)
                        {
                            switchChoice -= 2;
                            currentSwitchSlot = playerTeam.transform.FindChild("Pokemon"+switchChoice).gameObject;
                        } //end else
                    } //end else if Pokemon Switch on Continue Game -> Switch

                    //Pokemon Summary on Continue Game -> Summary
                    else if(gameState == MainGame.POKEMONSUMMARY && summaryChoice == 5 &&
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
                    } //end else if Pokemon Summary on Continue Game -> Summary

                    //Move Switch on Continue Game -> Summary -> Move Details
                    else if(gameState  == MainGame.MOVESWITCH &&
                            Input.mousePosition.y > Camera.main.WorldToScreenPoint(currentSwitchSlot.transform.
                            position).y + currentSwitchSlot.GetComponent<RectTransform>().rect.height/2)
                    {
                        //If not on first slot ,go up vertically
                        if(switchChoice > 0)
                        {
                            switchChoice--;                        
                            currentSwitchSlot = summaryScreen.transform.GetChild(5).
                                FindChild("Move"+(switchChoice+1)).gameObject;
                        } //end if
                    } //end else if Move Switch on Continue Game -> Summary -> Move Details

                    //Pokemon Ribbons on Continue Game -> Ribbons
                    else if(gameState  == MainGame.POKEMONRIBBONS &&
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
                    } //end else if Pokemon Ribbons on Continue Game -> Ribbons
                    break;
                } //end case OverallGame CONTINUEGAME
                //PC
                case OverallGame.PC:
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
                       selection.transform.position.y+1)
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
                    break;
                } //end case OverallGame PC
            } //end scene switch
        } //end else if Mouse Moves Up

        /***********************************************
         * Mouse Moves Left
         ***********************************************/ 
        if(Input.GetAxis("Mouse X") < 0)
        {
            //Scene switch
            switch(sceneState)
            {
                //Intro
                case OverallGame.INTRO:
                {
                    break;
                } //end case OverallGame INTRO
                    
                //Menu
                case OverallGame.MENU:
                {
                    break;
                } //end case OverallGame MENU
                    
                //New Game
                case OverallGame.NEWGAME:
                {
                    break;
                } //end case OverallGame NEWGAME
                    
                //Main Game scene
                case OverallGame.CONTINUE:
                {
                    //Continue Game -> My Team
                    if(gameState == MainGame.TEAM &&
                       Input.mousePosition.x < Camera.main.WorldToScreenPoint(currentTeamSlot.transform.
                       position).x - currentTeamSlot.GetComponent<RectTransform>().rect.width/2)
                    {
                        //If choice number is not odd, and is greater than 0, move left
                        if((choiceNumber&1) != 1 && choiceNumber > 0)
                        {
                            choiceNumber--;
                            currentTeamSlot = playerTeam.transform.FindChild("Pokemon"+choiceNumber).gameObject;
                        } //end if   
                    } //end if Continue Game -> My Team

                    //Pokemon Switch on Continue Game -> Switch
                    if(gameState == MainGame.POKEMONSWITCH &&
                       Input.mousePosition.x < Camera.main.WorldToScreenPoint(currentSwitchSlot.transform.
                       position).x - currentSwitchSlot.GetComponent<RectTransform>().rect.width/2)
                    {
                        //If switch number is not odd, move left
                        if((switchChoice&1) != 1)
                        {
                            switchChoice--;
                            currentSwitchSlot = playerTeam.transform.FindChild("Pokemon"+switchChoice).gameObject;
                        } //end if   
                    } //end if Pokemon Switch on Continue Game -> Switch

                    //Pokemon Ribbons on Continue Game -> Ribbons
                    else if(gameState  == MainGame.POKEMONRIBBONS &&
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
                    } //end else if Pokemon Ribbons on Continue Game -> Ribbons
                    break;
                } //end OverallGame CONTINUEGAME
                //PC
                case OverallGame.PC:
                {
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
                    break;
                } //end case OverallGame PC
            } //end scene switch
        } //end if Mouse Moves Left

        /***********************************************
         * Mouse Moves Right
         ***********************************************/ 
        else if(Input.GetAxis("Mouse X") > 0)
        {
            //Scene switch
            switch(sceneState)
            {
                //Intro
                case OverallGame.INTRO:
                {
                    break;
                } //end case OverallGame INTRO
                    
                //Menu
                case OverallGame.MENU:
                {
                    break;
                } //end case OverallGame MENU
                    
                //New Game
                case OverallGame.NEWGAME:
                {
                    break;
                } //end case OverallGame NEWGAME
                    
                //Main Game scene
                case OverallGame.CONTINUE:
                {
                    //Continue Game -> My Team
                    if(gameState == MainGame.TEAM && Input.mousePosition.x > Camera.main.
                       WorldToScreenPoint(currentTeamSlot.transform.position).x + currentTeamSlot.
                       GetComponent<RectTransform>().rect.width/2)
                    {
                        //If choice is odd and team is not odd numbered and choice is greater than 0, move right
                        if((choiceNumber&1) == 1 && choiceNumber != GameManager.instance.GetTrainer().Team.Count
                           && choiceNumber > 0)
                        {
                            choiceNumber++;
                            currentTeamSlot = playerTeam.transform.FindChild("Pokemon"+choiceNumber).gameObject;
                        } //end if   
                    } //end if Continue Game -> My Team

                    //Pokemon Switch on Continue Game -> Switch
                    if(gameState == MainGame.POKEMONSWITCH && Input.mousePosition.x > Camera.main.
                       WorldToScreenPoint(currentSwitchSlot.transform.position).x + currentSwitchSlot.
                       GetComponent<RectTransform>().rect.width/2)
                    {
                        //If switch is odd and team is not odd numbered, move right
                        if((switchChoice&1) == 1 && switchChoice != GameManager.instance.GetTrainer().Team.Count)
                        {
                            switchChoice++;
                            currentSwitchSlot = playerTeam.transform.FindChild("Pokemon"+switchChoice).gameObject;
                        } //end if   
                    } //end if Pokemon Switch on Continue Game -> Switch

                    //Pokemon Ribbons on Continue Game -> Ribbons
                    else if(gameState  == MainGame.POKEMONRIBBONS &&
                            Input.mousePosition.x > Camera.main.WorldToScreenPoint(currentRibbonSlot.transform.
                            position).x + currentRibbonSlot.GetComponent<RectTransform>().rect.width/2)
                    {
                        //If next slot is null, don't move
                        if(ribbonChoice + 1 < GameManager.instance.GetTrainer().Team[choiceNumber-1].GetRibbonCount() &&
                           ribbonChoice % 4 != 3)
                        {
                            ribbonChoice++;

                            //Read Ribbon
                            ReadRibbon();
                        } //end if
                        
                        //Set currentRibbonSlot
                        currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
                            GetChild(ribbonChoice).gameObject;
                    } //end else if Pokemon Ribbons on Continue Game -> Ribbons
                    break;
                } //end OverallGame CONTINUEGAME
                //PC
                case OverallGame.PC:
                {
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
                    break;
                } //end case OverallGame PC
            } //end scene switch
        } //end else if Mouse Moves Right

        /***********************************************
         * Left Mouse Button
         ***********************************************/ 
        else if(Input.GetMouseButtonUp(0))
        {
            //Scene switch
            switch(sceneState)
            {
                //Intro
                case OverallGame.INTRO:
                {
                    //If player cancels animation
                    if(playing && processing)
                    {
                        playing = false;
                    } //end if

                    //Fade out in prepareation for menu scene
                    else if(checkpoint == 3)
                    {
                        playing = true;
                        StartCoroutine(FadeOutAnimation(4));
                    } //end else if
                    break;
                } //end case OverallGame INTRO
                    
                //Menu
                case OverallGame.MENU:
                {
                    break;
                } //end case OverallGame MENU
                    
                //New Game
                case OverallGame.NEWGAME:
                {
                    if(checkpoint == 6 && inputText.text.Length != 0)
                    {
                        //Convert input name to player's name
                        playerName = inputText.text;
                        input.SetActive(false);
                        GameManager.instance.DisplayText("So your name is " + playerName + "?", false);
                        checkpoint = 7;
                    } //end if
                    else if(checkpoint == 8)
                    {
                        // Yes selected
                        if(choiceNumber == 0)
                        {
                            checkpoint = 9;
                        } //end if
                        // No selected
                        else if(choiceNumber == 1)
                        {
                            GameManager.instance.DisplayText("Ok let's try again. What is your name?", true);
                            checkpoint = 5;
                        } //end else if
                        
                        //Disable choice and selection
                        selection.SetActive(false);
                        confirm.SetActive(false);
                    } //end else if
                    break;
                } //end case OverallGame NEWGAME
                    
                //Main Game scene
                case OverallGame.CONTINUE:
                {
                    //Pokemon submenu on Continue Game -> My Team is Open
                    if(gameState == MainGame.POKEMONSUBMENU)
                    {
                        //Apply appropriate action based on submenu selection
                        switch(subMenuChoice)
                        {
                            //Summary
                            case 0:
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
                                gameState = MainGame.POKEMONSUMMARY;
                                break;
                            } //end case 0 (Summary)
                                
                            //Switch
                            case 1:
                            {
                                selection.SetActive(false);
                                choices.SetActive(false);
                                switchChoice = choiceNumber;
                                previousSwitchSlot = choiceNumber;
                                currentSwitchSlot = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
                                gameState = MainGame.POKEMONSWITCH;
                                break;
                            } //end case 1 (Switch)
                                
                            //Item
                            case 2:
                            {
                                if(GameManager.instance.GetTrainer().Team[choiceNumber-1].Item == 0)
                                {
                                GameManager.instance.GetTrainer().Team[choiceNumber-1].Item=
                                    GameManager.instance.RandomInt(1, 500);
                                }
                                else
                                {
                                    GameManager.instance.GetTrainer().Team[choiceNumber-1].Item = 0;
                                } //end else
                                selection.SetActive(false);
                                choices.SetActive(false);
                                initialize = false;
                                gameState = MainGame.TEAM;
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
                                gameState = MainGame.POKEMONRIBBONS;
                                break;
                            } //end case 3 (Ribbons)

                            //Cancel
                            case 4:
                            {
                                choices.SetActive(false);
                                selection.SetActive(false);
                                gameState = MainGame.TEAM;
                                break;
                            } //end case 4 (Cancel)
                        } //end switch
                    } //end if Pokemon submenu on Continue Game -> My Team is Open
                    
                    //Open menu if not open, as long as player isn't selecting a button
                    else if(gameState == MainGame.TEAM && choiceNumber > 0)
                    {
                        //Set submenu active
                        choices.SetActive(true);
                        selection.SetActive(true);

                        //Set up selection box at end of frame if it doesn't fit
                        if(selection.GetComponent<RectTransform>().sizeDelta != 
                           choices.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta)
                        {
                            selection.SetActive(false);
                            StartCoroutine("WaitForResize");
                        } //end if
                        
                        //Reset position to top of menu
                        initialize = false;
                        subMenuChoice = 0;
                        gameState = MainGame.POKEMONSUBMENU;
                    } //end else if Open menu if not open

                    //Pokemon Switch on Continue Game -> Switch
                    else if(gameState == MainGame.POKEMONSWITCH)
                    {
                        //If the choice equals the switch, don't run swap
                        if(choiceNumber != switchChoice)
                        {
                            //Switch function
                            GameManager.instance.GetTrainer().Swap(choiceNumber-1, switchChoice-1);
                        } //end if
                            
                        //Go back to team
                        initialize = false;
                        gameState = MainGame.TEAM;
                    } //end else if Pokemon Switch on Continue Game -> Switch

                    //Pokemon Summary on Continue Game -> Summary
                    else if(gameState == MainGame.POKEMONSUMMARY)
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
                            gameState = MainGame.MOVESWITCH;
                        } //end else if
                    } //end else if Pokemon Summary on Continue Game -> Summary

                    //Move Switch on Continue Game -> Summary -> Move Details
                    else if(gameState  == MainGame.MOVESWITCH)
                    {
                        //If switching spots aren't the same
                        if(moveChoice != switchChoice)
                        {
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].SwitchMoves(moveChoice, switchChoice);
                        } //end if

                        currentMoveSlot.GetComponent<Image>().color = Color.clear;
                        gameState = MainGame.POKEMONSUMMARY;
                    } //end else if Move Switch on Continue Game -> Summary -> Move Details

                    //Pokemon Ribbons on Continue Game -> Ribbons
                    else if(gameState  == MainGame.POKEMONRIBBONS)
                    {
                        //Make sure there are ribbons to be read
                        if(GameManager.instance.GetTrainer().Team[choiceNumber-1].GetRibbonCount() > 0)
                        {
                            selection.SetActive(!selection.activeSelf);
                            ReadRibbon();
                        } //end if
                    } //end else if Pokemon Ribbons on Continue Game -> Ribbons
                    break;
                } //end case OverallGame CONTINUEGAME
                //PC
                case OverallGame.PC:
                {
                    //PC Home
                    if(pcState == PCGame.HOME && (selectedPokemon != null || heldPokemon != null))
                    {
                        //Open submenu as long as player is in pokemon region
                        if(boxChoice > -1 && boxChoice < 30)
                        {
                            //Set submenu active
                            choices.SetActive(true);
                            selection.SetActive(true);

                            //Set up selection box at end of frame if it doesn't fit
                            if(selection.GetComponent<RectTransform>().sizeDelta != 
                               choices.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta)
                            {
                                selection.SetActive(false);
                                StartCoroutine("WaitForResize");
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
                            //Set submenu active
                            choices.SetActive(true);
                            selection.SetActive(true);

                            //Set up selection box at end of frame if it doesn't fit
                            if(selection.GetComponent<RectTransform>().sizeDelta != 
                               choices.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta)
                            {
                                selection.SetActive(false);
                                StartCoroutine("WaitForResize");
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
                                selection.SetActive(false);
                                choices.SetActive(false);
                                if(selectedPokemon.Item == 0)
                                {
                                    selectedPokemon.Item = GameManager.instance.RandomInt(1, 500);
                                } //end if
                                else
                                {
                                    selectedPokemon.Item = 0;
                                } //end else
                                pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
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
                                    //Get the pokemon
                                    selectedPokemon = GameManager.instance.GetTrainer().Team[choiceNumber-1];

                                    //Remove the pokemon from the party
                                    GameManager.instance.GetTrainer().RemovePokemon(choiceNumber-1);

                                    //Fill in party tab
                                    for(int i = 1; i < GameManager.instance.GetTrainer().Team.Count + 1; i++)
                                    {
                                        partyTab.transform.FindChild("Pokemon"+i).GetComponent<Image>().sprite =
                                            Resources.Load<Sprite>("Sprites/Icons/icon"+GameManager.instance.GetTrainer().
                                                                   Team[i-1].NatSpecies.ToString("000"));
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

                                choices.SetActive(false);
                                selection.SetActive(false);
                                GameManager.instance.DisplayText("You released " + selectedPokemon.Nickname, true);
                                selectedPokemon = null;
                                pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
                                break;
                            } //end case 5 (Release)

                            //Cancel
                            case 6:
                            {
                                choices.SetActive(false);
                                selection.SetActive(false);
                                pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
                                break;
                            } //end case 6 (Cancel)
                        } //end switch
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
                            //Fill in choices box
                            for (int i = choices.transform.childCount-1; i < 6; i++)
                            {
                                GameObject clone = Instantiate (choices.transform.GetChild (0).gameObject,
                                                                choices.transform.GetChild (0).position,
                                                                Quaternion.identity) as GameObject;
                                clone.transform.SetParent (choices.transform);
                            } //end for
                            choices.transform.GetChild (0).GetComponent<Text> ().text = "Move";
                            choices.transform.GetChild (1).GetComponent<Text> ().text = "Summary";
                            choices.transform.GetChild (2).GetComponent<Text> ().text = "Item";
                            choices.transform.GetChild (3).GetComponent<Text> ().text = "Ribbons";
                            choices.transform.GetChild (4).GetComponent<Text> ().text = "Markings";
                            choices.transform.GetChild (5).GetComponent<Text> ().text = "Release";
                            choices.transform.GetChild (6).GetComponent<Text> ().text = "Cancel";
                            choices.transform.GetChild (0).GetComponent<Text> ().color = Color.black;
                            choices.transform.GetChild (1).GetComponent<Text> ().color = Color.black;
                            choices.transform.GetChild (2).GetComponent<Text> ().color = Color.black;
                            choices.transform.GetChild (3).GetComponent<Text> ().color = Color.black;
                            choices.transform.GetChild (4).GetComponent<Text> ().color = Color.black;
                            choices.transform.GetChild (5).GetComponent<Text> ().color = Color.black;
                            choices.transform.GetChild (6).GetComponent<Text> ().color = Color.black;
                            if (choices.transform.childCount > 7)
                            {
                                for (int i = 7; i < choices.transform.childCount; i++)
                                {
                                    Destroy (choices.transform.GetChild (i).gameObject);
                                } //end for
                            } //end if
                            choices.SetActive(false);
                            selection.SetActive(false);

                            //If holding a pokemon
                            if(heldPokemon != null)
                            {
                                //Update held pokemon markings
                                heldPokemon.SetMarkings(markingChoices.ToArray());

                                //Return to home or party
                                pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
                            } //end if
                            //If in party
                            else if(partyTab.activeSelf)
                            {
                                //Update team pokemon markings
                                GameManager.instance.GetTrainer().Team[choiceNumber-1].SetMarkings(markingChoices.ToArray());

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
                            //Fill in choices box
                            for (int i = choices.transform.childCount-1; i < 6; i++)
                            {
                                GameObject clone = Instantiate (choices.transform.GetChild (0).gameObject,
                                                                choices.transform.GetChild (0).position,
                                                                Quaternion.identity) as GameObject;
                                clone.transform.SetParent (choices.transform);
                            } //end for
                            choices.transform.GetChild (0).GetComponent<Text> ().text = "Move";
                            choices.transform.GetChild (1).GetComponent<Text> ().text = "Summary";
                            choices.transform.GetChild (2).GetComponent<Text> ().text = "Item";
                            choices.transform.GetChild (3).GetComponent<Text> ().text = "Ribbons";
                            choices.transform.GetChild (4).GetComponent<Text> ().text = "Markings";
                            choices.transform.GetChild (5).GetComponent<Text> ().text = "Release";
                            choices.transform.GetChild (6).GetComponent<Text> ().text = "Cancel";
                            choices.transform.GetChild (0).GetComponent<Text> ().color = Color.black;
                            choices.transform.GetChild (1).GetComponent<Text> ().color = Color.black;
                            choices.transform.GetChild (2).GetComponent<Text> ().color = Color.black;
                            choices.transform.GetChild (3).GetComponent<Text> ().color = Color.black;
                            choices.transform.GetChild (4).GetComponent<Text> ().color = Color.black;
                            choices.transform.GetChild (5).GetComponent<Text> ().color = Color.black;
                            choices.transform.GetChild (6).GetComponent<Text> ().color = Color.black;
                            if (choices.transform.childCount > 7)
                            {
                                for (int i = 7; i < choices.transform.childCount; i++)
                                {
                                    Destroy (choices.transform.GetChild (i).gameObject);
                                } //end for
                            } //end if
                            choices.SetActive(false);
                            selection.SetActive(false);

                            //Return to home or party
                            pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
                        } //end else if
                    } //end else if Pokemon Markings on PC -> Submenu
                    break;
                } //end case OverallGame PC
            } //end scene switch
        } //end else if Left Mouse Button

        /***********************************************
         * Right Mouse Button
         ***********************************************/ 
        else if(Input.GetMouseButtonDown(1))
        {            
            //Scene switch
            switch(sceneState)
            {
                //Intro
                case OverallGame.INTRO:
                {
                    break;
                } //end case OverallGame INTRO
                    
                    //Menu
                case OverallGame.MENU:
                {
                    break;
                } //end case OverallGame MENU
                    
                    //New Game
                case OverallGame.NEWGAME:
                {
                    break;
                } //end case OverallGame NEWGAME
                    
                    //Main Game scene
                case OverallGame.CONTINUE:
                {
                    //Pokemon Summary on Continue Game -> Summary
                    if(gameState == MainGame.POKEMONSUMMARY)
                    {
                        //If on any page besides details
                        if(summaryChoice != 5)
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
                    } //end if Pokemon Summary on Continue Game -> Summary

                    //Pokemon submenu on Continue Game -> My Team
                    else if(gameState == MainGame.POKEMONSUBMENU)
                    {
                        //Deactivate submenu 
                        choices.SetActive(false);
                        selection.SetActive(false);

                        gameState = MainGame.TEAM;
                    } //end else if Pokemon submenu on Continue Game -> My Team

                    //Pokemon Ribbons on Continue Game -> Ribbons
                    else if(gameState == MainGame.POKEMONRIBBONS)
                    {
                        //Deactivate ribbons
                        ribbonScreen.SetActive(false);
                        selection.SetActive(false);
                        ribbonChoice = 0;
                        previousRibbonChoice = -1;
                        
                        //Return to team
                        gameState = MainGame.TEAM;
                    } //end else if Pokemon Ribbons on Continue Game -> Ribbons

                    //Continue Game -> My Team
                    else if(gameState == MainGame.TEAM)
                    {
                        EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
                        gameState = MainGame.HOME;
                    } //end else if Continue Game -> My Team

                    //Pokemon Switch on Continue Game -> Switch
                    else if(gameState == MainGame.POKEMONSWITCH)
                    {
                        //Go back to team
                        gameState = MainGame.TEAM;
                    } //end else if Pokemon Switch on Continue Game -> Switch

                    //Move Switch on Continue Game -> Summary -> Move Details
                    else if(gameState  == MainGame.MOVESWITCH)
                    {
                        //Return to summary
                        currentMoveSlot.GetComponent<Image>().color = Color.clear;
                        gameState = MainGame.POKEMONSUMMARY;
                    } //end else if Move Switch on Continue Game -> Summary -> Move Details

                    //Continue Game -> Gym Leader
                    else if(gameState == MainGame.GYMBATTLE)
                    {
                        EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
                        gameState = MainGame.HOME;
                    } //end else if 

                    //Continue Game -> Trainer Card
                    else if(gameState == MainGame.TRAINERCARD)
                    {
                        EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
                        gameState = MainGame.HOME;
                    } //end else if 

                    //Continue Game -> Debug Options
                    else if(gameState == MainGame.DEBUG)
                    {
                        EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
                        gameState = MainGame.HOME;
                    } //end else if 
                    break;
                } //end case OverallGame CONTINUEGAME
                //PC
                case OverallGame.PC:
                {
                    //PC Home
                    if(pcState == PCGame.HOME)
                    {
                        StartCoroutine(LoadScene("MainGame", OverallGame.CONTINUE, true));
                    } //end if PC Home

                    //Pokemon Submenu on PC
                    else  if(pcState == PCGame.POKEMONSUBMENU)
                    {
                        choices.SetActive(false);
                        selection.SetActive(false);
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
                        
                        //Return to home or party
                        pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
                    } //end else if Pokemon Ribbons on PC -> Ribbons

                    //Pokemon Markings on PC -> Submenu
                    else if(pcState == PCGame.POKEMONMARKINGS)
                    {
                        //Turn menu off
                        //Fill in choices box
                        for (int i = choices.transform.childCount-1; i < 6; i++)
                        {
                            GameObject clone = Instantiate (choices.transform.GetChild (0).gameObject,
                                                            choices.transform.GetChild (0).position,
                                                            Quaternion.identity) as GameObject;
                            clone.transform.SetParent (choices.transform);
                        } //end for
                        choices.transform.GetChild (0).GetComponent<Text> ().text = "Move";
                        choices.transform.GetChild (1).GetComponent<Text> ().text = "Summary";
                        choices.transform.GetChild (2).GetComponent<Text> ().text = "Item";
                        choices.transform.GetChild (3).GetComponent<Text> ().text = "Ribbons";
                        choices.transform.GetChild (4).GetComponent<Text> ().text = "Markings";
                        choices.transform.GetChild (5).GetComponent<Text> ().text = "Release";
                        choices.transform.GetChild (6).GetComponent<Text> ().text = "Cancel";
                        choices.transform.GetChild (0).GetComponent<Text> ().color = Color.black;
                        choices.transform.GetChild (1).GetComponent<Text> ().color = Color.black;
                        choices.transform.GetChild (2).GetComponent<Text> ().color = Color.black;
                        choices.transform.GetChild (3).GetComponent<Text> ().color = Color.black;
                        choices.transform.GetChild (4).GetComponent<Text> ().color = Color.black;
                        choices.transform.GetChild (5).GetComponent<Text> ().color = Color.black;
                        choices.transform.GetChild (6).GetComponent<Text> ().color = Color.black;
                        if (choices.transform.childCount > 7)
                        {
                            for (int i = 7; i < choices.transform.childCount; i++)
                            {
                                Destroy (choices.transform.GetChild (i).gameObject);
                            } //end for
                        } //end if
                        choices.SetActive(false);
                        selection.SetActive(false);
                        
                        //Return to home or party
                        pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
                    } //end else if Pokemon Markings on PC -> Submenu
                    break;
                } //end case OverallGame PC
            } //end scene switch
        } //end else if Right Mouse Button

        /***********************************************
         * Mouse Wheel Up
         ***********************************************/ 
        else if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            //Scene switch
            switch(sceneState)
            {
                //Intro
                case OverallGame.INTRO:
                {
                    break;
                } //end case OverallGame INTRO
                    
                //Menu
                case OverallGame.MENU:
                {
                    break;
                } //end case OverallGame MENU
                    
                //New Game
                case OverallGame.NEWGAME:
                {
                    break;
                } //end case OverallGame NEWGAME
                    
                //Main Game scene
                case OverallGame.CONTINUE:
                {
                    //Pokemon Summary on Continue Game -> Summary
                    if(gameState == MainGame.POKEMONSUMMARY)
                    {
                        //If on any page besides move details
                        if(summaryChoice != 5)
                        {
                            //Decrease (higher slots are lower childs)
                            choiceNumber--;
                            
                            //Clamp between 1 and team size
                            if(choiceNumber < 1)
                            {
                                choiceNumber = GameManager.instance.GetTrainer().Team.Count;
                            } //end if
                        } //end if
                    } //end else if Pokemon Summary on Continue Game -> Summary

                    //Pokemon Ribbons on Continue Game -> Ribbons
                    else if(gameState == MainGame.POKEMONRIBBONS)
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
                    } //end else if Pokemon Ribbons on Continue Game -> Ribbons
                    break;
                } //end case OverallGame CONTINUEGAME
                //PC
                case OverallGame.PC:
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
                    break;
                } //end case OverallGame PC
            } //end scene switch
        } //end else if Mouse Wheel Up

        /***********************************************
         * Mouse Wheel Down
         ***********************************************/ 
        else if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            //Scene switch
            switch(sceneState)
            {
                //Intro
                case OverallGame.INTRO:
                {
                    break;
                } //end case OverallGame INTRO
                    
                //Menu
                case OverallGame.MENU:
                {
                    break;
                } //end case OverallGame MENU
                    
                //New Game
                case OverallGame.NEWGAME:
                {
                    break;
                } //end case OverallGame NEWGAME
                    
                //Main Game scene
                case OverallGame.CONTINUE:
                {
                    //Pokemon Summary on Continue Game -> Summary
                    if(gameState == MainGame.POKEMONSUMMARY)
                    {
                        //If on any page besides move details
                        if(summaryChoice != 5)
                        {
                            //Increase (lower slots are on higher childs)
                            choiceNumber++;
                            
                            //Clamp between 1 and team size
                            if(choiceNumber > GameManager.instance.GetTrainer().Team.Count)
                            {
                                choiceNumber = 1;
                            } //end if
                        } //end if
                    } //end else if Pokemon Summary on Continue Game -> Summary

                    //Pokemon Ribbons in Continue Game -> Ribbons
                    else if(gameState == MainGame.POKEMONRIBBONS)
                    {
                        //Increase (lower slots are on higher childs)
                        choiceNumber++;
                        
                        //Clamp between 1 and team size
                        if(choiceNumber > GameManager.instance.GetTrainer().Team.Count)
                        {
                            choiceNumber = 1;
                        } //end if
                        
                        //Reload ribbons
                        initialize = false;
                    } //end else if Pokemon Ribbons in Continue Game -> Ribbons
                    break;
                } //end case OverallGame CONTINUEGAME
                 //PC
                case OverallGame.PC:
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
                    break;
                } //end case OverallGame PC
            } //end scene switch
        } //end else if Mouse Wheel Down

        /***********************************************
         * Enter/Return Key
         ***********************************************/ 
        if(Input.GetKeyDown(KeyCode.Return))
        {
            //Scene switch
            switch(sceneState)
            {
                //Intro
                case OverallGame.INTRO:
                {
                    //If player cancels animation
                    if(playing && processing)
                    {
                        playing = false;
                    } //end if

                    //Fade out in prepareation for menu scene
                    else if(checkpoint == 3)
                    {
                        playing = true;
                        StartCoroutine(FadeOutAnimation(4));
                    } //end else if
                    break;
                } //end case OverallGame INTRO
                    
                //Menu
                case OverallGame.MENU:
                {
                    if(checkpoint == 2)
                    {
                        selection.SetActive(false);
                        StartCoroutine(FadeOutAnimation(5));
                    } //end if
                    else if(checkpoint == 4)
                    {
                        selection.SetActive(false);
                        StartCoroutine(FadeOutAnimation(5));
                    } //end else if
                    break;
                } //end case OverallGame MENU
                    
                //New Game
                case OverallGame.NEWGAME:
                {
                    if(checkpoint == 6 && inputText.text.Length != 0)
                    {
                        //Convert input name to player's name
                        playerName = inputText.text;
                        input.SetActive(false);
                        GameManager.instance.DisplayText("So your name is " + playerName + "?", false, true);
                        checkpoint = 7;
                    } //end if
                    else if(checkpoint == 8)
                    {
                        // Yes selected
                        if(choiceNumber == 0)
                        {
                            checkpoint = 9;
                        } //end if
                        // No selected
                        else if(choiceNumber == 1)
                        {
                            GameManager.instance.DisplayText("Ok let's try again. What is your name?", true);
                            checkpoint = 5;
                        } //end else if
                        
                        //Disable choice and selection
                        selection.SetActive(false);
                        confirm.SetActive(false);
                    } //end else if
                    break;
                } //end case OverallGame NEWGAME
                    
                //Main Game scene
                case OverallGame.CONTINUE:
                {
                    //Pokemon submenu on Continue Game -> My Team is Open
                    if(gameState == MainGame.POKEMONSUBMENU)
                    {
                        //Apply appropriate action based on submenu selection
                        switch(subMenuChoice)
                        {
                            //Summary
                            case 0:
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
                                gameState = MainGame.POKEMONSUMMARY;
                                break;
                            } //end case 0 (Summary)

                            //Switch
                            case 1:
                            {
                                selection.SetActive(false);
                                choices.SetActive(false);
                                switchChoice = choiceNumber;
                                previousSwitchSlot = choiceNumber;
                                currentSwitchSlot = playerTeam.transform.FindChild("Pokemon"+choiceNumber).gameObject;
                                gameState = MainGame.POKEMONSWITCH;
                                break;
                            } //end case 1 (Switch)

                            //Item
                            case 2:
                            {
                                if(GameManager.instance.GetTrainer().Team[choiceNumber-1].Item == 0)
                                {
                                    GameManager.instance.GetTrainer().Team[choiceNumber-1].Item=
                                        GameManager.instance.RandomInt(1, 500);
                                }
                                else
                                {
                                    GameManager.instance.GetTrainer().Team[choiceNumber-1].Item = 0;
                                } //end else
                                selection.SetActive(false);
                                choices.SetActive(false);
                                initialize = false;
                                gameState = MainGame.TEAM;
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
                                gameState = MainGame.POKEMONRIBBONS;
                                break;
                            } //end case 3 (Ribbons)
                            //Cancel
                            case 4:
                            {
                                choices.SetActive(false);
                                selection.SetActive(false);
                                gameState = MainGame.TEAM;
                                break;
                            } //end case 4 (Cancel)
                        } //end switch
                    } //end if Pokemon submenu on Continue Game -> My Team is Open
                    
                    //Pokemon Team on Continue Game -> My Team
                    else if(gameState == MainGame.TEAM)
                    {
                        //Open menu if not open, as long as player isn't selecting a button
                        if(choiceNumber > 0)
                        {
                            //Set submenu active
                            choices.SetActive(true);
                            selection.SetActive(true);

                            //Set up selection box at end of frame if it doesn't fit
                            if(selection.GetComponent<RectTransform>().sizeDelta != 
                               choices.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta)
                            {
                                selection.SetActive(false);
                                StartCoroutine("WaitForResize");
                            } //end if
                            
                            //Reset position to top of menu
                            subMenuChoice = 0;
                            initialize = false;
                            gameState = MainGame.POKEMONSUBMENU;
                        } //end if
                        //Open PC if choice is PC
                        else if(choiceNumber == -1)
                        {
                            StartCoroutine(LoadScene("PC", SceneManager.OverallGame.PC, true));
                        } //end else if
                        //Return to Home if choice is Cancel
                        else if(choiceNumber == 0)
                        {
                            EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
                            gameState = MainGame.HOME;
                        } //end else if
                    } //end else if Pokemon Team on Continue Game -> My Team
                    
                    //Pokemon Switch on Continue Game -> Switch
                    else if(gameState == MainGame.POKEMONSWITCH)
                    {
                        //If the choice equals the switch, don't run swap
                        if(choiceNumber != switchChoice)
                        {
                            //Switch function
                            GameManager.instance.GetTrainer().Swap(choiceNumber-1, switchChoice-1);
                        } //end if
                        
                        //Go back to team
                        initialize = false;
                        playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Button>().
                            interactable = true;
                        playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Button>().
                            interactable = true;
                        gameState = MainGame.TEAM;
                    } //end else if Pokemon Switch on Continue Game -> Switch

                    //Pokemon Summary on Continue Game -> Summary
                    else if(gameState == MainGame.POKEMONSUMMARY)
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
                            gameState = MainGame.MOVESWITCH;
                        } //end else if
                    } //end else if Pokemon Summary on Continue Game -> Summary
                    
                    //Move Switch on Continue Game -> Summary -> Move Details
                    else if(gameState  == MainGame.MOVESWITCH)
                    {
                        //If switching spots aren't the same
                        if(moveChoice != switchChoice)
                        {
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].SwitchMoves(moveChoice, switchChoice);
                        } //end if
                        
                        currentMoveSlot.GetComponent<Image>().color = Color.clear;
                        gameState = MainGame.POKEMONSUMMARY;
                    } //end else if Move Switch on Continue Game -> Summary -> Move Details

                    //Pokemon Ribbons on Continue Game -> Ribbons
                    else if(gameState  == MainGame.POKEMONRIBBONS)
                    {
                        //Make sure there are ribbons to be read
                        if(GameManager.instance.GetTrainer().Team[choiceNumber-1].GetRibbonCount() > 0)
                        {
                            selection.SetActive(!selection.activeSelf);
                            ReadRibbon();
                        } //end if
                    } //end else if Pokemon Ribbons on Continue Game -> Ribbons
                    break;
                } //end case OverallGame CONTINUEGAME
                //PC
                case OverallGame.PC:
                {
                    //PC Home
                    if(pcState == PCGame.HOME)
                    {
                        //Open submenu as long as player is in pokemon region
                        if(boxChoice > -1 && boxChoice < 30 && (selectedPokemon != null || heldPokemon != null))
                        {
                            //Set submenu active
                            choices.SetActive(true);
                            selection.SetActive(true);

                            //Set up selection box at end of frame if it doesn't fit
                            if(selection.GetComponent<RectTransform>().sizeDelta != 
                               choices.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta)
                            {
                                selection.SetActive(false);
                                StartCoroutine("WaitForResize");
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
                            StartCoroutine(LoadScene("MainGame", OverallGame.CONTINUE, true));
                        } //end else if
                    } //end if Home

                    //Pokemon Party on PC -> Party Tab
                    else if(pcState == PCGame.PARTY)
                    {
                        //Open submenu as long as player is in party
                        if(choiceNumber > 0 && (selectedPokemon != null || heldPokemon != null))
                        {
                            //Set submenu active
                            choices.SetActive(true);
                            selection.SetActive(true);

                            //Set up selection box at end of frame if it doesn't fit
                            if(selection.GetComponent<RectTransform>().sizeDelta != 
                               choices.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta)
                            {
                                selection.SetActive(false);
                                StartCoroutine("WaitForResize");
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
                                selection.SetActive(false);
                                choices.SetActive(false);
                                if(selectedPokemon.Item == 0)
                                {
                                    selectedPokemon.Item = GameManager.instance.RandomInt(1, 500);
                                } //end if
                                else
                                {
                                    selectedPokemon.Item = 0;
                                } //end else
                                pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
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
                                    //Get the pokemon
                                    selectedPokemon = GameManager.instance.GetTrainer().Team[choiceNumber-1];

                                    //Remove the pokemon from the party
                                    GameManager.instance.GetTrainer().RemovePokemon(choiceNumber-1);
                                    
                                    //Fill in party tab
                                    for(int i = 1; i < GameManager.instance.GetTrainer().Team.Count + 1; i++)
                                    {
                                        partyTab.transform.FindChild("Pokemon"+i).GetComponent<Image>().sprite =
                                            Resources.Load<Sprite>("Sprites/Icons/icon"+GameManager.instance.GetTrainer().
                                                                   Team[i-1].NatSpecies.ToString("000"));
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
                                choices.SetActive(false);
                                selection.SetActive(false);
                                GameManager.instance.DisplayText("You released " + selectedPokemon.Nickname, true);
                                selectedPokemon = null;
                                pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
                                break;
                            } //end case 5 (Release)

                            //Cancel
                            case 6:
                            {
                                choices.SetActive(false);
                                selection.SetActive(false);
                                pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
                                break;
                            } //end case 6 (Cancel)
                        } //end switch
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
                            //Fill in choices box
                            for (int i = choices.transform.childCount-1; i < 6; i++)
                            {
                                GameObject clone = Instantiate (choices.transform.GetChild (0).gameObject,
                                                                choices.transform.GetChild (0).position,
                                                                Quaternion.identity) as GameObject;
                                clone.transform.SetParent (choices.transform);
                            } //end for
                            choices.transform.GetChild (0).GetComponent<Text> ().text = "Move";
                            choices.transform.GetChild (1).GetComponent<Text> ().text = "Summary";
                            choices.transform.GetChild (2).GetComponent<Text> ().text = "Item";
                            choices.transform.GetChild (3).GetComponent<Text> ().text = "Ribbons";
                            choices.transform.GetChild (4).GetComponent<Text> ().text = "Markings";
                            choices.transform.GetChild (5).GetComponent<Text> ().text = "Release";
                            choices.transform.GetChild (6).GetComponent<Text> ().text = "Cancel";
                            choices.transform.GetChild (0).GetComponent<Text> ().color = Color.black;
                            choices.transform.GetChild (1).GetComponent<Text> ().color = Color.black;
                            choices.transform.GetChild (2).GetComponent<Text> ().color = Color.black;
                            choices.transform.GetChild (3).GetComponent<Text> ().color = Color.black;
                            choices.transform.GetChild (4).GetComponent<Text> ().color = Color.black;
                            choices.transform.GetChild (5).GetComponent<Text> ().color = Color.black;
                            choices.transform.GetChild (6).GetComponent<Text> ().color = Color.black;
                            if (choices.transform.childCount > 7)
                            {
                                for (int i = 7; i < choices.transform.childCount; i++)
                                {
                                    Destroy (choices.transform.GetChild (i).gameObject);
                                } //end for
                            } //end if
                            choices.SetActive(false);
                            selection.SetActive(false);
                            
                            //If holding a pokemon
                            if(heldPokemon != null)
                            {
                                //Update held pokemon markings
                                heldPokemon.SetMarkings(markingChoices.ToArray());
                                
                                //Return to home or party
                                pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
                            } //end if
                            //If in party
                            else if(partyTab.activeSelf)
                            {
                                //Update team pokemon markings
                                GameManager.instance.GetTrainer().Team[choiceNumber-1].SetMarkings(markingChoices.ToArray());

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
                            //Turn menu off
                            //Fill in choices box
                            for (int i = choices.transform.childCount-1; i < 6; i++)
                            {
                                GameObject clone = Instantiate (choices.transform.GetChild (0).gameObject,
                                                                choices.transform.GetChild (0).position,
                                                                Quaternion.identity) as GameObject;
                                clone.transform.SetParent (choices.transform);
                            } //end for
                            choices.transform.GetChild (0).GetComponent<Text> ().text = "Move";
                            choices.transform.GetChild (1).GetComponent<Text> ().text = "Summary";
                            choices.transform.GetChild (2).GetComponent<Text> ().text = "Item";
                            choices.transform.GetChild (3).GetComponent<Text> ().text = "Ribbons";
                            choices.transform.GetChild (4).GetComponent<Text> ().text = "Markings";
                            choices.transform.GetChild (5).GetComponent<Text> ().text = "Release";
                            choices.transform.GetChild (6).GetComponent<Text> ().text = "Cancel";
                            choices.transform.GetChild (0).GetComponent<Text> ().color = Color.black;
                            choices.transform.GetChild (1).GetComponent<Text> ().color = Color.black;
                            choices.transform.GetChild (2).GetComponent<Text> ().color = Color.black;
                            choices.transform.GetChild (3).GetComponent<Text> ().color = Color.black;
                            choices.transform.GetChild (4).GetComponent<Text> ().color = Color.black;
                            choices.transform.GetChild (5).GetComponent<Text> ().color = Color.black;
                            choices.transform.GetChild (6).GetComponent<Text> ().color = Color.black;
                            if (choices.transform.childCount > 7)
                            {
                                for (int i = 7; i < choices.transform.childCount; i++)
                                {
                                    Destroy (choices.transform.GetChild (i).gameObject);
                                } //end for
                            } //end if
                            choices.SetActive(false);
                            selection.SetActive(false);

                            //Return to home or party
                            pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
                        } //end else if
                    } //end else if Pokemon Markings on PC -> Submenu
                    break;
                } //end OverallGame PC
            } //end scene switch
        } //end else if Enter/Return Key

        /***********************************************
         * X Key
         ***********************************************/ 
        if(Input.GetKeyDown(KeyCode.X))
        {
            //Scene switch
            switch(sceneState)
            {
                //Intro
                case OverallGame.INTRO:
                {
                    break;
                } //end case OverallGame INTRO
                    
                    //Menu
                case OverallGame.MENU:
                {
                    break;
                } //end case OverallGame MENU
                    
                    //New Game
                case OverallGame.NEWGAME:
                {
                    break;
                } //end case OverallGame NEWGAME
                    
                    //Main Game scene
                case OverallGame.CONTINUE:
                {
                    //Pokemon Summary on Continue Game -> Summary
                    if(gameState == MainGame.POKEMONSUMMARY)
                    {
                        //If on any page besides details
                        if(summaryChoice != 5)
                        {
                            //Deactivate summary
                            summaryScreen.SetActive(false);
                            
                            //Enable buttons again
                            playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Button>().
                                interactable = true;
                            playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Button>().
                                interactable = true;
                            gameState = MainGame.TEAM;
                        } //end if
                        else
                        {
                            summaryScreen.transform.GetChild(5).gameObject.SetActive(false);
                            selection.SetActive(false);
                            summaryChoice = 4;
                        } //end else
                    } //end if Pokemon Summary on Continue Game -> Summary

                    //Pokemon submenu on Continue Game -> My Team
                    else if(gameState == MainGame.POKEMONSUBMENU)
                    {
                        //Deactivate submenu 
                        choices.SetActive(false);
                        selection.SetActive(false);                        

                        gameState = MainGame.TEAM;
                    } //end else if Pokemon submenu on Continue Game -> My Team

                    //Pokemon Ribbons on Continue Game -> Ribbons
                    else if(gameState == MainGame.POKEMONRIBBONS)
                    {
                        //Deactivate ribbons
                        ribbonScreen.SetActive(false);
                        selection.SetActive(false);
                        ribbonChoice = 0;
                        previousRibbonChoice = -1;
                        
                        //Enable buttons again
                        playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Button>().
                            interactable = true;
                        playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Button>().
                            interactable = true;
                        gameState = MainGame.TEAM;
                    } //end else if Pokemon Ribbons on Continue Game -> Ribbons

                    //Continue Game -> My Team
                    else if(gameState == MainGame.TEAM)
                    {
                        EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
                        gameState = MainGame.HOME;
                    } //end else if Continue Game -> My Team

                    //Pokemon Switch on Continue Game -> Switch
                    else if(gameState == MainGame.POKEMONSWITCH)
                    {
                        //Return to team
                        gameState = MainGame.TEAM;
                    } //end else if Pokemon Switch on Continue Game -> My Team

                    //Move Switch on Continue Game -> Summary -> Move Details
                    else if(gameState  == MainGame.MOVESWITCH)
                    {
                        //Return to summary
                        currentMoveSlot.GetComponent<Image>().color = Color.clear;
                        gameState = MainGame.POKEMONSUMMARY;
                    } //end else if Move Switch on Continue Game -> Summary -> Move Details

                    //Continue Game -> Gym Leader
                    else if(gameState == MainGame.GYMBATTLE)
                    {
                        EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
                        gameState = MainGame.HOME;
                    } //end else if 

                    //Continue Game -> Trainer Card
                    else if(gameState == MainGame.TRAINERCARD)
                    {
                        EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
                        gameState = MainGame.HOME;
                    } //end else if 

                    //Continue Game -> Debug Options
                    else if(gameState == MainGame.DEBUG)
                    {
                        EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
                        gameState = MainGame.HOME;
                    } //end else if 
                    break;
                } //end case OverallGame CONTINUEGAME
                //PC
                case OverallGame.PC:
                {
                    //PC Home
                    if(pcState == PCGame.HOME)
                    {
                        StartCoroutine(LoadScene("MainGame", OverallGame.CONTINUE, true));
                    } //end if PC Home

                    //Pokemon Submenu on PC
                    else if(pcState == PCGame.POKEMONSUBMENU)
                    {
                        choices.SetActive(false);
                        selection.SetActive(false);
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

                        //Return to home or party
                        pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
                    } //end else if Pokemon Ribbons on PC -> Ribbons

                    //Pokemon Markings on PC -> Submenu
                    else if(pcState == PCGame.POKEMONMARKINGS)
                    {
                        //Turn menu off
                        //Turn menu off
                        //Fill in choices box
                        for (int i = choices.transform.childCount-1; i < 6; i++)
                        {
                            GameObject clone = Instantiate (choices.transform.GetChild (0).gameObject,
                                                            choices.transform.GetChild (0).position,
                                                            Quaternion.identity) as GameObject;
                            clone.transform.SetParent (choices.transform);
                        } //end for
                        choices.transform.GetChild (0).GetComponent<Text> ().text = "Move";
                        choices.transform.GetChild (1).GetComponent<Text> ().text = "Summary";
                        choices.transform.GetChild (2).GetComponent<Text> ().text = "Item";
                        choices.transform.GetChild (3).GetComponent<Text> ().text = "Ribbons";
                        choices.transform.GetChild (4).GetComponent<Text> ().text = "Markings";
                        choices.transform.GetChild (5).GetComponent<Text> ().text = "Release";
                        choices.transform.GetChild (6).GetComponent<Text> ().text = "Cancel";
                        choices.transform.GetChild (0).GetComponent<Text> ().color = Color.black;
                        choices.transform.GetChild (1).GetComponent<Text> ().color = Color.black;
                        choices.transform.GetChild (2).GetComponent<Text> ().color = Color.black;
                        choices.transform.GetChild (3).GetComponent<Text> ().color = Color.black;
                        choices.transform.GetChild (4).GetComponent<Text> ().color = Color.black;
                        choices.transform.GetChild (5).GetComponent<Text> ().color = Color.black;
                        choices.transform.GetChild (6).GetComponent<Text> ().color = Color.black;
                        if (choices.transform.childCount > 7)
                        {
                            for (int i = 7; i < choices.transform.childCount; i++)
                            {
                                Destroy (choices.transform.GetChild (i).gameObject);
                            } //end for
                        } //end if
                        choices.SetActive(false);
                        selection.SetActive(false);
                        
                        //Return to home or party
                        pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
                    } //end else if Pokemon Markings on PC -> Submenu
                    break;
                } //end case OverallGame PC
            } //end scene switch
        } //end else if X Key
    } //end GatherInput
    #endregion

	//Miscellaneous functions
	#region Misc
    /***************************************
     * Name: Reset
     * Soft resets the game to intro screen
     ***************************************/
	public void Reset()
	{
		checkpoint = 0;
		processing = false;
        selection.SetActive (false);
        text.SetActive (false);
        confirm.SetActive (false);
        choices.SetActive (false);
        input.SetActive (false);
		StopAllCoroutines ();
	} //end Reset

    /***************************************
     * Name: SetCheckpoint
     * Sets checkpoint to parameter
     ***************************************/
    public void SetCheckpoint(int newCheckpoint)
    {
        checkpoint = newCheckpoint; 
    } //end SetCheckpoint(int newCheckpoint)

    /***************************************
     * Name: SetGameState
     * Sets gameState to parameter
     ***************************************/
    public IEnumerator SetGameState(MainGame newGameState)
    {
        //Process at end of frame
        yield return new WaitForEndOfFrame ();

        //Set the new state
        EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
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
     * Name: LoadScene
     * Resets checkpoint and loads scene
     ***************************************/ 
    public IEnumerator LoadScene(string sceneName, OverallGame state, bool fadeOut = false)
    {        
        //Process at end of frame
        yield return new WaitForEndOfFrame ();

        //If holding a pokemon
        if (heldPokemon != null)
        {
            GameManager.instance.DisplayText ("You can't leave while holding a pokemon", true);
            return false;
        } //end if

        //Load new scene when fade out is done
        if (fadeOut)
        {
            //Fade out
            playing = true;
            StartCoroutine (FadeOutAnimation (0));

            //Wait for fade out to finish
            while(playing)
            {
                yield return null;
            } //end while

            //Move to next scene
            processing = false;
            sceneState = state;
            Application.LoadLevel (sceneName);
        } //end if

        //Load new scene if fade out is false
        else
        {
            checkpoint = 0;
            processing = false;
            sceneState = state;
            Application.LoadLevel (sceneName);
        } //end else
    } //end LoadScene(string sceneName, OverallGame state)

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
            pcState = PCGame.PARTY;
        } //end if
        //Party to be closed
        else
        {
            partyTab.SetActive(false);
            pcState = PCGame.HOME;
        } //end else
    } //end PartyState(bool state)
	#endregion
    #endregion
} //end SceneManager class
