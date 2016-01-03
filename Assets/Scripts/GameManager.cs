using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
	//SETTING VARIABLES
	public float VersionNumber = 0.1f;
	public int NumberOfMarkings = 4;
	public int NumberOfRibbons = 80;
    bool running = false;

	//Singleton handle
	public static GameManager instance = null;

	//SceneTools variables
	public GameObject pTool;				//Prefab of SceneTools
	public static GameObject tools = null;	//Canvas of SceneTools

	//Scene variables
	SceneManager scenes;					//Manages game scenes
	SystemManager sysm;						//Manages system features
    DataContents dataContents;

	// Use this for initialization
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

		//Get IntroMenu component
		scenes = GetComponent<SceneManager> ();

		//Get SystemManager component
		sysm = GetComponent<SystemManager> ();

        dataContents = GetComponent<DataContents> ();

		//Create error log
		sysm.InitErrorLog ();
	} //end Awake
	
	// Update is called once per frame
	void Update ()
	{
		//Try running game as normal
		try
		{
			//Reset
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
                if(!running)
                {
                    running = true;
                    System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
                    myStopwatch.Start();
                    sysm.StartINI(Environment.GetEnvironmentVariable ("USERPROFILE") + "/Saved Games/Pokemon Carousel/pokemon.txt");
                    /*for(int i = 0; i < 721; i++)
                    {
                        PokemonSpecies testSpecies = new PokemonSpecies();
                        string section = (i+1).ToString();
                        testSpecies.name = sysm.ReadINI<String>(section, "Name");
                        testSpecies.type1 = sysm.ReadINI<String>(section, "Type1");
                        testSpecies.type2 = sysm.ReadINI<String>(section, "Type2");
                        string holder = sysm.ReadINI<String>(section, "BaseStats");
                        if(holder != null && holder.Contains(","))
                        {
                            testSpecies.baseStats = Array.ConvertAll<string, int>(holder.Split(','), int.Parse);
                        } //end if
                        else
                        {
                            testSpecies.baseStats = new int[] {1, 1, 1, 1, 1, 1};
                        } //end else
                        testSpecies.genderRate = sysm.ReadINI<string>(section, "GenderRate");
                        testSpecies.growthRate = sysm.ReadINI<string>(section, "GrowthRate");
                        testSpecies.baseExp = sysm.ReadINI<int>(section, "BaseEXP");
                        holder = sysm.ReadINI<string>(section, "EffortPoints");
                        if(holder != null && holder.Contains(","))
                        {
                            testSpecies.effortPoints = Array.ConvertAll<string, int>(holder.Split(','), int.Parse);
                        } //end if
                        else
                        {
                            testSpecies.effortPoints = new int[] {1, 1, 1, 1, 1, 1};
                        } //end else
                        testSpecies.catchRate = sysm.ReadINI<int>(section, "Rareness");
                        testSpecies.happiness = sysm.ReadINI<int>(section, "Happiness");
                        holder = sysm.ReadINI<String>(section, "Abilities");  
                        if(holder != null && holder.Contains(","))
                        {
                            testSpecies.abilities = holder.Split(',');
                        } //end if
                        else
                        {
                            testSpecies.abilities = new string[] {holder};
                        } //end else
                        testSpecies.hiddenAbility = sysm.ReadINI<String>(section, "HiddenAbility");
                        holder = sysm.ReadINI<String>(section, "Moves");
                        testSpecies.moves = new Dictionary<int, List<string>>();
                        if(holder != null && holder.Contains(","))
                        {
                            String[] breaks = holder.Split(',');
                            for(int j = 0; j < breaks.Length; j+=2)
                            {
                                if(!testSpecies.moves.ContainsKey(int.Parse(breaks[j])))
                                {
                                    List<string> myList = new List<string>();
                                    myList.Add(breaks[j+1]);
                                    testSpecies.moves.Add(int.Parse(breaks[j]), myList);   
                                } //end if
                                else
                                {
                                    testSpecies.moves[int.Parse(breaks[j])].Add(breaks[j+1]);
                                } //end else
                            } //end for
                        } //end if
                        else
                        {
                            //Pokemon will only have Tackle, and it will be available immediately
                            List<string> myList = new List<string>();
                            myList.Add("TACKLE");
                            testSpecies.moves.Add(1, myList);   
                        } //end else
                        holder = sysm.ReadINI<string>(section, "EggMoves");
                        if(holder != null && holder.Contains(","))
                        {
                            testSpecies.eggMoves = holder.Split(',');
                        } //end if
                        else
                        {
                            testSpecies.eggMoves = new string[] {holder};
                        } //end else
                        holder = sysm.ReadINI<string>(section, "Compatibility");
                        if(holder != null && holder.Contains(","))
                        {
                            testSpecies.compatibility = holder.Split(',');
                        } //end if
                        else
                        {
                            testSpecies.compatibility = new string[] {holder};
                        } //end else
                        testSpecies.steps = sysm.ReadINI<int>(section, "StepsToHatch");
                        testSpecies.height = sysm.ReadINI<float>(section, "Height");
                        testSpecies.weight = sysm.ReadINI<float>(section, "Weight");
                        testSpecies.color = sysm.ReadINI<string>(section, "Color");
                        testSpecies.habitat = sysm.ReadINI<string>(section, "Habitat");
                        testSpecies.kind = sysm.ReadINI<string>(section, "Kind");
                        testSpecies.pokedex = sysm.ReadINI<string>(section, "Pokedex");
                        holder = sysm.ReadINI<string>(section, "Forms");
                        if(holder != null && holder.Contains(","))
                        {
                            testSpecies.forms = holder.Split(',');
                        } //end if
                        else
                        {
                            testSpecies.forms = new string[] {holder};
                        } //end else
                        testSpecies.battlerPlayerY = sysm.ReadINI<int>(section, "BattlerPlayerY");
                        testSpecies.battlerEnemyY = sysm.ReadINI<int>(section, "BattlerEnemyY");
                        testSpecies.battlerAltitude = sysm.ReadINI<int>(section, "BattlerAltitude");
                        dataContents.speciesData.Add(testSpecies);
                    } //end for
                    dataContents.Persist();
                    Debug.Log("Done saving data");*/
                    dataContents.GetPersist();
                    for(int i = 0; i < 5; i++)
                    {
                        int randomNum = Random.Range(0, 720);
                        Debug.Log("Name: " + dataContents.speciesData[randomNum].name);
                        Debug.Log("Height: " + dataContents.speciesData[randomNum].height);
                        Debug.Log("Pokedex: " + dataContents.speciesData[randomNum].pokedex);
                    }
                } //end if running
				//scenes.Intro();
			} //end if
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
		} //end try
		//Log error otherwise
		catch(System.Exception ex)
		{
			sysm.LogErrorMessage(ex.ToString());
		} //end catch(System.Exception ex)
	} //end Update

	//Menu functions
	#region Menu
	//Loads main game with current save file
	public void Continue()
	{
		scenes.Reset ();
		Application.LoadLevel ("Intro");
	} //end Continue

	//Starts a new game
	public void NewGame()
	{
		scenes.Reset ();
		Application.LoadLevel ("NewGame");
	} //end NewGame

	//Display game options
	public void Options()
	{
		scenes.Reset ();
		Application.LoadLevel ("Intro");
	} //end Options
	#endregion

	//System Manager functions
	#region SystemManager
	//Initializes text
	public void InitText(Transform textArea, Transform endArrow)
	{
		sysm.GetText (textArea.gameObject, endArrow.gameObject);
	} //end InitText(GameObject textArea, GameObject endArrow)

	//Displays a line of speech
	public bool DisplayText(string text)
	{
		return sysm.PlayText (text);
	} //end DisplayText(string text)

	//Returns if text has finished displaying
	public bool IsDisplaying()
	{
		return sysm.GetDisplay ();
	} //end IsDisplaying

	//Sets player's name
	public void SetName(string pName)
	{
		sysm.SetName(pName);
	} //end SetName(string pName)

	//Gets player's name
	public string GetPlayerName()
	{
		return sysm.GetPName ();
	} //end GetPlayerName

	//Sets player's badges
	public void SetBadges(int badges)
	{
		sysm.SetBadges (badges);
	} //end SetBadges(int badges)

	//Gets player's badges
	public int GetBadges()
	{
		return sysm.GetBadges ();
	} //end GetBadges

	//Gets player's hours
	public int GetHours()
	{
		return sysm.GetHours ();
	} //end GetHours

	//Get player's minutes
	public int GetMinutes()
	{
		return sysm.GetMinutes ();
	} //end GetMinutes

	//Creates persistant data
	public void Persist()
	{
		sysm.Persist ();
	} //end Persist

	//Gathers persistant data
	public bool GetPersist()
	{
		return sysm.GetPersist ();
	} //end GetPersist
	#endregion

} //end GameManager class
