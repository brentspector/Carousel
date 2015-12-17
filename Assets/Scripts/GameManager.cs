using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	//Singleton Variable
	public static GameManager instance = null;

	//Scene variables
	IntroMenu intro;		//Manages intro and menu scenes
	SystemManager sysm;		//Manages system features

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

		//Get IntroMenu component
		intro = GetComponent<IntroMenu> ();

		//Get SystemManager component
		sysm = GetComponent<SystemManager> ();
	} //end Awake
	
	// Update is called once per frame
	void Update ()
	{
		//Intro scene
		if(Application.loadedLevelName == "Intro")
		{
			intro.Intro();
		} //end if
		//Start Menu scene
		else if(Application.loadedLevelName == "StartMenu")
		{
			intro.Menu();
		} //end else if	
		//New Game scene
		else if(Application.loadedLevelName == "NewGame")
		{
			intro.NewGame();
		} //end else if
	} //end Update

	//System Manager functions
	#region SystemManager
	//Initializes text
	public void InitText(Text textArea, GameObject endArrow)
	{
		sysm.GetText (textArea, endArrow);
	} //end InitText(Text textArea, GameObject endArrow)

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
