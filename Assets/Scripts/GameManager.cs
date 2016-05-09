﻿/***************************************************************************************** 
 * File:    GameManager.cs
 * Summary: Singleton structure to handle all global or inter-class requests
 *****************************************************************************************/ 
#region Using
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Random = UnityEngine.Random;
#endregion

public class GameManager : MonoBehaviour
{
    #region Variables
	//GLOBAL SETTING VARIABLES
    [System.NonSerialized]
	public float VersionNumber = 0.3f;      //Version number for save file management
    [System.NonSerialized]
    public int NumberOfWallpaper = 25;      //How many wallpapers are available

	//Singleton handle
	public static GameManager instance = null;

	//SceneTools variables
	public GameObject pTool;				//Prefab of SceneTools
	public static GameObject tools = null;	//Canvas of SceneTools

	//Scene variables
    SystemManager sysm;                     //Manages system features
	SceneManager scenes;					//Manages game scenes
    bool running = false;                   //Allows methods to run once
    bool textDisplayed = false;             //If text is displayed
    bool continueImmediate;                 //Continue as soon as able, don't wait for enter
    #endregion

    #region Methods
    /***************************************
     * Name: Awake
     * Initializes singletons and garbage collection settings
     ***************************************/
	void Awake ()
	{
		//If an instance doesn't exist, set it to this
		if (instance == null) 
		{
			instance = this;
		} //end if
        //If an instance exists that isn't this, destroy this
        else if (instance != this) 
		{
			Destroy (gameObject);
		} //end else if	

		//Keep GameManager from destruction OnLoad
		DontDestroyOnLoad (instance);

		//If a SceneTools canvas doesn't exist, make one
		if(tools == null)
		{
			tools = Instantiate(pTool);
		} //end if

		//Keep SceneTools from destruction OnLoad
		DontDestroyOnLoad (tools);

        //Get SystemManager component
        sysm = GetComponent<SystemManager> ();
        
        //Create error log
        sysm.InitErrorLog ();
        
        //Initialize DataContents class 
        if(!DataContents.InitDataContents())
        {
            sysm.LogErrorMessage("Could not load data contents");
            Application.Quit();
        } //end if

		//Get SceneManager component
		scenes = GetComponent<SceneManager> ();
	} //end Awake
	
    /***************************************
     * Name: Update
     * Controls flow of program and game
     ***************************************/
	void Update ()
	{
		//Try running game as normal
		try
		{
			//Reset when F12 is pressed
			if(Input.GetKeyDown(KeyCode.F12))
			{
                textDisplayed = false;
                scenes.Reset();
				return;
			} //end if

            //Don't continue updating game until text box is gone
            if(textDisplayed)
            {
                textDisplayed = sysm.ManageTextbox(continueImmediate);
            } //end if textDisplayed

			//Intro scene
            else if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Intro")
			{
                //Allows running, testing, and compiling while bypassing standard operation
                //for one-off events.
                if(!running)
                {
                    //Prevent from running this section again
                    running = true;

#if UNITY_EDITOR
                    //Debug mode (development in the editor) commands go here
                    //sysm.GetPersist();
                    //sysm.Persist();
#else
                    //Stand-alone mode (user version) diagnostic commands go here

#endif
                } //end if !running


                //Run the intro after completion of one off methods
				scenes.Intro();
			} //end else if Intro

			//Start Menu scene
            else if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "StartMenu")
			{
				scenes.Menu();
			} //end else if	

			//New Game scene
            else if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "NewGame")
			{
				scenes.NewGame();
			} //end else if

            //Main Game scene
            else if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainGame")
            {
                scenes.ContinueGame();
            } //end else if

            //PC scene
            else if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "PC")
            {
                scenes.PC();
            } //end else if

            //Pokedex scene
            else if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Pokedex")
            {
                scenes.Pokedex();
            } //end else if
		} //end try

		//Log error otherwise
		catch(System.Exception ex)
		{
            try
            {
			    sysm.LogErrorMessage(ex.ToString());
            } //end try
            catch(System.Exception exc)
            {
                Debug.LogError("\nThe following error was encountered: " +
                               ex.ToString() + "\nadditionally the following " +
                                "occurred:\n" + exc.ToString());
            } //end catch
		} //end catch(System.Exception ex)
	} //end Update

	//Scene functions
	#region Scenes
    /***************************************
     * Name: ProcessSelection
     * Sets the appropriate checkpoint
     * depending on scene
     ***************************************/
    public void ProcessSelection()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "StartMenu")
        {
            scenes.SetCheckpoint (5);
        }  //end if
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainGame")
        {
            scenes.ReadRibbon();
        } //end else if
    } //end ProcessSelection

    /***************************************
     * Name: SetGameState
     * Sets the main game to the state given
     ***************************************/ 
    public void SetGameState(SceneManager.MainGame newState)
    {
        //Disable text screen if active
        if (tools.transform.GetChild (1).gameObject.activeSelf)
        {
            tools.transform.GetChild (1).gameObject.SetActive(false);
        } //end if

        StartCoroutine(scenes.SetGameState (newState));
    } //end SetGameState(SceneManager.MainGame newState)
   
    /***************************************
     * Name: SummaryChange
     * Brings up the requested summary screen
     ***************************************/ 
    public void SummaryChange(int summaryPage)
    {
        StartCoroutine(scenes.SetSummaryPage (summaryPage));
    } //end SummaryChange(int summaryPage)

    /***************************************
     * Name: LoadScene
     * Loads the requested scene, and sets
     * the appropriate overall state
     ***************************************/ 
    public void LoadScene(string levelName, SceneManager.OverallGame state, bool fadeOut = false)
    {
        StartCoroutine(scenes.LoadScene (levelName, state, fadeOut));
    } //end LoadScene(string levelName, SceneManager.OverallGame state, bool fadeOut)

    /***************************************
     * Name: PartyState
     * Opens/Closes the Party in PC box
     ***************************************/ 
    public void PartyState(bool state)
    {
        StartCoroutine(scenes.PartyState(state));
    } //end PartyState(bool state)

    /***************************************
     * Name: ToggleShown
     * Toggles Weakness/Resistance in Pokedex
     ***************************************/ 
    public void ToggleShown()
    {
        scenes.ToggleShown ();
    } //end ToggleShown
    #endregion

	//System Manager functions
	#region SystemManager
    /***************************************
     * Name: LogErrorMessage
     * Sends string to be saved in error log,
     * or output log if error log failed
     ***************************************/
    public void LogErrorMessage(string message)
    {
        sysm.LogErrorMessage (message);
    } //end LogErrorMessage(string message)

    /***************************************
     * Name: InitText
     * Loads text box components in SceneManager
     ***************************************/
	public void InitText(Transform textArea, Transform endArrow)
	{
		sysm.GetText (textArea.gameObject, endArrow.gameObject);
	} //end InitText(GameObject textArea, GameObject endArrow)

    /***************************************
     * Name: DisplayText
     * Shows text in SceneManager text box
     ***************************************/
	public void DisplayText(string text, bool closeAfter, bool immediate = false)
	{
		sysm.DisplayText (text, closeAfter);
        textDisplayed = true;
        continueImmediate = immediate;
	} //end DisplayText(string text, bool closeAfter, bool immediate)

    /***************************************
     * Name: GetTrainer
     * Retrieves the player's trainer profile
     ***************************************/
    public Trainer GetTrainer()
    {
        return sysm.PlayerTrainer;
    } //end GetTrainer

    /***************************************
     * Name: RandomInt
     * Returns a random integer
     ***************************************/
    public int RandomInt(int min, int max)
    {
        return sysm.RandomInt (min, max);
    } //end RandomInt(int min, int max)

    /***************************************
     * Name: Persist
     * Stores the player's progress
     ***************************************/
	public void Persist(bool message = true)
	{
        //If not already saving
        if (!tools.transform.GetChild (1).gameObject.activeSelf)
        {
            sysm.Persist ();
            if(message)
            {
                DisplayText ("Saved successfully!", true);
            } //end if
        } //end if
    } //end Persist(bool message = true)

    /***************************************
     * Name: GetPersist
     * Retrieves the player's progress
     ***************************************/
	public bool GetPersist()
	{
		return sysm.GetPersist ();
	} //end GetPersist

    /***************************************
     * Name: RestartFile
     * Clears data to create new game file
     ***************************************/
    public void RestartFile(bool savePrevious = false)
    {
        sysm.NewGameReset (savePrevious);
    } //end RestartFile(bool savePrevious = false)
	#endregion

    #region Debug
    /***************************************
     * Name: RandomTeam
     * Gives the player a random team
     ***************************************/ 
    public void RandomTeam()
    {
        sysm.PlayerTrainer.EmptyTeam();
        sysm.PlayerTrainer.RandomTeam();
        sysm.PlayerTrainer.Team [0].ChangeRibbons (GameManager.instance.RandomInt (0, 20));
        sysm.PlayerTrainer.Team [0].ChangeRibbons (GameManager.instance.RandomInt (0, 20));
        sysm.PlayerTrainer.Team [1].ChangeRibbons (GameManager.instance.RandomInt (0, 20));
    } //end RandomTeam
    
    /***************************************
     * Name: RandomPokemon
     * Adds a single random pokemon to the team
     ***************************************/ 
    public void RandomPokemon()
    {
        //Get region where all options are stored
        GameObject temp = GameObject.Find ("RightRegion");

        //Generate random pokemon
        Pokemon randomPoke = new Pokemon();

        //Update relevant portions
        temp.transform.FindChild ("Gender").GetComponent<Dropdown>().value = randomPoke.Gender;
        temp.transform.FindChild ("Gender").GetComponent<Dropdown> ().RefreshShownValue ();
        temp.transform.FindChild ("PokemonName").GetComponent<Dropdown> ().value = randomPoke.NatSpecies - 1;
        temp.transform.FindChild ("PokemonName").GetComponent<Dropdown> ().RefreshShownValue ();
        temp.transform.FindChild ("Level").GetComponent<InputField>().text = randomPoke.CurrentLevel.ToString();
        temp.transform.FindChild ("TrainerID").GetComponent<InputField>().text = randomPoke.TrainerID.ToString();
        temp.transform.FindChild ("Nature").GetComponent<Dropdown> ().value = randomPoke.Nature;
        temp.transform.FindChild ("Nature").GetComponent<Dropdown> ().RefreshShownValue ();
        temp.transform.FindChild ("Item").GetComponent<Dropdown> ().value = randomPoke.Item;
        temp.transform.FindChild ("Item").GetComponent<Dropdown> ().RefreshShownValue ();
        temp.transform.FindChild ("Ball").GetComponent<Dropdown> ().value = randomPoke.BallUsed;
        temp.transform.FindChild ("Ball").GetComponent<Dropdown> ().RefreshShownValue ();
        temp.transform.FindChild ("Ability").GetComponent<Dropdown> ().value = randomPoke.Ability;
        temp.transform.FindChild ("Ability").GetComponent<Dropdown> ().RefreshShownValue ();
        //temp.transform.FindChild ("Ribbons").GetComponent<Dropdown> ().value = randomPoke.GetRibbon(0);
        //temp.transform.FindChild ("Ribbons").GetComponent<Dropdown> ().RefreshShownValue ();
        temp.transform.FindChild ("Happiness").GetComponent<InputField>().text = randomPoke.Happiness.ToString();
        temp.transform.FindChild ("Shiny").GetComponent<Toggle> ().isOn = randomPoke.IsShiny;
        temp.transform.FindChild ("Pokerus").GetComponent<Toggle> ().isOn = randomPoke.HasPokerus;
        temp.transform.FindChild ("IV").GetChild (0).GetComponent<InputField> ().text = randomPoke.GetIV (0).ToString();
        temp.transform.FindChild ("IV").GetChild (1).GetComponent<InputField> ().text = randomPoke.GetIV (1).ToString();
        temp.transform.FindChild ("IV").GetChild (2).GetComponent<InputField> ().text = randomPoke.GetIV (2).ToString();
        temp.transform.FindChild ("IV").GetChild (3).GetComponent<InputField> ().text = randomPoke.GetIV (3).ToString();
        temp.transform.FindChild ("IV").GetChild (4).GetComponent<InputField> ().text = randomPoke.GetIV (4).ToString();
        temp.transform.FindChild ("IV").GetChild (5).GetComponent<InputField> ().text = randomPoke.GetIV (5).ToString();
        temp.transform.FindChild ("EV").GetChild (0).GetComponent<InputField> ().text = randomPoke.GetEV (0).ToString();
        temp.transform.FindChild ("EV").GetChild (1).GetComponent<InputField> ().text = randomPoke.GetEV (1).ToString();
        temp.transform.FindChild ("EV").GetChild (2).GetComponent<InputField> ().text = randomPoke.GetEV (2).ToString();
        temp.transform.FindChild ("EV").GetChild (3).GetComponent<InputField> ().text = randomPoke.GetEV (3).ToString();
        temp.transform.FindChild ("EV").GetChild (4).GetComponent<InputField> ().text = randomPoke.GetEV (4).ToString();
        temp.transform.FindChild ("EV").GetChild (5).GetComponent<InputField> ().text = randomPoke.GetEV (5).ToString();
        temp.transform.FindChild ("Moves").GetChild (0).GetComponent<Dropdown> ().value = randomPoke.GetMove (0);
        temp.transform.FindChild ("Moves").GetChild (0).GetComponent<Dropdown> ().RefreshShownValue ();
        temp.transform.FindChild ("Moves").GetChild (1).GetComponent<Dropdown> ().value = randomPoke.GetMove (1);
        temp.transform.FindChild ("Moves").GetChild (1).GetComponent<Dropdown> ().RefreshShownValue ();
        temp.transform.FindChild ("Moves").GetChild (2).GetComponent<Dropdown> ().value = randomPoke.GetMove (2);
        temp.transform.FindChild ("Moves").GetChild (2).GetComponent<Dropdown> ().RefreshShownValue ();
        temp.transform.FindChild ("Moves").GetChild (3).GetComponent<Dropdown> ().value = randomPoke.GetMove (3);
        temp.transform.FindChild ("Moves").GetChild (3).GetComponent<Dropdown> ().RefreshShownValue ();
    } //end RandomPokemon

    /***************************************
     * Name: UpdateSprite
     * Changes sprite to reflect name choice
     ***************************************/ 
    public void UpdateSprite()
    {
        //Get region where all options are stored
        GameObject temp = GameObject.Find ("RightRegion");

        //Develop string path to image
        string chosenString = (temp.transform.FindChild("PokemonName").GetComponent<Dropdown>().value + 1).
            ToString ("000");
        chosenString +=  temp.transform.FindChild("Gender").GetComponent<Dropdown>().value == 1 ? "f" : "";
        chosenString += temp.transform.FindChild("Shiny").GetComponent<Toggle>().isOn ? "s" : "";

        //Change sprite, and fix if sprite is null
        temp.transform.FindChild("Sprite").GetComponent<Image>().sprite = 
            Resources.Load<Sprite>("Sprites/Pokemon/" + chosenString);   
        if( temp.transform.FindChild("Sprite").GetComponent<Image>().sprite == null)
        {
            chosenString = chosenString.Replace("f", "");
            temp.transform.FindChild("Sprite").GetComponent<Image>().sprite = 
                Resources.Load<Sprite>("Sprites/Pokemon/" + chosenString);
            if( temp.transform.FindChild("Sprite").GetComponent<Image>().sprite == null)
            {
                temp.transform.FindChild("Sprite").GetComponent<Image>().sprite = 
                    Resources.Load<Sprite>("Sprites/Pokemon/0");
            } //end if
        } //end if
    } //end UpdateSprite()
    #endregion
    #endregion
} //end GameManager class