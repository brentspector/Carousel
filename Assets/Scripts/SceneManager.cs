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
    Image pokeFront;                //Front of the pokemon
    Image pokeBack;                 //Back of the pokemon
    Text  pokeName;                 //Name of the pokemon
    Text  pokeType;                 //Types of pokemon
    Text  pokeBaseStats;            //Base stats of pokemon
    Text  pokeAbilities;            //Abilities of pokemon
    Text  pokeHeightWeight;         //Height and weight of the pokemon
    Text  pokeForms;                //Forms of the pokemon
    Text  pokeEvolutions;           //Evolutions of the pokemon
    Text  pokePokedex;              //Pokedex entry of the pokemon
    Text  pokeMoves;                //Full list of moves of the pokemon
    int chosenPoke;                 //Pokemon currently viewed
    Toggle shinyFlag;               //Flag if shiny is checked
    Toggle femaleFlag;              //Flag if female is checked
    int formNum;                    //What form version is currently displayed
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
			if(Input.GetKeyDown(KeyCode.Return) && playing)
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
			if(Input.GetKeyDown(KeyCode.Return))
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
			if(Input.GetKeyDown(KeyCode.Return))
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
			if(Input.GetKeyDown(KeyCode.Return))
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
			if(Input.GetKeyDown(KeyCode.Return))
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
			if(Input.GetKeyDown(KeyCode.Return) && inputText.text.Length != 0)
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
			
			//If an option was selected, process it
			if(Input.GetKeyDown(KeyCode.Return))
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
			if(Input.GetKeyDown(KeyCode.Return) && !GameManager.instance.IsDisplaying())
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

            //Get references for variables
            pokeFront = GameObject.Find ("PokemonFront").GetComponent<Image> ();
            pokeBack = GameObject.Find ("PokemonBack").GetComponent<Image> ();
            pokeName = GameObject.Find ("Name").GetComponent<Text> ();
            pokeType = GameObject.Find ("Type").GetComponent<Text> ();
            pokeBaseStats = GameObject.Find ("BaseStats").GetComponent<Text> ();
            pokeAbilities = GameObject.Find ("Abilities").GetComponent<Text> ();
            pokeHeightWeight = GameObject.Find ("HeightWeight").GetComponent<Text> ();
            pokeForms = GameObject.Find ("Forms").GetComponent<Text> ();
            pokeEvolutions = GameObject.Find ("Evolutions").GetComponent<Text> ();
            pokePokedex = GameObject.Find ("Pokedex").GetComponent<Text> ();
            pokeMoves = GameObject.Find ("Moves").GetComponent<Text> ();
            shinyFlag = GameObject.Find("Shiny").GetComponent<Toggle>();
            femaleFlag = GameObject.Find("Female").GetComponent<Toggle>();

            //Fade in
            StartCoroutine (FadeInAnimation (1));
        } //end if
        else if (checkpoint == 1)
        {
            //Begin processing
            processing = true;
            
            //Disable fade screen
            fade.gameObject.SetActive (false);

            //Initialize first data
            pokeFront.sprite = Resources.Load<Sprite> ("Sprites/Pokemon/001");
            pokeBack.sprite = Resources.Load<Sprite> ("Sprites/Pokemon/001b");
            pokeName.text = DataContents.ExecuteSQL<string>("SELECT name FROM Pokemon WHERE rowid=1");
            pokeType.text = DataContents.ExecuteSQL<string>("SELECT type1 FROM Pokemon WHERE rowid=1") + ", " 
                + DataContents.ExecuteSQL<string>("SELECT type2 FROM Pokemon WHERE rowid=1");
            pokeHeightWeight.text = DataContents.ExecuteSQL<string>("SELECT height FROM Pokemon WHERE rowid=1") 
                + " height, " + DataContents.ExecuteSQL<string>("SELECT weight FROM Pokemon WHERE rowid=1") + " weight.";
            pokePokedex.text = DataContents.ExecuteSQL<string>("SELECT pokedex FROM Pokemon WHERE rowid=1");
            pokeBaseStats.text = DataContents.ExecuteSQL<string>("SELECT health FROM Pokemon WHERE rowid=1");
            pokeBaseStats.text += " " + DataContents.ExecuteSQL<string>("SELECT attack FROM Pokemon WHERE rowid=1");
            pokeBaseStats.text += " " + DataContents.ExecuteSQL<string>("SELECT defence FROM Pokemon WHERE rowid=0");
            pokeBaseStats.text += " " + DataContents.ExecuteSQL<string>("SELECT speed FROM Pokemon WHERE rowid=0");
            pokeBaseStats.text += " " + DataContents.ExecuteSQL<string>("SELECT specialAttack FROM Pokemon WHERE rowid=0");
            pokeBaseStats.text += " " + DataContents.ExecuteSQL<string>("SELECT specialDefence FROM Pokemon WHERE rowid=0");
            pokeAbilities.text = DataContents.ExecuteSQL<string>("SELECT ability1 FROM Pokemon WHERE rowid=0")
            + ", " + DataContents.ExecuteSQL<string>("SELECT ability2 FROM Pokemon WHERE rowid=0") + ", " +
                    DataContents.ExecuteSQL<string>("SELECT hiddenAbility FROM Pokemon WHERE rowid=0");      
            pokeEvolutions.text = "Evos: " + DataContents.ExecuteSQL<string>("SELECT evolutions FROM Pokemon WHERE rowid=0");
            pokeForms.text = "Forms: " + DataContents.ExecuteSQL<string>("SELECT forms FROM Pokemon WHERE rowid=0");       
            pokeMoves.text = "Moves: " + DataContents.ExecuteSQL<string>("SELECT moves FROM Pokemon WHERE rowid=0");

            //Set chosenPoke to 1
            chosenPoke = 1;

            //Move to next section 
            checkpoint = 2;
  
            //End processing
            processing = false;
        } //end else if
        else if (checkpoint == 2)
        {
            //Begin processing
            processing = true;

            //If right arrow is pressed, increment
            if(Input.GetKey(KeyCode.RightArrow))
            {
                formNum = 0;
                if(chosenPoke + 1 > 721)
                {
                    chosenPoke = 1;
                }
                else
                {
                    chosenPoke++;
                }
            } //end if
            //If left arrow is pressed, decrement
            else if(Input.GetKey(KeyCode.LeftArrow))
            {
                formNum = 0;
                if(chosenPoke - 1 < 1)
                {
                    chosenPoke = 721;
                }
                else
                {
                    chosenPoke--;
                }
            } //end else if
            //If up arrow is pressed, increase form
            else if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                if((formNum + 1) > DataContents.speciesData[chosenPoke].forms.Length)
                {
                    formNum = 0;
                } //end if
                else
                {
                    if(DataContents.speciesData[chosenPoke].forms[formNum] == null)
                    {
                        formNum = 0;
                    } //end if
                    else
                    {
                        formNum++;
                    } //end else
                } //end else
            } //end else if
            //If down arrow is pressed, decrease form
            else if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                if((formNum - 1) < 0)
                {
                    if(DataContents.speciesData[chosenPoke].forms[0] == null)
                    {
                        formNum = 0;
                    } //end if
                    else
                    {
                        formNum = DataContents.speciesData[chosenPoke].forms.Length;
                    } //end else
                } //end if
                else
                {
                    formNum--;
                } //end else
            } //end else if

            //Load appropriate data
            string chosenString = (chosenPoke + 1).ToString("000");
            chosenString += femaleFlag.isOn ? "f" : "";
            chosenString += shinyFlag.isOn ? "s" : "";
            chosenString += formNum > 0 ? "_" + formNum.ToString() : "";
            pokeFront.sprite = Resources.Load<Sprite>("Sprites/Pokemon/" + chosenString);
            if(pokeFront.sprite == null)
            {
                chosenString = chosenString.Replace("f", "");
                pokeFront.sprite = Resources.Load<Sprite>("Sprites/Pokemon/" + chosenString);
                if(pokeFront.sprite == null)
                {
                    pokeFront.sprite = Resources.Load<Sprite>("Sprites/Pokemon/0");
                } //end if
            } //end if
            pokeBack.sprite = Resources.Load<Sprite>("Sprites/Pokemon/" + chosenString + "b");
            if(pokeBack.sprite == null)
            {
                chosenString = chosenString.Replace("f", "");
                pokeBack.sprite = Resources.Load<Sprite>("Sprites/Pokemon/" + chosenString + "b");
                if(pokeBack.sprite == null)
                {
                    pokeBack.sprite = Resources.Load<Sprite>("Sprites/Pokemon/0b");
                } //end if
            } //end if
            pokeName.text = DataContents.speciesData [chosenPoke].name;
            pokeType.text = DataContents.speciesData [chosenPoke].type1 + ", " + DataContents.speciesData [chosenPoke].type2;
            pokeHeightWeight.text = DataContents.speciesData [chosenPoke].height + " height, " + DataContents.speciesData [chosenPoke].weight + " weight.";
            pokePokedex.text = DataContents.speciesData [chosenPoke].pokedex;
            pokeBaseStats.text = DataContents.speciesData [chosenPoke].baseStats [0].ToString ();
            for (int bStats = 1; bStats < 6; bStats++)
            {
                pokeBaseStats.text += ", " + DataContents.speciesData [chosenPoke].baseStats [bStats].ToString ();
            } //end for
            pokeAbilities.text = DataContents.speciesData [chosenPoke].abilities [0];
            for (int pAbil = 1; pAbil < DataContents.speciesData[chosenPoke].abilities.Length; pAbil++)
            {
                pokeAbilities.text += ", " + DataContents.speciesData [chosenPoke].abilities [pAbil];
            } //end for
            if(DataContents.speciesData[chosenPoke].evolutions.Count > 0)
            {
                pokeEvolutions.text = "Evos: " + DataContents.speciesData[chosenPoke].evolutions[0].species + " "
                    + DataContents.speciesData[chosenPoke].evolutions[0].method + " " + 
                        DataContents.speciesData[chosenPoke].evolutions[0].trigger;
                for(int pEvo = 1; pEvo < DataContents.speciesData[chosenPoke].evolutions.Count; pEvo++)
                {
                    pokeEvolutions.text += ", " + DataContents.speciesData[chosenPoke].evolutions[pEvo].species + " "
                        + DataContents.speciesData[chosenPoke].evolutions[pEvo].method + " " + 
                            DataContents.speciesData[chosenPoke].evolutions[pEvo].trigger;
                } //end for
            } //end if
            else
            {
                pokeEvolutions.text = "None";
            } //end else
            pokeForms.text = "Forms: " + DataContents.speciesData [chosenPoke].forms [0];
            for (int pForm = 1; pForm < DataContents.speciesData[chosenPoke].forms.Length; pForm++)
            {
                pokeForms.text += ", " + DataContents.speciesData [chosenPoke].forms [pForm];
            } //end for
            
            pokeMoves.text = "Moves:";
            //Get key levels nearest to current level
            foreach (KeyValuePair<int, List<string>> entry in DataContents.speciesData[chosenPoke].moves)
            {
                for (int pMove = 0; pMove < DataContents.speciesData[chosenPoke].moves[entry.Key].Count; pMove++)
                {
                    pokeMoves.text += ", " + entry.Key + " " + DataContents.speciesData [chosenPoke].moves [entry.Key] [pMove];
                } //end for
            } //end foreach

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
     * Name: JumpTo
     * Changes active pokemon to the one entered in the input field
     ***************************************/
    public void JumpTo()
    {
        chosenPoke = int.Parse(GameObject.Find ("LocToJump").GetComponent<InputField> ().text);
        chosenPoke = Math.Abs (chosenPoke);
        if (chosenPoke > 721)
        {
            chosenPoke = 720;
        } //end if
    } //end JumpTo
	#endregion
    #endregion
} //end SceneManager class
