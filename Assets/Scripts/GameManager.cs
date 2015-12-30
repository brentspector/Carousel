using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
	//SETTING VARIABLES
	public float VersionNumber = 0.1f;
	public int NumberOfMarkings = 4;
	public int NumberOfRibbons = 80;

	//Singleton handle
	public static GameManager instance = null;

	//SceneTools variables
	public GameObject pTool;				//Prefab of SceneTools
	public static GameObject tools = null;	//Canvas of SceneTools

	//Scene variables
	SceneManager scenes;					//Manages game scenes
	SystemManager sysm;						//Manages system features

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
