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
    DataContents dataContents;              //Holds pokemon data

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

        //Get DataContents component
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
                    /*System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
                    myStopwatch.Start();
                    sysm.GetContents(Environment.GetEnvironmentVariable ("USERPROFILE") + "/Saved Games/Pokemon Carousel/moves.txt");
                    for(int i = 0; i < 633; i++)
                    {
                        Move testMove = new Move();
                        string[] moveInfo = sysm.ReadCSV(i);
                        testMove.internalName = moveInfo[1];
                        testMove.gameName = moveInfo[2];
                        testMove.functionCode = int.Parse(moveInfo[3], System.Globalization.NumberStyles.HexNumber);
                        testMove.baseDamage = int.Parse(moveInfo[4]);
                        testMove.type = moveInfo[5];
                        testMove.category = moveInfo[6];
                        testMove.accuracy = int.Parse(moveInfo[7]);
                        testMove.totalPP = int.Parse(moveInfo[8]);
                        testMove.chanceEffect = int.Parse(moveInfo[9]);
                        testMove.target = int.Parse(moveInfo[10]);
                        testMove.priority = int.Parse(moveInfo[11]);
                        testMove.flags = moveInfo[12];
                        testMove.description = moveInfo[13];
                        for(int j = 14; j < moveInfo.Length; j++)
                            testMove.description += "," + moveInfo[j];
                        testMove.description = testMove.description.Replace("\"", "");
                        dataContents.moveData.Add(testMove);
                    } //end for
                    dataContents.PersistMoves();*/
                    /*System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
                    myStopwatch.Start();
                    sysm.GetContents(Environment.GetEnvironmentVariable ("USERPROFILE") + "/Saved Games/Pokemon Carousel/pokemon.txt");
                    for(int i = 0; i < 721; i++)
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
                        holder = sysm.ReadINI<string>(section, "FormNames");
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
                        holder = sysm.ReadINI<string>(section, "Evolutions");
                        testSpecies.evolutions = new List<Evolutions>();
                        if(holder != null && holder.Contains(","))
                        {
                            String[] breaks = holder.Split(',');
                            for(int j = 0; j < breaks.Length; j+=3)
                            {
                                Evolutions myEvo = new Evolutions();
                                myEvo.species = breaks[j];
                                myEvo.method = breaks[j+1];
                                try
                                {
                                    myEvo.trigger = breaks[j+2];
                                } //end try
                                catch(Exception)
                                {
                                    sysm.LogErrorMessage("Species " + i + " failed finding trigger for " + j);
                                } //end catch
                                testSpecies.evolutions.Add(myEvo);
                            } //end for
                        } //end if
                        dataContents.speciesData.Add(testSpecies);
                    } //end for
                    myStopwatch.Stop();
                    dataContents.PersistPokemon();
                    Debug.Log("Done saving data in " + myStopwatch.ElapsedMilliseconds);*/
                    /*sysm.GetContents(Environment.GetEnvironmentVariable ("USERPROFILE") + "/Saved Games/Pokemon Carousel/items.txt");
                    for(int i = 0; i < 525; i++)
                    {
                        Item testItem = new Item();
                        string[] itemInfo = sysm.ReadCSV(i);
                        testItem.internalName = itemInfo[1];
                        testItem.gameName = itemInfo[2];
                        testItem.bagNumber = int.Parse(itemInfo[3]);
                        testItem.cost = int.Parse(itemInfo[4]);
                        testItem.description = itemInfo[5];
                        int j = 5;
                        if(itemInfo[j].Contains("\""))
                        {
                            while(!itemInfo[j].EndsWith("\""))
                            {
                                j++;
                                Debug.Log("J is " + j + " and I is " + i);
                                testItem.description += "," + itemInfo[j];
                            } 
                            testItem.description = testItem.description.Replace("\"", "");
                        }
                        j+=2;
                        try
                        {                        
                            testItem.battleUse = int.Parse(itemInfo[j]);
                        }
                        catch(SystemException e)
                        {
                            sysm.LogErrorMessage("Game Name: " + itemInfo[2] + " gives " + itemInfo[j] + " at J of " + j);
                        }
                        dataContents.itemData.Add(testItem);
                    }
                    dataContents.PersistItems();
                    Debug.Log("Done saving");

                    dataContents.itemData[69].description = "A long, thin, bright-red string to be held by a Pokémon. If the holder becomes infatuated, the foe does too.";
                    dataContents.PersistItems();
                    int randomNumber = 69;
                    Debug.Log(dataContents.itemData[randomNumber].internalName + 
                              "\n" + dataContents.itemData[randomNumber].gameName + "\n" +
                              dataContents.itemData[randomNumber].bagNumber + "\n" +
                              dataContents.itemData[randomNumber].cost + "\n" +
                              dataContents.itemData[randomNumber].description + "\n" +
                              dataContents.itemData[randomNumber].battleUse);*/
                    dataContents.GetPersist();
#if UNITY_EDITOR


#else



#endif
                } //end if running
				scenes.Intro();
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
                               ex.ToString() + ", additionally the following " +
                                "occurred:\n" + exc.ToString());
            } //end catch
		} //end catch(System.Exception ex)
	} //end Update

	//Menu functions
	#region Menu
	//Loads main game with current save file
	public void Continue()
	{
		scenes.Reset ();
		Application.LoadLevel ("MainGame");
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

    public void Jump()
    {
        scenes.JumpTo ();
    } //end Jump
	#endregion

	//System Manager functions
	#region SystemManager
    //Log error message
    public void LogErrorMessage(string message)
    {
        sysm.LogErrorMessage (message);
    } //end LogErrorMessage(string message)

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

    //Sends dataContents
    public DataContents GetDataContents()
    {
        return dataContents;
    } //end GetDataContents
	#endregion
} //end GameManager class
