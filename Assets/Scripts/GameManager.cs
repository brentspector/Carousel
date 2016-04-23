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
	public int NumberOfMarkings = 6;        //How many markings are available to be used
	public int NumberOfRibbons = 80;        //How many ribbons are available
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
                    //sysm.GetPersist();
                    //sysm.Persist();

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

            //PC scene
            else if(Application.loadedLevelName == "PC")
            {
                scenes.PC();
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
        scenes.PartyState(false);
    } //end PartyState(bool state)
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
	public void Persist()
	{
        //If not already saving
        if (!tools.transform.GetChild (1).gameObject.activeSelf)
        {
            sysm.Persist ();
            sysm.PlayText ("Saved successfully!");
        } //end if
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
        //If the team isn't full, add a pokemon
        if (sysm.PlayerTrainer.Team.Count < 6)
        {
            sysm.PlayerTrainer.AddPokemon(new Pokemon(oType: 5, oWhere: 3));
        } //end if
        //Otherwise replace first slot
        else
        {
            sysm.PlayerTrainer.Team[0] = new Pokemon(oType: 5, oWhere: 3);
        } //end else
    } //end RandomPokemon

    /***************************************
     * Name: AddRandomPokemonToPC
     * Adds a single random pokemon to the pc
     ***************************************/ 
    public void AddRandomPokemonToPC(int box = -1, int slot = 0)
    {
        //Assign box
        if (box == -1)
        {
            box = sysm.PlayerTrainer.GetPCBox ();
        } //end if

        sysm.PlayerTrainer.AddToPC (box, slot, new Pokemon (oType: 5, oWhere: 3));
    } //end AddRandomPokemonToPC
    #endregion
    #endregion
} //end GameManager class