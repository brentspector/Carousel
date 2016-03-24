/***************************************************************************************** 
 * File:    SceneManager.cs
 * Summary: Controls behavior for each scene in the game
 *****************************************************************************************/ 
#region Using
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

public class SceneManager : MonoBehaviour 
{
    #region Variables
    //Main game states
    public enum MainGame
    {
        HOME, 
        GYMBATTLE,
        TEAM,
        PC,
        SHOP,
        POKEDEX,
        TRAINERCARD
    } //end MainGame

	//Scene variables
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
    GameObject buttonMenu;          //Menu of buttons in main game
    GameObject gymBattle;           //Screen of region leader battles
    GameObject playerTeam;          //Screen of the player's team
    GameObject trainerCard;         //Screen of the trainer card
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
		//If something is already happening, end it or return
		if(processing)
		{
			//If player cancels animation
			if((Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)) && playing)
			{
				playing = false;
				checkpoint = 3;
			} //end if
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
            if(Input.GetKeyDown(KeyCode.Return)|| Input.GetMouseButtonDown(0))
			{
				playing = true;
				checkpoint = 0;
				StartCoroutine(FadeOutAnimation(4));
			} //end if
			else
			{
				processing = false;
			} //end else
		} //end else if
		//Move to menu scene when finished fading out
		else if(checkpoint == 4)
		{
			checkpoint = 0;
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
			pName.text = GameManager.instance.GetPlayerName();
			badges.text = "Badges: " + GameManager.instance.GetBadges().ToString();
			totalTime.text = "Playtime: " + GameManager.instance.GetHours().ToString() + ":" + 
				GameManager.instance.GetMinutes().ToString("00");

			//Run fade animation
			StartCoroutine(FadeInAnimation(2));
		} //end else if
		//Run menu
		else if(checkpoint == 2)
		{
			//Begin processing
			processing = true;

			//If down arrow is pressed
			if(Input.GetKeyDown(KeyCode.DownArrow))
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

				//Menu finished this check
				processing = false;
			} //end if

			//If up arrow is pressed
			else if(Input.GetKeyDown(KeyCode.UpArrow))
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

				//Menu finished this check
				processing = false;
			} //end else if

            //If the mouse is lower than the selection box, and it moved, reposition to next choice
            else if(Input.GetAxis("Mouse Y") < 0 && Input.mousePosition.y < 
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
                
                //Menu finished this check
                processing = false;
            } //end else if
            
            //If the mouse is higher than the selection box, and it moved, reposition to next choice
            else if(Input.GetAxis("Mouse Y") > 0 && Input.mousePosition.y >
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
                
                //Menu finished this check
                processing = false;
            } //end else if
			//If an option was selected, process it
			else if(Input.GetKeyDown(KeyCode.Return))
			{
				StartCoroutine(FadeOutAnimation(5));
			} //end else if
			else
			{
				//Menu finished this check
				processing = false;
			} //end else
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

			//If down arrow is pressed
			if(Input.GetKeyDown(KeyCode.DownArrow))
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

				//Menu finished this check
				processing = false;
			} //end if
			
			//If up arrow is pressed
			else if(Input.GetKeyDown(KeyCode.UpArrow))
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

				//Menu finished this check
				processing = false;
			} //end else if
			
            //If the mouse is lower than the selection box, and it moved, reposition to next choice
            else if(Input.GetAxis("Mouse Y") < 0 && Input.mousePosition.y <
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

                //Menu finished this check
                processing = false;
            } //end else if

            //If the mouse is higher than the selection box, and it moved, reposition to next choice
            else if(Input.GetAxis("Mouse Y") > 0 && Input.mousePosition.y >
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

			//If an option was selected, process it
			else if(Input.GetKeyDown(KeyCode.Return))
			{
				selection.SetActive(false);
				StartCoroutine(FadeOutAnimation(5));
			} //end else if
			//Nothing was pressed
			else
			{
				//Menu finished this check
				processing = false;
			} //end else
		} //end else if
		//Move to relevant scene
		else if(checkpoint == 5)
		{
			// First choice selected, this is usually continue
			if(choiceNumber == 0)
			{
				GameManager.instance.Continue();
			} //end if
			// Second choice selected, this is usually new game
			else if(choiceNumber == 1)
			{
				GameManager.instance.NewGame();
			} //end else if
			// Third choice selected, this is usually options
			else if(choiceNumber == 2)
			{
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
			//Don't continue until player requests next text
			if(Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
			{
				//Attempt to display text
				if(GameManager.instance.DisplayText("This circus has attracted major gym leaders from " +
					"around the world! In fact, that's why you're here, isn't it?"))
				{
					checkpoint = 4;
				} //end if
			} //end if
			processing = false;
		} //end else if
		else if(checkpoint == 4)
		{
			processing = true;
			//Don't continue until player requests next text
			if(Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
			{
				//Attempt to display text
				if(GameManager.instance.DisplayText("Alright, let's get you set up. First, what is " +
					"your name?"))
				{
					checkpoint = 5;
				} //end if
			} //end if
			processing = false;
		} //end else if
		else if(checkpoint == 5)
		{
			processing = true;
			//Don't continue until player requests next text
			if(Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
			{
				//Display name input
				input.SetActive(true);
				input.transform.GetChild(2).GetComponent<Text>().text = "Please enter your name.";
				input.transform.GetChild(0).GetComponent<Text>().text = "Player name:";
				inputText.text = "";
				inputText.ActivateInputField();
				checkpoint = 6;
			} //end if
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
			//Don't continue until player requests next text
			if((Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)) && inputText.text.Length != 0)
			{
				//Convert input name to player's name
				playerName = inputText.text;
				input.SetActive(false);
				GameManager.instance.DisplayText("So your name is " + playerName + "?");
				checkpoint = 7;
			} //end if
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
				Vector3[] test = new Vector3[4];
				confirm.transform.GetChild(0).GetComponent<RectTransform>().GetWorldCorners(test);
				float width = test[2].x - test[0].x;
				float height = test[2].y - test[0].y;

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
			//If down arrow is pressed
			if(Input.GetKeyDown(KeyCode.DownArrow))
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
			
			//If up arrow is pressed
			if(Input.GetKeyDown(KeyCode.UpArrow))
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
			
            //If the mouse is lower than the selection box, and it moved, reposition to next choice
            else if(Input.GetAxis("Mouse Y") < 0 && Input.mousePosition.y < selection.transform.position.y-1)
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
                
                //Menu finished this check
                processing = false;
            } //end else if
            
            //If the mouse is higher than the selection box, and it moved, reposition to next choice
            else if(Input.GetAxis("Mouse Y") > 0 && Input.mousePosition.y > selection.transform.position.y+1)
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
                
                //Menu finished this check
                processing = false;
            } //end else if

			//If an option was selected, process it
			if(Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
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
			} //end if
			processing = false;
		} //end else if
		else if(checkpoint == 9)
		{
			processing = true;
			//Set name
			GameManager.instance.SetName(playerName);

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
			//Don't continue until player requests next text
			if((Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)) && !GameManager.instance.IsDisplaying())
			{
				text.SetActive(false);
				checkpoint = 0;
				GameManager.instance.Persist();
				Application.LoadLevel("Intro");
			} //end if
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
            buttonMenu = GameObject.Find("ButtonMenu");
            gymBattle = GameObject.Find("GymBattles");
            playerTeam = GameObject.Find("MyTeam");
            trainerCard = GameObject.Find("PlayerCard");

            //Disable screens
            gymBattle.SetActive(false);
            playerTeam.SetActive(false);
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
            } //end if
            else if(gameState == MainGame.GYMBATTLE)
            {
                buttonMenu.SetActive(false);
                gymBattle.SetActive(true);
            } //end else if
            else if(gameState == MainGame.TEAM)
            {
                buttonMenu.SetActive(false);
                playerTeam.SetActive(true);
            } //end else if
            else if(gameState == MainGame.TRAINERCARD)
            {
                if(Input.GetKeyDown(KeyCode.X) || Input.GetMouseButtonDown(1))
                {
                    gameState = MainGame.HOME;
                } //end if
                buttonMenu.SetActive(false);
                trainerCard.SetActive(true);
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
    public void SetGameState(MainGame newGameState)
    {
        gameState = newGameState; 
    } //end SetCheckpoint(MainGame newGameState)
	#endregion
    #endregion
} //end SceneManager class
