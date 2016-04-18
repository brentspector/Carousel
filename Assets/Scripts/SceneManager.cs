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
        CONTINUE
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
        PC,
        SHOP,
        POKEDEX,
        TRAINERCARD
    } //end MainGame

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
			checkpoint = 0;
            sceneState = OverallGame.MENU;
			Application.LoadLevel("StartMenu");
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
			// First choice selected, this is usually continue
			if(choiceNumber == 0)
			{
                sceneState = OverallGame.CONTINUE;
				GameManager.instance.Continue();
			} //end if
			// Second choice selected, this is usually new game
			else if(choiceNumber == 1)
			{
                sceneState = OverallGame.NEWGAME;
				GameManager.instance.NewGame();
			} //end else if
			// Third choice selected, this is usually options
			else if(choiceNumber == 2)
			{
                sceneState = OverallGame.INTRO;
				GameManager.instance.Options();
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
			processing = true;
			//Attempt to display text
			text.SetActive(true);
			GameManager.instance.InitText(text.transform.GetChild(0), text.transform.GetChild(1));
			if(GameManager.instance.DisplayText("Welcome to Pokemon Carousel! I am the Ringmaster " +
			                                 "Melotta."))
			{
				checkpoint = 3;
			} //end if
			processing = false;
		} //end else if
		else if(checkpoint == 3)
		{
			processing = true;

            //Get player input
            GatherInput();

			processing = false;
		} //end else if
		else if(checkpoint == 4)
		{
			processing = true;

            //Get player input
            GatherInput();

			processing = false;
		} //end else if
		else if(checkpoint == 5)
		{
			processing = true;

            //Get player input
            GatherInput();

			processing = false;
		} //end else if
		else if(checkpoint == 6)
		{
			processing = true;
			
            //Make sure text field is always active
			if(input.activeInHierarchy)
			{
				inputText.ActivateInputField();
			} //end if

            //Get player input
            GatherInput();

			processing = false;
		} //end else if
		else if(checkpoint == 7)
		{
			processing = true;
			//Make sure text has finished displaying before activating the confirm box
			if(!GameManager.instance.IsDisplaying())
			{
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
			} //end if
			processing = false;
		} //end else if
		else if(checkpoint == 8)
		{
			processing = true;

            //Get player input
            GatherInput();

            processing = false;
		} //end else if
		else if(checkpoint == 9)
		{
			processing = true;
			//Set name
			GameManager.instance.GetTrainer().PlayerName = playerName;

			//Attempt to display text
			if(GameManager.instance.DisplayText("Great! Now here's your things. See you again."))
			{
				checkpoint = 10;
			} //end if
			processing = false;
		} //end else if
		else if(checkpoint == 10)
		{
			processing = true;

            //Get player input
            GatherInput();

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

            //Disable screens
            gymBattle.SetActive(false);
            playerTeam.SetActive(false);
            summaryScreen.SetActive(false);
            ribbonScreen.SetActive(false);
            trainerCard.SetActive(false);

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
                initialize = false;
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
                    for(int i = 5; i > GameManager.instance.GetTrainer().Team.Count-1; i--)
                    {
                        playerTeam.transform.FindChild("Background").GetChild(i).gameObject.SetActive(false);
                        playerTeam.transform.FindChild("Pokemon" + (i+1)).gameObject.SetActive(false);
                    } //end if Count < 6

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
                    } //end else if
                    else if(choiceNumber == 0)
                    {
                        playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Image>().color = Color.gray*2;
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
                //Get player input
                GatherInput();
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
                        GameManager.instance.GetTrainer().Team[choiceNumber-1].GetMarkings();
                    ribbonScreen.transform.FindChild("Item").GetComponent<Text>().text=
                        DataContents.GetItemGameName(GameManager.instance.GetTrainer().Team[choiceNumber-1].Item);
                    ribbonScreen.transform.FindChild("RibbonName").gameObject.SetActive(false);
                    ribbonScreen.transform.FindChild("RibbonDescription").gameObject.SetActive(false);

                    //No ribbon selected yet
                    previousRibbonChoice = -1;
                    ribbonChoice = 0;

                    //Set existing ribbons to clear
                    foreach(Transform child in ribbonScreen.transform.FindChild("RibbonRegion").transform)
                    {
                        child.GetComponent<Image>().color = Color.clear;
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
                            newRibbon.GetComponent<Image>().color = Color.white;
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
                switch(summaryChoice)
                {
                    //Info screen
                    case 0:
                    {
                        summaryScreen.transform.GetChild(0).gameObject.SetActive(true);
                        summaryScreen.transform.GetChild(0).FindChild("Name").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].Nickname;
                        summaryScreen.transform.GetChild(0).FindChild("Level").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].CurrentLevel.ToString();
                        summaryScreen.transform.GetChild(0).FindChild("Ball").GetComponent<Image>().sprite=
                            Resources.Load<Sprite>("Sprites/Icons/summaryBall"+GameManager.instance.GetTrainer().
                            Team[choiceNumber-1].BallUsed.ToString("00"));
                        summaryScreen.transform.GetChild(0).FindChild("Gender").GetComponent<Image>().sprite=
                            Resources.Load<Sprite>("Sprites/Icons/gender"+GameManager.instance.GetTrainer().
                            Team[choiceNumber-1].Gender.ToString());
                        summaryScreen.transform.GetChild(0).FindChild("Sprite").GetComponent<Image>().sprite=
                            Resources.Load<Sprite>("Sprites/Pokemon/"+GameManager.instance.GetTrainer().
                            Team[choiceNumber-1].NatSpecies.ToString("000"));
                        summaryScreen.transform.GetChild(0).FindChild("Markings").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].GetMarkings();
                        summaryScreen.transform.GetChild(0).FindChild("Item").GetComponent<Text>().text=
                            DataContents.GetItemGameName(GameManager.instance.GetTrainer().Team[choiceNumber-1].Item);
                        summaryScreen.transform.GetChild(0).FindChild("DexNumber").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].NatSpecies.ToString();
                        summaryScreen.transform.GetChild(0).FindChild("Species").GetComponent<Text>().text=
                            DataContents.ExecuteSQL<String>("SELECT name FROM Pokemon WHERE rowid=" +
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].NatSpecies); 
                        summaryScreen.transform.GetChild(0).FindChild("OT").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].OTName;
                        summaryScreen.transform.GetChild(0).FindChild("IDNumber").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].TrainerID.ToString();
                        summaryScreen.transform.GetChild(0).FindChild("CurrentXP").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].CurrentEXP.ToString();
                        summaryScreen.transform.GetChild(0).FindChild("RemainingXP").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].RemainingEXP.ToString();
                        SetTypeSprites(summaryScreen.transform.GetChild(0).FindChild("Types").GetChild(0).GetComponent<Image>(),
                                       summaryScreen.transform.GetChild(0).FindChild("Types").GetChild(1).GetComponent<Image>());
                        break;
                    } //end case 0 (Info)
                    //Memo screen
                    case 1:
                    {
                        summaryScreen.transform.GetChild(1).gameObject.SetActive(true);
                        summaryScreen.transform.GetChild(1).FindChild("Name").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].Nickname;
                        summaryScreen.transform.GetChild(1).FindChild("Level").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].CurrentLevel.ToString();
                        summaryScreen.transform.GetChild(1).FindChild("Ball").GetComponent<Image>().sprite=
                            Resources.Load<Sprite>("Sprites/Icons/summaryBall"+GameManager.instance.GetTrainer().
                            Team[choiceNumber-1].BallUsed.ToString("00"));
                        summaryScreen.transform.GetChild(1).FindChild("Gender").GetComponent<Image>().sprite=
                            Resources.Load<Sprite>("Sprites/Icons/gender"+GameManager.instance.GetTrainer().
                            Team[choiceNumber-1].Gender.ToString());
                        summaryScreen.transform.GetChild(1).FindChild("Sprite").GetComponent<Image>().sprite=
                            Resources.Load<Sprite>("Sprites/Pokemon/"+GameManager.instance.GetTrainer().
                            Team[choiceNumber-1].NatSpecies.ToString("000"));
                        summaryScreen.transform.GetChild(1).FindChild("Markings").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].GetMarkings();
                        summaryScreen.transform.GetChild(1).FindChild("Item").GetComponent<Text>().text=
                            DataContents.GetItemGameName(GameManager.instance.GetTrainer().Team[choiceNumber-1].Item);
                        summaryScreen.transform.GetChild(1).FindChild("Nature").GetComponent<Text>().text= 
                            "<color=#cc0000ff>" + ((Natures)GameManager.instance.GetTrainer().Team[choiceNumber-1].
                                 Nature).ToString() + "</color> nature";
                        summaryScreen.transform.GetChild(1).FindChild("CaughtDate").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].ObtainTime.ToLongDateString() + 
                            " at " + GameManager.instance.GetTrainer().Team[choiceNumber-1].ObtainTime.
                            ToShortTimeString();
                        summaryScreen.transform.GetChild(1).FindChild("CaughtType").GetComponent<Text>().text=
                            ((ObtainType)GameManager.instance.GetTrainer().Team[choiceNumber-1].ObtainType).ToString()
                            + " from " +
                            ((ObtainFrom)GameManager.instance.GetTrainer().Team[choiceNumber-1].ObtainFrom).ToString();
                        summaryScreen.transform.GetChild(1).FindChild("CaughtLevel").GetComponent<Text>().text=
                            "Found at level " + GameManager.instance.GetTrainer().Team[choiceNumber-1].ObtainLevel;
                        break;
                    } //end case 1 (Memo)
                    //Stats
                    case 2:
                    {
                        summaryScreen.transform.GetChild(2).gameObject.SetActive(true);
                        summaryScreen.transform.GetChild(2).FindChild("Name").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].Nickname;
                        summaryScreen.transform.GetChild(2).FindChild("Level").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].CurrentLevel.ToString();
                        summaryScreen.transform.GetChild(2).FindChild("Ball").GetComponent<Image>().sprite=
                            Resources.Load<Sprite>("Sprites/Icons/summaryBall"+GameManager.instance.GetTrainer().
                            Team[choiceNumber-1].BallUsed.ToString("00"));
                        summaryScreen.transform.GetChild(2).FindChild("Gender").GetComponent<Image>().sprite=
                            Resources.Load<Sprite>("Sprites/Icons/gender"+GameManager.instance.GetTrainer().
                            Team[choiceNumber-1].Gender.ToString());
                        summaryScreen.transform.GetChild(2).FindChild("Sprite").GetComponent<Image>().sprite=
                            Resources.Load<Sprite>("Sprites/Pokemon/"+GameManager.instance.GetTrainer().
                            Team[choiceNumber-1].NatSpecies.ToString("000"));
                        summaryScreen.transform.GetChild(2).FindChild("Markings").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].GetMarkings();
                        summaryScreen.transform.GetChild(2).FindChild("Item").GetComponent<Text>().text=
                            DataContents.GetItemGameName(GameManager.instance.GetTrainer().Team[choiceNumber-1].Item);
                        summaryScreen.transform.GetChild(2).FindChild("HP").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].CurrentHP.ToString() +
                            "/" + GameManager.instance.GetTrainer().Team[choiceNumber-1].TotalHP.ToString();
                        summaryScreen.transform.GetChild(2).FindChild("RemainingHP").
                            GetComponent<RectTransform>().localScale = new Vector3((float)GameManager.instance.
                            GetTrainer().Team[choiceNumber-1].CurrentHP/(float)GameManager.instance.GetTrainer().
                            Team[choiceNumber-1].TotalHP, 1f, 1f);
                        summaryScreen.transform.GetChild(2).FindChild("Attack").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].Attack.ToString();
                        summaryScreen.transform.GetChild(2).FindChild("Defense").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].Defense.ToString();
                        summaryScreen.transform.GetChild(2).FindChild("SpAttack").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].SpecialA.ToString();
                        summaryScreen.transform.GetChild(2).FindChild("SpDefense").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].SpecialD.ToString();
                        summaryScreen.transform.GetChild(2).FindChild("Speed").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].Speed.ToString();
                        summaryScreen.transform.GetChild(2).FindChild("AbilityName").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].GetAbilityName();
                        summaryScreen.transform.GetChild(2).FindChild("AbilityDescription").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].GetAbilityDescription();
                        SetStatColor();
                        break;
                    } //end case 2 (Stats)
                    //EV-IV
                    case 3:
                    {
                        summaryScreen.transform.GetChild(3).gameObject.SetActive(true);
                        summaryScreen.transform.GetChild(3).FindChild("Name").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].Nickname;
                        summaryScreen.transform.GetChild(3).FindChild("Level").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].CurrentLevel.ToString();
                        summaryScreen.transform.GetChild(3).FindChild("Ball").GetComponent<Image>().sprite=
                            Resources.Load<Sprite>("Sprites/Icons/summaryBall"+GameManager.instance.GetTrainer().
                            Team[choiceNumber-1].BallUsed.ToString("00"));
                        summaryScreen.transform.GetChild(3).FindChild("Gender").GetComponent<Image>().sprite=
                            Resources.Load<Sprite>("Sprites/Icons/gender"+GameManager.instance.GetTrainer().
                            Team[choiceNumber-1].Gender.ToString());
                        summaryScreen.transform.GetChild(3).FindChild("Sprite").GetComponent<Image>().sprite=
                            Resources.Load<Sprite>("Sprites/Pokemon/"+GameManager.instance.GetTrainer().
                            Team[choiceNumber-1].NatSpecies.ToString("000"));
                        summaryScreen.transform.GetChild(3).FindChild("Markings").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].GetMarkings();
                        summaryScreen.transform.GetChild(3).FindChild("Item").GetComponent<Text>().text=
                            DataContents.GetItemGameName(GameManager.instance.GetTrainer().Team[choiceNumber-1].Item);
                        summaryScreen.transform.GetChild(3).FindChild("HP").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].GetEV(0).ToString() +
                            "/" + GameManager.instance.GetTrainer().Team[choiceNumber-1].GetIV(0).ToString();
                        summaryScreen.transform.GetChild(3).FindChild("RemainingHP").
                            GetComponent<RectTransform>().localScale = new Vector3((float)GameManager.instance.
                            GetTrainer().Team[choiceNumber-1].CurrentHP/(float)GameManager.instance.GetTrainer().
                            Team[choiceNumber-1].TotalHP, 1f, 1f);
                        summaryScreen.transform.GetChild(3).FindChild("Attack").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].GetEV(1).ToString() +
                            "/" + GameManager.instance.GetTrainer().Team[choiceNumber-1].GetIV(1).ToString();
                        summaryScreen.transform.GetChild(3).FindChild("Defense").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].GetEV(2).ToString() +
                            "/" + GameManager.instance.GetTrainer().Team[choiceNumber-1].GetIV(2).ToString();
                        summaryScreen.transform.GetChild(3).FindChild("SpAttack").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].GetEV(4).ToString() +
                            "/" + GameManager.instance.GetTrainer().Team[choiceNumber-1].GetIV(4).ToString();
                        summaryScreen.transform.GetChild(3).FindChild("SpDefense").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].GetEV(5).ToString() +
                            "/" + GameManager.instance.GetTrainer().Team[choiceNumber-1].GetIV(5).ToString();
                        summaryScreen.transform.GetChild(3).FindChild("Speed").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].GetEV(3).ToString() +
                            "/" + GameManager.instance.GetTrainer().Team[choiceNumber-1].GetIV(3).ToString();
                        summaryScreen.transform.GetChild(3).FindChild("AbilityName").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].GetAbilityName();
                        summaryScreen.transform.GetChild(3).FindChild("AbilityDescription").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].GetAbilityDescription();
                        break;
                    } //end case 3 (EV-IV)
                    //Moves
                    case 4:
                    {
                        summaryScreen.transform.GetChild(4).gameObject.SetActive(true);
                        summaryScreen.transform.GetChild(4).FindChild("Name").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].Nickname;
                        summaryScreen.transform.GetChild(4).FindChild("Level").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].CurrentLevel.ToString();
                        summaryScreen.transform.GetChild(4).FindChild("Ball").GetComponent<Image>().sprite=
                            Resources.Load<Sprite>("Sprites/Icons/summaryBall"+GameManager.instance.GetTrainer().
                            Team[choiceNumber-1].BallUsed.ToString("00"));
                        summaryScreen.transform.GetChild(4).FindChild("Gender").GetComponent<Image>().sprite=
                            Resources.Load<Sprite>("Sprites/Icons/gender"+GameManager.instance.GetTrainer().
                            Team[choiceNumber-1].Gender.ToString());
                        summaryScreen.transform.GetChild(4).FindChild("Sprite").GetComponent<Image>().sprite=
                            Resources.Load<Sprite>("Sprites/Pokemon/"+GameManager.instance.GetTrainer().
                            Team[choiceNumber-1].NatSpecies.ToString("000"));
                        summaryScreen.transform.GetChild(4).FindChild("Markings").GetComponent<Text>().text=
                            GameManager.instance.GetTrainer().Team[choiceNumber-1].GetMarkings();
                        summaryScreen.transform.GetChild(4).FindChild("Item").GetComponent<Text>().text=
                            DataContents.GetItemGameName(GameManager.instance.GetTrainer().Team[choiceNumber-1].Item);
                        SetMoveSprites(GameManager.instance.GetTrainer().Team[choiceNumber-1],
                                       summaryScreen.transform.GetChild(4));
                        break;
                    } //end case 4 (Moves)
                    //Move Details
                    case 5:
                    {
                        summaryScreen.transform.GetChild(5).gameObject.SetActive(true);
                        summaryScreen.transform.GetChild(5).FindChild("Sprite").GetComponent<Image>().sprite=
                            Resources.Load<Sprite>("Sprites/Icons/icon"+GameManager.instance.GetTrainer().
                            Team[choiceNumber-1].NatSpecies.ToString("000"));
                        SetMoveDetails(GameManager.instance.GetTrainer().Team[choiceNumber-1],
                                       summaryScreen.transform.GetChild(5));
                        SetTypeSprites(summaryScreen.transform.GetChild(5).FindChild("SpeciesTypes").GetChild(0).GetComponent<Image>(),
                                       summaryScreen.transform.GetChild(5).FindChild("SpeciesTypes").GetChild(1).GetComponent<Image>());
                        SetMoveSprites(GameManager.instance.GetTrainer().Team[choiceNumber-1],
                                       summaryScreen.transform.GetChild(5));
                        break;
                    } //end case 5 (Move Details)
                } //end switch
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
                Vector3 scale = new Vector3(moveScreen.FindChild("Move"+(moveChoice+1)).GetComponent<RectTransform>().rect.width,
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

            //End processing
            processing = false;
        } //end else if
    } //end ContinueGame
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
    void SetTypeSprites(Image type1, Image type2)
    {
        //Set the primary (first) type
        type1.sprite = DataContents.typeSprites[Convert.ToInt32(Enum.Parse(typeof(Types),
                       DataContents.ExecuteSQL<string>("SELECT type1 FROM Pokemon WHERE rowid=" +
                       GameManager.instance.GetTrainer().Team[choiceNumber-1].NatSpecies)))];

        //Get the string for the secondary type
        string type2SQL = DataContents.ExecuteSQL<string> ("SELECT type2 FROM Pokemon WHERE rowid=" +
                          GameManager.instance.GetTrainer ().Team [choiceNumber - 1].NatSpecies); 

        //If a second type exists, load the appropriate sprite
        if (!String.IsNullOrEmpty (type2SQL))
        {
            type2.gameObject.SetActive(true);
            type2.sprite = DataContents.typeSprites [Convert.ToInt32 (Enum.Parse (typeof(Types), type2SQL))];
        } //end if
        //Otherwise disable the image
        else
        {
            type2.gameObject.SetActive(false);
        } //end else
    } //end SetTypeSprites(Image type1, Image type2)

    /***************************************
     * Name: SetStatColor
     * Sets the color for stat ups and downs
     ***************************************/
    void SetStatColor()
    {
        /*Attack, Defence, Speed, SP Attack, SP Defence*/
        //Get the pokemon's nature
        int currentNature = GameManager.instance.GetTrainer ().Team [choiceNumber - 1].Nature;

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
    } //end SetStatColor()

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
            if( GameManager.instance.GetTrainer ().Team [choiceNumber - 1].GetMove(i) != -1)
            {
                //Set the move type
                moveScreen.FindChild("Move" + (i+1).ToString()).gameObject.SetActive(true);
                moveScreen.FindChild("Move" + (i+1).ToString()).GetChild(0).GetComponent<Image>().sprite = 
                    DataContents.typeSprites [Convert.ToInt32 (Enum.Parse (typeof(Types),
                    DataContents.ExecuteSQL<string> ("SELECT type FROM Moves WHERE rowid=" +
                    GameManager.instance.GetTrainer ().Team [choiceNumber - 1].GetMove(i))))];
                //Set the move name
                moveScreen.FindChild("Move" + (i+1).ToString()).GetChild(1).GetComponent<Text>().text = 
                    DataContents.ExecuteSQL<string> ("SELECT gameName FROM Moves WHERE rowid=" +
                    GameManager.instance.GetTrainer ().Team [choiceNumber - 1].GetMove(i));

                //Set the move PP
                moveScreen.FindChild("Move" + (i+1).ToString()).GetChild(2).GetComponent<Text>().text = "PP " +
                    GameManager.instance.GetTrainer ().Team [choiceNumber - 1].GetMovePP(i).ToString() + "/" +
                    DataContents.ExecuteSQL<string> ("SELECT totalPP FROM Moves WHERE rowid=" +
                    GameManager.instance.GetTrainer ().Team [choiceNumber - 1].GetMove(i));
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
            GameManager.instance.GetTrainer ().Team [choiceNumber - 1].GetMove(moveChoice))))];

        //Set the move power
        int temp = DataContents.ExecuteSQL<int> ("SELECT baseDamage FROM Moves WHERE rowid=" +
                      GameManager.instance.GetTrainer ().Team [choiceNumber - 1].GetMove(moveChoice));
        moveScreen.FindChild ("Power").GetComponent<Text> ().text = temp > 1 ? temp.ToString () : "---";
            
        //Set the move accuracy
        temp = DataContents.ExecuteSQL<int> ("SELECT accuracy FROM Moves WHERE rowid=" +
               GameManager.instance.GetTrainer ().Team [choiceNumber - 1].GetMove(moveChoice));
        moveScreen.FindChild("Accuracy").GetComponent<Text>().text = temp >= 1 ? temp.ToString () : "---";

        //Set font size of move description
        if (detailsSize != -1)
        {
            //Set the move description text
            moveScreen.FindChild ("MoveDescription").GetComponent<Text> ().text = 
                DataContents.ExecuteSQL<string> ("SELECT description FROM Moves WHERE rowid=" +
                GameManager.instance.GetTrainer ().Team [choiceNumber - 1].GetMove (moveChoice));
        } //end if
        else
        {
            //Get font size
            moveScreen.FindChild ("MoveDescription").GetComponent<Text> ().text = 
                DataContents.ExecuteSQL<string> ("SELECT description FROM Moves WHERE gameName='Rollout'");
            StartCoroutine (WaitForFontResize (moveScreen));
        } //end else
    } //end SetMoveDetails(Pokemon teamMember, Transform moveScreen)

    /***************************************
     * Name: ReadRibbon
     * Sets ribbon details based on ribbon selection
     ***************************************/
    public void ReadRibbon()
    {
        //If text isn't displayed
        if (ribbonChoice != previousRibbonChoice && selection.activeSelf)
        {
            //Activate the fields
            ribbonScreen.transform.FindChild("RibbonName").gameObject.SetActive(true);
            ribbonScreen.transform.FindChild("RibbonDescription").gameObject.SetActive(true);

            //Position selection rectangle
            selection.transform.position = Camera.main.WorldToScreenPoint(ribbonScreen.transform.
                FindChild("RibbonRegion").GetChild(ribbonChoice).GetComponent<RectTransform>().position);

            //Get the ribbon value at the index
            int ribbonValue = GameManager.instance.GetTrainer().Team[choiceNumber-1].GetRibbon(ribbonChoice);

            //Set the name and description
            ribbonScreen.transform.FindChild("RibbonName").GetComponent<Text>().text = 
                DataContents.ribbonData.GetRibbonName(ribbonChoice);
            ribbonScreen.transform.FindChild("RibbonDescription").GetComponent<Text>().text = 
                DataContents.ribbonData.GetRibbonDescription(ribbonChoice);

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
    } //end ReadRibbon()

    /***************************************
     * Name: WaitForResize
     * Waits for choice menu to resize before 
     * setting selection dimensions
     ***************************************/
    IEnumerator WaitForResize()
    {
        yield return new WaitForEndOfFrame ();
        if (gameState == MainGame.POKEMONSUBMENU)
        {
            Vector3 scale = new Vector3 (choices.GetComponent<RectTransform> ().rect.width,
                                        choices.GetComponent<RectTransform> ().rect.height /
                choices.transform.childCount, 0);
            selection.GetComponent<RectTransform> ().sizeDelta = scale;
            selection.transform.position = choices.transform.GetChild (0).
                GetComponent<RectTransform> ().position;
            selection.SetActive (true);
        } //end if
        else if (gameState == MainGame.POKEMONRIBBONS)
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
    IEnumerator WaitForFontResize(Transform moveScreen)
    {
        yield return new WaitForEndOfFrame ();
        detailsSize = moveScreen.FindChild("MoveDescription").GetComponent<Text>().cachedTextGenerator.
            fontSizeUsedForBestFit;
        moveScreen.FindChild("MoveDescription").GetComponent<Text>().fontSize = detailsSize;
        moveScreen.FindChild("MoveDescription").GetComponent<Text>().resizeTextForBestFit = false;
        moveScreen.FindChild ("MoveDescription").GetComponent<Text> ().text = 
            DataContents.ExecuteSQL<string> ("SELECT description FROM Moves WHERE rowid=" +
            GameManager.instance.GetTrainer ().Team [choiceNumber - 1].GetMove (moveChoice));
    } //end WaitForFontResize(Transform moveScreen)

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
                        
                        //Clamp between 0 and ribbon length
                        if(ribbonChoice < 0)
                        {
                            ribbonChoice = GameManager.instance.GetTrainer().Team[choiceNumber-1].GetRibbonCount()-1;
                        } //end if
                        
                        //Set currentRibbonSlot
                        currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
                            GetChild(ribbonChoice).gameObject;

                        //Read ribbon
                        ReadRibbon();
                    } //end else if Pokemon Ribbons on Continue Game -> Ribbons
                    break;
                } //end case OverallState CONTINUEGAME
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
                        
                        //Clamp between 0 and ribbon length
                        if(ribbonChoice >= GameManager.instance.GetTrainer().Team[choiceNumber-1].GetRibbonCount())
                        {
                            ribbonChoice = 0;
                        } //end if
                        
                        //Set currentRibbonSlot
                        currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
                            GetChild(ribbonChoice).gameObject;

                        //Read ribbon
                        ReadRibbon();
                    } //end else if Pokemon Ribbons on Continue Game -> Ribbons
                    break;
                } // end case OverallGame CONTINUEGAME
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
                            moveChoice++;
                            if(moveChoice >= GameManager.instance.GetTrainer().Team[choiceNumber-1].GetMoveCount())
                            {
                                moveChoice--;
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
                        switchChoice++;
                        if(switchChoice >= GameManager.instance.GetTrainer().Team[choiceNumber-1].GetMoveCount())
                        {
                            switchChoice--;
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
                        if(ribbonChoice-1 > -1)
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
                        if(ribbonChoice + 1 < GameManager.instance.GetTrainer().Team[choiceNumber-1].GetRibbonCount())
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
                    //Don't continue until player requests next text
                    if(checkpoint == 3)
                    {
                        //Attempt to display text
                        if(GameManager.instance.DisplayText("This circus has attracted major gym leaders from " +
                                                            "around the world! In fact, that's why you're here, isn't it?"))
                        {
                            checkpoint = 4;
                        } //end if
                    } //end if

                    //Don't continue until player requests next text
                    else if(checkpoint == 4)
                    {
                        //Attempt to display text
                        if(GameManager.instance.DisplayText("Alright, let's get you set up. First, what is " +
                                                            "your name?"))
                        {
                            checkpoint = 5;
                        } //end if
                    } //end else if

                    //Don't continue until player requests next text
                    else if(checkpoint == 5)
                    {
                        //Display name input
                        input.SetActive(true);
                        input.transform.GetChild(2).GetComponent<Text>().text = "Please enter your name.";
                        input.transform.GetChild(0).GetComponent<Text>().text = "Player name:";
                        inputText.text = "";
                        inputText.ActivateInputField();
                        checkpoint = 6;
                    } //end else if

                    //Don't continue until player requests next text
                    else if(checkpoint == 6 && inputText.text.Length != 0)
                    {
                        //Convert input name to player's name
                        playerName = inputText.text;
                        input.SetActive(false);
                        GameManager.instance.DisplayText("So your name is " + playerName + "?");
                        checkpoint = 7;
                    } //end else if

                    //If an option was selected, process it
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
                            GameManager.instance.DisplayText("Ok let's try again. What is your name?");
                            checkpoint = 5;
                        } //end else if
                        
                        //Disable choice and selection
                        selection.SetActive(false);
                        confirm.SetActive(false);
                    } //end else if

                    //Don't continue until player requests next text
                    else if(checkpoint == 10 && !GameManager.instance.IsDisplaying())
                    {
                        text.SetActive(false);
                        checkpoint = 0;
                        GameManager.instance.GetTrainer().RandomTeam();
                        GameManager.instance.Persist();
                        sceneState = OverallGame.INTRO;
                        Application.LoadLevel("Intro");
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
                                    UnityEngine.Random.Range(1, 500);
                                }
                                else
                                {
                                    GameManager.instance.GetTrainer().Team[choiceNumber-1].Item = 0;
                                } //end else
                                selection.SetActive(false);
                                choices.SetActive(false);
                                initialize = false;
                                playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Button>().
                                    interactable = true;
                                playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Button>().
                                    interactable = true;
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
                                playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Button>().
                                    interactable = true;
                                playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Button>().
                                    interactable = true;
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
                        
                        //Disable PC and Cancel buttons
                        playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Button>().
                            interactable = false;
                        playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Button>().
                            interactable = false;
                        
                        //Set up selection box at end of frame if it doesn't fit
                        if(selection.GetComponent<RectTransform>().sizeDelta != 
                           choices.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta)
                        {
                            selection.SetActive(false);
                            StartCoroutine("WaitForResize");
                        } //end if
                        
                        //Reset position to top of menu
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
                            detailsSize = -1;
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
                        
                        //Enable buttons again
                        playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Button>().
                            interactable = true;
                        playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Button>().
                            interactable = true;
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
                    } //end else if Pokemon submenuRibbons on Continue Game -> Ribbons

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
                        playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Button>().
                            interactable = true;
                        playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Button>().
                            interactable = true;
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
                    break;
                } //end case OverallGame CONTINUEGAME
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
                } //end OverallGame CONTINUEGAME
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
                } //end OverallGame CONTINUEGAME
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
                    //Don't continue until player requests next text
                    if(checkpoint == 3)
                    {
                        //Attempt to display text
                        if(GameManager.instance.DisplayText("This circus has attracted major gym leaders from " +
                                                            "around the world! In fact, that's why you're here, isn't it?"))
                        {
                            checkpoint = 4;
                        } //end if
                    } //end if

                    //Don't continue until player requests next text
                    else if(checkpoint == 4)
                    {
                        //Attempt to display text
                        if(GameManager.instance.DisplayText("Alright, let's get you set up. First, what is " +
                                                            "your name?"))
                        {
                            checkpoint = 5;
                        } //end if
                    } //end else if

                    //Don't continue until player requests next text
                    else if(checkpoint == 5)
                    {
                        //Display name input
                        input.SetActive(true);
                        input.transform.GetChild(2).GetComponent<Text>().text = "Please enter your name.";
                        input.transform.GetChild(0).GetComponent<Text>().text = "Player name:";
                        inputText.text = "";
                        inputText.ActivateInputField();
                        checkpoint = 6;
                    } //end else if

                    //Don't continue until player requests next text
                    else if(checkpoint == 6 && inputText.text.Length != 0)
                    {
                        //Convert input name to player's name
                        playerName = inputText.text;
                        input.SetActive(false);
                        GameManager.instance.DisplayText("So your name is " + playerName + "?");
                        checkpoint = 7;
                    } //end else if

                    //If an option was selected, process it
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
                            GameManager.instance.DisplayText("Ok let's try again. What is your name?");
                            checkpoint = 5;
                        } //end else if
                        
                        //Disable choice and selection
                        selection.SetActive(false);
                        confirm.SetActive(false);
                    } //end else if

                    //Don't continue until player requests next text
                    else if(checkpoint == 10 && !GameManager.instance.IsDisplaying())
                    {
                        text.SetActive(false);
                        checkpoint = 0;
                        GameManager.instance.GetTrainer().RandomTeam();
                        GameManager.instance.Persist();
                        sceneState = OverallGame.INTRO;
                        Application.LoadLevel("Intro");
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
                                        UnityEngine.Random.Range(1, 500);
                                }
                                else
                                {
                                    GameManager.instance.GetTrainer().Team[choiceNumber-1].Item = 0;
                                } //end else
                                selection.SetActive(false);
                                choices.SetActive(false);
                                initialize = false;
                                playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Button>().
                                    interactable = true;
                                playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Button>().
                                    interactable = true;
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
                                playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Button>().
                                    interactable = true;
                                playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Button>().
                                    interactable = true;
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
                            
                            //Disable PC and Cancel buttons
                            playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Button>().
                                interactable = false;
                            playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Button>().
                                interactable = false;
                            
                            //Set up selection box at end of frame if it doesn't fit
                            if(selection.GetComponent<RectTransform>().sizeDelta != 
                               choices.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta)
                            {
                                selection.SetActive(false);
                                StartCoroutine("WaitForResize");
                            } //end if
                            
                            //Reset position to top of menu
                            subMenuChoice = 0;
                            gameState = MainGame.POKEMONSUBMENU;
                        } //end if
                        //Open PC if choice is PC
                        else if(choiceNumber == -1)
                        {

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
                            detailsSize = -1;
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
                        
                        //Enable buttons again
                        playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Button>().
                            interactable = true;
                        playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Button>().
                            interactable = true;
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
                    } //end else if Pokemon submenuRibbons on Continue Game -> Ribbons

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
                    break;
                } //end case OverallGame CONTINUEGAME
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
                summaryScreen.transform.GetChild(summaryChoice).gameObject.SetActive(false);
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
	#endregion
    #endregion
} //end SceneManager class
