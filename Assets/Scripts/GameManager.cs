/***************************************************************************************** 
 * File:    GameManager.cs
 * Summary: Singleton structure to handle all global or inter-class requests
 *****************************************************************************************/ 
#region Using
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
#endregion

public class GameManager : MonoBehaviour
{
    #region Variables
	//GLOBAL SETTING VARIABLES
	public float VersionNumber = 0.1f;      //Version number for save file management
	public int NumberOfMarkings = 4;        //How many markings are available to be used
	public int NumberOfRibbons = 80;        //How many ribbons are available

	//Singleton handle
	public static GameManager instance = null;

	//SceneTools variables
	public GameObject pTool;				//Prefab of SceneTools
	public static GameObject tools = null;	//Canvas of SceneTools

	//Scene variables
	SceneManager scenes;					//Manages game scenes
	SystemManager sysm;						//Manages system features
    bool running = false;                   //Allows methods to run once
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

		//Get SceneManager component
		scenes = GetComponent<SceneManager> ();

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
				scenes.Reset();
				Application.LoadLevel("Intro");
				return;
			} //end if

			//Intro scene
			if(Application.loadedLevelName == "Intro")
			{
                //Allows running, testing, and compiling while bypassing standard operation
                //for one-off events.
                if(!running)
                {
                    //Prevent from running this section again
                    running = true;

#if UNITY_EDITOR
                    //Debug mode (development in the editor) commands go here
                    sysm.GetPersist();
                    //sysm.PlayerTrainer.EmptyTeam();
                    //sysm.PlayerTrainer.RandomTeam();
                    sysm.Persist();

#else
                    //Stand-alone mode (user version) diagnostic commands go here

#endif
                } //end if !running


                //Run the intro after completion of one off methods
				scenes.Intro();
			} //end if Intro

			//Start Menu scene
			else if(Application.loadedLevelName == "StartMenu")
			{
				scenes.Menu();
			} //end else if	

			//New Game scene
			else if(Application.loadedLevelName == "NewGame")
			{
				scenes.NewGame();
			} //end else if

            //Main Game scene
            else if(Application.loadedLevelName == "MainGame")
            {
                scenes.ContinueGame();
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
     * Name: Continue
     * Loads main game and player data
     ***************************************/
	public void Continue()
	{
        scenes.Reset ();
		Application.LoadLevel ("MainGame");
	} //end Continue

    /***************************************
     * Name: NewGame
     * Loads new game and creates new save file
     ***************************************/
	public void NewGame()
	{
		scenes.Reset ();
		Application.LoadLevel ("NewGame");
	} //end NewGame

    /***************************************
     * Name: Options
     * Displays list of options for player to 
     * control
     ***************************************/
	public void Options()
	{
		scenes.Reset ();
		Application.LoadLevel ("Intro");
	} //end Options

    /***************************************
     * Name: ProcessSelection
     * Sets the appropriate checkpoint
     * depending on scene
     ***************************************/
    public void ProcessSelection()
    {
        if (Application.loadedLevelName == "StartMenu")
        {
            scenes.SetCheckpoint (5);
        }  //end if
        else if (Application.loadedLevelName == "MainGame")
        {
            scenes.ReadRibbon();
        } //end else if
    } //end ProcessSelection

    /***************************************
     * Name: ReturnHome
     * Brings up the main game button menu
     ***************************************/ 
    public void ReturnHome()
    {
        scenes.SetGameState (SceneManager.MainGame.HOME);
    } //end ReturnHome

    /***************************************
     * Name: GymBattle
     * Brings up the gym battle screen
     ***************************************/ 
    public void GymBattle()
    {
        scenes.SetGameState (SceneManager.MainGame.GYMBATTLE);
    } //end GymBattle

    /***************************************
     * Name: TeamMenu
     * Brings up the party screen
     ***************************************/ 
    public void TeamMenu()
    {
        scenes.SetGameState (SceneManager.MainGame.TEAM);
    } //end TeamMenu

    /***************************************
     * Name: PlayerPC
     * Brings up the pokemon storage screen
     ***************************************/ 
    public void PlayerPC()
    {
        scenes.SetGameState (SceneManager.MainGame.PC);
    } //end PlayerPC
    
    /***************************************
     * Name: Shop
     * Switches to Shop scene
     ***************************************/ 
    public void Shop()
    {
        scenes.SetGameState (SceneManager.MainGame.SHOP);
    } //end Shop
    
    /***************************************
     * Name: Pokedex
     * Switches to pokedex scene
     ***************************************/ 
    public void Pokedex()
    {
        scenes.SetGameState (SceneManager.MainGame.POKEDEX);
    } //end Pokedex
    
    /***************************************
     * Name: TrainerCard
     * Brings up the trainer card screen
     ***************************************/ 
    public void TrainerCard()
    {
        scenes.SetGameState (SceneManager.MainGame.TRAINERCARD);
    } //end TrainerCard

    /***************************************
     * Name: SummaryChange
     * Brings up the requested summary screen
     ***************************************/ 
    public void SummaryChange(int summaryPage)
    {
        scenes.SetSummaryPage (summaryPage);
    } //end SummaryChange(int summaryPage)
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
	public bool DisplayText(string text)
	{
		return sysm.PlayText (text);
	} //end DisplayText(string text)

    /***************************************
     * Name: IsDisplaying
     * Returns if text has been fully displayed
     ***************************************/
	public bool IsDisplaying()
	{
		return sysm.GetDisplay ();
	} //end IsDisplaying

    /***************************************
     * Name: GetTrainer
     * Retrieves the player's trainer profile
     ***************************************/
    public Trainer GetTrainer()
    {
        return sysm.PlayerTrainer;
    } //end GetTrainer

    /***************************************
     * Name: Persist
     * Stores the player's progress
     ***************************************/
	public void Persist()
	{
		sysm.Persist ();
	} //end Persist

    /***************************************
     * Name: GetPersist
     * Retrieves the player's progress
     ***************************************/
	public bool GetPersist()
	{
		return sysm.GetPersist ();
	} //end GetPersist
	#endregion
    #endregion
} //end GameManager class