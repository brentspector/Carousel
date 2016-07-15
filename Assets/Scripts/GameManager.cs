/***************************************************************************************** 
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
	public float VersionNumber = 0.4f;      //Version number for save file management
    [System.NonSerialized]
    public int NumberOfWallpaper = 25;      //How many wallpapers are available

	//Singleton handle
	public static GameManager instance = null;

	//Delegates
	public delegate void CheckpointDelegate(int checkpoint);
	public CheckpointDelegate checkDel;
	public delegate void ConfirmDelegate(ConfirmChoice e);
	public ConfirmDelegate confirmDel;

	//SceneTools variables
	public GameObject pTool;				//Prefab of SceneTools
	public static GameObject tools = null;	//Canvas of SceneTools

	//Scene scripts
	AnimationManager anim;					//Manages animations for scenes
	IntroScene intro;						//Introduction scene script
	MenuScene menu;							//Menu scene script
	NewGameScene newgame;					//New game scene script
	MainGameScene mainGame;					//Main game scene script
	PCScene pc;								//PC scene script
	PokedexScene pokedex;					//Pokedex scene script
	InventoryScene inventory;				//Inventory scene script
	ShopScene shop;							//Shop scene script
	BattleScene battle;						//Battle scene script

	//Scene variables
    SystemManager sysm;                     //Manages system features
	bool loadingLevel = false;				//If a level is loading, disable update
    bool running = false;                   //Allows methods to run once
    bool textDisplayed = false;             //If text is displayed
	bool confirmDisplayed = false;			//If confirm is displayed
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

		//Reset scene tools canvas' camera
		tools.GetComponent<Canvas> ().worldCamera = Camera.main;

		//Set all tools to inactive
		tools.transform.GetChild(0).gameObject.SetActive(false);
		tools.transform.GetChild(1).gameObject.SetActive(false);
		tools.transform.GetChild(2).gameObject.SetActive(false);
		tools.transform.GetChild(3).gameObject.SetActive(false);
		tools.transform.GetChild(4).gameObject.SetActive(false);
		tools.transform.GetChild(5).gameObject.SetActive(false);

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

		//Initialize textbox
		sysm.GetText(tools.transform.FindChild("TextUnit").gameObject, 
			tools.transform.FindChild("TextUnit").GetChild(1).gameObject);

		//Initialize confirm
		sysm.GetConfirm();

		//Get scene scripts
		anim = GetComponent<AnimationManager>();
		intro = GetComponent<IntroScene>();
		menu = GetComponent<MenuScene>();
		newgame = GetComponent<NewGameScene>();
		mainGame = GetComponent<MainGameScene>();
		pc = GetComponent<PCScene>();
		pokedex = GetComponent<PokedexScene>();
		inventory = GetComponent<InventoryScene>();
		shop = GetComponent<ShopScene>();
		battle = GetComponent<BattleScene>();
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
				Reset();
				return;
			} //end if

            //Don't continue updating game until text box is gone
            if(textDisplayed)
            {				
                textDisplayed = sysm.ManageTextbox(continueImmediate);
            } //end if textDisplayed

			//Don't continue updating game until confirm box is gone
			else if(confirmDisplayed)
			{
				confirmDisplayed = sysm.ManageConfirm();
			} //end else if confirmDisplayed

			//Don't update game while a scene is loading or an animation is playing
			else if(loadingLevel || anim.IsProcessing())
			{
				return;
			} //end else if loadingLevel

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
				intro.RunIntro();
			} //end else if Intro

			//Start Menu scene
            else if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "StartMenu")
			{
				menu.RunMenu();
			} //end else if	

			//New Game scene
            else if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "NewGame")
			{
				newgame.RunNewGame();
			} //end else if

            //Main Game scene
            else if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainGame")
            {
				mainGame.RunMainGame();
            } //end else if

            //PC scene
            else if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "PC")
            {
				pc.RunPC();
            } //end else if

            //Pokedex scene
            else if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Pokedex")
            {
				pokedex.RunPokedex();                
            } //end else if

			//Inventory scene
			else if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Inventory")
			{
				inventory.RunInventory();
			} //end else if

			//Shop scene
			else if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Shop")
			{
				shop.RunShop();
			} //end else if

			//Battle scene
			else if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Battle")
			{
				battle.RunBattle();
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
     * Name: LoadScene
     * Loads the requested scene, and sets
     * the appropriate overall state
     ***************************************/ 
	public void LoadScene(string levelName, bool fadeOut = false)
	{
		//If fade out is requested
		if (fadeOut)
		{
			loadingLevel = true;
			tools.transform.FindChild("Selection").gameObject.SetActive(false);
			StartCoroutine(anim.FadeOutAnimation(levelName));
		} //end if
		else
		{
			//Move to next scene
			loadingLevel = true;
			ChangeCheckpoint(0);
			checkDel = null;
			confirmDel = null;
			UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);
			loadingLevel = false;
		} //end else
	} //end LoadScene(string levelName, bool fadeOut)

	/***************************************
     * Name: ChangeCheckpoint
     * Changes the checkpoint of a scene script
     ***************************************/ 
	public void ChangeCheckpoint(int checkpoint)
	{
		checkDel(checkpoint);
	} //end ChangeCheckpoint(int checkpoint)

    /***************************************
     * Name: Reset
     * Soft Reset game to Intro
     ***************************************/
	public void Reset()
    {
		//Stop any processes
		StopAllCoroutines();

		//Set all tools to inactive
		tools.transform.GetChild(0).gameObject.SetActive(false);
		tools.transform.GetChild(1).gameObject.SetActive(false);
		tools.transform.GetChild(2).gameObject.SetActive(false);
		tools.transform.GetChild(3).gameObject.SetActive(false);
		tools.transform.GetChild(4).gameObject.SetActive(false);
		tools.transform.GetChild(5).gameObject.SetActive(false);

		//Load intro
		LoadScene("Intro");
    } //end Reset

    /***************************************
     * Name: SetGameState
     * Sets the main game to the state given
     ***************************************/ 
	public void SetGameState(MainGameScene.MainGame newState)
    {
        //Disable text screen if active
        if (tools.transform.GetChild (1).gameObject.activeSelf)
        {
            tools.transform.GetChild (1).gameObject.SetActive(false);
        } //end if

		StartCoroutine(mainGame.SetGameState (newState));
    } //end SetGameState(MainGameScene.MainGame newState)
   
    /***************************************
     * Name: SummaryChange
     * Brings up the requested summary screen
     ***************************************/ 
    public void SummaryChange(int summaryPage)
    {
		if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainGame")
		{
			StartCoroutine(mainGame.SetSummaryPage(summaryPage));
		} //end if
		else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "PC")
		{
			StartCoroutine(pc.SetSummaryPage(summaryPage));
		} //end else if
		else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Battle")
		{
			StartCoroutine(battle.SetSummaryPage(summaryPage));
		} //end else if
    } //end SummaryChange(int summaryPage)

    /***************************************
     * Name: PartyState
     * Opens/Closes the Party in PC box
     ***************************************/ 
    public void PartyState(bool state)
    {
		StartCoroutine(pc.PartyState(state));
    } //end PartyState(bool state)

    /***************************************
     * Name: ToggleShown
     * Toggles Weakness/Resistance in Pokedex
     ***************************************/ 
    public void ToggleShown()
    {
		pokedex.ToggleShown ();
    } //end ToggleShown

	/***************************************
     * Name: ChangePocket
     * Moves to requested pocket
     ***************************************/
	public void ChangePocket(int requested)
	{
		sysm.PlayerTrainer.ChangePocket(requested);
		if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Inventory")
		{
			ChangeCheckpoint(1);
		} //end if
		else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Battle")
		{
			StartCoroutine(battle.FadeInBagPocket());
		} //end else if
		else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainGame")
		{
			StartCoroutine(mainGame.FadeInBagPocket());
		} //end else if
	} //end ChangePocket(int requested)

	/***************************************
	 * Name: SetupPickMove
	 * Sets up screen for player to pick a
	 * move to use
	 ***************************************/
	public void SetupPickMove()
	{
		if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Inventory")
		{
			inventory.SetupPickMove();
		} //end if
		else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Battle")
		{
			battle.SetupPickMove();
		} //end else if
	} //end SetupPickMove

	/***************************************
     * Name: ChangeFilter
     * Changes shop to display only items
     * within the filter
     ***************************************/ 
	public void ChangeFilter(int requested)
	{
		StartCoroutine(shop.ChangeFilter(requested));
	} //end ChangeFilter(int requested)

	/***************************************
     * Name: ItemsMode
     * Changes shop between buy and sell mode
     ***************************************/ 
	public void ItemMode()
	{
		StartCoroutine(shop.ItemMode());
	} //end ItemMode

	/***************************************
     * Name: PreviousPage
     * Changes shop's display to previous page
     ***************************************/ 
	public void PreviousPage()
	{
		shop.PreviousPage();
	} //end PreviousPage

	/***************************************
     * Name: NextPage
     * Changes shop's display to next page
     ***************************************/ 
	public void NextPage()
	{
		shop.NextPage();
	} //end NextPage

	/***************************************
     * Name: Codes
     * Allows player to input codes
     ***************************************/ 
	public void Codes()
	{
		StartCoroutine(shop.Codes());
	} //end Codes

	/***************************************
	 * Name: IncreaseQuantity
	 * Adds one to the purchase quantity
	 ***************************************/
	public void IncreaseQuantity()
	{
		shop.IncreaseQuantity();
	} //end IncreaseQuantity

	/***************************************
	 * Name: DecreaseQuantity
	 * Subtracts one from the purchase quantity
	 ***************************************/
	public void DecreaseQuantity()
	{
		shop.DecreaseQuantity();
	} //end DecreaseQuantity

	/***************************************
	 * Name: ConfirmPurchase
	 * Allows purchase and returns to 
	 * shop
	 ***************************************/
	public void ConfirmPurchase()
	{
		shop.ConfirmPurchase();
	} //end ConfirmPurchase

	/***************************************
	 * Name: CancelPurchase
	 * Cancels purchase and returns to 
	 * shop
	 ***************************************/
	public void CancelPurchase()
	{
		StartCoroutine(shop.CancelPurchase());
	} //end CancelPurchase

	/***************************************
	 * Name: CheckEffect
	 * Checks if an effect is in place on
	 * the field
	 ***************************************/
	public bool CheckEffect(int effect, int target)
	{
		return battle.CheckEffect(effect, target);
	} //end CheckEffect(int effect, int target)

	/***************************************
	 * Name: CheckMoveUser
	 * Returns the pokemon using the move
	 ***************************************/
	public PokemonBattler CheckMoveUser()
	{
		return battle.CheckMoveUser();
	} //end CheckMoveUser

	/***************************************
	 * Name: CheckMoveUsed
	 * Retrieves name of the attack used
	 ***************************************/
	public string CheckMoveUsed()
	{
		return battle.CheckMoveUsed();
	} //end CheckMoveUsed

	/***************************************
	 * Name: CheckField
	 * Retrieves integer of the active field
	 ***************************************/
	public int CheckField()
	{
		return battle.CheckField();
	} //end CheckField

	/***************************************
	 * Name: WriteBattleMessage
	 * Writes a message to the battle window
	 ***************************************/
	public void WriteBattleMessage(string message)
	{
		battle.WriteBattleMessage(message);
	} //end WriteBattleMessage(string message)

	/***************************************
	 * Name: SetupLearnMove
	 * Sets up screen for player allow a
	 * pokemon to learn a move
	 ***************************************/
	public void SetupLearnMove(List<int> toLearn, Pokemon leveledPokemon)
	{
		StartCoroutine(battle.SetupLearnMove(toLearn, leveledPokemon));
	} //end SetupLearnMove(List<int> toLearn, Pokemon leveledPokemon)

	/***************************************
	 * Name: QueueBattleItem
	 * Queues the use of a battle item
	 ***************************************/
	public bool QueueBattleItem(int item)
	{
		return battle.QueueBattleItem(item);
	} //end QueueBattleItem(int item)

	/***************************************
	 * Name: AdjustTargetHealth
	 * Allows adjustment of a battler for 
	 * effects outside combat (Leech Seed)
	 ***************************************/
	public void AdjustTargetHealth(int battlerTarget, int amount)
	{
		battle.AdjustTargetHealth(battlerTarget, amount);
	} //end AdjustTargetHealth(int battlerTarget, int amount)

	/***************************************
	 * Name: CheckForFaint
	 * Check if battler fainted
	 ***************************************/
	public bool CheckForFaint(int battler)
	{
		return battle.CheckForFaint(battler);
	} //end CheckForFaint(int battler)

	/***************************************
	 * Name: InitializeBattle
	 * Sets battle as a Single, Double, Triple
	 * or other type and the trainers 
	 ***************************************/
	public void InitializeBattle(int bType, List<Trainer> trainers)
	{
		int[] newMoves = new int[4];
		for(int i = 0; i < 4; i++)
		{
			newMoves[i] = trainers[1].Team[1].GetMove(i);
		} //end for
		battle.InitializeBattle(bType, trainers);
		sysm.PlayerTrainer.Team[0].ChangeMoves(new int[]{ 605 }, 3);
		LoadScene("Battle", true);
	} //end InitializeBattle(int bType)
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
     * Name: DisplayConfirm
     * Shows confirm box for player confirmation
     ***************************************/
	public void DisplayConfirm(string message, int start, bool close)
	{
		sysm.DisplayConfirm(message,start,close);
		sysm.confirm += confirmDel;
		confirmDisplayed = true;
	} //end DisplayConfirm(string message,int start,bool close)

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

	#region Animations
	/***************************************
	* Name: FadeInAnimation
	* Fades scene in (must be manually called
	* after a scene is loaded)
	***************************************/ 
	public void FadeInAnimation(int targetCheckpoint)
	{
		StartCoroutine(anim.FadeInAnimation(targetCheckpoint));
	} //end FadeInAnimation(int targetCheckpoint)

	/***************************************
	* Name: FadeInObjects
	* Fades in an array of objects
	***************************************/ 
	public void FadeInObjects(Image[] objects, int targetCheckpoint)
	{
		StartCoroutine(anim.FadeObjectIn(objects, targetCheckpoint));
	} //end FadeInObjects(Image[] objects, int targetCheckpoint)

	/***************************************
     * Name: OpenScene
     * Scales top and bottom images to 0 to
     * create a "shutter open" effect
     ***************************************/
	public void OpenScene(int targetCheckpoint)
	{
		StartCoroutine(anim.OpenScene(targetCheckpoint));
	} //end OpenScene(int targetCheckpoint)

	/***************************************
     * Name: ShowFoeParty
     * Plays animation for the foe's party to
     * appear
     ***************************************/
	public void ShowFoeParty()
	{
		anim.ShowFoeParty();
	} //end ShowFoeParty

	/***************************************
     * Name: HideFoeBox
     * Plays animation for the foe's pokemon
     * status box to disappear
     ***************************************/
	public void HideFoeBox()
	{
		GameObject.Find("FoeBox").GetComponent<Animator>().SetTrigger("HideBox");
	} //end HideFoeBox

	/***************************************
     * Name: ShowFoeBox
     * Plays animation for the foe's pokemon
     * status box to appear
     ***************************************/
	public void ShowFoeBox()
	{
		GameObject.Find("FoeBox").GetComponent<Animator>().SetTrigger("ShowBox");
	} //end ShowFoeBox

	/***************************************
     * Name: ShowPlayerBox
     * Plays animation for the player's 
     * pokemon box to appear
     ***************************************/
	public void ShowPlayerBox()
	{
		GameObject.Find("PlayerBox").GetComponent<Animator>().SetTrigger("ShowBox");
	} //end ShowPlayerBox

	/***************************************
	 * Name: IsProcessing
	 * Whether there is an animation in
	 * progress
	 ***************************************/
	public bool IsProcessing()
	{
		return anim.IsProcessing();
	} //end IsProcessing
	#endregion

    #region Debug
    /***************************************
     * Name: EditPokemonMode
     * Activates the pokemon edit panel
     ***************************************/ 
    public void EditPokemonMode()
    {
		mainGame.EditPokemonMode ();
    } //end EditPokemonMode

    /***************************************
     * Name: EditTrainerMode
     * Activates the trainer edit panel
     ***************************************/ 
    public void EditTrainerMode()
    {
		mainGame.EditTrainerMode ();
    } //end EditTrainerMode

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
		mainGame.RandomPokemon ();
    } //end RandomPokemon

    /***************************************
     * Name: EditPokemon
     * Grabs a pokemon out of team or pc
     * and populates debug with it
     ***************************************/ 
    public void EditPokemon()
    {
        mainGame.EditPokemon ();
    } //end EditPokemon

	/***************************************
     * Name: RemovePokemon
     * Remove a pokemon from PC or team
     ***************************************/ 
	public void RemovePokemon()
	{			
		mainGame.RemovePokemon ();
	} //end RemovePokemon

    /***************************************
     * Name: FinishEditing
     * Apply pokemon to requested spot
     ***************************************/ 
    public void FinishEditing()
    {
		mainGame.FinishEditing ();
    } //end FinishEditing

    /***************************************
     * Name: UpdateSprite
     * Changes sprite to reflect name choice
     ***************************************/ 
    public void UpdateSprite()
    {
		mainGame.UpdateSprite ();
    } //end UpdateSprite

	/***************************************
     * Name: FillInventory
     * Fills inventory with one of each item
     ***************************************/ 
	public void FillInventory()
	{	
		mainGame.FillInventory();
	} //end FillInventory
    #endregion
    #endregion
} //end GameManager class