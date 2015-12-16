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
	public void InitText(Text textArea)
	{
		sysm.GetText (textArea);
	} //end InitText()
	#endregion

} //end GameManager class
